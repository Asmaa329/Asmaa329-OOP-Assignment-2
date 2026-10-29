using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem
{
    public class Shelver : Staff
    {
        public string Section { get; private set; }

        public Shelver(string personId, string fullName, string phone, DateTime hireDate, decimal monthlySalary, string section)
            : base(personId, fullName, phone, hireDate, monthlySalary)
        {
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("Section cannot be null or empty.", nameof(section));
            Section = section;
        }

        public void Reassign(string newSection)
        {
            if (string.IsNullOrWhiteSpace(newSection))
                throw new ArgumentException("New section cannot be null or empty.", nameof(newSection));
            Section = newSection;
        }
    }
}
