using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.RequestFeatures
{
    public class AccountParameters : RequestParameters
    {
        public AccountParameters()
        {
            OrderBy = "lastName";
        }

        public string SearchTerm { get; set; }
    }
}
