using System;
using System.Collections.Generic;
using System.Text;

namespace SRP.src.SrpLab
{
    public class DiscountCalculator
    {
        public decimal CalculateDiscount(string? couponRaw, decimal subTotal)
        {
            if (string.IsNullOrWhiteSpace(couponRaw)) return 0m;

            var code = couponRaw.Trim().ToUpperInvariant();

            if (code.StartsWith("SAVE") && int.TryParse(code[4..], out var pct) && pct is > 0 and <= 50)
                return Math.Round(subTotal * pct / 100m, 2);

            if (code.Contains("FREESHIP")) return 0m;

            if (code == "WELCOME10") return Math.Min(10m, subTotal);

            return 0m;
        }
}


    public class GiftWrapFeeCalculator
    {
        private const decimal GiftWrapFee = 4.99m;

        public decimal GetWrapFee(bool isGiftWrapEnabled)
        {
            return isGiftWrapEnabled ? GiftWrapFee : 0m;
        }
}

    public class CheckoutBasket
    {
        private readonly List<(string Sku, decimal Price, int Qty)> _lines = new();

        public IReadOnlyList<(string Sku, decimal Price, int Qty)> Lines => _lines;
        public string? CouponRaw { get; private set; }
        public bool IsGiftWrapEnabled { get; private set; }

        public void AddLine(string sku, decimal price, int qty)
        {
            if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
            _lines.Add((sku, price, qty));
        }

        public void ApplyCouponText(string? couponText) => CouponRaw = couponText;
        public void EnableGiftWrap() => IsGiftWrapEnabled = true;

        public decimal SubTotal() => _lines.Sum(l => l.Price * l.Qty);

        public decimal GrandTotal(DiscountCalculator discountCalculator, GiftWrapFeeCalculator giftWrapFeeCalculator)
        {
            var subTotal = SubTotal();
            var discount = discountCalculator.CalculateDiscount(CouponRaw, subTotal);
            var wrapFee = giftWrapFeeCalculator.GetWrapFee(IsGiftWrapEnabled);

            var total = subTotal - discount + wrapFee;
            return Math.Max(0m, total);
        }
}


    public class GiftCardFormatter
    {
        public string FormatGiftMessageCard(CheckoutBasket basket, decimal grandTotal, string fromName)
        {
            var items = string.Join(", ", basket.Lines.Select(l => l.Sku));
            return $"Dear friend,\nA gift from {fromName} awaits ({items}).\nTotal surprise value: {grandTotal:C}\n";
        }
}

    public class PaymentGatewayStub
    {
        public string AuthorizePayment(CheckoutBasket basket, decimal grandTotal, string cardLast4)
        {
            var payload = $"{grandTotal:0.00}|{cardLast4}|{basket.Lines.Count}";
            var hash = payload.GetHashCode();
            return $"AUTH-{Math.Abs(hash):X8}";
        }
    }
}
