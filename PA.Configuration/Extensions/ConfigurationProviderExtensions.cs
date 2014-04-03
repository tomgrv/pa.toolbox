using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Reflection;
using PA.Converters.Extensions;
using System.Diagnostics;
using System.ComponentModel.Composition.Hosting;

namespace PA.Configuration
{
    public static class ConfigurationProviderExtensions
    {
        public class Item
        {
            public Contract Contract { get; private set; }
            public string Value { get; private set; }

            public Item(string v, Contract c)
            {
                this.Contract = c;
                this.Value = v;
            }
        }


        public class Contract
        {
            public Type Type { get; private set; }
            public string Name { get; private set; }

            public Contract(string n, Type t)
            {
                this.Type = t;
                this.Name = n;
            }
        }

        public static IEnumerable<Lazy<Type>> ExportTypes<T>(this IEnumerable<T> parts, ImportDefinition definition, params Type[] ctorParamsTypes)
             where T : ComposablePartDefinition
        {
            return parts.Select(part => part.ExportType(definition)).Where(t => t is Lazy<Type> && t.Value.GetConstructor(ctorParamsTypes) != null);
        }

        public static IEnumerable<Lazy<Type>> ExportTypes<T>(this IEnumerable<T> parts, Type targetType, params Type[] ctorParamsTypes)
             where T : ComposablePartDefinition
        {
            return parts.Select(part => part.ExportType(targetType)).Where(t => t is Lazy<Type> && t.Value.GetConstructor(ctorParamsTypes) != null);
        }

        public static IEnumerable<Lazy<Type>> ExportTypes<T>(this IEnumerable<T> parts, ImportDefinition definition)
             where T : ComposablePartDefinition
        {
            return parts.Select(part => part.ExportType(definition)).Where(lt => lt is Lazy<Type>);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> parts, string TypeName)
            where T : ComposablePartDefinition
        {
            return parts.FirstOrDefault(p => p.ExportDefinitions
                .Any(ed => ReflectionModelServices.GetExportingMember(ed)
                                        .GetAccessors()
                                        .Any(a => (a is Type && (a as Type).FullName == TypeName))
                    )
                );
        }

        public static Lazy<Type> ExportType(this ComposablePartDefinition part, ImportDefinition definition)
        {
            if (part.ExportDefinitions.Any(definition.Constraint.Compile()))
            {
                return ReflectionModelServices.GetPartType(part);
            }

            return null;
        }

        public static Type GetMemberUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
            }
        }

        public static Type GetMemberUnderlyingType(this LazyMemberInfo member)
        {
            return member.GetAccessors()
                .First(a => (a.MemberType == MemberTypes.Method && a.Name.StartsWith("get_")))
                .GetMemberUnderlyingType();
        }

        public static Lazy<Type> ExportType(this ComposablePartDefinition part, Type targetType)
        {
            if (part.ExportDefinitions.Any(def =>
                def.Metadata.ContainsKey("ExportTypeIdentity") &&
                def.Metadata["ExportTypeIdentity"].Equals(targetType.FullName)))
            {
                return ReflectionModelServices.GetPartType(part);
            }

            return null;
        }



        public static object GetInstance<T>(this IConfigurationProvider cp, Type targetType, string stringValue, params object[] args)
            where T : class, IConfigurationItem
        {
            try
            {
                if (targetType.IsGenericType && targetType.IsClass && typeof(T).IsAssignableFrom(targetType))
                {
                    return (T)Activator.CreateInstance(targetType, args);
                }
                else if (targetType.IsGenericType && typeof(T).IsAssignableFrom(targetType))
                {
                    targetType = typeof(ConfigurationItem<>).MakeGenericType(targetType.GetGenericArguments());
                    return (T)Activator.CreateInstance(targetType, args);
                }
                else if (targetType.IsValueType || targetType.IsArray || targetType.IsSerializable)
                {
                    return stringValue.ParseTo<T, string>(targetType);
                }
            }
            catch
            {
                Trace.TraceError("Error while configuring <" + targetType + "> in <" + targetType.DeclaringType + ">");
            }

            return default(T);
        }

        public static Contract GetConfigurationKey(this IConfigurationProvider cp, ImportDefinition definition)
        {
            string contractName = definition.ContractName;
            Type contractType = null;

            if (ReflectionModelServices.IsImportingParameter(definition))
            {
                var importingParameter = ReflectionModelServices.GetImportingParameter(definition);
                contractName = importingParameter.Value.Member.DeclaringType.FullName + "/" + importingParameter.Value.Name;
                contractType = importingParameter.Value.ParameterType;
            }
            else
            {
                var getAccessor = ReflectionModelServices
                  .GetImportingMember(definition)
                  .GetAccessors()
                  .Where(x => x is MethodInfo)
                  .Select(x => x as MethodInfo)
                  .FirstOrDefault(x => (x.Attributes & MethodAttributes.SpecialName) == MethodAttributes.SpecialName && x.Name.StartsWith("get_", StringComparison.Ordinal));

                contractType = getAccessor.ReturnType;

                if (contractName.StartsWith("./"))
                {
                    contractName = getAccessor.DeclaringType.FullName + contractName.Substring(1);
                }
                else if (!contractName.Contains('/'))
                {
                    contractName = getAccessor.DeclaringType.FullName + "/" + getAccessor.Name.Remove(0, 4);
                }
            }

            return new Contract(contractName, contractType);
        }

        public static IEnumerable<Item> GetConfigurationItems(this IConfigurationProvider cp, ImportDefinition definition)
        {
            Contract contract = cp.GetConfigurationKey(definition);

            if (string.IsNullOrEmpty(contract.Name) || contract.Type == null)
            {
                yield break;
            }

            if (definition.Cardinality == ImportCardinality.ZeroOrMore)
            {
                if (cp.Source.ContainsSetting(contract.Name + "/size"))
                {
                    int count = int.Parse(cp.Source.GetSetting(contract.Name + "/size"));

                    for (int i = 0; i < count; i++)
                    {
                        if (contract.Type.HasElementType)
                        {
                            yield return new Item(cp.Source.GetSetting(contract.Name + "/" + i), new Contract(contract.Name + "/" + i, contract.Type.GetElementType()));
                        }
                        else
                        {
                            string value = cp.Source.GetSetting(contract.Name + "/" + i);

                            if (value.StartsWith(">"))
                            {
                                yield return new Item(value, new Contract(contract.Name + "/" + i, typeof(object)));
                            }
                            else
                            {
                                yield return new Item(value, contract);
                            }
                        }
                    }
                }
            }
            else
            {
                if (cp.Source.ContainsSetting(contract.Name))
                {
                    yield return new Item(cp.Source.GetSetting(contract.Name), contract);
                }
            }

        }


        public static IEnumerable<Export> GetExports<T>(this IEnumerable<ConfigurationProviderExtensions.Item> ci, ImportDefinition definition, Func<Type, string, T> getInstance)
        {
            foreach (ConfigurationProviderExtensions.Item Configuration in ci)
            {
                if (!Configuration.Contract.Name.StartsWith("#/"))
                {
                    Type targetType = Configuration.Contract.Type;

                    if (targetType.IsArray)
                    {
                        Array value = Configuration.Value.AsArray(targetType, (t, s) => getInstance(t, s));

                        //Array value = Configuration.Value
                        //    .AsArray()
                        //    .Select(s => getInstance(targetType.GetElementType(), s))
                        //    .ToArray(targetType.GetElementType());

                        if (value is Array && value.Length > 0)
                        {
                            yield return new Export(definition.ContractName, () => value);
                        }
                    }
                    else
                    {
                        object value = getInstance(targetType, Configuration.Value);

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
