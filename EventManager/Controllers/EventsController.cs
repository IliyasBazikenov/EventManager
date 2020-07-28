using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using EventManager.ActionFilters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [ServiceFilter(typeof(ValidateAccountExistsAttribute))]
        public async Task<IActionResult> GetEventsForAccount(Guid accountId, [FromQuery] EventParameters eventParameters)
        {
            if (eventParameters.ValidDateRange!)
                return BadRequest("Max date can't be less than min age.");

            var accountEvents = await _repository.Event.GetEventsAsync(accountId, eventParameters, trackChanges: false);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(accountEvents.MetaData));

            var accountDTO = _mapper.Map<IEnumerable<EventDTO>>(accountEvents);

            return Ok(accountDTO);
        }

        [HttpGet("{eventId}", Name = "GetEventForAccount")]
        [ServiceFilter(typeof(ValidateEventExistsAttribute))]
        public IActionResult GetEventForAccount(Guid accountId, int eventId)
        {
            var accountEvent = HttpContext.Items["event"] as Event;

            var accountDTO = _mapper.Map<EventDTO>(accountEvent);
            return Ok(accountDTO);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateAccountExistsAttribute))]
        public async Task<IActionResult> CreateEvent(Guid accountId, [FromBody] EventForCreationDTO eventDTO)
        {
            var eventEntity = _mapper.Map<Event>(eventDTO);

            _repository.Event.CreateEvent(accountId, eventEntity);
            await _repository.SaveAsync();

            var eventToReturn = _mapper.Map<EventDTO>(eventEntity);

            return CreatedAtRoute("GetEventForAccount", new { accountId, eventId = eventToReturn.EventId }, eventToReturn);
        }

        [HttpPut("{eventId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEventExistsAttribute))]
        public async Task<IActionResult> UpdateEventForAccount(Guid accountId, int eventId, [FromBody] EventForUpdateDTO eventDTO)
        {
            var eventEntity = HttpContext.Items["event"] as Event;

            _mapper.Map(eventDTO, eventEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{eventId}")]
        [ServiceFilter(typeof(ValidateEventExistsAttribute))]
        public async Task<IActionResult> PartiallyUpdateEvent(Guid accountId, int eventId,
            [FromBody] JsonPatchDocument<EventForUpdateDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                _logger.LogError("Event patchDocument object sent from client is null.");
                return BadRequest("patchDocument object is null");
            }

            var eventEntity = HttpContext.Items["event"] as Event;

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

        [HttpDelete("{eventId}")]
        [ServiceFilter(typeof(ValidateEventExistsAttribute))]
        public async Task<IActionResult> DeleteEvent(Guid accountId, int eventId)
        {
            var accountEvent = HttpContext.Items["event"] as Event;

            _repository.Event.DeleteEvent(accountEvent);
            await _repository.SaveAsync();

            return NoContent();
        }

    }
}