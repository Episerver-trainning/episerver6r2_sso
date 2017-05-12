#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Web.UI;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    /// <summary>
    ///	Lists the pages that has been changed in the last x hours, 
    /// where x is the number of hours specified in the property "RecentHours".
    /// </summary>
    public partial class ChangedPages : UserControlBase
    {
        // "RecentContainer" (PropertyPageReference)[Optional, Default value=Start page for site] defines the root page for changed pages.

        // "RecentHours" (PropertyNumber)Required, No default value] Number of hours back in time to check for changed pages.
        private const string _recentHoursPropertyName = "RecentHours";

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PropertyCriteriaCollection pcc = new PropertyCriteriaCollection();
            PropertyCriteria pc = new PropertyCriteria
            {
                Name = "PageChanged",
                Value = DateTime.Now.Subtract(TimeSpan.FromHours(CorrectHoursPropertyValue())).ToString(),
                Condition = CompareCondition.GreaterThan,
                Type = PropertyDataType.Date
            };

            pcc.Add(pc);
            var pdc = DataFactory.Instance.FindPagesWithCriteria(CurrentPage["RecentContainer"] as PageReference ?? PageReference.StartPage, pcc);
            pagelist.DataSource = FilterForVisitor.Filter(pdc);
            pagelist.DataBind();
        }



        /// <summary>
        /// Corrects the hours property value based on a property on the currentpage
        /// </summary>
        /// <returns></returns>
        private int CorrectHoursPropertyValue()
        {
            DateTime minSqlValue = new DateTime(1900, 1, 1); // for SQL we count since 01 Jan 1900
            int hoursAllowed = (int)((DateTime.Now - minSqlValue).TotalHours);
            return (int)CurrentPage[_recentHoursPropertyName] > hoursAllowed ? hoursAllowed : (int)CurrentPage[_recentHoursPropertyName];
        }
    }
}