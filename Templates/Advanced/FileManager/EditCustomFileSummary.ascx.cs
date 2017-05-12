#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web.Hosting;

using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Web.Hosting;
using EPiServer.XForms;
using EPiServer.XForms.Util;
using EPiServer.Web.WebControls;
using EPiServer.HtmlParsing;

namespace EPiServer.Templates.Advanced.FileManager
{
	/// <summary>
	///	This control belongs to the file manager, for editing file content for versioned files
    /// In order to view this control, right click on a versioned file and select 'Edit file summary' in the right click menu
	/// </summary>
    public partial class EditCustomFileSummary : ContentUserControlBase
	{
		private	XFormData _data;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UnifiedFile selectedFile = FileManager.SingleSelectedFile;

			// Init the XForm.
            SerializableXmlDocument rawFile = new SerializableXmlDocument();
            VirtualFile vf = HostingEnvironment.VirtualPathProvider.GetFile(selectedFile.Parent.CustomFileSummaryVirtualPath);
            using (Stream stream = vf.Open())
            {
                rawFile.Load(stream);
            }
			XForm form = new XForm();
			form.Document = rawFile;
			_data = form.CreateFormData();
			
            // Populate the XForm.
            IDictionary dict = selectedFile.Summary.Dictionary;			
            foreach(DictionaryEntry entry in dict)
			{
                if (entry.Value != null)
                {
                    _data.SetValue(entry.Key.ToString(), entry.Value.ToString());
                }
			}
			XFormCtrl.Data = _data;
			XFormCtrl.FormDefinition = form;

            if (!FileSystemUtility.CanEdit(selectedFile))
            {
                SaveButton.Enabled = false;
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
            }
		}

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void CancelButton_Click(object sender, EventArgs e)
		{
            FileManager.RaiseCommand(this, new FileManagerCommandEventArgs("LoadView", "Default"));
		}

        /// <summary>
        /// Handles the Click event of the SaveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveButton_Click(object sender, EventArgs e)
		{
            if (!Page.IsValid)
            {
                return;
            }
            UnifiedFile selectedFile = FileManager.SingleSelectedFile;
            try
            {
                FileSystemUtility.UpdateFileSummary(selectedFile, _data.GetFilteredValues(new HtmlFilter(new DefaultFilterRules())));
                FileManager.RaiseCommand(this, new FileManagerCommandEventArgs("LoadView", "Default"));
            }
            catch (FileIsCheckedOutException)
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
                return;
            }
            catch (Exception ex)
            {
                Page.Validators.Add(new StaticValidator(ex.Message));
                return;
            }                        
		}
	}
}