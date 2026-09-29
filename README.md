# geometry3SharpSN

Strong-named version of [geometry3Sharp](https://github.com/gradientspace/geometry3Sharp).

Create the signed NuGet package locally with:

```powershell
.\build.ps1 --target Pack
```

Version tags in the form `1.2.3` publish the corresponding `geometry3SharpSN` package to NuGet.org. Publishing requires a repository secret named `NUGET_API_KEY`.
