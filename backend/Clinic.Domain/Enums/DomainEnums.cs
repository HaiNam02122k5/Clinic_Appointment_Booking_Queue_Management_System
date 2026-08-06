namespace Clinic.Domain.Enums
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum EmployeeStatus
    {
        Active,
        OnLeave,
        Resigned
    }

    public enum WorkScheduleStatus
    {
        Active,
        Full,
        Cancelled
    }

    /// <summary>
    /// Pending -> Confirmed -> CheckedIn -> Completed, hoặc Cancelled / NoShow.
    /// </summary>
    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        CheckedIn,
        Completed,
        Cancelled,
        NoShow
    }

    public enum QueueStatus
    {
        Waiting,
        Called,
        InProgress,
        Completed,
        Skipped
    }

    public enum NotificationType
    {
        AppointmentReminder,
        AppointmentConfirmation,
        QueueUpdate,
        System
    }

    public enum NotificationChannel
    {
        Email,
        Sms,
        InApp
    }

    public enum NotificationStatus
    {
        Pending,
        Sent,
        Failed
    }
}
