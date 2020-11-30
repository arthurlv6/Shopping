using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _service;
        public StockController(IStockService service)
        {
            _service = service;
        }
        [HttpPost]
        public IActionResult Post( CartDto model)
        {
            var result = _service.NewOrder(model);
            if (result.IsSuccess)
                return Ok("done");
            return BadRequest(result.Error);
        }
    }
}
