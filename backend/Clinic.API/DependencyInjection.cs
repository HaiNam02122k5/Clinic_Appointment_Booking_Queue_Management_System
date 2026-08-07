using Microsoft.Extensions.DependencyInjection;

namespace Clinic.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddControllers();

            // TODO: đăng ký GlobalExceptionHandler (IExceptionHandler) khi nhóm
            // triển khai chuẩn hóa response lỗi qua envelope ApiResponse - Program.cs
            // đã gọi sẵn app.UseExceptionHandler() để chờ đăng ký handler này.
            // services.AddExceptionHandler<GlobalExceptionHandler>();
            // services.AddProblemDetails();

            return services;
        }
    }
}
