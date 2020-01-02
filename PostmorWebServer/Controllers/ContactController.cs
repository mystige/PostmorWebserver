using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostmorWebServer.Contracts;
using PostmorWebServer.Contracts.Requests;
using PostmorWebServer.Contracts.Responses;
using PostmorWebServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }
        [HttpPost(ApiRoutes.Contacts.Add)]
        public async Task<IActionResult> Add([FromBody]ContactAddRequest request, [FromHeader] string authorization)
        {
            if (request == null)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Http request is empty" }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Http request has faulty modelstate" }
                });
            }
            var token = authorization.Substring("Bearer ".Length).Trim();
            var IsSuccssesfull = await _contactService.AddAsync(token, request.ContactId);

            if (IsSuccssesfull)
            {
                return Ok(new ContactAddResponse { Success = true });
            }
            else
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Error adding contact" }
                });
            }
        }

        [HttpPost(ApiRoutes.Contacts.Get)]
        public async Task<IActionResult> Get([FromBody]ContactGetRequest request, [FromHeader] string authorization)
        {
            if (request == null)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Http request is empty" }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Http request has faulty modelstate" }
                });
            }
            var token = authorization.Substring("Bearer ".Length).Trim();
            var requestUserCard = await _contactService.FindUserByIdAsync(token, request.ContactId);

            if (!requestUserCard.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { requestUserCard.Error }
                });
            }
            else
            {
                return Ok(new ContactGetResponse
                {
                    ContactId = requestUserCard.ContactId,
                    ContactName = requestUserCard.ContactName,
                    Address = requestUserCard.ContactAddress,
                    IsFriend = requestUserCard.IsFriend,
                    Picture = requestUserCard.Picture,
                    PublicKey = requestUserCard.PublicKey
                });
            }
        }

        [HttpPost(ApiRoutes.Contacts.Search)]
        public async Task<IActionResult> Search([FromBody]ContactSearchRequest request, [FromHeader] string authorization)
        {
            if (request == null)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Http request is empty" }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Http request has faulty modelstate" }
                });
            }
            var token = authorization.Substring("Bearer ".Length).Trim();
            var requestUserCard = await _contactService.FindUserByAddressAsync(token, request.Adress);

            if (!requestUserCard.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { requestUserCard.Error }
                });
            }
            else
            {
                return Ok(new ContactGetResponse
                {
                    ContactId = requestUserCard.ContactId,
                    ContactName = requestUserCard.ContactName,
                    Address = requestUserCard.ContactAddress,
                    IsFriend = requestUserCard.IsFriend,
                    Picture = requestUserCard.Picture,
                    PublicKey = requestUserCard.PublicKey
                });
            }



        }

        [HttpPost(ApiRoutes.Contacts.Remove)]
        public async Task<IActionResult> Remove([FromBody]ContactRemoveRequest request, [FromHeader] string authorization)
        {
            if (request == null)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Http request is empty" }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Http request has faulty modelstate" }
                });
            }
            var token = authorization.Substring("Bearer ".Length).Trim();
            return default;
        }
    }
}
