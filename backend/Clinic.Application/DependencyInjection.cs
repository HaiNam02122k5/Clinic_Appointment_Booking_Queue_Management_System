using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Clinic.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Đăng ký toàn bộ Handler (IRequestHandler<...>) trong assembly này.
            // Khi nhóm thêm Command/Query dưới Features/, MediatR sẽ tự tìm thấy,
            // không cần đăng ký thủ công từng handler.
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // TODO: nếu dùng Mapster theo pattern IRegister, quét cấu hình mapping tại đây:
            // TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
