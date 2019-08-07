using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;
using System.Threading.Tasks;

namespace Voyager.App.Providers
{
    public class HotelsProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers

        public HotelsProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion


        #region Product SRP
        public async Task<HotelsDeptSearchRes> GetHotelsByBookingDetails(BookingSearchReq bookingGetReq, string ticket)
        {
            HotelsDeptSearchRes HotelsDeptSearchRes = new HotelsDeptSearchRes();
            HotelsDeptSearchRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:GetHotelsByBookingDetails"), bookingGetReq, typeof(HotelsDeptSearchRes), ticket);
            return HotelsDeptSearchRes;
        }
        
        public async Task<HotelsByBookingGetRes> GetProductHotelDetails(ProductSRPHotelGetReq productSearchReq, string ticket)
        {
            HotelsByBookingGetRes productHotelGetRes = new HotelsByBookingGetRes();
            productHotelGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:GetProductHotelDetails"), productSearchReq, typeof(HotelsByBookingGetRes), ticket);
            return productHotelGetRes;
        }
        #endregion

        #region View Hotels By Booking
        public async Task<HotelAlternateServicesGetRes> GetAlternateServicesByBooking(HotelAlternateServicesGetReq productSearchReq, string ticket)
        {
            HotelAlternateServicesGetRes productHotelGetRes = new HotelAlternateServicesGetRes();
            productHotelGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:GetAlternateServicesByBooking"), productSearchReq, typeof(HotelAlternateServicesGetRes), ticket);
            return productHotelGetRes;
        }

        public async Task<HotelReservationEmailRes> SendHotelReservationRequestEmail(HotelReservationRequestEmail productSearchReq, string ticket)
        {
            HotelReservationEmailRes productHotelGetRes = new HotelReservationEmailRes();
            productHotelGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:SendHotelReservationRequestEmail"), productSearchReq, typeof(HotelReservationEmailRes), ticket);
            return productHotelGetRes;
        }
        
        public async Task<AvailabilityRequestDetailsGetRes> GetHotelAvailabilityRequestDetails(AvailabilityRequestDetailsGetReq availabilityGetReq)
        {
            AvailabilityRequestDetailsGetRes availabilityGetRes = new AvailabilityRequestDetailsGetRes();
            availabilityGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:GetHotelAvailabilityRequestDetails"), availabilityGetReq, typeof(AvailabilityRequestDetailsGetRes));
            return availabilityGetRes;
        }
        
        public async Task<HotelReservationEmailRes> UpdateHotelAvailabilityRequest(AvailabilityRequestDetailsSetReq AvailabilityReq)
        {
            HotelReservationEmailRes AvailibilityGetRes = new HotelReservationEmailRes();
            AvailibilityGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:UpdateHotelAvailabilityRequest"), AvailabilityReq, typeof(HotelReservationEmailRes));
            return AvailibilityGetRes;
        }

        public async Task<BudgetSupplementGetRes> GetBudgetSupplement(BudgetSupplementGetReq budgetSupplementGetReq, string ticket)
        {
            BudgetSupplementGetRes budgetSupplementGetRes = new BudgetSupplementGetRes();
            budgetSupplementGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:GetBudgetSupplement"), budgetSupplementGetReq, typeof(BudgetSupplementGetRes), ticket);
            return budgetSupplementGetRes;
        }

        public async Task<CommonResponse> SetBudgetSupplement(BudgetSupplementSetReq budgetSupplementSetReq, string ticket)
        {
            CommonResponse response = new CommonResponse();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:SetBudgetSupplement"), budgetSupplementSetReq, typeof(CommonResponse), ticket);
            return response;
        }

        public async Task<ActivateHotelDetailsGetRes> GetHotelActivationDetails(AvailabilityRequestDetailsGetReq availabilityGetReq, string ticket)
        {
            ActivateHotelDetailsGetRes availabilityGetRes = new ActivateHotelDetailsGetRes();
            availabilityGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:GetHotelActivationDetails"), availabilityGetReq, typeof(ActivateHotelDetailsGetRes), ticket);
            return availabilityGetRes;
        }

        public async Task<HotelReservationEmailRes> UpdateHotelActivationDetails(AvailabilityRequestDetailsGetReq AvailabilityReq, string ticket)
        {
            HotelReservationEmailRes AvailibilityGetRes = new HotelReservationEmailRes();
            AvailibilityGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceHotels:UpdateHotelActivationDetails"), AvailabilityReq, typeof(HotelReservationEmailRes), ticket);
            return AvailibilityGetRes;
        }

        public async Task<EmailGetRes> GenerateAndSendEmail(EmailGetReq EmailGetReq, string ticket)
        {
            EmailGetRes emailGetRes = new EmailGetRes();
            emailGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("EmailService:GenerateAndSendEmail"), EmailGetReq, typeof(EmailGetRes), ticket);
            return emailGetRes;
        }

        public async Task<EmailGetRes> SendEmail(EmailTemplateGetRes EmailTemplateReq, string ticket)
        {
            EmailGetRes emailGetRes = new EmailGetRes();
            emailGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("EmailService:SendEmail"), EmailTemplateReq, typeof(EmailGetRes), ticket);
            return emailGetRes;
        }

        #endregion

    }
}

