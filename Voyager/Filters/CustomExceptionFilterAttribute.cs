using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Voyager
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public CustomExceptionFilterAttribute(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            LogService(context.Exception.Message);

            if(context.HttpContext.Request.Cookies["JWTToken"] == "" || context.HttpContext.Request.Cookies["JWTToken"] == null)
            {
                string path = context.HttpContext.Request.Path.Value + context.HttpContext.Request.QueryString.Value;
                context.HttpContext.Response.Redirect("/Account/Login?returnUrl=" + path);
            }

            var result = new ViewResult { ViewName = "CustomError" };
            var modelMetadata = new EmptyModelMetadataProvider();
            result.ViewData = new ViewDataDictionary(modelMetadata, context.ModelState);
            result.ViewData.Add("HandleException", context.Exception.Message);
            if (context.HttpContext.Request.Headers["IsAjaxRequest"] == "true")
            {
                result.ViewData.Add("IsAjaxRequest", "true");
            }
            else
            {
                result.ViewData.Add("IsAjaxRequest", "false");
            }
            context.Result = result;
            context.ExceptionHandled = true;
        }

        private void LogService(string content)
        {
            string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ErrorLog", "ErrorLog_" + DateTime.Today.ToString("ddMMyyyy") + ".txt");
            if (!Directory.Exists(Path.GetDirectoryName(outputFilePath))) Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

            FileStream fs = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + " " + content + Environment.NewLine);
            sw.Flush();
            sw.Close();
        }
    }
}