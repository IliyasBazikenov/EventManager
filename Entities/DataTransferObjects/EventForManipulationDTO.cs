using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
    public abstract class EventForManipulationDTO
    {
        [Required(ErrorMessage = "Created date is required")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Event should have name")]
        [StringLength(100, ErrorMessage = "Event name length should be less than 100 characters", MinimumLength = 1)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfEvent { get; set; }
        public string EventInfo { get; set; }
        public int ParticipantAmount { get; set; }
    }
}
