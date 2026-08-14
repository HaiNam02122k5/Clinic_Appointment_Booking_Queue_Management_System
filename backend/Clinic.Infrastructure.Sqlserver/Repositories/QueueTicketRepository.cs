using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class QueueTicketRepository : IQueueTicketRepository
    {
        private readonly ApplicationDbContext _context;

        public QueueTicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetNextQueueNumberAsync(Guid doctorId, DateTime date)
        {
            var maxQueueNumber = await _context.QueueTickets
                .Where(q => q.Appointment.WorkSchedule.DoctorId == doctorId && q.CheckInTime.Date == date.Date)
                .Select(q => (int?)q.QueueNumber)
                .MaxAsync();

            return (maxQueueNumber ?? 0) + 1;
        }

        public async Task AddAsync(QueueTicket queueTicket)
        {
            await _context.QueueTickets.AddAsync(queueTicket);
        }

        public async Task<QueueTicket?> GetByIdAsync(Guid id)
        {
            return await _context.QueueTickets
                .Include(q => q.Appointment)
                    .ThenInclude(a => a.WorkSchedule)
                .Include(q => q.Appointment)
                    .ThenInclude(a => a.Patient)
                        .ThenInclude(p => p.Person)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<List<QueueTicket>> GetByDoctorAsync(Guid doctorId, DateTime date)
        {
            return await _context.QueueTickets
                .Include(q => q.Appointment)
                    .ThenInclude(a => a.WorkSchedule)
                .Include(q => q.Appointment)
                    .ThenInclude(a => a.Patient)
                        .ThenInclude(p => p.Person)
                .Where(q => q.Appointment.WorkSchedule.DoctorId == doctorId && q.CheckInTime.Date == date.Date)
                .OrderByDescending(q => q.Priority)
                .ThenBy(q => q.QueueNumber)
                .ToListAsync();
        }

        public async Task<QueueTicket?> GetNextWaitingAsync(Guid doctorId, DateTime date)
        {
            return await _context.QueueTickets
                .Include(q => q.Appointment)
                    .ThenInclude(a => a.WorkSchedule)
                .Include(q => q.Appointment)
                    .ThenInclude(a => a.Patient)
                        .ThenInclude(p => p.Person)
                .Where(q => q.Appointment.WorkSchedule.DoctorId == doctorId
                    && q.CheckInTime.Date == date.Date
                    && q.Status == QueueStatus.Waiting)
                .OrderByDescending(q => q.Priority)
                .ThenBy(q => q.QueueNumber)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(QueueTicket queueTicket)
        {
            _context.QueueTickets.Update(queueTicket);
        }
    }
}