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
                string value = this.Source.GetSetting(keys[i]);

                if (value.StartsWith(">"))
                {
                    int index;

                    if (int.TryParse(keys[i].Substring(keys[i].LastIndexOf("/") + 1), out index))
                    {

                        string root = keys[i].Substring(0, keys[i].LastIndexOf('/'));
                        CreateImportDefinition(root, index, value.Substring(1));
                    }
                    else
                    {
                        CreateImportDefinition(keys[i], -1, value.Substring(1));
                    }
                }

            }
        }

        private void CreateImportDefinition(string root, int index, string value)
        {
            if (value is string)
            {
                ComposablePartDefinition part = this.catalog.Parts.FirstOrDefault(value);

                if (part is ComposablePartDefinition)
                {
                    string name = index < 0 ? root : root + "/" + index;

                    var import = part.ImportDefinitions
                        .Select(id => id.ContractName.StartsWith("#/") ? CreateRelativeImportDefinition(id, name) : id)
                        .ToArray();

                    var export = part.ExportDefinitions
                        .Select(ed => CreateRelativeExportDefinition(ed, root, index))
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

        public ExportDefinition CreateRelativeExportDefinition(ExportDefinition id, string name, int index)
        {
            LazyMemberInfo pi = ReflectionModelServices.GetExportingMember(id);

            if (index < 0)
            {
                return ReflectionModelServices.CreateExportDefinition(pi,
                   name,
                    new Lazy<IDictionary<string, object>>(() => id.Metadata),
                   null);

            }
            else
            {
                Dictionary<string, object> metadata = new Dictionary<string, object>(id.Metadata);
                metadata.Add("Index", index);

                return ReflectionModelServices.CreateExportDefinition(pi,
                 name,
                 new Lazy<IDictionary<string, object>>(() => metadata),
                 null);
            }

        }

        public ImportDefinition CreateIndexedImportDefinition(ImportDefinition id, string root, int index)
        {
            return CreateRelativeImportDefinition(id, root + "/" + index);
        }


        public ImportDefinition CreateRelativeImportDefinition(ImportDefinition id, string root)
        {
            if (ReflectionModelServices.IsImportingParameter(id))
            {
                Lazy<ParameterInfo> pi = ReflectionModelServices.GetImportingParameter(id);

                return ReflectionModelServices.CreateImportDefinition(pi,
                    root + id.ContractName.Substring(1),
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
                  root + id.ContractName.Substring(1),
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
                IEnumerable<string> settings = this.Source.GetSettings(section);

                IEnumerable<string> arraysettings = this.Source.GetArrays(section);

                foreach (string setting in settings.Where(s => !arraysettings.Any(a => s.StartsWith(a))))
                {
                    yield return section + "/" + setting;
                }

                foreach (string array in arraysettings.ToArray())
                {
                    foreach (string setting in this.Source.GetArraySettings(section, array).ToArray())
                    {
                        yield return section + "/" + array + "/" + setting;
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
