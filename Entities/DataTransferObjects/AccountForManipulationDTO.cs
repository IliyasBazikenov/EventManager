﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
    public abstract class AccountForManipulationDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Created date is required")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

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

        [MaxLength(150, ErrorMessage = "Address length must be  less than 150 characters")]
        public string Address { get; set; }

        public int FriendsAmount { get; set; }

        [Required(ErrorMessage = "Account type is required")]
        public string AccountType { get; set; }

        public IEnumerable<EventForCreationDTO> Events { get; set; }

    }
}
