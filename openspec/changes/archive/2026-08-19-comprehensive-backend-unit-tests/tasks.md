## 1. Test Doubles and Helpers

- [x] 1.1 Implement `FakeMedicalReportRepository` in `Clinic.Application.UnitTests/Common/FakeMedicalReportRepository.cs`
- [x] 1.2 Implement `FakeAppointmentPolicySettings` in `Clinic.Application.UnitTests/Common/FakeAppointmentPolicySettings.cs`
- [x] 1.3 Extend `TestDataFactory` in `Clinic.Application.UnitTests/Common/TestDataFactory.cs` with factory methods for Patient, MedicalReport, WorkSchedule, and Employee fixtures

## 2. Appointment & Scheduling Application Unit Tests

- [x] 2.1 Implement `CreateAppointmentTests.cs` in `Clinic.Application.UnitTests/Features/Appointments/Commands/` (valid booking, capacity check, past date rejection, non-patient forbidden, transaction rollback)
- [x] 2.2 Implement `RescheduleAppointmentTests.cs` in `Clinic.Application.UnitTests/Features/Appointments/Commands/` (advance notice check, staff bypass, new schedule capacity check, own vs other patient permissions)
- [x] 2.3 Implement `GetMyAppointmentsTests.cs` in `Clinic.Application.UnitTests/Features/Appointments/Queries/` (patient appointment retrieval and sorting)
- [x] 2.4 Implement `GetAvailableSlotsTests.cs` in `Clinic.Application.UnitTests/Features/Slots/Queries/` (filtering by doctor/specialty/date, remaining capacity calculation, fully booked exclusion)

## 3. Queue & Medical Reports Application Unit Tests

- [x] 3.1 Implement `GetMyQueueStatusTests.cs` in `Clinic.Application.UnitTests/Features/Queue/Queries/` (queue position, estimated waiting time calculation, priority order, empty day queue)
- [x] 3.2 Implement `GetMyMedicalHistoryTests.cs` in `Clinic.Application.UnitTests/Features/MedicalReports/Queries/` (patient medical records retrieval, finalized status filtering, non-patient forbidden)

## 4. Personnel & User Management Application Unit Tests

- [x] 4.1 Implement `DoctorCommandsAndQueriesTests.cs` in `Clinic.Application.UnitTests/Features/Doctors/Commands/` (`CreateDoctorFromUserHandler`, `UpdateDoctorStatusHandler`)
- [x] 4.2 Implement `EmployeeCommandsAndQueriesTests.cs` in `Clinic.Application.UnitTests/Features/Employees/Commands/` (`CreateEmployeeFromUserHandler`, `UpdateEmployeeStatusHandler`)
- [x] 4.3 Implement `SpecialtyQueriesTests.cs` in `Clinic.Application.UnitTests/Features/Specialties/Queries/` (`GetSpecialtyQueryHandler` by ID)
- [x] 4.4 Implement `UserQueriesTests.cs` in `Clinic.Application.UnitTests/Features/Users/Queries/` (`GetAllUsersQueryHandler`, `GetUserByIdQueryHandler`, `GetPagedUsersQueryHandler`)

## 5. Domain Layer Unit Tests

- [x] 5.1 Implement `MedicalReportTests.cs` in `Clinic.Domain.UnitTests/` (Draft initialization, field updates, Finalized state locking, exam start/end times)
- [x] 5.2 Implement `PatientTests.cs` in `Clinic.Domain.UnitTests/` (creation with Person, soft-deletion)
- [x] 5.3 Implement `EmployeeTests.cs` in `Clinic.Domain.UnitTests/` (initialization, status updates, manager assignment)
- [x] 5.4 Implement `NotificationTests.cs` in `Clinic.Domain.UnitTests/` (Pending state, MarkSent, MarkFailed and retry counter)
- [x] 5.5 Implement `QueueCounterTests.cs` in `Clinic.Domain.UnitTests/` (sequential queue numbering per doctor/date)

## 6. Verification & Test Suite Execution

- [x] 6.1 Execute `dotnet test backend/Clinic.slnx` and verify that all test suites pass with 0 failures and 0 skipped tests
