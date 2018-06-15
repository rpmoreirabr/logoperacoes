using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogOperacoes
{
    public class EntryRecorder
    {
        private ILogger Logger;

        public EntryRecorder()
        {
            Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .ReadFrom
              .AppSettings()
              .CreateLogger();
        }
        public void LogDataAsync(string url,
            HttpMethod method,
            string correlationKey,
            DateTime entryTime,
            string requestContent,
            string responseContent,
            string client,
            HttpStatusCode responseStatusCode,
            bool success,
            IEnumerable<string> metadados = null,
            string user = null,
            long? duration = null,
            string applicationName = null,
            string serviceName = null)
        {
            var regexUri = new Regex(@"http[s]?://(\w*[\.]?)*:?\w*/(?<application>[\w\.]*)/(?<service>[^?]*)\?*(?<query>.*)", RegexOptions.Compiled);
                                     
            var entry = new EntryData()
            {
                url = url,
                client = client,
                requestContent = requestContent,
                responseContent = responseContent,
                correlationKey = correlationKey,
                entryTime = entryTime,
                metadados = metadados,
                method = method,
                responseStatusCode = responseStatusCode,
                success = success,
                user = user,
                duration = duration,
                applicationName = applicationName ?? regexUri.Match(url)?.Groups["application"]?.Value,
                serviceName =  serviceName ?? regexUri.Match(url)?.Groups["service"]?.Value
            };

            if (success)
            {
                Logger.Information("{url} {applicationName} {serviceName} {requestContent} {responseContent} {duration} {correlationKey} {entryTime} {method} {statusCode} {client} {metadata}",
                    entry.url,
                    entry.applicationName,
                    entry.serviceName,
                    entry.requestContent,
                    entry.responseContent,
                    entry.duration,
                    entry.correlationKey,
                    entry.entryTime,
                    entry.method,
                    entry.responseStatusCode,
                    entry.client,
                    entry.metadados);
            }
            else
            {
                Logger.Error("{url} {applicationName} {serviceName} {requestContent} {responseContent} {duration} {correlationKey} {entryTime} {method} {statusCode} {client} {metadata}",
                    entry.url,
                    entry.applicationName,
                    entry.serviceName,
                    entry.requestContent,
                    entry.responseContent,
                    entry.duration,
                    entry.correlationKey,
                    entry.entryTime,
                    entry.method,
                    entry.responseStatusCode,
                    entry.client,
                    entry.metadados);
            }
        }
    }
}
