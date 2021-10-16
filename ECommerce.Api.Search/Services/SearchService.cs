using ECommerce.Api.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly ICustomerService customerService;

        public SearchService(IOrderService orderService, IProductService productService, ICustomerService customerService)
        {
            this.orderService = orderService;
            this.productService = productService;
            this.customerService = customerService;
        }

        public async Task<(bool IsSuccess, dynamic searchResults)> SearchAsync(int customerId)
        {
            var orderResult = await orderService.GetOrdersAsync(customerId);
            var customerResult = await customerService.GetCustomersAsync(customerId);
            var productResult = await productService.GetProductsAsync();
            if (orderResult.IsSuccess)
            {
                foreach (var order in orderResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = productResult.IsSuccess ?
                            productResult.Products.FirstOrDefault(p => p.Id == item.Id)?.Name : 
                            "Product information not available";                        
                    }
                }
                var result = new
                {
                    Customer = customerResult.IsSuccess ?
                                customerResult.Customers :
                                new { Name = "Customer information is not available" },
                    Orders = new { orderResult.Orders  }
                };               
                return (true, result);
            }
            return (false, null);
        }        
    }
}
