using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
	public class AgentSearchViewModel
	{
		public string AgentId { get; set; }
		public string AgentName { get; set; }
		public string CNKReferenceNo { get; set; }

		public Guid? Country { get; set; }
		public string AgentCountry { get; set; }
		public List<Attributes> CountryList { get; set; }

		public Guid? City { get; set; }
		public List<Attributes> CityList { get; set; }

		public string Status { get; set; }
		public List<Attributes> StatusList { get; set; }

		public string ProductType { get; set; }
		public List<mProductType> ProductTypeList { get; set; }

		public bool IsSupplier { get; set; }

		public AgentPipelineViewModel AgentPipeline { get; set; }

		public AgentDetails AgentDetails { get; set; }

		public ContactAndStaffDetails ContactAndStaffDetails { get; set; }

		public string ContactId { get; set; }

		public AgentSearchViewModel()
		{
			StatusList = new List<Attributes>();
			CountryList = new List<Attributes>();
			CityList = new List<Attributes>();
			AgentPipeline = new AgentPipelineViewModel();
			ProductTypeList = new List<mProductType>();
			AgentDetails = new AgentDetails();
			ContactAndStaffDetails = new ContactAndStaffDetails();
		}
	}

	public class AgentPipelineViewModel
	{
		public List<AgentList> AgentList { get; set; }
		public bool IsSupplier { get; set; } = false;

		public AgentPipelineViewModel()
		{
			AgentList = new List<AgentList>();
		}
	}

	public class AgentDetails
	{
		public string CompanyId { get; set; }
		public string CompanyName { get; set; }
		public string CompanyCode { get; set; } 
        public string Street { get; set; }
		public string Street2 { get; set; }
		public string Street3 { get; set; }
		public string CountryId { get; set; }
		public string CountryName { get; set; }
		public List<Attributes> CountryList { get; set; }
		public string CityId { get; set; }
		public string CityName { get; set; }
		public List<Attributes> CityList { get; set; }
		public string Zipcode { get; set; }
		public string SalesOffices { get; set; }
		public string SalesOfficesName { get; set; }
		public List<ChildrenCompanies> SalesOfficesList { get; set; }
		public bool Issubagent { get; set; }
		public bool Issupplier { get; set; }
		public bool ISUSER { get; set; } = false;
		public string ContactBy { get; set; }
		public bool B2C { get; set; } = false;
		public string LogoFilePath { get; set; }
		public List<Branches> Branches { get; set; }
		public bool IsForSalesAgent { get; set; } = false;
		public string ProductSupplierId { get; set; }
		public string Status { get; set; }
		public string Application { get; set; }
		public List<Attributes> ApplicationList { get; set; }
		public List<SupplierMappings> SupplierMappingList { get; set; }
		public string CreateUser { get; set; }
		public string CreateDate { get; set; }
		public string EditUser { get; set; }
		public string EditDate { get; set; }
		public string PageName { get; set; }
		public int Index { get; set; }

		public AgentDetails()
		{
			CountryList = new List<Attributes>();
			CityList = new List<Attributes>();
			SalesOfficesList = new List<ChildrenCompanies>();
			Branches = new List<Branches>();
			ApplicationList = new List<Attributes>();
			SupplierMappingList = new List<SupplierMappings>();
		}
	}

	public class Branches
	{
		public string ProductSupplierId { get; set; }
		public bool IsForSalesAgent { get; set; }
		public bool Issupplier { get; set; }
		public List<string> existingCompanies { get; set; }
		public string ParentCompanyId { get; set; }
		public string BranchCompanyId { get; set; }
		public string BranchCompanyCode { get; set; }
		public string BranchCompanyName { get; set; }
		public string SalesAgentCompanyId { get; set; }
	}

	public class Contacts
	{
		public string Company_Id { get; set; }
		public string Company_Name { get; set; }
		public string ContactDetailId { get; set; }
		public List<ContactAndStaffDetails> CommonContacts { get; set; }
		public List<ContactAndStaffDetails> SharedContacts { get; set; }
		public List<ContactAndStaffDetails> EmergencyContacts { get; set; }
		public string CompanyName { get; set; }
		public List<ChildrenCompanies> BranchesList { get; set; }
		public string ContactId { get; set; }
		public string defaultContactId { get; set; }
		public string ContactName { get; set; }
		public List<Attributes> ContactsList { get; set; }
		public string Level { get; set; }
		public string Status { get; set; }
		public List<mStatus> StatusList { get; set; }

		public Contacts()
		{
			CommonContacts = new List<ContactAndStaffDetails>();
			SharedContacts = new List<ContactAndStaffDetails>();
			EmergencyContacts = new List<ContactAndStaffDetails>();
			BranchesList = new List<ChildrenCompanies>();
			ContactsList = new List<Attributes>();
			StatusList = new List<mStatus>();
		}
	}

	public class ContactAndStaffDetails
	{
		public bool IsUserExists { get; set; } = false;
		public string User_Id { get; set; }
		public string Contact_Id { get; set; }
		public string Company_Id { get; set; }
		public string Company_Name { get; set; }
		public bool Default { get; set; } = false;
		public string ActualContact_Id_AsShared { get; set; }
		public string ActualCompany_Name_AsShared { get; set; }
		public string CommonTitle { get; set; }
		public string TITLE { get; set; }
		public string FIRSTNAME { get; set; }
		public string LastNAME { get; set; }
		public string TEL { get; set; }
		public string MOBILE { get; set; }
		//public string FAX { get; set; }
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string MAIL { get; set; }
		public string WEB { get; set; }
		public string Country_Id { get; set; }
		public string Country { get; set; }
		public string EmergencyNo { get; set; }
		public string ContactName { get; set; }
		//public string ContactMail { get; set; }
		//public string ContactTel { get; set; }
		public string EmergencyContact_Id { get; set; }
		public string BusiType { get; set; }
		public string Department { get; set; }
		public string Status { get; set; }
		public string StartPageId { get; set; }
		public string StartPage { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
		public List<mStatus> StatusList { get; set; }
		public List<mDefStartPage> StartPageList { get; set; }
		public List<Attributes> CountryList { get; set; }
		public List<Attributes> ContactList { get; set; }
		public List<UserRolesDetails> UserRolesDetails { get; set; }
		public bool IsCentralEmail { get; set; }
        public TargetGetRes TargetGetRes { get; set; }

		public ContactAndStaffDetails()
		{
			StatusList = new List<mStatus>();
			StartPageList = new List<mDefStartPage>();
			CountryList = new List<Attributes>();
			ContactList = new List<Attributes>();
			UserRolesDetails = new List<UserRolesDetails>();
            TargetGetRes = new TargetGetRes();
        }
	}

	public class CommercialsAndPayment
	{
		public string Company_Id { get; set; }
		public bool IsSupplier { get; set; } = false;

		public string Markup_ForGroup { get; set; }
		public string Group { get; set; }
		public List<CompanyMarkup> GroupsBasisList { get; set; }

		public string Markup_ForFIT { get; set; }
		public string FIT { get; set; }
		public List<CompanyMarkup> FITBasisList { get; set; }

		public string Markup_ForSeries { get; set; }
		public string Series { get; set; }
		public List<CompanyMarkup> SeriesBasisList { get; set; }

		public string Markup_ForB2B2B { get; set; }
		public string B2B2B { get; set; }
		public List<CompanyMarkup> B2B2BBasisList { get; set; }

		public string MarkupSchemesGroupsType { get; set; }
		public string GroupsTypeText { get; set; }
		public List<Attributes> GroupsTypeList { get; set; }

		public string MarkupSchemesFITType { get; set; }
		public string FITTypeText { get; set; }
		public List<Attributes> FITTypeList { get; set; }

		public string MarkupSchemesSeriesType { get; set; }
		public string SeriesTypeText { get; set; }
		public List<Attributes> SeriesTypeList { get; set; }

		public string MarkupSchemesB2B2BType { get; set; }
		public string B2B2BTypeText { get; set; }
		public List<Attributes> B2B2BTypeList { get; set; }

		public decimal? GroupsValue { get; set; }
		public decimal? FITValue { get; set; }
		public decimal? SeriesValue { get; set; }
		public decimal? B2B2BValue { get; set; }

		public List<PaymentTermsDetails> PaymentTermsDetails { get; set; }

        public List<TaxRegDetails> TaxRegDetails { get; set; }

        public string FinanceContactId { get; set; }
        public string FinanceContact { get; set; }
        public List<Attributes> FinanceContactList { get; set; }

		public string AccountName { get; set; }
		public string AccountNo { get; set; }
		public string BankName { get; set; }
		public string BankAddr { get; set; }
		public string SortCode { get; set; }
		public string IBAN { get; set; }
		public string Swift { get; set; }
		public string FinanceNote { get; set; }

        public CommercialsAndPayment()
        {
            GroupsBasisList = new List<CompanyMarkup>();
            FITBasisList = new List<CompanyMarkup>();
            SeriesBasisList = new List<CompanyMarkup>();
            B2B2BBasisList = new List<CompanyMarkup>();
            GroupsTypeList = new List<Attributes>();
            FITTypeList = new List<Attributes>();
            SeriesTypeList = new List<Attributes>();
            B2B2BTypeList = new List<Attributes>();
            PaymentTermsDetails = new List<PaymentTermsDetails>();
            FinanceContactList = new List<Attributes>();
            TaxRegDetails = new List<TaxRegDetails>();

        }
    }

	public class PaymentTermsDetails
	{
		public string PaymentTerms_Id { get; set; }
		public string Company_Id { get; set; }

		public string From { get; set; }

		public int? Days { get; set; }

		public string ValueType { get; set; }

		public double? Value { get; set; }
		public string Currency_Id { get; set; }

		public string Currency { get; set; }
		public List<Currency> CurrencyList { get; set; }

		public bool? VoucherReleased { get; set; }
		public DateTime? Crea_Dt { get; set; }
		public string Crea_Us { get; set; }
		public DateTime? Modi_Dt { get; set; }
		public string Modi_Us { get; set; }
		public string STATUS { get; set; }

		public string BusiType { get; set; }

        public PaymentTermsDetails()
        {
            CurrencyList = new List<Currency>();
        }
    }

    public class TaxRegDetails
    {
        public string TaxReg_Id { get; set; }
        public string Type { get; set; }
        public string State_Id { get; set; }
        public string State { get; set; }
        public string Number { get; set; }
        public string TaxStatus { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? EditDate { get; set; }
        public string EditUser { get; set; }
        public string Company_Id { get; set; }
        public string Type_Id { get; set; }
        public string TaxStatus_Id { get; set; }
        public List<AttributeValues> TaxRegStatus { get; set; }
        public List<AttributeValues> TaxRegType { get; set; }
        public List<Attributes> States { get; set; }
        public TaxRegDetails()
        {
            TaxRegStatus = new List<AttributeValues>();
            TaxRegType = new List<AttributeValues>();
            States = new List<Attributes>();
        }
    }
   

    public class TermsAndConditionsDetails
    {
        public string Company_Id { get; set; }

		public List<ConditionsAndPaymentDetails> CompanyTerms { get; set; }
		public List<ConditionsAndPaymentDetails> PaymentDetails { get; set; }

		public string Condition { get; set; }
		public string Details { get; set; }

		public TermsAndConditionsDetails()
		{
			CompanyTerms = new List<ConditionsAndPaymentDetails>();
			PaymentDetails = new List<ConditionsAndPaymentDetails>();
		}
	}

	public class ConditionsAndPaymentDetails
	{
		public string TermsAndConditions_Id { get; set; }
		public string PaymentDetails_Id { get; set; }

		public string Company_Id { get; set; }
		public string DocumentType { get; set; }
		public int? OrderNr { get; set; }
		public string BusinessType { get; set; }
		public string Section { get; set; }
		public string SubSection { get; set; }
		public string TermsDescription { get; set; }
		public string PaymentDetails { get; set; }
		public string Currency_Id { get; set; }
		public string Currency { get; set; }

		public List<Attributes> DocumentTypeList { get; set; }
		public List<Attributes> SectionList { get; set; }
		public List<Attributes> SubSectionList { get; set; }
		public List<Currency> CurrencyList { get; set; }

		public string CreateUser { get; set; }
		public DateTime CreateDate { get; set; }
		public string EditUser { get; set; }
		public DateTime? EditDate { get; set; }

		public ConditionsAndPaymentDetails()
		{
			DocumentTypeList = new List<Attributes>();
			SectionList = new List<Attributes>();
			SubSectionList = new List<Attributes>();
			CurrencyList = new List<Currency>();
		}
	}

	public class NewAgent
	{
		public string CompanyId { get; set; }
		public string ContactId { get; set; }
		public string CompanyCode { get; set; }
		public string AgentName { get; set; }
		public string Street { get; set; }
		public string Street2 { get; set; }
		public string Zipcode { get; set; }
		public string CountryId { get; set; }
		public string CountryName { get; set; }
		public List<Attributes> CountryList { get; set; }
		public string CityId { get; set; }
		public string CityName { get; set; }
		public List<Attributes> CityList { get; set; }
		public string CommonTitle { get; set; }
		public string FIRSTNAME { get; set; }
		public string LastNAME { get; set; }
		public string TEL { get; set; }
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string MAIL { get; set; }
		public string ContactBy { get; set; }
		public string StartPageId { get; set; }
		public string StartPage { get; set; }
		public List<mDefStartPage> StartPageList { get; set; }
		public bool IsSupplier { get; set; } = false;

		public NewAgent()
		{
			CountryList = new List<Attributes>();
			CityList = new List<Attributes>();
			StartPageList = new List<mDefStartPage>();
		}
	}

	public class SupplierMappings
	{
		public string Application_Id { get; set; }
		public string Application { get; set; }
		public string PartnerEntityCode { get; set; }
		public string PartnerEntityName { get; set; }
		public bool IsDeleted { get; set; }
		public string CreateUser { get; set; }
		public DateTime? CreateDate { get; set; }
	}

    public class TargetCompany
    {
        public string CompanyId { get; set; }
        public string ContactId { get; set; }
        public Targets Targets { get; set; }
    }
}
