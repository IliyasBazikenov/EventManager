using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Controllers
{
    [Route("api/accounts/{accountId}/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private IRepositoryManager _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public EventsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventsForAccount(Guid accountId)
        {
            var account = await _repository.Account.GetAccountAsync(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }
            var accountEvents = await _repository.Event.GetEventsAsync(accountId, trackChanges: false);
            var accountDTO = _mapper.Map<IEnumerable<EventDTO>>(accountEvents);
            return Ok(accountDTO);
        }

        [HttpGet("{eventId}", Name = "GetEventForAccount")]
        public async Task<IActionResult> GetEventForAccount(Guid accountId, int eventId)
        {
            var account = await _repository.Account.GetAccountAsync(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            var accountEvent = await _repository.Event.GetEventAsync(accountId, eventId, trackChanges: false);
            if (accountEvent == null)
            {
                _logger.LogInfo($"Event with id: {eventId} doesn't exist in the databse.");
                return NotFound();
            }
            var accountDTO = _mapper.Map<EventDTO>(accountEvent);
            return Ok(accountDTO);
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEvent(Guid accountId, int eventId)
        {
            var account = await _repository.Account.GetAccountAsync(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            var accountEvent = await _repository.Event.GetEventAsync(accountId, eventId, trackChanges: false);
            if (accountEvent == null)
            {
                _logger.LogInfo($"Event with id: {eventId} doesn't exist in the databse.");
                return NotFound();
            }
            _repository.Event.DeleteEvent(accountEvent);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(Guid accountId, [FromBody] EventForCreationDTO eventDTO)
        {
            if (eventDTO == null)
            {
                _logger.LogInfo($"EventDTO sent from client is null.");
                return BadRequest("EventDTO is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EventForCreationDTO.");
                return UnprocessableEntity(ModelState);
            }

            var account = await _repository.Account.GetAccountAsync(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            var eventEntity = _mapper.Map<Event>(eventDTO);
            _repository.Event.CreateEvent(accountId, eventEntity);
            await _repository.SaveAsync();

            var eventToReturn = _mapper.Map<EventDTO>(eventEntity);

            return CreatedAtRoute("GetEventForAccount", new { accountId, eventId = eventToReturn.EventId }, eventToReturn);
        }

        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEventForAccount(Guid accountId, int eventId, [FromBody] EventForUpdateDTO eventDTO)
        {
            if (eventDTO == null)
            {
                _logger.LogError($"EventForUpdateDTO object sent from client is null.");
                return BadRequest("ventForUpdateDTO object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EventForUpdateDTO.");
                return UnprocessableEntity(ModelState);
            }

            var account = await _repository.Account.GetAccountAsync(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            var eventEntity = await _repository.Event.GetEventAsync(accountId, eventId, trackChanges: true);
            if (eventEntity == null)
            {
                _logger.LogInfo($"Event with id: {eventId} doesn't exist in the database.");
                return NotFound();
            }

            _mapper.Map(eventDTO, eventEntity);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{eventId}")]
        public async Task<IActionResult> PartiallyUpdateEvent(Guid accountId, int eventId,
            [FromBody]JsonPatchDocument<EventForUpdateDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                _logger.LogError("Event patchDocument object sent from client is null.");
                return BadRequest("patchDocument object is null");
            }

            var account = await _repository.Account.GetAccountAsync(accountId, false);
            if(account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the database.");
                return NoContent();
            }

            var eventEntity = await _repository.Event.GetEventAsync(accountId, eventId, true);
            if(eventEntity == null)
            {
                _logger.LogInfo($"Event with id: {eventId} doesn't exist in the database.");
            }

            var eventToPatch = _mapper.Map<EventForUpdateDTO>(eventEntity);
            patchDocument.ApplyTo(eventToPatch);

            TryValidateModel(eventToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EventForUpdateDTO.");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(eventToPatch, eventEntity);
            await _repository.SaveAsync();

            return NoContent();
        }
    }
}