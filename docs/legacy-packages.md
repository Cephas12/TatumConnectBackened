Legacy NuGet package report

Found the following packages restored as .NETFramework targets (NU1701 warnings):

- Antlr 3.4.1.9004
- Microsoft.AspNet.Razor 3.3.0
- Microsoft.AspNet.WebPages 3.3.0
- Microsoft.Web.Infrastructure 1.0.0
- WebGrease 1.6.0

Notes and suggested actions:
- These packages target older .NET Framework TFMs and may not be fully compatible with net10.0.
- Identify which packages are required by your app features. If unused, remove them from the csproj.
- For UI/static assets pipelines (WebGrease), consider using modern equivalents or removing the package if not required.
- For Razor/WebPages packages, if you are not using them directly in the API project, remove them. If you need Razor functionality, migrate to the ASP.NET Core Razor packages supported on .NET 10.
- Test thoroughly after any package changes.
