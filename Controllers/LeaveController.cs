using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Office_Management_.NET_MVC_Angular_JWT.Controllers;

[ApiController]
public class LeaveController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;
    private readonly ILeaveRepository _leave_repo;
    private readonly IAccountUtil _account_util;
    private readonly ICommonUtil _common_util;

    public LeaveController(ILogger<InventoryController> logger, IAccountUtil accountUtil, ILeaveRepository leaveRepo, ICommonUtil commonUtil)
    {
        _logger = logger;
        _account_util = accountUtil;
        _leave_repo = leaveRepo;
        _common_util = commonUtil;
    }

    [HttpGet, HttpPost]
    [Route("api/leave/create")]
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
            if (user.manager_id == null)
            {
                return NotFound(new
                {
                    detail = "Add manager first"
                });
            }
            return Ok();
        }
        else if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var title = bodyJson.GetValue("title")?.ToString();
                var start = Convert.ToDateTime(bodyJson.GetValue("start")?.ToString());
                var end = Convert.ToDateTime(bodyJson.GetValue("end")?.ToString());
                var days = Convert.ToUInt32(bodyJson.GetValue("days")?.ToString());
                var comment = bodyJson.GetValue("comment")?.ToString();

                if (!String.IsNullOrEmpty(title) && days > 0 && comment != null)
                {
                    if (await _leave_repo.Create(user, title, start, end, days, comment))
                    {
                        return Ok(new
                        {
                            detail = "Leave created."
                        });
                    }
                }
            }
        }
        return NotFound(new
        {
            detail = "Leave creation failed."
        });
    }

    [HttpGet]
    [Route("api/leave/my_list/{page:int?}")]
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
            var list = (List<ResponseModels.LeaveResponseModel>)await _leave_repo.GetLeaveListById(user.id, page);
            var count = await _leave_repo.GetListCountById(user.id);
            return Ok(new
            {
                leave_list = list,
                count = count
            });
        }
        return NotFound();
    }
    
    [HttpGet]
    [Route("api/leave/request_list/{page:int?}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> GetLeaveRequestList(int? page)
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null || !user.can_approve_leave)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var leaveList = (List<ResponseModels.LeaveResponseModel>)await _leave_repo.GetPendingApprovalList(user.id, page);
            var count = await _leave_repo.GetPendingApprovalListCount(user.id);
            return Ok(new
            {
                leave_list = leaveList,
                count = count
            });
        }
        return NotFound();
    }

    
    [HttpPost]
    [Route("api/leave/approve/{id:int}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Approve(int id)
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null || !user.can_approve_leave)
        {
            return Unauthorized();
        }
        if (Request.Method == "POST")
        {
            if (await _leave_repo.ApproveLeave(id))
            {
                return Ok("Leave approved");
            }
        }
        return NotFound("Leave approval failed");
    }

    [HttpPost]
    [Route("api/leave/deny/{id:int}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Deny(int id)
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null || !user.can_approve_leave)
        {
            return Unauthorized();
        }
        if (Request.Method == "POST")
        {
            if (await _leave_repo.DenyLeave(id))
            {
                return Ok("Leave denied");
            }
        }
        return NotFound("Leave denial failed");
    }

    [HttpGet]
    [Route("api/leave/summary/{year:int}/{page:int?}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> GetYearSummary(int year, int? page)
    {
        var user = await _account_util.AuthorizeUser(Request);
        if (user == null)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            List<ResponseModels.LeaveSummaryResponseModel> outputList;
            var list = (List<ResponseModels.LeaveSummaryResponseModel>)await _leave_repo.GetLeaveSummary(year);
            if (page != null)
            {
                var start = (page.Value - 1) * _common_util.GetPageSize();
                var count = start + _common_util.GetPageSize() > list.Count ? list.Count : _common_util.GetPageSize();
                outputList = list.GetRange(start, count);
            }
            else
            {
                outputList = list;
            }
            return Ok(new
            {
                leave_list = outputList,
                count = outputList.Count
            });
        }
        return NotFound();
    }
}