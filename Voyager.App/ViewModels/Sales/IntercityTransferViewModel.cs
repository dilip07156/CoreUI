using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class IntercityTransferViewModel
    {
        public string QRFID { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public List<IntercityTransferDetails> IntercityTransferDetails { get; set; }

        public IntercityTransferViewModel()
        {
            MenuViewModel = new MenuViewModel { MenuName = "IntercityTransfer" };
            IntercityTransferDetails = new List<IntercityTransferDetails>();
        }
    }

    public class IntercityTransferDetails
    {
        public long IntercityTransferId { get; set; }
        

        public bool IsDeleted { get; set; }
    }
}
