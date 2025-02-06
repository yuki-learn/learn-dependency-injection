using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ploeh.Samples.Commerce.Domain;
using Ploeh.Samples.Commerce.Domain.EventHandlers;
using Ploeh.Samples.Commerce.ExternalConnections;
using Ploeh.Samples.Commerce.SqlDataAccess;
using Ploeh.Samples.Commerce.SqlDataAccess.Aspects;
using Ploeh.Samples.Commerce.Web.Presentation;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

public class Program
{
    private static readonly Container container = new Container();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuration
        var configuration = builder.Configuration;
        var commerceConfiguration = new CommerceConfiguration(
            connectionString: configuration.GetConnectionString("CommerceConnectionString"));

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        builder.Services.AddControllersWithViews();

        container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));
        builder.Services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(container));

        builder.Services.EnableSimpleInjectorCrossWiring(container);
        builder.Services.UseSimpleInjectorAspNetRequestScoping(container);
        
        var app = builder.Build();

        // Simple Injectorの設定
        InitializeContainer(app, container, commerceConfiguration);

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        container.Verify();
        app.Run();
    }

    private static void RegisterAsImplementedInterfaces(IEnumerable<Type> implementationTypes, Container container)
    {
        foreach (Type type in implementationTypes)
        {
            foreach (Type service in type.GetInterfaces())
            {
                container.Register(service, type);
            }
        }
    }

    private static void RegisterAsImplementedInterfaces(Assembly asm, Func<Type, bool> predicate, Container container) =>
        RegisterAsImplementedInterfaces(asm.ExportedTypes.Where(predicate), container);

    private static void InitializeContainer(IApplicationBuilder app, Container container, CommerceConfiguration commerceConfiguration)
    {
        container.RegisterMvcControllers(app);

        container.RegisterSingleton<ITimeProvider, DefaultTimeProvider>();
        container.RegisterInstance<IUserContext>(new AspNetUserContextAdapter());
        container.Register(() => new CommerceContext(commerceConfiguration.ConnectionString),
            Lifestyle.Scoped);

        Assembly assembly = typeof(ITimeProvider).Assembly;

        // 指定されたアセンブリ内で ICommandService<T> を実装するすべてのクラスを自動的に登録してる
        container.Register(
            typeof(ICommandService<>), assembly);

        // ICommandService<T>を実装してるDecoratorパターンのクラスを登録する。
        // Decoratorの登録は、登録が早いほうが内側に内包され、一番最後に登録したDecoratorが一番外側(最初に実行される)のDecoratorになる。
        // ↓ のイメージ
        // SecureCommandServiceDecorator<T>
        //  └── SaveChangesCommandServiceDecorator<T>
        //        └── AuditingCommandServiceDecorator<T>
        //           └── AdjustInventoryService
        container.RegisterDecorator(
            typeof(ICommandService<>),
            typeof(AuditingCommandServiceDecorator<>));

        container.RegisterDecorator(
            typeof(ICommandService<>),
            typeof(SaveChangesCommandServiceDecorator<>));

        container.RegisterDecorator(
            typeof(ICommandService<>),
            typeof(SecureCommandServiceDecorator<>));

        container.Collection.Register(typeof(IEventHandler<>), assembly);
        container.Register(typeof(IEventHandler<>), typeof(CompositeEventHandler<>));

        // Register adapters to external systems
        RegisterAsImplementedInterfaces(typeof(WcfBillingSystem).Assembly, type => true, container);

        // Register repositories
        RegisterAsImplementedInterfaces(typeof(SqlProductRepository).Assembly,
            type => type.Name.EndsWith("Repository"), container);
    }
}
