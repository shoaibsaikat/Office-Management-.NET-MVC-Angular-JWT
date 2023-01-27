using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;

public interface IAccountUtil
{
    public int? AuthorizeRequest(HttpRequest request);
    public Task<AccountResponseModel?> AuthorizeUser(HttpRequest request);
    public Task<IEnumerable<AccountResponseModel>> GetAllUser();
    public Task<IEnumerable<AccountResponseModel>> GetAllRequisitionApprover();
    public Task<IEnumerable<AccountResponseModel>> GetAllRequisitionDistributor();
    public Task<IEnumerable<AccountResponseModel>> GetAllManager();
}