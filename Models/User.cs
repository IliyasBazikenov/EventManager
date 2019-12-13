using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class User
    {
        [Key, E]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "Invalid password")]
        public string Password { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Firstname length must be between 1 and 50 character", MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Lastname length must be between 1 and 50 character", MinimumLength = 1)]
        public string LastName { set; get; }

        [StringLength(50, ErrorMessage = "Secondname length must be between 1 and 50 character", MinimumLength = 1)]
        public string SecondName { set; get; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [MaxLength(50, ErrorMessage = "Country length must be  less than 50 characters")]
        public string Country { get; set; }

        [MaxLength(50, ErrorMessage = "City length must be  less than 50 characters")]
        public string City { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
