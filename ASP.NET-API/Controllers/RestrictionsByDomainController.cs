using ASP.NET_API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Entities;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class RestrictionsByDomainController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;

        public RestrictionsByDomainController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(RestrictionByDomainCreateDTO restrictionByDomainCreateDTO)
        {
            var keyDB = await _context.APIKeys.FirstOrDefaultAsync(x => x.Id == restrictionByDomainCreateDTO.KeyId);

            if (keyDB is null)
                return NotFound();

            var userId = GetUserId();

            if (keyDB.UserId != userId)
                return Forbid();

            var domainRestriction = new RestrictionByDomain
            {
                KeyId = restrictionByDomainCreateDTO.KeyId,
                Domain = restrictionByDomainCreateDTO.Domain,
            };

            await _context.RestrictionsByDomain.AddAsync(domainRestriction);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, RestrictionByDomainUpdateDTO restrictionByDomainUpdateDTO)
        {
            var restrictionDB = await _context.RestrictionsByDomain.Include(r => r.Key).FirstOrDefaultAsync(r => r.Id == id);

            if (restrictionDB is null)
                return NotFound();

            var userId = GetUserId();

            if (restrictionDB.Key.UserId != userId)
                return Forbid();

            restrictionDB.Domain = restrictionByDomainUpdateDTO.Domain;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restrictionDB = await _context.RestrictionsByDomain.Include(r => r.Key).FirstOrDefaultAsync(r => r.Id == id);

            if (restrictionDB is null)
                return NotFound();

            var userId = GetUserId();

            if (restrictionDB.Key.UserId != userId)
                return Forbid();

            _context.RestrictionsByDomain.Remove(restrictionDB);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
