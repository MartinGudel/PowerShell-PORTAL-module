﻿using System;
    using System.Management.Automation;
    
    namespace office365.PORTAL.Core
    {
        [Cmdlet("Remove", "PORTALServicePrincipalCredential")]
        public class RemovePORTALServicePrincipalCredential : PSCmdlet
        {
            [Parameter(Position=1)]
            public string Message { get; set; } = string.Empty;
    
            protected override void EndProcessing()
            {
                this.WriteObject($"not implemented yet. :(");
                base.EndProcessing();
            }
        }
    }
    
