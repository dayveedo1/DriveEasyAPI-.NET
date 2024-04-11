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
                ResponseMessage = "Price created successfully",
                ResponseData = response.Entity
            };

        }
    }
}
