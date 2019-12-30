using PostmorWebServer.Contracts;
using PostmorWebServer.Contracts.Requests;
using PostmorWebServer.Contracts.Responses;
using PostmorWebServer.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        public IdentityController(IIdentityService identityService)
        {
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
            if (request.Email == null || request.Adress == null || request.Password == null || request.Name == null)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] {"Null value detected"}
                });

            }
            
            var authRespons = await _identityService.RegisterAsync(request.Email, request.Password, request.Name, request.Adress, request.Picture);
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
            return Ok(new GenerateAdressesResponse
            {
                Adresses = new[] {"Katpissgatan 32",
                    "Casperärmogen gatan 69",
                    "Alkisgatan 12",
                    "Detärkalltisibiren 32",
                    "Gruvgrävarvägen 14",
                    "EyNick gränden 2"
                }
            });
        }


    }

}
