using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Core;
using System.Globalization;

namespace EPiServer.Templates.AlloyTech
{
    /// <summary>
    /// Extension methods for the date time class used by the alloy templates package.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the formatted string date and time based on a language definition
        /// </summary>
        /// <param name="page">The date to get the date from</param>
        /// <returns></returns>
        public static string ToFormattedDateAndTime(this DateTime date)
        {   
            return date.ToString("f");
        }

        /// <summary>
        /// Gets the formatted string time based on a language definition
        /// </summary>
        /// <param name="page">The date to get the date from</param>
        /// <returns></returns>
        public static string ToFormattedDate(this DateTime date)
        {
            return date.ToString("d");
        }

        /// <summary>
        /// Gets the formatted string time based on a language definition
        /// </summary>
        /// <param name="page">The date to get the date from</param>
        /// <returns></returns>
        public static string ToFormattedTime(this DateTime date)
        {
            return date.ToString("t");
        }
    }
}
