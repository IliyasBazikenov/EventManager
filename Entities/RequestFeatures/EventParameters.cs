using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.RequestFeatures
{
    public class EventParameters : RequestParameters
    {
        public DateTime MinEventDate { get; set; } = DateTime.Now;
        public DateTime MaxEventDate { get; set; } = DateTime.MaxValue;

        public bool ValidDateRange => DateTime.Compare(MinEventDate, MaxEventDate) >= 0;

        public string SearchTerm { get; set; }
    }
}
