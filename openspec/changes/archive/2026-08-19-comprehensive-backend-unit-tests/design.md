## Context

Hệ thống Backend được xây dựng theo kiến trúc Clean Architecture trên nền .NET 10, phân tách thành 4 project chính:
1. `Clinic.Domain`: Chứa các thực thể cốt lõi (`Appointment`, `Doctor`, `Employee`, `MedicalReport`, `Notification`, `Patient`, `Person`, `QueueTicket`, `QueueCounter`, `Role`, `ShiftRequest`, `Specialty`, `User`, `WorkHistory`, `WorkSchedule`).
2. `Clinic.Application`: Chứa các use case dạng CQRS (MediatR Handlers), DTOs, Exceptions, Interfaces.
3. `Clinic.Infrastructure.Sqlserver`: Triển khai EF Core DbContext, Repositories, Migrations, JWT Token Provider, Password Hasher.
4. `Clinic.API`: Controllers, Authorization Policies, Middlewares, Dependency Injection.

Hiện tại hệ thống có 2 project test: `Clinic.Domain.UnitTests` và `Clinic.Application.UnitTests`. Các test đã có đạt 204 tests pass. Tuy nhiên, nhiều use case trọng tâm ở tầng Application và nhiều Domain Entity quan trọng vẫn chưa được viết test.

## Goals / Non-Goals

**Goals:**
- Bổ sung trọn vẹn các test doubles (`FakeMedicalReportRepository`, `FakeAppointmentPolicySettings`, mở rộng `TestDataFactory`).
- Triển khai unit tests bao phủ 100% các use case (Commands/Queries) còn thiếu trong `Clinic.Application.UnitTests`:
  - `CreateAppointmentHandler`, `RescheduleAppointmentHandler`, `GetMyAppointmentsQueryHandler`.
  - `GetAvailableSlotsQueryHandler`.
  - `GetMyQueueStatusHandler`.
  - `GetMyMedicalHistoryQueryHandler`.
  - `Doctor` & `Employee` management handlers (`CreateDoctorFromUser`, `UpdateDoctorStatus`, `CreateEmployeeFromUser`, `UpdateEmployeeStatus`).
  - `Specialty` & `User` query handlers (`GetSpecialty`, `GetAllUsers`, `GetUserById`, `GetPagedUsers`).
- Triển khai unit tests bao phủ các Domain Entity chưa có test trong `Clinic.Domain.UnitTests`:
  - `MedicalReportTests`, `PatientTests`, `EmployeeTests`, `NotificationTests`, `QueueCounterTests`.
- Đảm bảo tất cả 260+ tests chạy độc lập trong bộ nhớ (in-memory test execution) siêu nhanh (< 2s), không phụ thuộc database thật, không phụ thuộc I/O mạng.

**Non-Goals:**
- Không sửa đổi mã nguồn Frontend trong phạm vi change này.
- Không thay đổi contracts/APIs của Backend production code.

## Decisions

1. **Sử dụng Custom Fakes (Test Doubles) thay vì Heavy Mocking Frameworks**:
   - *Rationale*: Dự án đã có sẵn phong cách viết Fake Repositories in-memory (`FakeAppointmentRepository`, `FakeWorkScheduleRepository`, `FakeUserRepository`, v.v.). Phong cách này giúp test rõ ràng, dễ bảo trì, dễ debug luồng logic so với mock phức tạp.
   - *Alternatives considered*: Dùng Moq hoặc NSubstitute. Bị loại vì không đồng nhất với codebase test hiện có.

2. **Tách bạch rõ rệt giữa Domain Unit Tests và Application Unit Tests**:
   - `Clinic.Domain.UnitTests`: Chỉ kiểm tra logic nghiệp vụ thuần túy, tính đóng gói (encapsulation), bất biến dữ liệu, ném ngoại lệ khi vi phạm rule.
   - `Clinic.Application.UnitTests`: Kiểm tra luồng điều phối use-case, phân quyền (RBAC permissions), kiểm tra tồn tại qua Repositories, quản lý Transaction (UnitOfWork Commit/Rollback), mapping DTO.

3. **Mô phỏng Transaction & Concurrency Lock trong In-Memory Fakes**:
   - `FakeUnitOfWork` theo dõi trạng thái `TransactionOpen`, `WasCommitted`, `WasRolledBack` để kiểm chứng handler xử lý rollback đúng cách khi gặp lỗi.
   - `FakeWorkScheduleRepository.LockAsync` hoạt động như một Task no-op an toàn để các handler có concurrency locking (`CreateAppointmentHandler`, `RescheduleAppointmentHandler`) thực thi mượt mà trong unit test.

## Risks / Trade-offs

- **[Risk]** Sự sai khác giữa logic LINQ in-memory của Fake Repository và EF Core Provider thật.
  - **Mitigation**: Thiết kế Fake Repository bám sát hành vi của query trong EF Core (vd: `GetAvailableSlotsAsync` lọc bỏ slot quá hạn, tính count appointment active).
- **[Risk]** Giả lập `DateTime.UtcNow` có thể gây flaky test nếu ca làm việc được thiết lập quá sát giờ chạy test.
  - **Mitigation**: Trong `TestDataFactory`, luôn tạo các mốc thời gian tương lai có biên an toàn (vd: `DateTime.UtcNow.AddDays(1)`).
