using Microsoft.EntityFrameworkCore;
using testKPMG.AppDbContext;
using testKPMG.AuxTools;
using testKPMG.DTOs.Products;
using testKPMG.Entities;
using testKPMG.Interfaces;

namespace testKPMG.Services
{
    public class ProdutService : IProductService
    {
        private readonly testKPMG.AppDbContext.AppDbContext dbContext;

        public ProdutService(testKPMG.AppDbContext.AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<GetListProductsDTO>> GetProductsList()
        {
            List<GetListProductsDTO> productsList = new List<GetListProductsDTO>();
            var objlist = await dbContext.products.ToListAsync();
            if (objlist.Count == 0)
                throw new NotFoundException("No products were Found");
            foreach (var obj in objlist)
            {
                var item = new GetListProductsDTO
                {
                    Id = obj.Id,
                    Name = obj.Name,
                    Price = obj.Price,
                    Stock = obj.Stock,
                };
                productsList.Add(item);

            }
            return productsList;
        }

        public async Task<GetListProductsDTO> GetProductById(Guid Id)
        {
            var response = await dbContext.products.Select(obj => new GetListProductsDTO
            {
                Id = obj.Id,
                Name= obj.Name,
                Price = obj.Price,
                Stock = obj.Stock,
            }).FirstOrDefaultAsync(p => p.Id == Id);
            if (response == null)
                throw new NotFoundException($"there's no product with that Id: {Id}");
            return response;
        }

        public async Task CreateProduct(PostProductDTO postProduct)
        {

            var existedProduct = await dbContext.products.FirstOrDefaultAsync(p => p.Name == postProduct.Name);
            if (existedProduct != null)
                throw new ConflictException($"there's already a product with that Name: {postProduct.Name}");
            var addProduct = new Product
            {
                Name = postProduct.Name,
                Price = postProduct.Price,
                Stock = postProduct.Stock,
            };
            dbContext.products.Add(addProduct);
            var result = await dbContext.SaveChangesAsync();
            if (result == 0)
                throw new InternalServerException("internal server error");
        }

        public async Task UpdateProduct(Guid Id, PostProductDTO updateProduct)
        {
            var existedProduct = await dbContext.products.FirstOrDefaultAsync(p => p.Name == updateProduct.Name);
            if (existedProduct != null)
                throw new ConflictException($"there's already a product with that Name: {updateProduct.Name}");
            var product = await dbContext.products.FirstOrDefaultAsync(o => o.Id == Id);
            product.Name = updateProduct.Name;
            product.Price = updateProduct.Price;
            product.Stock = updateProduct.Stock;
            dbContext.products.Entry(product).State = EntityState.Modified;
            var result = await dbContext.SaveChangesAsync();
            if (result == 0)
                throw new InternalServerException("internal server error");
        }

        public async Task DeleteProduct(Guid Id)
        {
            var deleteProduct = await dbContext.products.FindAsync(Id);
            dbContext.products.Remove(deleteProduct);
            var result = await dbContext.SaveChangesAsync();
            if (result == 0)
                throw new InternalServerException("internal server error");

        }


        

    }
}
