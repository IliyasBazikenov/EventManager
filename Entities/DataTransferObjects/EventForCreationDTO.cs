﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class EventForCreationDTO
    {
        public int EventId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string EventName { get; set; }
        public DateTime DateOfEvent { get; set; }
        public string EventInfo { get; set; }
        public int ParticipantAmount { get; set; }
        public Guid AccountId { get; set; }
    }
}
