using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class HeadLibrarian : Staff
    {
        private const decimal ResponsibilityAllowance = 400m;

        public HeadLibrarian(string personId, string fullName, string phone, DateTime hireDate, decimal monthlySalary)
            : base(personId, fullName, phone, hireDate, monthlySalary)
        {
        }

        public override decimal GetMonthlyPay()
        {
            return MonthlySalary + ResponsibilityAllowance;
        }

        public void UpdateItemFee(LibraryItem item, decimal newFee)
        {
            item.UpdateBaseFee(newFee);
        }

        public void WithdrawItem(LibraryItem item)
        {
            item.Withdraw();
        }

        public void RestoreItem(LibraryItem item)
        {
            item.Restore();
        }
    }
}
