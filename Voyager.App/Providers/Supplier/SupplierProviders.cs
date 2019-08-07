using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using Voyager.App.Proxy;

namespace Voyager.App.Providers
{
    public class SupplierProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers
        public SupplierProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }
        #endregion

        #region Supplier Details
        public async Task<SupplierGetRes> GetSupplierData(SupplierGetReq supplierGetReq, string ticket)
        {
            SupplierGetRes supplierGetRes = new SupplierGetRes();
            supplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:GetSupplierData"), supplierGetReq, typeof(SupplierGetRes), ticket);
            return supplierGetRes;
        }

        public async Task<SupplierGetRes> GetNoOfBookingsForSupplier(SupplierGetReq supplierGetReq, string ticket)
        {
            SupplierGetRes supplierGetRes = new SupplierGetRes();
            supplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:GetNoOfBookingsForSupplier"), supplierGetReq, typeof(SupplierGetRes), ticket);
            return supplierGetRes;
        }

        public async Task<SupplierGetRes> EnableDisableSupplierProduct(SupplierGetReq supplierGetReq, string ticket)
        {
            SupplierGetRes supplierGetRes = new SupplierGetRes();
            supplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:EnableDisableSupplierProduct"), supplierGetReq, typeof(SupplierGetRes), ticket);
            return supplierGetRes;
        }

        public async Task<List<ProductSupplierSalesMarket>> GetProductSupplierProductSalesMkt(string ticket)
        {
            List<ProductSupplierSalesMarket> supplierGetRes = new List<ProductSupplierSalesMarket>();
            supplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:GetProductSupplierSalesMkt"), null, typeof(List<ProductSupplierSalesMarket>), ticket);
            return supplierGetRes;
        }

        public async Task<List<ProductSupplierOperatingMarket>> GetProductSupplierProductOperatingMkt(string ticket)
        {
            List<ProductSupplierOperatingMarket> supplierGetRes = new List<ProductSupplierOperatingMarket>();
            supplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:GetProductSupplierProductOperatingMkt"), null, typeof(List<ProductSupplierOperatingMarket>), ticket);
            return supplierGetRes;
        }

        public async Task<List<mBusinessRegions>> GetBusinessRegions(string ticket)
        {
            List<mBusinessRegions> supplierGetRes = new List<mBusinessRegions>();
            supplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:GetBusinessRegions"), null, typeof(List<mBusinessRegions>), ticket);
            return supplierGetRes;
        }

        public async Task<SupplierSetRes> SetSupplierProduct(SupplierSetReq supplierSetReq, string ticket)
        {
            SupplierSetRes supplierSetRes = new SupplierSetRes();
            supplierSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:SetSupplierProduct"), supplierSetReq, typeof(SupplierSetRes), ticket);
            return supplierSetRes;
        }

        public async Task<ResponseStatus> SetCompany_Product(SetProduct_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompany_Product"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompany_ProductSalesMarket(SetProductSalesMarket_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompany_ProductSalesMarket"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompany_ProductOperatingMarket(SetProductOperatingMarket_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompany_ProductOperatingMarket"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompany_ProductSalesAgent(SetProductSalesAgent_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompany_ProductSalesAgent"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> DelCompany_ProductSalesAgent(SetProductSalesAgent_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:DelCompany_ProductSalesAgent"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }
        public async Task<TaxRegestrationDetails_Res> GetTaxRegDetails(TaxRegestrationDetails_Req taxRegReq, string ticket)
        {
            TaxRegestrationDetails_Res taxRegSetRes = new TaxRegestrationDetails_Res();
            taxRegSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:GetTaxRegestrationDetails"), taxRegReq, typeof(TaxRegestrationDetails_Res), ticket);
            return taxRegSetRes;
        }
		#endregion

		#region Mapping

		public async Task<List<mApplications>> GetApplications(string ticket)
		{
			List<mApplications> supplierGetRes = new List<mApplications>();
			supplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:GetApplications"), null, typeof(List<mApplications>), ticket);
			return supplierGetRes;
		}

		public async Task<List<Mappings>> GetSupplierMappings(SupplierGetReq request, string ticket)
		{
			List<Mappings> supplierGetRes = new List<Mappings>();
			supplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:GetSupplierMappings"), request, typeof(List<Mappings>), ticket);
			return supplierGetRes;
		}

		public async Task<SupplierSetRes> SetSupplierMapping(SupplierSetReq request, string ticket)
		{
			SupplierSetRes supplierGetRes = new SupplierSetRes();
			supplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("SupplierService:SetSupplierMapping"), request, typeof(SupplierSetRes), ticket);
			return supplierGetRes;
		}
		#endregion

	}
}
