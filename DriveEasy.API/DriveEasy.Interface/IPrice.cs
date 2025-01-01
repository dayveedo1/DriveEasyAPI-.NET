using DriveEasy.API.DriveEasy.Dto;

namespace DriveEasy.API.DriveEasy.Interface
{
    public interface IPrice
    {
        Task<ViewApiResponse> CreatePrice(PriceDto priceDto);
        Task<ViewApiResponse> UpdatePrice(UpdatePriceDto updatePriceDto);
        Task<ViewApiResponse> DeletePrice(int priceId);
        //Task<ViewApiResponse> GetPrices();
    }
}
