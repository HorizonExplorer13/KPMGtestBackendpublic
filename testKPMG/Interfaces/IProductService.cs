using testKPMG.AuxTools.models;
using testKPMG.DTOs.Products;

namespace testKPMG.Interfaces
{
    public interface IProductService
    {
        Task CreateProduct(PostProductDTO postProduct);
        Task DeleteProduct(Guid Id);
        Task<List<GetListProductsDTO>> GetProductsList();
        Task<GetListProductsDTO> GetProductById(Guid Id);
        Task UpdateProduct(Guid Id, PostProductDTO updateProduct);
        Task<MassiveProductResponse> CreateMassiveProducts(List<PostProductDTO> postProductDTOs);
    }
}
