using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.src
{
    public sealed class CourseRegistration
    {
        public string StudentEmail { get; }
        public string CourseCode { get; }
        public string AccessMode { get; }
        public string? GroupCode { get; }
        public string? DiscountCode { get; }
        public bool SendWhatsApp { get; }
        public bool SendEmailWelcome { get; }
        public string? MentorNote { get; }
        public DateOnly? PreferredStart { get; }

        internal CourseRegistration(
            string studentEmail,
            string courseCode,
            string accessMode,
            string? groupCode,
            string? discountCode,
            bool sendWhatsApp,
            bool sendEmailWelcome,
            string? mentorNote,
            DateOnly? preferredStart)
        {
            StudentEmail = studentEmail;
            CourseCode = courseCode;
            AccessMode = accessMode;
            GroupCode = groupCode;
            DiscountCode = discountCode;
            SendWhatsApp = sendWhatsApp;
            SendEmailWelcome = sendEmailWelcome;
            MentorNote = mentorNote;
            PreferredStart = preferredStart;
        }

        public override string ToString()
            => $"{StudentEmail} → {CourseCode} [{AccessMode}] group={GroupCode ?? "-"} discount={DiscountCode ?? "-"} wa={SendWhatsApp} mail={SendEmailWelcome}";
    }

    public sealed class CourseRegistrationBuilder
    {
        private readonly string _studentEmail;
        private readonly string _courseCode;
        private string _accessMode = "VideosOnly";
        private string? _groupCode;
        private string? _discountCode;
        private bool _sendWhatsApp = false;
        private bool _sendEmailWelcome = true;
        private string? _mentorNote;
        private DateOnly? _preferredStart;

        public CourseRegistrationBuilder(string studentEmail, string courseCode)
        {
            if (string.IsNullOrWhiteSpace(studentEmail))
                throw new ArgumentException("Student email is required.", nameof(studentEmail));
            if (string.IsNullOrWhiteSpace(courseCode))
                throw new ArgumentException("Course code is required.", nameof(courseCode));

            _studentEmail = studentEmail;
            _courseCode = courseCode;
        }

        public CourseRegistrationBuilder AsLiveGroup(string groupCode)
        {
            _accessMode = "LiveGroup";
            _groupCode = groupCode;
            return this;
        }

        public CourseRegistrationBuilder AsVideosOnly()
        {
            _accessMode = "VideosOnly";
            _groupCode = null;
            return this;
        }

        public CourseRegistrationBuilder WithDiscountCode(string? discountCode)
        {
            _discountCode = discountCode;
            return this;
        }

        public CourseRegistrationBuilder SendWhatsApp(bool sendWhatsApp = true)
        {
            _sendWhatsApp = sendWhatsApp;
            return this;
        }

        public CourseRegistrationBuilder SendEmailWelcome(bool sendEmailWelcome = true)
        {
            _sendEmailWelcome = sendEmailWelcome;
            return this;
        }

        public CourseRegistrationBuilder WithMentorNote(string? mentorNote)
        {
            _mentorNote = mentorNote;
            return this;
        }

        public CourseRegistrationBuilder WithPreferredStart(DateOnly? preferredStart)
        {
            _preferredStart = preferredStart;
            return this;
        }
        public CourseRegistration Build()
        {
            if (_accessMode == "LiveGroup" && string.IsNullOrWhiteSpace(_groupCode))
                throw new InvalidOperationException("LiveGroup registration requires a valid GroupCode.");

            if (_accessMode == "VideosOnly" && !string.IsNullOrWhiteSpace(_groupCode))
                throw new InvalidOperationException("VideosOnly registration cannot have a GroupCode.");

            return new CourseRegistration(
                _studentEmail,
                _courseCode,
                _accessMode,
                _groupCode,
                _discountCode,
                _sendWhatsApp,
                _sendEmailWelcome,
                _mentorNote,
                _preferredStart);
        }
        public static class RegistrationCallSites
        {
            public static CourseRegistration CreateLiveStudentUgly()
            {
                return new CourseRegistrationBuilder("sara@mail.com", "SEF-101")
                    .AsLiveGroup("G1")
                    .WithDiscountCode("EARLY10")
                    .SendWhatsApp(true)
                    .SendEmailWelcome(true)
                    .WithMentorNote("Needs evening slot")
                    .WithPreferredStart(new DateOnly(2026, 10, 1))
                    .Build();
            }

            public static CourseRegistration CreateVideosOnlyUgly()
            {
                return new CourseRegistrationBuilder("ali@mail.com", "SEF-101")
                    .AsVideosOnly()
                    .SendEmailWelcome(true)
                    .Build();
            }
        }
    }
}
