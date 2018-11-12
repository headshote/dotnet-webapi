﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebApplicationExercise.Logging
{
    public class Logger : ILogger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Information(string message)
        {
            log.Info(message);
        }

        public void Information(string fmt, params object[] vars)
        {
            Information(string.Format(fmt, vars));
        }

        public void Information(Exception exception, string fmt, params object[] vars)
        {
            Information(FormatExceptionMessage(exception, fmt, vars));
        }

        public void Warning(string message)
        {
            log.Warn(message);
        }

        public void Warning(string fmt, params object[] vars)
        {
            Warning(string.Format(fmt, vars));
        }

        public void Warning(Exception exception, string fmt, params object[] vars)
        {
            Warning(FormatExceptionMessage(exception, fmt, vars));
        }

        public void Error(string message)
        {
            log.Error(message);
        }

        public void Error(string fmt, params object[] vars)
        {
            Error(string.Format(fmt, vars));
        }

        public void Error(Exception exception, string fmt, params object[] vars)
        {
            Error(FormatExceptionMessage(exception, fmt, vars));
        }

        public void TraceApi(string controllerName, string method, TimeSpan timespan, string extraInfo)
        {
            string message = String.Concat("Controller:", controllerName, ". Method:", method, ". Timespan:", timespan.ToString(), ". ", extraInfo);
            log.Info(message);
        }

        public void TraceApi(string controllerName, string method, TimeSpan timespan)
        {
            TraceApi(controllerName, method, timespan, "");
        }

        public void TraceApi(string controllerName, string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(controllerName, method, timespan, string.Format(fmt, vars));
        }

        private static string FormatExceptionMessage(Exception exception, string fmt, object[] vars)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format(fmt, vars));
            sb.Append(" Exception: ");
            sb.Append(exception.ToString());
            return sb.ToString();
        }
    }
}