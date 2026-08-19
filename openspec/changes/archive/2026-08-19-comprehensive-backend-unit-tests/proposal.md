## Why

Backend hiện tại của hệ thống Đặt lịch & Quản lý hàng đợi phòng khám (.NET 10 Clean Architecture) đã triển khai 204 unit tests, tuy nhiên các luồng nghiệp vụ use-case quan trọng nhất ở tầng Application (`CreateAppointment`, `RescheduleAppointment`, `GetAvailableSlots`, `GetMyQueueStatus`, `GetMyAppointments`, `GetMyMedicalHistory`, `UserQueries`, `Doctor/Employee` management) và các Domain Entities (`MedicalReport`, `Patient`, `Employee`, `Notification`, `QueueCounter`) vẫn chưa có test suite bảo vệ. Việc bổ sung toàn diện bộ unit test này giúp đảm bảo độ tin cậy của hệ thống, phòng chống hồi quy (regression) và đáp ứng tiêu chí đánh giá chất lượng mã nguồn của đề tài.

## What Changes

- **Bổ sung Test Doubles & Common Test Helpers (`Clinic.Application.UnitTests/Common/`)**:
  - Tạo mới `FakeMedicalReportRepository` để mock các thao tác đọc/lấy lịch sử bệnh án.
  - Tạo mới `FakeAppointmentPolicySettings` để cấu hình tham số thời hạn đổi lịch hẹn (`RescheduleMinNoticeHours`).
  - Mở rộng `TestDataFactory` để sinh dữ liệu mẫu cho `MedicalReport`, `Patient`, `WorkSchedule`, `Doctor`, `Employee`.
- **Bổ sung Application Layer Unit Tests (`Clinic.Application.UnitTests/Features/`)**:
  - `Appointments`: Thêm `CreateAppointmentTests.cs`, `RescheduleAppointmentTests.cs`, `GetMyAppointmentsTests.cs`.
  - `Slots`: Thêm `GetAvailableSlotsTests.cs`.
  - `Queue`: Thêm `GetMyQueueStatusTests.cs`.
  - `MedicalReports`: Thêm `GetMyMedicalHistoryTests.cs`.
  - `Doctors` & `Employees`: Thêm `DoctorCommandsAndQueriesTests.cs`, `EmployeeCommandsAndQueriesTests.cs`.
  - `Specialties` & `Users`: Thêm `SpecialtyQueriesTests.cs`, `UserQueriesTests.cs`.
- **Bổ sung Domain Layer Unit Tests (`Clinic.Domain.UnitTests/`)**:
  - Thêm `MedicalReportTests.cs`, `PatientTests.cs`, `EmployeeTests.cs`, `NotificationTests.cs`, `QueueCounterTests.cs`.

## Capabilities

### New Capabilities
- `backend-application-unit-tests`: Bộ kiểm thử đơn vị cho toàn bộ các use-case (Commands/Queries) quan trọng trong tầng `Clinic.Application` (đặt lịch, đổi lịch, tính hàng đợi, lọc slot trống, tra cứu lịch sử khám bệnh, quản lý tài khoản/nhân sự).
- `backend-domain-unit-tests`: Bộ kiểm thử đơn vị kiểm chứng các quy tắc bất biến (invariants) và máy trạng thái (state machines) của các thực thể trong tầng `Clinic.Domain` (`MedicalReport`, `Patient`, `Employee`, `Notification`, `QueueCounter`).
- `backend-test-fakes`: Hệ thống Test Doubles và Fixture Factories cung cấp môi trường mock in-memory nhất quán cho tầng kiểm thử.

### Modified Capabilities
<!-- No modified requirement capabilities -->

## Impact

- **Affected Projects**:
  - `backend/Clinic.Application.UnitTests/`
  - `backend/Clinic.Domain.UnitTests/`
- **APIs & Dependencies**: Không làm thay đổi API contract hay dependencies production; chỉ thêm file kiểm thử và test double.
- **Verification**: Chạy `dotnet test backend/Clinic.slnx` đảm bảo toàn bộ test suites đều pass 100%.
