using ASP.NET_API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Entities;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class RestrictionsByIPController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;

        public RestrictionsByIPController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(RestrictionByIPCreateDTO restrictionByIPCreateDTO)
        {
            var keyDB = await _context.APIKeys.FirstOrDefaultAsync(x => x.Id == restrictionByIPCreateDTO.KeyId);

            if (keyDB is null)
                return NotFound();

            var userId = GetUserId();

            if (keyDB.UserId != userId)
                return Forbid();

            var domainRestriction = new RestrictionByIP
            {
                KeyId = restrictionByIPCreateDTO.KeyId,
                IP = restrictionByIPCreateDTO.IP,
            };

            await _context.RestrictionsByIP.AddAsync(domainRestriction);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, RestrictionByIPUpdateDTO restrictionByIPUpdateDTO)
        {
            var restrictionDB = await _context.RestrictionsByIP.Include(r => r.Key).FirstOrDefaultAsync(r => r.Id == id);

            if (restrictionDB is null)
                return NotFound();

            var userId = GetUserId();

            if (restrictionDB.Key.UserId != userId)
                return Forbid();

            restrictionDB.IP = restrictionByIPUpdateDTO.IP;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restrictionDB = await _context.RestrictionsByIP.Include(r => r.Key).FirstOrDefaultAsync(r => r.Id == id);

            if (restrictionDB is null)
                return NotFound();

            var userId = GetUserId();

            if (restrictionDB.Key.UserId != userId)
                return Forbid();

            _context.RestrictionsByIP.Remove(restrictionDB);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
