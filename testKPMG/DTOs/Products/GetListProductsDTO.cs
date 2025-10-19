using System.Text.Json.Serialization;

namespace testKPMG.DTOs.Products
{
    public record GetListProductsDTO
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        [JsonPropertyName("Price")]
        public decimal Price { get; set; }
        [JsonPropertyName("Stock")]
        public int Stock { get; set; }
    }
}
