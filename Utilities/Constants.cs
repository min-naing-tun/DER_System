using Microsoft.Extensions.Options;

namespace DER_System.Utilities
{
    public class Constants
    {
        //Mappings Table
        public string MaterialType = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["TableMapping:MaterialType"]!;
        public string Route = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["TableMapping:Route"]!;
        public string User = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["TableMapping:User"]!;
        public string Customer = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["TableMapping:Customer"]!;
        public string Material = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["TableMapping:Material"]!;
        public string CustomerMaterialListing = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["TableMapping:CustomerMaterialListing"]!;
        public string CustomerRouteListing = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["TableMapping:CustomerRouteListing"]!;
    }
}
