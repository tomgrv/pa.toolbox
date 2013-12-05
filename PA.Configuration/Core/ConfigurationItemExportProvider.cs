using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using PA.Converters.Extensions;
using System.Diagnostics;

namespace PA.Configuration
{
    public class ConfigurationItemExportProvider : ExportProvider, IConfigurationProvider
    {
        public IConfigurationSource Source { get; private set; }

        public ConfigurationItemExportProvider(IConfigurationSource configurationSource)
        {
            // the configuration source determines where configuration values come from (eg. App.Config file)
            this.Source = configurationSource;
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            lock (this.Source)
            {
                foreach (ConfigurationProviderExtensions.Item ConfigurationData in this.GetConfigurationItems(definition))
                {
                    if (!ConfigurationData.Contract.Name.StartsWith("#/"))
                    {
                        string stringValue = ConfigurationData.Value;
                        Type targetType = ConfigurationData.Contract.Type;

                        object value = null;

                        if (typeof(ConfigurationItem).IsAssignableFrom(targetType) 
                            && targetType.IsGenericType)
                        {
                            try
                            {
                                value = Activator.CreateInstance(targetType, definition.ContractName, Source);
                            }
                            catch
                            {
                                Trace.TraceWarning("Error while configuring <" + definition.ContractName + "> as <" + targetType + "> in <" + targetType.DeclaringType+">");
                            }
                        }
                        else if (targetType.IsArray
                            ? targetType.GetElementType().IsValueType || targetType.GetElementType().IsSerializable
                            : targetType.IsValueType || targetType.IsSerializable)
                        {
                            try
                            {
                                value = stringValue.ParseTo(targetType);
                            }
                            catch
                            {
                                Trace.TraceWarning("Error while configuring <" + definition.ContractName + "> as <" + targetType + "> in <" + targetType.DeclaringType + ">");
                            }
                        }
                        else if (typeof(IEnumerable).IsAssignableFrom(targetType) 
                            && targetType.IsGenericType 
                            && targetType.GetGenericArguments().Length == 1 
                            && targetType.GetGenericArguments().First().IsSerializable)
                        {
                            try
                            {
                                value = stringValue.ParseTo(targetType.GetGenericArguments().First());
                            }
                            catch
                            {
                                Trace.TraceWarning("Error while configuring <" + definition.ContractName + "> as <" + targetType + "> in <" + targetType.DeclaringType + ">");
                            }
                        }
                        if (typeof(IEnumerable).IsAssignableFrom(targetType)
                           && targetType.IsGenericType
                           && targetType.GetGenericArguments().Length == 1
                           && stringValue.StartsWith(">"))
                        {
                        }
                       

                        if (value is object)
                        {
                            yield return new Export(definition.ContractName, () => value);
                        }
                    }
                }
            }
        }
    }
}
