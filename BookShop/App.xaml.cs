﻿using System.Configuration;
using BookShop.Activation;
using BookShop.Contracts.Services;
using BookShop.Core.Api;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Services;
using BookShop.Helpers;
using BookShop.Models;
using BookShop.Services;
using BookShop.ViewModels;
using BookShop.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace BookShop;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();
    public static IShopRepository Repository
    {
        get; private set;
    }

    /// <summary>
    /// Build rest api call respository
    /// </summary>
    private void BuildRestRespository()
    {
        var config = ConfigurationManager.OpenExeConfiguration(
                        ConfigurationUserLevel.None);
        var baseUrl = config.AppSettings.Settings["BaseUrl"].Value;
        var apikey = config.AppSettings.Settings["apikey"].Value;
        Repository = new RestShopRepository(baseUrl, apikey);
    }

    public App()
    {
        InitializeComponent();
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<SplashViewModel>();
            services.AddTransient<SplashPage>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginPage>();

            services.AddTransient<UpsertProductViewModel>();
            services.AddTransient<UpsertProductPage>();

            services.AddTransient<CategoriesViewModel>();
            services.AddTransient<CategoriesPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<StatisticsViewModel>();
            services.AddTransient<StatisticsPage>();
            services.AddTransient<OrdersViewModel>();
            services.AddTransient<OrdersPage>();
            services.AddTransient<CreateOrderViewModel>();
            services.AddTransient<CreateOrderPage>();
            services.AddTransient<ProductsDetailViewModel>();
            services.AddTransient<ProductsDetailPage>();
            services.AddTransient<ProductsViewModel>();
            services.AddTransient<HomePage>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<DashboardPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();
        BuildRestRespository();
        UnhandledException += App_UnhandledException;
    }

    public static void SetMinWidthWindow(int width)
    {
        var manager = WinUIEx.WindowManager.Get(App.MainWindow);
        manager.MinWidth = width;
    }

    public static void SetMinHeightWindow(int height)
    {
        var manager = WinUIEx.WindowManager.Get(App.MainWindow);
        manager.MinHeight = height;
    }
    private async void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
        Console.WriteLine(sender);
        Console.WriteLine(e.Message);
        Console.WriteLine(e.Exception.StackTrace);
        await App.MainWindow.ShowMessageDialogAsync(e.Exception.StackTrace, "Unexpected error!");
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
