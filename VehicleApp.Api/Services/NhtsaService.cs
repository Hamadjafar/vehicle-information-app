using Microsoft.Extensions.Caching.Memory;
using VehicleApp.Api.Constants;
using VehicleApp.Api.DTOS.Nhtsa;
using VehicleApp.Api.DTOS.Responses;


namespace VehicleApp.Api.Services
{
    public class NhtsaService
    {
        private const string AllMakesCacheKey = "nhtsa:all-makes";
        private static readonly TimeSpan AllMakesCacheDuration = TimeSpan.FromHours(24);

        private static readonly SemaphoreSlim AllMakesLock = new(1, 1);

        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public NhtsaService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<IReadOnlyList<MakeResponseDto>> SearchMakesAsync(string? search, int take, CancellationToken cancellationToken = default)
        {
            var makes = await GetAllMakesAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(search))
            {
                return makes.Take(take).ToList();
            }

            var term = search.Trim();

            
            return makes
                .Where(make => make.MakeName.Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(make => make.MakeName.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                .ThenBy(make => make.MakeName, StringComparer.OrdinalIgnoreCase)
                .Take(take)
                .ToList();
        }

        public async Task<IReadOnlyList<MakeResponseDto>> GetAllMakesAsync(CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(AllMakesCacheKey, out IReadOnlyList<MakeResponseDto>? cached) && cached is not null)
            {
                return cached;
            }

            await AllMakesLock.WaitAsync(cancellationToken);
            try
            {
                if (_cache.TryGetValue(AllMakesCacheKey, out cached) && cached is not null)
                {
                    return cached;
                }

                var makes = await FetchAllMakesAsync(cancellationToken);
                _cache.Set(AllMakesCacheKey, makes, AllMakesCacheDuration);

                return makes;
            }
            finally
            {
                AllMakesLock.Release();
            }
        }

        private async Task<IReadOnlyList<MakeResponseDto>> FetchAllMakesAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetFromJsonAsync<NhtsaApiResponseDto<NhtsaMakeDto>>(NhtsaApiConstants.GetAllMakes, cancellationToken);
            if (response is null)
            {
                throw new HttpRequestException("NHTSA returned an empty response.");
            }

            // NHTSA data has padded names, blanks and repeated IDs, so clean it once before caching.
            return response.Results
                .Where(make => !string.IsNullOrWhiteSpace(make.MakeName))
                .DistinctBy(make => make.MakeId)
                .Select(make => new MakeResponseDto
                {
                    Id = make.MakeId,
                    MakeName = make.MakeName.Trim()
                })
                .OrderBy(make => make.MakeName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<List<VehicleTypeResponseDto>> GetVehicleTypesForMakeAsync(int makeId, CancellationToken cancellationToken = default)
        {

            if (makeId <= 0)
            {
                throw new ArgumentException("Make ID must be greater than zero.");
            }

            var response = await _httpClient.GetFromJsonAsync<NhtsaApiResponseDto<NhtsaVehicleTypeDto>>(string.Format(NhtsaApiConstants.GetVehicleTypesForMakeId, makeId), cancellationToken);

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


        public async Task<List<VehicleModelResponseDto>> GetModelsForMakeYearAsync(int makeId, int year, CancellationToken cancellationToken = default)
        {

            if (makeId <= 0 || year <= 0)
            {
                throw new ArgumentException("Make ID and year must be greater than zero.");
            }
            var endpoint = string.Format(NhtsaApiConstants.GetModelsForMakeIdYear, makeId, year);

            var response = await _httpClient.GetFromJsonAsync<NhtsaApiResponseDto<NhtsaModelDto>>(endpoint, cancellationToken);
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
