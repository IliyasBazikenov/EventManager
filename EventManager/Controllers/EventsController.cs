using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
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
        public IActionResult GetEventsForAccount(Guid accountId)
        {
            var account = _repository.Account.GetAccount(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }
            var accountEvents = _repository.Event.GetEvents(accountId, trackChanges: false);
            var accountDTO = _mapper.Map<IEnumerable<EventDTO>>(accountEvents);
            return Ok(accountDTO);
        }

        [HttpGet("{eventId}", Name = "GetEventForAccount")]
        public IActionResult GetEventForAccount(Guid accountId, int eventId)
        {
            var account = _repository.Account.GetAccount(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            var accountEvent = _repository.Event.GetEvent(accountId, eventId, trackChanges: false);
            if (accountEvent == null)
            {
                _logger.LogInfo($"Event with id: {eventId} doesn't exist in the databse.");
                return NotFound();
            }
            var accountDTO = _mapper.Map<EventDTO>(accountEvent);
            return Ok(accountDTO);
        }

        [HttpDelete("{eventId}")]
        public IActionResult DeleteEvent(Guid accountId, int eventId)
        {
            var account = _repository.Account.GetAccount(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            var accountEvent = _repository.Event.GetEvent(accountId, eventId, trackChanges: false);
            if (accountEvent == null)
            {
                _logger.LogInfo($"Event with id: {eventId} doesn't exist in the databse.");
                return NotFound();
            }
            _repository.Event.DeleteEvent(accountEvent);
            _repository.Save();

            return NoContent();
        }

        [HttpPost]
        public IActionResult CreateEvent(Guid accountId, [FromBody] EventForCreationDTO eventDTO)
        {
            if (eventDTO == null)
            {
                _logger.LogInfo($"EventDTO sent from client is null.");
                return BadRequest("EventDTO is null");
            }

            var account = _repository.Account.GetAccount(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            var eventEntity = _mapper.Map<Event>(eventDTO);
            _repository.Event.CreateEvent(accountId, eventEntity);
            _repository.Save();

            var eventToReturn = _mapper.Map<EventDTO>(eventEntity);

            return CreatedAtRoute("GetEventForAccount", new { accountId, eventId = eventToReturn.EventId }, eventToReturn);
        }

        [HttpPut("{eventId}")]
        public IActionResult UpdateEventForAccount(Guid accountId, int eventId, [FromBody] EventForUpdateDTO eventDTO)
        {
            if (eventDTO == null)
            {
                _logger.LogError($"EventForUpdateDTO object sent from client is null.");
                return BadRequest("ventForUpdateDTO object is null");
            }

            var account = _repository.Account.GetAccount(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            var eventEntity = _repository.Event.GetEvent(accountId, eventId, trackChanges: true);
            if (eventEntity == null)
            {
                _logger.LogInfo($"Event with id: {eventId} doesn't exist in the database.");
                return NotFound();
            }

            _mapper.Map(eventDTO, eventEntity);
            _repository.Save();
            return NoContent();
        }

        [HttpPatch("{eventId}")]
        public IActionResult PartiallyUpdateEvent(Guid accountId, int eventId,
            [FromBody]JsonPatchDocument<EventForUpdateDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                _logger.LogError("patchDocument object sent from client is null.");
                return BadRequest("patchDocument object is null");
            }

            var account = _repository.Account.GetAccount(accountId, false);
            if(account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the database.");
                return NoContent();
            }

            var eventEntity = _repository.Event.GetEvent(accountId, eventId, true);
            if(eventEntity == null)
            {
                _logger.LogInfo($"Employee with id: {eventId} doesn't exist in the database.");
            }

            var eventToPatch = _mapper.Map<EventForUpdateDTO>(eventEntity);
            patchDocument.ApplyTo(eventToPatch);

            _mapper.Map(eventToPatch, eventEntity);
            _repository.Save();

            return NoContent();
        }
    }
}