namespace PaymentGateway
{
    public class PaymentFailedException : Exception
    {
        public PaymentFailedException(string message) : base(message) { }
        public PaymentFailedException(string message, Exception inner) : base(message, inner) { }
    }
}
