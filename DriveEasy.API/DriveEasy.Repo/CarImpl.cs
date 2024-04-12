using DriveEasy.API.DriveEasy.Dto;
using DriveEasy.API.DriveEasy.Interface;
using DriveEasy.API.DriveEasy.Models;
using Microsoft.EntityFrameworkCore;

namespace DriveEasy.API.DriveEasy.Repo
{
    public class CarImpl : ICar
    {
        /* DbContext DI */
        private readonly DriveEasyApiDbContext context;

        public CarImpl(DriveEasyApiDbContext context)
        {
            this.context = context;
        }


        public async Task<ViewApiResponse> CreateCar(CarDto carDto)
        {
            if (carDto is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            var isCarExist = await context.Cars.AnyAsync(x => x.Make == carDto.Make && x.Model == carDto.Model && x.Year == carDto.Year);
            if (isCarExist)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Record already exist",
                    ResponseData = null
                };

            Car car = new()
            {
                Make = carDto.Make,
                Model = carDto.Model,
                Year = carDto.Year,
                Type = carDto.Type,
                Mileage = carDto.Mileage,
                Location = carDto.Location,
                Availability = carDto.Availability
            };

            var response = await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Record created successfully",
                ResponseData = response.Entity
            };

        }

        public async Task<ViewApiResponse> DeleteCar(int carId)
        {
            if (carId < 0)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            var car = await context.Cars.FindAsync(carId);

            if (car is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 404,
                    ResponseMessage = $"Record Not Found",
                    ResponseData = null
                };

            switch (car.Availability)
            {
                case true:
                    context.Cars.Remove(car);
                    await context.SaveChangesAsync();
                    return new ViewApiResponse
                    {
                        ResponseStatus = 200,
                        ResponseMessage = $"Record deleted successfully",
                        ResponseData = null
                    };

                case false:
                    return new ViewApiResponse
                    {
                        ResponseStatus = 400,
                        ResponseMessage = $"Record is not available",
                        ResponseData = null
                    };
            }
           
        }

        public async Task<ViewApiResponse> GetCarById(int carId)
        {
            if (carId < 0)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            var response = await context.Cars.FindAsync(carId);
            if (response is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 404,
                    ResponseMessage = $"Record Not Found",
                    ResponseData = null
                };

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Success",
                ResponseData = response
            };
        }

        public async Task<ViewApiResponse> GetCars()
        {
            var response = await context.Cars.ToListAsync();
            if (response is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            else if (response.Count.Equals(0))
                return new ViewApiResponse
                {
                    ResponseStatus = 200,
                    ResponseMessage = $"Success",
                    ResponseData = response
                };

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Success",
                ResponseData = response
            };
        }

        public async Task<ViewApiResponse> UpdateCar(CarUpdateDto carUpdateDto)
        {
            if (carUpdateDto is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            var car = await context.Cars.FindAsync(carUpdateDto.CarId);

            if (car is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 404,
                    ResponseMessage = $"Car Not Found",
                    ResponseData = null
                };

            var entityToBeModified = car;
            entityToBeModified.Make = carUpdateDto.Make;
            entityToBeModified.Year = carUpdateDto.Year;
            entityToBeModified.Model = carUpdateDto.Model;
            entityToBeModified.Type = carUpdateDto.Type;
            entityToBeModified.Mileage = carUpdateDto.Mileage;
            entityToBeModified.Location = carUpdateDto.Location;
            entityToBeModified.Availability = carUpdateDto.Availability;

            context.Cars.Update(entityToBeModified);
            await context.SaveChangesAsync();

            var response = await context.Cars.FindAsync(entityToBeModified.CarId);

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Car updated successfully",
                ResponseData = response
            };
        }
    }
}
