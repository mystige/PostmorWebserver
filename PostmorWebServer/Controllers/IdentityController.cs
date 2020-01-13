using PostmorWebServer.Contracts;
using PostmorWebServer.Contracts.Requests;
using PostmorWebServer.Contracts.Responses;
using PostmorWebServer.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IO;

namespace PostmorWebServer.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IContactService _contactService;
        private readonly IMessageService _messageService;
        public IdentityController(IIdentityService identityService, IContactService contactService, IMessageService messageService)
        {
            _messageService = messageService;
            _contactService = contactService;
            _identityService = identityService;
        }
        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody]UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });

            }
            if (request.Email == null || request.Address == null || request.Password == null || request.Name == null)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] {"Null value detected"}
                });

            }
            
            var authRespons = await _identityService.RegisterAsync(request.Email, request.Password, request.Name, request.Address, request.Picture);
            if (!authRespons.Succes)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authRespons.Errors
                });
            }

            var newuserResponse =await _identityService.GenerateUserRegisterResponseAsync(authRespons.UserID);

            if (!newuserResponse.Succes)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = newuserResponse.Error
                });
            }

            return Ok(new RegisterSuccessRespones
            {
                Id = authRespons.UserID,
                DeliveryTime = newuserResponse.DeliveryTime,
                PickupTime = newuserResponse.PickupTime,
                PrivateKey = newuserResponse.PrivateKey,
                PublicKey = newuserResponse.PubliciKey,
                Token = authRespons.Token,
                RefreshToken = authRespons.RefreshToken
            });
        }



        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody]UserLoginRequest request)
        {
            if (request == null)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] {"Http request is empty"}
                });
            }
            if (request.Email == null || request.Password == null)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Http request has bad formating/missing information" }
                });
            }
            
            var authRespons = await _identityService.LoginAsync(request.Email, request.Password);
            if (!authRespons.Succes)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authRespons.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authRespons.Token,
                RefreshToken = authRespons.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authRespons = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
            if (!authRespons.Succes)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authRespons.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authRespons.Token,
                RefreshToken = authRespons.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.GenerateAdresses)]
        public async Task<IActionResult> GenerateAdresses([FromBody] GenerateAdressesRequest request)
        {
            int amount;
            if (request.Amount == 0)
            {
                amount = 0;
            }
            amount = request.Amount;
            var result = await _identityService.GenerateAddresses(amount);
            return Ok(new GenerateAddressesResponse
            {
                Addresses = result.ToArray()
                
            });
        }

        [HttpPost(ApiRoutes.Identity.FetchAllData)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]        
        public async Task<IActionResult> FetchAllData([FromHeader] string authorization)
        {
            var token = authorization.Substring("Bearer ".Length).Trim();
            var response = await _identityService.FetchAllAsync(token);
            if (!response.Success )
            {
                return BadRequest(new FailedResponse
                {                  
                    Errors = new string[] { "Casper, jag har ingen anning vad som blev fel men det är nått som inte funkar :D", response.Error }
                });
            }
            return Ok(new UserFetchAllDataResponse
            {
                Contacts = response.Contacts,
                Messages = response.Messages,
                Userdata = response.RequesterUserCard
            });
        }
    }

}
