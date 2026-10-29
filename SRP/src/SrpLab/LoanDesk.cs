using SRP.src.SrpLab;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRP.src.SrpLab
{
    public class LoanDesk
    {
        public decimal RequestedAmount { get; }
        public int CreditScore { get; }
        public int EmploymentMonths { get; }
        public bool HasCollateral { get; }

        public LoanDesk(decimal requestedAmount, int creditScore, int employmentMonths, bool hasCollateral)
        {
            RequestedAmount = requestedAmount;
            CreditScore = creditScore;
            EmploymentMonths = employmentMonths;
            HasCollateral = hasCollateral;
        }

        public decimal RiskScore()
        {
            decimal score = 100m;
            score -= Math.Max(0, 700 - CreditScore) * 0.15m;
            if (EmploymentMonths < 6) score -= 20m;
            if (RequestedAmount > 50_000m && !HasCollateral) score -= 25m;
            if (RequestedAmount > 150_000m) score -= 10m;
            return Math.Clamp(score, 0m, 100m);
        }

        public bool IsEligible() => RiskScore() >= 55m && CreditScore >= 580;
}

    public class LoanComplianceChecker
    {
        public IReadOnlyList<string> RequiredDocuments(LoanDesk loan)
        {
            var docs = new List<string> { "National ID", "Proof of income (3 months)" };
            if (loan.RequestedAmount > 40_000m) docs.Add("Bank statements (6 months)");
            if (loan.HasCollateral) docs.Add("Collateral ownership deed");
            if (loan.EmploymentMonths < 12) docs.Add("Employer letter");
            if (!loan.IsEligible()) docs.Add("Manual underwriter referral form");
            return docs;
        }
}


    public class LoanDecisionLetterFormatter
    {
        private readonly LoanComplianceChecker _complianceChecker;

        public LoanDecisionLetterFormatter(LoanComplianceChecker complianceChecker)
        {
            _complianceChecker = complianceChecker;
        }

        public string FormatDecisionLetter(LoanDesk loan, string applicantName)
        {
            if (loan.IsEligible())
            {
                var docs = _complianceChecker.RequiredDocuments(loan);
                return $"Dear {applicantName},\nYour request for {loan.RequestedAmount:C} is pre-approved (risk {loan.RiskScore():0}).\n" +
                       $"Please upload: {string.Join("; ", docs)}.\n";
            }

            return $"Dear {applicantName},\nWe are unable to approve {loan.RequestedAmount:C} at this time.\n" +
                   $"Reference risk={loan.RiskScore():0}. You may reapply after improving documentation.\n";
        }
}


    public class LoanAnalyticsExporter
    {
        public string ExportUnderwriterCsvRow(LoanDesk loan, string applicationId)
        {
            return $"{applicationId},{loan.CreditScore},{loan.EmploymentMonths},{(loan.HasCollateral ? 1 : 0)},{loan.RiskScore():0.00},{(loan.IsEligible() ? "Y" : "N")}";
        }
    }
}
