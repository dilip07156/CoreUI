using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class AttachToMasterViewModel
    {
        public COHeaderViewModel COHeaderViewModel { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public GoAheadGetRes GoAheadGetRes { get; set; }             

        public ResponseStatus ResponseStatus { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string QRFPriceId { get; set; }

        public int VersionId { get; set; }

        public string ConfirmationDT { get; set; }

        public string Type { get; set; }      

        public long DepartureId { get; set; } 
        
        public List<AttributeValues> UserList { get; set; } = new List<AttributeValues>(); 

        public AttachToMasterViewModel()
        {
            COHeaderViewModel = new COHeaderViewModel();
            MenuViewModel = new MenuViewModel();
            GoAheadGetRes = new GoAheadGetRes();
        }
    }

    public class Materialisation
    {
        public long DepartureId { get; set; }

        public string GoAheadId { get; set; }

        public string QRFID { get; set; }

        public string UserName { get; set; }

        public List<RoomInfo> RoomInfo { get; set; }

        public List<ChildInfo> ChildInfo { get; set; }

        [Required(ErrorMessage = "*")]
        public List<AttributeValues> ChildTypeList { get; set; } = new List<AttributeValues>();
        public string ChildTypeId { get; set; }
        public string ChildType { get; set; }

        [Required(ErrorMessage = "*")]
        public List<AttributeValues> ChildAgeList { get; set; } = new List<AttributeValues>(); 
    }

    public class RoomInfo
    {
        public string RoomTypeID { get; set; }

        public string RoomTypeName { get; set; }

        [Required(ErrorMessage = "*")]
        public int RoomCount { get; set; }

        [Required(ErrorMessage = "*")]
        public int PaxCount { get; set; }
    }

    public class ChildInfo
    {
        public string ChildInfoId { get; set; }

        public int SeqNo { get; set; }

        [Required(ErrorMessage = "*")]
        public string Type { get; set; }

        [Required(ErrorMessage = "*")]
        public int Number { get; set; }

        [Required(ErrorMessage = "*")]
        public int Age { get; set; }

        public bool IsDeleted { get; set; } = false;
        public string DeleteBy { get; set; }
    }  
}
