using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;

namespace Office_Management_.NET_MVC_Angular_JWT.Utils;
class AccountUtil : IAccountUtil
{
    private readonly IAccountRepository _account_repo;
    private readonly ITokenUtil _tokenUtil;

    public AccountUtil(ITokenUtil token, IAccountRepository repo)
    {
        _tokenUtil = token;
        _account_repo = repo;
    }

    int? IAccountUtil.AuthorizeRequest(HttpRequest request)
    {
        var token = request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        return _tokenUtil.ValidateToken(token);
    }

    async Task<AccountResponseModel?> IAccountUtil.AuthorizeUser(HttpRequest request)
    {
        var userId =  _tokenUtil.ValidateToken(request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last());
        if (userId != null)
        {
            return await _account_repo.GetUserById(userId.Value);
        }
        return null;
    }

    async Task<IEnumerable<AccountResponseModel>> IAccountUtil.GetAllRequisitionApprover()
    {
        return await _account_repo.GetAllRequisitionApprover();
    }

    async Task<IEnumerable<AccountResponseModel>> IAccountUtil.GetAllRequisitionDistributor()
    {
        return await _account_repo.GetAllRequisitionDistributor();
    }

    async Task<IEnumerable<AccountResponseModel>> IAccountUtil.GetAllManager()
    {
        return await _account_repo.GetAllManager();
    }

    async Task<IEnumerable<AccountResponseModel>> IAccountUtil.GetAllUser()
    {
        return await _account_repo.GetAllUser();
    }

}