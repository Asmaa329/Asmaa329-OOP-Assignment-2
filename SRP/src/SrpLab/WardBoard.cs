using SRP.src.SrpLab;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRP.src.SrpLab
{
    public class AcuityScorer
    {
        public int ScoreAcuity(int heartRate, int spo2)
        {
            var score = 0;
            if (heartRate > 120 || heartRate < 45) score += 4;
            else if (heartRate > 100) score += 2;

            if (spo2 < 90) score += 5;
            else if (spo2 < 94) score += 2;

            return Math.Min(score, 10);
        }
}

    public class WardPagerService
    {
        private readonly List<string> _pagerLog = new();

        public void NotifyCodeYellowIfNeeded(int bed, int acuityScore)
        {
            if (acuityScore >= 8)
            {
                _pagerLog.Add($"CODE-YELLOW bed={bed} at {DateTime.UtcNow:HH:mm}");
            }
        }

        public IReadOnlyList<string> DrainPagerLog()
        {
            var copy = _pagerLog.ToList();
            _pagerLog.Clear();
            return copy;
        }
}


    public class WardBoard
    {
        private readonly Dictionary<int, string> _bedPatient = new();
        private readonly Dictionary<int, int> _vitalsScore = new();
        private readonly AcuityScorer _acuityScorer;
        private readonly WardPagerService _pagerService;

        public IReadOnlyDictionary<int, string> BedPatients => _bedPatient;
        public IReadOnlyDictionary<int, int> VitalsScores => _vitalsScore;

        public WardBoard(AcuityScorer acuityScorer, WardPagerService pagerService)
        {
            _acuityScorer = acuityScorer;
            _pagerService = pagerService;
        }

        public void AssignBed(int bed, string patientId, int heartRate, int spo2)
        {
            if (bed <= 0) throw new ArgumentOutOfRangeException(nameof(bed));
            if (string.IsNullOrWhiteSpace(patientId)) throw new ArgumentException("patient required", nameof(patientId));

            _bedPatient[bed] = patientId.Trim().ToUpperInvariant();
            var acuity = _acuityScorer.ScoreAcuity(heartRate, spo2);
            _vitalsScore[bed] = acuity;

            _pagerService.NotifyCodeYellowIfNeeded(bed, acuity);
        }
}


    public class NurseHandoffFormatter
    {
        public string BuildHandoffNote(WardBoard wardBoard, int bed)
        {
            if (!wardBoard.BedPatients.TryGetValue(bed, out var patient))
                return $"Bed {bed}: empty";

            var acuity = wardBoard.VitalsScores[bed];
            var tone = acuity >= 8 ? "ESCALATE" : acuity >= 4 ? "WATCH" : "STABLE";

            return $"[HANDOFF {DateTime.UtcNow:yyyy-MM-dd}] Bed {bed} · {patient} · acuity={acuity} · {tone}";
        }
}

    public class CensusCsvExporter
    {
        public string ExportCensusCsv(WardBoard wardBoard)
        {
            var lines = new List<string> { "bed,patient,acuity" };
            foreach (var bed in wardBoard.BedPatients.Keys.OrderBy(x => x))
                lines.Add($"{bed},{wardBoard.BedPatients[bed]},{wardBoard.VitalsScores[bed]}");

            return string.Join('\n', lines);
        }
    }
}
