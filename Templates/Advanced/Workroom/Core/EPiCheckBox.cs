#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace EPiServer.Templates.Advanced.Workroom.Core
{
    /// <summary>
    /// Check Box contorl with ability to rise Commands in Repeater
    /// </summary>
    public class EPiCheckBox : CheckBox
    {
        public string CommandName
        {
            get { return (this.ViewState["CommandName"] ?? string.Empty) as string;}
            set { this.ViewState["CommandName"] = value; }
        }

        public string CommandArgument
        {
            get { return (this.ViewState["CommandArgument"] ?? string.Empty) as string;}
            set { this.ViewState["CommandArgument"] = value;}
        }

        protected override void OnCheckedChanged(EventArgs e)
        {

            CommandEventArgs commandEventArgs = new CommandEventArgs(this.CommandName, this.CommandArgument);
            base.OnCheckedChanged(e);
            base.RaiseBubbleEvent(this, commandEventArgs);
        }
    }

}
