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
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });

            }
            var authRespons = await _identityService.RegisterAsyc(request.Email, request.Password, request.Name, request.Adress, request.Picture);
            if (!authRespons.Succes)
            {
                return BadRequest(new AuthFailedResponse
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


        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody]UserRegistrationRequest request)
        {
            if (request == null)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = new string[] {"Http request is empty"}
                });
            }
            var authRespons = await _identityService.LoginAsyc(request.Email, request.Password);
            if (!authRespons.Succes)
            {
                return BadRequest(new AuthFailedResponse
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
            var authRespons = await _identityService.RefreshTokenAsyc(request.Token, request.RefreshToken);
            if (!authRespons.Succes)
            {
                return BadRequest(new AuthFailedResponse
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
    }

}
