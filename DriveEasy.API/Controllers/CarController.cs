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
    public class CarController : Controller
    {
       private readonly ICar car;

        public CarController(ICar car)
        {
            this.car = car;
        }

        [SwaggerOperation(Summary = "Endpoint to a car record")]
        [HttpPost("CreateCar")]
        public async Task<ActionResult<ViewApiResponse>> CreateCar([FromBody] CarDto carDto)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = "Bad Request",
                    ResponseData = ModelState
                });

            var response = await car.CreateCar(carDto);
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [SwaggerOperation(Summary = "Endpoint to return car details by Car ID")]
        [HttpGet("GetCarById/{carId}")]
        public async Task<ActionResult<ViewApiResponse>> GetCarById(int carId)
        {
            var response = await car.GetCarById(carId);
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            else if (response.ResponseStatus.Equals(404))
                return StatusCode(StatusCodes.Status404NotFound, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [SwaggerOperation(Summary = "Endpoint to return all cars details")]
        [HttpGet("GetCars")]
        public async Task<ActionResult<ViewApiResponse>> GetCars()
        {
            var response = await car.GetCars();
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [SwaggerOperation(Summary = "Endpoint to update car details")]
        [HttpPut("UpdateCar")]
        public async Task<ActionResult<ViewApiResponse>> UpdateCar([FromBody] CarUpdateDto carUpdateDto)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = "Bad Request",
                    ResponseData = ModelState
                });

            var response = await car.UpdateCar(carUpdateDto);
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [SwaggerOperation(Summary = "Endpoint to delete car details by Car ID")]
        [HttpDelete("DeleteCar/{carId}")]
        public async Task<ActionResult<ViewApiResponse>> DeleteCar(int carId)
        {
            var response = await car.DeleteCar(carId);
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            else if (response.ResponseStatus.Equals(404))
                return StatusCode(StatusCodes.Status404NotFound, response);

            return StatusCode(StatusCodes.Status200OK, response);

        }   
    }
}
