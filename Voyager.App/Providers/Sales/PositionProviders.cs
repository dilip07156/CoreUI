using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;
using System.Threading.Tasks;

namespace Voyager.App.Providers
{
    public class PositionProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers

        public PositionProviders()
        {
        }

        public PositionProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion

        #region Get Set Position
        public async Task<PositionGetRes> GetPosition(PositionGetReq positionGetReq, string ticket)
        {
            PositionGetRes positionGetRes = new PositionGetRes();
            positionGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:GetPosition"), positionGetReq, typeof(PositionGetRes), ticket);
            return positionGetRes;
        }

        public async Task<PositionSetRes> SetPosition(PositionSetReq positionSetReq, string ticket)
        {
            PositionSetRes positionSetRes = new PositionSetRes();
            positionSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:SetPosition"), positionSetReq, typeof(PositionSetRes), ticket);
            return positionSetRes;
        }

        public async Task<PositionGetRoomTypeAndSuppRes> GetRoomTypeAndSupplement(ProdCategoryRangeGetReq prodCategoryRangeGetReq, string ticket)
        {
            PositionGetRoomTypeAndSuppRes positionGetRoomTypeAndSuppRes = new PositionGetRoomTypeAndSuppRes();
            positionGetRoomTypeAndSuppRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:GetPositionRoomAndSupplement"), prodCategoryRangeGetReq, typeof(PositionGetRoomTypeAndSuppRes), ticket);
            return positionGetRoomTypeAndSuppRes;
        }
        #endregion

        #region Prices
        public async Task<PositionPriceGetRes> GetPostionPrices(PositionPriceGetReq positionPricesGetReq, string ticket)
        {
            PositionPriceGetRes positionPricesGetRes = new PositionPriceGetRes();
            positionPricesGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:GetPositionPrice"), positionPricesGetReq, typeof(PositionPriceGetRes), ticket);
            return positionPricesGetRes;
        }

        public async Task<PositionPriceSetRes> SetPostionPrices(PositionPriceSetReq positionPricesSetReq, string ticket)
        {
            PositionPriceSetRes positionPricesSetRes = new PositionPriceSetRes();
            positionPricesSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:SetPositionPrice"), positionPricesSetReq, typeof(PositionPriceSetRes), ticket);
            return positionPricesSetRes;
        }
        #endregion

        #region FOC
        public async Task<PositionFOCGetRes> GetPostionFOC(PositionFOCGetReq positionFOCGetReq, string ticket)
        {
            PositionFOCGetRes positionFOCGetRes = new PositionFOCGetRes();
            positionFOCGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:GetPositionFOC"), positionFOCGetReq, typeof(PositionFOCGetRes), ticket);
            return positionFOCGetRes;
        }

        public async Task<PositionFOCSetRes> SetPostionFOC(PositionFOCSetReq positionFOCsSetReq, string ticket)
        {
            PositionFOCSetRes positionFOCSetRes = new PositionFOCSetRes();
            positionFOCSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:SetPositionFOC"), positionFOCsSetReq, typeof(PositionFOCSetRes), ticket);
            return positionFOCSetRes;
        }
        #endregion

        #region Tour Entities
        public async Task<TourEntitiesGetRes> GetDynamicTourEntities(TourEntitiesGetReq tourEntitiesGetReq, string ticket)
        {
            TourEntitiesGetRes tourEntitiesGetRes = new TourEntitiesGetRes();
            tourEntitiesGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:GetDynamicTourEntities"), tourEntitiesGetReq, typeof(TourEntitiesGetRes), ticket);
            return tourEntitiesGetRes;
        }

        public async Task<TourEntitiesGetRes> GetTourEntities(TourEntitiesGetReq tourEntitiesGetReq, string ticket)
        {
            TourEntitiesGetRes tourEntitiesGetRes = new TourEntitiesGetRes();
            tourEntitiesGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceTourEntity:GetTourEntities"), tourEntitiesGetReq, typeof(TourEntitiesGetRes), ticket);
            return tourEntitiesGetRes;
        }

        public async Task<TourEntitiesSetRes> SetTourEntities(TourEntitiesSetReq tourEntitiesSetReq, string ticket)
        {
            TourEntitiesSetRes tourEntitiesSetRes = new TourEntitiesSetRes();
            tourEntitiesSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceTourEntity:SetTourEntities"), tourEntitiesSetReq, typeof(TourEntitiesSetRes), ticket);
            return tourEntitiesSetRes;
        }
        #endregion

        #region Get Set Position
        public async Task<PosQuicePickGetRes> GetQuickPickActivities(PositionGetReq QuickPickGetReq, string ticket)
        {
            PosQuicePickGetRes QuickPickGetRes = new PosQuicePickGetRes();
            QuickPickGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:GetQuickPickActivities"), QuickPickGetReq, typeof(PosQuicePickGetRes), ticket);
            return QuickPickGetRes;
        }
        #endregion

        #region QRF Meal details & Position Def MealPlan
        public async Task<MealGetRes> GetMeals(MealGetReq objMealGetReq, string ticket)
        {
            MealGetRes objMealGetRes = new MealGetRes();
            objMealGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMeal:GetMeals"), objMealGetReq, typeof(MealGetRes), ticket);
            return objMealGetRes;
        }

        public async Task<MealSetRes> SetMeals(MealSetReq objMealSetReq, string ticket)
        {
            MealSetRes objPaxSetRes = new MealSetRes();
            objPaxSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMeal:SetMeals"), objMealSetReq, typeof(MealSetRes), ticket);
            return objPaxSetRes;
        }

        public async Task<PositionDefMealSetRes> SetDefaultMealPlan(PositionDefMealSetReq objPositionDefMealSetReq, string ticket)
        {
            PositionDefMealSetRes objPositionDefMealSetRes = new PositionDefMealSetRes();
            objPositionDefMealSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:SetDefaultMealPlan"), objPositionDefMealSetReq, typeof(PositionDefMealSetRes), ticket);
            return objPositionDefMealSetRes;
        }
        #endregion

        #region PositionRoomDetails
        public async Task<PositionRoomsGetRes> GetPositionRoomDetails(PositionRoomsGetReq positionRoomsGetReq, string ticket)
        {
            PositionRoomsGetRes positionRoomsGetRes = new PositionRoomsGetRes();
            positionRoomsGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:GetPositionRoomDetails"), positionRoomsGetReq, typeof(PositionRoomsGetRes), ticket);
            return positionRoomsGetRes;
        }

        public async Task<PositionRoomsSetRes> SetPositionRoomDetails(PositionRoomsSetReq positionRoomsSetReq, string ticket)
        {
            PositionRoomsSetRes positionRoomsSetRes = new PositionRoomsSetRes();
            positionRoomsSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePosition:SetPositionRoomDetails"), positionRoomsSetReq, typeof(PositionRoomsSetRes), ticket);
            return positionRoomsSetRes;
        }
        #endregion
    }
}

