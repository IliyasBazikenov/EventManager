using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Created date is required")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Event should have name")]
        [StringLength(100, ErrorMessage = "Event name length should be less than 100 characters", MinimumLength = 1)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfEvent { get; set; }
        public string EventInfo { get; set; }
        public int ParticipantAmount { get; set; }

        [ForeignKey(nameof(Account))]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public ICollection<EventParticipant> EventParticipants { get; set; }
    }
}
