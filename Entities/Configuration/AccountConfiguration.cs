using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Configuration
{
    public class AccountConfiguration
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasData
                (
                    new Account
                    {
                        AccountId = Guid.NewGuid(),
                        FirstName = "Alexander",
                        SecondName = "Maglev",
                        AccountType = "admin",
                        Email = "some@gmail.com",
                        BirthDate = DateTime.Parse("12-08-1997 07:22:16"),
                        CreatedDate = DateTime.UtcNow,
                        Password = "Qazwsxedc1!",
                        PhoneNumber = "+79999999999"
                    }
                );
        }
    }
}
