#region File Description
//-----------------------------------------------------------------------------
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
// Modified from the samples provided by
// Microsoft XNA Community Game Platform
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Build.Framework;
#endregion

namespace Extractor
{
    /// <summary>
    /// Custom implementation of the MSBuild ILogger interface records
    /// content build errors so we can later display them to the user.
    /// </summary>
    class ErrorLogger : ILogger
    {
        /// <summary>
        /// Initializes the custom logger, hooking the ErrorRaised notification event.
        /// </summary>
        public void Initialize(IEventSource eventSource)
        {
            if (eventSource != null)
            {
                eventSource.ErrorRaised += ErrorRaised;
                eventSource.WarningRaised += WarningRaised;
                //eventSource.MessageRaised += MessageRaised;
            }
        }


        /// <summary>
        /// Shuts down the custom logger.
        /// </summary>
        public void Shutdown()
        {
        }


        /// <summary>
        /// Handles error notification events by storing the error message string.
        /// </summary>
        void ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            errors.Add("Error: " + e.Message);
        }

        /// <summary>
        /// Gets a list of all the errors that have been logged.
        /// </summary>
        public List<string> Errors
        {
            get { return errors; }
        }

        List<string> errors = new List<string>();

        public void ClearErrors()
        {
            errors.Clear();
            warnings.Clear();
        }

        /// <summary>
        /// Handles error notification warnings by storing the error message string.
        /// </summary>
        void WarningRaised(object sender, BuildWarningEventArgs e)
        {
            warnings.Add("Warning: " + e.Message);
        }

        /*
        void MessageRaised(object sender, BuildMessageEventArgs e)
        {
            warnings.Add("Message: " + e.Message);
        }
         * */

        /// <summary>
        /// Gets a list of all the errors that have been logged.
        /// </summary>
        public List<string> Warnings
        {
            get { return warnings; }
        }

        List<string> warnings = new List<string>();

        #region ILogger Members

        
        /// <summary>
        /// Implement the ILogger.Parameters property.
        /// </summary>
        string ILogger.Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        string parameters;


        /// <summary>
        /// Implement the ILogger.Verbosity property.
        /// </summary>
        LoggerVerbosity ILogger.Verbosity
        {
            get { return verbosity; }
            set { verbosity = value; }
        }

        LoggerVerbosity verbosity = LoggerVerbosity.Normal;     //.Detailed; //.Normal;


        #endregion
    }
}
