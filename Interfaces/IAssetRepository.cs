using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;
using Office_Management_.NET_MVC_Angular_JWT.Models;

public interface IAssetRepository
{
    public IDictionary<int, string> GetStatus();
    public IDictionary<int, string> GetType();
    public Task<int> GetListCount();
    public Task<IEnumerable<AssetResponseModel>> GetAllList(int? page);
    public Task<int> GetMyListCount(int id);
    public Task<IEnumerable<AssetResponseModel>> GetMyList(int id, int? page);
    public Task<int> GetMyPendingListCount(int id);
    public Task<IEnumerable<AssetResponseModel>> GetMyPendingList(int id, int? page);
    public Task<AssetResponseModel?> GetById(int id);
    public Task<Boolean> Update(int id, string name, ushort status, ulong warranty, string description);
    public Task<Boolean> Create(int id, string name, string model, string serial, DateTime purchaseDate, ushort type, ushort status, ulong warranty, string description);
    public Task<Boolean> Assign(int id, int userId);
    public Task<Boolean> Accept(int id);
    public Task<Boolean> Decline(int id);
}