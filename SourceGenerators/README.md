🚧 Work In Progress 
========

The following disclaimer is taken directly from where [this project (sort of) originated](https://github.com/dotnet/roslyn-sdk/tree/master/samples/CSharp/SourceGenerators):

> These samples are for an in-progress feature of Roslyn. As such they may change or break as the feature is developed, and no level of support is implied.

> For more information on the Source Generators feature, see the [design document](https://github.com/dotnet/roslyn/blob/master/docs/features/source-generators.md).

Prerequisites
-----

These samples require **Visual Studio 16.9.0 Preview 2.0** or higher.

Building the samples
-----
Open `SourceGenerators.sln` in Visual Studio or run `dotnet build` from the `\SourceGenerators` directory.

Running the samples
-----

The generators must be run as part of another build, as they inject source into the project being built. This repo contains a sample project `GeneratorDemo` that relies of the sample generators to add code to it's compilation. 

Run `GeneratedDemo` in Visual studio or run `dotnet run` from the `GeneratorDemo` directory.

Using the samples in your project
-----

You can add the sample generators to your own project by adding an item group containing an analyzer reference:

```xml
<ItemGroup>
    <Analyzer Include="path\to\SourceGeneratorSamples.dll">
</ItemGroup>
```

You will most likely need to close and reopen the solution in Visual Studio for any changes made to the generators to take effect.  From what I understand, Microsoft is looking into resolving this issue; but until then... just restart. ;-)

Debugging a Source Generator
-----

The following information is pulled from [a video](https://www.youtube.com/watch?v=3YwwdoRg2F4) (12:00) and placing here for future follow up.

- Debugger.Launch() - might not be a good idea within Visual Studio.
- GeneratorDriver
  - Test console app
  - Unit tests
- https://github.com/davidwengier/SourceGeneratorTemplate
- https://github.com/chsienki/kittitas
- https://sourcegen.dev/  <------ This is something that would be really nice to have in VB.
