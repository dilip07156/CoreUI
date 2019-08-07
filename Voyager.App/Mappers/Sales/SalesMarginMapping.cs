using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Voyager.App.ViewModels;
using VGER_WAPI_CLASSES;

namespace Voyager.App.Mappers
{
    public class SalesMarginMapping
    {
        public QuoteMarginViewModel QRFMarginGet(MarginGetRes marginGetRes)
        {
            QuoteMarginViewModel quoteMarginViewModel = new QuoteMarginViewModel();
            quoteMarginViewModel.QRFID = marginGetRes.QRFID;
            quoteMarginViewModel.SelectedMargin = string.IsNullOrEmpty(marginGetRes.Margins.SelectedMargin) ? "Package" : marginGetRes.Margins.SelectedMargin;

            if (marginGetRes.ResponseStatus.Status.ToLower() == "success")
            {
                if (marginGetRes.Margins != null && marginGetRes.Margins.Package != null && marginGetRes.Margins.Package.PackageProperties != null && marginGetRes.Margins.Package.PackageProperties.Count > 0)
                {
                    foreach (var item in marginGetRes.Margins.Package.PackageProperties)
                    {
                        if (item.ComponentName.ToLower() == "Package not including Accommodation".ToLower())
                        {
                            quoteMarginViewModel.PackageMarginDetails.ExcMarginUnit = item.MarginUnit;
                            quoteMarginViewModel.PackageMarginDetails.ExcPackageID = item.PackageID;
                            quoteMarginViewModel.PackageMarginDetails.ExcSellingPrice = item.SellingPrice;
                            quoteMarginViewModel.PackageMarginDetails.Accommodation = "Package not including Accommodation";
                        }
                        else if (item.ComponentName.ToLower() == "Package including Accommodation".ToLower())
                        {
                            quoteMarginViewModel.PackageMarginDetails.IncMarginUnit = item.MarginUnit;
                            quoteMarginViewModel.PackageMarginDetails.IncPackageID = item.PackageID;
                            quoteMarginViewModel.PackageMarginDetails.IncSellingPrice = item.SellingPrice;
                            quoteMarginViewModel.PackageMarginDetails.Accommodation = "Package including Accommodation";
                        }
                        else if (item.ComponentName.ToLower() == "Suppliments".ToLower())
                        {
                            quoteMarginViewModel.PackageMarginDetails.SupSelected = true;
                            quoteMarginViewModel.PackageMarginDetails.SupMarginUnit = item.MarginUnit;
                            quoteMarginViewModel.PackageMarginDetails.SupPackageID = item.PackageID;
                            quoteMarginViewModel.PackageMarginDetails.SupSellingPrice = item.SellingPrice;
                            //quoteMarginViewModel.PackageMarginDetails.AdditionalAccommodation = "Suppliments";
                        }
                        else if (item.ComponentName.ToLower() == "Optionals".ToLower())
                        {
                            quoteMarginViewModel.PackageMarginDetails.OptSelected = true;
                            quoteMarginViewModel.PackageMarginDetails.OptMarginUnit = item.MarginUnit;
                            quoteMarginViewModel.PackageMarginDetails.OptPackageID = item.PackageID;
                            quoteMarginViewModel.PackageMarginDetails.OptSellingPrice = item.SellingPrice;
                            //quoteMarginViewModel.PackageMarginDetails.AdditionalAccommodation = "Optionals";
                        }
                    }
                }
                else
                {
                    quoteMarginViewModel.PackageMarginDetails.Accommodation = "Package including Accommodation";
                }
                quoteMarginViewModel.PackageMarginDetails.MarkupType = string.IsNullOrEmpty(marginGetRes.Margins.Package.MarginComputed.MarkupType) ? "Corporate Markup" : marginGetRes.Margins.Package.MarginComputed.MarkupType;
                quoteMarginViewModel.PackageMarginDetails.TotalCost = string.IsNullOrEmpty(marginGetRes.Margins.Package.MarginComputed.TotalCost) ? "Including" : marginGetRes.Margins.Package.MarginComputed.TotalCost;
                quoteMarginViewModel.PackageMarginDetails.TotalLeadersCost = string.IsNullOrEmpty(marginGetRes.Margins.Package.MarginComputed.TotalLeadersCost) ? "Including" : marginGetRes.Margins.Package.MarginComputed.TotalLeadersCost;
                quoteMarginViewModel.PackageMarginDetails.Upgrade = string.IsNullOrEmpty(marginGetRes.Margins.Package.MarginComputed.Upgrade) ? "Including" : marginGetRes.Margins.Package.MarginComputed.Upgrade;

                if (marginGetRes.Margins != null && marginGetRes.Margins.Product != null && marginGetRes.Margins.Product.ProductProperties != null)
                {
                    string ProdMaster = "";
                    foreach (var item in marginGetRes.Margins.Product.ProductProperties)
                    {
                        ViewModels.ProductProperties productProperties;
                        if (item.ProductMaster != ProdMaster)
                        {
                            productProperties = new ViewModels.ProductProperties();
                            productProperties.HowMany = "";
                            //productProperties.IsProdtype = (item.ProductID != "" && item.ProductID != null) ? true : false;
                            productProperties.SellingPrice = 0;
                            productProperties.MarginUnit = "";
                            productProperties.Prodtype = item.ProductMaster;
                            productProperties.ProductID = "";
                            productProperties.VoyagerProductType_Id = "";
                            productProperties.ProductMaster = item.ProductMaster;
                            productProperties.IsProduct = false;

                            quoteMarginViewModel.ProductMarginDetails.ProductProperties.Add(productProperties);
                        }

                        productProperties = new ViewModels.ProductProperties();
                        productProperties.HowMany = item.HowMany;
                        productProperties.IsProdtype = (item.ProductID != "" && item.ProductID != null) ? true : false;
                        productProperties.SellingPrice = item.SellingPrice;
                        productProperties.MarginUnit = item.MarginUnit;
                        productProperties.Prodtype = item.Prodtype;
                        productProperties.ProductID = item.ProductID;
                        productProperties.VoyagerProductType_Id = item.VoyagerProductType_Id;
                        productProperties.ProductMaster = item.ProductMaster;
                        productProperties.IsProduct = true;
                        quoteMarginViewModel.ProductMarginDetails.ProductProperties.Add(productProperties);
                        ProdMaster = item.ProductMaster;
                    }

                    var ItemList = quoteMarginViewModel.ProductMarginDetails.ProductProperties;
                    var Typelist = ItemList.Select(a => a.ProductMaster).Distinct().ToList();
                    foreach (var item in Typelist)
                    {
                        var TypeValue = ItemList.Where(a => a.ProductMaster == item && a.IsProduct == true).FirstOrDefault();
                        int TypeCount = ItemList.Where(a => a.ProductMaster == item && a.IsProduct == true).Count();
                        int TypeValueCount = ItemList.Where(a => a.ProductMaster == TypeValue.ProductMaster && a.SellingPrice == TypeValue.SellingPrice && a.MarginUnit == TypeValue.MarginUnit && a.IsProduct == true).Count();
                        if (TypeCount == TypeValueCount)
                        {
                            var TypeStdValue = ItemList.Where(a => a.ProductMaster == TypeValue.ProductMaster && a.IsProduct == false).FirstOrDefault();
                            TypeStdValue.IsProdtype = TypeValue.IsProdtype;
                            TypeStdValue.SellingPrice = TypeValue.SellingPrice;
                            TypeStdValue.MarginUnit = TypeValue.MarginUnit;
                        }
                    }

                    quoteMarginViewModel.ProductMarginDetails.MarkupType = marginGetRes.Margins.Product.MarginComputed.MarkupType;
                    quoteMarginViewModel.ProductMarginDetails.TotalCost = marginGetRes.Margins.Product.MarginComputed.TotalCost;
                    quoteMarginViewModel.ProductMarginDetails.TotalLeadersCost = marginGetRes.Margins.Product.MarginComputed.TotalLeadersCost;
                    quoteMarginViewModel.ProductMarginDetails.Upgrade = marginGetRes.Margins.Product.MarginComputed.Upgrade;
                }

                if (marginGetRes.Margins != null && marginGetRes.Margins.Itemwise != null && marginGetRes.Margins.Itemwise.ItemProperties != null)
                {
                    string ProdType = "";
                    foreach (var item in marginGetRes.Margins.Itemwise.ItemProperties)
                    {
                        if (item.Prodtype != ProdType)
                        {
                            quoteMarginViewModel.ItemwiseMarginDetails.ProductProperties.Add(new ViewModels.ProductProperties
                            {
                                IsPosition = false,
                                PositionID = "",
                                ProductName = item.Prodtype,
                                HowMany = "",
                                IsProdtype = false,
                                SellingPrice = 0,
                                MarginUnit = "",
                                Prodtype = item.Prodtype,
                                ProductID = "",
                                VoyagerProductType_Id = item.VoyagerProductType_Id
                            });
                        }
                        quoteMarginViewModel.ItemwiseMarginDetails.ProductProperties.Add(new ViewModels.ProductProperties
                        {
                            IsPosition = true,
                            PositionID = item.PositionID,
                            ProductName = item.ProductName,
                            HowMany = item.HowMany,
                            IsProdtype = (item.SellingPrice > 0) ? true : false,
                            SellingPrice = item.SellingPrice,
                            MarginUnit = item.MarginUnit,
                            Prodtype = item.Prodtype,
                            ProductID = item.ItemID,
                            VoyagerProductType_Id = item.VoyagerProductType_Id
                        });
                        ProdType = item.Prodtype;
                    }

                    var ItemList = quoteMarginViewModel.ItemwiseMarginDetails.ProductProperties;
                    var Typelist = ItemList.Select(a => a.Prodtype).Distinct().ToList();
                    foreach (var item in Typelist)
                    {
                        var TypeValue = ItemList.Where(a => a.Prodtype == item && a.IsPosition == true).FirstOrDefault();
                        int TypeCount = ItemList.Where(a => a.Prodtype == item && a.IsPosition == true).Count();
                        int TypeValueCount = ItemList.Where(a => a.Prodtype == TypeValue.Prodtype && a.SellingPrice == TypeValue.SellingPrice && a.MarginUnit == TypeValue.MarginUnit && a.IsPosition == true).Count();
                        if (TypeCount == TypeValueCount)
                        {
                            var TypeStdValue = ItemList.Where(a => a.Prodtype == TypeValue.Prodtype && a.IsPosition == false).FirstOrDefault();
                            TypeStdValue.IsProdtype = TypeValue.IsProdtype;
                            TypeStdValue.SellingPrice = TypeValue.SellingPrice;
                            TypeStdValue.MarginUnit = TypeValue.MarginUnit;
                        }
                    }

                    quoteMarginViewModel.ItemwiseMarginDetails.MarkupType = marginGetRes.Margins.Itemwise.MarginComputed.MarkupType;
                    quoteMarginViewModel.ItemwiseMarginDetails.TotalCost = marginGetRes.Margins.Itemwise.MarginComputed.TotalCost;
                    quoteMarginViewModel.ItemwiseMarginDetails.TotalLeadersCost = marginGetRes.Margins.Itemwise.MarginComputed.TotalLeadersCost;
                    quoteMarginViewModel.ItemwiseMarginDetails.Upgrade = marginGetRes.Margins.Itemwise.MarginComputed.Upgrade;
                }
            }

            return quoteMarginViewModel;
        }
    }
}
