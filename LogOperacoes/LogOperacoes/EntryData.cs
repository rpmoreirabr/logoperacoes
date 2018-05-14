using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LogOperacoes
{
    public class EntryData
    {
        public string applicationName { get; set; }
        public string serviceName { get; set; }
        public string url { get; set; }
        public HttpMethod method { get; set; }
        public string correlationKey { get; set; }
        public DateTime entryTime { get; set; }
        public long? duration { get; set; }
        public string requestContent { get; set; }
        public string responseContent { get; set; }
        public string client { get; set; }
        public HttpStatusCode? responseStatusCode { get; set; }
        public IEnumerable<string> metadados { get; set; }
        public bool success { get; set; }
        public string user { get; set; }
    }
}
