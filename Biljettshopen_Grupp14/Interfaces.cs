namespace Biljettshopen
{
    public interface IPaymentProcessor
    {
        void ProcessPayment(int amount);
    }
}
