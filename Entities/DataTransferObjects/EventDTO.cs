using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Entities.DataTransferObjects
{
    public class EventDTO
    {
        public Guid EventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EventName { get; set; }
        public DateTime DateOfEvent { get; set; }

        [XmlElement(IsNullable = true)]
        public string EventInfo { get; set; } = "";
        public int ParticipantAmount { get; set; }
    }
}
