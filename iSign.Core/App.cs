using System;
using System.Reflection;
using iSign.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace iSign.Core
{
	public class App : MvxApplication
	{
		public App()
		{

			RegisterServicesFromAllAssemblies();
			RegisterAllViewModels();

            Mvx.RegisterType<IMvxAppStart, AppStart> ();
		}

		private void RegisterServicesFromAllAssemblies()
		{
			RegisterServicesForAssembly(typeof(App).GetTypeInfo().Assembly);
		}

		private void RegisterAllViewModels()
		{
			CreatableTypes().Inherits(typeof(MvxViewModel)).AsTypes().RegisterAsDynamic();
		}

		private void RegisterServicesForAssembly(Assembly assembly)
		{
			var creatableTypes = CreatableTypes(assembly);

			creatableTypes.WithAttribute<RegisterInterfacesAsDynamicAttribute>().AsInterfaces().RegisterAsDynamic();
			creatableTypes.WithAttribute<RegisterInterfacesAsSingletonAttribute>().AsInterfaces().RegisterAsSingleton();
			creatableTypes.WithAttribute<RegisterSelfAsDynamicAttribute>().AsTypes().RegisterAsDynamic();
			creatableTypes.WithAttribute<RegisterSelfAsSingletonAttribute>().AsTypes().RegisterAsSingleton();
		}
	}

	public class AppStart : MvxNavigatingObject, IMvxAppStart
	{
		private INavigationService NavigationService { get; }
		public AppStart(INavigationService navigationService)
		{
			NavigationService = navigationService;
		}
		public void Start(object hint = null)
		{
			NavigationService.ShowHomePage();
		}
	}
}
