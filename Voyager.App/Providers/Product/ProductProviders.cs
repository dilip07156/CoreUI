using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;
using System.Threading.Tasks;

namespace Voyager.App.Providers
{
    public class ProductProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers
        
        public ProductProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion
        
        #region ProductSRP
        public async Task<ProductSRPSearchRes> GetProductSRPDetails(ProductSRPSearchReq productSRPSearchReq, string ticket)
        {
            ProductSRPSearchRes productSRPGetRes = new ProductSRPSearchRes();
            productSRPGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceProduct:GetProductSRPDetails"), productSRPSearchReq, typeof(ProductSRPSearchRes), ticket);
            return productSRPGetRes;
        } 

        public async Task<ProductSRPHotelGetRes> GetProductSRPHotelDetails(ProductSRPHotelGetReq productSRPSearchReq, string ticket)
        {
            ProductSRPHotelGetRes productSRPHotelGetRes = new ProductSRPHotelGetRes();
            productSRPHotelGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceProduct:GetProductSRPHotelDetails"), productSRPSearchReq, typeof(ProductSRPHotelGetRes), ticket);
            return productSRPHotelGetRes;
        }
        #endregion

        #region ProductPDP
        public async Task<ProductPDPSearchRes> GetProductPDPDetails(List<string> ProductIdList, string ticket)
        {
            ProductPDPSearchRes productPDPGetRes = new ProductPDPSearchRes();
            productPDPGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceProduct:GetProductPDPDetails"), ProductIdList, typeof(ProductPDPSearchRes), ticket);
            return productPDPGetRes;
        }
        #endregion

    }
}

