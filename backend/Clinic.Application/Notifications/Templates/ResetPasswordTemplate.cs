using Clinic.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Notifications.Templates
{
    public class ResetPasswordTemplate
    {
        public EmailContent RenderEmail(Account info)
        {
            var body =
                $"""
                <!DOCTYPE html>
                <html>
                <body style="font-family: Arial, sans-serif;">
                    <h2 style="color: #333;">Đặt lại mật khẩu</h2>

                    <p>Xin chào {info.FullName},</p>

                    <p>Chúng tôi vừa nhận được yêu cầu đặt lại mật khẩu của bạn.</p>

                    <p>
                        Mật khẩu mới của bạn là: <strong>{info.Password}</strong>
                    </p>

                    <p>Trân trọng,<br>Phòng khám của chúng tôi</p>
                </body>
                </html>
                """;
            return new EmailContent("Đặt lại mật khẩu", body);
        }

        public SmsContent RenderSms(Account info)
        {
            var message = $"Mật khẩu mới của bạn là: {info.Password}";
            return new SmsContent(message);
        }
    }
}
