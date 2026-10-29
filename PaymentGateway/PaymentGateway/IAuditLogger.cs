namespace PaymentGateway
{
    public interface IAuditLogger
    {
        void LogTransaction(string transactionId, bool success);
    }
}
