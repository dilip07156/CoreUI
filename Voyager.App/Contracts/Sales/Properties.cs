using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voyager.App.Contracts
{
    public class Properties
    {
        public string PropertyName { get; set; }
        public string Property_Id { get; set; }
        public List<Attributes> Attribute;
    }
}
