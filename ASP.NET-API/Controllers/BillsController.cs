using ASP.NET_API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace ASP.NET_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pay(PayBillDTO payBillDTO)
        {
            var bill = await _context.Bills.Include(b => b.User)
                                           .FirstOrDefaultAsync(b => b.Id == payBillDTO.BillId);

            if (bill is null)
                return NotFound();

            if (bill.Paid)
                return BadRequest("The bill is already paid!");

            //Add interaction with payments api

            bill.Paid = true;
            await _context.SaveChangesAsync();

            var pendingBillExist = await _context.Bills.AnyAsync(b => b.UserId == bill.UserId &&
                                                                      !b.Paid &&
                                                                      b.PaydayLimit < DateTime.Today);

            if (!pendingBillExist)
            {
                bill.User.Debtor = false;
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }
    }
}
