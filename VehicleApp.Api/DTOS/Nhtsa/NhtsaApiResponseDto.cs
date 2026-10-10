namespace VehicleApp.Api.DTOS.Nhtsa
{
    public class NhtsaApiResponseDto<T>
    {
        public int Count { get; set; }
        public string Message { get; set; }
        public string SearchCriteria { get; set; }
        public List<T> Results { get; set; }
    }
}
