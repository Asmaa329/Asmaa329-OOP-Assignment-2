using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class Magazine : LibraryItem
    {
        public override int LoanPeriodInDays => 3;

        public Magazine(string catalogNumber, string title, decimal baseLateFee)
            : base(catalogNumber, title, baseLateFee)
        {
        }

        public override decimal GetDailyLateFee()
        {
            return BaseLateFee * 0.5m; 
        }
    }
}
