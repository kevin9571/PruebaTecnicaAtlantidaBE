using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaAtlantidaBE.Models;

namespace PruebaAtlantidaBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarjetaCreditoController : ControllerBase
    {
        private readonly DbPruebaAtlantida _context;

        public TarjetaCreditoController(DbPruebaAtlantida context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<TarjetaCreditoCLS>> Get()
        {
            return await _context.TarjetaCredito.ToListAsync();
        }

        [HttpGet("{Numero}")]
        public async Task<IActionResult> GetTarjeta(string Numero)
        {
            var tarjeta = await _context.TarjetaCredito.FirstOrDefaultAsync(c => c.Numero == Numero);

            if (tarjeta == null) return NotFound();

            return Ok(tarjeta);
        }


    }
}
