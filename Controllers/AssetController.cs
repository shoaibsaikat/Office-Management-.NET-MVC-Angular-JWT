using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Office_Management_.NET_MVC_Angular_JWT.Utils;

namespace Office_Management_.NET_MVC_Angular_JWT.Controllers;

[ApiController]
public class AssetController : ControllerBase
{
    private readonly ILogger<AssetController> _logger;
    private readonly IAssetRepository _asset_repo;
    private readonly IAccountUtil _account_util;

    public AssetController(ILogger<AssetController> logger, IAccountUtil accountUtil, IAssetRepository repo)
    {
        _logger = logger;
        _account_util = accountUtil;
        _asset_repo = repo;
    }

    [HttpGet]
    [Route("api/asset/list/{page:int?}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> GetAllList(int? page)
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null || !user.can_manage_asset)
        {
            return Unauthorized();
        }
        var list = (List<ResponseModels.AssetResponseModel>)await _asset_repo.GetAllList(page);
        var count = await _asset_repo.GetListCount();
        return Ok(new
        {
            status = _asset_repo.GetStatus(),
            type = _asset_repo.GetType(),
            asset_list = list,
            count = count
        });
    }

    [HttpGet, HttpPost]
    [Route("api/asset/my_list/{page:int?}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> GetMyList(int? page)
    {
        var userId = _account_util.AuthorizeRequest(Request);
        if (userId == null)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var list = (List<ResponseModels.AssetResponseModel>)await _asset_repo.GetMyList(userId.Value, page);
            var count = await _asset_repo.GetMyListCount(userId.Value);
            return Ok(new
            {
                asset_list = list,
                user_list = await _account_util.GetAllUser(),
                count = count,
            });
        }
        else if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var id = Convert.ToInt32(bodyJson.GetValue("pk")?.ToString());
                var user = Convert.ToInt32(bodyJson.GetValue("assignee")?.ToString());
                if (await _asset_repo.Assign(id, user))
                {
                    return Ok("Asset assigned");
                }
            }
        }
        return NotFound("Asset assign failed");
    }

    [HttpGet, HttpPost, HttpPut]
    [Route("api/asset/my_pending_list/{page:int?}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> GetMyPendingList(int? page)
    {
        var userId = _account_util.AuthorizeRequest(Request);
        if (userId == null)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var list = (List<ResponseModels.AssetResponseModel>)await _asset_repo.GetMyPendingList(userId.Value, page);
            var count =  await _asset_repo.GetMyPendingListCount(userId.Value);
            return Ok(new
            {
                asset_list = list,
                count = count,
            });
        }
        else if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var id = Convert.ToInt32(bodyJson.GetValue("pk")?.ToString());
                if (await _asset_repo.Accept(id))
                {
                    return Ok("Asset assigned");
                }
                return NotFound("Asset assign failed");
            }
        }
        else if (Request.Method == "PUT")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var id = Convert.ToInt32(bodyJson.GetValue("pk")?.ToString());
                if (await _asset_repo.Decline(id))
                {
                    return Ok("Asset declined");
                }
            }
        }
        return NotFound("Asset decline failed");
    }

    [HttpGet, HttpPost]
    [Route("api/asset/edit/{id:int}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Update(int id)
    {
        var userId = _account_util.AuthorizeRequest(Request);
        if (userId == null)
        {
            return Unauthorized();
        }
        var asset = await _asset_repo.GetById(id);
        if (asset == null)
        {
            return NotFound();
        }
        if (Request.Method == "GET")
        {
            var statusList = _asset_repo.GetStatus();
            return Ok(new
            {
                asset = asset,
                status = statusList,
            });
        }
        else if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var name = bodyJson.GetValue("name")?.ToString();
                var warranty = Convert.ToInt32(bodyJson.GetValue("warranty")?.ToString());
                var status = Convert.ToInt32(bodyJson.GetValue("status")?.ToString());
                var description = bodyJson.GetValue("description")?.ToString();
                if (await _asset_repo.Update(
                    id,
                    !String.IsNullOrEmpty(name) ? name : asset.name,
                    Convert.ToUInt16(status),
                    Convert.ToUInt64(warranty),
                    !String.IsNullOrEmpty(description) ? description : string.Empty))
                {
                    return Ok("Asset updated");
                }
            }
        }
        return NotFound("Asset update failed");
    }

    [HttpGet, HttpPost]
    [Route("api/asset/add")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create()
    {
        var userId = _account_util.AuthorizeRequest(Request);
        if (userId == null)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var statusList = _asset_repo.GetStatus();
            var typeList = _asset_repo.GetType();
            return Ok(new
            {
                type = typeList,
                status = statusList,
            });
        }
        else if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var name = bodyJson.GetValue("name")?.ToString();
                var model = bodyJson.GetValue("model")?.ToString();
                var serial = bodyJson.GetValue("serial")?.ToString();
                // Console.WriteLine("---------" + bodyJson.GetValue("purchaseDate")?.ToString());
                var purchaseDate = CommonUtil.ConvertFromUnixTimestamp(Convert.ToDouble(bodyJson.GetValue("purchaseDate")?.ToString()));
                var warranty = Convert.ToInt32(bodyJson.GetValue("warranty")?.ToString());
                var status = Convert.ToInt32(bodyJson.GetValue("status")?.ToString());
                var type = Convert.ToInt32(bodyJson.GetValue("type")?.ToString());
                var description = bodyJson.GetValue("description")?.ToString();

                if (!String.IsNullOrEmpty(name) &&
                    !String.IsNullOrEmpty(model) &&
                    !String.IsNullOrEmpty(serial))
                {
                    if (await _asset_repo.Create(
                        userId.Value,
                        name,
                        model,
                        serial,
                        purchaseDate,
                        Convert.ToUInt16(type),
                        Convert.ToUInt16(status),
                        Convert.ToUInt64(warranty),
                        !String.IsNullOrEmpty(description) ? description : String.Empty
                    )) {
                        return Ok("Asset created");
                    } else {
                        return NotFound("Asset creation failed");
                    }
                }
            }
        }
        return NotFound();
    }
}