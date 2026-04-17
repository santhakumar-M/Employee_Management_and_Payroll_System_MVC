using EmployeeHrSystem.Models;

namespace EmployeeHrSystem.Services
{
    public interface ILeaveService
    {
        Task<List<LeaveRequest>> GetAllLeaveRequestsAsync();
        Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);
        Task<List<LeaveRequest>> GetEmployeeLeaveRequestsAsync(int employeeId);
        Task<bool> ApplyLeaveAsync(LeaveRequest leaveRequest);
        Task<bool> UpdateLeaveStatusAsync(int leaveId, string status);
        Task<bool> DeleteLeaveRequestAsync(int id);
        Task<List<LeaveRequest>> GetLeaveRequestsByStatusAsync(string status);
    }
}
