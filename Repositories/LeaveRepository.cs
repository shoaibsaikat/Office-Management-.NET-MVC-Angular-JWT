using Microsoft.EntityFrameworkCore;
using Office_Management_.NET_MVC_Angular_JWT.Models;
using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;

namespace Office_Management_.NET_MVC_Angular_JWT.Repositories;

class LeaveRepository : ILeaveRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICommonUtil _common_util;

    public LeaveRepository(ApplicationDbContext context, ICommonUtil commonUtil)
    {
        _context = context;
        _common_util = commonUtil;
    }

    async Task<bool> ILeaveRepository.Create(AccountResponseModel user, string title, DateTime start, DateTime end, UInt32 days, string comment)
    {
        if (user.manager_id != null)
        {
            var leave = new Leave()
            {
                Title = title,
                UserId = user.id,
                ApproverId = user.manager_id.Value,
                StartDate = start,
                EndDate = end,
                DayCount = days,
                Comment = comment,
                CreationDate = DateTime.UtcNow
            };
            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();

            return true;
        }
        return false;
    }

    async Task<int> ILeaveRepository.GetListCountById(int userId)
    {
        return await _context.Leaves.Where(i => i.UserId == userId).CountAsync();
    }

    async Task<IEnumerable<LeaveResponseModel>> ILeaveRepository.GetLeaveListById(int userId, int? page)
    {        
        List<Leave> list;
        if (page != null)
        {
            list = await _context.Leaves
                                    .Include(i => i.User)
                                    .Where(i => i.UserId == userId)
                                    .OrderByDescending(i => i.Id)
                                    .Skip((page.Value - 1) * _common_util.GetPageSize())
                                    .Take(_common_util.GetPageSize())
                                    .ToListAsync();
        }
        else
        {
            // .Include() is a lazy loading, foreign tables are not by default included in query, rather they has to be explicitly loaded
            list = await _context.Leaves
                                    .Include(i => i.User)
                                    .Where(i => i.UserId == userId)
                                    .ToListAsync();
        }
        var responseList = new List<LeaveResponseModel>();
        foreach (var item in list)
        {
            var model = new LeaveResponseModel
            {
                id = item.Id,
                title = item.Title,
                start_date = item.StartDate,
                end_date = item.EndDate,
                day_count = item.DayCount,
                user = item.UserId,
                user_first_name = item.User.FirstName,
                user_last_name = item.User.LastName,
                approver = item.ApproverId,
                approved = item.Approved,
                comment = item.Comment,
                creation_date = item.CreationDate,
                approve_date = item.ApproveDate
            };
            responseList.Add(model);
        }
        return responseList;
    }

    async Task<int> ILeaveRepository.GetPendingApprovalListCount(int approverId)
    {
        return await _context.Leaves.Where(i => i.ApproverId == approverId && i.Approved == null).CountAsync();
    }

    async Task<IEnumerable<LeaveResponseModel>> ILeaveRepository.GetPendingApprovalList(int approverId, int? page)
    {
        List<Leave> list;
        if (page != null)
        {
            list = await _context.Leaves
                                    .Where(i => i.ApproverId == approverId && i.Approved == null)
                                    .Include(i => i.User)
                                    .OrderByDescending(i => i.Id)
                                    .Skip((page.Value - 1) * _common_util.GetPageSize())
                                    .Take(_common_util.GetPageSize())
                                    .ToListAsync();
        }
        else
        {
            list = await _context.Leaves
                            .Where(i => i.ApproverId == approverId && i.Approved == null)
                            .Include(i => i.User)
                            .OrderByDescending(i => i.Id)
                            .ToListAsync();
        }
        // .Include() is a lazy loading, foreign tables are not by default included in query, rather they has to be explicitly loaded
        var responseList = new List<LeaveResponseModel>();
        foreach (var item in list)
        {
            var model = new LeaveResponseModel
            {
                id = item.Id,
                title = item.Title,
                start_date = item.StartDate,
                end_date = item.EndDate,
                day_count = item.DayCount,
                user = item.UserId,
                user_first_name = item.User.FirstName,
                user_last_name = item.User.LastName,
                approver = item.ApproverId,
                approved = item.Approved,
                comment = item.Comment,
                creation_date = item.CreationDate,
                approve_date = item.ApproveDate
            };
            responseList.Add(model);
        }
        return responseList;
    }

    async Task<bool> ILeaveRepository.ApproveLeave(int id)
    {
        var leave = await _context.Leaves.FirstOrDefaultAsync(i => i.Id == id);
        if (leave != null)
        {
            leave.Approved = true;
            leave.ApproveDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    async Task<bool> ILeaveRepository.DenyLeave(int id)
    {
        var leave = await _context.Leaves.FirstOrDefaultAsync(i => i.Id == id);
        if (leave != null)
        {
            leave.Approved = false;
            leave.ApproveDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    async Task<IEnumerable<LeaveSummaryResponseModel>> ILeaveRepository.GetLeaveSummary(int year)
    {
        var responseList = new List<LeaveSummaryResponseModel>();
        var leaveQuery = from l in _context.Leaves
                    where l.StartDate.Year == year && l.Approved == true
                    group l by l.UserId into g
                    select new
                    {
                        days = g.Sum(i => i.DayCount),
                        user = g.Key
                    };
        var userList = await _context.Users.ToListAsync();
        foreach (var item in leaveQuery)
        {
            var user = userList.Where(i => i.Id == item.user).FirstOrDefault();
            if (user != null)
            {
                var model = new LeaveSummaryResponseModel
                {
                    user = user.Id,
                    first_name = user.FirstName,
                    last_name = user.LastName,
                    days = item.days
                };
                responseList.Add(model);
            }
        }
        return responseList;
    }
}