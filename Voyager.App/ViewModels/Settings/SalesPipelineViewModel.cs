using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
	public class SalesPipelineViewModel
	{
		public SalesPipelineViewModel()
		{
			RoleList = new List<AttributeValues>();
			DestinationList = new List<AttributeValues>();
			SalesOfficeList = new List<AttributeValues>();
			UserList = new List<AttributeValues>();
			RolesByDestinationList = new List<Values>();
			RolesBySalesOfficeList = new List<Values>();
		}
		
		public string Type { get; set; }

		[Required(ErrorMessage = "*")]
		public string Role { get; set; }
		public List<AttributeValues> RoleList { get; set; }

		[Required(ErrorMessage = "*")]
		public string Destination { get; set; }
		public List<AttributeValues> DestinationList { get; set; }

		[Required(ErrorMessage = "*")]
		public string SalesOffice { get; set; }
		public List<AttributeValues> SalesOfficeList { get; set; }

		[Required(ErrorMessage = "*")]
		public string User { get; set; }
		public List<AttributeValues> UserList { get; set; }

		public List<Values> RolesByDestinationList { get; set; }
		public List<Values> RolesBySalesOfficeList { get; set; }
	}

	//public class SPValues
	//{
	//	public string DestinationId { get; set; }
	//	public string DestinationName { get; set; }
	//	public string RoleId { get; set; }
	//	public string RoleName { get; set; }
	//	public string UserId { get; set; }
	//	public string UserName { get; set; }
	//}
}
