using VehicleApp.Api.Constants;
using VehicleApp.Api.DTOS.Nhtsa;
using VehicleApp.Api.DTOS.Responses;


namespace VehicleApp.Api.Services
{
    public class NhtsaService
    {
        private readonly HttpClient _httpClient;

        public NhtsaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MakeResponseDto>> GetAllMakesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<NhtsaApiResponseDto<NhtsaMakeDto>>(NhtsaApiConstants.GetAllMakes);
            if (response is null)
            {
                throw new HttpRequestException("NHTSA returned an empty response.");
            }

            return response.Results
                .Select(make => new MakeResponseDto
                {
                    Id = make.MakeId,
                    MakeName = make.MakeName
                })
                .ToList();
        }

        public async Task<List<VehicleTypeResponseDto>> GetVehicleTypesForMakeAsync(int makeId)
        {

            if (makeId <= 0)
            {
                throw new ArgumentException("Make ID must be greater than zero.");
            }

            var response = await _httpClient.GetFromJsonAsync<NhtsaApiResponseDto<NhtsaVehicleTypeDto>>(string.Format(NhtsaApiConstants.GetVehicleTypesForMakeId, makeId));

            if (response is null)
            {
                throw new HttpRequestException("NHTSA returned an empty response.");
            }

            return response.Results
                .Select(vehicleType => new VehicleTypeResponseDto
                {
                    VehicleTypeId = vehicleType.VehicleTypeId,
                    VehicleTypeName = vehicleType.VehicleTypeName
                })
                .ToList();
        }


        public async Task<List<VehicleModelResponseDto>> GetModelsForMakeYearAsync(int makeId, int year)
        {

            if (makeId <= 0 || year <= 0)
            {
                throw new ArgumentException("Make ID and year must be greater than zero.");
            }
            var endpoint = string.Format(NhtsaApiConstants.GetModelsForMakeIdYear, makeId, year);

            var response = await _httpClient.GetFromJsonAsync<NhtsaApiResponseDto<NhtsaModelDto>>(endpoint);
            if (response is null)
            {
                throw new HttpRequestException("NHTSA returned an empty response.");
            }

            return response.Results
                .Select(model => new VehicleModelResponseDto
                {
                    ModelId = model.ModelId,
                    ModelName = model.ModelName
                })
                .ToList();
        }
    }
}
