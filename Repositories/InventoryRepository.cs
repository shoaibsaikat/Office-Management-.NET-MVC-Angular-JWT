using Microsoft.EntityFrameworkCore;
using Office_Management_.NET_MVC_Angular_JWT.Models;
using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;

namespace Office_Management_.NET_MVC_Angular_JWT.Repositories;

class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICommonUtil _common_util;

    public InventoryRepository(ApplicationDbContext context, ICommonUtil commonUtil)
    {
        _context = context;
        _common_util = commonUtil;
    }

    async Task<IEnumerable<InventoryResponseModel>> IInventoryRepository.GetAllList(int? page)
    {
        List<Inventory> list;
        if (page != null)
        {
            list = await _context.Inventories
                                    .Skip((page.Value - 1) * _common_util.GetPageSize())
                                    .Take(_common_util.GetPageSize())
                                    // .Where(i => i.Id >= page * _common_util.GetPageSize() - _common_util.GetPageSize() && i.Id <= page * _common_util.GetPageSize())
                                    .ToListAsync();
        }
        else
        {
            list = await _context.Inventories.ToListAsync();
        }
        var responseList = new List<InventoryResponseModel>();
        foreach (var item in list)
        {
            var model = new InventoryResponseModel
            {
                id = item.Id,
                name = item.Name,
                count = item.Count,
                unit = item.Unit
            };
            responseList.Add(model);
        }
        return responseList;
    }

    async Task<int> IInventoryRepository.GetListCount()
    {
        return await _context.Inventories.CountAsync();
    }

    async Task<InventoryResponseModel?> IInventoryRepository.GetById(int id)
    {
        var item = await _context.Inventories.FirstOrDefaultAsync(i => i.Id == id);
        if (item != null)
        {
            return new InventoryResponseModel
            {
                id = item.Id,
                name = item.Name,
                count = item.Count,
                unit = item.Unit,
                description = item.Description == null ? String.Empty : item.Description,
            };
        }
        return null;
    }

    async Task<Boolean> IInventoryRepository.Update(int id, UInt32 amount)
    {
        var item = await _context.Inventories.FirstOrDefaultAsync(i => i.Id == id);
        if (item != null)
        {
            item.Count = amount;
            item.LastModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    async Task<Boolean> IInventoryRepository.Create(string name, UInt32 count, string unit, string? description)
    {
        var inventory = new Inventory()
        {
            Name = name,
            Count = count,
            Unit = unit,
            Description = description,
            LastModifiedDate = DateTime.UtcNow
        };
        await _context.Inventories.AddAsync(inventory);        
        await _context.SaveChangesAsync();
        return true;
    }
}