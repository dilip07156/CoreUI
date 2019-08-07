
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using VGER_WAPI_CLASSES;
using Voyager.App.Providers;
using Voyager.App.ViewModels;
using System.Linq;
using Voyager.App.Library;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.Net.Http;
using VGER_WAPI_CLASSES.BRIDGE.Company;

namespace Voyager.Controllers
{
	[Authorize]
	public class SupplierController : VoyagerController
	{
		#region Declaration
		private readonly IConfiguration _configuration;
		private IHostingEnvironment _env;
		MasterProviders masterProviders;
		AgentProviders agentProviders;
		SupplierProviders supplierProviders;
		AgentGetReq request;
		AgentGetRes response;
        TaxRegestrationDetails_Req requestForTaxReg;
        TaxRegestrationDetails_Res responseForTaxReg;
        SalesQuoteLibrary TaxTypeLibrary;
        #endregion

        public SupplierController(IHostingEnvironment env, IConfiguration configuration) : base(configuration)
		{
			_env = env;
			_configuration = configuration;
			masterProviders = new MasterProviders(_configuration);
			agentProviders = new AgentProviders(_configuration);
			supplierProviders = new SupplierProviders(_configuration);
            TaxTypeLibrary = new SalesQuoteLibrary(_configuration);
        }

		#region Supplier Search
		public IActionResult Supplier()
		{
			AgentSearchViewModel model = new AgentSearchViewModel();

			#region Dropdown Binding          
			AgentGetReq request = new AgentGetReq();
			var countrylist = masterProviders.GetAllCountries(token).Result;
			model.CountryList = countrylist;
			ProdTypeGetReq req = new ProdTypeGetReq();
			ProdTypeGetRes res = new ProdTypeGetRes();
			res = masterProviders.GetProdTypeByProdType(req, token).Result;
			model.ProductTypeList = res.ProductTypeList.OrderBy(x => x.Prodtype).ToList();
			#endregion

			return View(model);
		}

		public IActionResult _SupplierPipeline()
		{
			var model = new AgentPipelineViewModel();
			model.IsSupplier = true;
			return PartialView("_AgentPipeline", model);
		}

		public IActionResult PopulateCitiesByCountryId(string CountryId)
		{
			var list = masterProviders.GetAllCitiesByCountryId(CountryId, token).Result;
			return Json(new { response = list });
		}
		#endregion

		#region Supplier Details

		public IActionResult SupplierDetail(string CompanyId)
		{
			try
			{
				if (UserRoles.Contains("Administrator"))
				{
					ViewBag.IsAllowToEdit = true;
				}

				AgentSearchViewModel model = new AgentSearchViewModel();
				request = new AgentGetReq();
				response = new AgentGetRes();
				request.CompanyId = CompanyId;
				response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				if (response != null)
				{
					bool issupplier = false; if (response.AgentDetails.Issupplier == null || response.AgentDetails.Issupplier == false) { issupplier = false; } else { issupplier = true; }
					model.AgentDetails.CompanyId = response.AgentDetails.Company_Id;
					model.AgentDetails.CompanyName = response.AgentDetails.Name;
					model.AgentDetails.CompanyCode = response.AgentDetails.CompanyCode;
					model.AgentDetails.Street = response.AgentDetails.Street;
					model.AgentDetails.Street2 = response.AgentDetails.Street2;
					model.AgentDetails.Street3 = response.AgentDetails.Street3;
					model.AgentDetails.Zipcode = response.AgentDetails.Zipcode;
					model.AgentDetails.SalesOfficesList = response.AgentDetails.SalesOffices;
					model.AgentDetails.Issubagent = response.AgentDetails.Issubagent.HasValue ? response.AgentDetails.Issubagent.Value : false;
					model.AgentDetails.B2C = response.AgentDetails.B2C.HasValue ? response.AgentDetails.B2C.Value : false;
					model.AgentDetails.LogoFilePath = response.AgentDetails.LogoFilePath;
					model.AgentDetails.CountryId = response.AgentDetails.Country_Id;
					model.AgentDetails.CountryName = response.AgentDetails.CountryName;
					model.AgentDetails.CityId = response.AgentDetails.City_Id;
					model.AgentDetails.CityName = response.AgentDetails.CityName;
					model.AgentDetails.SalesOffices = response.AgentDetails.ParentAgent_Id;
					model.AgentDetails.SalesOfficesName = response.AgentDetails.ParentAgent_Name;
					model.AgentDetails.Issupplier = issupplier;
					model.AgentDetails.Status = (response.AgentDetails.STATUS == "" || response.AgentDetails.STATUS == " ") ? "Active" : "Inactive";
					if (response.AgentDetails.SalesOffices == null) model.AgentDetails.SalesOfficesList = new List<ChildrenCompanies>();

					#region Get country and city list from masters
					var countrylist = masterProviders.GetAllCountries(token).Result;
					var countryId = response.AgentDetails.Country_Id;
					model.AgentDetails.CountryList = countrylist;
					var citylist = masterProviders.GetAllCitiesByCountryId(countryId, token).Result;
					model.AgentDetails.CityList = citylist;
					#endregion
				}

				return View("AgentDetail", model);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult Products(string CompanyId)
		{
			try
			{
				SupplierViewModel model = new SupplierViewModel();

				//#region Get Company info
				//AgentGetReq request = new AgentGetReq() { CompanyId = CompanyId };
				//AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				//#endregion

				//if (response.AgentDetails.Products != null && response.AgentDetails.Products.Count > 0)
				//{
				//	#region Get Booking info
				//	List<BookingCount> list = new List<BookingCount>();

				//	foreach (var a in response.AgentDetails.Products)
				//	{
				//		BookingCount b = new BookingCount();
				//		b.ProductId = a.Product_Id;
				//		b.SupplierId = a.Supplier_Id;
				//		list.Add(b);
				//	}
				//	SupplierGetReq request1 = new SupplierGetReq() { CompanyId = CompanyId, bookingCount = list };
				//	SupplierGetRes response1 = supplierProviders.GetNoOfBookingsForSupplier(request1, token).Result;
				//	#endregion

				//	#region Data binding
				//	List<SupplierProducts> products = new List<SupplierProducts>();

				//	foreach (var p in response.AgentDetails.Products)
				//	{
				//		SupplierProducts prod = new SupplierProducts();
				//		prod.ProductId = p.Product_Id;
				//		prod.ProductType = p.ProductType;
				//		prod.ProductSupplierId = p.ProductSupplier_Id;
				//		prod.Code = p.ProductCode;
				//		prod.Name = p.ProductName;
				//		prod.Status = string.IsNullOrWhiteSpace(p.SupplierStatus) ? "Active" : "Inactive";
				//		prod.Type = p.ProductType;
				//		prod.Country = p.Country;
				//		prod.City = p.City;
				//		prod.Default = (p.IsDefault == null || p.IsDefault == false) ? "N" : "Y";
				//		prod.Preferred = (p.IsPreferred == null || p.IsPreferred == false) ? "N" : "Y";
				//		prod.Usage = response1.BookingCount.Where(x => x.ProductId == p.Product_Id).Select(x => x.TotalCount).FirstOrDefault(); //bookinglist.Where(x => x.Positions.Any(y => y.Product_Id == p.Product_Id && y.SupplierInfo.Id == p.Supplier_Id)).Count();
				//		products.Add(prod);
				//	}
				//	model.SupplierProducts = products.OrderBy(x => x.Name).ToList();
				//	#endregion
				//}

				model.CompanyId = CompanyId;
				return PartialView("_Products", model);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[HttpPost]
		public ActionResult LoadData(string CompanyId, int draw, int start, int length)
		{
			List<SupplierProducts> products = new List<SupplierProducts>();

			#region Get Company info			
			AgentGetReq request = new AgentGetReq() { CompanyId = CompanyId, Start = start, Length = (length == 0) ? 10 : length };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			#endregion

			if (response.AgentDetails.Products != null && response.AgentDetails.Products.Count > 0)
			{
				#region Get Booking info
				List<BookingCount> list = new List<BookingCount>();			

				foreach (var a in response.AgentDetails.Products)
				{
					BookingCount b = new BookingCount();
					b.ProductId = a.Product_Id;
					b.SupplierId = a.Supplier_Id;
					list.Add(b);
				}
				SupplierGetReq request1 = new SupplierGetReq() { CompanyId = CompanyId, bookingCount = list };
				SupplierGetRes response1 = supplierProviders.GetNoOfBookingsForSupplier(request1, token).Result;
				#endregion

				#region Data binding				
				foreach (var p in response.AgentDetails.Products)
				{
					SupplierProducts prod = new SupplierProducts();
					prod.ProductId = p.Product_Id;
					prod.ProductType = p.ProductType;
					prod.ProductSupplierId = p.ProductSupplier_Id;
					prod.Code = p.ProductCode;
					prod.Name = p.ProductName;
					prod.Status = string.IsNullOrWhiteSpace(p.SupplierStatus) ? "Active" : "Inactive";
					prod.Type = p.ProductType;
					prod.Country = p.Country;
					prod.City = p.City;
					prod.Default = (p.IsDefault == null || p.IsDefault == false) ? "N" : "Y";
					prod.Preferred = (p.IsPreferred == null || p.IsPreferred == false) ? "N" : "Y";
					prod.Usage = response1.BookingCount.Where(x => x.ProductId == p.Product_Id).Select(x => x.TotalCount).FirstOrDefault(); //bookinglist.Where(x => x.Positions.Any(y => y.Product_Id == p.Product_Id && y.SupplierInfo.Id == p.Supplier_Id)).Count();
					products.Add(prod);
				}
				//products = products.OrderBy(x => x.Name).ToList();
				#endregion
			}

			return Json(new
			{
				draw = draw,
				recordsTotal = response.ProductsTotalCount,
				recordsFiltered = response.ProductsTotalCount,
				data = products
			});
		}

		public IActionResult ViewManageProduct(string CompanyId, string ProductSupplierId)
		{
			ProductViewModel model = new ProductViewModel();

			#region Get currency list
			SalesProviders objSalesProvider = new SalesProviders(_configuration);
			CurrencyResponse objCurrencyResponse = objSalesProvider.GetCurrencyList(token).Result;
			model.CurrencyList = objCurrencyResponse.CurrencyList.OrderBy(x => x.CurrencyCode).ToList();
			#endregion

			#region Get Company info
			AgentGetReq request = new AgentGetReq() { CompanyId = CompanyId };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			var product = response.AgentDetails.Products.Where(x => x.ProductSupplier_Id == ProductSupplierId).FirstOrDefault();
			var contactlist = response.AgentDetails.ContactDetails;
			model.lstSalesAgent = product.SalesAgent.OrderBy(x => x.Company_Name).ToList();
			#endregion

			#region Get Product Supplier Sales/Operating list
			var lstSalesMarket = supplierProviders.GetBusinessRegions(token).Result;
			model.lstSalesMarket = lstSalesMarket.Select(x => new ProductSupplierSalesMarket { BusinessRegion_Id = x.BusinessRegion_Id, BusinessRegion = x.BusinessRegion }).ToList();
			//var lstSalesMarket = supplierProviders.GetProductSupplierProductSalesMkt(token).Result;
			//model.lstSalesMarket = lstSalesMarket.Select(x => new ProductSupplierSalesMarket { ProductSupplierSalesMkt_Id = x.ProductSupplierSalesMkt_Id + "|" + x.BusinessRegion_Id, BusinessRegion = x.BusinessRegion }).ToList();
			model.SelectedSalesMarket = product.SalesMarket.Select(x => new ProductSupplierSalesMarket { BusinessRegion_Id = x.BusinessRegion_Id, BusinessRegion = x.BusinessRegion }).ToList();

			var lstOperMarket = supplierProviders.GetBusinessRegions(token).Result;
			model.lstOperatingMarket = lstOperMarket.Select(x => new ProductSupplierOperatingMarket { BusinessRegion_Id = x.BusinessRegion_Id, BusinessRegion = x.BusinessRegion }).ToList();
			//var lstOperatingMarket = supplierProviders.GetProductSupplierProductOperatingMkt(token).Result;
			//model.lstOperatingMarket = lstOperatingMarket.Select(x => new ProductSupplierOperatingMarket { ProductSupplierOperatingMkt_Id = x.ProductSupplierOperatingMkt_Id + "|" + x.BusinessRegion_Id, BusinessRegion = x.BusinessRegion }).ToList();
			model.SelectedOperatingMarket = product.OperatingMarket.Select(x => new ProductSupplierOperatingMarket { BusinessRegion_Id = x.BusinessRegion_Id, BusinessRegion = x.BusinessRegion }).ToList();
			#endregion

			#region Html Controls binding
			List<Contact> contacts = new List<Contact>();
			foreach (var a in contactlist)
			{
				Contact list = new Contact();
				list.Contact_Id = a.Contact_Id;
				list.Contact_Name = a.FIRSTNAME + " " + a.LastNAME;
				list.Contact_Mail = a.MAIL;
				contacts.Add(list);
			}
			model.lstSales = contacts;
			model.lstGroup = contacts;
			model.lstFit = contacts;
			model.lstEmergency = contacts;
			model.lstFinance = contacts;
			model.lstComplaint = contacts;

			model.CompanyId = CompanyId;
			model.ProductSupplierId = ProductSupplierId;
			model.Status = string.IsNullOrWhiteSpace(product.SupplierStatus) ? "Active" : "Inactive";
			model.ActiveFrom = product.ActiveFrom!=null ? product.ActiveFrom.Value.ToString("dd/MM/yyyy"): "";
			model.ActiveTo = product.ActiveTo != null ? product.ActiveTo.Value.ToString("dd/MM/yyyy") : "";
            model.IsPreferred = Convert.ToBoolean(product.IsPreferred);
			model.IsDefault = Convert.ToBoolean(product.IsDefault);
			model.Currency_Id = product.CurrencyId;
			model.Currency = product.CurrencyName;
			model.ContactSalesId = product.Contact_Sales_Id;
			model.ContactSalesName = product.Contact_Sales_Name;
			model.ContactFitId = product.Contact_FIT_Id;
			model.ContactFitName = product.Contact_FIT_Name;
			model.ContactGroupId = product.Contact_Group_Id;
			model.ContactGroupName = product.Contact_Group_Name;
			model.ContactFinanceId = product.Contact_Finance_Id;
			model.ContactFinanceName = product.Contact_Finance_Name;
			model.ContactEmergencyId = product.Contact_Emergency_Id;
			model.ContactEmergencyName = product.Contact_Emergency_Name;
			model.ContactComplaintId = product.Contact_Complaints_Id;
			model.ContactComplaintName = product.Contact_Complaints_Name;
			#endregion

			return PartialView("_ViewManageProduct", model);
		}

		public IActionResult GetProductSalesAgents(string CompanyId, string ProductSupplierId)
		{
			try
			{
				AgentDetails model = new AgentDetails();
				request = new AgentGetReq() { CompanyId = CompanyId };
				AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				model.CompanyId = CompanyId;
				model.Issupplier = response.AgentDetails.Issupplier ?? false;
				CompanyProducts product = response.AgentDetails.Products.Where(x => x.ProductSupplier_Id == ProductSupplierId).FirstOrDefault();

				List<Branches> lst = new List<Branches>();
				foreach (var s in product.SalesAgent)
				{
					Branches prod = new Branches();
					prod.ParentCompanyId = s.Company_Id;
					prod.BranchCompanyId = s.ProductSupplierSalesAgent_Id; // Array Id assigned
					prod.BranchCompanyName = s.Company_Name;
					prod.BranchCompanyCode = s.Company_Code;
					lst.Add(prod);
				}
				model.Branches = lst.OrderBy(x => x.BranchCompanyName).ToList();
				model.IsForSalesAgent = true;
				model.ProductSupplierId = ProductSupplierId;
				return PartialView("_AgentDetailsBranches", model);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult SaveProductSalesAgent(Branches model)
		{
			var status = "";
			request = new AgentGetReq() { CompanyId = model.ParentCompanyId };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			var msg = ""; var name = "";
			if (!string.IsNullOrEmpty(model.BranchCompanyName))
			{
				name = model.BranchCompanyName;
				if (response.AgentDetails.Products != null && (model.BranchCompanyId != response.AgentDetails.Company_Id))
				{
					var product = response.AgentDetails.Products.Where(x => x.ProductSupplier_Id == model.ProductSupplierId).FirstOrDefault();
					var IsDuplicate = product.SalesAgent.Where(x => x.Company_Name == name).Select(x => x.Company_Name).FirstOrDefault();
					if (!string.IsNullOrEmpty(IsDuplicate))
					{
						status = "error";
						msg = "Duplicate record. Cannot insert as " + IsDuplicate + " is already exists. Please select another agent.";
					}
					else
					{
						SupplierSetReq req = new SupplierSetReq();
						req.Company_Id = model.ParentCompanyId;
						req.ProductSupplier_Id = model.ProductSupplierId;
						req.EditUser = ckUserEmailId;
						req.IsAddSalesAgent = true;
						ProductSupplierSalesAgent newAgent = new ProductSupplierSalesAgent()
						{
							Company_Id = model.BranchCompanyId,
							Company_Code = model.BranchCompanyCode,
							Company_Name = model.BranchCompanyName
						};
						req.Product.SalesAgent.Add(newAgent);
						SupplierSetRes res = supplierProviders.SetSupplierProduct(req, token).Result;
						var salesAgentId = res.SalesAgentId;

						//Call Bridge service to save Contact data in SQL databse
						ResponseStatus result = new ResponseStatus();
						if (res.ResponseStatus != null)
						{
							if (res.ResponseStatus.Status.ToLower() == "success")
							{
								SetProductSalesAgent_RQ request = new SetProductSalesAgent_RQ();
								request.ProductSupplier_Id = model.ProductSupplierId;
								request.ProductSupplierSalesAgent_Id = salesAgentId;
								request.User = ckUserEmailId;
								result = supplierProviders.SetCompany_ProductSalesAgent(request, token).Result;

								status = result.Status;
								msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
							}
							else
							{
								status = "failure";
								msg = res.ResponseStatus.ErrorMessage;
							}
						}
					}
				}
				else
				{
					status = "error";
					msg = "Cannot insert existing company name";
				}
			}
			else
			{
				status = "error";
				msg = "Please Select Agent Name";
			}
			return Json(new { status = status, responseText = msg });
			//return View();
		}

		public IActionResult DeleteProductSalesAgent(string CompanyId, string ProductSupplier_Id, string ProductSupplierSalesAgent_Id)
		{
			var status = ""; var msg = ""; var contactId = "";
			SupplierSetReq req = new SupplierSetReq();
			req.Company_Id = CompanyId;
			req.ProductSupplierSalesAgent_Id = ProductSupplierSalesAgent_Id;
			req.ProductSupplier_Id = ProductSupplier_Id;
			req.IsRemoveSalesAgent = true;
			req.EditUser = ckUserEmailId;
			SupplierSetRes res = supplierProviders.SetSupplierProduct(req, token).Result;

			//Call Bridge service to delete sales agent data in SQL databse
			ResponseStatus result = new ResponseStatus();
			if (res.ResponseStatus != null)
			{
				if (res.ResponseStatus.Status.ToLower() == "success")
				{
					SetProductSalesAgent_RQ request = new SetProductSalesAgent_RQ();
					request.ProductSupplierSalesAgent_Id = res.SalesAgentId;
					request.User = ckUserEmailId;
					result = supplierProviders.DelCompany_ProductSalesAgent(request, token).Result;

					status = result.Status;
					msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
				}
				else
				{
					status = "failure";
					msg = res.ResponseStatus.ErrorMessage;
				}
			}
			return null;
		}

		public IActionResult GetProductSRPDetail(string CompanyId)
		{
			ProductsSRPViewModel model = new ProductsSRPViewModel();

			#region Get Company info
			AgentGetReq request = new AgentGetReq() { CompanyId = CompanyId };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			var product = response.AgentDetails.Products.Where(x => x.Supplier_Id == CompanyId).FirstOrDefault();
			#endregion

			if (product != null && !string.IsNullOrWhiteSpace(product.Product_Id))
			{
				#region Get ProductSRPDetails
				ProductProviders productProviders = new ProductProviders(_configuration);
				ProductSRPSearchReq objProdReq = new ProductSRPSearchReq() { ProdId = product.Product_Id };
				ProductSRPSearchRes objProdRes = productProviders.GetProductSRPDetails(objProdReq, token).Result;
				var productSRPDetails = objProdRes.ProductSearchDetails.Select(a => new ProductSRPDetails
				{
					Address = a.Address,
					BdgPriceCategory = a.BdgPriceCategory,
					Chain = a.Chain,
					CityName = a.CityName,
					CountryName = a.CountryName,
					DefaultSupplier = a.DefaultSupplier,
					HotelImageURL = a.HotelImageURL,
					HotelType = a.HotelType,
					Location = a.Location,
					PostCode = a.PostCode,
					ProdDesc = a.ProdDesc,
					ProdName = a.ProdName,
					ProductCode = a.ProductCode,
					ProductType = a.ProductType,
					StarRating = a.StarRating,
					Street = a.Street,
					VoyagerProduct_Id = a.VoyagerProduct_Id,
					ProductType_Id = a.ProductType_Id
				}).ToList();

				model.ProductSRPDetails = productSRPDetails;

				var userRoles = HttpContext.Request.Cookies["UserRoles"] ?? string.Join(",", UserRoles);
				if (userRoles != null && userRoles.Contains("Administrator"))
				{
					model.PageName = "PrdSupplierMapping";
				}
			}
			return PartialView("~/Areas/Product/Views/ProductSRP/_ProductInfo.cshtml", model);
			#endregion
		}

		public IActionResult SaveManageProduct(ProductViewModel model)
		{
			try
			{
				var status = ""; var msg = "";
				SupplierSetReq req = new SupplierSetReq();
				req.Company_Id = model.CompanyId;
				req.ProductSupplier_Id = model.ProductSupplierId;
				req.EditUser = ckUserEmailId;
				req.IsProduct = true;
				req.Product.SupplierStatus = model.Status == "Active" ? string.Empty : "X";
				req.Product.CurrencyId = model.Currency_Id;
				req.Product.IsPreferred = model.IsPreferred;
				req.Product.IsDefault = model.IsDefault;
                if (string.IsNullOrEmpty(model.ActiveFrom))
                    req.Product.ActiveFrom = null;
                else
                {
                    var strFromDT = model.ActiveFrom.Split("/");
                    if (strFromDT?.Count() >= 3)
                    {
                        DateTime fromDT = new DateTime(Convert.ToInt32(strFromDT[2]), Convert.ToInt32(strFromDT[1]), Convert.ToInt32(strFromDT[0]));
                        req.Product.ActiveFrom = fromDT;
                    }
                    else
                    {
                        req.Product.ActiveFrom = null;
                    }
                }
                if (string.IsNullOrEmpty(model.ActiveTo))
                    req.Product.ActiveTo = null;
                else
                {
                    var strToDT = model.ActiveTo.Split("/");
                    if (strToDT?.Count() >= 3)
                    {
                        DateTime toDT = new DateTime(Convert.ToInt32(strToDT[2]), Convert.ToInt32(strToDT[1]), Convert.ToInt32(strToDT[0]));
                        req.Product.ActiveTo = toDT;
                    }
                    else
                    {
                        req.Product.ActiveTo = null;
                    }
                } 

				req.Product.Contact_Sales_Id = model.ContactSalesId;
				req.Product.Contact_Sales_Name = model.ContactSalesName;

				req.Product.Contact_FIT_Id = model.ContactFitId;
				req.Product.Contact_FIT_Name = model.ContactFitName;

				req.Product.Contact_Group_Id = model.ContactGroupId;
				req.Product.Contact_Group_Name = model.ContactGroupName;

				req.Product.Contact_Emergency_Id = model.ContactEmergencyId;
				req.Product.Contact_Emergency_Name = model.ContactEmergencyName;

				req.Product.Contact_Finance_Id = model.ContactFinanceId;
				req.Product.Contact_Finance_Name = model.ContactFinanceName;

				req.Product.Contact_Complaints_Id = model.ContactComplaintId;
				req.Product.Contact_Complaints_Name = model.ContactComplaintName;

				req.Product.OperatingMarket = model.selectedlstOperatingMarket.Select(x => new ProductSupplierOperatingMarket { ProductSupplierOperatingMkt_Id = x.ProductSupplierOperatingMkt_Id, BusinessRegion_Id = x.BusinessRegion_Id, BusinessRegion = x.BusinessRegion }).ToList();
				req.Product.SalesMarket = model.selectedlstSalesMarket.Select(x => new ProductSupplierSalesMarket { ProductSupplierSalesMkt_Id = x.ProductSupplierSalesMkt_Id, BusinessRegion_Id = x.BusinessRegion_Id, BusinessRegion = x.BusinessRegion }).ToList();

				SupplierSetRes res = supplierProviders.SetSupplierProduct(req, token).Result;

				//Call Bridge service to save Product data in SQL databse
				ResponseStatus result = new ResponseStatus();
				if (res.ResponseStatus != null)
				{
					if (res.ResponseStatus.Status.ToLower() == "success")
					{
						SetProduct_RQ request = new SetProduct_RQ();
						request.ProductSupplier_Id = model.ProductSupplierId;
						request.User = ckUserEmailId;
						result = supplierProviders.SetCompany_Product(request, token).Result;
						var salesAgentId = res.SalesAgentId;

						status = result.Status;
						msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;

						//Call Bridge service to save Sales Market data in SQL databse
						ResponseStatus result1 = new ResponseStatus();
						if (res.ResponseStatus != null)
						{
							if (res.ResponseStatus.Status.ToLower() == "success")
							{
								SetProductSalesMarket_RQ request1 = new SetProductSalesMarket_RQ();
								request1.ProductSupplier_Id = model.ProductSupplierId;
								request1.User = ckUserEmailId;
								result1 = supplierProviders.SetCompany_ProductSalesMarket(request1, token).Result;

								status = result.Status;
								msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
							}
							else
							{
								status = "failure";
								msg = res.ResponseStatus.ErrorMessage;
							}
						}

						//Call Bridge service to save Operating Market data in SQL databse
						ResponseStatus res1 = new ResponseStatus();
						if (res.ResponseStatus != null)
						{
							if (res.ResponseStatus.Status.ToLower() == "success")
							{
								SetProductOperatingMarket_RQ req1 = new SetProductOperatingMarket_RQ();
								req1.ProductSupplier_Id = model.ProductSupplierId;
								req1.User = ckUserEmailId;
								res1 = supplierProviders.SetCompany_ProductOperatingMarket(req1, token).Result;

								status = result.Status;
								msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
							}
							else
							{
								status = "failure";
								msg = res.ResponseStatus.ErrorMessage;
							}
						}


						if (model.IsDefault == true)
						{
							//Call Bridge service to save Sales Agent data in SQL databse
							ResponseStatus res2 = new ResponseStatus();
							if (res.ResponseStatus != null)
							{
								if (res.ResponseStatus.Status.ToLower() == "success")
								{
									SetProductSalesAgent_RQ req2 = new SetProductSalesAgent_RQ();
									req2.ProductSupplier_Id = model.ProductSupplierId;
									req2.User = ckUserEmailId;
									res2 = supplierProviders.DelCompany_ProductSalesAgent(req2, token).Result;

									status = result.Status;
									msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
								}
								else
								{
									status = "failure";
									msg = res.ResponseStatus.ErrorMessage;
								}
							}
						}
					}
					else
					{
						status = "failure";
						msg = res.ResponseStatus.ErrorMessage;
					}
				}
				return Json(new { status = status, responseText = msg });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult EnableDisableProduct(string CompanyId, string ProductSupplierId, string Status)
		{
			try
			{
				var status = ""; var msg = "";
				#region Set Supplier Product status
				if (Status == "Active") Status = "X"; else Status = " ";
				SupplierGetReq req = new SupplierGetReq() { CompanyId = CompanyId, ProductSupplierId = ProductSupplierId, Status = Status, EditUser = ckUserEmailId };
				SupplierGetRes res = supplierProviders.EnableDisableSupplierProduct(req, token).Result;

				//Call Bridge service to save Contact data in SQL databse
				ResponseStatus result = new ResponseStatus();
				if (res.ResponseStatus != null)
				{
					if (res.ResponseStatus.Status.ToLower() == "success")
					{
						SetProduct_RQ request = new SetProduct_RQ();
						request.ProductSupplier_Id = ProductSupplierId;
						request.User = ckUserEmailId;
						result = supplierProviders.SetCompany_Product(request, token).Result;

						status = result.Status;
						msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
					}
					else
					{
						status = "failure";
						msg = res.ResponseStatus.ErrorMessage;
					}
				}
				return Json(new { });
				#endregion
			}
			catch (Exception ex)
			{
				throw;
			}
		}
        public IActionResult ViewTaxRegDetails(string CompanyId, string TaxRegId)
        {
            TaxRegDetails model = new TaxRegDetails();
           
            requestForTaxReg = new TaxRegestrationDetails_Req();
            responseForTaxReg = new TaxRegestrationDetails_Res();
            requestForTaxReg.Company_Id = CompanyId;
            requestForTaxReg.TaxReg_Id = TaxRegId;

            responseForTaxReg = supplierProviders.GetTaxRegDetails(requestForTaxReg, token).Result;
            model.Company_Id = CompanyId;

            #region Get currency list
            QuoteSearchViewModel modelMasterForTaxRgStatus = new QuoteSearchViewModel();
            modelMasterForTaxRgStatus = TaxTypeLibrary.BindMasterData(_configuration, token, "TaxRgStatus");
            QuoteSearchViewModel modelMasterForTaxRgType = new QuoteSearchViewModel();
            modelMasterForTaxRgType = TaxTypeLibrary.BindMasterData(_configuration, token, "TaxRgType");
            model.TaxRegStatus = modelMasterForTaxRgStatus.TaxRgStatusList;
            model.TaxRegType = modelMasterForTaxRgType.TaxRgTypeList;
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            var objAgentResponse = agentProviders.GetSystemCompanyDetails(ckUserContactId, token).Result;
            AgentGetReq request = new AgentGetReq();
            request.CompanyId = objAgentResponse.SystemCompanyId;
            request.UserId = ckLoginUser_Id;
            AgentGetRes responseForCompany = new AgentGetRes();
            responseForCompany = agentProviders.GetAgentDetails(request, token).Result;
            List<Attributes> stateList  = masterProviders.GetAllStatesByCountryId(responseForCompany.AgentList.FirstOrDefault()?.CountryId, token).Result;
            model.States = stateList;

            #endregion

            if (responseForTaxReg != null && !string.IsNullOrEmpty(TaxRegId))
            {
                var TaxRegDetails = responseForTaxReg.TaxRegestrationDetails.Where(x => x.TaxReg_Id == TaxRegId).FirstOrDefault();
                model.TaxReg_Id = TaxRegDetails.TaxReg_Id;
                model.State = TaxRegDetails.State;
                model.State_Id = TaxRegDetails.State_Id;
                model.Number = TaxRegDetails.Number;
                model.Status = TaxRegDetails.Status;
                model.Type = TaxRegDetails.Type;
                model.TaxStatus = TaxRegDetails.TaxStatus;
                model.Type_Id = TaxRegDetails.Type_Id;
                model.TaxStatus_Id = TaxRegDetails.Taxstatus_Id;
                model.Company_Id = TaxRegDetails.Company_id;
            }
            return PartialView("ViewTaxRegDetails", model);


        }
        public IActionResult SaveTaxRegDetails(TaxRegDetails model)
        {
           
            request = new AgentGetReq() { CompanyId = model.Company_Id };
            AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
            var msg = ""; var taxRegID = ""; var status = "";
            AgentSetReq req = new AgentSetReq();
            var details = response.AgentDetails.TaxRegestrationDetails.Where(x => x.TaxReg_Id == model.TaxReg_Id).FirstOrDefault();
            var existsrecord = response.AgentDetails.TaxRegestrationDetails.Where(x => x.Type == model.Type && x.State == model.State && x.Number == model.Number && x.State_Id == model.State_Id && x.Type_Id == model.Type_Id).FirstOrDefault();

            if (details != null)
            {
                req.IsNewTaxRegistrationDetails = false;
                taxRegID = details.TaxReg_Id;
                req.companies.Company_Id = model.Company_Id;

                TaxRegestrationDetails b = new TaxRegestrationDetails();
                b.TaxReg_Id = model.TaxReg_Id;
                b.Company_id = model.Company_Id;
                b.Type = model.Type;
                b.Type_Id = model.Type_Id;
                b.State = model.State;
                b.State_Id = model.State_Id;
                b.Number = model.Number;
                b.Taxstatus_Id = model.TaxStatus_Id;
                b.TaxStatus = model.TaxStatus;
                b.Status = model.Status;
                b.CreateUser = details.CreateUser;
                b.CreateDate = details.CreateDate;
                b.EditUser = ckUserEmailId;
                req.companies.TaxRegestrationDetails.Add(b);
                AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
                if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; taxRegID = res.TaxRegId; }
                else if (res.ResponseStatus.Status.ToLower() == "failure") { status = "error1"; }
                else status = "error";
            }
            else
            {
                req.IsNewTaxRegistrationDetails = true;

                if (existsrecord != null)
                {
                    status = "error1";
                    msg = "Duplicate record";
                }
                else
                {
                    req.companies.Company_Id = model.Company_Id;
                    TaxRegestrationDetails b = new TaxRegestrationDetails();

                    b.TaxReg_Id = model.TaxReg_Id;
                    b.Company_id = model.Company_Id;
                    b.Type = model.Type;
                    b.Type_Id = model.Type_Id;
                    b.State = model.State;
                    b.State_Id = model.State_Id;
                    b.Number = model.Number;
                    b.Taxstatus_Id = model.TaxStatus_Id;
                    b.TaxStatus = model.TaxStatus;
                    b.Status = model.Status ;
                    b.CreateUser = ckUserEmailId;
                   


                    req.companies.TaxRegestrationDetails.Add(b);

                    AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
                    model.TaxReg_Id = res.TaxRegId;
                    if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; taxRegID = res.TaxRegId; }
                    else status = "error";
                }
            }

            //Call Bridge service to save Contact data in SQL databse
            ResponseStatus result = new ResponseStatus();
            if (status == "success")
            {
                SetCompanyTaxReg_RQ request = new SetCompanyTaxReg_RQ();
                request.TaxRegId = model.TaxReg_Id;
                request.User = ckUserEmailId;
                result = agentProviders.SetCompanyTaxRegDetails(request, token).Result;

                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.Status) && result.Status == "Success")
                    {
                        status = "success";
                        msg = "Details Updated Successfully.";
                    }
                    else
                    {
                        status = "error";
                        msg = result.ErrorMessage + " " + result.StatusMessage;
                    }
                }
                else
                {
                    status = "error";
                    msg = "Error in updating records.";
                }
            }
            return Json(new { status = status, responseText = msg, TaxRegId = taxRegID });
        }
        #endregion
    }
}