using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VGER_WAPI_CLASSES;
//using Voyager.App.Contracts;
using Voyager.App.Library;
using Voyager.App.Models;
using Voyager.App.Providers;
using Microsoft.AspNetCore.Http;
using Voyager.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Voyager.App.Mappers;

namespace Voyager.Controllers
{
    public class VoyagerController : Controller
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _env;
        CommunicationTrailMapping communicationTrailMapping;
        public COCommonLibrary cOCommonLibrary;
        public List<ProductAttributeDetails> commonSession;
        private string SessionName = "IntegrationInfo";
        public string cToken = string.Empty;
        public string c_ckLoginUser_Id = string.Empty;
        public string c_ckUserEmailId = string.Empty;
        public string c_ckUserName = string.Empty;
        public string c_ckUserContactId = string.Empty;
        public string c_ckUserCompanyId = string.Empty;
        public string c_ckUserCompanyName = string.Empty;
        #endregion

        public VoyagerController(IConfiguration configuration)
        {
            _configuration = configuration;
            communicationTrailMapping = new CommunicationTrailMapping(configuration);
            cOCommonLibrary = new COCommonLibrary(configuration);
        }

        public VoyagerController(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        #region Getter Properties
        public string token
        {
            get
            {
                cToken = Request.Cookies["JWTToken"] ?? cToken;

                if(string.IsNullOrEmpty(cToken))
                {
                    if (commonSession == null)
                    {
                        commonSession = HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) ?? null;
                        cToken = commonSession.Where(a => a.AttributeId == "JWTToken").Select(b => b.Value).FirstOrDefault();
                    }
                    else
                    {
                        cToken = commonSession.Where(a => a.AttributeId == "JWTToken").Select(b => b.Value).FirstOrDefault();
                    }
                }
                    
                return cToken;
            }
        }

        public string ckLoginUser_Id
        {
            get
            {
                c_ckLoginUser_Id = Request.Cookies["VoyagerUser_Id"] ?? c_ckLoginUser_Id;

                if (string.IsNullOrEmpty(c_ckLoginUser_Id))
                {
                    if (commonSession == null)
                    {
                        commonSession = HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) ?? null;
                        c_ckLoginUser_Id = commonSession.Where(a => a.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault();
                    }
                    else
                    {
                        c_ckLoginUser_Id = commonSession.Where(a => a.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault();
                    }
                }

                    return c_ckLoginUser_Id;
            }
        }

        public string ckUserEmailId
        {
            get
            {
                c_ckUserEmailId = Request.Cookies["EmailId"] ?? c_ckUserEmailId;

                if (string.IsNullOrEmpty(c_ckUserEmailId))
                {
                    if (commonSession == null)
                    {
                        commonSession = HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) ?? null;
                        c_ckUserEmailId = commonSession.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault();
                    }
                    else
                    {
                        c_ckUserEmailId = commonSession.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault();
                    }
                }

                return c_ckUserEmailId;
            }
        }

        public string ckUserName
        {
            get
            {
                c_ckUserName = Request.Cookies["UserName"] ?? c_ckUserEmailId;

                if (string.IsNullOrEmpty(c_ckUserName))
                {
                    if (commonSession == null)
                    {
                        commonSession = HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) ?? null;
                        c_ckUserName = commonSession.Where(a => a.AttributeId == "UserName").Select(b => b.Value).FirstOrDefault();
                    }
                    else
                    {
                        c_ckUserName = commonSession.Where(a => a.AttributeId == "UserName").Select(b => b.Value).FirstOrDefault();
                    }
                }

                return c_ckUserName;
            }
        }
		public string ckUserContactId
		{
			get
			{
                c_ckUserContactId = Request.Cookies["ContactId"] ?? c_ckUserContactId;

                if (string.IsNullOrEmpty(c_ckUserContactId))
                {
                    if (commonSession == null)
                    {
                        commonSession = HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) ?? null;
                        c_ckUserContactId = commonSession.Where(a => a.AttributeId == "ContactId").Select(b => b.Value).FirstOrDefault();
                    }
                    else
                    {
                        c_ckUserContactId = commonSession.Where(a => a.AttributeId == "ContactId").Select(b => b.Value).FirstOrDefault();
                    }
                }

                return c_ckUserContactId;
            }
		}
		private string _UserRoles;

        public string[] UserRoles
        {
            get
            {
                _UserRoles = Request.Cookies["UserRoles"] ?? _UserRoles;
                if (string.IsNullOrEmpty(_UserRoles))
                {
                    if (commonSession == null)
                    {
                        commonSession = HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) ?? null;
                        _UserRoles = commonSession.Where(a => a.AttributeId == "UserRoles").Select(b => b.Value).FirstOrDefault();
                    }
                    else
                    {
                        _UserRoles = commonSession.Where(a => a.AttributeId == "UserRoles").Select(b => b.Value).FirstOrDefault();
                    }
                }

                if (!string.IsNullOrEmpty(_UserRoles))
                    return _UserRoles.Split(',');
                else
                    return null;
            }
        }
        #endregion

        public string ckUserCompanyId
        {
            get
            {
                c_ckUserCompanyId = Request.Cookies["CompanyId"] ?? c_ckUserCompanyId;

                if (string.IsNullOrEmpty(c_ckUserCompanyId))
                {
                    if (commonSession == null)
                    {
                        commonSession = HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) ?? null;
                        c_ckUserCompanyId = commonSession.Where(a => a.AttributeId == "CompanyId").Select(b => b.Value).FirstOrDefault();
                    }
                    else
                    {
                        c_ckUserCompanyId = commonSession.Where(a => a.AttributeId == "CompanyId").Select(b => b.Value).FirstOrDefault();
                    }
                }

                return c_ckUserCompanyId;
            }
        }

		public string ckUserCompanyName
		{
			get
			{
                c_ckUserCompanyName = Request.Cookies["CompanyName"] ?? c_ckUserCompanyName;

                if (string.IsNullOrEmpty(c_ckUserCompanyName))
                {
                    if (commonSession == null)
                    {
                        commonSession = HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) ?? null;
                        c_ckUserCompanyName = commonSession.Where(a => a.AttributeId == "CompanyName").Select(b => b.Value).FirstOrDefault();
                    }
                    else
                    {
                        c_ckUserCompanyName = commonSession.Where(a => a.AttributeId == "CompanyName").Select(b => b.Value).FirstOrDefault();
                    }
                }

                return c_ckUserCompanyName;
            }
		}

		#region Set/Remove Cookies
		public bool SetCookiesForLoginUser(UserDetailsResponse objUserDetailsResponse, string EmailId, int time)
        {
            Response.Cookies.Append("EmailId", EmailId, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("UserName", objUserDetailsResponse.FirstName + " " + objUserDetailsResponse.LastName, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("ContactDisplayMessage", objUserDetailsResponse.ContactDisplayMessage, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("CompanyName", objUserDetailsResponse.CompanyName, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("Currency", objUserDetailsResponse.Currency, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("BalanceAmount", objUserDetailsResponse.BalanceAmount, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("CreditAmount", objUserDetailsResponse.CreditAmount, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("Photo", objUserDetailsResponse.Photo ?? "", new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("UserRoles", objUserDetailsResponse.UserRoleDetails.Count > 0 ? string.Join(",", objUserDetailsResponse.UserRoleDetails.Select(a => a.RoleName)) : "No Role", new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("VoyagerUser_Id", objUserDetailsResponse.VoyagerUser_Id, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
            Response.Cookies.Append("CompanyId", objUserDetailsResponse.CompanyId, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
			Response.Cookies.Append("ContactId", objUserDetailsResponse.ContactId, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(time) });
			return true;
        }

        public bool DeleteAllCookies()
        {
            Response.Cookies.Delete("JWTToken");
            Response.Cookies.Delete("EmailId");
            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("ContactDisplayMessage");
            Response.Cookies.Delete("CompanyName");
            Response.Cookies.Delete("Currency");
            Response.Cookies.Delete("BalanceAmount");
            Response.Cookies.Delete("CreditAmount");
            Response.Cookies.Delete("UserRoles");
            Response.Cookies.Delete("Photo");
            Response.Cookies.Delete("IsCollaps"); 


            return true;
        }
        #endregion

        public JsonResult GetCityName(string term, string QRFID)
        {
            CityLookupRequest objCityLookupRequest = new CityLookupRequest();
            if (term.Length >= 3 && term.Substring(0, 3) == "###") term = "";
            objCityLookupRequest.CityName = term;
            objCityLookupRequest.QRFID = QRFID;

            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            CityLookupResponse objCityLookupResponse = objSalesProvider.GetCityList(objCityLookupRequest, token).Result;
            if (objCityLookupResponse.CityLookupProperties == null)
                return Json("");
            else
            {
                List<AutoCompleteTextBox> cityList = new List<AutoCompleteTextBox>();
                cityList = objCityLookupResponse.CityLookupProperties.Select(data => new AutoCompleteTextBox { value = data.Voyager_Resort_Id, label = data.Lookup }).ToList();
                return Json(cityList);
            }
        }

        public JsonResult GetProductNameList(ProductSearchReq objProductSearchReq)
        {
            string strProdType = objProductSearchReq.ProdType[0];
            objProductSearchReq.ProdType = new List<string>();
            objProductSearchReq.ProdType.AddRange(strProdType.Replace("\"", "").Split("|"));
            // if (objProductSearchReq.ProdName.Length >= 3 && objProductSearchReq.ProdName == "###") objProductSearchReq.ProdName = "";

            MasterProviders objMasterProviders = new MasterProviders(_configuration);
            ProductSearchRes objProductSearchRes = objMasterProviders.GetProductDetailsBySearchCriteria(objProductSearchReq, token).Result;

            if (objProductSearchRes.ResponseStatus.Status == "Success" && objProductSearchRes.ProductSearchDetails.Count > 0)
            {
                List<AutoCompleteTextBox> nameList = new List<AutoCompleteTextBox>();
                nameList = objProductSearchRes.ProductSearchDetails.Select(data => new AutoCompleteTextBox { value = data.VoyagerProduct_Id, label = data.ProdName, type = data.ProdType, typeId = data.ProdTypeId, placeholder = data.PlaceHolder }).ToList();
                return Json(nameList);
            }
            else
                return Json("");
        }

        public JsonResult GetProductCategoryByParam(ProductCatGetReq request)
        {
            MasterProviders objMasterProviders = new MasterProviders(_configuration);

            ProductCatGetRes productCatGetRes = objMasterProviders.GetProductCategoryByParam(request, token).Result;

            if (productCatGetRes.ResponseStatus.Status == "Success" && productCatGetRes.ProdCategoryDetails.Count > 0)
            {
                List<AutoCompleteTextBox> catlist = new List<AutoCompleteTextBox>();
                catlist = productCatGetRes.ProdCategoryDetails.Select(data => new AutoCompleteTextBox { value = data.ProductCategoryId, label = data.ProductCategoryName }).ToList();
                return Json(catlist);
            }
            else
                return Json("");
        }

        public JsonResult GetProductRange(string ProductId, string CategoryId, string AdditionalYN, string positionname = "")
        {
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            ProductRangeGetRes prodRangeGetRes = quoteLibrary.GetProductRangeList(ProductId, CategoryId, AdditionalYN, token);

            List<AutoCompleteTextBox> prodRangeList = new List<AutoCompleteTextBox>();
            if (prodRangeGetRes.ResponseStatus.Status.ToLower() == "success" && prodRangeGetRes.ProductRangeDetails.Count > 0)
            {
                if (positionname == "cruise")
                {
                    var roomsuppliment = new List<AutoCompleteTextBox>();
                    var roomservice = new List<AutoCompleteTextBox>();

                    roomservice = prodRangeGetRes.ProductRangeDetails.Where(a => a.AdditionalYN == false).ToList().
                        Select(a => new AutoCompleteTextBox { value = a.VoyagerProductRange_Id, label = a.ProductRangeCode + " (" + a.PersonType + (a.AgeRange == null ? "" : " | " + a.AgeRange) + ") ", type = a.AdditionalYN.ToString() }).
                         OrderBy(a => a.label.Contains("SINGLE") ? "A" : a.label.Contains("DOUBLE") ? "B" : a.label.Contains("TWIN") ? "C" : a.label.Contains("TRIPLE") ? "D" :
                                            a.label.Contains("QUAD") ? "E" : a.label.Contains("TSU") ? "F" :
                                            a.label.ToLower().Contains("child + bed") ? "G" : a.label.ToLower().Contains("child - bed") ? "H" :
                                            a.label.ToLower().Contains("infant") ? "I" : "J").ThenBy(a => a.label).ToList();

                    roomsuppliment = prodRangeGetRes.ProductRangeDetails.Where(a => a.AdditionalYN == true).
                                     Select(a => new AutoCompleteTextBox { value = a.VoyagerProductRange_Id, label = a.ProductRangeCode + " (" + a.PersonType + (a.AgeRange == null ? "" : " | " + a.AgeRange) + ") ", type = a.AdditionalYN.ToString() }).
                                     OrderBy(a => a.type).ThenBy(a => a.label).ToList();

                    roomservice.AddRange(roomsuppliment);
                    prodRangeList = roomservice;
                }
                else
                {
                    prodRangeList = prodRangeGetRes.ProductRangeDetails.Select(a => new AutoCompleteTextBox { value = a.VoyagerProductRange_Id, label = a.ProductRangeCode + " (" + a.PersonType + (a.AgeRange == null ? "" : " | " + a.AgeRange) + ") ", type = a.AdditionalYN.ToString() }).OrderBy(a => a.type).ThenBy(a => a.label).ToList();
                }

                return Json(new { prodRangeGetRes.SupplierId, prodRangeGetRes.SupplierName, prodRangeList });
            }
            else
                return Json("");
        }

        public JsonResult GetPickUpDropOffLocations(string term)
        {
            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
            if (term.Length >= 3 && term.Substring(0, 3) == "###") term = "";
            objMasterTypeRequest.Property = "QRF Masters";
            objMasterTypeRequest.Name = "PickUpDropOffLocations";
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
            {
                if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "PickUpDropOffLocations")
                {
                    List<AutoCompleteTextBox> pickUpDropOffLocationsList = new List<AutoCompleteTextBox>();
                    var result = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.ToList().Where(p => p.Value.ToLower().Contains(term.ToLower()));
                    pickUpDropOffLocationsList = result.Select(data => new AutoCompleteTextBox { value = data.AttributeValue_Id, label = data.Value }).ToList();
                    return Json(pickUpDropOffLocationsList);
                }
                else
                {
                    return Json("");
                }
            }
            else
            {
                return Json("");
            }
        }

        // DO NOT DELETE THIS CODE UNLESS WE NO LONGER REQUIRE ASSEMBLY A!!!
        private void DummyFunctionToMakeSureReferencesGetCopiedProperly_DO_NOT_DELETE_THIS_CODE()
        {
            var typ = typeof(VGER_WAPI_CLASSES.mExchangeRate);
            // Assembly A is used by this file, and that assembly depends on assembly B,
            // but this project does not have any code that explicitly references assembly B. Therefore, when another project references
            // this project, this project's assembly and the assembly A get copied to the project's bin directory, but not
            // assembly B. So in order to get the required assembly B copied over, we add some dummy code here (that never
            // gets called) that references assembly B; this will flag VS/MSBuild to copy the required assembly B over as well.
            var dummyType = typeof(VGER_WAPI_CLASSES.UserRoleDetails);
            string a = (dummyType.AssemblyQualifiedName == null) ? "" : dummyType.AssemblyQualifiedName;
        }

        #region Communication Trail
        [HttpPost]
        public IActionResult GetCommunicationTrail(DocumentStoreGetReq model)
        {
            CommunicationTrailViewModel objCommunicationTrailViewModel = communicationTrailMapping.GetCommunicationTrail(model, token);
            return PartialView("~/Views/Shared/CommunicationTrail/_CommunicationTrail.cshtml", objCommunicationTrailViewModel);
        }

        [HttpPost]
        public IActionResult GetCommunicationTrailById(DocumentStoreGetReq model)
        {
            DocumentStoreInfo objDocumentStoreInfo = communicationTrailMapping.GetCommunicationTrailById(model, token);
            if (objDocumentStoreInfo?.ResponseStatus?.Status.ToLower()=="success")
            {
                return Json(new { model = objDocumentStoreInfo, maildt = objDocumentStoreInfo.SendDate.ToString("MMM dd yyy HH:mm") });
            }
            else
            {
                return Json(new { model = objDocumentStoreInfo, maildt = "" });
            }            
        }

        [Authorize]
        [HttpGet]
        public ActionResult Download(string file)
        {
            //string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\documents", file);
            string filepath = Path.Combine(_configuration.GetValue<string>("SystemSettings:ProposalDocumentFilePath"), file);

            if (System.IO.File.Exists(filepath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                var response = new FileContentResult(fileBytes, "application/octet-stream");
                response.FileDownloadName = file;
                return response;
            }
            else
            {
                throw new FileNotFoundException(file + " File Not Found.");
            }
        }  
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult GetUploadEmailClient(CommunicationTrailGetReq model)
        //{
        //    CommunicationTrailViewModel objCommunicationTrailViewModel = new CommunicationTrailViewModel();
        //    objCommunicationTrailViewModel.EmailDetail = new EmailDetail();
        //    objCommunicationTrailViewModel.EmailDetail.QRFID = model.QRFID;
        //    objCommunicationTrailViewModel.EmailDetail.QRFPriceID = model.QRFPriceID;
        //    objCommunicationTrailViewModel.EmailDetail.AgentTourName = model.AgentTourName;
        //    objCommunicationTrailViewModel.EmailDetail.BCC = "";
        //    objCommunicationTrailViewModel.EmailDetail.CC = "";
        //    objCommunicationTrailViewModel.EmailDetail.EmailHtml = "";
        //    objCommunicationTrailViewModel.EmailDetail.From = "";
        //    objCommunicationTrailViewModel.EmailDetail.MailSent = false;
        //    objCommunicationTrailViewModel.EmailDetail.MailSentOn = DateTime.Now;
        //    objCommunicationTrailViewModel.EmailDetail.MailSentBy = "";
        //    objCommunicationTrailViewModel.EmailDetail.MailStatus = "";
        //    objCommunicationTrailViewModel.EmailDetail.MailType = "";
        //    objCommunicationTrailViewModel.EmailDetail.Remarks = "";
        //    objCommunicationTrailViewModel.EmailDetail.Subject = "";
        //    objCommunicationTrailViewModel.EmailDetail.To = "";

        //    return PartialView("CommunicationTrail/_UploadEmailFromClient", objCommunicationTrailViewModel.EmailDetail);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SetUploadEmailClient(EmailDetail model)
        //{
        //    MailSetRes objMailSetRes = agentApprovalMapping.SetEmailDetails(model, HttpContext.Request.Cookies["UserName"], token);
        //    return Json(new { status = objMailSetRes.ResponseStatus.Status, msg = objMailSetRes.ResponseStatus.ErrorMessage });
        //}
        #endregion
    }
}