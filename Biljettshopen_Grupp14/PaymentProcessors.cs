using System;

namespace Biljettshopen
{
    public class InvoicePaymentProcessor : IPaymentProcessor
    {
        public void ProcessPayment(int amount)
        {
            Console.WriteLine($"Betalning via faktura har valts. Du kommer att få en faktura på {amount} SEK via e-post.");
        }
    }

    public class DirectPaymentProcessor : IPaymentProcessor
    {
        public void ProcessPayment(int amount)
        {
            Console.WriteLine($"Direktbetalning på {amount} SEK har genomförts. Tack för ditt köp!");
        }
    }
}
