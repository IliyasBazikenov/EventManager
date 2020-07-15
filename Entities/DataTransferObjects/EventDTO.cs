using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class EventDTO
    {
        public int EventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EventName { get; set; }
        public DateTime DateOfEvent { get; set; }
        public string EventInfo { get; set; }
        public int ParticipantAmount { get; set; }
    }
}
