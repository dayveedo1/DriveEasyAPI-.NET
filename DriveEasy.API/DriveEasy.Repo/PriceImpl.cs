using DriveEasy.API.DriveEasy.Dto;
using DriveEasy.API.DriveEasy.Interface;
using DriveEasy.API.DriveEasy.Models;

namespace DriveEasy.API.DriveEasy.Repo
{
    public class PriceImpl : IPrice
    {
        private readonly DriveEasyApiDbContext context;

        public PriceImpl(DriveEasyApiDbContext context)
        {
            this.context = context;
        }

        public async Task<ViewApiResponse> CreatePrice(PriceDto priceDto)
        {
            if (priceDto == null)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = "Bad Request",
                    ResponseData = null
                };

            Price price = new()
            {
                CarType = priceDto.CarType,
                Location = priceDto.Location,
                RentalPrice = priceDto.RentalPrice
            };

            var response = await context.Prices.AddAsync(price);
            await context.SaveChangesAsync();


            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Price created successfully",
                ResponseData = response.Entity
            };

        }

        public async Task<ViewApiResponse> UpdatePrice(UpdatePriceDto updatePriceDto)
        {
            if (updatePriceDto is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            var getPriceRecord = await context.Prices.FindAsync(updatePriceDto.PriceId);
            if (getPriceRecord is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 404,
                    ResponseMessage = $"Record not found.",
                    ResponseData = null
                };

            var entityToBeModified = getPriceRecord;
            entityToBeModified.RentalPrice = updatePriceDto.RentalPrice;
            entityToBeModified.CarType = updatePriceDto.CarType;
            entityToBeModified.Location = updatePriceDto.Location;

            context.Prices.Update(entityToBeModified);
            await context.SaveChangesAsync();

            var response = await context.Prices.FindAsync(entityToBeModified.PriceId);
            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Car updated successfully",
                ResponseData = response
            };
        }

        public async Task<ViewApiResponse> DeletePrice(int priceId)
        {
            if (priceId < 0)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            var findPriceRecord = await context.Prices.FindAsync(priceId);
            if (findPriceRecord is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 404,
                    ResponseMessage = $"Record not found.",
                    ResponseData = null
                };

            context.Prices.Remove(findPriceRecord);
            await context.SaveChangesAsync();

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Record deleted successfully!",
                ResponseData = null
            };
        }
    }
}
