using System;

namespace PaymentGateway
{
    public class PaymentProcessor
    {
        private readonly IPaymentProvider _paymentProvider;
        private readonly IAuditLogger _auditLogger;

        public PaymentProcessor(IPaymentProvider paymentProvider, IAuditLogger auditLogger)
        {
            _paymentProvider = paymentProvider ?? throw new ArgumentNullException(nameof(paymentProvider));
            _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        }

        public void Pay(string cardNumber, decimal amount)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length != 16)
                throw new ArgumentException("Номер карты должен содержать 16 символов.", nameof(cardNumber));

            string transactionId = Guid.NewGuid().ToString();

            bool success = _paymentProvider.ProcessPayment(cardNumber, amount);

            _auditLogger.LogTransaction(transactionId, success);

            if (!success)
                throw new PaymentFailedException($"Платёж на сумму {amount} по карте {cardNumber} не удался.");
        }
    }
}