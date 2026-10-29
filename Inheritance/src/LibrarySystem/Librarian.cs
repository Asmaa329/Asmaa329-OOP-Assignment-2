using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class Librarian : Staff
    {
        public Librarian(string personId, string fullName, string phone, DateTime hireDate, decimal monthlySalary)
            : base(personId, fullName, phone, hireDate, monthlySalary)
        {
        }

        public void ProcessReturn(Loan loan, DateTime returnDate)
        {
            loan.ReturnLoan(returnDate);
        }

        public void MarkAsLost(Loan loan)
        {
            loan.MarkAsLost();
        }
    }
}
