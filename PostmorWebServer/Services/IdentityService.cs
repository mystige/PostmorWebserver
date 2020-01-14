using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using PostmorWebServer.Data;
using PostmorWebServer.Domain;
using PostmorWebServer.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PostmorWebServer.Options;
using PostmorWebServer.Models;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;

namespace PostmorWebServer.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _dataContext;
        private readonly IHostingEnvironment _hostingEnvironment;

        public IdentityService(DataContext dataContext, IHostingEnvironment environment, UserManager<User> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters)
        {
            _hostingEnvironment = environment;
            _dataContext = dataContext;
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exists" }
                };
            }

            var UserHasVaildPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!UserHasVaildPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User/Password is wrong" }
                };
            }
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Invaild token" } };
            }
            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = await _dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
            }
            if (DateTime.UtcNow > storedRefreshToken.Expirydate)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }
            if (storedRefreshToken.Invaildated)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invaildated" } };
            }
            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            }
            if (storedRefreshToken.JwtID != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }
            storedRefreshToken.Used = true;
            _dataContext.RefreshTokens.Update(storedRefreshToken);
            await _dataContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var tokenVaildatioParameters = _tokenValidationParameters.Clone();
                tokenVaildatioParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtwithVaildSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
        private bool IsJwtwithVaildSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password, string name, string address, string picture)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this email adress already exists" }
                };
            }

            var existingAdress = await _dataContext.Users.SingleOrDefaultAsync(x => x.Address == address);
            if (existingAdress != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this adress already exists" }
                };
            }
            string[] splitaddress = address.Split(' ');
            string adresspart = "";
            for (int i = 0; i < splitaddress.Length - 2; i++)
            {
                adresspart = string.Concat(adresspart, splitaddress[i]);
            }
            var streetnumber = string.Concat (splitaddress[splitaddress.Length - 2], " ", splitaddress[splitaddress.Length - 1]);


            var newUser = new User
            {
                UserName = email,
                Email = email,
                Name = name,
                Address = adresspart,
                ProfilePic = picture,
                PrivateKey = "hej",
                PublicKey = "hej",
                ActiveUser = true,
                PickupTime = GenerateTimeString(true),
                SendTime = GenerateTimeString(false),
                Streetnumber = streetnumber,
                Contacts = new List<User>()
            };
            var createdUser = await _userManager.CreateAsync(newUser, password);
            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }
            return await GenerateAuthenticationResultForUserAsync(newUser);

        }
        private string GenerateTimeString(bool IsPickuptime)
        {
            Random rnd = new Random();
            var temp = (DateTime.Now.Hour + rnd.Next(0, 23)) % 4;
            if (IsPickuptime)
            {
                temp += 15;
                return temp.ToString() + ":00";
            }
            temp += 9;
            return temp.ToString() + ":00";
            

        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id.ToString(), "int")

                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtID = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                Expirydate = DateTime.UtcNow.AddMonths(6)
            };

            await _dataContext.RefreshTokens.AddAsync(refreshToken);
            await _dataContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                UserID = user.Id,
                Succes = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<RegisterResult> GenerateUserRegisterResponseAsync(int Id)
        {
            var user = await _userManager.FindByIdAsync((Id.ToString()));
            if (user == null)
            {
                return new RegisterResult
                {
                    Error = new[] { "Something went wrong while fetching user" }
                };
            }
            return new RegisterResult
            {
                Succes = true,
                PickupTime = user.PickupTime,
                DeliveryTime = user.SendTime,
                PrivateKey = user.PrivateKey,
                PubliciKey = user.PublicKey
            };
        }

        public async Task<FetchAllResult> FetchAllAsync(string token)
        {
            var messages = new List<Message>();
            var interlocutor = new List<UserCard>();
            var IDs = new HashSet<int>();
            int requesterID = ExtractIdFromJwtToken(token);
            var requester = await _dataContext.Users
                .Include(x => x.Contacts)
                .FirstOrDefaultAsync(x => x.Id == requesterID);

            var entries = await _dataContext.Letters
                .Include(x => x.Sender)
                .Include(x => x.Retriver)
                .Where(x => x.RetrieverId == requesterID || x.SenderId == requesterID).ToListAsync();
            foreach (var entry in entries)
            {
                messages.Add(new Message
                {
                    MessageId = entry.Id,
                    SenderId = entry.SenderId,
                    ReceiverId = entry.RetrieverId,
                    Content = entry.Message,
                    Type = entry.Type,
                    DeliveryTime = entry.ReceivedTime.ToString("dd-MM-yyyy hh:mm"),
                    Timestamp = entry.ReceivedTime.ToString("dd-MM-yyyy hh:mm")
                });
                if (entry.RetrieverId == requesterID && !IDs.Contains(entry.SenderId))
                {
                    interlocutor.Add(new UserCard
                    {
                        ContactId = entry.SenderId,
                        ContactName = entry.Sender.Name,
                        ContactAddress = entry.Sender.Address + " " + entry.Sender.Streetnumber,
                        Picture = entry.Sender.ProfilePic,
                        PublicKey = entry.Sender.PublicKey,
                        IsFriend = requester.Contacts.Contains(entry.Sender),
                        Success = true
                    });
                    IDs.Add(entry.SenderId);
                }
                if (entry.SenderId == requesterID && !IDs.Contains(entry.RetrieverId))
                {
                    interlocutor.Add(new UserCard
                    {
                        ContactId = entry.RetrieverId,
                        ContactName = entry.Retriver.Name,
                        ContactAddress = entry.Retriver.Address + " " + entry.Retriver.Streetnumber,
                        Picture = entry.Retriver.ProfilePic,
                        PublicKey = entry.Retriver.PublicKey,
                        IsFriend = requester.Contacts.Contains(entry.Retriver),
                        Success = true
                    });
                    IDs.Add(entry.RetrieverId);
                }


            }

            foreach (var contact in requester.Contacts)
            {
                if (!IDs.Contains(contact.Id))
                {
                    interlocutor.Add(new UserCard
                    {
                        ContactId = contact.Id,
                        ContactName = contact.Name,
                        ContactAddress = contact.Address + " " + contact.Streetnumber,
                        Picture = contact.ProfilePic,
                        PublicKey = contact.PublicKey,
                        IsFriend = true,
                        Success = true
                    });
                    IDs.Add(contact.Id);
                }
            }


            var requesterCard = new RequesterUserCard
            {
                Id = requesterID,
                Name = requester.Name,
                Address = requester.Address + " " + requester.Streetnumber,
                PrivateKey = requester.PrivateKey,
                PublicKey = requester.PublicKey,
                PickupTime = requester.PickupTime,
                DeliveryTime = requester.SendTime,
                Picture = requester.ProfilePic,
                Success = true
                
                
                
            };
            return (new FetchAllResult
            {
                Contacts = interlocutor,
                Messages = messages,
                RequesterUserCard = requesterCard,
                Success = true
            }); ;

        }
        private int ExtractIdFromJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var id = tokenS.Claims.First(claim => claim.Type == "id").Value;
            return Convert.ToInt32(id);
        }

        //Generates addresses
        public async Task<List<string>> GenerateAddresses(int amount)
        {
            var words = new List<string>();
            //Fetches all addresses in use
            var usedAddresses = await _dataContext.Users.Select(x => x.Address).ToListAsync(); ;
            string[] endings = { "Street", "Road", "Avenue", "Row", "Square" };
            
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "most-common-nouns-english.csv");

            //reads word from file
            using (var reader = new StreamReader(path))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    words.Add(line);
                }
            }

            List<Tuple<int, int>> gennumb = new List<Tuple<int,int>>();
            for (int i = 0; i < amount; i++)
            {
                gennumb.Add(NumberGen());
            }
            var FinalAddresses = new List<string>();
            foreach (var pair in gennumb)
            {
                StringBuilder address = new StringBuilder();
                address.Append(words[pair.Item1]);
                address.Append(words[pair.Item2]);
                while (usedAddresses.Contains(address.ToString()))
                {
                    address.Clear();
                    var newNumber = NumberGen();
                    address.Append(words[newNumber.Item1]);
                    address.Append(words[newNumber.Item2]);
                }
                Random rnd = new Random();
                int end = rnd.Next(0, 5);
                address.Append(" ");
                address.Append(endings[end]);
                address.Append(" ");
                address.Append(rnd.Next(10, 40));
                FinalAddresses.Add(System.Globalization
                    .CultureInfo.CurrentCulture.TextInfo
                    .ToTitleCase(address.ToString().ToLower()));
                
            }
            return FinalAddresses;
       }
        private Tuple<int,int> NumberGen()
        {
            Random rnd = new Random();
            int x = rnd.Next(0,987);
            int y = rnd.Next(0,987);
            while (x == y)
            {
                x = rnd.Next(0, 987);
                y = rnd.Next(0, 987);
            }
            return new Tuple<int, int>(x,y);
        }
    }
}
