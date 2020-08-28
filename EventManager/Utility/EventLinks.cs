using Contracts;
using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Utility
{
    public class EventLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EventDTO> _dataShaper;

        public EventLinks(LinkGenerator linkGenerator, IDataShaper<EventDTO> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<EventDTO> eventDTOs,
            string fields, Guid accountId, HttpContext httpContext)
        {
            List<Entity> shapedEvents = ShapeData(eventDTOs, fields);

            if (ShouldGenerateLinks(httpContext))
                return ReturnLinkedEvents(eventDTOs, fields, accountId, httpContext, shapedEvents);

            return ReturnShapedEvents(shapedEvents);
        }

        private LinkResponse ReturnShapedEvents(List<Entity> shapedEvents)
        {
            return new LinkResponse { ShapedEntities = shapedEvents };
        }

        private LinkResponse ReturnLinkedEvents(IEnumerable<EventDTO> eventDTOs,
            string fields, Guid accountId, HttpContext httpContext, List<Entity> shapedEvents)
        {
            var eventDTOsList = eventDTOs.ToList();
            for (int index = 0; index < eventDTOsList.Count; index++)
            {
                List<Link> eventLinks = CreateLinksForEvent(httpContext, accountId, eventDTOsList[index].EventId, fields);
                shapedEvents[index].Add("Links", eventLinks);
            }

            var eventCollection = new LinkCollectionWrapper<Entity>(shapedEvents);
            LinkCollectionWrapper<Entity> linkedEvents = CreateLinksForEvent(httpContext, eventCollection);

            return new LinkResponse { HasLinks = true, LinkedEntites = linkedEvents };
        }

        private LinkCollectionWrapper<Entity> CreateLinksForEvent(HttpContext httpContext, LinkCollectionWrapper<Entity> eventsWrapper)
        {
            eventsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext,
                "GetEventsForAccount", values: new { }),
                "self",
                "GET"));

            return eventsWrapper;
        }

        private List<Link> CreateLinksForEvent(HttpContext httpContext, Guid accountId, Guid eventId, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetEventForAccount",
                values: new { accountId, eventId, fields }),
                "self",
                "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteEvent",
                values: new { accountId, eventId }),
                "delete_event",
                "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateEventForAccount",
                values: new { accountId, eventId }),
                "update_event",
                "PUT"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateEvent",
                values: new { accountId, eventId }),
                "partially_update_event",
                "PATCH")
            };
            return links;
        }

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas",
                StringComparison.InvariantCultureIgnoreCase);
        }

        private List<Entity> ShapeData(IEnumerable<EventDTO> eventDTOs, string fields)
        {
            return _dataShaper.ShapeData(eventDTOs, fields)
                .Select(e => e.Entity)
                .ToList();
        }
    }
}
