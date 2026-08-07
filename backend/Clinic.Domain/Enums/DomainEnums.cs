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

    /// <summary>Trạng thái 1 giai đoạn trong lịch sử chuyển khoa của bác sĩ.</summary>
    public enum WorkHistoryStatus
    {
        Active,
        Ended
    }

    /// <summary>Trạng thái duyệt yêu cầu ca làm việc của bác sĩ.</summary>
    public enum ShiftRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }
    public enum MedicalReportStatus 
    { 
        Draft, 
        Finalized 
    }
}
