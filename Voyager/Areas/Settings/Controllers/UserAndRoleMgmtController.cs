using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VGER_WAPI_CLASSES;
using Voyager.App.Providers;
using Voyager.App.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using VGER_WAPI_CLASSES.User;
using Voyager.App.ViewModels.Settings;
using VGER_WAPI_CLASSES.MIS;

namespace Voyager.Controllers
{
	[Authorize]
	public class UserAndRoleMgmtController : VoyagerController
	{
		private readonly IConfiguration _configuration;
		private IHostingEnvironment _env;
		LoginProviders loginProviders;
		AgentProviders agentProviders;
		UserAndRoleMgmtProviders userAndRoleMgmtProviders;
        SalesProviders objSalesProvider;
        MISProviders objMISProviders;

        public UserAndRoleMgmtController(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
		{
			_env = env;
			_configuration = configuration;
			loginProviders = new LoginProviders(_configuration);
			agentProviders = new AgentProviders(_configuration);
			userAndRoleMgmtProviders = new UserAndRoleMgmtProviders(_configuration);
            objSalesProvider = new SalesProviders(_configuration);
            objMISProviders = new MISProviders(_configuration);
        }

		public IActionResult ViewSalesPipelineRoles(string tabName)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(tabName))
				{
					ViewBag.tabName = tabName;
				}

				SalesPipelineViewModel model = new SalesPipelineViewModel();

				#region Destination dropdown binding
				MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
				objMasterTypeRequest.Property = "QRF Masters";
				objMasterTypeRequest.Name = "QRF Destination";
				SalesProviders objSalesProvider = new SalesProviders(_configuration);
				MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;

				model.DestinationList = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.Select(a => new AttributeValues
				{
					AttributeValue_Id = a.AttributeValue_Id,
					CityName = a.Value.Contains("|") ? a.Value.Split("|")[0].Trim() : "",
					Value = a.Value.Contains("|") ? a.Value.Split("|")[1].Trim() : a.Value
				}).ToList();
				#endregion

				#region Role dropdown binding
				UserByRoleGetRes response = new UserByRoleGetRes();
				UserByRoleGetReq request = new UserByRoleGetReq();
				request.RoleName = new List<string>() { "Costing Officer", "Product Accountant" };
				response = loginProviders.GetRoleIdByRoleName(request, token).Result;
				model.RoleList = response.Users.Select(x => new AttributeValues { AttributeValue_Id = x.UserRoleId, Value = x.UserRole }).ToList();
				#endregion

				#region Sales Office dropdown binding
				CompanyOfficerGetRes res = new CompanyOfficerGetRes();
				res = agentProviders.GetSalesOfficesOfSystemCompany(token).Result;
				model.SalesOfficeList = res.Branches.Select(x => new AttributeValues { AttributeValue_Id = x.Company_Id, Value = x.Company_Name }).ToList();
				#endregion

				#region Destination list binding
				SettingsGetReq request1 = new SettingsGetReq();
				SettingsGetRes response1 = new SettingsGetRes();
				request1.Type = "Destination";
				request1.LoggedInUserContact_Id = ckUserContactId;
				response1 = userAndRoleMgmtProviders.GetSalesPipelineRoles(request1, token).Result;
				model.RolesByDestinationList = response1.Values.ToList();
				model.RolesByDestinationList.Add(new Values());
				#endregion

				#region Sales office list binding
				SettingsGetReq request2 = new SettingsGetReq();
				SettingsGetRes response2 = new SettingsGetRes();
				request2.Type = "SalesOffice";
				request2.LoggedInUserContact_Id = ckUserContactId;
				response2 = userAndRoleMgmtProviders.GetSalesPipelineRoles(request2, token).Result;
				model.RolesBySalesOfficeList = response2.Values.ToList();
				model.RolesBySalesOfficeList.Add(new Values());
				#endregion

				return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewSalesPipelineRoles.cshtml", model);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public JsonResult GetUsersByRole(string selectedRoleName)
		{
			try
			{
				UserByRoleGetRes response = new UserByRoleGetRes();
				UserByRoleGetReq request = new UserByRoleGetReq();
				request.RoleName = new List<string>() { selectedRoleName };
				response = loginProviders.GetUserDetailsByRole(request, token).Result;
				var UsersList = response.Users.Where(x => !string.IsNullOrWhiteSpace(x.FirstName) && !string.IsNullOrWhiteSpace(x.LastName)).Select(x => new AttributeValues { AttributeValue_Id = x.UserId, Value = x.FirstName + " " + x.LastName }).Distinct().ToList();

				return Json(new { UsersList = UsersList });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[HttpPost]
		public IActionResult SaveRoleByDestination(SalesPipelineViewModel model)
		{
			try
			{
				bool IsDuplicate = false;
				SettingsGetReq request1 = new SettingsGetReq();
				SettingsGetRes response1 = new SettingsGetRes();
				SettingsSetReq request = new SettingsSetReq();
				SettingsSetRes response = new SettingsSetRes();

				//Get and check for duplicate				
				request1.Type = "Destination";
				request1.LoggedInUserContact_Id = ckUserContactId;
				response1 = userAndRoleMgmtProviders.GetSalesPipelineRoles(request1, token).Result;

				//Self check for duplicate
				if (model.RolesByDestinationList.Where(x => string.IsNullOrWhiteSpace(x.CreateUser)).Count() > 1)
				{
					var newlist = model.RolesByDestinationList.Where(x => string.IsNullOrWhiteSpace(x.CreateUser)).ToList();
					for (int i = 0; i < newlist.Count(); i++)
					{
						for (int j = i + 1; j < newlist.Count(); j++)
						{
							if (newlist[i].TypeId == newlist[j].TypeId && newlist[i].RoleId == newlist[j].RoleId && newlist[i].UserId == newlist[j].UserId && newlist[i].Status == newlist[j].Status)
							{
								IsDuplicate = true;
								response.ResponseStatus.Status = "Failure";
								response.ResponseStatus.StatusMessage = "Duplicate Record";
								break;
							}
						}
					}
				}

				//db check for duplicate if self not having any duplicate values
				if (IsDuplicate == false)
				{
					foreach (var a in model.RolesByDestinationList.Where(x => string.IsNullOrWhiteSpace(x.CreateUser)))
					{

						var dup1 = response1.Values.Where(x => x.TypeId == a.TypeId && x.RoleId == a.RoleId && x.UserId == a.UserId && a.Status == a.Status).FirstOrDefault();
						if (dup1 != null)
						{
							IsDuplicate = true;
							response.ResponseStatus.Status = "Failure";
							response.ResponseStatus.StatusMessage = "Duplicate Record";
							break;
						}
					}
				}

				if (IsDuplicate == false)
				{
					//Set details				
					List<Values> lst = new List<Values>();
					request.LoggedInUserContact_Id = ckUserContactId;
					request.Type = "Destination";
					foreach (var m in model.RolesByDestinationList)
					{
						Values obj = new Values();
						obj.TypeId = m.TypeId;
						obj.TypeName = m.TypeName;
						obj.RoleId = m.RoleId;
						obj.RoleName = m.RoleName;
						obj.UserId = m.UserId;
						obj.UserName = m.UserName;
						obj.Status = m.Status ?? "";
						if (!string.IsNullOrWhiteSpace(obj.Status))
						{
							obj.EditUser = ckUserEmailId;
							obj.EditDate = DateTime.Now;
							obj.CreateDate = m.CreateDate;
							obj.CreateUser = m.CreateUser;
						}
						else
						{
							obj.CreateUser = m.CreateUser ?? ckUserEmailId;
							obj.CreateDate = m.CreateDate == DateTime.MinValue ? DateTime.Now : m.CreateDate;
						}
						lst.Add(obj);
					}
					request.Values = lst;
					response = userAndRoleMgmtProviders.SetSalesPipelineRoles(request, token).Result;
				}

				return Json(new { responseText = response.ResponseStatus.StatusMessage, status = response.ResponseStatus.Status });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[HttpPost]
		public IActionResult SaveRoleBySalesOffice(SalesPipelineViewModel model)
		{
			try
			{
				bool IsDuplicate = false;
				SettingsGetReq request1 = new SettingsGetReq();
				SettingsGetRes response1 = new SettingsGetRes();
				SettingsSetReq request = new SettingsSetReq();
				SettingsSetRes response = new SettingsSetRes();

				//Get and check for duplicate				
				request1.Type = "SalesOffice";
				request1.LoggedInUserContact_Id = ckUserContactId;
				response1 = userAndRoleMgmtProviders.GetSalesPipelineRoles(request1, token).Result;

				//Self check for duplicate
				if (model.RolesBySalesOfficeList.Where(x => string.IsNullOrWhiteSpace(x.CreateUser)).Count() > 1)
				{
					var newlist = model.RolesBySalesOfficeList.Where(x => string.IsNullOrWhiteSpace(x.CreateUser)).ToList();
					for (int i = 0; i < newlist.Count(); i++)
					{
						for (int j = i + 1; j < newlist.Count(); j++)
						{
							if (newlist[i].TypeId == newlist[j].TypeId && newlist[i].RoleId == newlist[j].RoleId && newlist[i].UserId == newlist[j].UserId && newlist[i].Status == newlist[j].Status)
							{
								IsDuplicate = true;
								response.ResponseStatus.Status = "Failure";
								response.ResponseStatus.StatusMessage = "Duplicate Record";
								break;
							}
						}
					}
				}

				//db check for duplicate if self not having any duplicate values
				if (IsDuplicate == false)
				{
					foreach (var a in model.RolesBySalesOfficeList.Where(x => string.IsNullOrWhiteSpace(x.CreateUser)))
					{
						var dup1 = response1.Values.Where(x => x.TypeId == a.TypeId && x.RoleId == a.RoleId && x.UserId == a.UserId && a.Status == a.Status).FirstOrDefault();
						if (dup1 != null)
						{
							IsDuplicate = true;
							response.ResponseStatus.Status = "Failure";
							response.ResponseStatus.StatusMessage = "Duplicate Record";
							break;
						}
					}
				}

				if (IsDuplicate == false)
				{
					//Set details				
					List<Values> lst = new List<Values>();
					request.LoggedInUserContact_Id = ckUserContactId;
					request.Type = "SalesOffice";
					foreach (var m in model.RolesBySalesOfficeList)
					{
						Values obj = new Values();
						obj.TypeId = m.TypeId;
						obj.TypeName = m.TypeName;
						obj.RoleId = m.RoleId;
						obj.RoleName = m.RoleName;
						obj.UserId = m.UserId;
						obj.UserName = m.UserName;
						obj.Status = m.Status ?? "";
						if (!string.IsNullOrWhiteSpace(obj.Status))
						{
							obj.EditUser = ckUserEmailId;
							obj.EditDate = DateTime.Now;
							obj.CreateDate = m.CreateDate;
							obj.CreateUser = m.CreateUser;
						}
						else
						{
							obj.CreateUser = m.CreateUser ?? ckUserEmailId;
							obj.CreateDate = m.CreateDate == DateTime.MinValue ? DateTime.Now : m.CreateDate;
						}
						lst.Add(obj);
					}
					request.Values = lst;
					response = userAndRoleMgmtProviders.SetSalesPipelineRoles(request, token).Result;
				}

				return Json(new { responseText = response.ResponseStatus.StatusMessage, status = response.ResponseStatus.Status });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

        #region Integration Credentials

        public IActionResult ViewIntegrationCredentials(IntegartionCredentailsViewModel model)
        {
            CompanyOfficerGetReq officerGetReq = new CompanyOfficerGetReq();
            Integration_Search_Request request = new Integration_Search_Request();
            Integration_Search_Response response = new Integration_Search_Response();
            officerGetReq.CompanyId = ckUserCompanyId;

            var officerGetRes = agentProviders.GetCompanyContacts(officerGetReq, token).Result;
            model.ContactPersonList = officerGetRes.ContactDetails.Where(a=>a.Systemuser_id != null).ToList();

            model.ApplicationList = userAndRoleMgmtProviders.GetApplicationAttributes(request, token).Result.Application_DataList;

            model.ContactPersonList.ForEach(a => a.FIRSTNAME = a.CommonTitle + " " + a.FIRSTNAME + " " + a.LastNAME);

            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewIntegrationCredentials.cshtml", model);
        }

        public IActionResult ViewIntegrationCredentialsList(IntegartionCredentailsViewModel model)
        {
            Integration_Search_Request request = new Integration_Search_Request();
            Integration_Search_Response response = new Integration_Search_Response();

            request.ckUserCompanyId = ckUserCompanyId;
            request.Application_Id = model.Application_Id;
            request.UserId = model.ContactPersonID;
            request.CreatedUser = ckUserEmailId;

            if (model.isSearch)
            {
                response = userAndRoleMgmtProviders.GetIntegrationCredentials(request, token).Result;
                model.Integration_Search_DataList = response.IntegrationSearchDataList;
            }

            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewIntegrationCredentialsList.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveIntegrationCredentials(IntegartionCredentailsViewModel model)
        {
            Integration_Search_Request request = new Integration_Search_Request();
            Integration_Search_Response response = new Integration_Search_Response();
            request.ckUserCompanyId = ckUserCompanyId;
            request.Application_Id = model.Application_Id;
            request.UserId = model.ContactPersonID;
            request.CreatedUser = ckUserEmailId;

            var retValue = userAndRoleMgmtProviders.CheckCredentialsExit(request, token).Result;
            if (retValue)
            {
                response.ResponseStatus.Status = "Failure";
                response.ResponseStatus.StatusMessage = "Key already generated.";
                return Json(new { responseText = response.ResponseStatus.StatusMessage, status = response.ResponseStatus.Status });
            }
            else
            {
                response = userAndRoleMgmtProviders.SaveIntegrationCredentials(request, token).Result;
                response = userAndRoleMgmtProviders.GetIntegrationCredentials(request, token).Result;
                model.Integration_Search_DataList = response.IntegrationSearchDataList;
            }

            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewIntegrationCredentialsList.cshtml", model);

        }

        [HttpPost]
        public IActionResult DeleteIntegrationCredentials(IntegartionCredentailsViewModel model)//(string VoyagerUser_Id, string Application_Id)
        {
            Integration_Search_Request request = new Integration_Search_Request();
            Integration_Search_Response response = new Integration_Search_Response();
            request.ckUserCompanyId = ckUserCompanyId;
            request.Application_Id = model.Application_Id;
            request.UserId = model.ContactPersonID;
            request.CreatedUser = ckUserEmailId;

            try
            {
                response = userAndRoleMgmtProviders.DeleteIntegrationCredentials(request, token).Result;

                model = new IntegartionCredentailsViewModel();
                request.Application_Id = model.Application_Id;
                request.UserId = model.ContactPersonID;
                response = userAndRoleMgmtProviders.GetIntegrationCredentials(request, token).Result;
                model.Integration_Search_DataList = response.IntegrationSearchDataList;

                //model.ResponseStatus.Status = "Success";
                //model.ResponseStatus.ErrorMessage = "Integration Credentials deleted successfully.";
            }
            catch (Exception ex)
            {
                response.ResponseStatus.Status = "Error";
                response.ResponseStatus.StatusMessage = "Error occurred while deleting record";
                return Json(new { responseText = response.ResponseStatus.StatusMessage, status = response.ResponseStatus.Status });
            }            

            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewIntegrationCredentialsList.cshtml", model);

        }

        [HttpPost]
        public FileResult ExportToExcel(IntegartionCredentailsViewModel model)
        {
            DataTable dt = new DataTable("IntegrationCredentials");
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("Platform"),
                                            new DataColumn("Existing Users"),
                                            new DataColumn("Key"),
                                  new DataColumn("User")});

            Integration_Search_Request request = new Integration_Search_Request();
            Integration_Search_Response response = new Integration_Search_Response();

            request.ckUserCompanyId = ckUserCompanyId;
            request.Application_Id = model.Application_Id;
            request.UserId = model.ContactPersonID;
            request.CreatedUser = ckUserEmailId;
            request.IsExport = true;

            response = userAndRoleMgmtProviders.GetIntegrationCredentials(request, token).Result;
            model.Integration_Search_DataList = response.IntegrationSearchDataList;

            foreach (var item in model.Integration_Search_DataList)
            {
                dt.Rows.Add(item.Application_Name, item.UserName, item.Keys, item.UserKey);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "IntegrationCredentials.xlsx");
                }
            }
        }

        #endregion

        #region Out Bound Integration Credentials
        [HttpGet]
        public IActionResult ViewOutBoundIntegrationCredentials(OutBoundIntegartionCredentailsViewModel model)
        {
            Integration_Search_Request request = new Integration_Search_Request();
            model.ApplicationList = userAndRoleMgmtProviders.GetApplicationAttributes(request, token).Result.Application_DataList;


            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewOutBoundIntegrationCredentials.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveOutBoundIntegrationCredentials(OutBoundIntegartionCredentailsViewModel model)
        {
            OutBoundIntegrationCredentialsReq request = new OutBoundIntegrationCredentialsReq();
            OutBoundIntegrationCredentialsRes response = new OutBoundIntegrationCredentialsRes();
            request.Application_Id = model.Application_Id;
            request.ConfigId = model.ConfigId;
            request.Key = model.Key;
            request.Value = model.Value;
            request.CreatedUser = ckUserEmailId;
            request.EditUser = ckUserEmailId;

            var retValue = userAndRoleMgmtProviders.CheckOutBoundConfigExit(request, token).Result;
            if (retValue)
            {
                response.ResponseStatus.Status = "Failure";
                response.ResponseStatus.ErrorMessage = "Record already exists.";
                return Json(new { responseText = response.ResponseStatus.ErrorMessage, status = response.ResponseStatus.Status });
            }
            else
            {
                response = userAndRoleMgmtProviders.SaveOutBoundIntegrationCredentials(request, token).Result;
            }

            return Json(new { responseText = response.ResponseStatus.ErrorMessage, status = response.ResponseStatus.Status });

        }

        [HttpPost]
        public IActionResult ViewOutBoundIntegrationCredentialsList(OutBoundIntegartionCredentailsViewModel model)
        {
            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewOutBoundIntegrationCredentialsList.cshtml", model);
        }

        [HttpGet]
        public IActionResult GetOutBoundIntegrationCredentialsList(OutBoundIntegartionCredentailsViewModel model, int draw, int start, int length)
        {
            OutBoundIntegrationCredentialsReq request = new OutBoundIntegrationCredentialsReq();
            OutBoundIntegrationCredentialsRes response = new OutBoundIntegrationCredentialsRes();

            request.CreatedUser = ckUserEmailId;
            request.Application_Id = model.Application_Id;
            request.Key = model.Key;
            request.Value = model.Value;
            request.Start = start;
            if (length == 0)
                length = 10;
            request.Length = length;


            response = userAndRoleMgmtProviders.GetOutBoundIntegrationCredentialsList(request, token).Result;

            return Json(new
            {
                draw = draw,
                recordsTotal = response.TotalCount,
                recordsFiltered = response.TotalCount,
                data = response.OutBoundIntegrationSearchDataList
            });
        }

        [HttpPost]
        public IActionResult DeleteOutBoundIntegrationCredentials(OutBoundIntegartionCredentailsViewModel model)
        {
            OutBoundIntegrationCredentialsReq request = new OutBoundIntegrationCredentialsReq();
            OutBoundIntegrationCredentialsRes response = new OutBoundIntegrationCredentialsRes();
            request.Application_Id = model.Application_Id;
            request.ConfigId = model.ConfigId;
            request.Key = model.Key;
            request.Value = model.Value;
            request.CreatedUser = ckUserEmailId;
            request.EditUser = ckUserEmailId;

            response = userAndRoleMgmtProviders.DeleteOutBoundIntegrationCredentials(request, token).Result;

            return Json(response.ResponseStatus);
        }

        #endregion

        #region Integration Application Mapping Data

        [HttpGet]
        public IActionResult ViewIntegrationApplicationMapping(IntegrationApplicationMappingViewModel model)
        {
            model.ApplicationList = userAndRoleMgmtProviders.GetApplicationAttributes(null, token).Result.Application_DataList;
            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewIntegrationApplicationMapping.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveIntegrationApplicationMappingInfo(IntegrationApplicationMappingViewModel model)
        {
            IntegrationMappingDataReq request = new IntegrationMappingDataReq();
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();
            request.EditUser = request.CreateUser = ckUserEmailId;
            request.Application = model.Application;
            request.ApplicationName = model.ApplicationName;
            request.Type = model.Type;
            request.Entity = model.Entity;
            request.IntegrationApplicationMapping_Id = model.IntegrationApplicationMapping_Id;

            bool isExists = userAndRoleMgmtProviders.CheckApplicationMappingExists(request, token).Result;

            if (isExists)
            {
                response.ResponseStatus.Status = "Failure";
                response.ResponseStatus.ErrorMessage = "Record already exists.";
                return Json(new { responseText = response.ResponseStatus.ErrorMessage, status = response.ResponseStatus.Status });
            }
            else
            {
                response = userAndRoleMgmtProviders.SaveIntegrationApplicationMappingInfo(request, token).Result;
            }

            return Json(new { responseText = response.ResponseStatus.ErrorMessage, status = response.ResponseStatus.Status });
        }

        [HttpGet]
        public IActionResult ViewIntegrationApplicationMappingList(IntegrationApplicationMappingViewModel model)
        {
            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewApplicationMappingList.cshtml", model);
        }

        [HttpGet]
        public IActionResult GetApplicationMappingList(IntegrationApplicationMappingViewModel model, int draw, int start, int length)
        {
            IntegrationMappingDataReq request = new IntegrationMappingDataReq();
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();

            request.EditUser = request.CreateUser = ckUserEmailId;
            request.Application = model.Application;
            request.ApplicationName = model.ApplicationName;
            request.Type = model.Type;
            request.Entity = model.Entity;
            request.Start = start;
            if (length == 0)
                length = 10;
            request.Length = length;


            response = userAndRoleMgmtProviders.GetApplicationMappingList(request, token).Result;

            return Json(new
            {
                draw = draw,
                recordsTotal = response.TotalCount,
                recordsFiltered = response.TotalCount,
                data = response.IntegrationMappingList
            });
        }

        [HttpPost]
        public IActionResult DeleteIntegrationApplicationMappingInfo(IntegrationApplicationMappingViewModel model)
        {
            IntegrationMappingDataReq request = new IntegrationMappingDataReq();
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();
            request.EditUser = request.CreateUser = ckUserEmailId;
            request.Application = model.Application;
            request.ApplicationName = model.ApplicationName;
            request.Type = model.Type;
            request.Entity = model.Entity;
            request.IntegrationApplicationMapping_Id = model.IntegrationApplicationMapping_Id;

            response = userAndRoleMgmtProviders.DeleteIntegrationApplicationMappingInfo(request, token).Result;

            return Json(response);
        }

        [HttpGet]
        public IActionResult ViewIntegrationMappingData(IntegrationApplicationMappingViewModel model)
        {
            IntegrationMappingDataReq request = new IntegrationMappingDataReq();
            request.CreateUser = ckUserEmailId;
            request.Application = model.Application;
            request.ApplicationName = model.ApplicationName;
            request.IntegrationApplicationMapping_Id = model.IntegrationApplicationMapping_Id;
            request.IntegrationApplicationMappingItem_Id = model.IntegrationApplicationMappingItem_Id;
            request.Type = model.Type;
            request.Entity = model.Entity;

            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewApplicationMappingItem.cshtml", model);
        }

        [HttpGet]
        public IActionResult ViewApplicationMappingDataList(IntegrationApplicationMappingViewModel model)
        {

            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewApplicationMappingItemList.cshtml", model);
        }

        [HttpGet]
        public IActionResult GetApplicationMappingDataList(IntegrationApplicationMappingViewModel model, int draw, int start, int length)
        {
            IntegrationMappingDataReq request = new IntegrationMappingDataReq();
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();

            request.EditUser = request.CreateUser = ckUserEmailId;
            request.Application = model.Application;
            request.ApplicationName = model.ApplicationName;
            request.Type = model.Type;
            request.Entity = model.Entity;
            request.IntegrationApplicationMappingItem_Id = model.IntegrationApplicationMappingItem_Id;
            request.IntegrationApplicationMapping_Id = model.IntegrationApplicationMapping_Id;
            request.PartnerEntityCode = model.PartnerEntityCode;
            request.PartnerEntityName = model.PartnerEntityName;
            request.SystemEntityCode = model.SystemEntityCode;
            request.SystemEntityName = model.SystemEntityName;
            request.Start = start;

            if (length == 0)
                length = 10;
            request.Length = length;


            response = userAndRoleMgmtProviders.GetApplicationMappingDataList(request, token).Result;

            return Json(new
            {
                draw = draw,
                recordsTotal = response.TotalCount,
                recordsFiltered = response.TotalCount,
                data = response.IntegrationMappingItemList
            });
        }

        [HttpPost]
        public IActionResult SaveIntegrationApplicationMappingItemsInfo(IntegrationApplicationMappingViewModel model)
        {
            IntegrationMappingDataReq request = new IntegrationMappingDataReq();
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();

            request.EditUser = request.CreateUser = ckUserEmailId;
            request.Application = model.Application;
            request.ApplicationName = model.ApplicationName;
            request.Type = model.Type;
            request.Entity = model.Entity;
            request.IntegrationApplicationMappingItem_Id = model.IntegrationApplicationMappingItem_Id;
            request.IntegrationApplicationMapping_Id = model.IntegrationApplicationMapping_Id;
            request.PartnerEntityCode = model.PartnerEntityCode;
            request.PartnerEntityName = model.PartnerEntityName;
            request.SystemEntityCode = model.SystemEntityCode;
            request.SystemEntityName = model.SystemEntityName;

            bool isExists = userAndRoleMgmtProviders.CheckApplicationMappingDataExists(request, token).Result;

            if (isExists)
            {
                response.ResponseStatus.Status = "Failure";
                response.ResponseStatus.ErrorMessage = "Record already exists.";
                return Json(new { responseText = response.ResponseStatus.ErrorMessage, status = response.ResponseStatus.Status });
            }
            else
            {
                response = userAndRoleMgmtProviders.SaveIntegrationApplicationMappingDataInfo(request, token).Result;
            }

            return Json(new { responseText = response.ResponseStatus.ErrorMessage, status = response.ResponseStatus.Status });
        }

        [HttpPost]
        public IActionResult DeleteIntegrationApplicationMappingItemsInfo(IntegrationApplicationMappingViewModel model)
        {
            IntegrationMappingDataReq request = new IntegrationMappingDataReq();
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();
            request.EditUser = request.CreateUser = ckUserEmailId;
            request.Application = model.Application;
            request.ApplicationName = model.ApplicationName;
            request.Type = model.Type;
            request.Entity = model.Entity;
            request.IntegrationApplicationMappingItem_Id = model.IntegrationApplicationMappingItem_Id;
            request.IntegrationApplicationMapping_Id = model.IntegrationApplicationMapping_Id;
            request.PartnerEntityCode = model.PartnerEntityCode;
            request.PartnerEntityName = model.PartnerEntityName;
            request.SystemEntityCode = model.SystemEntityCode;
            request.SystemEntityName = model.SystemEntityName;

            response = userAndRoleMgmtProviders.DeleteIntegrationApplicationMappingItemsInfo(request, token).Result;

            return Json(response);
        }

        #endregion

        #region Integration Platform

        public IActionResult ViewIntegrationPlatform(IntegrationPlatformViewModel model)
        {
            Integration_Search_Request request = new Integration_Search_Request();
            model.ApplicationList = userAndRoleMgmtProviders.GetApplicationAttributes(request, token).Result.Application_DataList;

            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest { Property = "QRF Masters", Name = "Integration_Modules" };
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            model.ModuleList = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.OrderBy(a => a.Value).ToList();

            objMasterTypeRequest = new MasterTypeRequest { Property = "QRF Masters", Name = "Integration_Actions" };
            objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            model.ActionList = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.OrderBy(a => a.Value).ToList();
            List<string> actionList = new List<string>() { "GET","PUT","POST", "PATCH","DELETE" };
            model.Type = actionList;
            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewIntegrationPlatform.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveIntegrationPlatform(IntegrationPlatformViewModel model)
        {
           
            IntegartionPlatform_Res response = new IntegartionPlatform_Res();
            IntegartionPlatform_Req request = new IntegartionPlatform_Req();
            request.CreateUser = ckUserEmailId;
            request.Application = model.Application_Id;
            request.ApplicationName = model.ApplicationName;
            request.Module = model.Module;
            request.ModuleName = model.ModuleName;
            request.Action = model.ActionId;
            request.ActionName = model.ActionName;
            request.TypeName = model.TypeName;
            var retValue = userAndRoleMgmtProviders.CheckPlatformExit(request, token).Result;
            if (retValue)
            {
                response.ResponseStatus.Status = "Alert";
                response.ResponseStatus.StatusMessage = "Integration Platform already exist.";
                return Json(new { responseText = response.ResponseStatus.StatusMessage, status = response.ResponseStatus.Status });
            }
            else
            {
                response = userAndRoleMgmtProviders.SaveIntegrationPlatform(request, token).Result;
                return RedirectToAction("ViewIntegrationPlatformList");
            }
        }

        [HttpGet]
        public IActionResult ViewIntegrationPlatformList(IntegrationPlatformViewModel model)
        {
            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewIntegrationPlatformList.cshtml", model);
        }

        [HttpGet]
        public IActionResult GetIntegrationPlatformList(IntegrationPlatformViewModel model, int draw, int start, int length)
        {
            IntegartionPlatform_Req request = new IntegartionPlatform_Req();
            IntegartionPlatform_Res response = new IntegartionPlatform_Res();

            request.CreateUser = ckUserEmailId;
            request.Application = model.Application_Id;
            request.ApplicationName = model.ApplicationName;
            request.Module = model.Module;
            request.ModuleName = model.ModuleName;
            request.Action = model.ActionId;
            request.ActionName = model.ActionName;
            request.TypeName = model.TypeName == "Select" ? null:model.TypeName;
            request.Start = start;
            if (length == 0)
                length = 10;
            request.Length = length;


            response = userAndRoleMgmtProviders.GetIntegrationPlatformList(request, token).Result;

            return Json(new
            {
                draw = draw,
                recordsTotal = response.PlatformTotalCount,
                recordsFiltered = response.PlatformTotalCount,
                data = response.AppModuleActionInfoList
            });
        }

        [HttpGet]
        public IActionResult ViewIntegrationPlatformURL(IntegrationPlatformViewModel model)
        {
            IntegartionPlatform_Req request = new IntegartionPlatform_Req();
            request.CreateUser = ckUserEmailId;
            request.Application = model.Application_Id;
            request.ApplicationName = model.ApplicationName;
            request.Module = model.Module;
            request.ModuleName = model.ModuleName;
            request.Action = model.ActionId;
            request.ActionName = model.ActionName;
            request.TypeName = model.TypeName;

            request = userAndRoleMgmtProviders.GetIntegrationPlatformConfigInfo(request, token).Result;
            model.Configurations = request.Configurations;
            model.URL = request.URL;
            model.SystemFieldName = request.SystemFieldName;
            model.ApplicationFieldName = request.ApplicationFieldName;
            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ViewIntegrationPlatformURL.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveIntegrationPlatformConfig(IntegrationPlatformViewModel model)
        {
             IntegartionPlatform_Res response = new IntegartionPlatform_Res();
            IntegartionPlatform_Req request = new IntegartionPlatform_Req();
            request.CreateUser = ckUserEmailId;
            request.Application = model.Application_Id;
            request.ApplicationName = model.ApplicationName;
            request.Module = model.Module;
            request.ModuleName = model.ModuleName;
            request.Action = model.ActionId;
            request.ActionName = model.ActionName;
            request.URL = model.URL;
            request.SystemFieldName = model.SystemFieldName;
            request.ApplicationFieldName = model.ApplicationFieldName;
            request.EditUser = ckUserEmailId;
            request.Configurations = model.Configurations;
            request.TypeName = model.TypeName;
            response = userAndRoleMgmtProviders.SaveIntegrationPlatform(request, token).Result;
            return Json(response);
        }

        [HttpPost]
        public IActionResult DeleteIntegrationPlatform(IntegrationPlatformViewModel model)
        {
           IntegartionPlatform_Res response = new IntegartionPlatform_Res();
            IntegartionPlatform_Req request = new IntegartionPlatform_Req();
            request.CreateUser = ckUserEmailId;
            request.Application = model.Application_Id;
            request.ApplicationName = model.ApplicationName;
            request.Module = model.Module;
            request.ModuleName = model.ModuleName;
            request.Action = model.ActionId;
            request.ActionName = model.ActionName;
            request.URL = model.URL;
            request.SystemFieldName = model.SystemFieldName;
            request.ApplicationFieldName = model.ApplicationFieldName;
            try
            {
                response = userAndRoleMgmtProviders.DeleteIntegrationPlatform(request, token).Result;
                return RedirectToAction("ViewIntegrationPlatformList");
            }
            catch (Exception ex)
            {
                response.ResponseStatus.Status = "Error";
                response.ResponseStatus.StatusMessage = "Error occurred while deleting record";
                return Json(new { responseText = response.ResponseStatus.StatusMessage, status = response.ResponseStatus.Status });
            }

        }

        #endregion
        #region Mis Artifacts
        public IActionResult GetMisArtifactsData()
        {
            MisArtifactsViewModel model = new MisArtifactsViewModel();
            List<TypeMaster> typmasterList = new List<TypeMaster> { new TypeMaster { TypeId = "Report", TypeName = "Report" }, new TypeMaster { TypeId = "Dashboard", TypeName = "Dashboard" } };
            RoleGetRes response = new RoleGetRes();
            model.typeMaster = typmasterList;
            response = loginProviders.GetRoles(token).Result;
            foreach (var m in response.RoleDetails)
            {
                RoleList rl = new RoleList();
                rl.RoleName = m.RoleName;
                rl.RoleId = m.RoleId;
                model.Roles.Add(rl);
            }
            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_GetMisArtifactsData.cshtml", model);
        }
        public IActionResult SearchMisArtifactsList(MisArtifactsViewModel request)
        {
           // MisArtifactsViewModel modelList = new MisArtifactsViewModel();

            
            return PartialView("~/Areas/Settings/Views/UserAndRoleMgmt/_ListMisMapping.cshtml", request);
        }
        public IActionResult SearchMisArtifacts(SearchMisReqGet request, int draw, int start, int length)
        {
            List<MisArtifactsViewModel> modelList = new List<MisArtifactsViewModel>();
            MisArtifactsViewModel artifacts;
            if (length == 0)
            {
                length = 10;
            }
            request.TypeName = request.TypeName == "Select" ? null : request.TypeName;
            request.RoleName = request.RoleName == "Select" ? null : request.RoleName;
            var result = objMISProviders.SearchMisData(request, token).Result;
          
            foreach (var m in result.MisResults)
            {
                foreach (var n in m.Groups)
                {
                    artifacts = new MisArtifactsViewModel();
                    artifacts.TypeNameResponse = m.Type;
                    artifacts.ItemResponse = m.Item;
                    artifacts.Id = m._Id.ToString();
                    artifacts.RoleNameResponse = n;
                    modelList.Add(artifacts);
                }
            }
            if (!String.IsNullOrEmpty(request.RoleName))
            {
                 modelList = modelList.Where(x => x.RoleNameResponse == request.RoleName).ToList();
            }
            var response = modelList.Skip(start).Take(length);
            return Json(new
            {
                draw = draw,
                recordsTotal = modelList.Count,
                recordsFiltered = modelList.Count,
                data = response
            });
        }

        [HttpPost]
        public IActionResult SaveArtifactsData(MisArtifactsViewModel model)
        {
            MisSaveResponse response = new MisSaveResponse();
            SearchMisReqGet request = new SearchMisReqGet();
            request.TypeName = model.TypeName;
            request.Item = model.Item;
            request.RoleName = model.RoleName;
            var retValue = objMISProviders.SearchMisData(request, token).Result;
            if (retValue != null && retValue.MisResults.Any())
            {
                response.Response.Status = "Alert";
                response.Response.StatusMessage = "Artifacts Data to be Added already exist.";
                return Json(new { responseText = response.Response.StatusMessage, status = response.Response.Status });
            }
            else
            {
                response = objMISProviders.SaveMisData(request, token).Result;
                return RedirectToAction("SearchMisArtifactsList");
            }
        }
        [HttpPost]
        public IActionResult DeleteMisArtifactData(MisArtifactsViewModel model)
        {
            MisSaveResponse response = new MisSaveResponse();
            SearchMisReqGet request = new SearchMisReqGet();
            request.TypeName = model.TypeName;
            request.Item = model.Item;
            request.RoleName = model.RoleName;
            try
            {
                response = objMISProviders.DeleteMisArtifactData(request, token).Result;
                return RedirectToAction("SearchMisArtifactsList");
            }
            catch (Exception ex)
            {  
                response.Response.Status = "Error";
                response.Response.StatusMessage = "Error occurred while deleting record";
                return Json(new { responseText = response.Response.StatusMessage, status = response.Response.Status });
            }
           
        }
        #endregion

    }
}