using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using EventManager.ActionFilters;
using EventManager.Utility;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventManager.Controllers
{
    [Route("api/accounts/{accountId}/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly EventLinks _eventLinks;
        public EventsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper,
            EventLinks eventLinks)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _eventLinks = eventLinks;
        }

        [HttpGet]
        [HttpHead]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        [ServiceFilter(typeof(ValidateAccountExistsAttribute))]
        public async Task<IActionResult> GetEventsForAccount(Guid accountId, [FromQuery] EventParameters eventParameters)
        {
            if (eventParameters.ValidDateRange!)
                return BadRequest("Max date can't be less than min age.");

            PagedList<Event> accountEvents = await _repository.Event.GetEventsAsync(accountId, eventParameters, trackChanges: false);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(accountEvents.MetaData));

            var eventsDTO = _mapper.Map<IEnumerable<EventDTO>>(accountEvents);

            var links = _eventLinks.TryGenerateLinks(eventsDTO, 
                eventParameters.Fields, accountId, HttpContext);

            return links.HasLinks ? Ok(links.LinkedEntites) : Ok(links.ShapedEntities);
        }

        [HttpGet("{eventId}", Name = "GetEventForAccount")]
        [HttpHead]
        [ServiceFilter(typeof(ValidateEventExistsAttribute))]
        public IActionResult GetEventForAccount(Guid accountId, Guid eventId)
        {
            var accountEvent = HttpContext.Items["event"] as Event;

            var eventDTO = _mapper.Map<EventDTO>(accountEvent);
            return Ok(eventDTO);
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
        public async Task<IActionResult> UpdateEventForAccount(Guid accountId, Guid eventId, [FromBody] EventForUpdateDTO eventDTO)
        {
            var eventEntity = HttpContext.Items["event"] as Event;

            _mapper.Map(eventDTO, eventEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{eventId}")]
        [ServiceFilter(typeof(ValidateEventExistsAttribute))]
        public async Task<IActionResult> PartiallyUpdateEvent(Guid accountId, Guid eventId,
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
        public async Task<IActionResult> DeleteEvent(Guid accountId, Guid eventId)
        {
            var accountEvent = HttpContext.Items["event"] as Event;

            _repository.Event.DeleteEvent(accountEvent);
            await _repository.SaveAsync();

            return NoContent();
        }

    }
}