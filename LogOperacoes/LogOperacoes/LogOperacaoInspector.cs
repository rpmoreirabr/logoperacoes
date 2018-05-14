using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace LogOperacoes
{
    public class LogOperacaoInspector
    {
        private bool incluirTagSecurity;

        private bool monitoramentoSaude;

        private bool debug;

        public LogOperacaoInspector(bool incluirTagSecurity, bool monitoramentoSaude, bool debug)
        {
            this.incluirTagSecurity = incluirTagSecurity;
            this.monitoramentoSaude = monitoramentoSaude;
            this.debug = debug;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            EntryData entry = null;
            var sw = new Stopwatch();
            sw.Start(); sw.Stop();

            try
            {
                var applicationNameRegex = new Regex(@"http[s]?://[\w\.]*:?[\w\.]*/(?<application>[\w\./]*)", RegexOptions.Compiled);
                var serviceNameRegex = new Regex(@"\w+$", RegexOptions.Compiled);
                RemoteEndpointMessageProperty item = OperationContext.Current.IncomingMessageProperties["System.ServiceModel.Channels.RemoteEndpointMessageProperty"] as RemoteEndpointMessageProperty;

                string name = "Usuário anônimo";
                if (ServiceSecurityContext.Current != null && ServiceSecurityContext.Current.PrimaryIdentity != null && !string.IsNullOrEmpty(ServiceSecurityContext.Current.PrimaryIdentity.Name))
                {
                    name = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                }

                IEnumerable<string> metatags = HttpContext.Current.Items["Metadados"] != null ? (IEnumerable<string>)HttpContext.Current.Items["Metadados"] : new string[] { };
                string correlationKey = (string)HttpContext.Current.Items["Correlation-Key"];

                entry = new EntryData()
                {
                    applicationName = applicationNameRegex.Match(request.Headers.To.AbsoluteUri)?.Groups["application"].Value,
                    serviceName = serviceNameRegex.Match(request.Headers.Action)?.Value,
                    client = item.Address,
                    correlationKey = correlationKey,
                    entryTime = DateTime.Now,
                    metadados = metatags,
                    requestContent = request.ToString(),
                    url = request.Headers.To.AbsoluteUri,
                    user = name
                };

            }
            catch (Exception)
            {             
                if (this.debug)
                {
                    throw;
                }
            }

            return entry;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            try
            {
                var entry = (EntryData)correlationState;
                entry.responseContent = reply.ToString();
                entry.success = this.XPathQuery(entry.responseContent, "//s:Fault").Count > 0;
                entry.duration = (entry.entryTime - DateTime.Now).Milliseconds;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (this.debug)
                {
                    throw exception;
                }
            }
        }
        

        private StringCollection XPathQuery(string data, string query)
        {
            var xmlData = new XmlDocument();
            xmlData.LoadXml(data);

            var namespaceManager = MapearNamespaces(xmlData);

            StringCollection stringCollections = new StringCollection();
            XPathNavigator xPathNavigator = xmlData.CreateNavigator();
            XPathExpression xPathExpression = xPathNavigator.Compile(query);
            xPathExpression.SetContext(namespaceManager);
            XPathNodeIterator xPathNodeIterators = xPathNavigator.Select(xPathExpression);
            while (xPathNodeIterators.MoveNext())
            {
                stringCollections.Add(xPathNodeIterators.Current.Value);
            }
            return stringCollections;
        }

        private XmlNamespaceManager MapearNamespaces(XmlDocument xmlData)
        {
            var namespaceManager = new XmlNamespaceManager(xmlData.NameTable);
            foreach (Match match in (new Regex("xmlns:(?<prefixo>[\\w]+)=\"(?<endereco>[\\S]+)\"")).Matches(xmlData.InnerXml)) 
            {
                string value = match.Groups["prefixo"].Value;
                string str = match.Groups["endereco"].Value;
                namespaceManager.AddNamespace(value, str);
            }

            return namespaceManager;
        }
    }
}
