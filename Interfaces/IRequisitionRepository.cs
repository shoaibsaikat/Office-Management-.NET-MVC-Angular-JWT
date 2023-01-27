using Office_Management_.NET_MVC_Angular_JWT.Models;
using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;

public interface IRequisitionRepository
{
    public Task<bool> Create(int user, string title, int inventory, int apporver, UInt32 amount, string? comment);
    public Task<int> GetListCountById(int userId);
    public Task<IEnumerable<RequisitionResponseModel>> GetRequisitionListById(int userId, int? page);
    public Task<int> GetListCount();
    public Task<IEnumerable<RequisitionResponseModel>> GetAllRequisitionList(int? page);
    public Task<int> GetPendingApprovalListCount(int approverId);
    public Task<IEnumerable<RequisitionResponseModel>> GetPendingApprovalList(int userId, int? page);
    public Task<bool> ApproveRequisition(int id, int distributorId);
    public Task<bool> DenyRequisition(int id);
    public Task<int> GetPendingDistributionListCount(int distributorId);
    public Task<IEnumerable<RequisitionResponseModel>> GetPendingDistributionList(int userId, int? page);
    public Task<bool> DistributeRequisition(int id);
}