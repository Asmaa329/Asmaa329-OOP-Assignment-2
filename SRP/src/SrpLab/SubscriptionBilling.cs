using SRP.src.SrpLab;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRP.src.SrpLab
{
    public class InvoiceNumberGenerator
    {
        private static int _invoiceSeq = 1000;

        public string NextInvoiceNumber(DateOnly periodStart)
        {
            var n = Interlocked.Increment(ref _invoiceSeq);
            return $"INV-{periodStart:yyyyMM}-{n:D5}";
        }
}


    public class SubscriptionBilling
    {
        public string CustomerId { get; }
        public decimal MonthlyPrice { get; }
        public DateOnly PeriodStart { get; }
        public DateOnly PeriodEnd { get; }
        public int FailedPayments { get; private set; }

        public SubscriptionBilling(string customerId, decimal monthlyPrice, DateOnly periodStart, DateOnly periodEnd)
        {
            CustomerId = customerId;
            MonthlyPrice = monthlyPrice;
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
        }

        public decimal Prorate(DateOnly activeFrom)
        {
            if (activeFrom <= PeriodStart) return MonthlyPrice;
            if (activeFrom >= PeriodEnd) return 0m;
            var totalDays = PeriodEnd.DayNumber - PeriodStart.DayNumber;
            if (totalDays <= 0) return MonthlyPrice;
            var used = PeriodEnd.DayNumber - activeFrom.DayNumber;
            return Math.Round(MonthlyPrice * used / totalDays, 2);
        }

        public void RegisterFailedPayment() => FailedPayments++;
}


    public class DunningEmailFormatter
    {
        private readonly InvoiceNumberGenerator _invoiceNumberGenerator;

        public DunningEmailFormatter(InvoiceNumberGenerator invoiceNumberGenerator)
        {
            _invoiceNumberGenerator = invoiceNumberGenerator;
        }

        public string DraftDunningEmail(SubscriptionBilling subscription, string customerName, DateOnly asOf)
        {
            var amount = subscription.Prorate(subscription.PeriodStart);
            var invoice = _invoiceNumberGenerator.NextInvoiceNumber(subscription.PeriodStart);
            var severity = subscription.FailedPayments switch
            {
                <= 1 => "friendly reminder",
                2 => "second notice",
                _ => "final notice before suspension"
            };
            return $"Subject: {severity} {invoice}\nHi {customerName},\nBalance {amount:C} as of {asOf:o} ({subscription.FailedPayments} failures).\n";
        }
    }


    public class LedgerJournalExporter
    {
        private readonly InvoiceNumberGenerator _invoiceNumberGenerator;

        public LedgerJournalExporter(InvoiceNumberGenerator invoiceNumberGenerator)
        {
            _invoiceNumberGenerator = invoiceNumberGenerator;
        }

        public string ExportLedgerJournalLine(SubscriptionBilling subscription, DateOnly activeFrom)
        {
            var invoice = _invoiceNumberGenerator.NextInvoiceNumber(subscription.PeriodStart);
            var amount = subscription.Prorate(activeFrom);
            return $"{subscription.CustomerId},{invoice},{amount:0.00},AR-SUB";
        }
    }
}
