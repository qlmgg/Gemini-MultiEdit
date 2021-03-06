using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini
{
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Gu.Localization;

    public class AppBootstrapper : BootstrapperBase
    {
        private List<Assembly> _priorityAssemblies;

		protected CompositionContainer Container { get; set; }

        internal IList<Assembly> PriorityAssemblies
        {
            get { return _priorityAssemblies; }
        }

        public AppBootstrapper()
        {
            this.PreInitialize();
            this.Initialize();
        }

        protected virtual void PreInitialize()
        {
            var code = Properties.Settings.Default.LanguageCode;

            if (!string.IsNullOrWhiteSpace(code))
            {
                var culture = CultureInfo.GetCultureInfo(code);
                // If code == "en", force to use default resource (Resources.resx)
                // See PO #243
                if (!Translator.Cultures.Contains(culture))
                    culture = CultureInfo.InvariantCulture;
                Translator.Culture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }


        }

		/// <summary>
		/// By default, we are configured to use MEF
		/// </summary>
		protected override void Configure()
		{
            MessageBinder.SpecialValues.Add("$scaledmousex", (ctx) =>
            {
                var img = ctx.Source as Image;
                var input = ctx.Source as IInputElement;
                var e = ctx.EventArgs as MouseEventArgs;
                //// If there is an image control, get the scaled position
                if (img != null && e != null)
                {
                    Point position = e.GetPosition(img);
                    return (int)(img.Source.Width * (position.X / img.ActualWidth));
                }

                // If there is another type of of IInputControl get the non-scaled position - or do some processing to get a scaled position, whatever needs to happen
                if (e != null && input != null)
                    return e.GetPosition(input).X;

                // Return 0 if no processing could be done
                return 0;
            });
            MessageBinder.SpecialValues.Add("$scaledmousey", (ctx) =>
            {
                var img = ctx.Source as Image;
                var input = ctx.Source as IInputElement;
                var e = ctx.EventArgs as MouseEventArgs;

                // If there is an image control, get the scaled position
                if (img != null && e != null)
                {
                    Point position = e.GetPosition(img);
                    return (int)(img.Source.Height * (position.Y / img.ActualHeight));
                }

                // If there is another type of of IInputControl get the non-scaled position - or do some processing to get a scaled position, whatever needs to happen
                if (e != null && input != null)
                    return e.GetPosition(input).Y;

                // Return 0 if no processing could be done
                return 0;
            });
            // Add all assemblies to AssemblySource (using a temporary DirectoryCatalog).
            var directoryCatalog = new DirectoryCatalog(@"./");
            try
            {

            AssemblySource.Instance.AddRange(
                directoryCatalog.Parts
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));
            }
            catch (Exception ex)
            {
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                }
            }

            // Prioritise the executable assembly. This allows the client project to override exports, including IShell.
            // The client project can override SelectAssemblies to choose which assemblies are prioritised.
            _priorityAssemblies = SelectAssemblies().ToList();
		    var priorityCatalog = new AggregateCatalog(_priorityAssemblies.Select(x => new AssemblyCatalog(x)));
		    var priorityProvider = new CatalogExportProvider(priorityCatalog);
            
            // Now get all other assemblies (excluding the priority assemblies).
			var mainCatalog = new AggregateCatalog(
                AssemblySource.Instance
                    .Where(assembly => !_priorityAssemblies.Contains(assembly))
                    .Select(x => new AssemblyCatalog(x)));
		    var mainProvider = new CatalogExportProvider(mainCatalog);

			Container = new CompositionContainer(priorityProvider, mainProvider);
		    priorityProvider.SourceProvider = Container;
		    mainProvider.SourceProvider = Container;

			var batch = new CompositionBatch();

		    BindServices(batch);
            batch.AddExportedValue(mainCatalog);

			Container.Compose(batch);


        }

	    protected virtual void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(Container);
	        batch.AddExportedValue(this);
        }

		protected override object GetInstance(Type serviceType, string key)
		{
			string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
			var exports = Container.GetExports<object>(contract);

			if (exports.Any())
				return exports.First().Value;

			throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
		}

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
		}

		protected override void BuildUp(object instance)
		{
			Container.SatisfyImportsOnce(instance);
		}

	    protected override void OnStartup(object sender, StartupEventArgs e)
	    {
	        base.OnStartup(sender, e);
            DisplayRootViewFor<IMainWindow>();
	    }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetEntryAssembly() };
        }
	}
}
