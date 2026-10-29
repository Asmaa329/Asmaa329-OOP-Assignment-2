using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class PremiumMember : Member
    {
        public override int MaxLoans => 10;
        public override decimal LateFeeDiscount { get; }

        public PremiumMember(string personId, string fullName, string phone, decimal discountPercentage)
            : base(personId, fullName, phone)
        {
            if (discountPercentage < 0)
                throw new System.ArgumentException("Discount percentage cannot be negative.");
            LateFeeDiscount = discountPercentage;
        }
    }
}
