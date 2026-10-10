using System.Text.Json.Serialization;

namespace VehicleApp.Api.DTOS.Nhtsa
{
    public class NhtsaMakeDto
    {
        [JsonPropertyName("Make_ID")]
        public int MakeId { get; set; }

        [JsonPropertyName("Make_Name")]
        public string MakeName { get; set; } = string.Empty;
    }
}
