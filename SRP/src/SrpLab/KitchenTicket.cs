using SRP.src.SrpLab;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRP.src.SrpLab
{
    public class AllergenDetector
    {
        public IReadOnlyList<string> DetectAllergens(IEnumerable<List<string>> itemIngredientsList)
        {
            var hits = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var ingredients in itemIngredientsList)
            {
                foreach (var ing in ingredients)
                {
                    if (ing.Contains("milk") || ing.Contains("cheese") || ing.Contains("butter")) hits.Add("dairy");
                    if (ing.Contains("wheat") || ing.Contains("flour") || ing.Contains("bread")) hits.Add("gluten");
                    if (ing.Contains("peanut") || ing.Contains("almond") || ing.Contains("cashew")) hits.Add("nuts");
                    if (ing.Contains("shrimp") || ing.Contains("prawn") || ing.Contains("crab")) hits.Add("shellfish");
                }
            }

            return hits.OrderBy(x => x).ToList();
        }
}


    public class KitchenTimingCalculator
    {
        public int EstimatedReadyMinutes(KitchenTicket ticket, AllergenDetector allergenDetector, int openStations)
        {
            if (openStations <= 0) openStations = 1;

            var sequential = ticket.Items.Sum(i => i.PrepMinutes);
            var parallel = (int)Math.Ceiling(sequential / (double)openStations);

            var allergens = allergenDetector.DetectAllergens(ticket.Items.Select(i => i.Ingredients));
            if (allergens.Count > 0) parallel += 3;

            var longest = ticket.Items.Count == 0 ? 0 : ticket.Items.Max(i => i.PrepMinutes);
            return Math.Max(parallel, longest);
        }
}


    public class KitchenTicket
    {
        private readonly List<(string Item, List<string> Ingredients, int PrepMinutes)> _items = new();

        public IReadOnlyList<(string Item, List<string> Ingredients, int PrepMinutes)> Items => _items;

        public void AddItem(string item, IEnumerable<string> ingredients, int prepMinutes)
        {
            _items.Add((item, ingredients.Select(i => i.Trim().ToLowerInvariant()).ToList(), prepMinutes));
        }
}

    public class ThermalTicketPrinter
    {
        private readonly AllergenDetector _allergenDetector;
        private readonly KitchenTimingCalculator _timingCalculator;

        public ThermalTicketPrinter(AllergenDetector allergenDetector, KitchenTimingCalculator timingCalculator)
        {
            _allergenDetector = allergenDetector;
            _timingCalculator = timingCalculator;
        }

        public string RenderThermalTicket(KitchenTicket ticket, int orderNumber)
        {
            var width = 32;
            var line = new string('=', width);
            var body = string.Join('\n', ticket.Items.Select(i => $"* {i.Item.ToUpperInvariant()} ({i.PrepMinutes}m)"));

            var allergens = _allergenDetector.DetectAllergens(ticket.Items.Select(i => i.Ingredients));
            var allergyLine = allergens.Count == 0 ? "ALLERGENS: none" : "ALLERGENS: " + string.Join(",", allergens);

            var eta = _timingCalculator.EstimatedReadyMinutes(ticket, _allergenDetector, 2);

            return $"{line}\nORDER #{orderNumber}\nETA {eta} MIN\n{body}\n{allergyLine}\n{line}\n";
        }
}

    public class ExpoLaneRouting
    {
        private readonly AllergenDetector _allergenDetector;
        private readonly KitchenTimingCalculator _timingCalculator;

        public ExpoLaneRouting(AllergenDetector allergenDetector, KitchenTimingCalculator timingCalculator)
        {
            _allergenDetector = allergenDetector;
            _timingCalculator = timingCalculator;
        }

        public string ExpoLaneHint(KitchenTicket ticket)
        {
            var allergens = _allergenDetector.DetectAllergens(ticket.Items.Select(i => i.Ingredients));
            if (allergens.Count > 0) return "LANE-ALLERGY";

            var eta = _timingCalculator.EstimatedReadyMinutes(ticket, _allergenDetector, 2);
            return eta > 20 ? "LANE-SLOW" : "LANE-FAST";
        }
    }
}
