using testKPMG.DTOs.Products;

namespace testKPMG.AuxTools.models
{
    public class MassiveProductResponse
    {
        public List<PostProductDTO>? unAllowrecords { get; set; }
        public string? Message { get; set; }
    }
}
