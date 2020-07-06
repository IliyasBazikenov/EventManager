using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class AccountForCreationDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string FirstName { get; set; }
        public string LastName { set; get; }
        public string SecondName { set; get; }
        public DateTime BirthDate { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public int FriendsAmount { get; set; }
        public string AccountType { get; set; }

        public IEnumerable<EventForCreationDTO> Events { get; set; }
        
    }
}
