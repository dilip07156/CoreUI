using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    public class AccountController : VoyagerController
    {
        private readonly IConfiguration _configuration;
        private LoginProviders loginProviders;
        public COCommonLibrary cOCommonLibrary;
        private AgentProviders agentProviders;
        private OperationsProviders operationsProviders;
        private IHostingEnvironment _env;
        private string SessionName = "IntegrationInfo";

        public AccountController(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
            _env = env;
            _configuration = configuration;
            loginProviders = new LoginProviders(_configuration);
            cOCommonLibrary = new COCommonLibrary(configuration);
            agentProviders = new AgentProviders(_configuration);
            operationsProviders = new OperationsProviders(_configuration);
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            LoginRequest objLoginRequest = new LoginRequest();
            objLoginRequest.UserName = model.Email;
            objLoginRequest.Password = model.Password;
            DeleteAllCookies();
            LoginResponse objTokenResponse = loginProviders.GetToken(objLoginRequest).Result;

            if (objTokenResponse.Message == "Success" && objTokenResponse.Token != "")
            {
                Response.Cookies.Append("JWTToken", objTokenResponse.Token, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(Convert.ToInt32(objTokenResponse.Expiry)) });

                UserDetailsRequest objUserDetailsRequest = new UserDetailsRequest();
                objUserDetailsRequest.UserName = model.Email;
                UserDetailsResponse objUserDetailsResponse = loginProviders.GetUserDetails(objUserDetailsRequest, objTokenResponse.Token).Result;

                if (objUserDetailsResponse == null)
                {
                    throw new ApplicationException("UserDetailsResponse is null");
                }

                SetCookiesForLoginUser(objUserDetailsResponse, model.Email, Convert.ToInt32(objTokenResponse.Expiry));

                //Set user login date in mUsers
                UserSetReq request = new UserSetReq();
                request.User.VoyagerUser_Id = objUserDetailsResponse.VoyagerUser_Id;
                request.User.LastLoginDate = DateTime.Now;
                UserSetRes response = loginProviders.UpdateUser(request, objTokenResponse.Token).Result;

                // create claims
                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Email, model.Email)
            };

                // create identity
                ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

                // create principal
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // sign-in
                await HttpContext.SignInAsync(
                        principal: principal,
                        properties: new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToInt32(objTokenResponse.Expiry))
                        });

                if (returnUrl != null)
                    return RedirectToLocal(returnUrl);
                else
                    return RedirectToAction("Quote", "Quote");
                //return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        // GET: /Account/IntegrationLogin
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> IntegrationLogin(IntegrationLoginViewModel model, string returnUrl)
        {
            IntegrationLoginRequest objLoginRequest = new IntegrationLoginRequest();
            objLoginRequest.User = !string.IsNullOrEmpty(model.User) ? model.User.Replace(' ', '+') : string.Empty;// User secret key Encrypted
            objLoginRequest.Key = !string.IsNullOrEmpty(model.Key) ? model.Key.Replace(' ', '+') : string.Empty;//Application Key Encrypted
            objLoginRequest.Source = model.Source;
            objLoginRequest.Module = model.Module;
            objLoginRequest.Operation = model.Operation;

            if (string.IsNullOrEmpty(returnUrl))
            {
                DeleteAllCookies();
            }
            else
            {
                return RedirectToLocal(returnUrl);
            }

            List<ProductAttributeDetails> commonSession = new List<ProductAttributeDetails>();
            if (!string.IsNullOrEmpty(returnUrl) && HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) != null)
            {
                commonSession = HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName);
                return RedirectToLocal(returnUrl);
            }

            IntegrationLoginResponse objTokenResponse = loginProviders.GetIntegrationToken(objLoginRequest).Result;

            if (objTokenResponse.Message == "Success" && objTokenResponse.Token != "")
            {
                Response.Cookies.Append("JWTToken", objTokenResponse.Token, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(Convert.ToInt32(1000)) });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "JWTToken", Value = objTokenResponse.Token });

                UserDetailsRequest objUserDetailsRequest = new UserDetailsRequest();
                objUserDetailsRequest.UserName = objTokenResponse.UserInfo.Email;
                UserDetailsResponse objUserDetailsResponse = loginProviders.GetUserDetails(objUserDetailsRequest, objTokenResponse.Token).Result;

                if (objUserDetailsResponse == null)
                {
                    throw new ApplicationException("UserDetailsResponse is null");
                }

                string EmailId = objTokenResponse.UserInfo.Email;
                int time = Convert.ToInt32(objTokenResponse.Expiry);
                SetCookiesForLoginUser(objUserDetailsResponse, EmailId, Convert.ToInt32(objTokenResponse.Expiry));
                commonSession.Add(new ProductAttributeDetails { AttributeId = "EmailId", Value = EmailId });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "UserName", Value = objUserDetailsResponse.FirstName + " " + objUserDetailsResponse.LastName });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "ContactDisplayMessage", Value = objUserDetailsResponse.ContactDisplayMessage });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "CompanyName", Value = objUserDetailsResponse.CompanyName });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "Currency", Value = objUserDetailsResponse.Currency });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "BalanceAmount", Value = objUserDetailsResponse.BalanceAmount });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "CreditAmount", Value = objUserDetailsResponse.CreditAmount });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "Photo", Value = objUserDetailsResponse.Photo ?? ""  });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "UserRoles", Value = objUserDetailsResponse.UserRoleDetails.Count > 0 ? string.Join(",", objUserDetailsResponse.UserRoleDetails.Select(a => a.RoleName)) : "No Role" });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "VoyagerUser_Id", Value = objUserDetailsResponse.VoyagerUser_Id });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "CompanyId", Value = objUserDetailsResponse.CompanyId });
                commonSession.Add(new ProductAttributeDetails { AttributeId = "ContactId", Value = objUserDetailsResponse.ContactId });

                HttpContext.Session.SetComplexData(SessionName, commonSession);

                //Set user login date in mUsers
                UserSetReq request = new UserSetReq();
                request.User.VoyagerUser_Id = objUserDetailsResponse.VoyagerUser_Id;
                request.User.LastLoginDate = DateTime.Now;
                UserSetRes response = loginProviders.UpdateUser(request, objTokenResponse.Token).Result;

                // create claims
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, objTokenResponse.UserInfo.Email),
                    new Claim(ClaimTypes.Email, objTokenResponse.UserInfo.Email)
                };

                // create identity
                ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

                // create principal
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // sign-in
                await HttpContext.SignInAsync(
                        principal: principal,
                        properties: new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToInt32(objTokenResponse.Expiry))
                        });

                if (returnUrl != null)
                    return RedirectToLocal(returnUrl);
                else
                    return CommonRedirectionBasedOnSource(objLoginRequest, objTokenResponse.Token);
            }
            else
            {
                ViewBag.Unautherized = true;
                return View("IntegrationError");
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        public JsonResult ForgetPassword(LoginViewModel model)
        {
            try
            {
                LoginRequest objLoginRequest = new LoginRequest();
                objLoginRequest.UserName = model.Email;
                LoginProviders objLoginProvider = new LoginProviders(_configuration);
                bool tskCheckUser = objLoginProvider.UserPasswordRecover(objLoginRequest).Result;

                if (tskCheckUser)
                {
                    return Json(new { msg = "Password changed." });
                }
                else
                {
                    return Json(new { msg = "Invalid" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync();
            DeleteAllCookies();
            return RedirectToAction("Login");
        }

        #region Password encryption

        public static string EncryptPassword(string plainText)
        {

            SHA512 sha512 = SHA512Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        #endregion

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Manage()
        {
            string EmailId = HttpContext.Request.Cookies["EmailId"] ?? ckUserEmailId;

            ManageViewModel manageViewModel = new ManageViewModel();

            LoginProviders objLoginProvider = new LoginProviders(_configuration);
            UserDetailsRequest objUserDetailsRequest = new UserDetailsRequest();
            objUserDetailsRequest.UserName = EmailId;

            UserDetailsResponse objUserDetailsResponse = objLoginProvider.GetUserDetails(objUserDetailsRequest, token).Result;
            ContactDetailsRequest req1 = new ContactDetailsRequest() { Email = EmailId, Users = new mUsers { VoyagerUser_Id = ckLoginUser_Id } };
            ContactDetailsResponse res1 = objLoginProvider.GetContactDetails(req1, token).Result;
            manageViewModel.Telephone = res1.Contacts.TEL;
            manageViewModel.MobileNumber = res1.Contacts.MOBILE;
            manageViewModel.FaxNumber = res1.Contacts.FAX;
            manageViewModel.WebSite = res1.Contacts.WEB;
            manageViewModel.VoyagerUser_Id = objUserDetailsResponse.VoyagerUser_Id;
            //manageViewModel.PhotoPath = _configuration.GetValue<string>("SystemSettings:CountryImageInitial") + objUserDetailsResponse.Photo;
            manageViewModel.PhotoPath = _configuration.GetValue<string>("UIBaseUrl") + objUserDetailsResponse.Photo;
            return View(manageViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Manage(ManageViewModel manageViewModel, string command, IFormFile file)
        {
            string rootPath = _configuration.GetValue<string>("SystemSettings:UserImagePath");
            //string pathIntial = _configuration.GetValue<string>("SystemSettings:CountryImageInitial");
            string pathIntial = _configuration.GetValue<string>("UIBaseUrl");
            string imgDbPath = _configuration.GetValue<string>("SystemSettings:UserImageDbPath");
            //string rootPath = _configuration.GetValue<string>("UserImagePath");

            //string destPath = _env.WebRootPath + @"\resources\DeleteImages";
            ManageViewModel manageViewModel1 = manageViewModel;
            string EmailId = HttpContext.Request.Cookies["EmailId"] ?? ckUserEmailId;

            LoginProviders objLoginProvider = new LoginProviders(_configuration);
            string SuccessMessage = "";
            string ErrorMessage = "";

            switch (command.ToLower())
            {
                case "update":
                    ContactDetailsRequest req1 = new ContactDetailsRequest()
                    {
                        Email = EmailId,
                        FAX = manageViewModel.FaxNumber,
                        TEL = manageViewModel.Telephone,
                        MOBILE = manageViewModel.MobileNumber,
                        //WEB = manageViewModel.WebSite,
                        Users = new mUsers { Password = manageViewModel.Password, VoyagerUser_Id = manageViewModel.VoyagerUser_Id }
                    };

                    ContactDetailsResponse res1 = objLoginProvider.UpdateContactDetails(req1, token).Result;
                    ContactDetailsResponse res2 = null;
                    ResponseStatus resultusepwd = new ResponseStatus();

                    if (manageViewModel.IsPasswordEnabled)
                    {
                        req1.IsUpdateCompany = true;
                        res2 = objLoginProvider.UpdateUserPassword(req1, token).Result;
                        if (res2 != null && res2.Status == "Sucess" && res2.Contacts != null && !string.IsNullOrEmpty(res2.Contacts.VoyagerContact_Id))
                        {
                            // Call Bridge service to update User Password in SQL database                        
                            UserRolesSetReq userreq = new UserRolesSetReq();
                            userreq.Contact_Id = res2.Contacts.VoyagerContact_Id;
                            resultusepwd = agentProviders.SetUserPassword(userreq, token).Result;
                        }
                        else
                        {
                            res2 = res2 ?? new ContactDetailsResponse();
                            res2.Status = "Error";
                        }
                    }

                    if (res1 != null)
                    {
                        manageViewModel1.MobileNumber = res1.Contacts.MOBILE;
                        manageViewModel1.Telephone = res1.Contacts.TEL;
                        manageViewModel1.WebSite = res1.Contacts.WEB;
                        manageViewModel1.FaxNumber = res1.Contacts.FAX;
                    }

                    if (res2 != null && res1 != null)
                    {
                        if (res2.Status == "Sucess" && res1.Status == "Sucess" && resultusepwd.Status.ToLower() != "success")
                        {
                            SuccessMessage = "Data updated successfully.";
                        }
                        else
                        {
                            if (resultusepwd != null && !string.IsNullOrEmpty(resultusepwd.Status) && resultusepwd.Status.ToLower() != "success")
                            {
                                ErrorMessage = resultusepwd.Status;
                            }
                            else if (res2.Status != "Sucess")
                            {
                                ErrorMessage = res2.Status;
                            }
                            else if (res1.Status != "Sucess")
                            {
                                ErrorMessage = res2.Status;
                            }
                        }
                    }
                    else if (res1 != null)
                    {
                        if (res1.Status == "Sucess")
                        {
                            SuccessMessage = "Data updated successfully.";
                        }
                        else if (res1.Status != "Sucess")
                        {
                            ErrorMessage = res1.Status;
                        }
                    }
                    break;
                case "upload":
                    if (file != null)
                    {
                        string _FileName = Path.GetFileName(file.FileName);
                        string oldFilePath = Request.Cookies["Photo"];
                        string oldfileName = Path.GetFileName(oldFilePath);
                        string oldFilePathURL = Path.Combine(rootPath, oldfileName);
                        string extension = Path.GetExtension(file.FileName);
                        //Guid g = Guid.NewGuid();
                        string _path = Path.Combine(rootPath, manageViewModel.VoyagerUser_Id + extension);

                        //var savePath = "~/resources/UserImages/" + manageViewModel.VoyagerUser_Id + extension;

                        var savePath = imgDbPath + manageViewModel.VoyagerUser_Id + extension;
                        using (var fileStream = new FileStream(_path, FileMode.OpenOrCreate))
                        {

                            manageViewModel1.PhotoPath = _path;
                            file.CopyTo(fileStream);
                            fileStream.Flush();
                            fileStream.Close();
                        }

                        ContactDetailsRequest request = new ContactDetailsRequest()
                        {
                            //WEB = manageViewModel.WebSite,
                            Users = new mUsers { Photo = savePath, VoyagerUser_Id = manageViewModel.VoyagerUser_Id }
                        };

                        string imgLocation = pathIntial + savePath;
                        ContactDetailsResponse response = objLoginProvider.UpdateUserDetails(request, token).Result;
                        Response.Cookies.Append("Photo", imgLocation);

                        //if (System.IO.File.Exists(oldFilePathURL))
                        //{
                        //    FileInfo fileInfo = new FileInfo(oldFilePathURL);
                        //    fileInfo.MoveTo(Path.Combine(destPath, oldfileName));

                        //}
                        if (response.Status == "Sucess")
                        {
                            SuccessMessage = "Photo Uploaded Successfully.";
                        }
                        else
                        {
                            ErrorMessage = response.Status;
                        }
                    }
                    break;
                case "cancel":

                    break;
                default:
                    break;
            }

            if (!(string.IsNullOrEmpty(SuccessMessage)))
                TempData["success"] = SuccessMessage;

            if (!(string.IsNullOrEmpty(ErrorMessage)))
                TempData["error"] = ErrorMessage;

            //return View(manageViewModel1);
            return RedirectToAction("Manage");
        }

        public IActionResult GetUsersByRole(string SalesPerson, string CostingOfficer, string ProdAccountant, string FileHandler, string QRFID, string ModuleName, string BookingNumber)
        {
            try
            {
                UserDetailsViewModel model = new UserDetailsViewModel();
                UserByRoleGetRes response = new UserByRoleGetRes();
                UserByRoleGetReq request = new UserByRoleGetReq();
                request.RoleName = new List<string>() { "Sales Officer", "Costing Officer", "Product Accountant", "Groups", "Operations_Management" };

                response = loginProviders.GetUsersByRole(request, token).Result;

                model.SalesOfficerList = response.Users.Where(a => a.UserRole == "Sales Officer").ToList();
                model.CostingOfficerList = response.Users.Where(a => a.UserRole == "Costing Officer").ToList();
                model.ProductAccountantList = response.Users.Where(a => a.UserRole == "Product Accountant").ToList();
                //model.FileHandlerList = response.Users.Where(a => a.UserRole == "Groups" || a.UserRole == "Operations_Management").Distinct().ToList();
                //model.FileHandlerList.ForEach(a => { a.UserRole = ""; a.UserRoleId = ""; });
                //model.FileHandlerList = model.FileHandlerList.Distinct().ToList();

                var FileHandlerList = response.Users.Where(a => a.UserRole == "Groups" || a.UserRole == "Operations_Management").Select(a => a.Email).Distinct().ToList();
                FileHandlerList.ForEach(a => model.FileHandlerList.Add(new UserDetails() { Email = a }));

                model.SalesOfficer = SalesPerson;
                model.CostingOfficer = CostingOfficer;
                model.ProductAccountant = ProdAccountant;
                model.FileHandler = FileHandler;

                CompanyOfficerGetRes officerGetRes = new CompanyOfficerGetRes();
                CompanyOfficerGetReq officerGetReq = new CompanyOfficerGetReq();
                if (ModuleName == "ops")
                {
                    ProductSRPHotelGetReq BookingReq = new ProductSRPHotelGetReq() { QRFID = BookingNumber };
                    OpsBookingSummaryGetRes BookingRes = operationsProviders.GetOpsBookingSummary(BookingReq, token).Result;
                    if (BookingRes != null)
                    {
                        model.ContactPersonID = BookingRes.OpsBookingSummaryDetails.ContactId;
                        model.EmailAddress = BookingRes.OpsBookingSummaryDetails.ContactEmail;
                        model.MobileNo = BookingRes.OpsBookingSummaryDetails.ContactTel;
                        officerGetReq.CompanyId = BookingRes.OpsBookingSummaryDetails.AgentId;
                    }
                }
                else
                {
                    NewQuoteViewModel quoteModel = new NewQuoteViewModel();
                    quoteModel.QRFID = QRFID;
                    cOCommonLibrary.GetCOTourInfoHeader(ref quoteModel, token);
                    model.ContactPersonID = quoteModel.COHeaderViewModel.ContactPersonID;
                    model.EmailAddress = quoteModel.COHeaderViewModel.EmailAddress;
                    model.MobileNo = quoteModel.COHeaderViewModel.MobileNo;
                    officerGetReq.CompanyId = quoteModel.COHeaderViewModel.AgentID;
                }

                officerGetReq.UserRole = "";
                officerGetRes = agentProviders.GetCompanyContacts(officerGetReq, token).Result;
                model.ContactPersonList = officerGetRes.ContactDetails;
                model.ContactPersonList.ForEach(a => a.FIRSTNAME = a.CommonTitle + " " + a.FIRSTNAME + " " + a.LastNAME);

                return PartialView("_ChangeUserForQuote", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public IActionResult GetFollowUpDetails(string CostingOfficer, string WithClient)
        {
            try
            {
                COHeaderViewModel model = new COHeaderViewModel();
                model.FollowUpCostingOfficer = CostingOfficer;
                model.WithClient = WithClient;
                return PartialView("_ChangeFollowUpDetails", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult UpdateFollowUp(string QRFId, string CostingOfficer, string WithClient)
        {
            try
            {
                CommonResponse response = new CommonResponse();
                UpdateUserGetReq request = new UpdateUserGetReq();
                request.QRFID = QRFId;
                request.FollowUpCostingOfficer = CostingOfficer;
                request.FollowUpWithClient = WithClient;
                request.EditUser = ckUserEmailId;

                response = loginProviders.UpdateFollowup(request, token).Result;

                if (response.ResponseStatus.Status.ToLower() == "success")
                    return Json(new { status = "success", responseText = response.ResponseStatus.StatusMessage });
                else
                    return Json(new { status = "error", responseText = response.ResponseStatus.ErrorMessage });
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult UpdateUserForQuote(UpdateUserGetReq request)
        {
            CommonResponse response = new CommonResponse();
            //UpdateUserGetReq request = new UpdateUserGetReq();
            //request.QRFID = QRFID;
            //request.SalesOfficer = SalesOfficer;
            //request.CostingOfficer = CostingOfficer;
            //request.ProductAccountant = ProductAccountant;
            request.EditUser = ckUserEmailId;

            response = loginProviders.UpdateUserForQuote(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }

        public IActionResult ErrorTest()
        {
            string aa = null;

            if (aa.ToUpper() == "")
                return View();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Settings()
        {
            if (!UserRoles.Contains("Administrator"))
            {
                return View("Unauthorize");
            }
            return View();
        }


        [HttpPost]
        public JsonResult CookieSet(string isCollaps)
        {
            var st = Convert.ToString(isCollaps);
            Response.Cookies.Append("IsCollaps", st);

            return Json("true");
        }

        public IActionResult CommonApplicationBasedOnSource(string source, string page, string id)
        {
            switch (source.ToUpper())
            {
                case "CRM":
                    switch (page)
                    {
                        case "SalesSearch":
                            return RedirectToAction("Quote", "Quote");
                        case "SalesNew":
                            return RedirectToAction("NewQuote", "Quote");
                        case "SalesManage":
                            return RedirectToAction("NewQuote", "Quote", new { QRFId = id });
                        case "AGENT":
                            if (!string.IsNullOrEmpty(id))
                            {
                                return RedirectToAction("GetPartnerAgentDetails", "Agent", new { PartnerEntityCode = id, Application = source });
                            }
                            return RedirectToAction("Agent", "Agent");
                        case "CONTACT":
                            if (!string.IsNullOrEmpty(id))
                            {
                                return RedirectToAction("GetPartnerAgentContactDetails", "Agent", new { PartnerEntityCode = id, Application = source });
                            }
                            return RedirectToAction("Agent", "Agent");
                        /*case "AgentManage":
                            return RedirectToAction("AgentDetail", "Agent", new { CompanyId = id});*/
                        default:
                            ViewBag.Unautherized = false;
                            return View("IntegrationError");
                    }
                default:
                    ViewBag.Unautherized = false;
                    return View("IntegrationError");
            }
        }

        public IActionResult CommonRedirectionBasedOnSource(IntegrationLoginRequest objLoginRequest, string token)
        {
            List<IntegrationConfigurationInfo> objIntegrationConfig = loginProviders.GetIntegrationRedirection(objLoginRequest, token).Result;
            string quetyString = string.Empty;
            if (objIntegrationConfig != null && objIntegrationConfig.Any())
            {
                quetyString = "?Application=" + objLoginRequest.Source;
                quetyString += "&Module=" + objLoginRequest.Module;
                quetyString += "&Operation=" + objLoginRequest.Operation;

                foreach (var item in objIntegrationConfig)
                {
                    quetyString += "&" + item.SystemFieldName + "=" + Request.Query[item.ApplicationFieldName];
                }

                return Redirect(objIntegrationConfig[0].URL + quetyString);
            }

            ViewBag.Unautherized = false;
            return View("IntegrationError");
        }
    }
}