🚧 Work In Progress 
========

The following disclaimer is taken directly from where [this project (sort of) originated](https://github.com/dotnet/roslyn-sdk/tree/master/samples/CSharp/SourceGenerators):

> These samples are for an in-progress feature of Roslyn. As such they may change or break as the feature is developed, and no level of support is implied.

> For more information on the Source Generators feature, see the [design document](https://github.com/dotnet/roslyn/blob/master/docs/features/source-generators.md).

In addition to the samples from the original C# repo, I've added a rough proof-of-concept for a RecordGenerator that takes a simple class in VB and expands it to support most of the functionality provided by C# 'records'.  More work will be done to get this beyond the prototype stage as it's one of the main interests that I have with regards to code generation. The original question I posited is "Could source generators be utilized to mimic what C# records provide without any language changes?"  So this is the starting point to answer that question. ;-)

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

There is some information floating around regarding source generators sometimes needing to require Visual Studio to be reloaded.  This appears to only be while you are actually writing/debugging the generators.  Additionally, there also seems to be some confusion around whether or not this is only related to project references versus "compiled" references. I currently have not dug into this very deep as all of my interaction that this point has been as a project reference since I'm more focused on actually building the generators at this point than actually consuming them.

Debugging a Source Generator
-----

You will most likely need to close and reopen the solution in Visual Studio for any changes made to the generators to take effect.  From what I understand, Microsoft is looking into resolving this issue; but until then... just restart. ;-)

The following information is pulled from [a video](https://www.youtube.com/watch?v=3YwwdoRg2F4) (12:00) and placing here for future follow up.

- Debugger.Launch() - might not be a good idea within Visual Studio.
- GeneratorDriver
  - Test console app
  - Unit tests
- https://github.com/davidwengier/SourceGeneratorTemplate
- https://github.com/chsienki/kittitas
- https://sourcegen.dev/  <------ This is something that would be really nice to have in VB.

Note that I've added a test console project so that the generators can be referenced, tested and debugged; however, leaving the above information active since I believe they provide value from a conceptual point of view and... as it turns out... I believe that @chsienki is one of the primary devs on C# source generators - so might be a good person to follow if this is something that you are interested in.