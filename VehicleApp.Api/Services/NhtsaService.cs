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
            try
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
            catch (HttpRequestException)
            {
                throw;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
        }
    }
}
