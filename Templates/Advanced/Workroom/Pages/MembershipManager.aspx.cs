#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
#region Known Limitations
/*
The template has the following known limitations:

- Performance Limitations
  Due to the fact that workroom membership is stored with ACLs on EPiServer pages, 
  it's not recommended to have excessive number of members in a single workroom.
*/
#endregion


using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using EPiServer.Templates.Advanced.Workroom.Core;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    /// <summary>
    /// Page template for adding, removing or changing workroom membership.
    /// </summary>
    public partial class MembershipManager : WorkroomPageBase
    { 
    }
}
