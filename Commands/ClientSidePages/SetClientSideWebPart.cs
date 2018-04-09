﻿#if !ONPREMISES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Base.PipeBinds;

namespace SharePointPnP.PowerShell.Commands.ClientSidePages
{
    [Cmdlet(VerbsCommon.Set, "PnPClientSideWebPart")]
    [CmdletHelp("Set Client-Side Web Part properties",
        SupportedPlatform = CmdletSupportedPlatform.Online,
        DetailedDescription = "Sets specific client side webpart properties. Notice that the title parameter will only set the -internal- title of webpart. The title which is shown in the UI will, if possible, have to be set using the PropertiesJson parameter. Use Get-PnPClientSideComponent to retrieve the instance id and properties of a webpart.",
        Category = CmdletHelpCategory.WebParts)]
    [CmdletExample(
        Code = @"PS:> Set-PnPClientSideWebPart -Page Home -Identity a2875399-d6ff-43a0-96da-be6ae5875f82 -PropertiesJson $myproperties",
        Remarks = @"Sets the properties of the client side webpart given in the $myproperties variable.", SortOrder = 1)]
    public class SetClientSideWebPart : PnPWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "The name of the page")]
        public ClientSidePagePipeBind Page;

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The identity of the webpart. This can be the webpart instance id or the title of a webpart")]
        public ClientSideWebPartPipeBind Identity;

        [Parameter(Mandatory = false, ValueFromPipeline = true, HelpMessage = "Sets the internal title of the webpart. Notice that this will NOT set a visible title.")]
        public string Title;

        [Parameter(Mandatory = false, ValueFromPipeline = true, HelpMessage = "Sets the properties as a JSON string.")]
        public string PropertiesJson;

        protected override void ExecuteCmdlet()
        {
            var clientSidePage = Page.GetPage(ClientContext);

            if (clientSidePage == null)
                throw new Exception($"Page '{Page?.Name}' does not exist");

            var controls = Identity.GetWebPart(clientSidePage);
            if (controls.Any())
            {
                if (controls.Count > 1)
                {
                    throw new Exception("Found multiple webparts with the same name. Please use the InstanceId to retrieve the cmdlet.");
                }
                var webpart = controls.First();
                bool updated = false;

                if (MyInvocation.BoundParameters.ContainsKey("PropertiesJson"))
                {
                    webpart.PropertiesJson = PropertiesJson;
                    updated = true;
                }
                if (MyInvocation.BoundParameters.ContainsKey("Title"))
                {
                    webpart.Title = Title;
                    updated = true;
                }

                if (updated)
                {
                    clientSidePage.Save();
                }

            }
            else
            {
                throw new Exception($"WebPart does not exist");
            }
        }
    }
}
#endif
