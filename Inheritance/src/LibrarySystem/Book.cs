using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class Book : LibraryItem
    {
        public override int LoanPeriodInDays => 21;

        public Book(string catalogNumber, string title, decimal baseLateFee)
            : base(catalogNumber, title, baseLateFee)
        {
        }

        public override decimal GetDailyLateFee()
        {
            return BaseLateFee; 
        }
    }
}
