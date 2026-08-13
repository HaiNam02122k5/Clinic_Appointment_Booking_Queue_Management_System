using System;

namespace Clinic.Application.Common.Exceptions
{
    /// <summary>
    /// Ném ra khi request hợp lệ về mặt dữ liệu/quyền nhưng vi phạm một business rule
    /// (vd: đổi lịch quá sát giờ hẹn). Được GlobalExceptionHandler ánh xạ sang HTTP 409,
    /// để phân biệt với ForbiddenException (403 - không có quyền) và ArgumentException (400 - dữ liệu sai).
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}