using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Reflection;
using System.ComponentModel.Composition;
using System.Collections;

namespace PA.Configuration
{
    public class ConfigurationCatalog : ComposablePartCatalog, IConfigurationProvider
    {
        private List<ComposablePartDefinition> _parts = new List<ComposablePartDefinition>();

        private ComposablePartCatalog catalog;

        public IConfigurationSource Source { get; private set; }

        public ConfigurationCatalog(IConfigurationSource configurationSource, ComposablePartCatalog catalog)
            : base()
        {
            this.catalog = catalog;
            this.Source = configurationSource;

            string[] keys = this.GetEntries().ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].EndsWith("/size"))
                {
                    string root = keys[i].Substring(0, keys[i].LastIndexOf('/'));
                    string size = this.Source.GetSetting(keys[i]);

                    for (int index = 0; index < int.Parse(size); index++)
                    {
                        string t = this.Source.GetSetting(root + "/" + index);

                        if (t.StartsWith(">"))
                        {
                            ComposablePartDefinition part = this.catalog.Parts.FirstOrDefault(t.Substring(1));

                            if (part is ComposablePartDefinition)
                            {
                                var import = part.ImportDefinitions
                                    .Select(id => id.ContractName.StartsWith("#/") ? CreateIndexedImportDefinition(id, root, index) : id)
                                    .ToArray();

                                var export = part.ExportDefinitions
                                    .Select(ed => CreateIndexedExportDefinition(ed, index))
                                    .ToArray();

                                var newpart = ReflectionModelServices.CreatePartDefinition(
                                    ReflectionModelServices.GetPartType(part),
                                    ReflectionModelServices.IsDisposalRequired(part),
                                    new Lazy<IEnumerable<ImportDefinition>>(() => import),
                                    new Lazy<IEnumerable<ExportDefinition>>(() => export),
                                    new Lazy<IDictionary<string, object>>(() => part.Metadata),
                                    null);

                                this._parts.Add(newpart);
                            }
                        }
                    }
                }
            }
        }

        public ExportDefinition CreateIndexedExportDefinition(ExportDefinition id, int index)
        {
            LazyMemberInfo pi = ReflectionModelServices.GetExportingMember(id);

            Dictionary<string, object> metadata = new Dictionary<string, object>(id.Metadata);
            metadata.Add("Index", index);

            return ReflectionModelServices.CreateExportDefinition(pi,
                    id.ContractName,
                    new Lazy<IDictionary<string, object>>(() => metadata),
                    null);
        }

        public ImportDefinition CreateIndexedImportDefinition(ImportDefinition id, string root, int index)
        {
            if (ReflectionModelServices.IsImportingParameter(id))
            {
                Lazy<ParameterInfo> pi = ReflectionModelServices.GetImportingParameter(id);

                return ReflectionModelServices.CreateImportDefinition(pi,
                    root + "/" + index + id.ContractName.Substring(1),
                    AttributedModelServices.GetTypeIdentity(pi.Value.ParameterType),
                    new Dictionary<string, Type>(),
                    id.Cardinality,
                    CreationPolicy.Any,
                    null);
            }
            else
            {
                LazyMemberInfo pi = ReflectionModelServices.GetImportingMember(id);

                return ReflectionModelServices.CreateImportDefinition(pi,
                  root + "/" + index + id.ContractName.Substring(1),
                   AttributedModelServices.GetTypeIdentity(pi.GetMemberUnderlyingType()),
                   new Dictionary<string, Type>(),
                   id.Cardinality,
                   id.IsRecomposable,
                   CreationPolicy.Any,
                   null);
            }
        }

        public IEnumerable<string> GetEntries()
        {
            foreach (string section in this.Source.GetSectionNames())
            {
                string[] settings = this.Source.GetSettings(section);

                string array = settings.SingleOrDefault(s => s.EndsWith("/size"));

                if (array is string)
                {
                    foreach (string setting in settings)
                    {
                        yield return section + "/" + setting;
                    }
                }
                else
                {
                    foreach (string setting in settings)
                    {
                        yield return section + "/" + setting;
                    }
                }
            }
        }

        public void RegisterType<TImplementation, TContract>()
        {

            var part = ReflectionModelServices.CreatePartDefinition(
                new Lazy<Type>(() => typeof(TImplementation)),
                false,
                new Lazy<IEnumerable<ImportDefinition>>(() => GetImportDefinitions(typeof(TImplementation))),
                new Lazy<IEnumerable<ExportDefinition>>(() => GetExportDefinitions(typeof(TImplementation), typeof(TContract))),
                new Lazy<IDictionary<string, object>>(() => new Dictionary<string, object>()),
                null);

            this._parts.Add(part);
        }

        private ImportDefinition[] GetImportDefinitions(Type implementationType)
        {
            var constructors = implementationType.GetConstructors()[0];
            var imports = new List<ImportDefinition>();

            foreach (var param in constructors.GetParameters())
            {
                var cardinality = this.GetCardinality(param);
                var importType = cardinality == ImportCardinality.ZeroOrMore ? GetCollectionContractType(param.ParameterType) : param.ParameterType;

                imports.Add(
                    ReflectionModelServices.CreateImportDefinition(
                        new Lazy<ParameterInfo>(() => param),
                        AttributedModelServices.GetContractName(importType),
                        AttributedModelServices.GetTypeIdentity(importType),
                        Enumerable.Empty<KeyValuePair<string, Type>>(),
                        cardinality,
                        CreationPolicy.Any,
                        null));

            }
            return imports.ToArray();
        }

        private ImportCardinality GetCardinality(ParameterInfo param)
        {
            if (typeof(IEnumerable).IsAssignableFrom(param.ParameterType))
                return ImportCardinality.ZeroOrMore;
            else
                return ImportCardinality.ExactlyOne;
        }

        //This is hacky! Needs to be cleaned up as it makes many assumptions.
        private Type GetCollectionContractType(Type collectionType)
        {
            var itemType = collectionType.GetGenericArguments().First();
            var contractType = itemType.GetGenericArguments().First();
            return contractType;
        }

        private ExportDefinition[] GetExportDefinitions(Type implementationType, Type contractType)
        {
            var lazyMember = new LazyMemberInfo(implementationType);
            var contracName = AttributedModelServices.GetContractName(contractType);
            var metadata = new Lazy<IDictionary<string, object>>(() =>
            {
                var md = new Dictionary<string, object>();
                md.Add(CompositionConstants.ExportTypeIdentityMetadataName, AttributedModelServices.GetTypeIdentity(contractType));
                return md;
            });

            return new ExportDefinition[] { ReflectionModelServices.CreateExportDefinition(lazyMember, contracName, metadata, null) };
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return this._parts.AsQueryable();
            }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            var exports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();

            foreach (var part in this.Parts)
            {
                foreach (var export in part.ExportDefinitions)
                {
                    if (definition.IsConstraintSatisfiedBy(export))
                    {
                        exports.Add(new Tuple<ComposablePartDefinition, ExportDefinition>(part, export));
                    }
                }
            }
            return exports;
        }

    }
}
