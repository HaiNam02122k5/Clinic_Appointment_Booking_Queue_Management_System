## ADDED Requirements

### Requirement: In-Memory Medical Report Repository Test Double
The testing infrastructure SHALL provide a `FakeMedicalReportRepository` implementing `IMedicalReportRepository` for fast in-memory execution.

#### Scenario: Querying report by ID with ownership
- **WHEN** `GetByIdWithOwnershipAsync` is called with an existing report ID
- **THEN** the fake repository returns the entity with its associated doctor and patient references.

#### Scenario: Querying history by patient ID
- **WHEN** `GetHistoryByPatientIdAsync` is called with a patient ID
- **THEN** all stored medical reports for that patient are returned in descending chronological order.

### Requirement: Configurable Policy Settings Test Double
The testing infrastructure SHALL provide a `FakeAppointmentPolicySettings` implementing `IAppointmentPolicySettings`.

#### Scenario: Configurable minimum notice hours
- **WHEN** `RescheduleMinNoticeHours` is configured in test setup
- **THEN** the value is supplied to the rescheduling handler to simulate varying clinic rules.

### Requirement: Test Data Factory Extensions
The `TestDataFactory` in test projects SHALL support creating consistent domain model fixtures.

#### Scenario: Fixture creation for complex entities
- **WHEN** test data helpers are invoked for `MedicalReport`, `Patient`, `WorkSchedule`, or `Employee`
- **THEN** fully populated, internally consistent domain instances are returned ready for assertions.
