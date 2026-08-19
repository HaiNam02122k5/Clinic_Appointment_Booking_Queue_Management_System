## ADDED Requirements

### Requirement: MedicalReport Entity Invariants
The Domain test suite SHALL verify the state transitions, symptom/diagnosis updates, and finalization rules for `MedicalReport`.

#### Scenario: Initialization in Draft status
- **WHEN** a new `MedicalReport` is instantiated
- **THEN** its status MUST be `MedicalReportStatus.Draft`.

#### Scenario: Updating details in Draft status
- **WHEN** symptoms, diagnosis, and prescription are assigned while in Draft status
- **THEN** the fields are updated and the entity reflects the changes.

#### Scenario: Finalizing medical report
- **WHEN** the report status is changed to `Finalized`
- **THEN** it transition to `MedicalReportStatus.Finalized` representing a locked medical record.

### Requirement: Patient Entity Lifecycle
The Domain test suite SHALL verify `Patient` creation and status management.

#### Scenario: Patient creation with Person reference
- **WHEN** a `Patient` entity is created with a valid `Person`
- **THEN** `Patient` retains the `PersonId` reference and initializes with `Active` status.

#### Scenario: Soft deletion of patient
- **WHEN** a patient is deleted
- **THEN** `IsDeleted` is set to true and `UpdatedAt` is recorded.

### Requirement: Employee Entity Management
The Domain test suite SHALL verify `Employee` status transitions and manager assignment.

#### Scenario: Employee status update
- **WHEN** an employee's status is modified to `Inactive` or `Suspended`
- **THEN** the status updates and the entity is marked as updated.

#### Scenario: Manager assignment
- **WHEN** a manager employee is assigned to a subordinate employee
- **THEN** `ManagerId` and `Manager` properties are set accordingly.

### Requirement: Notification Entity State Transitions
The Domain test suite SHALL verify `Notification` dispatch states and retry count tracking.

#### Scenario: Notification created in Pending state
- **WHEN** a notification is created
- **THEN** it initializes with status `Pending`.

#### Scenario: Notification marked as Sent
- **WHEN** a notification is marked as sent
- **THEN** its status changes to `Sent` and `SentAt` timestamp is recorded.

#### Scenario: Notification failure increments retry count
- **WHEN** sending a notification fails
- **THEN** the failure state is tracked for future retry attempts.

### Requirement: QueueCounter Sequence Tracking
The Domain test suite SHALL verify atomic daily queue sequence numbering per doctor.

#### Scenario: Sequential queue number generation
- **WHEN** queue counter is incremented for a doctor on a specific date
- **THEN** the next integer sequence value is generated sequentially.
