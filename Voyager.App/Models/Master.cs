using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.Models
{
    public class PipelineStages
    {
        public string AttributeValue_Id { get; set; }
        public string Value { get; set; }
    }

    public class Priority
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class Date
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class QuoteResult
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class Month
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class Year
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class Status
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
     
    public class AutoCompleteTextBox
    {
        public string label { get; set; }
        public string value { get; set; }
        public string type { get; set; }
        public string typeId { get; set; }
        public string starrating { get; set; }
        public string location { get; set; }
        public string categoryid { get; set; }
        public string category { get; set; }
        public string chainid { get; set; }
        public string chain { get; set; }
        public bool? placeholder { get; set; }
        public string code { get; set; }
    }

    public class JsonList
    {
        public List<ProductAttributeDetails> list { get; set; }
    }

    public class AttributeValuesWithSquence
    {
        public string Value { get; set; }
        public string AttributeValue_Id { get; set; }
        public int SequenceNo { get; set; }
    }
}
