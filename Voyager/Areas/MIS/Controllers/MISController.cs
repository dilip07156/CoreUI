using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;
using Voyager.Controllers;

namespace Voyager.Areas.Operations.Controllers
{
    [Authorize]
    [Area("MIS")]
    [Route("MIS")]
    public class MISController : VoyagerController
    {
        private readonly IConfiguration _configuration;
        MISProviders objMISProviders;
        AgentProviders agentProviders;
        SalesProviders objSalesProvider;
        SalesQuoteLibrary salesLibrary;

        public MISController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            objMISProviders = new MISProviders(_configuration);
            agentProviders = new AgentProviders(_configuration);
            objSalesProvider = new SalesProviders(_configuration);
            salesLibrary = new SalesQuoteLibrary(_configuration);
        }

        [Route("MISModule")]
        public IActionResult MISModule()
        {
            MISModuleViewModel model = new MISModuleViewModel();
            var AgentRes = objMISProviders.CheckMisMappingsRoles(new AgentCompanyReq { AgentName = "", CompanyId = ckUserCompanyId, UserId = ckLoginUser_Id, SpecificFilterName = "ALL", Roles = UserRoles }, token).Result;
            if (AgentRes?.MISMappingList?.Count > 0)
            {
                model.MISMappingList = AgentRes.MISMappingList?.OrderBy(a => a.ItemSeq).ToList();
                model.ActiveMenu = model.MISMappingList?.FirstOrDefault()?.ItemName;
                model.ActiveMenuUrl = model.MISMappingList?.FirstOrDefault()?.ItemUrl;
                model.MISMappingList = AgentRes.MISMappingList?.OrderBy(a => a.ItemName).ToList();
            }


            return View(model);
        }

        #region Sales Dashboard

        [Route("SalesDashboard")]
        public IActionResult SalesDashboard()
        {
            SalesDashboardViewModel model = new SalesDashboardViewModel();
            try
            {
                #region Bind Dropdown Data
                var AgentRes = objMISProviders.GetSalesDashboardFiltersList(new AgentCompanyReq { AgentName = "", CompanyId = ckUserCompanyId, UserId = ckLoginUser_Id, SpecificFilterName = "ALL", Roles = UserRoles }, token).Result;
                //model.SalesDashboardFilters.MISMapping = AgentRes.MISMappingList;
                model.SalesDashboardFilters.SalesOfficeList = AgentRes.SalesOfficeList;
                model.SalesDashboardFilters.SalesPersonList = AgentRes.SalesPersonList;
                model.SalesDashboardFilters.AgentNameList = AgentRes.AgentList.Select(a => new AttributeValues { AttributeValue_Id = a.VoyagerCompany_Id, Value = a.Name }).ToList();


                var DestinationRes = salesLibrary.BindMasterData(_configuration, token, "QRF Destination");
                model.SalesDashboardFilters.DestinationList = DestinationRes.DestinationList;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(model);
        }

        [Route("LoadSalesOfficeBySalesOffice")]
        public JsonResult LoadSalesOfficeBySalesOffice(string SalesOfficeID)
        {
            List<AttributeValues> SalesPersonList = new List<AttributeValues>();
            try
            {
                if (string.IsNullOrEmpty(SalesOfficeID)) SalesOfficeID = ckUserCompanyId;
                var AgentRes = objMISProviders.GetSalesDashboardFiltersList(new AgentCompanyReq { AgentName = "", CompanyId = SalesOfficeID, UserId = ckLoginUser_Id, SpecificFilterName = "SALESOFFICER" }, token).Result;
                SalesPersonList = AgentRes.SalesPersonList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(SalesPersonList);
        }

        [Route("_SalesDashboardResult")]
        public IActionResult _SalesDashboardResult(SalesDashboardFilters filters)
        {
            try
            {
                #region Bind Dashboard Data
                SalesDashboardReq request = new SalesDashboardReq()
                {
                    SalesOffice = filters.SalesOffice,
                    SalesOfficeID = filters.SalesOfficeID,
                    SalesPerson = filters.SalesPerson,
                    SalesPersonID = filters.SalesPersonID,
                    Destination = filters.Destination,
                    DestinationID = filters.DestinationID,
                    Agent = filters.Agent,
                    AgentID = filters.AgentID,
                };

                var response = objMISProviders.GetSalesDashboardSummary(request, token).Result;
                var sales = response.SalesDashboardSummary.PassengerForecastGraph;
                StringBuilder json;
                SalesOfficeWiseDetailsGraph paxdetail;

                #region Quote Forecast
                json = new StringBuilder();

                foreach (var pax in sales.OrderBy(a => a.MonthYear))
                {
                    if (json.Length > 0) json.Append(",");
                    else json.Append("[");
                    //change
                    json.Append("{ \"month\" : \"");
                    json.Append(pax.MonthYear);
                    json.Append("\"");
                    foreach (var SalesOffice in response.SalesDashboardSummary.SalesOfficeList) //pax.PaxDetails)
                    {
                        paxdetail = pax.PaxDetails.Where(a => a.SalesOffice == SalesOffice).FirstOrDefault();

                        json.Append(" , \"");
                        json.Append(SalesOffice);

                        if (paxdetail == null || paxdetail?.Quotes == 0)
                        {
                            json.Append("\" : \"");
                            json.Append(0);
                            json.Append("\"");
                        }
                        else
                        {
                            json.Append("\" : \"");
                            json.Append(paxdetail?.Quotes);
                            json.Append("\"");
                        }
                    }
                    json.Append(" }");
                }
                json.Append("]");

                response.SalesDashboardSummary.QuoteChartJson = json.ToString();
                #endregion

                #region Passenger Forecast
                json = new StringBuilder();

                foreach (var pax in sales.OrderBy(a => a.MonthYear))
                {
                    if (json.Length > 0) json.Append(",");
                    else json.Append("[");
                    //change
                    json.Append("{ \"month\" : \"");
                    json.Append(pax.MonthYear);
                    json.Append("\"");
                    foreach (var SalesOffice in response.SalesDashboardSummary.SalesOfficeList) //pax.PaxDetails)
                    {
                        paxdetail = pax.PaxDetails.Where(a => a.SalesOffice == SalesOffice).FirstOrDefault();

                        json.Append(" , \"");
                        json.Append(SalesOffice);

                        if (paxdetail == null || paxdetail?.TotalPax == 0)
                        {
                            json.Append("\" : \"");
                            json.Append(0);
                            json.Append("\"");
                        }
                        else
                        {
                            json.Append("\" : \"");
                            json.Append(paxdetail?.TotalPax);
                            json.Append("\"");
                        }
                    }
                    json.Append(" }");
                }
                json.Append("]");

                response.SalesDashboardSummary.PassengerChartJson = json.ToString();
                #endregion

                #region Passenger Forecast
                json = new StringBuilder();

                foreach (var pax in sales.OrderBy(a => a.MonthYear))
                {
                    if (json.Length > 0) json.Append(",");
                    else json.Append("[");
                    //change
                    json.Append("{ \"month\" : \"");
                    json.Append(pax.MonthYear);
                    json.Append("\"");
                    foreach (var SalesOffice in response.SalesDashboardSummary.SalesOfficeList) //pax.PaxDetails)
                    {
                        paxdetail = pax.PaxDetails.Where(a => a.SalesOffice == SalesOffice).FirstOrDefault();

                        json.Append(" , \"");
                        json.Append(SalesOffice);

                        if (paxdetail == null || paxdetail?.SalesValue == 0)
                        {
                            json.Append("\" : \"");
                            json.Append(0);
                            json.Append("\"");
                        }
                        else
                        {
                            json.Append("\" : \"");
                            json.Append(paxdetail?.SalesValue);
                            json.Append("\"");
                        }
                    }
                    json.Append(" }");
                }
                json.Append("]");

                response.SalesDashboardSummary.RevenueChartJson = json.ToString();
                #endregion

                return View(response.SalesDashboardSummary);
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(new SalesDashboardSummary());
            }
        }
        #endregion

        #region Bookings Dashboard

        [Route("BookingsDashboard")]
        public IActionResult BookingsDashboard()
        {
            try
            {
                #region Bind Dashboard Data
                BookingsDashboardRes response = objMISProviders.GetBookingsDashboardSummary(new SalesDashboardReq(), token).Result;
                var sales = response.BookingsDashboardSummary.BookingVolumeGraph;
                StringBuilder json;
                SalesOfficeWiseDetailsGraph paxdetail;

                #region Booking Volume Json
                json = new StringBuilder();

                foreach (var pax in sales.OrderBy(a => a.MonthYear))
                {
                    if (json.Length > 0) json.Append(",");
                    else json.Append("[");

                    json.Append("{ \"month\" : \"");
                    json.Append(pax.MonthYear);
                    json.Append("\"");

                    foreach (var SalesOffice in response.BookingsDashboardSummary.SalesOfficeList)
                    {
                        paxdetail = pax.PaxDetails.Where(a => a.SalesOffice == SalesOffice).FirstOrDefault();

                        json.Append(" , \"");
                        json.Append(SalesOffice);

                        if (paxdetail == null || paxdetail?.Quotes == 0)
                        {
                            json.Append("\" : \"");
                            json.Append(0);
                            json.Append("\"");
                        }
                        else
                        {
                            json.Append("\" : \"");
                            json.Append(paxdetail?.Quotes);
                            json.Append("\"");
                        }
                    }
                    json.Append(" }");
                }
                json.Append("]");

                response.BookingsDashboardSummary.BookingVolumeJson = json.ToString();
                #endregion

                #region Passenger Volume Json
                json = new StringBuilder();

                foreach (var pax in sales.OrderBy(a => a.MonthYear))
                {
                    if (json.Length > 0) json.Append(",");
                    else json.Append("[");

                    json.Append("{ \"month\" : \"");
                    json.Append(pax.MonthYear);
                    json.Append("\"");

                    foreach (var SalesOffice in response.BookingsDashboardSummary.SalesOfficeList)
                    {
                        paxdetail = pax.PaxDetails.Where(a => a.SalesOffice == SalesOffice).FirstOrDefault();

                        json.Append(" , \"");
                        json.Append(SalesOffice);

                        if (paxdetail == null || paxdetail?.TotalPax == 0)
                        {
                            json.Append("\" : \"");
                            json.Append(0);
                            json.Append("\"");
                        }
                        else
                        {
                            json.Append("\" : \"");
                            json.Append(paxdetail?.TotalPax);
                            json.Append("\"");
                        }
                    }
                    json.Append(" }");
                }
                json.Append("]");

                response.BookingsDashboardSummary.PassengerVolumeJson = json.ToString();
                #endregion

                #region Booking Volume Json
                json = new StringBuilder();

                foreach (var pax in sales.OrderBy(a => a.MonthYear))
                {
                    if (json.Length > 0) json.Append(",");
                    else json.Append("[");

                    json.Append("{ \"month\" : \"");
                    json.Append(pax.MonthYear);
                    json.Append("\"");

                    foreach (var SalesOffice in response.BookingsDashboardSummary.SalesOfficeList)
                    {
                        paxdetail = pax.PaxDetails.Where(a => a.SalesOffice == SalesOffice).FirstOrDefault();

                        json.Append(" , \"");
                        json.Append(SalesOffice);

                        if (paxdetail == null || paxdetail?.SalesValue == 0)
                        {
                            json.Append("\" : \"");
                            json.Append(0);
                            json.Append("\"");
                        }
                        else
                        {
                            json.Append("\" : \"");
                            json.Append(paxdetail?.SalesValue);
                            json.Append("\"");
                        }
                    }
                    json.Append(" }");
                }
                json.Append("]");

                response.BookingsDashboardSummary.BookingRevenueJson = json.ToString();
                #endregion

                return View(response.BookingsDashboardSummary);
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Content("An error occured while processing your request");
            }
        }
        #endregion
    }
}
