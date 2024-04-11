using DriveEasy.API.DriveEasy.Dto;

namespace DriveEasy.API.DriveEasy.Interface
{
    public interface IPrice
    {
        Task<ViewApiResponse> CreatePrice(PriceDto priceDto);
    }
}
