using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels.Settings
{
   public class MisArtifactsViewModel
    {
     public   MisArtifactsViewModel()
        {
            Roles = new List<RoleList>();
            RolesName = new List<string>();
            typeMaster = new List<TypeMaster>();
        }
        public string Item { get; set; }

        public string Id { get; set; }

        public string TypeId { get; set; }

        public string TypeName { get; set; }

        public string RoleID { get; set; }

        public String RoleName { get; set; }

        public List<TypeMaster> typeMaster { get; set; }

        public List<RoleList> Roles { get; set; }
        public List<string> RolesName { get; set; }

        public string TypeNameResponse { get; set; }
        public string ItemResponse { get; set; }
        public string RoleNameResponse { get; set; }


       
    }
    public class TypeMaster
    {
        public string TypeId { get; set; }
        public string TypeName { get; set; }

    }
    public class RoleList
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }

    }
}
