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

        [SwaggerOperation(Summary = "Endpoint to add a price record")]
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

        [SwaggerOperation(Summary = "Endpoint to update a price record")]
        [HttpPut("UpdatePrice")]
        public async Task<ActionResult<ViewApiResponse>> UpdatePrice(UpdatePriceDto updatePriceDto)
        {
            if (!ModelState.IsValid)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = "Bad Request",
                    ResponseData = ModelState
                };

            var response = await price.UpdatePrice(updatePriceDto);
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(404))
                return StatusCode(StatusCodes.Status404NotFound, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [SwaggerOperation(Summary = "Endpoint to delete a price record(s)")]
        [HttpDelete("DeletePrice/{priceId}")]
        public async Task<ActionResult<ViewApiResponse>> DeletePrice(int priceId)
        {
            var response = await price.DeletePrice(priceId);

            if (response.ResponseStatus.Equals(500))
                if (response.ResponseStatus.Equals(500))
                    return StatusCode(StatusCodes.Status500InternalServerError, response);

                else if (response.ResponseStatus.Equals(404))
                    return StatusCode(StatusCodes.Status404NotFound, response);

                else if (response.ResponseStatus.Equals(400))
                    return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        //[SwaggerOperation(Summary = "Endpoint to return list of all prices")]
        //[HttpGet("GetPrices")]
        //public async Task<ActionResult<ViewApiResponse>> GetPrices(){
        //    var response = await price.GetPrices();

        //    if (response.ResponseStatus.Equals(500))
        //        return StatusCode(StatusCodes.Status500InternalServerError, response);
            
        //    else if (response.ResponseStatus.Equals(400))
        //        return StatusCode(StatusCodes.Status400BadRequest, response);

        //    return StatusCode(StatusCodes.Status200OK, response);
        //}
    }
}
