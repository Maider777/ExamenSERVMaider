using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cripto.Models;

namespace CriptoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly CryptoContext db;

        public QueryController(CryptoContext context)
        {
            db = context;
        }

        [HttpGet("0")]
        public ActionResult Query0(int ValorActual = 50)
        {
            // Ejemplo de método en controlador
            var list = db.Moneda.ToListAsync();

            return Ok(new
            {
                ValorActual = "Parámetros para usar cuando sea posible",
                Descripcion = "Ejemplo en MODO NO ASYNC - DEBE SER ASÍNCRONOS",
                Valores = list,
            });
        }

        [HttpGet("/query1/")]
        public async Task<ActionResult> Query1()
        {
            var list=db.Moneda.Where(m => m.Actual >= 50).OrderBy(m => m.MonedaId);

            return Ok(new
            {
                ValorActual = "Parámetros para usar cuando sea posible",
                Descripcion = "Monedas con valor actual superior a 50€ ordenadas alfabéticamente",
                Valores = list,
            });

        }

        [HttpGet("/query2/")]
        public async Task<ActionResult> Query2()
        {
            var list=db.Contrato.GroupBy(c => c.CarteraId).Select(c => new{
                CarteraId=c.Key,
                TotalMonedas=c.Count()
            }).Where(c => c.TotalMonedas > 2).ToListAsync();

            return Ok(new
            {
                ValorActual = "Parámetros para usar cuando sea posible",
                Descripcion = "Carteras con más de 2 monedas contratadas",
                Valores = list,
            });

        }

        [HttpGet("/query3/")]
        public async Task<ActionResult> Query3()
        {
            var list=await db.Cartera.GroupBy(c => c.Exchange).Select(f => new{
                Exchange=f.Key,
                TotalCarteras=f.Count()
            }).OrderByDescending(f=>f.TotalCarteras).ToListAsync();

            return Ok(new
            {
                ValorActual = "Parámetros para usar cuando sea posible",
                Descripcion = "Exchanges ordenados por números de carteras",
                Valores = list,
            });

        }

        [HttpGet("/query4/")]
        public async Task<ActionResult> Query4()
        {
            var list = await db.Contrato.Join(db.Cartera, Contrato => Contrato.CarteraId, Cartera => Cartera.CarteraId, (Contrato, Cartera) => new {
                Contrato = Contrato, Cartera = Cartera 
                }).GroupBy(join => join.Cartera.Exchange).Select(join => new { 
                Exchange = join.Key, TotalMonedas = join.Count() 
                }).OrderByDescending(join => join.TotalMonedas).ToListAsync();

            return Ok(new
            {
                ValorActual = "Parámetros para usar cuando sea posible",
                Descripcion = "Exchanges ordenados por cantidad de monedas",
                Valores = list,
            });

        }

        [HttpGet("/query5/")]
        public async Task<ActionResult> Query5()
        {
            var list= await db.Moneda.Select(m => new {
                    moneda = m.MonedaId,
                    valorActual = m.Actual
                }).OrderBy(m => m.valorActual).ToListAsync();

            return Ok(new
            {
                ValorActual = "Parámetros para usar cuando sea posible",
                Descripcion = "Monedas en contratos ordenadas por valor total actual",
                Valores = list,
            });

        }

        [HttpGet("/query6/")]
        public async Task<ActionResult> Query6()
        {
            var list= await db.Moneda.GroupBy(x => x.MonedaId).Select(a => new {
                    Moneda = a.Key,
                    ValorTotal = a.Sum(x => x.Actual)
                }).OrderByDescending(e => e.ValorTotal).ToListAsync();

            return Ok(new
            {
                ValorActual = "Parámetros para usar cuando sea posible",
                Descripcion = "Monedas en contratos ordenadas por valor actual total en todos los contratos",
                Valores = list,
            });

        }
    }
}
