using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Office_Management_.NET_MVC_Angular_JWT.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountUtil _account_util;
    private readonly IAccountRepository _account_repo;

    public AccountController(ILogger<AccountController> logger, IAccountUtil accountUtil, IAccountRepository repo)
    {
        _logger = logger;
        _account_util = accountUtil;
        _account_repo = repo;
    }

    [HttpPost]
    [Route("api/user/register")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Register()
    {
        using (var reader = new StreamReader(Request.Body))
        {
            var body = await reader.ReadToEndAsync();
            var bodyJson = JObject.Parse(body);
            var username = bodyJson.GetValue("username")?.ToString();
            var password = bodyJson.GetValue("password")?.ToString();
            var superUser = bodyJson.GetValue("super")?.ToString();
            var isSuperUser = superUser != null ? Boolean.Parse(superUser) : false;
            var result = await _account_repo.RegisterUser(username, password, isSuperUser);
            return Ok(result);
        }
    }

    [HttpPost]
    [Route("api/user/login")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Login()
    {
        using (var reader = new StreamReader(Request.Body))
        {
            var body = await reader.ReadToEndAsync();
            var bodyJson = JObject.Parse(body);
            var username = bodyJson.GetValue("username")?.ToString();
            var password = bodyJson.GetValue("password")?.ToString();
            var token = await _account_repo.Login(username, password);
            if (String.IsNullOrEmpty(token))
            {
                return Unauthorized(new
                {
                    detail = "Invalid Login credential"
                });
            }
            return Ok(new
            {
                refresh = token,
                access = token
            });
        }
    }

    [HttpPost]
    [Route("api/user/signout")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public IActionResult Logout()
    {
        return Ok();
    }

    [HttpGet]
    [Route("api/user/get")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> Get()
    {
        var userId = _account_util.AuthorizeRequest(Request);
        if (userId == null)
        {
            return Unauthorized();
        }
        var user = await _account_repo.GetUserById(userId.Value);
        return Ok(user);
    }

    [HttpPost]
    [Route("api/user/change_password")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> ChangePassword()
    {
        var userId = _account_util.AuthorizeRequest(Request);
        if (userId == null)
        {
            return Unauthorized();
        }
        using (var reader = new StreamReader(Request.Body))
        {
            var body = await reader.ReadToEndAsync();
            var bodyJson = JObject.Parse(body);
            var oldPassword = bodyJson.GetValue("lastpassword")?.ToString();
            var newPassword = bodyJson.GetValue("newpassword")?.ToString();
            if (!String.IsNullOrEmpty(oldPassword) && !String.IsNullOrEmpty(newPassword) && await _account_repo.ValidatePassword(userId.Value, oldPassword))
            {
                await _account_repo.SetPassword(userId.Value, newPassword);
                return Ok();
            }
        }
        return ValidationProblem("Wrong password");
    }

    [HttpGet, HttpPost]
    [Route("api/user/change_manager")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> ChangeManger()
    {
        var userId = _account_util.AuthorizeRequest(Request);
        if (userId == null)
        {
            return Unauthorized();
        }
        if (Request.Method == "GET")
        {
            var responseList = _account_repo.GetAllManager();
            return Ok(new
            {
                user_list = responseList
            });
        }
        else if (Request.Method == "POST")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var bodyJson = JObject.Parse(body);
                var managerId = Convert.ToInt32(bodyJson.GetValue("manager")?.ToString());
                if (await _account_repo.SetManager(userId.Value, managerId))
                {
                    return Ok();
                }
            }
        }
        return ValidationProblem("Update failed");
    }

    [HttpPost]
    [Route("api/user/change_profile")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> ChangeProfile()
    {
        var userId = _account_util.AuthorizeRequest(Request);
        if (userId == null)
        {
            return Unauthorized();
        }
        using (var reader = new StreamReader(Request.Body))
        {
            var body = await reader.ReadToEndAsync();
            var bodyJson = JObject.Parse(body);
            var firstName = bodyJson.GetValue("first_name")?.ToString();
            var lastName = bodyJson.GetValue("last_name")?.ToString();
            var email = bodyJson.GetValue("email")?.ToString();
            if (!String.IsNullOrEmpty(firstName) &&
                !String.IsNullOrEmpty(lastName) &&
                !String.IsNullOrEmpty(email) &&
                await _account_repo.UpdateUserInfo(userId.Value, firstName, lastName, email))
            {
                return Ok();
            }
        }
        return ValidationProblem("Update failed");
    }

}
