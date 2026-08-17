using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Notifications.Interfaces
{
    public interface IAppointmentHandler
    {
        Task HandleAsync(NotificationJob<Appointment> appointmentJob, CancellationToken cancellationToken);
    }
}
