using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class DVD : LibraryItem
    {
        public override int LoanPeriodInDays => 7;

        public DVD(string catalogNumber, string title, decimal baseLateFee)
            : base(catalogNumber, title, baseLateFee)
        {
        }

        public override decimal GetDailyLateFee()
        {
            return BaseLateFee * 2m; 
        }
    }
}
