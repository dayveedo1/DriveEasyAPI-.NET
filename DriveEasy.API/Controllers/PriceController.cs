using DriveEasy.API.DriveEasy.Dto;
using DriveEasy.API.DriveEasy.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DriveEasy.API.Controllers
{
    [ApiController]
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    public class PriceController : Controller
    {
        private readonly IPrice price;

        public PriceController(IPrice price)
        {
            this.price = price;
        }

        [SwaggerOperation(Summary = "Endpoint to a price record")]
        [HttpPost("CreatePrice")]
        public async Task<ActionResult<ViewApiResponse>> CreatePrice(PriceDto priceDto)
        {
            if (!ModelState.IsValid)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = "Bad Request",
                    ResponseData = ModelState
                };

            var response = await price.CreatePrice(priceDto);
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}
