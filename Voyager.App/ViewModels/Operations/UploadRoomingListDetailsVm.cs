using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels.Operations
{
    public class UploadRoomingListDetailsVm
    {
        public UploadRoomingListDetailsVm()
        {
            Response = new ResponseStatus();
            DisplayRoomingData = new List<ViewRoomingListCompare>();
            PassengerDetailsFromExcel = new List<PersonalDetailsTobeSaved>();
            SuppliersDetails = new List<SupplierDetails>();
        }
        public ResponseStatus Response { get; set; }

        public string BookingNumber { get; set; }

      public List<ViewRoomingListCompare> DisplayRoomingData { get; set; }
        public List<PersonalDetailsTobeSaved> PassengerDetailsFromExcel { get; set; }
        public List<SupplierDetails> SuppliersDetails { get; set; }

        public ResponseStatus UploadUserFile(IFormFile file)
        {
            try
            {
                var supportedTypes = new[] {  "xls", "xlsx" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    Response.StatusMessage = "Rooming List is not an spreadsheet in format .xls or .xlsx";
                    Response.Status = "Failure";
                    return Response;
                }
                else
                {
                    Response.StatusMessage = "Rooming List is in proper  format .xls or .xlsx";
                    Response.Status = "Success";
                    return Response;
                }
            }
            catch (Exception ex)
            {
                Response.StatusMessage = ex.Message.ToString();
                Response.Status = "Failure";
                return Response;
            }
        }
    }
}
