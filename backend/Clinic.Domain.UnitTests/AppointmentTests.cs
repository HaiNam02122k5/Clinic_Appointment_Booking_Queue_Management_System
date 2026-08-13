using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.UnitTests
{
    public class AppointmentTests
    {
        [Fact]
        public void TestCreateAppointment()
        {
            var doctorPatient = new Person("Doctor Name", "0987654321", "doctor@example.com", new DateOnly(1980, 1, 1), Enums.Gender.Male, "");
            var patientPerson = new Person("Patient Name", "1234567890", "patient@example.com", new DateOnly(1990, 1, 1), Enums.Gender.Male, "");
            var employee = new Employee(doctorPatient, new DateOnly(2023, 1, 1));
            var specialty = new Specialty("Cardiology", "Heart specialist", new DateOnly(2020, 1, 1));
            var doctor = new Doctor(employee, "123ABC", "MD", specialty);
            var workSchedule = new WorkSchedule(doctor, new DateOnly(2027, 1, 1), new TimeOnly(7, 0), new TimeOnly(11, 0), 20);
            var patient = new Patient
            {
                Person = patientPerson,
                PersonId = patientPerson.Id,
                InsuranceNumber = "INS123456",
                EmergencyContact = "John Doe - 0987654321"
            };

            var appointment = new Appointment(patient, workSchedule, new TimeOnly(7, 0), "Reason", Guid.NewGuid());
            var appointment2 = new Appointment(patient, workSchedule, new TimeOnly(7, 15), "Reason2", Guid.NewGuid(), true);

            Assert.Throws<ArgumentNullException>(() => new Appointment(null, workSchedule, new TimeOnly(7, 0), "Reason", Guid.NewGuid()));
            Assert.Throws<ArgumentNullException>(() => new Appointment(patient, null, new TimeOnly(7, 0), "Reason", Guid.NewGuid()));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Appointment(patient, workSchedule, new TimeOnly(6, 0), "Reason", Guid.NewGuid()));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Appointment(patient, workSchedule, new TimeOnly(12, 0), "Reason", Guid.NewGuid()));
        }

        [Fact]
        public void TestUpdateAppointment()
        {
            var doctorPatient = new Person("Doctor Name", "0987654321", "doctor@example.com", new DateOnly(1980, 1, 1), Enums.Gender.Male, "");
            var patientPerson = new Person("Patient Name", "1234567890", "patient@example.com", new DateOnly(1990, 1, 1), Enums.Gender.Male, "");
            var employee = new Employee(doctorPatient, new DateOnly(2023, 1, 1));
            var specialty = new Specialty("Cardiology", "Heart specialist", new DateOnly(2020, 1, 1));
            var doctor = new Doctor(employee, "123ABC", "MD", specialty);
            var workSchedule = new WorkSchedule(doctor, new DateOnly(2027, 1, 1), new TimeOnly(7, 0), new TimeOnly(11, 0), 20);
            var patient = new Patient
            {
                Person = patientPerson,
                PersonId = patientPerson.Id,
                InsuranceNumber = "INS123456",
                EmergencyContact = "John Doe - 0987654321"
            };

            var appointment = new Appointment(patient, workSchedule, new TimeOnly(7, 0), "Reason", Guid.NewGuid());
            appointment.Update(workSchedule, new TimeOnly(7, 15), "Reason", Guid.NewGuid());
            Assert.Equal(new TimeOnly(7, 15), appointment.TimeSlot);
            Assert.Throws<ArgumentOutOfRangeException>(() => appointment.Update(workSchedule, new TimeOnly(6, 0), "Reason", Guid.NewGuid()));
            Assert.Throws<ArgumentOutOfRangeException>(() => appointment.Update(workSchedule, new TimeOnly(12, 0), "Reason", Guid.NewGuid()));
            Assert.Throws<ArgumentNullException>(() => appointment.Update(null, new TimeOnly(7, 15), "Reason", Guid.NewGuid()));
            appointment.UpdateStatus(AppointmentStatus.Completed, Guid.NewGuid());
            Assert.Equal(AppointmentStatus.Completed, appointment.Status);
            Assert.Throws<InvalidOperationException>(() => appointment.Update(workSchedule, new TimeOnly(7, 30), "Reason", Guid.NewGuid()));
            Assert.Throws<InvalidOperationException>(() => appointment.UpdateStatus(AppointmentStatus.Cancelled, Guid.NewGuid()));
        }

        [Fact]
        public void TestCancelAppointment()
        {
            var doctorPatient = new Person("Doctor Name", "0987654321", "doctor@example.com", new DateOnly(1980, 1, 1), Enums.Gender.Male, "");
            var patientPerson = new Person("Patient Name", "1234567890", "patient@example.com", new DateOnly(1990, 1, 1), Enums.Gender.Male, "");
            var employee = new Employee(doctorPatient, new DateOnly(2023, 1, 1));
            var specialty = new Specialty("Cardiology", "Heart specialist", new DateOnly(2020, 1, 1));
            var doctor = new Doctor(employee, "123ABC", "MD", specialty);
            var now = DateTime.UtcNow.AddHours(7);
            if (now.Hour >= 20)
            {
                now = now.AddHours(12);
            }
            var workSchedule = new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(1)), TimeOnly.FromDateTime(now.AddHours(3)), 20);
            var patient = new Patient
            {
                Person = patientPerson,
                PersonId = patientPerson.Id,
                InsuranceNumber = "INS123456",
                EmergencyContact = "John Doe - 0987654321"
            };

            var appointment = new Appointment(patient, workSchedule, TimeOnly.FromDateTime(now.AddHours(2).AddMinutes(30)), "Reason", Guid.NewGuid());
            appointment.Cancel(Guid.NewGuid());
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);

            // Try to cancel an appointment that is within the cancellation limit (less than 2 hours before the appointment time)
            now = DateTime.UtcNow.AddHours(7);
            var workSchedule2 = new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddSeconds(1)), TimeOnly.FromDateTime(now.AddMinutes(30)), 20);
            var appointment2 = new Appointment(patient, workSchedule2, TimeOnly.FromDateTime(now.AddMinutes(15)), "Reason", Guid.NewGuid());
            Assert.Throws<InvalidOperationException>(() => appointment2.Cancel(Guid.NewGuid()));
        }
    }
}
