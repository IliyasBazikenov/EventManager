using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    public class EventParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EventParticipantId { get; set; }

        [ForeignKey(nameof(Event))]
        public Guid EventId { get; set; }
        public Event Event { get; set; }

        [ForeignKey(nameof(Account))]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        [Range(0, 1, ErrorMessage = "Please enter a valid participant status for exmp: 0,1 - pending, participant")]
        public int ParticipantStatus { get; set; }
    }
}
