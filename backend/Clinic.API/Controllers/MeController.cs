using Clinic.Application.Features.MedicalReports.Queries;
using Clinic.Application.Features.Queue.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    /// <summary>
    /// Các API tự-phục-vụ (self-service) cho bệnh nhân đang đăng nhập: theo dõi vị trí hàng đợi
    /// và tra cứu lịch sử khám bệnh của chính mình.
    /// </summary>
    [ApiController]
    [Route("/me")]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly ISender _sender;

        public MeController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Trạng thái hàng đợi hiện tại của bệnh nhân (vị trí, thời gian chờ ước tính).
        /// Trả về 204 nếu bệnh nhân chưa check-in trong ngày hôm nay.
        /// </summary>
        [HttpGet("queue-status")]
        public async Task<IActionResult> GetMyQueueStatus()
        {
            var result = await _sender.Send(new GetMyQueueStatusQuery());
            return result is null ? NoContent() : Ok(result);
        }

        /// <summary>Lịch sử khám bệnh (các hồ sơ đã chốt) của bệnh nhân đang đăng nhập.</summary>
        [HttpGet("medical-history")]
        public async Task<IActionResult> GetMyMedicalHistory()
        {
            var result = await _sender.Send(new GetMyMedicalHistoryQuery());
            return Ok(result);
        }
    }
}