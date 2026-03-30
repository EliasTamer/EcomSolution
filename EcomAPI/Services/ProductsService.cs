using EcomAPI.Interfaces;
using System.Data;

namespace EcomAPI.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IDbConnection _db;
        public ProductsService(IDbConnection db) {
            _db = db;
        }
    }
}
