using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocNetworkApp.API.Data;
using SocNetworkApp.API.Dtos;
using SocNetworkApp.API.Filters;
using SocNetworkApp.API.Helpers;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Controllers
{
    
    [ServiceFilter(typeof(UserLogActivityFilter))]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IDataRepository _repository;
        private readonly IMapper _mapper;

        public MessagesController(IDataRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(Guid userId, Guid id)
        { 
            if (userId != Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            {
                return Unauthorized();
            }

            Message message = await _repository.GetMessage(id);

            if (message == null) 
            {
                return NotFound();
            }

            return Ok(message);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(Guid userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;

            PagedList<Message> pagedList = await _repository.GetMessagesForUser(messageParams);
            IEnumerable<MessageReturnDto> messages = _mapper.Map<IEnumerable<MessageReturnDto>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize, pagedList.TotalPages, pagedList.TotalCount);

            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(Guid userId, MessageCreationDto messageCreationDto)
        {
            User sender = await _repository.GetUser(userId);

            if (sender?.Id != Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            {
                return Unauthorized();
            }

            messageCreationDto.SenderId = userId;

            User recipient = await _repository.GetUser(messageCreationDto.RecipientId);

            if (recipient == null)
            {
                return BadRequest("Could not find user");
            }

            Message message = _mapper.Map<Message>(messageCreationDto);
            _repository.Add(message);       

            if (await _repository.SaveAll())
            {
                MessageReturnDto messageReturnDto = _mapper.Map<MessageReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageReturnDto);
            }

            throw new Exception("Creating the message failed on save");
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(Guid userId, Guid recipientId)
        {
            if (userId != Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            {
                return Unauthorized();
            }

            IEnumerable<Message> messages = await _repository.GetMessageThread(userId, recipientId);

            IEnumerable<MessageReturnDto> messageThread = _mapper.Map<IEnumerable<MessageReturnDto>>(messages);

            return Ok(messageThread);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id, Guid userId)
        {
            if (userId != Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            {
                return Unauthorized();
            }

            Message message = await _repository.GetMessage(id);

            if (message.SenderId == userId)
            {
                message.SenderDeleted = true;
            }

            if (message.RecipientId == userId)
            {
                message.RecipientDeleted = true;
            }

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _repository.Delete(message);
            }

            if (await _repository.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("Error deleting the message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(Guid userId, Guid id)
        {
            if (userId != Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            {
                return Unauthorized();
            }

            Message message = await _repository.GetMessage(id);

            if (message.RecipientId != userId)
            {
                return Unauthorized();
            }

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await _repository.SaveAll();

            return NoContent();
        }
    }
}