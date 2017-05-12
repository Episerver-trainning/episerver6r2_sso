#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;


namespace EPiServer.Templates.Advanced.FileManager.Core
{
    /// <summary>
    /// Event argument describing a <see cref="WebControls.FileManagerControl"/> command.
    /// </summary>
    [Serializable]
    public class FileManagerCommandEventArgs : EventArgs
    {
        private string _commandName;
        private object _commandArguments;
        private bool _cancelCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManagerCommandEventArgs"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command to execute.</param>
        /// <param name="arguments">Additional command arguments.</param>
        public FileManagerCommandEventArgs(string commandName, object arguments)
        {
            CommandName = commandName;
            CommandArguments = arguments;
        }

        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
        public string CommandName {
            get { return _commandName; }
            set { _commandName = value; }
        }

        /// <summary>
        /// Gets or sets the additional command argument data.
        /// </summary>
        /// <value>The command argument data.</value>
        public object CommandArguments 
        {
            get { return _commandArguments; }
            set { _commandArguments = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether command should be cancelled.
        /// </summary>
        /// <value><c>true</c> if the command has been cancelled; otherwise, <c>false</c>.</value>
        public bool CancelCommand
        {
            get { return _cancelCommand; }
            set { _cancelCommand |= value; }
        }
    }
}
