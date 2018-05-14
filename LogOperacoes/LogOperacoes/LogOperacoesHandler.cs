using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LogOperacoes
{

    public class LogOperacoesHandler : DelegatingHandler
    {

        public LogOperacoesHandler()
        {
        }

      

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;

            if (ConfigurationManager.AppSettings["LogOperacoes.Enabled"]?.ToLower() == "true")
            {
                var recorder = new EntryRecorder();
                var entryTime = DateTime.Now;
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                response = await base.SendAsync(request, cancellationToken);

                var requestContent = await request.Content?.ReadAsStringAsync();
                string responseContent = string.Empty;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }
                
                IEnumerable<string> metatags = HttpContext.Current.Items["Metadados"] != null ? (IEnumerable<string>)HttpContext.Current.Items["Metadados"] : new string[] { };
                string correlationKey = (string)HttpContext.Current.Items["Correlation-Key"];

                sw.Stop();

                recorder.LogDataAsync(request.RequestUri.AbsoluteUri,
                request.Method,
                correlationKey,
                entryTime,
                requestContent,
                responseContent,
                HttpContext.Current.Request.UserHostAddress,
                response.StatusCode,
                response.IsSuccessStatusCode,
                metatags,
                duration: sw.ElapsedMilliseconds
                );
            }
            else
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            return response;
        }        
    }




    //.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    //{
    //    AutoRegisterTemplate = true,
    //    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
    //    IndexFormat = "logoperacoes-{0:yyyy.MM}",
    //    CustomFormatter = new ElasticsearchJsonFormatter()
    //})
    //.CreateLogger();
}
