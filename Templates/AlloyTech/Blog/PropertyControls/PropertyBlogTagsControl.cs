#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Web.WebControls;
using System.Text.RegularExpressions;

namespace EPiServer.Templates.AlloyTech.Blog.PropertyControls
{
    /// <summary>
    /// The edit GUI class for PropertyBlogTags.
    /// </summary>
    public class PropertyBlogTagsControl : Web.PropertyControls.PropertyTextBoxControlBase
    {
        //private static readonly string _translationPropertyName = "Tags";
        private Panel _editControlContainer;

        /// <summary>
        /// Gets or sets the container for the EditControl.
        /// </summary>
        /// <value>The container for the EditControl .</value>
        public Panel EditControlContainer
        {
            get { return _editControlContainer; }
            set { _editControlContainer = value; }
        }

        /// <summary>
        /// Creates an edit interface for the property.
        /// </summary>
        public override void CreateEditControls()
        {
            EditControlContainer = new Panel();
            EditControlContainer.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
            EditControlContainer.Style.Add("float", "left");
            Controls.Add(EditControlContainer);

            EditControl = new TextBox();
            EditControl.ID = "EditControl";
            EditControl.MaxLength = 255;
            EditControl.Rows = 4;
            EditControl.TextMode = TextBoxMode.MultiLine;
            EditControl.Text = PropertyData.ToString();

            Label editControlLabel = new Label();
            editControlLabel.Style.Add(HtmlTextWriterStyle.Display, "block");
            editControlLabel.Text = LanguageManager.Instance.Translate("/blog/propertyblogtags/selectedtagslabel");
            editControlLabel.AssociatedControlID = EditControl.ID;

            EditControlContainer.Controls.Add(editControlLabel);
            EditControlContainer.Controls.Add(EditControl);
            
            Category rootCategory = Category.Find(BlogUtility.RootCategoryName);

            if (rootCategory != null && rootCategory.Categories.Count != 0)
            {
                CreateExistingCategoryControl(rootCategory);
                AddCategoryControlScripts();
            }
        }

        /// <summary>
        /// Adds selection controls for existing categories
        /// </summary>
        /// <param name="rootCategory">The root category for existing categories.</param>
        private void CreateExistingCategoryControl(Category rootCategory)
        {
            Panel existingCategoriesContainer = new Panel();
            existingCategoriesContainer.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
            existingCategoriesContainer.Style.Add(HtmlTextWriterStyle.MarginLeft, "10px");
            existingCategoriesContainer.Style.Add("float", "left");
            Controls.Add(existingCategoriesContainer);

            HtmlSelect tagList = new HtmlSelect();
            tagList.EnableViewState = false;
            tagList.ID = "TagList";
            tagList.Multiple = true;
            tagList.Size = 4;
            tagList.Style.Add(HtmlTextWriterStyle.Display, "block");
            tagList.Attributes.Add("ondblclick", "BlogTagAdd(this);");
            
            Label tagListLabel = new Label();
            tagListLabel.Style.Add(HtmlTextWriterStyle.Display, "block");
            tagListLabel.Text = LanguageManager.Instance.Translate("/blog/propertyblogtags/existingtagslabel");
            tagListLabel.AssociatedControlID = tagList.ID;
            
            existingCategoriesContainer.Controls.Add(tagListLabel);
            existingCategoriesContainer.Controls.Add(tagList);

            HtmlButton buttonAdd = new HtmlButton();
            buttonAdd.Attributes.Add("onclick", String.Format("BlogTagAdd(document.getElementById('{0}'));", tagList.ClientID));
            // We have to add type="button" since we don't want submit behavior in any browser.
            buttonAdd.Attributes.Add("type", "button");
            buttonAdd.InnerText = LanguageManager.Instance.Translate("/blog/propertyblogtags/existingtagsbutton");
            
            existingCategoriesContainer.Controls.Add(buttonAdd);
            
            List<string> blogCategories = new List<string>(rootCategory.Categories.Count);

            foreach (string categoryName in rootCategory.Categories.Cast<Category>().Select(category => category.Name.Trim()))
            {
                blogCategories.Add(categoryName);
            }
            blogCategories.Sort(StringComparer.OrdinalIgnoreCase);

            foreach (string category in blogCategories)
            {
                tagList.Items.Add(category);
            }
        }

        /// <summary>
        /// Add client side scripts to be able to add existing categories.
        /// </summary>
        private void AddCategoryControlScripts()
        {
            string editControl = String.Format("var editControl = document.getElementById(\"{0}\");", EditControl.ClientID);

            string blogTagAddScript = @"
               function BlogTagAdd(node) 
                {
                    if (node.selectedIndex != -1) 
                    {
                        var addSelected;
                        " + editControl + @"
                        var selectedArray = editControl.value.split("","");
                        var trimPattern = /^\s*|\s*$/g;
                        
                        for (var i = node.selectedIndex; i < node.options.length; i++)
                        {
                            if (node.options[i].selected) 
                            {
                                addSelected = true;
                                if (editControl.value.length != 0) 
                                {
                                    for (var j = 0; j < selectedArray.length; j++)
                                    {
                                        if (selectedArray[j].replace(trimPattern, """") == node.options[i].value) 
                                        {
                                            addSelected = false;
                                            break;
                                        }
                                    }
                                    if (addSelected) 
                                    {
                                        editControl.value = editControl.value + "", "";
                                    }
                                }
                                if (addSelected)
                                {
                                    editControl.value = editControl.value + node.options[i].value;
                                }
                            }
                        }
                        
                        node.selectedIndex = -1;
                        
                        if (typeof(BlogTagInputScroll) == ""function"")
                        {
                            BlogTagInputScroll(editControl);
                        }
                    }
                }
                ";

            string blogTagInputScroll = @"
                function BlogTagInputScroll(node)
                {
                    if (typeof node.scrollTop != 'undefined' && typeof node.scrollHeight != 'undefined')
                    {
                        node.scrollTop = node.scrollHeight;
                    }
                }
                ";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "BlogTagAdd", blogTagAddScript.ToString(), true);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "BlogTagInputScroll", blogTagInputScroll.ToString(), true);
        }

        /// <summary>
        /// Adds an error validator to the EditControlContainer and to the Page.Validators collection.
        /// Override since we want to add the validator to the correct control collection.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <remarks>This method is used to indicate that invalid data has been entered by the user.</remarks>
        public override void AddErrorValidator(string errorMessage)
        {
            StaticValidator validator = new StaticValidator(errorMessage);
            validator.Text = "*";
            validator.ValidationGroup = ValidationGroup;
            EditControlContainer.Controls.Add(validator);
        }

        /// <summary>
        /// Validates tags for invalid characters
        /// </summary>
        public override void ApplyEditChanges()
        {
            bool errors = false;

            if (!string.IsNullOrEmpty(EditControl.Text.Trim()))
            {
                Regex regex = new Regex(@"^[^\d\+\-].*");
                string[] tags = EditControl.Text.Split(',');


                foreach (string tag in tags)
                {
                    // Don't allow tag name to start with a number or +-
                    if (!regex.IsMatch(tag.Trim()))
                    {
                        AddErrorValidator(new EPiServer.Core.InvalidPropertyValueException(Name, tag.Trim()).Message);
                        errors = true;

                    }
                }
            }

            if (!errors)
            {
                base.ApplyEditChanges();
            }
        }
    }
}
