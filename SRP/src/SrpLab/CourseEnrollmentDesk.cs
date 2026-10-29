using SRP.src.SrpLab;
using System;
using System.Collections.Generic;
using System.Text;
namespace SrpLab;

    public class CourseEnrollmentDesk
    {
        private readonly HashSet<string> _seated = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _waitlist = new();
    public int Capacity { get; }
        public decimal Tuition { get; }
        public string CourseCode { get; }

        public CourseEnrollmentDesk(string courseCode, int capacity, decimal tuition)
        {
            CourseCode = courseCode;
            Capacity = capacity;
            Tuition = tuition;
        }

        public string Register(string studentEmail)
        {
            if (string.IsNullOrWhiteSpace(studentEmail))
                throw new ArgumentException("email cannot be empty", nameof(studentEmail));

            var email = studentEmail.Trim();

            if (_seated.Contains(email) || _waitlist.Contains(email))
                return "ALREADY_REGISTERED";

            if (_seated.Count < Capacity)
            {
                _seated.Add(email);
                return "SEATED";
            }

            _waitlist.Add(email);
            return $"WAITLIST:{_waitlist.Count}";
        }

        public int WaitlistPosition(string studentEmail)
        {
            var idx = _waitlist.FindIndex(x => x.Equals(studentEmail, StringComparison.OrdinalIgnoreCase));
            return idx < 0 ? -1 : idx + 1;
        }

        public bool IsSeated(string studentEmail) => _seated.Contains(studentEmail);

        public void PromoteFromWaitlist(int seats)
        {
            while (seats > 0 && _waitlist.Count > 0 && _seated.Count < Capacity)
            {
                var next = _waitlist[0];
                _waitlist.RemoveAt(0);
                _seated.Add(next);
                seats--;
            }
        }
    }

    public class CourseWelcomePacketFormatter
    {
        public string DraftWelcomePacket(CourseEnrollmentDesk enrollment, string studentEmail, string studentName)
        {
            var status = enrollment.IsSeated(studentEmail)
                ? "confirmed seat"
                : $"waitlist #{enrollment.WaitlistPosition(studentEmail)}";

            return $"# Welcome to {enrollment.CourseCode}\n" +
                   $"Hi {studentName},\n" +
                   $"Your status: **{status}**.\n" +
                   $"Bring a laptop. Discord onboarding link: https://example.invalid/{enrollment.CourseCode.ToLowerInvariant()}\n";
        }
    }


    public class CourseInvoiceCalculator
    {
        private const decimal VatRate = 0.14m;

        public string GenerateTuitionInvoiceLine(CourseEnrollmentDesk enrollment, string studentEmail)
        {
            if (!enrollment.IsSeated(studentEmail))
                return $"{enrollment.CourseCode},WAITLIST,0.00";

            var tuition = enrollment.Tuition;
            var vat = Math.Round(tuition * VatRate, 2);
            var total = tuition + vat;

            return $"{enrollment.CourseCode},TUITION,{tuition:0.00},VAT,{vat:0.00},TOTAL,{total:0.00}";
        }
    }

