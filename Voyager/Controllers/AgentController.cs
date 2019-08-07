using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VGER_WAPI_CLASSES;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
	[Authorize]
	public class AgentController : VoyagerController
	{
		#region Declaration
		private readonly IConfiguration _configuration;
		private IHostingEnvironment _env;
		private AgentProviders agentProviders;
		private SupplierProviders supplierProviders;
		private MasterProviders masterProviders;
		private LoginProviders loginProviders;
		private AgentGetReq request;
		private AgentGetRes response;
		private SupplierGetReq request1;
		private SupplierGetRes response1;
		private TaxRegestrationDetails_Req requestForTaxReg;
		private TaxRegestrationDetails_Res responseForTaxReg;
        #endregion

        public AgentController(IHostingEnvironment env, IConfiguration configuration) : base(configuration)
		{
			_env = env;
			_configuration = configuration;
			agentProviders = new AgentProviders(_configuration);
			supplierProviders = new SupplierProviders(_configuration);
			masterProviders = new MasterProviders(_configuration);
			loginProviders = new LoginProviders(_configuration);
		}

		#region Agent Search
		public IActionResult Agent()
		{
			AgentSearchViewModel model = new AgentSearchViewModel();

			#region Dropdown Binding          
			AgentGetReq request = new AgentGetReq();
			var countrylist = masterProviders.GetAllCountries(token).Result;
			model.CountryList = countrylist;
			#endregion

			return View(model);
		}

		[HttpGet]
		public ActionResult LoadData(AgentSearchViewModel searchFilters, int draw, int start, int length)
		{
			if (searchFilters.IsSupplier == false)
			{
				// To get Agent details
				AgentGetReq request = new AgentGetReq();
				AgentGetRes response = new AgentGetRes();
				request.AgentName = searchFilters.AgentName;
				request.AgentReference = searchFilters.CNKReferenceNo;
				request.CountryId = searchFilters.Country == null ? Guid.Empty : searchFilters.Country;
				request.Status = searchFilters.Status;
				request.UserId = ckLoginUser_Id;
				request.Start = start;
				if (length == 0)
					length = 10;
				request.Length = length;
				response = agentProviders.GetAgentDetails(request, token).Result;

				return Json(new
				{
					draw = draw,
					recordsTotal = response.AgentTotalCount,
					recordsFiltered = response.AgentTotalCount,
					data = response.AgentList
				});

			}
			else if (searchFilters.IsSupplier == true)
			{
				// To get Supplier details
				SupplierGetReq request = new SupplierGetReq();
				SupplierGetRes response = new SupplierGetRes();
				List<SupplierList> lstSupplement = new List<SupplierList>();
				List<AgentList> lstAgent = new List<AgentList>();
				request.SupplierName = searchFilters.AgentName;
				request.SupplierCode = searchFilters.CNKReferenceNo;
				request.CountryId = searchFilters.Country == null ? Guid.Empty : searchFilters.Country;
				request.CityId = searchFilters.City == null ? Guid.Empty : searchFilters.City;
				request.ProductType = searchFilters.ProductType;
				request.Status = searchFilters.Status;
				request.UserId = ckLoginUser_Id;
				request.Start = start;
				if (length == 0)
					length = 10;
				request.Length = length;
				response = supplierProviders.GetSupplierData(request, token).Result;

				return Json(new
				{
					draw = draw,
					recordsTotal = response.SupplierTotalCount,
					recordsFiltered = response.SupplierTotalCount,
					data = response.SupplierList
				});
			}
			else
				return null;
		}

		public IActionResult _AgentPipeline()
		{
			var model = new AgentPipelineViewModel();
			model.IsSupplier = false;
			return PartialView("_AgentPipeline", model);
		}

		public IActionResult CreateNewAgent(bool IsSupplier = false)
		{
			NewAgent model = new NewAgent();
			#region Dropdown binding
			var countrylist = masterProviders.GetAllCountries(token).Result;
			model.CountryList = countrylist;

			var contactbylist = agentProviders.GetStartPageForAgents(token).Result;
			model.StartPageList = contactbylist.OrderBy(x => x.Name).ToList();
			#endregion

			model.IsSupplier = IsSupplier;
			return PartialView("_CreateNewAgent", model);
		}

		public IActionResult SaveNewAgent(NewAgent model)
		{
			try
			{
				model.AgentName = HttpUtility.HtmlDecode(model.AgentName);
				string status = "", msg = "", contactId = "", companyId = "", companyCode = "", type = ""; bool IsDuplicate = false;

				if (model.CompanyId == null)
				{
					IsDuplicate = CheckDuplicate(model.AgentName, model.CountryId, model.CityId, model.IsSupplier, out companyId);
					if (IsDuplicate)
					{
						if (model.IsSupplier == true) type = "Supplier"; else type = "Agent";
						status = "error";
						msg = type + " already exists";
					}
					else
					{
						if (model.CompanyCode == null)
						{
							//Call Bridge service to get company code
							GetCompany_RS result = new GetCompany_RS();
							GetCompany_RQ request = new GetCompany_RQ();
							if (model.IsSupplier == true) type = "Supplier"; else type = "Agent";
							request.Type = type;
							result = agentProviders.GetLatestSQLReferenceNumber(request, token).Result;
							status = result.ResponseStatus.Status;
							if (status.ToLower() == "success")
							{
								//msg = result.ResponseStatus.StatusMessage;
								model.CompanyCode = Convert.ToString(result.ReferenceNumber);
							}
							else
							{
								status = "error";
								msg = "Error in generating new company code.";
							}
						}
					}
				}

				if (IsDuplicate == false)
				{
					AgentSetReq req = new AgentSetReq();
					req.IsCompany = true;
					req.companies.Issupplier = model.IsSupplier;
					req.companies.Company_Id = model.CompanyId;
					req.companies.Name = model.AgentName;
					req.companies.CompanyCode = model.CompanyCode;
					req.companies.Street = model.Street;
					req.companies.Street2 = model.Street2;
					req.companies.Zipcode = model.Zipcode;
					req.companies.Country_Id = model.CountryId;
					req.companies.CountryName = model.CountryName;
					req.companies.City_Id = model.CityId;
					req.companies.CityName = model.CityName;
					req.companies.ContactBy = model.ContactBy;
					req.companies.CreateUser = ckUserEmailId;
					req.companies.EditUser = ckUserEmailId;
					req.EditUser = ckUserEmailId;
					req.LoggedInUserContactId = ckUserContactId;
					req.companies.ParentAgent_Id = ckUserCompanyId;
					req.companies.ParentAgent_Name = ckUserCompanyName;

					CompanyContacts contact = new CompanyContacts()
					{
						Company_Id = model.CompanyId,
						Contact_Id = model.ContactId,
						CommonTitle = model.CommonTitle,
						FIRSTNAME = model.FIRSTNAME,
						LastNAME = model.LastNAME,
						TEL = model.TEL,
						MAIL = model.MAIL,
						CreateUser = ckUserEmailId,
						EditUser = ckUserEmailId
					};
					req.companies.ContactDetails.Add(contact);
					AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
					contactId = res.ContactId;
					companyId = res.CompanyId;
					companyCode = res.CompanyCode;

					if (res.ResponseStatus.Status.ToLower() == "success")
					{
						//Call Bridge service to set Agent in Company table in Sql
						ResponseStatus result = new ResponseStatus();
						SetCompany_RQ request = new SetCompany_RQ();
						request.Company_Id = companyId;
						request.User = ckUserEmailId;
						result = agentProviders.SetCompany(request, token).Result;
						if (result.Status.ToLower() == "success")
						{
							//Call Bridge service to add default contact in Contact table in Sql
							ResponseStatus result1 = new ResponseStatus();
							SetCompanyContact_RQ request1 = new SetCompanyContact_RQ();
							request1.Contact_Id = contactId;
							request1.User = ckUserEmailId;
							result1 = agentProviders.SetCompanyContact(request1, token).Result;
							if (result1.Status.ToLower() == "success")
							{
								status = "success";
								msg = "Record Saved Successfully." + result.StatusMessage;
							}
							else
							{
								status = "error";
								msg = res.ResponseStatus.ErrorMessage + result1.StatusMessage;
							}
						}
						else
						{
							status = "error";
							msg = res.ResponseStatus.ErrorMessage + result.StatusMessage;
						}
					}
					else
					{
						status = "error";
						msg = msg + res.ResponseStatus.ErrorMessage;
					}
				}

				return Json(new { status = status, responseText = msg, contactId = contactId, companyId = companyId, companyCode = companyCode, type = type });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Json(new { status = "error", responseText = "Error in saving record" });
			}
		}

		public bool CheckDuplicate(string AgentName, string CountryId, string CityId, bool IsSupplier, out string CompanyId)
		{
			bool isDuplicate = false;
			request = new AgentGetReq();
			response = new AgentGetRes();
			request1 = new SupplierGetReq();
			response1 = new SupplierGetRes();
			if (IsSupplier == true)
			{
				request1.SupplierName = AgentName;
				request1.CountryId = string.IsNullOrWhiteSpace(CountryId) ? Guid.Parse(CountryId) : Guid.Empty;
				request1.CityId = string.IsNullOrWhiteSpace(CityId) ? Guid.Parse(CityId) : Guid.Empty;
				response1 = supplierProviders.GetSupplierData(request1, token).Result;
				var existingSupplier = response1.SupplierList.Where(x => x.Name == AgentName && x.CountryId == CountryId && x.CityId == CityId).ToList();
				CompanyId = existingSupplier.Select(a => a.CompanyId).FirstOrDefault();
				if (existingSupplier.Count > 0)
					isDuplicate = true;
				else
					isDuplicate = false;
			}
			else
			{
				response = agentProviders.GetAgentDetails(request, token).Result;
				var existingAgent = response.AgentList.Where(x => x.Name == AgentName && x.CountryId == CountryId && x.CityId == CityId).ToList();
				CompanyId = existingAgent.Select(a => a.CompanyId).FirstOrDefault();
				if (existingAgent.Count > 0)
					isDuplicate = true;
				else
					isDuplicate = false;
			}
			return isDuplicate;
		}

		#endregion

		#region Agent Details
		public IActionResult AgentDetail(string CompanyId, string ContactId)
		{
			AgentSearchViewModel model = new AgentSearchViewModel();
			try
			{
				if (UserRoles.Contains("Administrator"))
				{
					ViewBag.IsAllowToEdit = true;
				}

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
					string[] TargetTabRoles = new string[] { "Administrator", "Sales Manager" };
					var targetrole = UserRoles.Where(a => TargetTabRoles.Contains(a)).FirstOrDefault();
					if (targetrole != null && response.AgentDetails.HeadOffice_Id == ckUserCompanyId)
					{
						ViewBag.IsAllowToEditForTarget = true;
					}
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

				model.ContactId = ContactId;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return View("AgentDetail", model);
		}

		public IActionResult PopulateCitiesByCountryId(string CountryId)
		{
			var list = masterProviders.GetAllCitiesByCountryId(CountryId, token).Result;
			return Json(new { response = list });
		}

		public IActionResult PopulateContactsByCompanyId(string CompanyId, string BranchCompanyId)
		{
			if (!string.IsNullOrEmpty(CompanyId))
			{
				//list of selected dropdown contacts
				List<Attributes> newlist = new List<Attributes>();
				AgentGetReq request = new AgentGetReq();
				request.CompanyId = BranchCompanyId;
				var list = agentProviders.GetAgentDetailedInfo(request, token).Result;

				//list of current company contacts                
				AgentGetReq req = new AgentGetReq();
				req.CompanyId = CompanyId;
				var currentlist = agentProviders.GetAgentDetailedInfo(req, token).Result;
				var currentcontacts = currentlist.AgentDetails.ContactDetails.Select(x => x.FIRSTNAME + " " + x.LastNAME);
				list.AgentDetails.ContactDetails.RemoveAll(x => currentcontacts.Contains(x.FIRSTNAME + " " + x.LastNAME));

				foreach (var r in list.AgentDetails.ContactDetails)
				{
					Attributes a = new Attributes();
					a.Attribute_Id = r.Contact_Id;
					a.AttributeName = r.FIRSTNAME + " " + r.LastNAME;
					newlist.Add(a);
				}
				return Json(new { response = newlist });
			}
			else
			{
				return Json(new { response = 0 });
			}
		}

		[HttpPost]
		public IActionResult UploadFilesAjax()
		{
			string message = "";
			try
			{
				string rootPath = _configuration.GetValue<string>("SystemSettings:AgentLogoImagePath");
				//string pathIntial = _configuration.GetValue<string>("SystemSettings:CountryImageInitial");
				string pathIntial = _configuration.GetValue<string>("UIBaseUrl");
				string imgDbPath = _configuration.GetValue<string>("SystemSettings:AgentLogoImageDbPath");

				var files = Request.Form.Files;
				if (files == null || files.Count <= 0)
					message = "Please select file";
				else
				{
					foreach (var file in files)
					{
						string CompanyId = Path.GetFileName(file.FileName);
						// string rootPath = _env.WebRootPath + @"\resources\AgentLogos";

						string _FileName = Path.GetFileNameWithoutExtension(file.Name);
						string extension = Path.GetExtension(file.Name);
						string _path = Path.Combine(rootPath, _FileName + extension);
						var savePath = imgDbPath + _FileName + extension;

						using (var fileStream = new FileStream(_path, FileMode.OpenOrCreate))
						{
							file.CopyTo(fileStream);
							fileStream.Flush();
							fileStream.Close();
						}

						#region Set logo path
						AgentSetReq request = new AgentSetReq();
						request.companies.LogoFilePath = savePath;
						request.companies.Company_Id = CompanyId;
						request.companies.EditUser = ckUserEmailId;
						request.IsCompany = true;
						AgentSetRes response = agentProviders.SetAgentDetailedInfo(request, token).Result;
						#endregion

						var Status = response.ResponseStatus.Status;
						if (Status.ToLower() == "success")
							message = "success";
						else
							message = "error";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return Json(message);
		}

		[Authorize]
		[HttpPost]
		public IActionResult UpdateAgentDetails(AgentDetails model)
		{
			try
			{
				model.CompanyName = HttpUtility.HtmlDecode(model.CompanyName);
				string FrmSuccessMessage = "", FrmErrorMessage = "", type = "", CompanyId = "";
				if (string.IsNullOrEmpty(model.CountryId)) model.CountryName = string.Empty;
				if (string.IsNullOrEmpty(model.CityId)) model.CityName = string.Empty;
				if (model != null && !string.IsNullOrEmpty(model.CompanyId))
				{
					bool IsDuplicate = CheckDuplicate(model.CompanyName, model.CountryId, model.CityId, model.Issupplier, out CompanyId);
					if (CompanyId != model.CompanyId && IsDuplicate)
					{
						if (model.Issupplier == true) type = "Supplier";
						else type = "Agent";

						FrmErrorMessage = type + " already exists";
					}
					else
					{
						AgentSetReq req = new AgentSetReq();
						req.IsCompany = true;
						req.EditUser = ckUserEmailId;
						req.companies.Company_Id = model.CompanyId;
						req.companies.Name = model.CompanyName;
						req.companies.CompanyCode = model.CompanyCode;
						req.companies.Street = model.Street ?? " ";
						req.companies.Street2 = model.Street2 ?? " ";
						req.companies.Street3 = model.Street3 ?? " ";
						req.companies.Country_Id = model.CountryId;
						req.companies.CountryName = model.CountryName;
						req.companies.City_Id = model.CityId;
						req.companies.CityName = model.CityName;
						req.companies.Zipcode = model.Zipcode;
						req.companies.Issubagent = model.Issubagent;
						req.companies.B2C = model.B2C;
						req.companies.EditUser = ckUserEmailId;
						req.companies.ParentAgent_Id = model.SalesOffices;
						req.companies.ParentAgent_Name = model.SalesOfficesName;
						req.companies.STATUS = model.Status.ToLower() == "active" ? string.Empty : "X";
						AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;

						//Call Bridge service to save Contact data in SQL databse
						ResponseStatus result = new ResponseStatus();
						if (res.ResponseStatus != null)
						{
							if (res.ResponseStatus.Status.ToLower() == "success")
							{
								SetCompanyKeyInfo_RQ request = new SetCompanyKeyInfo_RQ();
								request.Company_Id = model.CompanyId;
								request.User = ckUserEmailId;
								result = agentProviders.SetCompanyKeyInfo(request, token).Result;
								if (result.Status == "Success")
								{
									FrmSuccessMessage = "Details Updated Successfully. " + result.StatusMessage;
								}
								else
								{
									FrmErrorMessage = "Error in updating records. " + result.StatusMessage;
								}
							}
							else
							{
								FrmErrorMessage = "Error in updating records.";
							}
						}
					}
				}
				else
					FrmErrorMessage = "Error in updating records.";

				if (!(string.IsNullOrEmpty(FrmSuccessMessage))) TempData["frmsuccess"] = FrmSuccessMessage;
				if (!(string.IsNullOrEmpty(FrmErrorMessage))) TempData["frmerror"] = FrmErrorMessage;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			if (model.Issupplier == false)
				return RedirectToAction("AgentDetail", new { model.CompanyId });
			else
				return RedirectToAction("SupplierDetail", "Supplier", new { model.CompanyId });
		}

		public IActionResult GetAgentDetailsBranches(string CompanyId)
		{
			AgentDetails model = new AgentDetails();
			try
			{
				request = new AgentGetReq() { CompanyId = CompanyId };
				AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				model.CompanyId = CompanyId;
				bool issupplier = false; if (response.AgentDetails.Issupplier == null || response.AgentDetails.Issupplier == false) { issupplier = false; } else { issupplier = true; }
				model.Issupplier = issupplier;

				List<Branches> lst = new List<Branches>();
				foreach (var b in response.AgentDetails.Branches.OrderBy(x => x.Company_Name))
				{
					Branches branch = new Branches();
					branch.ParentCompanyId = b.ParentCompany_Id ?? CompanyId;
					branch.BranchCompanyId = b.Company_Id;
					branch.BranchCompanyName = b.Company_Name;
					branch.BranchCompanyCode = b.Company_Code;
					lst.Add(branch);
				}
				model.Branches = lst;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return PartialView("_AgentDetailsBranches", model);
		}

		public IActionResult GetAgentDetailsLogo(string CompanyId)
		{
			AgentDetails model = new AgentDetails();
			try
			{
				request = new AgentGetReq() { CompanyId = CompanyId };
				AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				model.LogoFilePath = response.AgentDetails.LogoFilePath;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return PartialView("_AgentDetailLogoUpload", model);
		}

		public IActionResult AddNewBranch(string ParentCompanyId, string CompanyId, bool Issupplier = false, bool IsForSalesAgent = false, string ProductSupplierId = "")
		{
			Branches model = new Branches();

			model.ParentCompanyId = ParentCompanyId;
			model.IsForSalesAgent = IsForSalesAgent;
			model.ProductSupplierId = ProductSupplierId;
			if (IsForSalesAgent == true) model.Issupplier = false; else model.Issupplier = Issupplier;
			return PartialView("_AddNewBranch", model);
		}

		public IActionResult SaveNewBranch(Branches model)
		{
			var status = "";
			request = new AgentGetReq() { CompanyId = model.ParentCompanyId };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			var msg = ""; var name = "";
			if (!string.IsNullOrEmpty(model.BranchCompanyName))
			{
				name = model.BranchCompanyName;

				if (response.AgentDetails.Branches != null && (model.BranchCompanyId != response.AgentDetails.Company_Id))
				{
					var IsDuplicate = response.AgentDetails.Branches.Where(x => x.Company_Name == name).Select(x => x.Company_Name).FirstOrDefault();
					if (!string.IsNullOrEmpty(IsDuplicate))
					{
						status = "error";
						msg = "Duplicate record. Cannot insert as " + IsDuplicate + " is already exists. Please select another agent.";
					}
					else
					{
						AgentSetReq req = new AgentSetReq();
						req.companies.Company_Id = model.ParentCompanyId;
						req.EditUser = ckUserEmailId;
						req.IsNewBranch = true;
						ChildrenCompanies b = new ChildrenCompanies()
						{
							ParentCompany_Id = model.ParentCompanyId,
							Company_Id = model.BranchCompanyId,
							Company_Code = model.BranchCompanyCode,
							Company_Name = model.BranchCompanyName
						};
						req.companies.Branches.Add(b);
						AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;

						//Call Bridge service to save Contact data in SQL databse
						ResponseStatus result = new ResponseStatus();
						if (res.ResponseStatus != null)
						{
							if (res.ResponseStatus.Status.ToLower() == "success")
							{
								SetCompanyKeyInfo_RQ request = new SetCompanyKeyInfo_RQ();
								request.Company_Id = model.ParentCompanyId;
								request.User = ckUserEmailId;
								result = agentProviders.SetCompanyBranches(request, token).Result;

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

		public IActionResult NewAgentDetails(string CompanyId, string CompanyName)
		{
			try
			{
				AgentDetails model = new AgentDetails();

				request = new AgentGetReq();
				response = new AgentGetRes();
				request.CompanyId = CompanyId;
				response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				if (response != null)
				{
					bool issupplier = false; if (response.AgentDetails.Issupplier == null || response.AgentDetails.Issupplier == false) { issupplier = false; } else { issupplier = true; }
					model.CompanyId = response.AgentDetails.Company_Id;
					model.CompanyName = response.AgentDetails.Name;
					model.CompanyCode = response.AgentDetails.CompanyCode;
					model.Street = response.AgentDetails.Street;
					model.Street2 = response.AgentDetails.Street2;
					model.Street3 = response.AgentDetails.Street3;
					model.Zipcode = response.AgentDetails.Zipcode;
					model.SalesOfficesList = response.AgentDetails.SalesOffices;
					model.Issubagent = response.AgentDetails.Issubagent.HasValue ? response.AgentDetails.Issubagent.Value : false;
					model.B2C = response.AgentDetails.B2C.HasValue ? response.AgentDetails.B2C.Value : false;
					model.LogoFilePath = response.AgentDetails.LogoFilePath;
					model.CountryId = response.AgentDetails.Country_Id;
					model.CountryName = response.AgentDetails.CountryName;
					model.CityId = response.AgentDetails.City_Id;
					model.CityName = response.AgentDetails.CityName;
					model.SalesOffices = response.AgentDetails.ParentAgent_Id;
					model.SalesOfficesName = response.AgentDetails.ParentAgent_Name;
					model.Issupplier = issupplier;
					model.Status = (response.AgentDetails.STATUS == "" || response.AgentDetails.STATUS == " ") ? "Active" : "Inactive";

					#region Get country and city list from masters
					var countrylist = masterProviders.GetAllCountries(token).Result;
					var countryId = response.AgentDetails.Country_Id;
					model.CountryList = countrylist;
					var citylist = masterProviders.GetAllCitiesByCountryId(countryId, token).Result;
					model.CityList = citylist;
					#endregion
				}
				return PartialView("_AgentDetails", model);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		public IActionResult DeleteBranch(string ParentCompanyId, string CompanyId)
		{
			var status = ""; var msg = ""; var contactId = "";
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;


			AgentSetReq req = new AgentSetReq();
			req.companies.Company_Id = ParentCompanyId;
			req.IsRemoveBranch = true;
			req.EditUser = ckUserEmailId;
			ChildrenCompanies b = new ChildrenCompanies()
			{
				ParentCompany_Id = ParentCompanyId,
				Company_Id = CompanyId
			};
			req.companies.Branches.Add(b);
			AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;

			//Call Bridge service to save Contact data in SQL databse
			ResponseStatus result = new ResponseStatus();
			if (res.ResponseStatus != null)
			{
				if (res.ResponseStatus.Status.ToLower() == "success")
				{
					SetCompanyKeyInfo_RQ request = new SetCompanyKeyInfo_RQ();
					request.Company_Id = ParentCompanyId;
					request.User = ckUserEmailId;
					result = agentProviders.SetCompanyBranches(request, token).Result;

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
		#endregion

		#region Contacts and Staff Details
		public IActionResult ContactsAndStaffDetails(string CompanyId, string CompanyName, string ContactId)
		{
			try
			{
				Contacts model = new Contacts();
				model.Company_Id = CompanyId;
				model.Company_Name = CompanyName;
				model.defaultContactId = ContactId;
				request = new AgentGetReq();
				response = new AgentGetRes();
				request.CompanyId = CompanyId;
				response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				model.BranchesList = response.AgentDetails.Branches;

				List<mStatus> lstStatus = new List<mStatus>();
				lstStatus = agentProviders.GetStatusForAgents(token).Result;
				if (lstStatus != null && lstStatus.Count > 0)
					model.StatusList = lstStatus;
				model.Status = " ";

				if (response != null)
				{
					List<ContactAndStaffDetails> CommonContacts = new List<ContactAndStaffDetails>();
					foreach (var c in response.AgentDetails.ContactDetails.Where(x => string.IsNullOrEmpty(x.ActualContact_Id_AsShared)))//&& string.IsNullOrWhiteSpace(x.STATUS)
					{
						ContactAndStaffDetails newContact = new ContactAndStaffDetails();
						newContact.Company_Id = c.Company_Id;
						newContact.Contact_Id = c.Contact_Id;
						//newContact.Company_Name = c.Company_Name;
						//newContact.ActualCompany_Name_AsShared = c.Company_Name;
						newContact.TITLE = c.TITLE;
						newContact.FIRSTNAME = c.FIRSTNAME;
						newContact.LastNAME = c.LastNAME;
						newContact.MAIL = c.MAIL;
						newContact.MOBILE = c.MOBILE;
						newContact.Status = c.STATUS;
						newContact.Default = c.Default == 2 ? false : true;
						CommonContacts.Add(newContact);
					}
					model.CommonContacts = CommonContacts.OrderBy(x => x.FIRSTNAME).ToList();

					List<ContactAndStaffDetails> SharedContacts = new List<ContactAndStaffDetails>();
					foreach (var c in response.AgentDetails.ContactDetails.Where(x => !string.IsNullOrEmpty(x.ActualContact_Id_AsShared)))// && string.IsNullOrWhiteSpace(x.STATUS)
					{
						ContactAndStaffDetails newContact = new ContactAndStaffDetails();
						newContact.Company_Id = c.Company_Id;
						newContact.Contact_Id = c.Contact_Id;
						newContact.Company_Name = c.Company_Name;
						newContact.TITLE = c.TITLE;
						newContact.FIRSTNAME = c.FIRSTNAME;
						newContact.LastNAME = c.LastNAME;
						newContact.MAIL = c.MAIL;
						newContact.MOBILE = c.MOBILE;
						newContact.Status = c.STATUS;
						newContact.Default = c.Default == 2 ? false : true;
						SharedContacts.Add(newContact);
					}
					model.SharedContacts = SharedContacts.OrderBy(x => x.FIRSTNAME).ToList();

					List<ContactAndStaffDetails> EmergencyContacts = new List<ContactAndStaffDetails>();
					foreach (var c in response.AgentDetails.EmergencyContacts)
					{
						ContactAndStaffDetails newContact = new ContactAndStaffDetails();
						newContact.EmergencyContact_Id = c.EmergencyContact_Id;
						newContact.Company_Id = c.Company_Id;
						newContact.Contact_Id = c.Contact_Id;
						newContact.Country_Id = c.Country_Id;
						newContact.Country = c.Country;
						newContact.ContactName = c.ContactName;
						newContact.EmergencyNo = c.EmergencyNo;
						newContact.Default = Convert.ToBoolean(c.Default);
						newContact.BusiType = c.BusiType;
						newContact.Status = c.Status;
						EmergencyContacts.Add(newContact);
					}
					model.EmergencyContacts = EmergencyContacts.OrderBy(x => x.ContactName).ToList();
				}
				return PartialView("_ContactsAndStaffDetails", model);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		public IActionResult ViewContactDetails(string CompanyId, string CompanyName, string ContactId)
		{
			if (UserRoles.Contains("Administrator"))
			{
				ViewBag.IsAllowToEdit = true;
			}

			ContactAndStaffDetails model = new ContactAndStaffDetails();
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			model.Company_Id = CompanyId;
			model.Company_Name = response.AgentDetails.Name;

			List<mStatus> lstStatus = new List<mStatus>();
			lstStatus = agentProviders.GetStatusForAgents(token).Result;
			if (lstStatus != null && lstStatus.Count > 0)
				model.StatusList.AddRange(lstStatus);
			model.Status = " ";

			List<mDefStartPage> lstStartPage = new List<mDefStartPage>();
			lstStartPage = agentProviders.GetStartPageForAgents(token).Result;
			if (lstStartPage != null && lstStartPage.Count > 0)
				model.StartPageList = lstStartPage;

			AgentGetReq request1 = new AgentGetReq();
			request1.CompanyId = CompanyId;
			request1.ContactId = ContactId;
			AgentGetRes response1 = new AgentGetRes();
			response1 = agentProviders.GetUserDetailsByContactId(request1, token).Result;
			if (!string.IsNullOrEmpty(response1.UserId))
				model.IsUserExists = true;
			else
				model.IsUserExists = false;

			model.User_Id = response1.UserId;

			UserRoleGetReq req = new UserRoleGetReq() { UserID = model.User_Id, CompanyID = CompanyId, ContactID = ContactId };
			UserRoleGetRes res = loginProviders.GetUserRoleDetails(req, token).Result;
			model.UserRolesDetails = res.UserRolesDetails.Where(x => !string.IsNullOrWhiteSpace(x.RoleName)).ToList();

			if (response != null && !string.IsNullOrEmpty(ContactId))
			{
				var contact = response.AgentDetails.ContactDetails.Where(x => x.Contact_Id == ContactId).FirstOrDefault();
				bool defval; if (contact.Default == 1) defval = true; else defval = false;
				string statusval; if (contact.STATUS == null || contact.STATUS == "") statusval = " "; else statusval = contact.STATUS;

				model.Contact_Id = contact.Contact_Id;
				//model.Company_Id = contact.Company_Id;
				model.CommonTitle = contact.CommonTitle;
				model.FIRSTNAME = contact.FIRSTNAME;
				model.LastNAME = contact.LastNAME;
				model.Department = contact.DEPARTMENT;
				model.TITLE = contact.TITLE; // Job title
				model.MOBILE = contact.MOBILE;
				model.TEL = contact.TEL;
				model.MAIL = contact.MAIL;
				model.WEB = contact.WEB;
				model.Status = statusval;
				model.StartPageId = contact.StartPage_Id;
				model.StartPage = contact.Start_Page;
				model.Default = defval;
				model.UserName = contact.UserName ?? contact.MAIL;
				model.Password = contact.Password;
				model.ConfirmPassword = contact.Password;
				model.IsCentralEmail = contact.IsCentralEmail;
				//model.StatusList = response.StatusList;


				string[] TargetTabRoles = new string[] { "Administrator", "Sales Manager" };
				var targetrole = UserRoles.Where(a => TargetTabRoles.Contains(a)).FirstOrDefault();
				if (targetrole != null && response.AgentDetails.HeadOffice_Id == ckUserCompanyId)
				{
					ViewBag.IsAllowToEditForTarget = true;
					AgentGetReq agentGetReq = new AgentGetReq() { CompanyId = CompanyId, ContactId = ContactId };
					TargetGetRes objTargetGetRes = agentProviders.GetCompanyTargets(agentGetReq, token).Result;
					objTargetGetRes.TargetList = objTargetGetRes.TargetList?.OrderBy(a => a.Month).ToList();
					model.TargetGetRes = objTargetGetRes != null ? objTargetGetRes : new TargetGetRes() { ResponseStatus = new ResponseStatus() { Status = "Error" }, TargetList = new List<Targets>() };
				}
			}
			model.UserRolesDetails = model.UserRolesDetails.OrderBy(x => x.RoleName).ToList();

			return PartialView("_ViewContactDetails", model);
		}

		public IActionResult CreateUser(ContactAndStaffDetails model)
		{
			var status = ""; var msg = ""; var userId = "";
			try
			{
				//Save into mCompanies
				AgentSetReq req = new AgentSetReq();
				req.companies.Company_Id = model.Company_Id;
				req.EditUser = ckUserEmailId;
				CompanyContacts b = new CompanyContacts()
				{
					Company_Id = model.Company_Id,
					Contact_Id = model.Contact_Id,
					UserName = model.UserName,
					PasswordText = model.Password,
					//Password = model.Password,
					EditUser = ckUserEmailId
				};
				req.companies.ContactDetails.Add(b);
				AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
				var contactId = res.ContactId;
				//End of mcompanies

				AgentGetReq req1 = new AgentGetReq();
				AgentGetRes res1 = new AgentGetRes();
				req1.CompanyId = model.Company_Id;
				res1 = agentProviders.GetAgentDetailedInfo(req1, token).Result;
				var contact = res1.AgentDetails.ContactDetails.Where(x => x.Contact_Id == contactId).FirstOrDefault();
				UserSetReq request = new UserSetReq();
				UserSetRes response = new UserSetRes();
				request.User.Email = contact.MAIL;
				request.User.UserName = contact.UserName;
				request.User.Password = contact.Password;
				request.User.FirstName = contact.FIRSTNAME;
				request.User.LastName = contact.LastNAME;
				request.User.Email = contact.MAIL;
				request.User.IsActive = true;
				request.User.Contact_Id = contact.Contact_Id;
				request.User.Company_Id = model.Company_Id;
				request.User.CreateUser = ckUserEmailId;
				request.User.CreateDate = DateTime.Now;
				response = loginProviders.CreateUser(request, token).Result;
				if (response.ResponseStatus.Status.ToLower() == "error")
				{
					status = "error";
					msg = "User is already created with same Email Id";
				}
				else
				{
					//AgentGetReq request1 = new AgentGetReq();
					//request1.CompanyId = model.Company_Id;
					//request1.ContactId = model.Contact_Id;
					//AgentGetRes response1 = new AgentGetRes();
					//response1 = agentProviders.GetUserDetailsByContactId(request1, token).Result;
					userId = response.UserId;
					model.User_Id = response.UserId;

					if (!string.IsNullOrEmpty(model.User_Id))
					{
						// Call Bridge service to save Contact data in SQL databse
						ResponseStatus resultcontact = new ResponseStatus();
						SetCompanyContact_RQ requestcontact = new SetCompanyContact_RQ();
						requestcontact.Contact_Id = res.ContactId;
						requestcontact.User = ckUserEmailId;
						resultcontact = agentProviders.SetCompanyContact(requestcontact, token).Result;
						if (resultcontact != null && !string.IsNullOrEmpty(resultcontact.Status))
						{
							if (resultcontact.Status.ToLower() == "success")
							{
								model.UserRolesDetails.ForEach(a => a.UserId = userId);
								//foreach (var id in model.UserRolesDetails)
								//{
								//    id.UserId = userId;
								//}

								UserRoleSetReq urreq = new UserRoleSetReq();
								urreq.UserName = response.UserName;
								urreq.CompanyId = model.Company_Id;
								urreq.ContactId = model.Contact_Id;
								urreq.UserId = userId;
								urreq.EditUser = ckUserEmailId;
								urreq.UserRoleDetailsList = model.UserRolesDetails.Where(x => x.IsRoled == true).ToList();
								UserRoleSetRes ures = loginProviders.SetUserRoleDetails(urreq, token).Result;

								if (ures != null && ures.ResponseStatus != null && !string.IsNullOrEmpty(ures.ResponseStatus.Status))
								{
									// Call Bridge service to create & update User Password in SQL database 
									ResponseStatus resultusepwd = new ResponseStatus();
									UserRolesSetReq userreq = new UserRolesSetReq();
									userreq.Contact_Id = res.ContactId;
									resultusepwd = agentProviders.SetUserPassword(userreq, token).Result;

									if (resultusepwd.Status.ToLower() == "success")
									{
										// Call Bridge service to save & update User Roles in SQL database
										ResponseStatus result = new ResponseStatus();
										result = agentProviders.SetUserRoles(userreq, token).Result;
										status = "success";
										msg = "User details created successfully.";
									}
									else
									{
										status = "error";
										msg = resultusepwd.ErrorMessage + " " + resultusepwd.StatusMessage;
									}
								}
								else
								{
									status = "error";
									msg = "User Roles not updated";
								}
							}
							else
							{
								status = "error";
								msg = resultcontact.ErrorMessage + " " + resultcontact.StatusMessage;
							}
						}
						else
						{
							status = "error";
							msg = "Company Contact not updated";
						}
					}
					else
					{
						status = "success";
						msg = "User already exists";
					}
				}

				return Json(new { status = status, responseText = msg, userId = userId });
			}
			catch (Exception ex)
			{
				return Json(new { status = "error", responseText = ex.Message, userId = userId });
			}
		}

		public IActionResult RemoveUser(ContactAndStaffDetails model)
		{
			var status = ""; var msg = ""; var userId = "";
			try
			{
				AgentGetReq request1 = new AgentGetReq();
				request1.CompanyId = model.Company_Id;
				request1.ContactId = model.Contact_Id;
				AgentGetRes response1 = new AgentGetRes();
				response1 = agentProviders.GetUserDetailsByContactId(request1, token).Result;
				userId = response1.UserId;

				if (userId != null)
				{
					UserSetReq request = new UserSetReq();
					UserSetRes response = new UserSetRes();
					request.User.VoyagerUser_Id = userId;
					response = loginProviders.EnableDisableUser(request, token).Result;
					status = "success";
				}
				else
				{
					status = "error";
				}

				return Json(new { status = status, responseText = msg, userId = userId });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Json(new { status = "error", responseText = ex.Message });
			}
		}

		public IActionResult SendResetUserPassword(string UserName)
		{
			AgentGetReq request = new AgentGetReq();
			request.UserName = UserName;
			AgentGetRes response1 = new AgentGetRes();
			response = agentProviders.GetUserDetailsByContactId(request, token).Result;

			LoginRequest objLoginRequest = new LoginRequest();
			objLoginRequest.UserName = UserName;
			objLoginRequest.CompanyId = response.User.Company_Id;
			objLoginRequest.ContactId = response.User.Contact_Id;
			LoginProviders objLoginProvider = new LoginProviders(_configuration);
			bool tskCheckUser = objLoginProvider.UserPasswordRecover(objLoginRequest).Result;
			if (tskCheckUser)
			{
				// Call Bridge service to update User Password in SQL database
				ResponseStatus resultusepwd = new ResponseStatus();
				UserRolesSetReq userreq = new UserRolesSetReq();
				userreq.Contact_Id = response.User.Contact_Id;
				resultusepwd = agentProviders.SetUserPassword(userreq, token).Result;
				if (resultusepwd != null && !string.IsNullOrEmpty(resultusepwd.Status) && resultusepwd.Status.ToLower() == "success")
				{
					return Json(new { status = "success", msg = "Password Reset and sent via email" });
				}
				else
				{
					return Json(new { status = "error", msg = !string.IsNullOrEmpty(resultusepwd.ErrorMessage) ? resultusepwd.ErrorMessage : "Error Occurs." });
				}
			}
			else
			{
				return Json(new { status = "error", msg = "Error" });
			}
		}

		public IActionResult SaveNewContact(ContactAndStaffDetails model)
		{
			var status = ""; var msg = "";
			request = new AgentGetReq() { CompanyId = model.Company_Id };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			AgentSetReq req = new AgentSetReq();
			var contact = response.AgentDetails.ContactDetails.Where(x => x.Contact_Id == model.Contact_Id).FirstOrDefault();
			if (contact != null)
				req.IsNewContact = false;
			else
				req.IsNewContact = true;

			req.companies.Company_Id = model.Company_Id;
			req.EditUser = ckUserEmailId;

			var pwd = "";
			if (contact != null)
				pwd = string.IsNullOrEmpty(model.Password) ? contact.Password : model.Password;
			else
				pwd = string.IsNullOrEmpty(model.Password) ? "" : model.Password;

			byte defval;
			if (model.Default == true)
				defval = 1;
			else
				defval = 2;

			if (string.IsNullOrEmpty(model.StartPageId)) model.StartPage = string.Empty;

			CompanyContacts b = new CompanyContacts()
			{
				Company_Id = model.Company_Id,
				Contact_Id = model.Contact_Id,
				Company_Name = model.Company_Name,
				ActualCompany_Name_AsShared = model.Company_Name,
				CommonTitle = model.CommonTitle,
				FIRSTNAME = model.FIRSTNAME,
				LastNAME = model.LastNAME,
				DEPARTMENT = model.Department,
				TITLE = model.TITLE,
				MOBILE = model.MOBILE,
				TEL = model.TEL,
				MAIL = model.MAIL,
				WEB = model.WEB,
				STATUS = model.Status ?? " ",
				StartPage_Id = model.StartPageId,
				Start_Page = model.StartPage,
				Default = defval,
				UserName = model.UserName ?? model.MAIL,
				PasswordText = model.Password,
				IsCentralEmail = model.IsCentralEmail,
				CreateUser = ckUserEmailId,
				EditUser = ckUserEmailId
			};
			req.companies.ContactDetails.Add(b);
			AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;

			if (res != null && res.ResponseStatus != null && !string.IsNullOrEmpty(res.ResponseStatus.Status))
			{
				// Call Bridge service to save Contact data in SQL databse
				ResponseStatus result = new ResponseStatus();
				if (res.ResponseStatus.Status.ToLower() == "success")
				{
					SetCompanyContact_RQ request = new SetCompanyContact_RQ();
					request.Contact_Id = res.ContactId;
					request.User = ckUserEmailId;
					result = agentProviders.SetCompanyContact(request, token).Result;

					if (result != null && !string.IsNullOrEmpty(result.Status) && result.Status.ToLower() == "success")
					{
						//var userId = response.UserId;
						AgentGetReq request1 = new AgentGetReq();
						request1.CompanyId = model.Company_Id;
						request1.ContactId = model.Contact_Id;
						AgentGetRes response1 = new AgentGetRes();
						response1 = agentProviders.GetUserDetailsByContactId(request1, token).Result;
						model.User_Id = response1.UserId;

						if (!string.IsNullOrEmpty(model.User_Id))
						{
							model.UserRolesDetails.ForEach(a => a.UserId = model.User_Id);
							//foreach (var id in model.UserRolesDetails)
							//{
							//    id.UserId = model.User_Id;
							//}

							UserRoleSetReq urreq = new UserRoleSetReq();
							urreq.CompanyId = model.Company_Id;
							urreq.ContactId = model.Contact_Id;
							urreq.UserName = res.UserName;
							urreq.UserId = model.User_Id;
							urreq.EditUser = ckUserEmailId;
							urreq.UserRoleDetailsList = model.UserRolesDetails.ToList();
							UserRoleSetRes ures = loginProviders.SetUserRoleDetails(urreq, token).Result;

							if ((!string.IsNullOrEmpty(contact.MAIL) && !string.IsNullOrEmpty(model.MAIL) && contact.MAIL != model.MAIL) ||
								(!string.IsNullOrEmpty(contact.FIRSTNAME) && !string.IsNullOrEmpty(model.FIRSTNAME) && contact.FIRSTNAME != model.FIRSTNAME) ||
								(!string.IsNullOrEmpty(contact.LastNAME) && !string.IsNullOrEmpty(model.LastNAME) && contact.LastNAME != model.LastNAME))
							{
								UserSetReq req1 = new UserSetReq();
								req1.User.VoyagerUser_Id = model.User_Id;
								req1.User.UserName = model.UserName ?? model.MAIL;
								req1.User.FirstName = model.FIRSTNAME;
								req1.User.LastName = model.LastNAME;
								req1.User.Email = model.UserName ?? model.MAIL;
								req1.User.EditUser = ckUserEmailId;
								UserSetRes res1 = loginProviders.UpdateUser(req1, token).Result;
							}

							// Call Bridge service to save User Roles in SQL database
							if (ures != null && ures.ResponseStatus != null && !string.IsNullOrEmpty(ures.ResponseStatus.Status))
							{
								ResponseStatus resultuserrole = new ResponseStatus();
								UserRolesSetReq userreq = new UserRolesSetReq();
								userreq.Contact_Id = res.ContactId;

								resultuserrole = agentProviders.SetUserRoles(userreq, token).Result;
								if (resultuserrole?.Status?.ToLower() == "success")
								{
									status = "success";
									msg = "Details Saved successfully.";
								}
								else
								{
									if (resultuserrole.StatusMessage == "User Roles Not Found in MongoDB")
									{
										status = "success";
										msg = "Details Saved successfully.";
									}
									else
									{
										status = "error1";
										msg = resultuserrole.StatusMessage;
									}
								}
							}
							else
							{
								status = "error1";
								msg = "User Roles not updated";
							}
						}
						else
						{
							msg = "Please create user.";
							status = "error1";
						}
					}
					else
					{
						msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
						status = "error";
					}
				}
				else
				{
					status = "error";
					msg = res.ResponseStatus.ErrorMessage;
				}
			}
			else
			{
				status = "error";
				msg = "An Error Occurs while saving Contact details";
			}
			return Json(new { status = status, responseText = msg, contactId = res.ContactId, username = res.UserName });
		}

		public IActionResult ViewEmergencyContacts(string CompanyId, string EmergencyContactId)
		{
			ContactAndStaffDetails model = new ContactAndStaffDetails();
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			model.Company_Id = CompanyId;

			List<mStatus> lstStatus = new List<mStatus>();
			lstStatus = agentProviders.GetStatusForAgents(token).Result;
			if (lstStatus != null && lstStatus.Count > 0)
				model.StatusList = lstStatus;

			#region Get country and city list from masters
			var countrylist = masterProviders.GetAllCountries(token).Result;
			model.CountryList = countrylist;
			#endregion

			#region Get Contact list
			var contactlist = response.AgentDetails.ContactDetails.Where(x => string.IsNullOrWhiteSpace(x.STATUS)).ToList();

			//list of current company contacts                
			var currentcontacts = response.AgentDetails.EmergencyContacts.Select(x => x.ContactName);
			//contactlist.RemoveAll(x => currentcontacts.Contains(x.FIRSTNAME + " " + x.LastNAME));

			List<Attributes> contacts = new List<Attributes>();
			foreach (var a in contactlist)
			{
				Attributes list = new Attributes();
				list.Attribute_Id = a.Contact_Id;
				list.AttributeName = a.FIRSTNAME + " " + a.LastNAME;
				contacts.Add(list);
			}
			model.ContactList = contacts;
			#endregion

			if (response != null && !string.IsNullOrEmpty(EmergencyContactId))
			{
				var contact = response.AgentDetails.EmergencyContacts.Where(x => x.EmergencyContact_Id == EmergencyContactId).FirstOrDefault();
				model.EmergencyContact_Id = contact.EmergencyContact_Id;
				model.Country_Id = contact.Country_Id;
				model.Country = contact.Country;
				model.EmergencyNo = contact.EmergencyNo;
				model.Default = Convert.ToBoolean(contact.Default);
				model.Contact_Id = contact.Contact_Id;
				model.ContactName = contact.ContactName;
				model.MAIL = contact.ContactMail;
				model.TEL = contact.ContactTel;
				model.BusiType = contact.BusiType;
				model.Status = contact.Status;
			}

			return PartialView("_ViewEmergencyContacts", model);
		}

		public IActionResult SaveEmergencyContact(ContactAndStaffDetails model)
		{
			var status = "";
			request = new AgentGetReq() { CompanyId = model.Company_Id };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			var msg = ""; var EmergencyContactId = "";
			AgentSetReq req = new AgentSetReq();
			var contact = response.AgentDetails.EmergencyContacts.Where(x => x.EmergencyContact_Id == model.EmergencyContact_Id).FirstOrDefault();
			var existscont = response.AgentDetails.EmergencyContacts.Where(x => x.Country_Id == model.Country_Id && x.Country == model.Country && x.ContactName == model.ContactName && x.EmergencyNo == model.EmergencyNo).FirstOrDefault();
			if (contact != null)
			{
				req.IsNewEmergencyContact = false;
				EmergencyContactId = contact.EmergencyContact_Id;

				byte defval;
				if (model.Default == true)
					defval = 1;
				else
					defval = 2;

				req.companies.Company_Id = model.Company_Id;
				req.EditUser = ckUserEmailId;
				EmergencyContacts b = new EmergencyContacts()
				{
					EmergencyContact_Id = model.EmergencyContact_Id,
					Company_Id = model.Company_Id,
					Country_Id = model.Country_Id,
					Country = model.Country,
					EmergencyNo = model.EmergencyNo,
					Default = model.Default,
					Contact_Id = model.Contact_Id,
					ContactName = model.ContactName,
					ContactMail = model.MAIL,
					ContactTel = model.TEL,
					BusiType = model.BusiType,
					Status = model.Status ?? " ",
				};
				req.companies.EmergencyContacts.Add(b);
				AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
				if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; msg = "Details saved successfully"; EmergencyContactId = res.EmergencyContactId; }
				else if (res.ResponseStatus.Status.ToLower() == "failure") { status = "error"; msg = res.ResponseStatus.ErrorMessage; }
			}
			else
			{
				req.IsNewEmergencyContact = true;
				if (existscont != null)
				{
					status = "error";
					if (existscont.Status == "X")
						msg = "Record is inactive and already exists. Please Activate it";
					else
						msg = "Duplicate record";
				}
				else
				{
					byte defval;
					if (model.Default == true)
						defval = 1;
					else
						defval = 2;

					req.companies.Company_Id = model.Company_Id;
					req.EditUser = ckUserEmailId;
					EmergencyContacts b = new EmergencyContacts()
					{
						EmergencyContact_Id = model.EmergencyContact_Id,
						Company_Id = model.Company_Id,
						Country_Id = model.Country_Id,
						Country = model.Country,
						EmergencyNo = model.EmergencyNo,
						Default = model.Default,
						Contact_Id = model.Contact_Id,
						ContactName = model.ContactName,
						ContactMail = model.MAIL,
						ContactTel = model.TEL,
						BusiType = model.BusiType,
						Status = model.Status ?? " ",
					};
					req.companies.EmergencyContacts.Add(b);
					AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
					model.EmergencyContact_Id = res.EmergencyContactId;
					if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; EmergencyContactId = res.EmergencyContactId; }
					else status = "error";
				}
			}

			if (status == "success")
			{
				//the below function will call the EmergencyContactSetReq Bridge service and update the EmergencyContact into 
				//EmergencyContacts SQL table
				EmergencyContactSetReq request = new EmergencyContactSetReq();
				request.Company_Id = model.Company_Id;
				request.EmergencyContact_Id = model.EmergencyContact_Id;
				request.User = ckUserEmailId;
				ResponseStatus res = agentProviders.SetEmergencyContact(request, token).Result;
				if (res != null)
				{
					if (!string.IsNullOrEmpty(res.Status) && res.Status == "Success")
					{
						status = "success";
						msg = "Details Updated Successfully.";
					}
					else
					{
						status = "error";
						msg = res.StatusMessage;
					}
				}
				else
				{
					status = "error";
					msg = "Error in updating records.";
				}
			}
			return Json(new { status = status, responseText = msg, emergencyContactId = EmergencyContactId });
		}

		public IActionResult AddSharedContact(string CompanyId, string CompanyName, string SelectedCompanyId, string SelectedCompanyName, string SelectedContactId, string SelectedDefault, string SelectedStatus)
		{
			try
			{
				var status = ""; var msg = ""; var contactId = "";
				if (!string.IsNullOrEmpty(SelectedCompanyName) && !string.IsNullOrEmpty(SelectedContactId) && !string.IsNullOrEmpty(SelectedDefault))
				{

					//Get new company details whos details we want to insert
					request = new AgentGetReq();
					response = new AgentGetRes();
					request.CompanyId = SelectedCompanyId;
					response = agentProviders.GetAgentDetailedInfo(request, token).Result;
					CompanyContacts newcontactdetails = response.AgentDetails.ContactDetails.Where(x => x.Contact_Id == SelectedContactId).FirstOrDefault();

					//insert into current company
					AgentSetReq req = new AgentSetReq();
					req.companies.Company_Id = CompanyId;
					req.IsNewContact = true;
					req.EditUser = ckUserEmailId;
					newcontactdetails.Company_Id = CompanyId;
					newcontactdetails.Company_Name = CompanyName;
					newcontactdetails.ActualContact_Id_AsShared = newcontactdetails.Contact_Id;
					newcontactdetails.ActualCompany_Id_AsShared = SelectedCompanyId;
					newcontactdetails.ActualCompany_Name_AsShared = SelectedCompanyName;
					newcontactdetails.Default = Convert.ToByte(SelectedDefault);
					newcontactdetails.STATUS = SelectedStatus ?? " ";

					req.companies.ContactDetails.Add(newcontactdetails);

					AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;

					//Call Bridge service to save Shared Contact data in SQL databse
					ResponseStatus result = new ResponseStatus();
					if (res.ResponseStatus != null)
					{
						if (res.ResponseStatus.Status.ToLower() == "success")
						{
							SetCompanyContact_RQ request = new SetCompanyContact_RQ();
							request.Contact_Id = res.ContactId;
							request.User = ckUserEmailId;
							result = agentProviders.SetCompanySharedContact(request, token).Result;

							status = result.Status;
							msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
						}
						else
						{
							status = "Failure";
							msg = res.ResponseStatus.ErrorMessage;
						}
					}
				}
				else
				{
					status = "error";
					msg = "Please select Company, Contact and Level";
				}
				return Json(new { status = status, responseText = msg, contactId = contactId });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Json(new { status = "error", responseText = "Error in saving record" });
			}
		}

		public IActionResult GetSharedContactDetails(string CompanyId, string SelectedcompanyId, string SelectedcompanyName, string SelectedContactId)
		{
			try
			{
				CompanyContacts newcontactdetails = new CompanyContacts();
				request = new AgentGetReq();
				response = new AgentGetRes();
				request.CompanyId = CompanyId;
				response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				newcontactdetails = response.AgentDetails.ContactDetails.Where(x => x.Contact_Id == SelectedContactId).FirstOrDefault();
				var companyId = newcontactdetails.ActualCompany_Id_AsShared;
				var actualCompanyName = newcontactdetails.ActualCompany_Name_AsShared;
				var actualContactId = newcontactdetails.ActualContact_Id_AsShared;
				var contactName = newcontactdetails.FIRSTNAME + " " + newcontactdetails.LastNAME;
				var def = newcontactdetails.Default == 0 ? null : newcontactdetails.Default;
				var contactdetailId = newcontactdetails.Contact_Id;
				var status = newcontactdetails.STATUS == "" ? " " : newcontactdetails.STATUS;

				return Json(new { companyId = companyId, actualCompanyName = actualCompanyName, actualContactId = actualContactId, contactName = contactName, def = def, contactdetailId = contactdetailId, status = status });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Json(new { status = "error" });
			}
		}

		public IActionResult UpdateSharedContactDetails(string CompanyId, string SelectedDefault, string SelectedContactId, string SelectedStatus)
		{
			try
			{
				var status = ""; var msg = ""; var contactId = "";
				request = new AgentGetReq();
				response = new AgentGetRes();
				request.CompanyId = CompanyId;
				response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				CompanyContacts newcontactdetails = response.AgentDetails.ContactDetails.Where(x => x.ActualContact_Id_AsShared == SelectedContactId).FirstOrDefault();

				AgentSetReq req = new AgentSetReq();
				req.companies.Company_Id = CompanyId;
				req.EditUser = ckUserEmailId;
				newcontactdetails.STATUS = SelectedStatus;
				newcontactdetails.Default = Convert.ToByte(SelectedDefault);
				req.companies.ContactDetails.Add(newcontactdetails);

				AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;

				//Call Bridge service to save Shared Contact data in SQL databse
				ResponseStatus result = new ResponseStatus();
				if (res.ResponseStatus != null)
				{
					if (res.ResponseStatus.Status.ToLower() == "success")
					{
						SetCompanyContact_RQ request = new SetCompanyContact_RQ();
						request.Contact_Id = res.ContactId;
						request.User = ckUserEmailId;
						result = agentProviders.SetCompanySharedContact(request, token).Result;

						status = result.Status;
						msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
					}
					else
					{
						status = "Failure";
						msg = res.ResponseStatus.ErrorMessage;
					}
				}

				return Json(new { status = status, responseText = msg, contactId = contactId });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Json(new { status = "Failure", msg = ex.Message });
			}
		}

		public IActionResult RemoveSharedContactDetails(string CompanyId, string SelectedContactId)
		{
			var status = ""; var msg = ""; var contactId = "";
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			//CompanyContacts newcontactdetails = response.AgentDetails.ContactDetails.Where(x => x.Contact_Id == SelectedContactId).FirstOrDefault();

			AgentSetReq req = new AgentSetReq();
			req.companies.Company_Id = CompanyId;
			req.EditUser = ckUserEmailId;
			CompanyContacts b = new CompanyContacts();
			b.Contact_Id = SelectedContactId;
			b.STATUS = "X";
			req.companies.ContactDetails.Add(b);

			AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;

			//Call Bridge service to save Shared Contact data in SQL databse
			ResponseStatus result = new ResponseStatus();
			if (res.ResponseStatus != null)
			{
				if (res.ResponseStatus.Status.ToLower() == "success")
				{
					SetCompanyContact_RQ request = new SetCompanyContact_RQ();
					request.Contact_Id = res.ContactId;
					request.User = ckUserEmailId;
					result = agentProviders.SetCompanySharedContact(request, token).Result;

					status = result.Status;
					msg = res.ResponseStatus.ErrorMessage + " " + result.StatusMessage;
				}
				else
				{
					status = "Failure";
					msg = res.ResponseStatus.ErrorMessage;
				}
			}

			return Json(new { status = status, responseText = msg, contactId = contactId });
		}

		public IActionResult RemoveEmergencyContact(string CompanyId, string SelectedEmergencyContactId, string Status)
		{
			var status = ""; var msg = ""; var contactId = "";
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;

			AgentSetReq req = new AgentSetReq();
			req.companies.Company_Id = CompanyId;
			req.EditUser = ckUserEmailId;
			EmergencyContacts b = new EmergencyContacts();
			b.EmergencyContact_Id = SelectedEmergencyContactId;
			if (Status != "X") b.Status = "X"; else b.Status = " ";
			req.companies.EmergencyContacts.Add(b);

			AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
			if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; contactId = res.ContactId; }
			else status = "error";

			if (status == "success" && b.Status == "X")
			{
				//the below function will call the DelEmergencyContact Bridge service and delete the EmergencyContact from 
				//EmergencyContacts SQL table
				EmergencyContactSetReq request = new EmergencyContactSetReq();
				request.EmergencyContact_Id = SelectedEmergencyContactId;
				request.Company_Id = CompanyId;
				request.User = ckUserEmailId;
				ResponseStatus resDel = agentProviders.DelEmergencyContact(request, token).Result;
				if (resDel != null)
				{
					if (!string.IsNullOrEmpty(resDel.Status) && resDel.Status == "Success")
					{
						status = "success";
						msg = "Details Deleted Successfully.";
					}
					else
					{
						status = "error";
						msg = resDel.StatusMessage;
					}
				}
				else
				{
					status = "error";
					msg = "Error in updating records.";
				}
			}
			else if (status == "success" && b.Status != "X")
			{
				//the below function will call the EmergencyContactSetReq Bridge service and update the EmergencyContact into 
				//EmergencyContacts SQL table
				EmergencyContactSetReq request = new EmergencyContactSetReq();
				request.Company_Id = CompanyId;
				request.EmergencyContact_Id = SelectedEmergencyContactId;
				request.User = ckUserEmailId;
				ResponseStatus resEmergency = agentProviders.SetEmergencyContact(request, token).Result;
				if (resEmergency != null)
				{
					if (!string.IsNullOrEmpty(resEmergency.Status) && resEmergency.Status == "Success")
					{
						status = "success";
						msg = "Details Updated Successfully.";
					}
					else
					{
						status = "error";
						msg = resEmergency.StatusMessage;
					}
				}
				else
				{
					status = "error";
					msg = "Error in updating records.";
				}
			}

			return Json(new { status = status, responseText = msg, contactId = contactId });
		}

		public IActionResult CheckExistingEmail(string EmailId)
		{
			bool res = loginProviders.CheckExistingEmail(EmailId, token).Result;
			string result = ""; var status = "";
			if (res == true)
				result = "true";
			else
				result = "false";

			return Json(new { status = result });
		}

		#endregion

		#region Commercials and Payment
		public IActionResult CommercialsAndPaymentDetails(string CompanyId)
		{
			try
			{
				CommercialsAndPayment model = new CommercialsAndPayment();

				#region Basis binding
				request = new AgentGetReq() { CompanyId = CompanyId };
				requestForTaxReg = new TaxRegestrationDetails_Req() { Company_Id = CompanyId };
				AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				TaxRegestrationDetails_Res responseForTaxReg = supplierProviders.GetTaxRegDetails(requestForTaxReg, token).Result;

				model.Company_Id = CompanyId;
				bool issupplier = false; if (response.AgentDetails.Issupplier == null || response.AgentDetails.Issupplier == false) { issupplier = false; } else { issupplier = true; }
				model.IsSupplier = issupplier;

				if (response.AgentDetails.Markups != null)
				{
					model.Group = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "groups").Select(x => x.Markup_Basis).FirstOrDefault();
					model.GroupsBasisList = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "groups" && string.IsNullOrEmpty(x.Markup_For)).ToList();
					model.MarkupSchemesGroupsType = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "groups").Select(x => x.Markup_Id).FirstOrDefault();
					model.GroupsTypeText = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "groups").Select(x => x.Markup).FirstOrDefault();
					model.GroupsValue = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "groups").Select(x => x.Markup_Value).FirstOrDefault();

					model.FIT = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "fit").Select(x => x.Markup_Basis).FirstOrDefault();
					model.FITBasisList = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "fit" && string.IsNullOrEmpty(x.Markup_For)).ToList();
					model.MarkupSchemesFITType = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "fit").Select(x => x.Markup_Id).FirstOrDefault();
					model.FITTypeText = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "fit").Select(x => x.Markup).FirstOrDefault();
					model.FITValue = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "fit").Select(x => x.Markup_Value).FirstOrDefault();

					model.Series = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "series").Select(x => x.Markup_Basis).FirstOrDefault();
					model.SeriesBasisList = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "series" && string.IsNullOrEmpty(x.Markup_For)).ToList();
					model.MarkupSchemesSeriesType = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "series").Select(x => x.Markup_Id).FirstOrDefault();
					model.SeriesTypeText = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "series").Select(x => x.Markup).FirstOrDefault();
					model.SeriesValue = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "series").Select(x => x.Markup_Value).FirstOrDefault();

					model.B2B2B = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "b2b2b").Select(x => x.Markup_Basis).FirstOrDefault();
					model.B2B2BBasisList = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "b2b2b" && string.IsNullOrEmpty(x.Markup_For)).ToList();
					model.MarkupSchemesB2B2BType = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "b2b2b").Select(x => x.Markup_Id).FirstOrDefault();
					model.B2B2BTypeText = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "b2b2b").Select(x => x.Markup).FirstOrDefault();
					model.B2B2BValue = response.AgentDetails.Markups.Where(x => x.Markup_For.ToLower() == "b2b2b").Select(x => x.Markup_Value).FirstOrDefault();
				}
				#endregion

				#region Master binding
				List<Attributes> attr = new List<Attributes>();
				attr.Add(new Attributes { Attribute_Id = "AMOUNT", AttributeName = "AMOUNT" });
				var markuplist = masterProviders.GetMarkups(token).Result;
				markuplist = markuplist.OrderBy(x => x.AttributeName).ToList();

				if (model.Group == "Percentage") model.Group = "Percentage / Scheme";
				if (model.FIT == "Percentage") model.FIT = "Percentage / Scheme";
				if (model.Series == "Percentage") model.Series = "Percentage / Scheme";
				if (model.B2B2B == "Percentage") model.B2B2B = "Percentage / Scheme";

				if (model.Group == "Amount") model.MarkupSchemesGroupsType = "AMOUNT";
				if (model.FIT == "Amount") model.MarkupSchemesFITType = "AMOUNT";
				if (model.Series == "Amount") model.MarkupSchemesSeriesType = "AMOUNT";
				if (model.B2B2B == "Amount") model.MarkupSchemesB2B2BType = "AMOUNT";

				model.GroupsTypeList = model.Group == "Amount" ? attr : markuplist;
				model.FITTypeList = model.FIT == "Amount" ? attr : markuplist;
				model.SeriesTypeList = model.Series == "Amount" ? attr : markuplist;
				model.B2B2BTypeList = model.B2B2B == "Amount" ? attr : markuplist;
				#endregion

				#region Account Details binding

				#region Get Contact list
				var contactlist = response.AgentDetails.ContactDetails.Where(x => string.IsNullOrWhiteSpace(x.STATUS));
				var emergencylist = response.AgentDetails.EmergencyContacts;
				List<Attributes> contacts = new List<Attributes>();

				foreach (var a in contactlist)
				{
					Attributes list = new Attributes();
					list.Attribute_Id = a.Contact_Id;
					list.AttributeName = a.FIRSTNAME + " " + a.LastNAME;
					contacts.Add(list);
				}

				model.FinanceContactList = contacts.OrderBy(x => x.AttributeName).ToList();
				#endregion

				model.FinanceContactId = response.AgentDetails.AccountDetails.Select(x => x.FinanceContact).FirstOrDefault();
				model.AccountName = response.AgentDetails.AccountDetails.Select(x => x.AccountName).FirstOrDefault();
				model.AccountNo = response.AgentDetails.AccountDetails.Select(x => x.AccountNumber).FirstOrDefault();
				model.BankName = response.AgentDetails.AccountDetails.Select(x => x.BankName).FirstOrDefault();
				model.BankAddr = response.AgentDetails.AccountDetails.Select(x => x.BankAddress).FirstOrDefault();
				model.SortCode = response.AgentDetails.AccountDetails.Select(x => x.SortCode).FirstOrDefault();
				model.IBAN = response.AgentDetails.AccountDetails.Select(x => x.IBAN).FirstOrDefault();
				model.Swift = response.AgentDetails.AccountDetails.Select(x => x.Swift).FirstOrDefault();
				model.FinanceNote = response.AgentDetails.AccountDetails.Select(x => x.FinanceNote).FirstOrDefault();
				#endregion

				#region Payment Terms binding
				List<PaymentTermsDetails> PaymentTermsDetails = new List<PaymentTermsDetails>();
				foreach (var p in response.AgentDetails.PaymentTerms)
				{
					PaymentTermsDetails t = new PaymentTermsDetails();
					t.PaymentTerms_Id = p.PaymentTerms_Id;
					t.Company_Id = p.Company_Id;
					t.From = p.From;
					t.Days = p.Days;
					t.ValueType = p.ValueType;
					t.Value = p.Value;
					t.Currency_Id = p.Currency_Id;
					t.Currency = p.Currency;
					t.VoucherReleased = p.VoucherReleased;
					t.STATUS = p.STATUS;
					t.BusiType = p.BusiType;
					PaymentTermsDetails.Add(t);
				}
				model.PaymentTermsDetails = PaymentTermsDetails;
				#endregion
				#region Tax Reg Details
				List<TaxRegDetails> TaxRegDetails = new List<TaxRegDetails>();
				foreach (var p in responseForTaxReg.TaxRegestrationDetails)
				{
					TaxRegDetails t = new TaxRegDetails();
					t.TaxReg_Id = p.TaxReg_Id;
					t.State = p.State;
					t.Number = p.Number;
					t.TaxStatus = p.TaxStatus;
					t.Type = p.Type;
					TaxRegDetails.Add(t);
				}
				model.TaxRegDetails = TaxRegDetails;
				#endregion

				return PartialView("_CommercialsAndPaymentDetails", model);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		public IActionResult GetMarkupsList()
		{
			var markuplist = masterProviders.GetMarkups(token).Result;
			return Json(new { response = markuplist.OrderBy(x => x.AttributeName).ToList() });
		}

		public IActionResult ViewPaymentTerms(string CompanyId, string PaymentTermsId)
		{
			PaymentTermsDetails model = new PaymentTermsDetails();
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			model.Company_Id = CompanyId;

			#region Get currency list
			SalesProviders objSalesProvider = new SalesProviders(_configuration);
			CurrencyResponse objCurrencyResponse = objSalesProvider.GetCurrencyList(token).Result;
			model.CurrencyList = objCurrencyResponse.CurrencyList.OrderBy(x => x.CurrencyCode).ToList();
			#endregion

			if (response != null && !string.IsNullOrEmpty(PaymentTermsId))
			{
				var paymentTerms = response.AgentDetails.PaymentTerms.Where(x => x.PaymentTerms_Id == PaymentTermsId).FirstOrDefault();
				model.PaymentTerms_Id = paymentTerms.PaymentTerms_Id;
				model.Company_Id = paymentTerms.Company_Id;
				model.From = paymentTerms.From;
				model.Days = paymentTerms.Days;
				model.Value = paymentTerms.Value;
				model.ValueType = paymentTerms.ValueType;
				model.Currency_Id = paymentTerms.Currency_Id;
				model.Currency = paymentTerms.Currency;
				model.VoucherReleased = paymentTerms.VoucherReleased;
				model.STATUS = paymentTerms.STATUS;
				model.BusiType = paymentTerms.BusiType;
			}
			return PartialView("_ViewPaymentTerms", model);
		}

		public IActionResult SavePaymentTerms(PaymentTermsDetails model)
		{
			if (string.IsNullOrEmpty(model.Currency_Id)) model.Currency = "";
			request = new AgentGetReq() { CompanyId = model.Company_Id };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			var msg = ""; var PaymentTermsId = ""; var status = "";
			AgentSetReq req = new AgentSetReq();
			var details = response.AgentDetails.PaymentTerms.Where(x => x.PaymentTerms_Id == model.PaymentTerms_Id).FirstOrDefault();
			var existsrecord = response.AgentDetails.PaymentTerms.Where(x => x.From == model.From && x.Days == model.Days && x.Value == model.Value && x.ValueType == model.ValueType && x.Currency_Id == model.Currency_Id && x.Currency == model.Currency && x.BusiType == model.BusiType).FirstOrDefault();

			if (details != null)
			{
				req.IsNewPaymentTerms = false;
				PaymentTermsId = details.PaymentTerms_Id;
				req.companies.Company_Id = model.Company_Id;

				PaymentTerms b = new PaymentTerms()
				{
					PaymentTerms_Id = model.PaymentTerms_Id,
					Company_Id = model.Company_Id,
					From = model.From,
					Days = model.Days,
					Value = model.Value,
					ValueType = model.ValueType,
					Currency_Id = model.Currency_Id,
					Currency = model.Currency,
					VoucherReleased = model.VoucherReleased,
					STATUS = model.STATUS ?? " ",
					BusiType = model.BusiType,
					Crea_Us = ckUserEmailId
				};
				req.companies.PaymentTerms.Add(b);
				AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
				if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; PaymentTermsId = res.PaymentTermsId; }
				else if (res.ResponseStatus.Status.ToLower() == "failure") { status = "error1"; }
				else status = "error";
			}
			else
			{
				req.IsNewPaymentTerms = true;

				if (existsrecord != null)
				{
					status = "error1";
					msg = "Duplicate record";
				}
				else
				{
					req.companies.Company_Id = model.Company_Id;
					PaymentTerms b = new PaymentTerms()
					{
						PaymentTerms_Id = model.PaymentTerms_Id,
						Company_Id = model.Company_Id,
						From = model.From,
						Days = model.Days,
						Value = model.Value,
						ValueType = model.ValueType,
						Currency_Id = model.Currency_Id,
						Currency = model.Currency,
						VoucherReleased = model.VoucherReleased,
						STATUS = model.STATUS ?? " ",
						BusiType = model.BusiType,
						Crea_Us = ckUserEmailId
					};
					req.companies.PaymentTerms.Add(b);

					AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
					model.PaymentTerms_Id = res.PaymentTermsId;
					if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; PaymentTermsId = res.PaymentDetailsId; }
					else status = "error";
				}
			}

			//Call Bridge service to save Contact data in SQL databse
			ResponseStatus result = new ResponseStatus();
			if (status == "success")
			{
				SetCompanyPaymentTerms_RQ request = new SetCompanyPaymentTerms_RQ();
				request.PaymentTerms_Id = model.PaymentTerms_Id;
				request.User = ckUserEmailId;
				result = agentProviders.SetCompanyPaymentTerms(request, token).Result;

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
			return Json(new { status = status, responseText = msg, paymentTermsId = PaymentTermsId });
		}



		[Authorize]
		[HttpPost]
		public IActionResult UpdateCommercialsAndPayment(CommercialsAndPayment model)
		{
			try
			{
                string EmailId = HttpContext.Request.Cookies["EmailId"] ?? ckUserEmailId;
				string message = ""; string status = "";

				// If id is null, name should be blank for 2nd dropdown
				if (string.IsNullOrEmpty(model.MarkupSchemesGroupsType)) model.GroupsTypeText = "";
				if (string.IsNullOrEmpty(model.MarkupSchemesFITType)) model.FITTypeText = "";
				if (string.IsNullOrEmpty(model.MarkupSchemesSeriesType)) model.SeriesTypeText = "";
				if (string.IsNullOrEmpty(model.MarkupSchemesB2B2BType)) model.B2B2BTypeText = "";

				//If basis is Percentage / Scheme then, value should be null
				if (model.Group == "Percentage / Scheme") model.GroupsValue = null;
				if (model.FIT == "Percentage / Scheme") model.FITValue = null;
				if (model.Series == "Percentage / Scheme") model.SeriesValue = null;
				if (model.B2B2B == "Percentage / Scheme") model.B2B2BValue = null;

				//If basis is Amount then, markup id and value shoould be null
				if (model.Group == "Amount") { model.MarkupSchemesGroupsType = string.Empty; model.GroupsTypeText = ""; }
				if (model.FIT == "Amount") { model.MarkupSchemesFITType = string.Empty; model.FITTypeText = ""; }
				if (model.Series == "Amount") { model.MarkupSchemesSeriesType = string.Empty; model.SeriesTypeText = ""; }
				if (model.B2B2B == "Amount") { model.MarkupSchemesB2B2BType = string.Empty; model.B2B2BTypeText = ""; }

				if (model != null && !string.IsNullOrEmpty(model.Company_Id))
				{
					AgentSetReq req = new AgentSetReq();
					req.EditUser = ckUserEmailId;
					req.companies.Company_Id = model.Company_Id;
					CompanyAccounts accobj = new CompanyAccounts();
					accobj.VoyagerCompany_Id = model.Company_Id;
					accobj.FinanceContact = model.FinanceContactId;
					accobj.AccountName = model.AccountName;
					accobj.AccountNumber = model.AccountNo;
					accobj.BankName = model.BankName;
					accobj.BankAddress = model.BankAddr;
					accobj.SortCode = model.SortCode;
					accobj.IBAN = model.IBAN;
					accobj.Swift = model.Swift;
					accobj.FinanceNote = model.FinanceNote;
					req.companies.AccountDetails.Add(accobj);

					#region markuplist binding
					List<CompanyMarkup> markuplist = new List<CompanyMarkup>();
					if (!string.IsNullOrWhiteSpace(model.Group) || !string.IsNullOrWhiteSpace(model.MarkupSchemesGroupsType) || !string.IsNullOrWhiteSpace(model.GroupsTypeText) || model.GroupsValue != null)
					{
						CompanyMarkup obj = new CompanyMarkup();
						obj.Markup_For = "Groups";
						obj.Markup_Basis = model.Group;
						obj.Markup_Id = model.MarkupSchemesGroupsType;
						obj.Markup = model.GroupsTypeText;
						obj.Markup_Value = model.GroupsValue;
						markuplist.Add(obj);
					}

					if (!string.IsNullOrWhiteSpace(model.FIT) || !string.IsNullOrWhiteSpace(model.MarkupSchemesFITType) || !string.IsNullOrWhiteSpace(model.FITTypeText) || model.FITValue != null)
					{
						CompanyMarkup obj = new CompanyMarkup();
						obj.Markup_For = "FIT";
						obj.Markup_Basis = model.FIT;
						obj.Markup_Id = model.MarkupSchemesFITType;
						obj.Markup = model.FITTypeText;
						obj.Markup_Value = model.FITValue;
						markuplist.Add(obj);
					}

					if (!string.IsNullOrWhiteSpace(model.Series) || !string.IsNullOrWhiteSpace(model.MarkupSchemesSeriesType) || !string.IsNullOrWhiteSpace(model.SeriesTypeText) || model.SeriesValue != null)
					{
						CompanyMarkup obj = new CompanyMarkup();
						obj.Markup_For = "Series";
						obj.Markup_Basis = model.Series;
						obj.Markup_Id = model.MarkupSchemesSeriesType;
						obj.Markup = model.SeriesTypeText;
						obj.Markup_Value = model.SeriesValue;
						markuplist.Add(obj);
					}

					if (!string.IsNullOrWhiteSpace(model.B2B2B) || !string.IsNullOrWhiteSpace(model.MarkupSchemesB2B2BType) || !string.IsNullOrWhiteSpace(model.B2B2BTypeText) || model.B2B2BValue != null)
					{
						CompanyMarkup obj = new CompanyMarkup();
						obj.Markup_For = "B2B2B";
						obj.Markup_Basis = model.B2B2B;
						obj.Markup_Id = model.MarkupSchemesB2B2BType;
						obj.Markup = model.B2B2BTypeText;
						obj.Markup_Value = model.B2B2BValue;
						markuplist.Add(obj);
					}
					req.companies.Markups = markuplist;
					#endregion

					AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;

					var frmStatus = res.ResponseStatus.Status;
					if (frmStatus.ToLower() == "success")
					{
						//the below function will call the SetCompanyAccountsAndMarkup Bridge service and update the CompanyAccount and Markup details into 
						//Company SQL table
						SetCompanyKeyInfo_RQ request = new SetCompanyKeyInfo_RQ();
						request.Company_Id = model.Company_Id;
						request.User = ckUserEmailId;
						ResponseStatus response = agentProviders.SetCompanyAccountsAndMarkup(request, token).Result;
						if (response != null)
						{
							if (!string.IsNullOrEmpty(response.Status) && response.Status == "Success")
							{
								status = "success";
								message = "Details Updated Successfully.";
							}
							else
							{
								status = "error";
								message = response.StatusMessage;
							}
						}
						else
						{
							status = "error";
							message = "Error in updating records.";
						}
					}
					else
					{
						status = "error";
						message = "Error in updating records.";
					}
				}
				else
				{
					status = "error";
					message = "Error in updating records.";
				}

				//if (!(string.IsNullOrEmpty(FrmSuccessMessage)))
				//    TempData["frmsuccess"] = FrmSuccessMessage;

				//if (!(string.IsNullOrEmpty(FrmErrorMessage)))
				//    TempData["frmerror"] = FrmErrorMessage;

				//return RedirectToAction("CommercialsAndPaymentDetails", new { CompanyId = model.Company_Id });

				//return Json(new { status = status, responseText = msg, emergencyContactId = EmergencyContactId });
				return Json(new { message = message, status = status });
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		#endregion

		#region Terms and Conditions
		public IActionResult TermsAndConditionsDetails(string CompanyId)
		{
			try
			{
				TermsAndConditionsDetails model = new TermsAndConditionsDetails();

				request = new AgentGetReq() { CompanyId = CompanyId };
				AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				model.Company_Id = CompanyId;
				//model.CompanyTerms = response.AgentDetails.TermsAndConditions.OrderBy(x=>x.OrderNr).ToList();
				//model.PaymentDetails = response.AgentDetails.PaymentDetails;

				List<ConditionsAndPaymentDetails> TermsAndConditions = new List<ConditionsAndPaymentDetails>();
				foreach (var a in response.AgentDetails.TermsAndConditions)
				{
					ConditionsAndPaymentDetails obj = new ConditionsAndPaymentDetails();
					obj.TermsAndConditions_Id = a.TermsAndConditions_Id;
					obj.OrderNr = a.OrderNr;
					obj.DocumentType = a.DocumentType;
					obj.BusinessType = a.BusinessType;
					obj.Section = a.Section;
					obj.SubSection = a.SubSection;
					obj.TermsDescription = a.TermsDescription;
					TermsAndConditions.Add(obj);
				}
				model.CompanyTerms = TermsAndConditions.OrderBy(x => x.OrderNr).ToList();

				List<ConditionsAndPaymentDetails> PaymentDetails = new List<ConditionsAndPaymentDetails>();
				foreach (var a in response.AgentDetails.PaymentDetails)
				{
					ConditionsAndPaymentDetails obj = new ConditionsAndPaymentDetails();
					obj.PaymentDetails_Id = a.PaymentDetails_Id;
					obj.PaymentDetails = a.Details;
					obj.Currency_Id = a.Currency_Id;
					obj.Currency = a.Currency;
					PaymentDetails.Add(obj);
				}
				model.PaymentDetails = PaymentDetails.OrderBy(x => x.PaymentDetails).ToList();

				//#region dropdown bindings
				//model.DocumentTypeList = response.DefDocumentTypes;
				//model.SubSectionList = response.ProductList;

				//MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
				//objMasterTypeRequest.Property = "QRF Masters";
				//objMasterTypeRequest.Name = "DocumentSections";
				//SalesProviders objSalesProvider = new SalesProviders(_configuration);
				//MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;

				//#region Section list binding
				//List<Attributes> section = new List<Attributes>();
				//if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
				//{
				//    if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "DocumentSections")
				//    {
				//        var result = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.ToList();
				//        foreach (var a in result)
				//        {
				//            Attributes aa = new Attributes();
				//            aa.Attribute_Id = a.AttributeValue_Id;
				//            aa.AttributeName = a.Value;
				//            section.Add(aa);
				//        }
				//    }
				//    model.SectionList = section;
				//}
				//#endregion

				//#region Get currency list

				//CurrencyResponse objCurrencyResponse = objSalesProvider.GetCurrencyList(token).Result;
				//model.CurrencyList = objCurrencyResponse.CurrencyList;
				//#endregion
				//#endregion

				return PartialView("_TermsAndConditionsDetails", model);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		public IActionResult ViewConditions(string CompanyId, string TermsAndConditionsId)
		{
			ConditionsAndPaymentDetails model = new ConditionsAndPaymentDetails();
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			model.Company_Id = CompanyId;

			#region dropdown bindings
			List<Attributes> lstDocTypes = new List<Attributes>();
			lstDocTypes = agentProviders.GetDefDocumentTypes(token).Result;
			if (lstDocTypes != null && lstDocTypes.Count > 0)
				model.DocumentTypeList = lstDocTypes.OrderBy(x => x.AttributeName).ToList();

			List<Attributes> lstProductTypes = new List<Attributes>();
			lstProductTypes = agentProviders.GetProductTypes(token).Result;
			if (lstProductTypes != null && lstProductTypes.Count > 0)
				model.SubSectionList = lstProductTypes.OrderBy(x => x.AttributeName).ToList();

			MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
			objMasterTypeRequest.Property = "QRF Masters";
			objMasterTypeRequest.Name = "DocumentSections";
			SalesProviders objSalesProvider = new SalesProviders(_configuration);
			MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;

			#region Section list binding
			List<Attributes> section = new List<Attributes>();
			if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
			{
				if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "DocumentSections")
				{
					var result = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.ToList();
					foreach (var a in result)
					{
						Attributes aa = new Attributes();
						aa.Attribute_Id = a.AttributeValue_Id;
						aa.AttributeName = a.Value;
						section.Add(aa);
					}
				}
				model.SectionList = section.OrderBy(x => x.AttributeName).ToList();
			}
			#endregion

			//#region Get currency list

			//CurrencyResponse objCurrencyResponse = objSalesProvider.GetCurrencyList(token).Result;
			//model.CurrencyList = objCurrencyResponse.CurrencyList.OrderBy(x=>x.CurrencyCode).ToList();
			//#endregion
			#endregion
			if (response != null && !string.IsNullOrEmpty(TermsAndConditionsId))
			{
				var conditions = response.AgentDetails.TermsAndConditions.Where(x => x.TermsAndConditions_Id == TermsAndConditionsId).FirstOrDefault();
				model.TermsAndConditions_Id = conditions.TermsAndConditions_Id;
				model.OrderNr = conditions.OrderNr;
				model.DocumentType = conditions.DocumentType;
				model.BusinessType = conditions.BusinessType;
				model.Section = conditions.Section;
				model.SubSection = conditions.SubSection;
				model.TermsDescription = conditions.TermsDescription;
			}
			return PartialView("_ViewConditions", model);
		}

		public IActionResult SaveConditions(ConditionsAndPaymentDetails model)
		{
			var status = "";
			request = new AgentGetReq() { CompanyId = model.Company_Id };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			var msg = ""; var TermsAndConditionsId = "";
			AgentSetReq req = new AgentSetReq();
			var details = response.AgentDetails.TermsAndConditions.Where(x => x.TermsAndConditions_Id == model.TermsAndConditions_Id).FirstOrDefault();
			var existsrecord = response.AgentDetails.TermsAndConditions.Where(x => x.OrderNr == model.OrderNr && x.DocumentType == model.DocumentType && x.BusinessType == model.BusinessType && x.Section == model.Section && x.SubSection == model.SubSection && x.TermsDescription.Trim() == model.TermsDescription.Trim()).FirstOrDefault();
			if (details != null)
			{
				req.IsNewTermAndCondition = false;
				TermsAndConditionsId = details.TermsAndConditions_Id;

				req.companies.Company_Id = model.Company_Id;
				CompanyTerms b = new CompanyTerms()
				{
					TermsAndConditions_Id = model.TermsAndConditions_Id,
					Company_Id = model.Company_Id,
					DocumentType = model.DocumentType,
					OrderNr = model.OrderNr,
					BusinessType = model.BusinessType,
					Section = model.Section,
					SubSection = model.SubSection,
					TermsDescription = model.TermsDescription,
					CreateUser = ckUserEmailId,
					EditUser = ckUserEmailId,
					CreateDate = DateTime.Now
				};
				req.companies.TermsAndConditions.Add(b);

				AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
				if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; TermsAndConditionsId = res.TermsAndConditionsId; }
				else if (res.ResponseStatus.Status.ToLower() == "failure") { status = "error1"; }
				else status = "error";
			}
			else
			{
				req.IsNewTermAndCondition = true;
				if (existsrecord != null)
				{
					status = "error1";
					msg = "Duplicate record";
				}
				else
				{
					req.companies.Company_Id = model.Company_Id;
					CompanyTerms b = new CompanyTerms()
					{
						TermsAndConditions_Id = model.TermsAndConditions_Id,
						Company_Id = model.Company_Id,
						DocumentType = model.DocumentType,
						OrderNr = model.OrderNr,
						BusinessType = model.BusinessType,
						Section = model.Section,
						SubSection = model.SubSection,
						TermsDescription = model.TermsDescription,
						CreateUser = ckUserEmailId,
						EditUser = ckUserEmailId,
						CreateDate = DateTime.Now
					};
					req.companies.TermsAndConditions.Add(b);

					AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
					model.TermsAndConditions_Id = res.TermsAndConditionsId;
					if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; TermsAndConditionsId = res.TermsAndConditionsId; }
					else status = "error";
				}
			}

			if (status == "success")
			{
				//the below function will call the SetTermsAndCondition Bridge service and update the TermsAndCondition into 
				//TermsAndConditions SQL table
				TermsAndConditionsSetReq request = new TermsAndConditionsSetReq();
				request.Company_Id = model.Company_Id;
				request.TermsAndConditions_Id = model.TermsAndConditions_Id;
				request.User = ckUserEmailId;
				ResponseStatus res = agentProviders.SetTermsAndCondition(request, token).Result;
				if (res != null)
				{
					if (!string.IsNullOrEmpty(res.Status) && res.Status == "Success")
					{
						status = "success";
						msg = "Details Updated Successfully.";
					}
					else
					{
						status = "error";
						msg = res.StatusMessage;
					}
				}
				else
				{
					status = "error";
					msg = "Error in updating records.";
				}
			}
			return Json(new { status = status, responseText = msg, termscondId = TermsAndConditionsId });
		}

		public IActionResult RemoveCondition(string CompanyId, string TermsAndConditionId)
		{
			var status = ""; var msg = ""; var termscondId = "";
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;

			AgentSetReq req = new AgentSetReq();
			req.companies.Company_Id = CompanyId;
			req.IsRemoveCondition = true;
			req.EditUser = ckUserEmailId;
			CompanyTerms b = new CompanyTerms()
			{
				Company_Id = CompanyId,
				TermsAndConditions_Id = TermsAndConditionId
			};
			req.companies.TermsAndConditions.Add(b);
			AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
			if (res.ResponseStatus.Status == "success") status = "success";
			return Json(status);
		}

		public IActionResult ViewPaymentDetails(string CompanyId, string PaymentDetailsId)
		{
			ConditionsAndPaymentDetails model = new ConditionsAndPaymentDetails();
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			model.Company_Id = CompanyId;

			#region Get currency list
			SalesProviders objSalesProvider = new SalesProviders(_configuration);
			CurrencyResponse objCurrencyResponse = objSalesProvider.GetCurrencyList(token).Result;
			model.CurrencyList = objCurrencyResponse.CurrencyList.OrderBy(x => x.CurrencyCode).ToList();
			#endregion

			if (response != null && !string.IsNullOrEmpty(PaymentDetailsId))
			{
				var details = response.AgentDetails.PaymentDetails.Where(x => x.PaymentDetails_Id == PaymentDetailsId).FirstOrDefault();
				model.PaymentDetails_Id = details.PaymentDetails_Id;
				model.PaymentDetails = details.Details;
				model.Currency_Id = details.Currency_Id;
				model.Currency = details.Currency;
			}
			return PartialView("_ViewPaymentDetails", model);
		}

		public IActionResult SavePaymentDetails(ConditionsAndPaymentDetails model)
		{
			var status = "";
			request = new AgentGetReq() { CompanyId = model.Company_Id };
			AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
			var msg = ""; var PaymentDetailsId = "";
			AgentSetReq req = new AgentSetReq();
			var details = response.AgentDetails.PaymentDetails.Where(x => x.PaymentDetails_Id == model.PaymentDetails_Id).FirstOrDefault();
			var existsrecord = response.AgentDetails.PaymentDetails.Where(x => x.Currency_Id == model.Currency_Id && x.Currency == model.Currency && x.Details == model.PaymentDetails).FirstOrDefault();
			if (details != null)
			{
				req.IsNewPaymentDetail = false;
				PaymentDetailsId = details.PaymentDetails_Id;

				req.companies.Company_Id = model.Company_Id;
				PaymentDetails b = new PaymentDetails()
				{
					PaymentDetails_Id = model.PaymentDetails_Id,
					Company_Id = model.Company_Id,
					Currency_Id = model.Currency_Id,
					Currency = model.Currency,
					Details = model.PaymentDetails,
					CreateUser = ckUserEmailId,
					EditUser = ckUserEmailId
				};
				req.companies.PaymentDetails.Add(b);

				AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
				if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; PaymentDetailsId = res.PaymentDetailsId; }
				else if (res.ResponseStatus.Status.ToLower() == "failure") { status = "error1"; }
				else status = "error";
			}
			else
			{
				req.IsNewPaymentDetail = true;
				if (existsrecord != null)
				{
					status = "error1";
					msg = "Duplicate record";
				}
				else
				{
					req.companies.Company_Id = model.Company_Id;
					PaymentDetails b = new PaymentDetails()
					{
						PaymentDetails_Id = model.PaymentDetails_Id,
						Company_Id = model.Company_Id,
						Currency_Id = model.Currency_Id,
						Currency = model.Currency,
						Details = model.PaymentDetails,
						CreateUser = ckUserEmailId,
						EditUser = ckUserEmailId
					};
					req.companies.PaymentDetails.Add(b);

					AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
					model.PaymentDetails_Id = res.PaymentDetailsId;
					if (res.ResponseStatus.Status.ToLower() == "success") { status = "success"; PaymentDetailsId = res.PaymentDetailsId; }
					else status = "error";
				}
			}

			if (status == "success")
			{
				//the below function will call the SetCompanyPayment Bridge service and update the Company Payment details into 
				//CompanyAccountDetail SQL table
				CompanyPaymentSetReq request = new CompanyPaymentSetReq();
				request.Company_Id = model.Company_Id;
				request.CompanyAccountDetail_Id = model.PaymentDetails_Id;
				request.User = ckUserEmailId;
				ResponseStatus responsecompanypay = agentProviders.SetCompanyPayment(request, token).Result;
				if (responsecompanypay != null)
				{
					if (!string.IsNullOrEmpty(responsecompanypay.Status) && responsecompanypay.Status == "Success")
					{
						status = "success";
						msg = "Details Updated Successfully.";
					}
					else
					{
						status = "error";
						msg = responsecompanypay.StatusMessage;
					}
				}
				else
				{
					status = "error";
					msg = "Error in updating records.";
				}
			}
			return Json(new { status = status, responseText = msg, paymentDtlsId = PaymentDetailsId });
		}

		public IActionResult RemovePaymentDetail(string CompanyId, string PaymentDetailsId)
		{
			var status = "";
			request = new AgentGetReq();
			response = new AgentGetRes();
			request.CompanyId = CompanyId;
			response = agentProviders.GetAgentDetailedInfo(request, token).Result;

			AgentSetReq req = new AgentSetReq();
			req.EditUser = ckUserEmailId;
			req.companies.Company_Id = CompanyId;
			req.IsRemovePaymentDetail = true;
			PaymentDetails b = new PaymentDetails()
			{
				Company_Id = CompanyId,
				PaymentDetails_Id = PaymentDetailsId
			};
			req.companies.PaymentDetails.Add(b);
			AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;
			if (res.ResponseStatus.Status == "success") status = "success";
			return Json(status);
		}
		#endregion

		#region System Options
		public IActionResult SystemOptionsDetails(string CompanyId)
		{
			try
			{
				#region System Options
				AgentDetails model = new AgentDetails();
				request = new AgentGetReq() { CompanyId = CompanyId };
				AgentGetRes response = agentProviders.GetAgentDetailedInfo(request, token).Result;
				model.CompanyId = CompanyId;
				model.ContactBy = response.AgentDetails.ContactBy;
				model.ISUSER = response.AgentDetails.ISUSER == true ? true : false;
				model.CreateDate = Convert.ToDateTime(response.AgentDetails.CreateDate).ToString("dd MMM yy");
				model.CreateUser = response.AgentDetails.CreateUser;
				model.EditUser = response.AgentDetails.EditUser;
				model.EditDate = Convert.ToDateTime(response.AgentDetails.EditDate).ToString("dd MMM yy");
                #endregion

                var userRoles = HttpContext.Request.Cookies["UserRoles"] ?? string.Join(",", UserRoles);
                if (userRoles != null && userRoles.Contains("Administrator"))
				{
					ViewBag.IsAdmin = true;
				}

				return PartialView("_SystemOptionsDetails", model);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		public IActionResult ViewSupplierMapping(string Id, string PageName, int Index)
		{
			try
			{
				AgentDetails model = new AgentDetails();
				model.CompanyId = Id;
				model.PageName = PageName;
				model.Index = Index;

				#region Dropdown binding
				var appList = supplierProviders.GetApplications(token).Result;
				if (appList != null && appList.Count > 0)
					model.ApplicationList = appList.Select(x => new Attributes { Attribute_Id = x.Application_Id, AttributeName = x.Application_Name }).ToList();
				#endregion

				#region Supplier Mapping
				SupplierGetReq req = new SupplierGetReq() { Id = Id, PageName = PageName };
				if (!string.IsNullOrWhiteSpace(Id))
				{
					List<Mappings> mappings = supplierProviders.GetSupplierMappings(req, token).Result ?? new List<Mappings>();
					model.SupplierMappingList = mappings.Select(x => new SupplierMappings { Application_Id = x.Application_Id, Application = x.Application, PartnerEntityCode = x.PartnerEntityCode, PartnerEntityName = x.PartnerEntityName, CreateDate = x.CreateDate, CreateUser = x.CreateUser, IsDeleted = false }).ToList();
					model.SupplierMappingList.Add(new SupplierMappings());
				}
				#endregion

				return PartialView("_ViewSupplierMapping", model);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[HttpPost]
		public IActionResult SaveSupplierMapping(AgentDetails model)
		{
			try
			{
				SupplierSetReq request1 = new SupplierSetReq();
				SupplierSetRes response1 = new SupplierSetRes();

				//Set details				
				List<Mappings> lst = new List<Mappings>();
				request1.Id = model.CompanyId;
				request1.PageName = model.PageName;
				foreach (var m in model.SupplierMappingList.Where(x => x.IsDeleted == false))
				{
					Mappings obj = new Mappings();
					obj.Application_Id = m.Application_Id;
					obj.Application = m.Application;
					obj.PartnerEntityCode = m.PartnerEntityCode;
					obj.PartnerEntityName = m.PartnerEntityName;
					obj.CreateUser = m.CreateUser ?? ckUserEmailId;
					obj.CreateDate = (m.CreateDate == DateTime.MinValue || m.CreateDate == null) ? DateTime.Now : m.CreateDate;
					lst.Add(obj);
				}
				request1.SupplierMappings = lst;
				response1 = supplierProviders.SetSupplierMapping(request1, token).Result;

				return Json(new { responseText = response1.ResponseStatus.StatusMessage, status = response1.ResponseStatus.Status, Id = request1.Id });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult UpdateSystemOptionsDetails(string CompanyId, string ContactBy, bool IsSystemUser)
		{
			try
			{
				string status = "";

				if (!string.IsNullOrEmpty(CompanyId))
				{
					AgentSetReq req = new AgentSetReq();
					req.IsSystemUser = true;
					req.companies.Company_Id = CompanyId;
					req.companies.ContactBy = ContactBy;
					req.companies.ISUSER = IsSystemUser;
					req.companies.EditUser = ckUserEmailId;
					AgentSetRes res = agentProviders.SetAgentDetailedInfo(req, token).Result;

					var frmStatus = res.ResponseStatus.Status;
					if (frmStatus.ToLower() == "success") status = "success"; else status = "error";
				}
				else
					status = "error";

				return Json(new { status = status });
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		#endregion

		#region Agent Targets
		public IActionResult GetCompanyTargets(string CompanyId, string ContactId, string ActionType)
		{
			request = new AgentGetReq() { CompanyId = CompanyId, ContactId = ContactId, ActionType = ActionType };
			try
			{
				TargetGetRes response = agentProviders.GetCompanyTargets(request, token).Result;
				response.TargetList = response.TargetList?.OrderBy(a => a.Month).ToList();
				return PartialView("_Targets", response);
			}
			catch (Exception ex)
			{
				return PartialView("_Targets", response);
			}
		}

		public IActionResult SetCompanyTargets(TargetCompany targetCompany)
		{
			AgentSetRes res = new AgentSetRes() { ResponseStatus = new ResponseStatus() { Status = "Error" }, };
			try
			{
				AgentSetReq request = new AgentSetReq();
				if (!string.IsNullOrEmpty(targetCompany.CompanyId))
				{
					bool IsCompanyTarget = false;
					bool IsContactTarget = false;
					bool IsNewCompanyTarget = false;
					bool IsNewContactTarget = false;
					if (!string.IsNullOrEmpty(targetCompany.ContactId) && !string.IsNullOrEmpty(targetCompany.Targets.TargetId))
					{
						IsContactTarget = true;
					}
					else if (string.IsNullOrEmpty(targetCompany.ContactId) && !string.IsNullOrEmpty(targetCompany.Targets.TargetId))
					{
						IsCompanyTarget = true;
					}
					else if (!string.IsNullOrEmpty(targetCompany.ContactId))
					{
						IsNewContactTarget = true;
					}
					else if (string.IsNullOrEmpty(targetCompany.ContactId))
					{
						IsNewCompanyTarget = true;
					}

					request = new AgentSetReq()
					{
						companies = new mCompanies()
						{
							Company_Id = targetCompany.CompanyId,
							ContactDetails = new List<CompanyContacts>() { new CompanyContacts() { Contact_Id = targetCompany.ContactId, Targets = new List<Targets>() { targetCompany.Targets } } },
							Targets = new List<Targets>() { targetCompany.Targets }
						},
						IsCompanyTarget = IsCompanyTarget,
						IsContactTarget = IsContactTarget,
						IsNewCompanyTarget = IsNewCompanyTarget,
						IsNewContactTarget = IsNewContactTarget,
						EditUser = ckUserEmailId
					};

					res = agentProviders.SetAgentDetailedInfo(request, token).Result;
				}
				return Json(new
				{
					status = res.ResponseStatus.Status,
					msg = res.ResponseStatus.ErrorMessage,
					targets = res.Targets
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					status = "Error",
					msg = ex.Message,
					targets = new Targets()
				});
			}
		}
		#endregion

		#region 3rd party Search Agent Details

		public IActionResult GetPartnerAgentDetails(AgentThirdPartyGetReq model)
		{
			AgentThirdPartyGetRes response = new AgentThirdPartyGetRes();

			response = agentProviders.GetPartnerAgentDetailedInfo(model, token).Result;

			if (!string.IsNullOrEmpty(response.CompanyId))
			{
				return RedirectToAction("AgentDetail", new { CompanyId = response.CompanyId });
			}

			return View("IntegrationError");

		}

		public IActionResult GetPartnerAgentContactDetails(AgentThirdPartyGetReq model)
		{
			AgentThirdPartyGetRes response = new AgentThirdPartyGetRes();

			response = agentProviders.GetPartnerAgentContactDetails(model, token).Result;

			if (!string.IsNullOrEmpty(response.CompanyId))
			{
				return RedirectToAction("AgentDetail", new { CompanyId = response.CompanyId, ContactId = response.ContactId });
			}

			return View("IntegrationError");

		}
		#endregion
	}
}