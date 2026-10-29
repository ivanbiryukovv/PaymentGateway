using Moq;
using System;
using PaymentGateway;

namespace PaymentGatewayTests
{
    public class PaymentGatewayTests
    {
        [Fact]
        public void Pay_ValidCardAndSuccessfulPayment_LogsSuccessTrue()
        {
            // Arrange
            var paymentProviderMock = new Mock<IPaymentProvider>();
            paymentProviderMock
                .Setup(p => p.ProcessPayment(It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns(true);

            var auditLoggerMock = new Mock<IAuditLogger>();
            var processor = new PaymentProcessor(paymentProviderMock.Object, auditLoggerMock.Object);

            // Act
            processor.Pay("1234567890123456", 100m);

            // Assert
            paymentProviderMock.Verify(
                p => p.ProcessPayment("1234567890123456", 100m),
                Times.Once);

            auditLoggerMock.Verify(
                l => l.LogTransaction(It.IsAny<string>(), true),
                Times.Once);
        }

        [Fact]
        public void Pay_FailedPayment_ThrowsPaymentFailedException_LogsSuccessFalse()
        {
            // Arrange
            var paymentProviderMock = new Mock<IPaymentProvider>();
            paymentProviderMock
                .Setup(p => p.ProcessPayment("1234567890123456", 50m))
                .Returns(false);

            var auditLoggerMock = new Mock<IAuditLogger>();
            var processor = new PaymentProcessor(paymentProviderMock.Object, auditLoggerMock.Object);

            // Act & Assert
            var ex = Assert.Throws<PaymentFailedException>(
                () => processor.Pay("1234567890123456", 50m));

            Assert.Contains("не удался", ex.Message);

            auditLoggerMock.Verify(
                l => l.LogTransaction(It.IsAny<string>(), false),
                Times.Once);
        }

        [Fact]
        public void Pay_InvalidCardNumber_ThrowsArgumentException_DoesNotCallPaymentProvider()
        {
            // Arrange
            var paymentProviderMock = new Mock<IPaymentProvider>();
            var auditLoggerMock = new Mock<IAuditLogger>();
            var processor = new PaymentProcessor(paymentProviderMock.Object, auditLoggerMock.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => processor.Pay("123", 100m));

            paymentProviderMock.Verify(
                p => p.ProcessPayment(It.IsAny<string>(), It.IsAny<decimal>()),
                Times.Never);

            auditLoggerMock.Verify(
                l => l.LogTransaction(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public void LogTransaction_IsCalled_OnBothSuccessAndFailure()
        {
            // Arrange
            var paymentProviderMock = new Mock<IPaymentProvider>();
            paymentProviderMock
                .Setup(p => p.ProcessPayment(It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns(true);

            var auditLoggerMock = new Mock<IAuditLogger>();
            var processor = new PaymentProcessor(paymentProviderMock.Object, auditLoggerMock.Object);

            // Act
            processor.Pay("1111222233334444", 200m);

            // Assert
            auditLoggerMock.Verify(
                l => l.LogTransaction(It.IsAny<string>(), true),
                Times.Once);


            // Arrange
            var auditLoggerMock2 = new Mock<IAuditLogger>();
            var processor2 = new PaymentProcessor(paymentProviderMock.Object, auditLoggerMock2.Object);

            paymentProviderMock
                .Setup(p => p.ProcessPayment("1111222233334444", 0m))
                .Returns(false);

            // Act & Assert
            Assert.Throws<PaymentFailedException>(
                () => processor2.Pay("1111222233334444", 0m));

            // Assert
            auditLoggerMock2.Verify(
                l => l.LogTransaction(It.IsAny<string>(), false),
                Times.Once);
        }
    }
}