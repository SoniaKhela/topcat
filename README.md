
Topcat - the JNCC Data Resource Catalogue
====================================

A simple data resource catalogue supporting a sensible profile of UK Gemini.

Licensed under [Open Government Licence v2](http://www.nationalarchives.gov.uk/doc/open-government-licence/version/2/).

Development
-----------

###Web Compiler
There are no build-time or run-time steps used to compile Coffeescript and LESS.The application will expect the correct .js and .css files to have been design-time generated.

Originally we used the Visual Studio Web Essentials extension; now the "Web Compiler" extension for design-time support for CoffeeScript (.coffee) and LESS (.less). 

https://visualstudiogallery.msdn.microsoft.com/3b329021-cd7a-4a01-86fc-714c2d05bb6c

If you don't have this installed, changes to coffeescript and less files will not occur. This isn't ideal any more, since you now have to explicitly add files to be compiled to the compilerconfig.json file (by using the contect menu).


###AngularJS
There is (allegedly) [Resharper support](http://blogs.jetbrains.com/dotnet/2013/02/angularjs-support-for-resharper/).

###Resharper
Currently best to disable Resharper > Options > Tools > Unit Testing > Javascript Tests > 
* Enable QUnit support
* Enable Jasmine support

###RavenDB
RavenDB studio can be accessed in development at http://localhost:8888

To upgrade RavenDB, after updating the NuGet packages you currently need to update the Raven.Studio.Html5.zip file which can be got from the downloadable distribution.
Hopefully this will be embedded in a forthcoming version, making this extra step unnecessary.

Deployment
----------

Topcat runs with no special setup in Visual Studio for local development.

Here's what you need to do to create a production instance:

###Build
Run the `build/built.targets` MSBuild file (or use `build.bat`).
* Use the the Developer Command Prompt for Visual Studio, else you may need to fix up (copy) the `Microsoft.WebApplication.targets` into the necessary place in the MSBuild installation.
* You may need to install / correct the path for Git.
* You may need to install / correct the path for NUnit-console.

###Windows Authentication
This is an corporate / intranet application and user account details and authentication rely on
Active Directory and Windows authentication which gives us a great user experience in Chrome and IE.

You need to disable Anonymous Authentication and enable Windows Authentication in the IIS website hosting Topcat.

###RavenDB
You can deploy Raven in various ways; the recommended is using the Windows installer:

* Download the correct version from https://ravendb.net/download (see packages folder for the version number currently in use) 
* Supply the license file when requested
* Set up as an IIS-hosted application
** Port: 8090
** Path: C:\topcat\RavenDB
* Browse to http://localhost:8090/ to check the installation succeeded
* In the Create a New Database dialogue, create e.g. Catalogue.Data.Beta with Versioning Bundle

When deploying a new database instance, the **Versioning bundle** needs to be enabled.

**Important** The Catalogue.Data.dll must be deployed into Raven/Analyzers folder because RavenDB needs to be able to load the custom Lucene analyzer we use.  

LinqPad
-------
RavenDB doesn't support ad-hoc querying very well. We use LinqPad to do ad-hoc querying and basic data visualisation. 

You need to use the RavenDB LinqPad adaptor. Create a connection like `RavenDB: Topcat LIVE`, and add the references to the assemblies in the output folder of your build.

    Catalogue.Data.dll
	Catalogue.Gemini.dll

You can then write queries like this:

    from r in Query<Catalogue.Data.Model.Record>()
    where r.Gemini.Title.StartsWith("Sea")
    select r
  
