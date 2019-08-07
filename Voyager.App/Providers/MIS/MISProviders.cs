using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES.MIS;

namespace Voyager.App.Providers
{
    public class MISProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers
        public MISProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }
        #endregion

        #region MIS Role Mapping
        public async Task<MISMappingRes> CheckMisMappingsRoles(AgentCompanyReq request, string ticket)
        {
            MISMappingRes misMappingRes = new MISMappingRes();
            misMappingRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMIS:CheckMisMappingsRoles"), request, typeof(MISMappingRes), ticket);
            return misMappingRes;
        }
        public async Task<MisSearchGetResList> SearchMisData(SearchMisReqGet request, string ticket)
        {
            MisSearchGetResList misMappingRes = new MisSearchGetResList();
            misMappingRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMIS:SearchMisData"), request, typeof(MisSearchGetResList), ticket);
            return misMappingRes;
        }
        public async Task<MisSaveResponse> DeleteMisArtifactData(SearchMisReqGet request, string ticket)
        {
            MisSaveResponse misMappingRes = new MisSaveResponse();
            misMappingRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMIS:DeleteMisArtifactData"), request, typeof(MisSaveResponse), ticket);
            return misMappingRes;
        }
        public async Task<MisSaveResponse> SaveMisData(SearchMisReqGet request, string ticket)
        {
            MisSaveResponse misMappingRes = new MisSaveResponse();
            misMappingRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMIS:SaveMisData"), request, typeof(MisSaveResponse), ticket);
            return misMappingRes;
        }
        #endregion

        #region Sales Dashboard
        public async Task<SalesDashboardRes> GetSalesDashboardSummary(SalesDashboardReq request, string ticket)
        {
            SalesDashboardRes salesDashboardRes = new SalesDashboardRes();
            salesDashboardRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMIS:GetSalesDashboardSummary"), request, typeof(SalesDashboardRes), ticket);
            return salesDashboardRes;
        }

        public async Task<SalesDashboardFiltersRes> GetSalesDashboardFiltersList(AgentCompanyReq request, string ticket)
        {
            SalesDashboardFiltersRes salesDashboardRes = new SalesDashboardFiltersRes();
            salesDashboardRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMIS:GetSalesDashboardFiltersList"), request, typeof(SalesDashboardFiltersRes), ticket);
            return salesDashboardRes;
        }
        #endregion

        #region Bookings Dashboard
        public async Task<BookingsDashboardRes> GetBookingsDashboardSummary(SalesDashboardReq request, string ticket)
        {
            BookingsDashboardRes salesDashboardRes = new BookingsDashboardRes();
            salesDashboardRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMIS:GetBookingsDashboardSummary"), request, typeof(BookingsDashboardRes), ticket);
            return salesDashboardRes;
        }
        #endregion
    }
}

