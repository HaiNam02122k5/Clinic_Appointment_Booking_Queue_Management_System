using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Features.WorkSchedules.Commands
{
    // Use-case: Admin cancels a work schedule for a doctor, with a reason for cancellation to notify the doctor and patients
    public record CancelWorkScheduleCommand(
        Guid UserId,
        Guid Id,
        string Reason
    ) : IRequest<Guid>;
    public class CancelWorkScheduleCommandHandler : IRequestHandler<CancelWorkScheduleCommand, Guid>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CancelWorkScheduleCommandHandler(IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CancelWorkScheduleCommand request, CancellationToken cancellationToken)
        {
            var workSchedule = await _workScheduleRepository.GetWorkScheduleByIdAsync(request.Id);
            if (workSchedule == null) throw new NotFoundException("Work schedule not found.");

            workSchedule.Cancel(request.Reason);
            foreach (var appointment in workSchedule.Appointments)
            {
                appointment.AdminCancel(request.UserId);
                // TODO: notify patients and doctor
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return workSchedule.Id;
        }
    }
}
