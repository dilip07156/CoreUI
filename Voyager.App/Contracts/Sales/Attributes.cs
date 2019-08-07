using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;

namespace Voyager.App.Contracts
{
    public class Attributes
    {
        public string AttributeName { get; set; }
        public string Attribute_Id { get; set; }
        public List<AttributeValues> Values;
    }
}
