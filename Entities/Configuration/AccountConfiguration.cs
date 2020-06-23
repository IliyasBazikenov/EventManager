using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Configuration
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasData
                (
                    new Account
                    {
                        AccountId = Guid.NewGuid(),
                        FirstName = "Alexander",
                        LastName = "Maglev",
                        AccountType = "admin",
                        Email = "some@gmail.com",
                        BirthDate = DateTime.Parse("12-08-1997 07:22:16"),
                        CreatedDate = DateTime.Now,
                        Password = "Qazwsxedc1!",
                        PhoneNumber = "+79999999999"
                    },

                    new Account
                    {
                        AccountId = Guid.NewGuid(),
                        FirstName = "Ivan",
                        LastName = "Maglev",
                        AccountType = "admin",
                        Email = "some@mail.com",
                        BirthDate = DateTime.Parse("12-08-1999 12:22:16"),
                        CreatedDate = DateTime.Now,
                        Password = "Qazwsxedc1!",
                        PhoneNumber = "+79999999998"
                    }
                );
        }
    }
}
