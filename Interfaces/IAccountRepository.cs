using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;

public interface IAccountRepository
{
    public Task<Boolean> RegisterUser(string? username, string? password, bool isSuperUser = false);
    public Task<Boolean> ValidatePassword(int id, string password);
    public Task<string> Login(string? username, string? password);
    public Task<AccountResponseModel> GetUserById(int id);
    public Task<Boolean> SetPassword(int id, string password);
    public Task<Boolean> UpdateUserInfo(int id, string firstName, string lastName, string email);
    public Task<IEnumerable<AccountResponseModel>> GetAllUser();
    public Task<IEnumerable<AccountResponseModel>> GetAllRequisitionApprover();
    public Task<IEnumerable<AccountResponseModel>> GetAllRequisitionDistributor();
    public Task<IEnumerable<AccountResponseModel>> GetAllManager();
    public Task<Boolean> SetManager(int id, int mangerId);
}