using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class Loan
    {
        public string LoanId { get; }
        public DateTime BorrowDate { get; }
        public Member Member { get; }
        public LibraryItem Item { get; }
        public LoanStatus Status { get; private set; }
        public DateTime? ReturnDate { get; private set; }

        public DateTime DueDate => BorrowDate.AddDays(Item.LoanPeriodInDays);

        public decimal LateFee
        {
            get
            {
                DateTime effectiveReturnDate = ReturnDate ?? DateTime.Now;
                if (Status == LoanStatus.Returned && effectiveReturnDate <= DueDate)
                {
                    return 0m;
                }

                int daysLate = 0;
                if (effectiveReturnDate > DueDate)
                {
                    daysLate = (int)(effectiveReturnDate - DueDate).TotalDays;
                }

                decimal calculatedFee = daysLate * Item.GetDailyLateFee() - Member.LateFeeDiscount;
                return calculatedFee < 0 ? 0m : calculatedFee;
            }
        }

        public Loan(string loanId, DateTime borrowDate, Member member, LibraryItem item)
        {
            if (string.IsNullOrWhiteSpace(loanId)) throw new ArgumentNullException(nameof(loanId));
            LoanId = loanId;
            BorrowDate = borrowDate;
            Member = member ?? throw new ArgumentNullException(nameof(member));
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Status = LoanStatus.Borrowed;
            ReturnDate = null;
        }

        public void ReturnLoan(DateTime returnDate)
        {
            if (Status != LoanStatus.Borrowed)
                throw new InvalidOperationException("Only active borrowed loans can be returned.");

            if (returnDate < BorrowDate)
                throw new ArgumentException("Return date cannot be earlier than borrow date.");

            ReturnDate = returnDate;
            Status = LoanStatus.Returned;
            Item.SetOnLoanStatus(false);

            if (Member is PremiumMember)
            {
                Member.AddReadingPoints(5);
            }
        }

        public void MarkAsLost()
        {
            if (Status != LoanStatus.Borrowed)
                throw new InvalidOperationException("Only active borrowed loans can be marked as lost.");

            Status = LoanStatus.Lost;
            Item.SetOnLoanStatus(false);
        }
    }
}
