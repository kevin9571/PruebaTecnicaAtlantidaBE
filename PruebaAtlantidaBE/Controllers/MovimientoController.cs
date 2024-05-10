using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaAtlantidaBE.Models;
using PruebaAtlantidaBE.Shared;
using static ClosedXML.Excel.XLPredefinedFormat;
using DateTime = System.DateTime;

namespace PruebaAtlantidaBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoController : ControllerBase
    {
        private readonly DbPruebaAtlantida _context;

        public MovimientoController(DbPruebaAtlantida context)
        {
            _context = context;
        }


        [HttpGet("Compras/{IdtarjetaCredito}")]
        public async Task<IActionResult> GetCompras(int IdtarjetaCredito)
        {
            // Obtén la fecha del primer día del mes actual
            DateTime primerDiaMesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var Movimientos = await _context.Movimientos.Where(c => 
                    c.IdtarjetaCredito == IdtarjetaCredito && 
                    c.IdTipoMovimiento == 1 &&
                    c.Fecha >= primerDiaMesActual)
                .ToListAsync();

            if (Movimientos == null) return NotFound();

            return Ok(Movimientos);
        }

        [HttpGet("Historial/{IdtarjetaCredito}")]
        public async Task<IActionResult> Historial(int IdtarjetaCredito)
        {
            // Obtén la fecha del primer día del mes actual
            DateTime primerDiaMesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var Movimientos = await _context.Movimientos
                    .Where(c => c.IdtarjetaCredito == IdtarjetaCredito &&
                                c.Fecha >= primerDiaMesActual)
                    .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            if (Movimientos == null) return NotFound();

            return Ok(Movimientos);
        }

        [HttpGet("MontoTotalActual/{IdtarjetaCredito}")]
        public async Task<IActionResult> GetMontoTotalActual(int IdtarjetaCredito)
        {
            // Obtén la fecha del primer día del mes actual
            DateTime primerDiaMesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var Movimientos = await _context.Movimientos
                .Where(c => c.IdtarjetaCredito == IdtarjetaCredito &&
                            c.IdTipoMovimiento == 1 &&
                            c.Fecha >= primerDiaMesActual)
                .ToListAsync();

            if (Movimientos == null) return Ok(0);

            double Total=0;

            foreach(var Movimiento in Movimientos)
            {
                Total = Total + Movimiento.Monto;
            }

            return Ok(Total);
        }

        [HttpGet("MontoTotalAnterior/{IdtarjetaCredito}")]
        public async Task<IActionResult> GetMontoTotalAnterior(int IdtarjetaCredito)
        {
            // Obtén la fecha del primer día del mes anterior
            DateTime primerDiaMesAnterior = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1);
            // Obtén la fecha del primer día del mes actual
            DateTime primerDiaMesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var Movimientos = await _context.Movimientos
                .Where(c => c.IdtarjetaCredito == IdtarjetaCredito &&
                            c.IdTipoMovimiento == 1 &&
                            c.Fecha >= primerDiaMesAnterior &&
                            c.Fecha < primerDiaMesActual)
                .ToListAsync();

            if (Movimientos == null) return Ok(0);

            double Total = 0;

            foreach (var Movimiento in Movimientos)
            {
                Total = Total + Movimiento.Monto;
            }

            return Ok(Total);
        }

        [HttpGet("DescargarCompras/{IdtarjetaCredito}")]
        public async Task<IActionResult> DescargarCompras(int IdtarjetaCredito)
        {
            // Obtén la fecha del primer día del mes actual
            DateTime primerDiaMesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var Movimientos = await _context.Movimientos                
                .Where(c =>
                    c.IdtarjetaCredito == IdtarjetaCredito &&
                    c.IdTipoMovimiento == 1 &&
                    c.Fecha >= primerDiaMesActual)
                .Select(c => new
                {
                    Fecha = c.Fecha.ToString("dd/MM/yyyy"),
                    Descripción = c.Descripción,
                    Monto = c.Monto
                })
                .ToListAsync();

            if (Movimientos == null) return NotFound();

            var DT = Utilidades.ConvertirListaADataTable(Movimientos);

            var archivoExcel = Utilidades.DescargarExcelDatatable(DT,$"Compras - {DateTime.Now.ToString("dd/MM/yyyy")}","Datos");

            return File(archivoExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Compras - {DateTime.Now.ToString("dd/MM/yyyy")}.xlsx");
        }

        [HttpPost("RegistrarCompra")]
        public async Task<IActionResult> RegistrarCompra(MovimientoCLS Movimiento)
        {

            if (Movimiento.Descripción == null) Movimiento.Descripción = "";

            MovimientoValidator validator = new MovimientoValidator();
            ValidationResult result = validator.Validate(Movimiento);

            if (!result.IsValid || !ModelState.IsValid)
            {
                return BadRequest();
            }

            if (Movimiento == null) return NotFound();
            Movimiento.IdTipoMovimiento = 1;

            //Agrego el Movimiento
            await _context.Movimientos.AddAsync(Movimiento);
            await _context.SaveChangesAsync();

            //Actualizo el Saldo Actual
            _context.ActualizarSaldos_Compra(Movimiento.IdtarjetaCredito, (float)Movimiento.Monto);

            /*var Tarjeta = await _context.TarjetaCredito.FirstOrDefaultAsync(c => c.Id == Movimiento.IdtarjetaCredito);
            
            if (Tarjeta != null)
            {
                Tarjeta.SaldoActual = Tarjeta.SaldoActual + Movimiento.Monto;
                Tarjeta.SaldoDisponible = Tarjeta.SaldoDisponible - Movimiento.Monto;
                _context.TarjetaCredito.Update(Tarjeta);
                await _context.SaveChangesAsync();
            }*/

            return CreatedAtAction("RegistrarCompra", Movimiento.Id, Movimiento);
        }

        [HttpPost("RegistrarPago")]
        public async Task<IActionResult> RegistrarPago(MovimientoCLS Movimiento)
        {
            if (Movimiento.Descripción == null) Movimiento.Descripción = "";

            MovimientoValidator validator = new MovimientoValidator();
            ValidationResult result = validator.Validate(Movimiento);

            if (!result.IsValid || !ModelState.IsValid)
            {
                return BadRequest();
            }

            if (Movimiento == null) return NotFound();
            Movimiento.IdTipoMovimiento = 2;

            //No pide que se registre la descripcion
            Movimiento.Descripción = "";

            //Agrego el Movimiento
            await _context.Movimientos.AddAsync(Movimiento);
            await _context.SaveChangesAsync();

            //Actualizo el Saldo Actual
            _context.ActualizarSaldos_Pago(Movimiento.IdtarjetaCredito, (float)Movimiento.Monto);

            /*var Tarjeta = await _context.TarjetaCredito.FirstOrDefaultAsync(c => c.Id == Movimiento.IdtarjetaCredito);

            if (Tarjeta != null)
            {
                Tarjeta.SaldoActual = Tarjeta.SaldoActual - Movimiento.Monto;
                Tarjeta.SaldoDisponible = Tarjeta.SaldoDisponible + Movimiento.Monto;
                _context.TarjetaCredito.Update(Tarjeta);
                await _context.SaveChangesAsync();
            }*/

            return CreatedAtAction("RegistrarPago", Movimiento.Id, Movimiento);
        }




    }
}
