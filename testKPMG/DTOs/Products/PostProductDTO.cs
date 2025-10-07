namespace testKPMG.DTOs.Products
{
    public record PostProductDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
