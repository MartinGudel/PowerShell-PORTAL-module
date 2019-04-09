using System;
    using System.Management.Automation;
    
    namespace office365.PORTAL.Core
    {
        [Cmdlet("Get", "PORTALAdministrativeUnitMember")]
        public class GetPORTALAdministrativeUnitMember : PSCmdlet
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
    
