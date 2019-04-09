using System;
using System.Collections.Generic;
using System.Text;

namespace office365.PORTAL.Core
{
    public class PORTALUserObjectList
    {
        public string odatacontext { get; set; }
        public PORTALUserObject[] Value { get; set; }
    }
}
