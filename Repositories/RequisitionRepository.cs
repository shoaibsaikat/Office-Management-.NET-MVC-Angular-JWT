using Microsoft.EntityFrameworkCore;
using Office_Management_.NET_MVC_Angular_JWT.Models;
using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;

namespace Office_Management_.NET_MVC_Angular_JWT.Repositories;

class RequisitionRepository : IRequisitionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICommonUtil _common_util;

    public RequisitionRepository(ApplicationDbContext context, ICommonUtil commonUtil)
    {
        _context = context;
        _common_util = commonUtil;
    }

    async Task<bool> IRequisitionRepository.Create(int user, string title, int inventory, int apporver, UInt32 amount, string? comment)
    {
        // if approved field is null then its on approver
        // if  i'ts approved then approved field is 1
        // if denied it's 0
        var requisition = new Requisition()
        {
            Title = title,
            Amount = amount,
            Comment = comment,
            RequestDate = DateTime.UtcNow,
            UserId = user,
            ApproverId = apporver,
            InventoryId = inventory
        };
        await _context.Requisitions.AddAsync(requisition);
        await _context.SaveChangesAsync();
        return true;
    }
    
    async Task<int> IRequisitionRepository.GetListCountById(int userId)
    {
        return await _context.Requisitions.Where(i => i.UserId == userId).CountAsync();
    }

    async Task<IEnumerable<RequisitionResponseModel>> IRequisitionRepository.GetRequisitionListById(int userId, int? page)
    {
        List<Requisition> list;
        if (page != null)
        {
            list = await _context.Requisitions
                                    .Include(i => i.User)
                                    .Include(i => i.Inventory)
                                    .Include(i => i.Approver)
                                    .Include(i => i.Distributor)
                                    .OrderByDescending(i => i.Id)
                                    .Where(i => i.UserId == userId)
                                    .Skip((page.Value - 1) * _common_util.GetPageSize())
                                    .Take(_common_util.GetPageSize())
                                    .ToListAsync();
        }
        else
        {
            list = await _context.Requisitions
                                    .Include(i => i.User)
                                    .Include(i => i.Inventory)
                                    .Include(i => i.Approver)
                                    .Include(i => i.Distributor)
                                    .Where(i => i.UserId == userId)
                                    .OrderByDescending(i => i.Id)
                                    .ToListAsync();
        }

        // .Include() is a lazy loading, foreign tables are not by default included in query, rather they has to be explicitly loaded
        var responseList = new List<RequisitionResponseModel>();
        foreach (var item in list)
        {
            var model = new RequisitionResponseModel
            {
                id = item.Id,
                item_name = item.Inventory.Name,
                title = item.Title,
                date = item.RequestDate,
                amount = item.Amount,
                total = item.Inventory.Count,
                unit = item.Inventory.Unit,
                user_id = item.UserId,
                user_name = item.User.FirstName + " " + item.User.LastName,
                approver_id = item.ApproverId,
                approver_name = item.Approver.FirstName + " " + item.Approver.LastName,
                distributor_id = item.DistributorId,
                distributor_name = item.Distributor?.FirstName + " " + item.Distributor?.LastName,
                approved = item.Approved,
                distributed = item.Distributed,
                comment = item.Comment
            };
            responseList.Add(model);
        }
        return responseList;
    }

    async Task<int> IRequisitionRepository.GetListCount()
    {
        return await _context.Requisitions.CountAsync();
    }

    async Task<IEnumerable<RequisitionResponseModel>> IRequisitionRepository.GetAllRequisitionList(int? page)
    {
        List<Requisition> list;
        if (page != null)
        {
            list = await _context.Requisitions
                                    .Include(i => i.User)
                                    .Include(i => i.Inventory)
                                    .Include(i => i.Approver)
                                    .Include(i => i.Distributor)
                                    .OrderByDescending(i => i.Id)
                                    .Skip((page.Value - 1) * _common_util.GetPageSize())
                                    .Take(_common_util.GetPageSize())
                                    .ToListAsync();
        }
        else
        {
            list = await _context.Requisitions
                                    .Include(i => i.User)
                                    .Include(i => i.Inventory)
                                    .Include(i => i.Approver)
                                    .Include(i => i.Distributor)
                                    .OrderByDescending(i => i.Id)
                                    .ToListAsync();
        }

        // .Include() is a lazy loading, foreign tables are not by default included in query, rather they has to be explicitly loaded
        var responseList = new List<RequisitionResponseModel>();
        foreach (var item in list)
        {
            var model = new RequisitionResponseModel
            {
                id = item.Id,
                item_name = item.Inventory.Name,
                title = item.Title,
                date = item.RequestDate,
                amount = item.Amount,
                total = item.Inventory.Count,
                unit = item.Inventory.Unit,
                user_id = item.UserId,
                user_name = item.User.FirstName + " " + item.User.LastName,
                approver_id = item.ApproverId,
                approver_name = item.Approver.FirstName + " " + item.Approver.LastName,
                distributor_id = item.DistributorId,
                distributor_name = item.Distributor?.FirstName + " " + item.Distributor?.LastName,
                approved = item.Approved,
                distributed = item.Distributed,
                comment = item.Comment
            };
            responseList.Add(model);
        }
        return responseList;
    }

    async Task<int> IRequisitionRepository.GetPendingApprovalListCount(int approverId)
    {
        return await _context.Requisitions.Where(i => i.ApproverId == approverId && i.Approved == null).CountAsync();
    }

    async Task<IEnumerable<RequisitionResponseModel>> IRequisitionRepository.GetPendingApprovalList(int approverId, int? page)
    {
        List<Requisition> list;
        if (page != null)
        {
            list = await _context.Requisitions
                                    .Include(i => i.User)
                                    .Include(i => i.Inventory)
                                    .Include(i => i.Approver)
                                    .Include(i => i.Distributor)
                                    .OrderByDescending(i => i.Id)
                                    .Where(i => i.ApproverId == approverId && i.Approved == null)
                                    .Skip((page.Value - 1) * _common_util.GetPageSize())
                                    .Take(_common_util.GetPageSize())
                                    .ToListAsync();
        }
        else
        {
            list = await _context.Requisitions
                                    .Include(i => i.User)
                                    .Include(i => i.Inventory)
                                    .Include(i => i.Approver)
                                    .Include(i => i.Distributor)
                                    .Where(i => i.ApproverId == approverId && i.Approved == null)
                                    .OrderByDescending(i => i.Id)
                                    .ToListAsync();
        }

        // .Include() is a lazy loading, foreign tables are not by default included in query, rather they has to be explicitly loaded
        var responseList = new List<RequisitionResponseModel>();
        foreach (var item in list)
        {
            var model = new RequisitionResponseModel
            {
                id = item.Id,
                item_name = item.Inventory.Name,
                title = item.Title,
                date = item.RequestDate,
                amount = item.Amount,
                total = item.Inventory.Count,
                unit = item.Inventory.Unit,
                user_id = item.UserId,
                user_name = item.User.FirstName + " " + item.User.LastName,
                approver_id = item.ApproverId,
                approver_name = item.Approver.FirstName + " " + item.Approver.LastName,
                distributor_id = item.DistributorId,
                distributor_name = item.Distributor?.FirstName + " " + item.Distributor?.LastName,
                approved = item.Approved,
                distributed = item.Distributed,
                comment = item.Comment
            };
            responseList.Add(model);
        }
        return responseList;
    }

    async Task<bool> IRequisitionRepository.ApproveRequisition(int id, int distributorId)
    {
        var requisition = await _context.Requisitions.FirstOrDefaultAsync(i => i.Id == id);
        var disributor = await _context.Users.FirstOrDefaultAsync(i => i.Id == distributorId);
        if (requisition != null && disributor != null)
        {
            requisition.Approved = true;
            requisition.ApproveDate = DateTime.UtcNow;
            requisition.DistributorId = distributorId;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    async Task<bool> IRequisitionRepository.DenyRequisition(int id)
    {
        var requisition = await _context.Requisitions.FirstOrDefaultAsync(i => i.Id == id);
        if (requisition != null)
        {
            requisition.Approved = false;
            requisition.ApproveDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    async Task<int> IRequisitionRepository.GetPendingDistributionListCount(int distributorId)
    {
        return await _context.Requisitions.Where(i => i.DistributorId == distributorId && i.Distributed == null).CountAsync();
    }

    async Task<IEnumerable<RequisitionResponseModel>> IRequisitionRepository.GetPendingDistributionList(int distributorId, int? page)
    {
        List<Requisition> list;
        if (page != null)
        {
            list = await _context.Requisitions
                                    .Include(i => i.User)
                                    .Include(i => i.Inventory)
                                    .Include(i => i.Approver)
                                    .Include(i => i.Distributor)
                                    .OrderByDescending(i => i.Id)
                                    .Where(i => i.DistributorId == distributorId && i.Distributed == null)
                                    .Skip((page.Value - 1) * _common_util.GetPageSize())
                                    .Take(_common_util.GetPageSize())
                                    .ToListAsync();
        }
        else
        {
            list = await _context.Requisitions
                                    .Include(i => i.User)
                                    .Include(i => i.Inventory)
                                    .Include(i => i.Approver)
                                    .Include(i => i.Distributor)
                                    .Where(i => i.DistributorId == distributorId && i.Distributed == null)
                                    .OrderByDescending(i => i.Id)
                                    .ToListAsync();
        }

        // .Include() is a lazy loading, foreign tables are not by default included in query, rather they has to be explicitly loaded
        var responseList = new List<RequisitionResponseModel>();
        foreach (var item in list)
        {
            var model = new RequisitionResponseModel
            {
                id = item.Id,
                item_name = item.Inventory.Name,
                title = item.Title,
                date = item.RequestDate,
                amount = item.Amount,
                total = item.Inventory.Count,
                unit = item.Inventory.Unit,
                user_id = item.UserId,
                user_name = item.User.FirstName + " " + item.User.LastName,
                approver_id = item.ApproverId,
                approver_name = item.Approver.FirstName + " " + item.Approver.LastName,
                distributor_id = item.DistributorId,
                distributor_name = item.Distributor?.FirstName + " " + item.Distributor?.LastName,
                approved = item.Approved,
                distributed = item.Distributed,
                comment = item.Comment
            };
            responseList.Add(model);
        }
        return responseList;
    }

    async Task<bool> IRequisitionRepository.DistributeRequisition(int id)
    {
        var requisition = await _context.Requisitions.Include(i => i.Inventory).FirstOrDefaultAsync(i => i.Id == id);
        if (requisition != null && requisition.Inventory.Count >= requisition.Amount)
        {
            requisition.Distributed = true;
            requisition.DistributionDate = DateTime.UtcNow;
            requisition.Inventory.Count = requisition.Inventory.Count - requisition.Amount;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}