using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;
using testKPMG.AppDbContext;
using testKPMG.AuxTools;
using testKPMG.AuxTools.models;
using testKPMG.DTOs.Products;
using testKPMG.Entities;
using testKPMG.Interfaces;
using ManagerResult = (testKPMG.DTOs.Products.PostProductDTO item, bool isAllow);
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
                Name = obj.Name,
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

        public async Task<MassiveProductResponse> CreateMassiveProducts(List<PostProductDTO> postProductDTOs)
        {
            List<PostProductDTO> listInsert = new List<PostProductDTO>();
            List<PostProductDTO> unAllowrecords = new List<PostProductDTO>();
            #region dataflow pattern
            // ActionBlocks for a productor/customer dataFlow concurrency pattern
            
            var unAllowBlock = new ActionBlock<ManagerResult>(
                        item => { unAllowrecords.Add(item.item); }
                        );
            var allowBlock = new ActionBlock<ManagerResult>(
                        item => { listInsert.Add(item.item); }
                        );

            var managerBlock = new TransformBlock<PostProductDTO, (PostProductDTO item, bool isAllow)>(
                async item =>
                {
                    var existedProduct = await dbContext.products.FirstOrDefaultAsync(p => p.Name == item.Name);
                    if (existedProduct != null)
                    {
                        bool isAllow = false;
                        return (item, isAllow);
                    }
                    else
                    {
                        bool isAllow = true;
                        return (item, isAllow);
                    }

                },
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );

            managerBlock.LinkTo(allowBlock, tuple => tuple.isAllow);
            managerBlock.LinkTo(allowBlock, tuple => tuple.isAllow);
            #endregion
            List<Product> records = new List<Product>();
            foreach (var item in listInsert)
            {
                var record = new Product
                {
                    Name = item.Name,
                    Price = item.Price,
                    Stock = item.Stock,
                };
                records.Add(record);
            }
            dbContext.products.AddRange(records);
            var result = await dbContext.SaveChangesAsync();
            if (result == 0)
                throw new InternalServerException("internal server error");
            string message = "";
            if (unAllowrecords.Count == 0)
                unAllowrecords = null;

            return new MassiveProductResponse
            {
                unAllowrecords = unAllowrecords,
                Message = message = "These records already existed"
            };


            // Sequential logic to departure between those records that already existed on the db table.
            #region Sequetial listcreate
            ////foreach (var postProduct in postProductDTOs)
            ////{
            ////    var existedProduct = await dbContext.products.FirstOrDefaultAsync(p => p.Name == postProduct.Name);
            ////    if (existedProduct != null)
            ////    {
            ////        unAllowrecords.Add(postProduct);
            ////    }
            ////    else
            ////    {
            ////        listInsert.Add(postProduct);
            ////    }
            ////}
            ////List<Product> records = new List<Product>();
            ////foreach (var product in listInsert)
            ////{
            ////    var record = new Product
            ////    {
            ////        Name = product.Name,
            ////        Price = product.Price,
            ////        Stock = product.Stock,
            ////    };
            ////    records.Add(record);
            ////}
            ////dbContext.products.AddRange(records);
            ////var result = await dbContext.SaveChangesAsync();
            ////if (result == 0)
            ////    throw new InternalServerException("internal server error");
            ////string message = "";
            ////if (unAllowrecords.Count == 0)
            ////    unAllowrecords = null;

            ////return new
            ////{
            ////    unAllowrecords = unAllowrecords,
            ////    Message = message = "These rrecord already"
            ////};
            #endregion
            //
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
