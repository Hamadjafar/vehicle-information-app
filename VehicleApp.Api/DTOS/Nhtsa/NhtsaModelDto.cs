using System.Text.Json.Serialization;

namespace VehicleApp.Api.DTOS.Nhtsa
{
    public class NhtsaModelDto
    {
        [JsonPropertyName("Make_ID")]
        public int MakeId { get; set; }

        [JsonPropertyName("Make_Name")]
        public string MakeName { get; set; }

        [JsonPropertyName("Model_ID")]
        public int ModelId { get; set; }

        [JsonPropertyName("Model_Name")]
        public string ModelName { get; set; }
    }
}
