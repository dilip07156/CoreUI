using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;
using System.Linq;
using System.IO;

namespace Voyager.App.Mappers
{
    public class CommunicationTrailMapping
    {
        #region Declaration 
        private readonly IConfiguration _configuration;
        CommunicationTrailProviders communictaionTrailProviders;
        public COCommonLibrary cOCommonLibrary;
        #endregion

        public CommunicationTrailMapping(IConfiguration configuration)
        {
            _configuration = configuration;
            cOCommonLibrary = new COCommonLibrary(configuration);
            communictaionTrailProviders = new CommunicationTrailProviders(_configuration);
        }

        #region Communication Trail
        public CommunicationTrailViewModel GetCommunicationTrail(DocumentStoreGetReq request, string token)
        {
            CommunicationTrailViewModel model = new CommunicationTrailViewModel();
            model.DocumentStoreInfoGetRes = new DocumentStoreInfoGetRes();
            try
            {
                model.DocumentStoreInfoGetRes = communictaionTrailProviders.GetCommunicationTrail(request, token)?.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return model;
        }

        public DocumentStoreInfo GetCommunicationTrailById(DocumentStoreGetReq request, string token)
        {
            DocumentStoreInfo objDocumentStoreInfo = new DocumentStoreInfo() { };
            objDocumentStoreInfo = communictaionTrailProviders.GetCommunicationTrailById(request, token).Result;
            if (objDocumentStoreInfo == null)
            {
                objDocumentStoreInfo = new DocumentStoreInfo() { ResponseStatus = new ResponseStatus() { Status = "Error", ErrorMessage = "Details not found." } };
            }
            else
            {
                var files = objDocumentStoreInfo.DocumentPath.Split(',');
                if (files != null && files.Count() > 0)
                {
                    for (int i = 0; i < files.Count(); i++)
                    {
                        files[i] =System.IO.Path.GetFileName(files[i]);
                    }

                    objDocumentStoreInfo.DocumentPath = string.Join(',', files);
                }
            }
            return objDocumentStoreInfo;
        }

        //public MailSetRes SetEmailDetails(EmailDetail request, string usernamee, string token)
        //{
        //    MailSetRes objMailSetRes = new MailSetRes();
        //    try
        //    {
        //        MailSetReq objMailSetReq = new MailSetReq();
        //        objMailSetReq.EmailDetails = new EmailDetails();
        //        objMailSetReq.EmailDetails.BCC = request.BCC.Split(";").ToList();
        //        objMailSetReq.EmailDetails.CC = request.CC.Split(";").ToList();
        //        objMailSetReq.EmailDetails.EditUser = usernamee;
        //        objMailSetReq.EmailDetails.EmailHtml = request.EmailHtml;
        //        objMailSetReq.EmailDetails.From = request.From;
        //        objMailSetReq.EmailDetails.MailSent = objMailSetReq.EmailDetails.MailSent;
        //        objMailSetReq.EmailDetails.MailSentBy = objMailSetReq.EmailDetails.MailSentBy;
        //        if (!string.IsNullOrEmpty(request.Time))
        //        {
        //            var time = request.Time.Split(":");
        //            request.MailSentOn = request.MailSentOn.AddHours(Convert.ToDouble(time[0])).AddMinutes(Convert.ToDouble(time[1]));
        //        }

        //        objMailSetReq.EmailDetails.MailSentOn = request.MailSentOn;
        //        objMailSetReq.EmailDetails.MailStatus = request.MailStatus;
        //        objMailSetReq.EmailDetails.MailType = request.MailType;
        //        objMailSetReq.EmailDetails.Remarks = request.Remarks;
        //        objMailSetReq.EmailDetails.Subject = request.Subject;
        //        objMailSetReq.EmailDetails.To = request.To;

        //        objMailSetReq.QRFID = request.QRFID;
        //        objMailSetReq.QRFPriceID = request.QRFPriceID;

        //        objMailSetRes = agentApprovalProviders.SetEmailDetails(objMailSetReq, token).Result;
        //        if (objMailSetRes != null && objMailSetRes.ResponseStatus != null && !string.IsNullOrEmpty(objMailSetRes.ResponseStatus.Status))
        //        {
        //        }
        //        else
        //        {
        //            objMailSetRes.ResponseStatus.Status = "Error";
        //            objMailSetRes.ResponseStatus.ErrorMessage = "An Error Occurs";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objMailSetRes.ResponseStatus.Status = "Error";
        //        objMailSetRes.ResponseStatus.ErrorMessage = ex.Message;
        //    }
        //    return objMailSetRes;
        //}
        #endregion 
    }
}