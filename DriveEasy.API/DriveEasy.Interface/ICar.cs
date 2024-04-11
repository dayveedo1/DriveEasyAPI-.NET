using DriveEasy.API.DriveEasy.Dto;

namespace DriveEasy.API.DriveEasy.Interface
{
    public interface ICar
    {
        Task<ViewApiResponse> CreateCar(CarDto carDto);
        Task<ViewApiResponse> GetCarById(int carId);
        Task<ViewApiResponse> GetCars();
        Task<ViewApiResponse> UpdateCar(CarUpdateDto carUpdateDto);
        Task<ViewApiResponse> DeleteCar(int carId);
    }
}
