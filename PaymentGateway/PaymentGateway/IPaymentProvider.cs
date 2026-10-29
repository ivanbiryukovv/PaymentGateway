namespace PaymentGateway
{
    public interface IPaymentProvider
    {
        bool ProcessPayment(string cardNumber, decimal amount);
    }
}
