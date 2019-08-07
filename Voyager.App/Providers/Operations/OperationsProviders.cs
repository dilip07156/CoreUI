using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using VGER_WAPI_CLASSES.Booking;
using Voyager.App.Proxy;

namespace Voyager.App.Providers
{
    public class OperationsProviders
    {
        private readonly IConfiguration _configuration;
        private ServiceProxy serviceProxy;

        #region Initializers
        public OperationsProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }
        #endregion

        #region ops search page & view summary
        public async Task<OpsBookingSearchRes> GetOpsBookingDetails(OpsBookingSearchReq bookingGetReq, string ticket)
        {
            OpsBookingSearchRes HotelsDeptSearchRes = new OpsBookingSearchRes();
            HotelsDeptSearchRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetOpsBookingDetails"), bookingGetReq, typeof(OpsBookingSearchRes), ticket);
            return HotelsDeptSearchRes;
        }       

        public async Task<BookingPaxDetailsGetResponse> GetOpsBookingPaxDetails(BookingPaxDetailsGetRequest bookingroomsGetReq, string ticket)
        {
            BookingPaxDetailsGetResponse bookingoomPax = new BookingPaxDetailsGetResponse();
            bookingoomPax = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetOpsBookingPaxDetails"), bookingroomsGetReq, typeof(BookingPaxDetailsGetResponse), ticket);
            return bookingoomPax;
        }

        public async Task<OpsBookingSummaryGetRes> GetOpsBookingSummary(ProductSRPHotelGetReq productSearchReq, string ticket)
        {
            OpsBookingSummaryGetRes productHotelGetRes = new OpsBookingSummaryGetRes();
            productHotelGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetOpsBookingSummary"), productSearchReq, typeof(OpsBookingSummaryGetRes), ticket);
            return productHotelGetRes;
        }

        public async Task<PositionsFromBookingGetRes> GetPositionsFromBooking(PositionsFromBookingGetReq productSearchReq, string ticket)
        {
            PositionsFromBookingGetRes positionGetRes = new PositionsFromBookingGetRes();
            positionGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetPositionsFromBooking"), productSearchReq, typeof(PositionsFromBookingGetRes), ticket);
            return positionGetRes;
        }

        #endregion

        #region Rooming List
        public async Task<BookingRoomHotelsGetRes> GetRoomingDetailsForHotels(BookingRoomingGetRequest bookingRoomingGetRequest, string ticket)
        {
            BookingRoomHotelsGetRes bookingRoomHotelsGetRes = new BookingRoomHotelsGetRes();
            bookingRoomHotelsGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetRoomingDetailsForHotels"), bookingRoomingGetRequest, typeof(BookingRoomHotelsGetRes), ticket);
            return bookingRoomHotelsGetRes;
        }

        public async Task<QrfPackagePriceGetRes> GetQRFPackagePriceForRoomsDetails(QrfPackagePriceGetReq bookingroomsReq, string ticket)
        {
            QrfPackagePriceGetRes bookingoomsinPackage = new QrfPackagePriceGetRes();
            bookingoomsinPackage = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetQRFPackagePriceForRoomsDetails"), bookingroomsReq, typeof(QrfPackagePriceGetRes), ticket);
            return bookingoomsinPackage;
        }

        public async Task<BookingRoomGetResponse> GetOpsBookingRoomsDetails(BookingRoomsGetRequest bookingroomsGetReq, string ticket)
        {
            BookingRoomGetResponse bookingoomRes = new BookingRoomGetResponse();
            bookingoomRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetOpsBookingRoomsDetails"), bookingroomsGetReq, typeof(BookingRoomGetResponse), ticket);
            return bookingoomRes;
        }

        public async Task<BookingRoomingSetResponse> SetRoomingDetails(SetPassengerDetailsReq PassengerRequest, string ticket)
        {
            BookingRoomingSetResponse roomingResponse = new BookingRoomingSetResponse();
            roomingResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:SetRoomingDetails"), PassengerRequest, typeof(BookingRoomingSetResponse), ticket);
            return roomingResponse;

        }

        public async Task<BookingRoomingSetResponse> SaveRoomingListPersonDetails(SetPassengerDetailsReq PassengerRequest, string ticket)
        {
            BookingRoomingSetResponse roomingResponse = new BookingRoomingSetResponse();
            roomingResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:SaveRoomingListPersonDetails"), PassengerRequest, typeof(BookingRoomingSetResponse), ticket);
            return roomingResponse;

        }

        public async Task<BookingRoomsSetResponse> UpdateBookingRoomsDataAsperExcel(BookingRoomsSetRequest BookingRoomData, string ticket)
        {
            BookingRoomsSetResponse roomingResponse = new BookingRoomsSetResponse();
            roomingResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:UpdateBookingRoomsDataAsperExcel"), BookingRoomData, typeof(BookingRoomsSetResponse), ticket);
            return roomingResponse;

        }

        public async Task<BookingRoomingGetResponse> GetRoomingDetails(BookingRoomingGetRequest PassengerRequest, string ticket)
        {
            BookingRoomingGetResponse roomingResponse = new BookingRoomingGetResponse();
            roomingResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetRoomingDetails"), PassengerRequest, typeof(BookingRoomingGetResponse), ticket);
            return roomingResponse;

        }
        public async Task<OpsBookingSetRes> CancelRoomingListUpdate(string BookingNumber ,string ticket)
        {
            OpsBookingSetRes roomingResponse = new OpsBookingSetRes();
            roomingResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:CancelRoomingListUpdate"), BookingNumber, typeof(string), ticket);
            return roomingResponse;

        }
        #endregion

        #region View Service Status->Itinerary
        public async Task<OpsBookingItineraryGetRes> GetBookingItineraryDetails(OpsBookingItineraryGetReq opsBookingItineraryGetReq, string ticket)
        {
            OpsBookingItineraryGetRes objOpsBookingItineraryGetRes = new OpsBookingItineraryGetRes();
            objOpsBookingItineraryGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetBookingItineraryDetails"), opsBookingItineraryGetReq, typeof(OpsBookingItineraryGetRes), ticket);
            return objOpsBookingItineraryGetRes;
        }

        public async Task<OpsBookingItineraryGetRes> GetPositionTypeByPositionId(OpsBookingItineraryGetReq opsBookingItineraryGetReq, string ticket)
        {
            OpsBookingItineraryGetRes objOpsBookingItineraryGetRes = new OpsBookingItineraryGetRes();
            objOpsBookingItineraryGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetPositionTypeByPositionId"), opsBookingItineraryGetReq, typeof(OpsBookingItineraryGetRes), ticket);
            return objOpsBookingItineraryGetRes;
        }
        #endregion

        #region Common Opeartion Header
        public async Task<OperationHeaderInfo> GetOperationHeaderDetails(OpsHeaderGetReq opsHeaderGetReq, string ticket)
        {
            OperationHeaderInfo operationHeaderInfo = new OperationHeaderInfo();
            operationHeaderInfo = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetOperationHeaderDetails"), opsHeaderGetReq, typeof(OperationHeaderInfo), ticket);
            return operationHeaderInfo;
        }

        public async Task<OpsProductTypeGetRes> GetOpsProductTypeDetails(OpsProductTypeGetReq opsProductTypeGetReq, string ticket)
        {
            OpsProductTypeGetRes opsProductTypeGetRes = new OpsProductTypeGetRes();
            opsProductTypeGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetOpsProductTypeDetails"), opsProductTypeGetReq, typeof(OpsProductTypeGetRes), ticket);
            return opsProductTypeGetRes;
        }

        public async Task<OpsProductTypeGetRes> GetProdTypePositionByParam(OpsProdTypePositionGetReq opsProdTypePositionGetReq, string ticket)
        {
            OpsProductTypeGetRes opsProductTypeGetRes = new OpsProductTypeGetRes();
            opsProductTypeGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetProdTypePositionByParam"), opsProdTypePositionGetReq, typeof(OpsProductTypeGetRes), ticket);
            return opsProductTypeGetRes;
        }

        public async Task<OpsProdRangePersTypeGetRes> GetPersonTypeByProductRange(OpsProdRangePersTypeGetReq opsProdTypePositionGetReq, string ticket)
        {
            OpsProdRangePersTypeGetRes opsProductTypeGetRes = new OpsProdRangePersTypeGetRes();
            opsProductTypeGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetPersonTypeByProductRange"), opsProdTypePositionGetReq, typeof(OpsProdRangePersTypeGetRes), ticket);
            return opsProductTypeGetRes;
        }
        #endregion

        #region Position FOC
        public async Task<OpsFOCSetRes> SetBookingPositionFOC(OpsFOCSetReq opsFocSetReq, string ticket)
        {
            OpsFOCSetRes opsFocSetRes = new OpsFOCSetRes();
            opsFocSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:SetBookingPositionFOC"), opsFocSetReq, typeof(OpsFOCSetRes), ticket);
            return opsFocSetRes;
        }
        #endregion

        #region Financials
        public async Task<OpsFinancialsGetRes> GetOpsFinancialDetails(OpsFinancialsGetReq opsFinancialGetReq, string ticket)
        {
            OpsFinancialsGetRes opsFinancialGetRes = new OpsFinancialsGetRes();
            opsFinancialGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetOpsFinancialDetails"), opsFinancialGetReq, typeof(OpsFinancialsGetRes), ticket);
            return opsFinancialGetRes;
        }
        #endregion

        #region Ops SetBookingByWorkflow
        public async Task<OpsBookingSetRes> SetBookingByWorkflow(OpsBookingSetReq opsBookingSetReq, string ticket)
        {
            OpsBookingSetRes opsBookingSetRes = new OpsBookingSetRes();
            opsBookingSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:SetBookingByWorkflow"), opsBookingSetReq, typeof(OpsBookingSetRes), ticket);
            return opsBookingSetRes;

        }
        #endregion

        #region Itinerary Builder
        public async Task<OpsBookingItineraryGetRes> GetItineraryBuilderDetails(OpsBookingItineraryGetReq opsBookingItineraryGetReq, string ticket)
        {
            OpsBookingItineraryGetRes objOpsBookingItineraryGetRes = new OpsBookingItineraryGetRes();
            objOpsBookingItineraryGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetItineraryBuilderDetails"), opsBookingItineraryGetReq, typeof(OpsBookingItineraryGetRes), ticket);
            return objOpsBookingItineraryGetRes;
        }

        public async Task<OpsBookingItineraryGetRes> GetItineraryBuilderPositionDetailById(OpsBookingItineraryGetReq opsBookingItineraryGetReq, string ticket)
        {
            OpsBookingItineraryGetRes objOpsBookingItineraryGetRes = new OpsBookingItineraryGetRes();
            objOpsBookingItineraryGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:GetItineraryBuilderPositionDetailById"), opsBookingItineraryGetReq, typeof(OpsBookingItineraryGetRes), ticket);
            return objOpsBookingItineraryGetRes;
        }

        public async Task<OpsBookingItinerarySetRes> SetRemarksForItineraryBuilderDetails(OpsBookingItinerarySetReq opsBookingItinerarySetReq, string ticket)
        {
            OpsBookingItinerarySetRes objOpsBookingItinerarySetRes = new OpsBookingItinerarySetRes();
            objOpsBookingItinerarySetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:SetRemarksForItineraryBuilderDetails"), opsBookingItinerarySetReq, typeof(OpsBookingItinerarySetRes), ticket);
            return objOpsBookingItinerarySetRes;
        }

        public async Task<OpsBookingItinerarySetRes> SetItineraryBuilderDetails(OpsBookingItinerarySetReq opsBookingItinerarySetReq, string ticket)
        {
            OpsBookingItinerarySetRes objOpsBookingItinerarySetRes = new OpsBookingItinerarySetRes();
            objOpsBookingItinerarySetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceOperations:SetItineraryBuilderDetails"), opsBookingItinerarySetReq, typeof(OpsBookingItinerarySetRes), ticket);
            return objOpsBookingItinerarySetRes;
        }
        #endregion 
    }
}