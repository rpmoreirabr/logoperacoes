using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace LogOperacoes
{
    public class LogOperacaoInspectorBehavior : IEndpointBehavior
    {    
        private bool debug;

        public LogOperacaoInspectorBehavior(bool debug)
        {
            this.debug = debug;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            LogOperacaoInspector logOperacaoInspector = new LogOperacaoInspector(this.debug);
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(logOperacaoInspector);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
