using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Office_Management_.NET_MVC_Angular_JWT.Controllers;

[ApiController]
public class RequisitionController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;
    private readonly IInventoryRepository _inventory_repo;
    private readonly IRequisitionRepository _requisition_repo;
    private readonly IAccountUtil _account_util;

    public RequisitionController(ILogger<InventoryController> logger, IAccountUtil accountUtil, IInventoryRepository inventoryRepo, IRequisitionRepository requisitionRepo)
    {
        _logger = logger;
        _account_util = accountUtil;
        _requisition_repo = requisitionRepo;
        _inventory_repo = inventoryRepo;
    }

    [HttpGet, HttpPost]
    [Route("api/inventory/requisition/create")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create()
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var inventoryList = (List<ResponseModels.InventoryResponseModel>)await _inventory_repo.GetAllList(null);
            return Ok(new
            {
                approver_list = await _account_util.GetAllRequisitionApprover(),
                inventory_list = inventoryList,
            });
        }
        else if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var title = bodyJson.GetValue("title")?.ToString();
                var amount = Convert.ToUInt32(bodyJson.GetValue("amount")?.ToString());
                var inventory = Convert.ToInt32(bodyJson.GetValue("inventory")?.ToString());
                var approver = Convert.ToInt32(bodyJson.GetValue("approver")?.ToString());
                var comment = bodyJson.GetValue("comment")?.ToString();

                if (!String.IsNullOrEmpty(title) && amount > 0 && inventory > 0 && approver > 0)
                {
                    await _requisition_repo.Create(user.id, title, inventory, approver, amount, comment);
                    return Ok("Requisition created");
                }
            }
        }
        return NotFound("Requisition creation failed");
    }

    [HttpGet]
    [Route("api/inventory/requisition/my_list/{page:int?}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> GetMyList(int? page)
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var list = (List<ResponseModels.RequisitionResponseModel>)await _requisition_repo.GetRequisitionListById(user.id, page);
            var count = await _requisition_repo.GetListCountById(user.id);
            return Ok(new
            {
                requisition_list = list,
                count = count
            });
        }
        return NotFound();
    }

    [HttpGet]
    [Route("api/inventory/requisition/history/{page:int?}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> GetAll(int? page)
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null || !(user.can_distribute_inventory || user.can_approve_inventory))
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var list = (List<ResponseModels.RequisitionResponseModel>)await _requisition_repo.GetAllRequisitionList(page);
            var count = await _requisition_repo.GetListCount();
            return Ok(new
            {
                requisition_list = list,
                count = count
            });
        }
        return NotFound();
    }

    [HttpGet, HttpPost]
    [Route("api/inventory/requisition/approval/{page:int?}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Approve(int? page)
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null || !user.can_approve_inventory)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var requisitionList = (List<ResponseModels.RequisitionResponseModel>)await _requisition_repo.GetPendingApprovalList(user.id, page);
            var distributorList = (List<ResponseModels.AccountResponseModel>)await _account_util.GetAllRequisitionDistributor();
            var count = await _requisition_repo.GetPendingApprovalListCount(user.id);
            return Ok(new
            {
                requisition_list = requisitionList,
                distributor_list = distributorList,
                count = count
            });
        }
        else if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var id = Convert.ToInt32(bodyJson.GetValue("pk")?.ToString());
                var distributor = Convert.ToInt32(bodyJson.GetValue("distributor")?.ToString());
                if (await _requisition_repo.ApproveRequisition(id, distributor))
                {
                    return Ok("Requisition approved");
                }
            }
        }
        return NotFound("Requisition approval failed");
    }

    [HttpPost]
    [Route("api/inventory/requisition/denial")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Deny()
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null || !user.can_approve_inventory)
        {
            return Unauthorized();
        }
        if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var id = Convert.ToInt32(bodyJson.GetValue("pk")?.ToString());
                if (await _requisition_repo.DenyRequisition(id))
                {
                    return Ok("Requisition denied");
                }
            }
        }
        return NotFound("Requisition denial failed");
    }

    [HttpGet, HttpPost]
    [Route("api/inventory/requisition/distribution/{page:int?}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Distribute(int? page)
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null || !user.can_distribute_inventory)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var requisitionList = (List<ResponseModels.RequisitionResponseModel>)await _requisition_repo.GetPendingDistributionList(user.id, page);
            var count = await _requisition_repo.GetPendingDistributionListCount(user.id);
            return Ok(new
            {
                requisition_list = requisitionList,
                count = count
            });
        }
        else if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var id = Convert.ToInt32(bodyJson.GetValue("pk")?.ToString());
                if (await _requisition_repo.DistributeRequisition(id))
                {
                    return Ok("Item distributed");
                }
                return NotFound("Distribution failed! Inventory low, please add items to the inventory first.");
            }
        }
        return NotFound();
    }
}
