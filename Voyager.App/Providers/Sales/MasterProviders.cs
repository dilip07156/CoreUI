using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Voyager.App.Contracts;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;

namespace Voyager.App.Providers
{
    public class MasterProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        public MasterProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        public async Task<ProdCatDefGetRes> GetProdCatDefByName(ProdCatDefGetReq prodCatDefGetReq, string ticket)
        {
            ProdCatDefGetRes prodCatDefGetRes = new ProdCatDefGetRes();
            prodCatDefGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:ServiceGetProdCatDefByName"), prodCatDefGetReq, typeof(ProdCatDefGetRes), ticket);
            return prodCatDefGetRes;
        }

        public async Task<ProdCatDefGetRes> GetProdCatDef(string ticket)
        {
            ProdCatDefGetRes prodCatDefGetRes = new ProdCatDefGetRes();
            prodCatDefGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:ServiceGetProdCatDef"), null, typeof(ProdCatDefGetRes), ticket);
            return prodCatDefGetRes;
        }

        public async Task<ProductAttributeRes> GetProdAttributeDetailsByNameOrVal(ProductAttributeReq productAttributeReq, string ticket)
        {
            ProductAttributeRes productAttributeRes = new ProductAttributeRes();
            productAttributeRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:ServiceGetProdAttributeDetailsByNameOrVal"), productAttributeReq, typeof(ProductAttributeRes), ticket);
            return productAttributeRes;
        }

        public async Task<ProductSearchRes> GetProductDetailsBySearchCriteria(ProductSearchReq productSearchReq, string ticket)
        {
            ProductSearchRes productSearchRes = new ProductSearchRes();
            productSearchRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:ServiceGetProductDetailsBySearchCriteria"), productSearchReq, typeof(ProductSearchRes), ticket);
            return productSearchRes;
        }

        public async Task<ProductWithRateSearchRes> GetProductWithRateBySearchCriteria(ProductWithRateSearchReq productSearchReq, string ticket)
        {
            ProductWithRateSearchRes productSearchRes = new ProductWithRateSearchRes();
            productSearchRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:ServiceGetProductWithRateBySearchCriteria"), productSearchReq, typeof(ProductWithRateSearchRes), ticket);
            return productSearchRes;
        }

        public async Task<ProdCategoryRangeGetRes> GetProductCategoryRangeByProductID(ProdCategoryRangeGetReq prodCategoryRangeGetReq, string ticket)
        {
            ProdCategoryRangeGetRes prodCategoryRangeGetRes = new ProdCategoryRangeGetRes();
            prodCategoryRangeGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:ServiceGetProductCategoryRangeByProductID"), prodCategoryRangeGetReq, typeof(ProdCategoryRangeGetRes), ticket);
            return prodCategoryRangeGetRes;
        }

        public async Task<ProductCatGetRes> GetProductCategoryByParam(ProductCatGetReq productCatGetReq, string ticket)
        {
            ProductCatGetRes productCatGetRes = new ProductCatGetRes();
            productCatGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetProductCategoryByParam"), productCatGetReq, typeof(ProductCatGetRes), ticket);
            return productCatGetRes;
        }

        public async Task<ProductRangeGetRes> GetProductRangeByParam(ProductRangeGetReq productRangeGetReq, string ticket)
        {
            ProductRangeGetRes productRangeGetRes = new ProductRangeGetRes();
            productRangeGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetProductRangeByParam"), productRangeGetReq, typeof(ProductRangeGetRes), ticket);
            return productRangeGetRes;
        }

        public async Task<ProdTypeGetRes> GetProdTypeByProdType(ProdTypeGetReq prodTypeGetReq, string ticket)
        {
            ProdTypeGetRes prodTypeGetRes = new ProdTypeGetRes();
            prodTypeGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetProductTypeByProdType"), prodTypeGetReq, typeof(ProdTypeGetRes), ticket);
            return prodTypeGetRes;
        }

        public async Task<DefMealPlanGetRes> GetDefMealPlan(DefMealPlanGetReq defMealPlanGetReq, string ticket)
        {
            DefMealPlanGetRes defMealPlanGetRes = new DefMealPlanGetRes();
            defMealPlanGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetDefMealPlan"), defMealPlanGetReq, typeof(DefMealPlanGetRes), ticket);
            return defMealPlanGetRes;
        }

        public async Task<ProductSupplierGetRes> GetProductSupplierList(ProductSupplierGetReq productSupplierGetReq, string ticket)
        {
            ProductSupplierGetRes productSupplierGetRes = new ProductSupplierGetRes();
            productSupplierGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetProductSupplierList"), productSupplierGetReq, typeof(ProductSupplierGetRes), ticket);
            return productSupplierGetRes;
        }

        public async Task<List<VGER_WAPI_CLASSES.Attributes>> GetAllCountries(string ticket)
        {
            List<VGER_WAPI_CLASSES.Attributes> countrylist = new List<VGER_WAPI_CLASSES.Attributes>();
            countrylist = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetAllCountryList"), null, typeof(List<VGER_WAPI_CLASSES.Attributes>), ticket);
            return countrylist;
        }

        public async Task<List<VGER_WAPI_CLASSES.Attributes>> GetAllCitiesByCountryId(string CountryId, string ticket)
        {
            List<VGER_WAPI_CLASSES.Attributes> citylist = new List<VGER_WAPI_CLASSES.Attributes>();
            citylist = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetAllCitiesByCountryId"), CountryId, typeof(List<VGER_WAPI_CLASSES.Attributes>), ticket);
            return citylist;
        }
        public async Task<List<VGER_WAPI_CLASSES.Attributes>> GetAllStatesByCountryId(string CountryId, string ticket)
        {
            List<VGER_WAPI_CLASSES.Attributes> citylist = new List<VGER_WAPI_CLASSES.Attributes>();
            citylist = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetAllStatesByCountryId"), CountryId, typeof(List<VGER_WAPI_CLASSES.Attributes>), ticket);
            return citylist;
        }

        public async Task<List<VGER_WAPI_CLASSES.Attributes>> GetMarkups(string ticket)
        {
            List<VGER_WAPI_CLASSES.Attributes> markuplist = new List<VGER_WAPI_CLASSES.Attributes>();
            markuplist = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetMarkups"), null, typeof(List<VGER_WAPI_CLASSES.Attributes>), ticket);
            return markuplist;
        }
        
        public async Task<SimilarHotelsGetRes> GetSimilarHotels(SimilarHotelsGetReq similarHotelsGetReq, string ticket)
        {
            SimilarHotelsGetRes similarHotelsGetRes = new SimilarHotelsGetRes();
            similarHotelsGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetSimilarHotels"), similarHotelsGetReq, typeof(SimilarHotelsGetRes), ticket);
            return similarHotelsGetRes;
        }

        public async Task<SimilarHotelsSetRes> SetSimilarHotels(SimilarHotelsSetReq similarHotelsSetReq, string ticket)
        {
            SimilarHotelsSetRes similarHotelsSetRes = new SimilarHotelsSetRes();
            similarHotelsSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:SetSimilarHotels"), similarHotelsSetReq, typeof(SimilarHotelsSetRes), ticket);
            return similarHotelsSetRes;
        }

        public async Task<DefProductTypeRes> GetAllProductTypes(string ticket)
        {
            DefProductTypeRes prodtypelist = new DefProductTypeRes();
            prodtypelist = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetProductTypes"), null, typeof(DefProductTypeRes), ticket);
            return prodtypelist;
        }

		public async Task<ProductRangeGetRes> GetProdRangeNameByProdId(ProductRangeGetReq request,string ticket)
		{
			ProductRangeGetRes response = new ProductRangeGetRes();
			response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetProdRangeNameByProdId"), request, typeof(ProductRangeGetRes), ticket);
			return response;
		}

        public async Task<DefChargeBasisRes> GetDefChargeBasis(string ticket)
        {
            DefChargeBasisRes defChargeBasisRes = new DefChargeBasisRes();
            defChargeBasisRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetDefChargeBasis"), null, typeof(DefChargeBasisRes), ticket);
            return defChargeBasisRes;
        }

        public async Task<DefPersonTypeRes> GetPersonType(string ticket)
        {
            DefPersonTypeRes defPersonTypeRes = new DefPersonTypeRes();
            defPersonTypeRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetPersonType"), null, typeof(DefPersonTypeRes), ticket);
            return defPersonTypeRes;
        }

        public async Task<DefMealTypeRes> GetMealType(DefMealTypeGetReq request, string ticket)
        {
            DefMealTypeRes DefMealTypeRes = new DefMealTypeRes();
            DefMealTypeRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetMealType"), request, typeof(DefMealTypeRes), ticket);
            return DefMealTypeRes;
        }

        public async Task<ProductTemplatesGetRes> GetProductTemplates(ProductTemplatesGetReq request, string ticket)
        {
            ProductTemplatesGetRes ProductTemplatesGetRes = new ProductTemplatesGetRes();
            ProductTemplatesGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetProductTemplates"), request, typeof(ProductTemplatesGetRes), ticket);
            return ProductTemplatesGetRes;
        }

        public async Task<ProdContractGetRes> GetContractRatesByProductID(ProdContractGetReq prodContractGetReq, string ticket)
        {
            ProdContractGetRes prodContractGetRes = new ProdContractGetRes();
            prodContractGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetContractRatesByProductID"), prodContractGetReq, typeof(ProdContractGetRes), ticket);
            return prodContractGetRes;
        }
    }
}
