using ASP.NET_API.Data;
using ASP.NET_API.Services;
using AutoMapper;
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
    [Route("api/apikeys")]
    public class ApiKeysController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly APIKeyService _keyService;

        public ApiKeysController(ApplicationDbContext context, IMapper mapper, APIKeyService keyService)
        {
            _context = context;
            _mapper = mapper;
            _keyService = keyService;
        }

        [HttpGet]
        public async Task<APIKeyDTO[]> GetMyKeys()
        {
            var userId = GetUserId();
            var keys = await _context.APIKeys.Where(x => x.UserId == userId).ToArrayAsync();
            return _mapper.Map<APIKeyDTO[]>(keys);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAPIKey(APIKeyCreateDTO aPIKeyCreateDTO)
        {
            var userId = GetUserId();

            if (aPIKeyCreateDTO.KeyType == Shared.Entities.KeyType.Free)
            {
                var userAlreadyHaveFreeKey = await _context.APIKeys.AnyAsync(x => x.UserId == userId &&
                                                                                  x.KeyType == KeyType.Free);

                if (userAlreadyHaveFreeKey)
                    return BadRequest("The user already own a free key");
            }

            await _keyService.CreateKey(userId, aPIKeyCreateDTO.KeyType);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAPIKey(APIKeyUpdateDTO aPIKeyUpdateDTO)
        {
            var userId = GetUserId();

            var apiKey = await _context.APIKeys.FirstOrDefaultAsync(x => x.Id == aPIKeyUpdateDTO.Id);

            if (apiKey is null)
                return NotFound();

            if (userId != apiKey.UserId)
                return Forbid();

            if (aPIKeyUpdateDTO.UpdateKey)
                apiKey.Key = _keyService.GenerateKey();

            apiKey.Active = aPIKeyUpdateDTO.Active;

            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
