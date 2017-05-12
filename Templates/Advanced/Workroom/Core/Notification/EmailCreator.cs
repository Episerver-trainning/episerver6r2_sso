#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EPiServer.Core;
namespace EPiServer.Templates.Advanced.Workroom.Core.Notification
{
    /// <summary>
    /// A abstract class that provide a information for creating email according to the email template.
    /// </summary>
    public abstract class EmailCreator
    {
        /// <summary>
        /// Create a email creator instance according to the base workroom page reference
        /// </summary>
        /// <param name="workroomBaseRoot"></param>
        protected EmailCreator(PageReference workroomBaseRoot)
        {
            WorkroomBaseRoot = workroomBaseRoot;
            InitializeTemplateProperties();
        }

        /// <summary>
        /// Returns a subject string as the result of compilation a subject property with values of variableSet instance.
        /// </summary>
        /// <param name="variableSet">A class with variables for replacement.</param>
        /// <returns>A compiled email subject string.</returns>
        public string GetSubject(EmailVariableSet variableSet)
        {
            return UpdateTemplateWithValues(EmailSubject, variableSet);
        }

        /// <summary>
        /// Returns a body string as the result of compilation a body property with values of variableSet instance.
        /// </summary>
        /// <param name="variableSet">A class with variables for replacement.</param>
        /// <returns>A compiled email subject string.</returns>
        public string GetBody(EmailVariableSet variableSet)
        {
            return UpdateTemplateWithValues(EmailBody, variableSet);
        }


        /// <summary>
        /// Gets a template page type name for email creation.
        /// </summary>
        public abstract string EmailTemplatePageType
        {
            get;
        }

        /// <summary>
        /// Gets a template page name for email creation.
        /// </summary>
        public abstract string EmailTemplatePageName
        {
            get;
        }

        /// <summary>
        /// Gets, sets a page reference to a root page for workrooms.
        /// </summary>
        protected PageReference WorkroomBaseRoot
        {
            get;
            set;
        }

        protected string EmailSubject { get; set; }
        protected string EmailBody { get; set; }

        private const string PAGE_TYPE_TEMPLATES = "[AlloyTech] Standard page";
        private const string PAGE_NAME_WORKROOM_TEMPLATES = "[Workroom Templates]";
        private const string PAGE_NAME_EMAIL_TEMPLATES = "[Email templates]";

        private const string PAGE_PROPERTY_NAME_SUBJECT = "MailSubject";
        private const string PAGE_PROPERTY_NAME_BODY = "MailBody";

        private static PageData FindPageByName(PageReference rootPage, string pageName, string pageType)
        {
            PageDataCollection children = DataFactory.Instance.GetChildren(rootPage);
            children.Add(DataFactory.Instance.GetChildren(rootPage, LanguageSelector.MasterLanguage()));

            return children.FirstOrDefault(page => page.PageName.Equals(pageName) && page.PageTypeName.Equals(pageType));
        }

        private PageData FindTemplateInPageTree()
        {
            PageData workroomRootPage = DataFactory.Instance.GetPage(WorkroomBaseRoot);
            PageReference workroomTemplateRoot = workroomRootPage[WorkroomPageBase.TemplateContainer] as PageReference;
            PageData workroomTemplatesPage = DataFactory.Instance.GetPage(workroomTemplateRoot);
            
            if( workroomTemplatesPage == null )
            {
                throw new EPiServerException(string.Format("There is not a special workroom template root page {0} in {1}", PAGE_NAME_WORKROOM_TEMPLATES, DataFactory.Instance.GetPage(WorkroomBaseRoot).PageName));
            }
            PageData emailTemplateRootPage = FindPageByName(workroomTemplatesPage.PageLink, PAGE_NAME_EMAIL_TEMPLATES, PAGE_TYPE_TEMPLATES);
            if( emailTemplateRootPage == null )
            {
                throw new EPiServerException(string.Format("There is not a special email template root page {0} for email templates in {1}", PAGE_NAME_EMAIL_TEMPLATES, workroomTemplatesPage.PageName));
            }
            PageData emailTemplatePage = FindPageByName(emailTemplateRootPage.PageLink, EmailTemplatePageName, EmailTemplatePageType);
            if( emailTemplateRootPage == null )
            {
                throw new EPiServerException(string.Format("There is not a special email template page {0} for email templates in {1}", EmailTemplatePageName, emailTemplateRootPage.PageName));
            }
            return emailTemplatePage;
        }

        private void InitializeTemplateProperties()
        {
            PageData emailTemplatePage = FindTemplateInPageTree();
            EmailSubject = emailTemplatePage.GetValue(PAGE_PROPERTY_NAME_SUBJECT) as string;
            EmailBody = emailTemplatePage.GetValue(PAGE_PROPERTY_NAME_BODY) as string;
        }

        private static string UpdateTemplateWithValues(string rawString, EmailVariableSet variableSet)
        {
            Regex expression = new Regex(@"\[[\w]*\]");
            MatchCollection matches = expression.Matches(rawString);

            string updatedString = rawString;

            foreach( Match match in matches )
            {
                string propertyValue = GetPropertyValueFromSet(variableSet, match.Value.Substring(1, match.Value.Length - 2));
                updatedString = updatedString.Replace(match.Value, propertyValue);
            }

            return updatedString;
        }

        private static string GetPropertyValueFromSet(EmailVariableSet instance, string propertyName)
        {
            Type instanceType = instance.GetType();
            Console.WriteLine(instanceType.Name);
            PropertyInfo propertyBanch = instanceType.GetProperty(propertyName);
            return propertyBanch.GetValue(instance, null) as string;
        }
    }
}
