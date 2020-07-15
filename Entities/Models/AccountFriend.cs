using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class AccountFriend
    {

        [Required(ErrorMessage = "Account id is required")]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "Friend id is reuqired")]
        public Guid FriendId { get; set; }

        [Range(0, 1, ErrorMessage = "Please enter a valid friend status for exmp: 0,1 - pending, firends")]
        public int Status { get; set; }
    }
}
