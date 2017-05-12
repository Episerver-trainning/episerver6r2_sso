#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System.ComponentModel;
using System.Web.UI;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    /// <summary>
    /// Simple integration of TinyMCE editor in Alloy Tech Templates
    /// </summary>
    [DefaultProperty("Text"), ToolboxData("<{0}:Editor runat=server></{0}:Editor>"), ValidationProperty("Text")]
    public partial class TinyMCETextEditor : UserControlBase
    {
        /// <summary>
        /// A text in the editor
        /// </summary>
        public string Text
        {
            get
            {
                return TextBoxContent.Text;
            }
            set
            {
                TextBoxContent.Text = value;
            }
        }

        /// <summary>
        /// A path to a CSS file with styles for the content
        /// </summary>
        public string ContentCss { get; set; }

        /// <summary>
        /// The editor width
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// The editor height
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// If the editor has the advanced toolbar
        /// </summary>
        public bool Advanced { get; set; }

        /// <summary>
        /// Tools configuration string
        /// </summary>
        protected string ToolBar
        {
            get
            {
                return Advanced
                    ? "bold,italic,|,undo,redo,|,numlist,bullist,|,link,unlink,image,table,|,formatselect,|,blockquote"
                    : "bold,italic,|,undo,redo,|,numlist,bullist,|,link,unlink";
            }
        }

        /// <summary>
        /// Initialize the editor with default values
        /// </summary>
        public TinyMCETextEditor()
        {
            ContentCss = "~/Templates/AlloyTech/Styles/Default/editor.css";
            Width = "100%";
            Height = "250px";
            Advanced = false;
        }
    }
}