using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class LibraryItem
    {
        public string CatalogNumber { get; }
        public string Title { get; }
        public decimal BaseLateFee { get; private set; }
        public bool IsWithdrawn { get; private set; }
        public bool IsOnLoan { get; private set; }

        public virtual int LoanPeriodInDays => 0;

        protected LibraryItem(string catalogNumber, string title, decimal baseLateFee)
        {
            if (string.IsNullOrWhiteSpace(catalogNumber))
                throw new ArgumentException("Catalog number cannot be null or empty.", nameof(catalogNumber));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            if (baseLateFee <= 0)
                throw new ArgumentException("Base late fee must be greater than zero.");

            CatalogNumber = catalogNumber;
            Title = title;
            BaseLateFee = baseLateFee;
            IsWithdrawn = false;
            IsOnLoan = false;
        }

        public void UpdateBaseFee(decimal newFee)
        {
            if (newFee <= 0)
                throw new ArgumentException("New base fee must be greater than zero.");
            BaseLateFee = newFee;
        }

        public void Withdraw()
        {
            IsWithdrawn = true;
        }

        public void Restore()
        {
            IsWithdrawn = false;
        }

        internal void SetOnLoanStatus(bool status)
        {
            IsOnLoan = status;
        }

        public virtual decimal GetDailyLateFee()
        {
            return BaseLateFee;
        }
    }
}
