#region Copyright
// Copyright © 1996-2010 EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;

namespace EPiServer.Templates.Advanced.PageProviders
{
    public class ExcelParserException : Exception
    {
        public string ExcelFileName { get; private set; }
        public string CustomMessage { get; private set; }

        public ExcelParserException(string excelFileName, string customMessage)
        {
            ExcelFileName = excelFileName;
            CustomMessage = customMessage;
        }

        public override string Message
        {
            get { return string.Format("Error occured while parsing excel file {0}. {1}", ExcelFileName, CustomMessage); }
        }
    }
}
