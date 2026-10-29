using SRP.src.SrpLab;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRP.src.SrpLab
{
    public class TicketPriorityClassifier
    {
        public string ClassifyPriority(string subject, string body)
        {
            var blob = (subject + " " + body).ToLowerInvariant();

            if (blob.Contains("down") || blob.Contains("outage") || blob.Contains("cannot login"))
                return "P1";

            if (blob.Contains("urgent") || blob.Contains("asap") || blob.Contains("blocked"))
                return "P2";

            return "P3";
        }
}
 public class SupportTicket
    {
        private readonly TicketPriorityClassifier _priorityClassifier;

        public string Id { get; } = string.Empty;
        public string Subject { get; } = string.Empty;
        public string Body { get; private set; } = string.Empty;  
        public DateTimeOffset OpenedAt { get; }
        public string Priority { get; private set; } = string.Empty;

        public SupportTicket(string id, string subject , string body, DateTimeOffset openedAt, TicketPriorityClassifier priorityClassifier)
        {
            Id = id;
            Subject = subject;
            Body = body;
            OpenedAt = openedAt;
            _priorityClassifier = priorityClassifier;

            RecalculatePriority();
        }

        public void AppendCustomerMessage(string text)
        {
            Body += "\n---\n" + text;
            RecalculatePriority();
        }

        private void RecalculatePriority()
        {
            Priority = _priorityClassifier.ClassifyPriority(Subject, Body);
        }
}

    public class SlaCalculator
    {
        public DateTimeOffset CalculateSlaDeadline(SupportTicket ticket)
        {
            var hours = ticket.Priority switch
            {
                "P1" => 4,
                "P2" => 24,
                _ => 72
            };
            return ticket.OpenedAt.AddHours(hours);
        }

        public bool IsBreached(SupportTicket ticket, DateTimeOffset now)
        {
            return now > CalculateSlaDeadline(ticket);
        }
}

    public class PublicReplyFormatter
    {
        private readonly SlaCalculator _slaCalculator;

        public PublicReplyFormatter(SlaCalculator slaCalculator)
        {
            _slaCalculator = slaCalculator;
        }

        public string DraftPublicReply(SupportTicket ticket, string agentName)
        {
            var apology = ticket.Priority == "P1"
                ? "We are treating this as a critical incident."
                : "Thanks for reaching out.";

            var deadline = _slaCalculator.CalculateSlaDeadline(ticket);

            return $"Hi,\n{apology}\nTicket {ticket.Id} is with {agentName}. Next update before {deadline:u}.\n";
        }
}

    public class InternalEscalationFormatter
    {
        private readonly SlaCalculator _slaCalculator;

        public InternalEscalationFormatter(SlaCalculator slaCalculator)
        {
            _slaCalculator = slaCalculator;
        }

        public string DraftInternalEscalationBlurb(SupportTicket ticket)
        {
            var deadline = _slaCalculator.CalculateSlaDeadline(ticket);
            return $"ESCALATE {ticket.Id} priority={ticket.Priority} breachAt={deadline:u} keywords-scanned=yes";
        }
    }
}
