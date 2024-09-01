using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using src.App_Data.Context;

namespace src.Controllers
{
    public class EFCoreController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public EFCoreController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> ToListSample()
        {
            // States starting with letter S with cities 

            var states = await _appDbContext.States
                .ToListAsync();

            return await Task.Run(() => View("State", states));
        }

        public async Task<IActionResult> IncludeSample()
        {
            // States starting with letter S with cities 

            var states = await _appDbContext.States
                .Include(s => s.Cities)
                .ToListAsync();

            return await Task.Run(() => View("state", states));
        }

        public async Task<IActionResult> ManualJoin()
        {
            var query = _appDbContext.States
                .Join(_appDbContext.Cities, s => s.Id, c => c.StateId, (s, c) => new
                {
                    s.StateName,
                    c.CityName,
                    c.Population
                })
                .AsQueryable();

            var sql = query.ToQueryString();

            return await Task.Run(() => View());
        }
    }
}
