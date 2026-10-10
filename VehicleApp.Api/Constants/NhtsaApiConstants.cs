namespace VehicleApp.Api.Constants
{
    public static class NhtsaApiConstants
    {
        public const string GetAllMakes = "vehicles/getallmakes?format=json";
        public const string GetVehicleTypesForMakeId = "vehicles/GetVehicleTypesForMakeId/{0}?format=json";
        public const string GetModelsForMakeIdYear = "vehicles/GetModelsForMakeIdYear/makeId/{0}/modelyear/{1}?format=json";
    }
}
