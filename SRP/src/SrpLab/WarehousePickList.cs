using SRP.src.SrpLab;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRP.src.SrpLab
{
    public class PickingPathOptimizer
    {
        public IReadOnlyList<(string Aisle, int Bin, string Sku, int Qty)> CalculateWalkingOrder(WarehousePickList pickList)
        {
            return pickList.Lines
                .OrderBy(l => l.Aisle)
                .ThenBy(l => l.Bin)
                .Select(l => (l.Aisle, l.Bin, l.Sku, Qty: Math.Min(l.QtyNeeded, l.QtyOnHand)))
                .Where(x => x.Qty > 0)
                .ToList();
        }
    }


    public class WarehousePickList
    {
        private readonly List<(string Sku, string Aisle, int Bin, int QtyNeeded, int QtyOnHand)> _lines = new();

        public IReadOnlyList<(string Sku, string Aisle, int Bin, int QtyNeeded, int QtyOnHand)> Lines => _lines;

        public void AddNeed(string sku, string aisle, int bin, int qtyNeeded, int qtyOnHand)
        {
            _lines.Add((sku, aisle, bin, qtyNeeded, qtyOnHand));
        }

        public IReadOnlyList<(string Sku, int Allocated)> Allocate()
        {
            var result = new List<(string, int)>();
            foreach (var line in _lines)
            {
                var alloc = Math.Min(line.QtyNeeded, line.QtyOnHand);
                result.Add((line.Sku, alloc));
            }
            return result;
        }
    }

    public class PickerScriptFormatter
    {
        private readonly PickingPathOptimizer _pathOptimizer;

        public PickerScriptFormatter(PickingPathOptimizer pathOptimizer)
        {
            _pathOptimizer = pathOptimizer;
        }

        public string BuildPickerScript(WarehousePickList pickList)
        {
            var steps = _pathOptimizer.CalculateWalkingOrder(pickList)
                .Select((s, i) => $"{i + 1}. Go aisle {s.Aisle} bin {s.Bin}: pick {s.Qty} × {s.Sku}");

            var allocations = pickList.Allocate();
            var shortfalls = allocations.Where(a =>
            {
                var need = pickList.Lines.First(l => l.Sku == a.Sku).QtyNeeded;
                return a.Allocated < need;
            });

            var warn = shortfalls.Any()
                ? "SHORTAGES: " + string.Join(", ", shortfalls.Select(s => s.Sku))
                : "SHORTAGES: none";

            return string.Join('\n', steps) + "\n" + warn;
        }
    }


    public class WmsXmlExporter
    {
        public string ExportWmsXmlBatch(WarehousePickList pickList, string batchId)
        {
            var parts = pickList.Allocate().Select(a => $"<line sku=\"{a.Sku}\" qty=\"{a.Allocated}\" />");
            return $"<batch id=\"{batchId}\">{string.Join("", parts)}</batch>";
        }
    }
}
