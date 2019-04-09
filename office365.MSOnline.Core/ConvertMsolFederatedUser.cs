using System;
    using System.Management.Automation;
    
    namespace office365.PORTAL.Core
    {
        [Cmdlet("Convert", "PORTALFederatedUser")]
        public class ConvertPORTALFederatedUser : PSCmdlet
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
    
