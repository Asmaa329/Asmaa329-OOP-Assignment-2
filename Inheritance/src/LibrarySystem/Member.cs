using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class Member : Person
    {
        private readonly List<Loan> _loans = new List<Loan>();
        public IReadOnlyList<Loan> Loans => _loans.AsReadOnly();

        public virtual int MaxLoans => 0;
        public virtual decimal LateFeeDiscount => 0m;
        public int ReadingPoints { get; protected set; }

        protected Member(string personId, string fullName, string phone)
            : base(personId, fullName, phone)
        {
        }

        public virtual Loan BorrowItem(LibraryItem item, DateTime borrowDate)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            int activeLoansCount = 0;
            foreach (var loan in _loans)
            {
                if (loan.Status == LoanStatus.Borrowed)
                    activeLoansCount++;
            }

            if (activeLoansCount >= MaxLoans)
                throw new InvalidOperationException("Member has reached the maximum limit of active loans.");

            if (item.IsWithdrawn)
                throw new InvalidOperationException("Cannot borrow a withdrawn item.");

            if (item.IsOnLoan)
                throw new InvalidOperationException("Item is already on loan.");

            var loanObj = new Loan(Guid.NewGuid().ToString(), borrowDate, this, item);
            _loans.Add(loanObj);
            item.SetOnLoanStatus(true);

            return loanObj;
        }

        internal void AddReadingPoints(int points)
        {
            ReadingPoints += points;
        }
    }
}
