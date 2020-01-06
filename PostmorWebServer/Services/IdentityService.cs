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

namespace PostmorWebServer.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _dataContext;

        public IdentityService(DataContext dataContext, UserManager<User> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters)
        {
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
            string adresspart="";
            for (int i = 0; i < splitaddress.Length-1; i++)
            {
                adresspart = string.Concat(adresspart, splitaddress[i]);
            }


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
                PickupTime = "09:00",
                SendTime = "17:00",
                Streetnumber = splitaddress[splitaddress.Length -1],
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
                    ReceiverID = entry.RetrieverId,
                    Content = entry.Message,
                    Type = entry.Type,
                    DeliveryTime = entry.ReceivedTime
                });
                if (entry.RetrieverId == requesterID && !IDs.Contains(entry.SenderId))
                {
                    interlocutor.Add(new UserCard
                    {
                        ContactId = entry.SenderId,
                        ContactName = entry.Sender.Name,
                        ContactAddress = entry.Sender.Address,
                        Picture = entry.Sender.ProfilePic,
                        PublicKey = entry.Sender.PublicKey,
                        IsFriend = requester.Contacts.Contains(entry.Sender),
                        Success = true
                    }) ;
                    IDs.Add(entry.SenderId);
                }
                if (entry.SenderId == requesterID && !IDs.Contains(entry.RetrieverId))
                {
                    interlocutor.Add(new UserCard
                    {
                        ContactId = entry.RetrieverId,
                        ContactName = entry.Retriver.Name,
                        ContactAddress = entry.Retriver.Address,
                        Picture = entry.Retriver.ProfilePic,
                        PublicKey = entry.Retriver.PublicKey,
                        IsFriend = requester.Contacts.Contains(entry.Retriver),
                        Success = true
                    });
                    IDs.Add(entry.SenderId);
                }


            }
            var requesterCard = new RequesterUserCard
            {
              RequesterId = requesterID,
              RequesterName = requester.Name,
              RequesterAddress = requester.Address,
              RequesterPrivateKey = requester.PrivateKey,
              RequesterPublicKey = requester.PublicKey,
              RequesterPickupTime = requester.PickupTime,
              RequesterDeliveryTime = requester.SendTime
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
    }
}
