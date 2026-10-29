
using SRP.src.SrpLab;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRP.src.SrpLab
{
    public class GradeBook
    {
        private readonly Dictionary<string, List<decimal>> _scores = new(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<string> StudentIds => _scores.Keys.OrderBy(x => x);

        public void Record(string studentId, decimal score)
        {
            if (score is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(score));
            if (!_scores.TryGetValue(studentId, out var list))
            {
                list = new List<decimal>();
                _scores[studentId] = list;
            }
            list.Add(score);
        }

        public decimal Average(string studentId)
        {
            if (!_scores.TryGetValue(studentId, out var list) || list.Count == 0) return 0m;
            return Math.Round(list.Average(), 2);
        }
}


    public class GradingPolicy
    {
        public string CalculateLetter(decimal average)
        {
            if (average >= 90) return "A";
            if (average >= 80) return "B";
            if (average >= 70) return "C";
            if (average >= 60) return "D";
            return "F";
        }

        public bool MeetsHonorRoll(decimal average)
        {
            var letter = CalculateLetter(average);
            return average >= 85 && letter is "A" or "B";
        }
}

    public class TranscriptFormatter
    {
        private readonly GradingPolicy _gradingPolicy;

        public TranscriptFormatter(GradingPolicy gradingPolicy)
        {
            _gradingPolicy = gradingPolicy;
        }

        public string FormatPlainTranscript(GradeBook gradeBook, string studentId, string fullName)
        {
            var avg = gradeBook.Average(studentId);
            var letter = _gradingPolicy.CalculateLetter(avg);
            var isHonor = _gradingPolicy.MeetsHonorRoll(avg);

            return $"TRANSCRIPT\nStudent: {fullName} ({studentId})\nAverage: {avg}\nLetter: {letter}\nHonor: {isHonor}\n";
        }
    }

    public class GradeBookCsvExporter
    {
        private readonly GradingPolicy _gradingPolicy;

        public GradeBookCsvExporter(GradingPolicy gradingPolicy)
        {
            _gradingPolicy = gradingPolicy;
        }

        public string ExportCsv(GradeBook gradeBook)
        {
            var rows = new List<string> { "studentId,average,letter,honor" };

            foreach (var id in gradeBook.StudentIds)
            {
                var avg = gradeBook.Average(id);
                var letter = _gradingPolicy.CalculateLetter(avg);
                var isHonor = _gradingPolicy.MeetsHonorRoll(avg);

                rows.Add($"{id},{avg},{letter},{(isHonor ? 1 : 0)}");
            }

            return string.Join('\n', rows);
        }
    }
}
