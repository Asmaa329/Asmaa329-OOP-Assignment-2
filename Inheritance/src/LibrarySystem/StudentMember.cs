using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class StudentMember : Member
    {
        public override int MaxLoans => 3;
        public override decimal LateFeeDiscount => 0m;

        public StudentMember(string personId, string fullName, string phone)
            : base(personId, fullName, phone)
        {
        }
    }
}
