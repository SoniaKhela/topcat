﻿using System.Collections.Generic;
using System.Linq;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;
using System.Security.Principal;
using System.Web;
using Catalogue.Web.Account;
using Catalogue.Web.Controllers.Patch;
using Raven.Abstractions.Extensions;
using Raven.Client.Document;
using Raven.Client.Listeners;

namespace Catalogue.Web.Injection
{
    /// <summary>
    /// Defines the type bindings for dependency injection.
    /// </summary>
    public class MainNinjectModule : NinjectModule
    {
        public override void Load()
        {
            // use Ninject.Extensions.Conventions for easy ISomeType -> SomeType bindings
            Kernel.Bind(x => x
                .FromAssembliesMatching("Catalogue.*")
                .SelectAllClasses()
                .BindDefaultInterface());

            // inject a once-per-request raven document session
            Bind<IDocumentSession>()
                .ToMethod(x => WebApiApplication.DocumentStore.OpenSession())
                .InRequestScope();

            // convenience binding for the asp.net-provided current user
            // which is used by the once-per-request user context object
            Bind<IPrincipal>().ToMethod(x => HttpContext.Current.User);
            Rebind<IUserContext>().To<UserContext>().InRequestScope();
        }
    }
}
