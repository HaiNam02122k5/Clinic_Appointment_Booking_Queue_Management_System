using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.UnitTests.Common;

namespace Clinic.Domain.UnitTests
{
    public class NotificationTests
    {
        [Fact]
        public void CreateNotification_ShouldInitializePending()
        {
            // Arrange
            var person = TestDataFactory.CreatePerson();

            // Act
            var notification = new Notification
            {
                PersonId = person.Id,
                Person = person,
                Type = NotificationType.AppointmentConfirmation,
                Channel = NotificationChannel.Email,
                Message = "Your appointment is confirmed."
            };

            // Assert
            Assert.Equal(NotificationStatus.Pending, notification.Status);
            Assert.Equal(NotificationType.AppointmentConfirmation, notification.Type);
            Assert.Equal(NotificationChannel.Email, notification.Channel);
            Assert.Null(notification.SendTime);
        }

        [Fact]
        public void MarkSent_ShouldUpdateStatusAndSendTime()
        {
            // Arrange
            var person = TestDataFactory.CreatePerson();
            var notification = new Notification
            {
                PersonId = person.Id,
                Person = person,
                Type = NotificationType.AppointmentReminder,
                Channel = NotificationChannel.Sms,
                Message = "Reminder: Your appointment is in 1 hour."
            };

            // Act
            notification.Status = NotificationStatus.Sent;
            notification.SendTime = DateTime.UtcNow;

            // Assert
            Assert.Equal(NotificationStatus.Sent, notification.Status);
            Assert.NotNull(notification.SendTime);
        }
    }
}
