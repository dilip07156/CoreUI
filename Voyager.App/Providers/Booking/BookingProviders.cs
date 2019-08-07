using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using Voyager.App.Proxy;

namespace Voyager.App.Providers
{
    public class BookingProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers

        public BookingProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion

        #region Booking Details

        public async Task<BookingSearchRes> GetSearchBookingDetails(BookingSearchReq bookingGetReq, string ticket)
        {
            BookingSearchRes bookingGetRes = new BookingSearchRes();
            bookingGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceBooking:GetSearchBookingDetails"), bookingGetReq, typeof(BookingSearchRes), ticket);
            return bookingGetRes;
        }

        public async Task<BookingInfoRes> GetBookingDetailsByParam(BookingDetailReq bookingDetailReq, string ticket)
        {
            BookingInfoRes bookingInfoRes = new BookingInfoRes();
            bookingInfoRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceBooking:GetBookingDetailsByParam"), bookingDetailReq, typeof(BookingInfoRes), ticket);
            return bookingInfoRes;
        }

        public async Task<BookingSearchRes> GetBookingStatusList(string ticket)
        {
            BookingSearchRes response = new BookingSearchRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceBooking:GetBookingStatusList"), null, typeof(BookingSearchRes), ticket);
            return response;
        }

        public async Task<BookingSearchRes> GetBookingRoomDetails(BookingSearchReq bookingGetReq, string ticket)
        {
            BookingSearchRes bookingGetRes = new BookingSearchRes();
            bookingGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceBooking:GetBookingRoomDetails"), bookingGetReq, typeof(BookingSearchRes), ticket);
            return bookingGetRes;
        }

        public async Task<BookingSearchRes> GetBookingPositionPricingDetails(BookingSearchReq bookingGetReq, string ticket)
        {
            BookingSearchRes bookingGetRes = new BookingSearchRes();
            bookingGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceBooking:GetBookingPositionPricingDetails"), bookingGetReq, typeof(BookingSearchRes), ticket);
            return bookingGetRes;
        }

        public async Task<BookingDetailRes> GetAllBookingData(BookingDetailReq bookingGetReq, string ticket)
        {
            BookingDetailRes bookingGetRes = new BookingDetailRes();
            bookingGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("DistributionServiceBooking:GetAllBookingData"), bookingGetReq, typeof(BookingDetailRes), ticket, "Distribution");
            return bookingGetRes;
        }

        public async Task<BookingDocumentGetRes> GenerateItinerary(BookingDocumentGetReq bookingdocGetReq)
        {
            BookingDocumentGetRes bookingdocGetRes = new BookingDocumentGetRes();
            bookingdocGetRes = await serviceProxy.GetServiceData(_configuration.GetValue<string>("BookingDocumentService:GenerateItinerary") + "/" + bookingdocGetReq.Booking_Id + "/" + bookingdocGetReq.User, typeof(BookingDocumentGetRes), null, "BridgeGet");
            return bookingdocGetRes;
        }

        public async Task<BookingDocumentGetRes> DownloadItinerary(BookingDocumentGetReq bookingdocGetReq)
        {
            BookingDocumentGetRes bookingdocGetRes = new BookingDocumentGetRes();
            bookingdocGetRes = await serviceProxy.GetServiceData(_configuration.GetValue<string>("BookingDocumentService:DownloadItinerary") + "/" + bookingdocGetReq.Document_Id + "/" + bookingdocGetReq.User, typeof(BookingDocumentGetRes), null, "BridgeGet");
            return bookingdocGetRes;
        }

        public async Task<BookingDocumentGetRes> GenerateFinalItinerary(BookingDocumentGetReq bookingdocGetReq)
        {
            BookingDocumentGetRes bookingdocGetRes = new BookingDocumentGetRes();
            bookingdocGetRes = await serviceProxy.GetServiceData(_configuration.GetValue<string>("BookingDocumentService:GenerateFinalItinerary") + "/" + bookingdocGetReq.Booking_Id + "/" + bookingdocGetReq.User, typeof(BookingDocumentGetRes), null, "BridgeGet");
            return bookingdocGetRes;
        }

        public async Task<BookingDocumentGetRes> DownloadFinalItinerary(BookingDocumentGetReq bookingdocGetReq)
        {
            BookingDocumentGetRes bookingdocGetRes = new BookingDocumentGetRes();
            bookingdocGetRes = await serviceProxy.GetServiceData(_configuration.GetValue<string>("BookingDocumentService:DownloadFinalItinerary") + "/" + bookingdocGetReq.Document_Id + "/" + bookingdocGetReq.User, typeof(BookingDocumentGetRes), null, "BridgeGet");
            return bookingdocGetRes;
        }

        public async Task<BookingDocumentGetRes> GenerateInvoice(BookingDocumentGetReq bookingdocGetReq)
        {
            BookingDocumentGetRes bookingdocGetRes = new BookingDocumentGetRes();
            bookingdocGetRes = await serviceProxy.GetServiceData(_configuration.GetValue<string>("BookingDocumentService:GenerateInvoice") + "/" + bookingdocGetReq.Booking_Id + "/" + bookingdocGetReq.User, typeof(BookingDocumentGetRes), null, "BridgeGet");
            return bookingdocGetRes;
        }

        public async Task<BookingDocumentGetRes> DownloadInvoice(BookingDocumentGetReq bookingdocGetReq)
        {
            BookingDocumentGetRes bookingdocGetRes = new BookingDocumentGetRes();
            bookingdocGetRes = await serviceProxy.GetServiceData(_configuration.GetValue<string>("BookingDocumentService:DownloadInvoice") + "/" + bookingdocGetReq.Document_Id + "/" + bookingdocGetReq.User, typeof(BookingDocumentGetRes), null, "BridgeGet");
            return bookingdocGetRes;
        }

        public async Task<BookingDocumentGetRes> GenerateVoucher(BookingDocumentGetReq bookingdocGetReq)
        {
            BookingDocumentGetRes bookingdocGetRes = new BookingDocumentGetRes();
            bookingdocGetRes = await serviceProxy.GetServiceData(_configuration.GetValue<string>("BookingDocumentService:GenerateVoucher") + "/" + bookingdocGetReq.Booking_Id + "/" + bookingdocGetReq.User, typeof(BookingDocumentGetRes), null, "BridgeGet");
            return bookingdocGetRes;
        }

        public async Task<BookingDocumentGetRes> DownloadVoucher(BookingDocumentGetReq bookingdocGetReq)
        {
            BookingDocumentGetRes bookingdocGetRes = new BookingDocumentGetRes();
            bookingdocGetRes = await serviceProxy.GetServiceData(_configuration.GetValue<string>("BookingDocumentService:DownloadVoucher") + "/" + bookingdocGetReq.Document_Id + "/" + bookingdocGetReq.User, typeof(BookingDocumentGetRes), null, "BridgeGet");
            return bookingdocGetRes;
        }

        public async Task<BookingDocumentGetRes> GetBookingDocumentDetails(BookingDocumentGetReq bookingGetReq, string ticket)
        {
            BookingDocumentGetRes bookingGetRes = new BookingDocumentGetRes();
            bookingGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceBooking:GetBookingDocumentDetails"), bookingGetReq, typeof(BookingDocumentGetRes), ticket);
            return bookingGetRes;
        }

        public async Task<BookingDocumentSetRes> SetBookingDocumentDetails(BookingDocumentSetReq bookingSetReq, string ticket)
        {
            BookingDocumentSetRes bookingSetRes = new BookingDocumentSetRes();
            bookingSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceBooking:SetBookingDocumentDetails"), bookingSetReq, typeof(BookingDocumentSetRes), ticket);
            return bookingSetRes;
        }
        #endregion
    }
}
