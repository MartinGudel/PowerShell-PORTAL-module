using System;
using System.Management.Automation;

namespace office365.PORTAL.Core
{
  public class WellKnownOpenIDConfiguration
    {
        public string tenant_discovery_endpoint { get; set; }
        public string apiversion { get; set; }
        public Metadata[] metadata { get; set; }
    }

    public class Metadata
    {
        public string preferred_network { get; set; }
        public string preferred_cache { get; set; }
        public string[] aliases { get; set; }
    }


}

