## Requirements

### Requirement: Appointment Booking Handler Verification
The Application test suite SHALL verify that `CreateAppointmentHandler` properly validates patient permissions, shift status, schedule capacity, and transaction rollback on failure.

#### Scenario: Successful appointment creation within capacity
- **WHEN** a patient requests an available future time slot in an active work schedule with remaining capacity
- **THEN** the handler commits the transaction and returns the new appointment ID.

#### Scenario: Rejection when capacity limit reached
- **WHEN** a patient attempts to book a slot where active appointments equal `PatientLimitPerSlot`
- **THEN** the handler rolls back the transaction and throws a `ConflictException`.

#### Scenario: Rejection for non-patient users
- **WHEN** a user without a `PatientId` attempts to create an appointment
- **THEN** the handler throws a `ForbiddenException`.

#### Scenario: Rejection for past time slots
- **WHEN** a booking request contains a time slot in the past
- **THEN** the handler throws an `ArgumentException`.

### Requirement: Appointment Rescheduling Handler Verification
The Application test suite SHALL verify that `RescheduleAppointmentHandler` enforces advance notice policies, capacity checks, and authorization rules.

#### Scenario: Patient reschedules before notice deadline
- **WHEN** a patient requests to move their appointment to an available slot before `RescheduleMinNoticeHours`
- **THEN** the handler updates the appointment slot and commits the transaction.

#### Scenario: Patient rescheduling within forbidden notice window
- **WHEN** a patient attempts to reschedule an appointment less than `RescheduleMinNoticeHours` before the appointment time
- **THEN** the handler throws a `ConflictException`.

#### Scenario: Staff bypasses notice window
- **WHEN** a staff user with `appointment.update` permission reschedules an appointment on behalf of a patient
- **THEN** the handler permits the reschedule regardless of the notice window.

### Requirement: Available Slots Query Verification
The Application test suite SHALL verify that `GetAvailableSlotsQueryHandler` filters shifts by doctor, specialty, date range, and remaining capacity.

#### Scenario: Filtering slots by doctor and date range
- **WHEN** a query requests slots for a specific doctor within a future date range
- **THEN** only active future work schedules of that doctor are returned with correct `RemainingCapacity`.

#### Scenario: Fully booked slots excluded
- **WHEN** a work schedule has reached its `PatientLimitPerSlot`
- **THEN** it is excluded from the available slots result list.

### Requirement: Queue Status Query Verification
The Application test suite SHALL verify that `GetMyQueueStatusHandler` accurately calculates queue position and estimated waiting time.

#### Scenario: Checked-in patient views queue status
- **WHEN** a patient with an active waiting ticket queries queue status
- **THEN** the system returns their ticket with `PositionInQueue` and `EstimatedWaitMinutes` calculated as 10 minutes per person ahead.

#### Scenario: Patient with no active tickets today
- **WHEN** a patient who has not checked in today queries queue status
- **THEN** the system returns an empty list.

### Requirement: Medical History Query Verification
The Application test suite SHALL verify that `GetMyMedicalHistoryHandler` returns only finalized medical records belonging to the authenticated patient.

#### Scenario: Patient retrieves finalized medical reports
- **WHEN** a patient queries their medical history
- **THEN** only records with status `Finalized` are returned.

#### Scenario: Draft reports are hidden from patient
- **WHEN** a medical report is in `Draft` status
- **THEN** it is excluded from the patient's medical history response.

### Requirement: Administrative & Personnel Use Case Verification
The Application test suite SHALL verify account and personnel lifecycle handlers for Doctors, Employees, Specialties, and Users.

#### Scenario: Create doctor from existing user
- **WHEN** an administrator creates a doctor profile for an existing user
- **THEN** the user is assigned the Doctor role and a Doctor entity with the selected specialty is persisted.

#### Scenario: Retrieve paged users
- **WHEN** an administrator queries paged users with search and sorting parameters
- **THEN** a paginated response with the requested page size and items is returned.
