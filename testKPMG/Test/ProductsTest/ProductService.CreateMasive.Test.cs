using FluentAssertions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using testKPMG.AuxTools.models;
using testKPMG.DTOs.Products;
using testKPMG.Interfaces;

namespace testKPMG.Test.ProductsTest
{
    [TestClass]
    public class ProductService
    {
        private readonly IProductService productService;

        public ProductService(IProductService productService)
        {
            this.productService = productService;
        }

        // This test class should create a list of products, and in cause of service evaluate it should return a list of objets with the entrance input list json structure.
        [TestMethod]
        public async Task CreateMassiveProducts_Ideal_Test()
        {
            //Arrange: Here we define the params, variables, objects, etc, that will be use in our test case, and here should be define de expected resolve.
            StringBuilder sb = new StringBuilder();
            sb.Append("testP");
            var testList = new List<PostProductDTO>();
            for (int i = 0; i < 10; i++)
            {
                Random rg = new Random();
                var rn = rg.Next(1, 50).ToString();
                var item = new PostProductDTO
                {
                    Name = sb.Append(rn).ToString(),
                    Price = rg.Next(1, 10000),
                    Stock = rg.Next(1,100)
                };
                testList.Add(item);
            }

                var expectedResponseJson = new MassiveProductResponse
                {
                    unAllowrecords = null,
                    Message = "These records already existed"
                };
            //Act: Here will be ran the logic across the test.
            var response = await productService.CreateMassiveProducts(testList);
            var jsonResponse = JsonSerializer.Serialize(response);
            var actualResponse = JsonSerializer.Deserialize<MassiveProductResponse>(JsonSerializer.Serialize(response));
            //Assert: here is define the validation of the test, using the expected resolve to made the contrast.
            Assert.IsNull(actualResponse.unAllowrecords, "");
            Assert.AreEqual(expectedResponseJson.Message, actualResponse.Message, "");
            actualResponse.Should().BeEquivalentTo(expectedResponseJson);


        }
    }

    
}
