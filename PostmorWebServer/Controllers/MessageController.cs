using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostmorWebServer.Contracts;
using PostmorWebServer.Contracts.Requests;
using PostmorWebServer.Contracts.Responses;
using PostmorWebServer.Models;
using PostmorWebServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;

        }
        [HttpPost(ApiRoutes.Messages.Send)]
        public async Task<IActionResult> Send([FromBody]MessageSendRequest request, [FromHeader] string authorization)
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
            if (request.Message == null || request.Type == null || request.ContactId == 0 )
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Null value detected" }
                });

            }
            var token = authorization.Substring("Bearer ".Length).Trim();
            var Message = await _messageService.SendAsync(request.Message, request.Type, token, request.ContactId);
            if (Message.Item1 == 0)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new string[] { "Something went wrong" }
                });
            }
            return Ok(new MessageSendResponse { MessageId = Message.Item1, Timestamp = Message.Item2 });

        }

        [HttpPost(ApiRoutes.Messages.FetchNew)]
        public async Task<IActionResult> FetchNew([FromBody]MessageFetchNewRequest request, [FromHeader] string authorization)
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
            var unSendLetters = await _messageService.FetchNewAsync(token, request.LatestMessageId);
            var response = new MessageFetchNewResponse();
            response.Messages = new List<Message>();
            if (unSendLetters!=null)
            {
                foreach (var Letter in unSendLetters)
                {
                    response.Messages.Add(new Message
                    {
                        MessageId = Letter.MessageId,
                        SenderId = Letter.SenderId,
                        DeliveryTime = Letter.DeliveryTime,
                        Content = Letter.Content,
                        Type = Letter.Type,
                        Timestamp = Letter.Timestamp
                    });
                }
            }
            
            return Ok(response);
            }
        
    }
}
