using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionCreator.Models
{
    public class Component
    {
        public int ComponentNumber { get; set; }
        public string Size { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
