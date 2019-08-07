using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using Voyager.App.Proxy;
using Voyager.App.ViewModels;

namespace Voyager.App.Providers
{
    public class MealProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers

        public MealProviders()
        {
        }

        public MealProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion

        #region Meals
        public async Task<MealsGetRes> GetMealsDetailsByQRFID(QuoteGetReq objQuoteGetReq, string ticket)
        {
            MealsGetRes objMealsGetRes = new MealsGetRes();
            objMealsGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMeals:GetMealsDetailsByQRFID"), objQuoteGetReq, typeof(MealsGetRes), ticket);
            return objMealsGetRes;
        }

        public async Task<MealSetRes> SetMealDetailsByID(MealSetReq objMealSetReq, string ticket)
        {
            MealSetRes objMealSetRes = new MealSetRes();
            objMealSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMeals:SetMealDetailsByID"), objMealSetReq, typeof(MealSetRes), ticket);
            return objMealSetRes;
        }

        public async Task<MealVenueSetRes> SetMealVenueDetailsByID(MealVenueSetReq objMealVenueSetReq, string ticket)
        {
            MealVenueSetRes objMealVenueSetRes = new MealVenueSetRes();
            objMealVenueSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMeals:SetMealVenueDetailsByID"), objMealVenueSetReq, typeof(MealVenueSetRes), ticket);
            return objMealVenueSetRes;
        }

        public async Task<MealVenueGetRes> GetMealVenueDetailsByID(MealVenueGetReq objMealVenueGetReq, string ticket)
        {
            MealVenueGetRes objMealVenueGetRes = new MealVenueGetRes();
            objMealVenueGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMeals:GetMealVenueDetailsByID"), objMealVenueGetReq, typeof(MealVenueGetRes), ticket);
            return objMealVenueGetRes;
        }
        #endregion
    }
}
