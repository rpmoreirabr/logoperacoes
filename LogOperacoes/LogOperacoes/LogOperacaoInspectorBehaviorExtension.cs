    using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace LogOperacoes
{
    public class LogOperacaoInspectorBehaviorExtension : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection _properties;

        public override Type BehaviorType
        {
            get
            {
                return typeof(LogOperacaoInspectorBehavior);
            }
        }

        [ConfigurationProperty("debug", DefaultValue = false, IsRequired = false)]
        public bool Debug
        {
            get
            {
                return (bool)base["debug"];
            }
            set
            {
                base["debug"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this._properties == null)
                {
                    this._properties = new ConfigurationPropertyCollection()
                    {
                        new ConfigurationProperty("debug", typeof(bool), false)
                    };
                }
                return this._properties;
            }
        }

        public LogOperacaoInspectorBehaviorExtension()
        {
        }

        protected override object CreateBehavior()
        {
            return new LogOperacaoInspectorBehavior(this.Debug);
        }
    }
}
