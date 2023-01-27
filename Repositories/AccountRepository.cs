using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Office_Management_.NET_MVC_Angular_JWT.Models;
using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;


namespace Office_Management_.NET_MVC_Angular_JWT.Repositories;

class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITokenUtil _tokenUtil;
    private readonly int _ITERATION_COUNT = 100000;
    private readonly int _SALT_LENGTH = 128;
    private readonly int _HASH_LENGTH = 256;
    private readonly int _BYTE_LENGTH = 8;
    private readonly int _PART_LENGTH = 3;

    public AccountRepository(ApplicationDbContext context, IConfiguration configuration, ITokenUtil tokenUtil)
    {
        _context = context;
        _configuration = configuration;
        _tokenUtil = tokenUtil;
    }

    private Task<string> GetHashedPassword(string password)
    {
        return Task.Run(()=>
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[_SALT_LENGTH / _BYTE_LENGTH];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: _ITERATION_COUNT,
                numBytesRequested: _HASH_LENGTH / _BYTE_LENGTH));
            return $"{_ITERATION_COUNT}.{Convert.ToBase64String(salt)}.{hashed}";
        });
    }

    private Task<string> GetHashedPasswordBySaltAndIteration(string password, byte[] salt, int iteration)
    {
        return Task.Run(()=>
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: iteration,
                numBytesRequested: _HASH_LENGTH / _BYTE_LENGTH));
            return $"{iteration}.{Convert.ToBase64String(salt)}.{hashed}";
        });
    }

    async Task<Boolean> IAccountRepository.RegisterUser(string? username, string? password, bool isSuperUser)
    {
        if (username != null && password != null)
        {
            var user = new User()
            {
                Username = username,
                Password = await GetHashedPassword(password),
                IsSuperuser = isSuperUser
            };
            await _context.Users.AddAsync(user);
            // Console.WriteLine(user.Username + ":" + user.Password);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    private async Task<Boolean> IsPassowrdValid(User user, string password)
    {            
        var parts = user?.Password.Split('.', _PART_LENGTH);
        if (parts?.Length != _PART_LENGTH)
        {
            throw new FormatException("Unexpected hash format. " + "Should be formatted as `{iterations}.{salt}.{hash}`");
        }
        var iterations = Convert.ToInt32(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);

        var hashedPassword = await GetHashedPasswordBySaltAndIteration(password, salt, iterations);
        if (user?.Password == hashedPassword)
        {
            return true;
        }
        return false;
    }

    async Task<Boolean> IAccountRepository.ValidatePassword(int id, string password)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == id);
        return await IsPassowrdValid(user, password);
    }

    async Task<string> IAccountRepository.Login(string? username, string? password)
    {
        if (username != null && password != null)
        {
            try
            {
                var user = await _context.Users.FirstAsync(u => u.Username == username);
                if (user != null && await IsPassowrdValid(user, password))
                {
                    return _tokenUtil.GenerateJwtToken(user.Id, username);
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
        return String.Empty;
    }

    async Task<bool> IAccountRepository.SetPassword(int id, string password)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == id);
        user.Password = await GetHashedPassword(password);
        await _context.SaveChangesAsync();
        return true;
    }

    async Task<AccountResponseModel> IAccountRepository.GetUserById(int id)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == id);
        var account = new AccountResponseModel
        {
            id = id,
            username = user.Username,
            first_name = user.FirstName,
            last_name = user.LastName,
            email = user.Email,
            access_token = String.Empty,
            refresh_token = String.Empty,
            manager_id = user.SupervisorId,
            can_approve_inventory = user.CanApproveInventory,
            can_approve_leave = user.CanApproveLeave,
            can_distribute_inventory = user.CanDistributeInventory,
            can_manage_asset = user.CanManageAsset
        };
        return account;
    }

    async Task<Boolean> IAccountRepository.UpdateUserInfo(int id, string firstName, string lastName, string email)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == id);
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        await _context.SaveChangesAsync();
        return true;
    }

    // Returns all users in response format
    async Task<IEnumerable<AccountResponseModel>> IAccountRepository.GetAllUser()
    {
        var list = await _context.Users.ToListAsync();
        var responseList = new List<AccountResponseModel>();
        foreach (var item in list)
        {
            if (!String.IsNullOrEmpty(item.FirstName))
            {
                responseList.Add(new AccountResponseModel
                {
                    id = item.Id,
                    username = item.Username,
                    first_name = item.FirstName,
                    last_name = item.LastName == null ? "" : item.LastName
                });
            }
        }
        return responseList;
    }

    // get all approver who can approve requisition
    async Task<IEnumerable<AccountResponseModel>> IAccountRepository.GetAllRequisitionApprover()
    {
        var list = await _context.Users.Where(i => i.CanApproveInventory).ToListAsync();
        var responseList = new List<AccountResponseModel>();
        foreach (var item in list)
        {
            if (!String.IsNullOrEmpty(item.FirstName))
            {
                responseList.Add(new AccountResponseModel
                {
                    id = item.Id,
                    username = item.Username,
                    first_name = item.FirstName,
                    last_name = item.LastName == null ? "" : item.LastName
                });
            }
        }
        return responseList;
    }

    // get all distributor who can distribute requisition
    async Task<IEnumerable<AccountResponseModel>> IAccountRepository.GetAllRequisitionDistributor()
    {
        var list = await _context.Users.Where(i => i.CanDistributeInventory).ToListAsync();
        var responseList = new List<AccountResponseModel>();
        foreach (var item in list)
        {
            if (!String.IsNullOrEmpty(item.FirstName))
            {
                responseList.Add(new AccountResponseModel
                {
                    id = item.Id,
                    username = item.Username,
                    first_name = item.FirstName,
                    last_name = item.LastName == null ? "" : item.LastName
                });
            }
        }
        return responseList;
    }

    // get all user who can approve leave
    async Task<IEnumerable<AccountResponseModel>> IAccountRepository.GetAllManager()
    {
        var list = await _context.Users.Where(i => i.CanApproveLeave).ToListAsync();
        var responseList = new List<AccountResponseModel>();
        foreach (var item in list)
        {
            if (!String.IsNullOrEmpty(item.FirstName))
            {
                responseList.Add(new AccountResponseModel
                {
                    id = item.Id,
                    username = item.Username,
                    first_name = item.FirstName,
                    last_name = item.LastName == null ? "" : item.LastName
                });
            }
        }
        return responseList;
    }

    async Task<Boolean> IAccountRepository.SetManager(int id, int mangerId)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == id);
        try
        {
            user.Supervisor = await _context.Users.FirstAsync(u => u.Id == mangerId);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return false;
        }
        return true;
    }

}