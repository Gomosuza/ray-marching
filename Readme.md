# Ray marching demo with Signed distance fields

Realtime compute shader based raymarching with [CSG](https://en.wikipedia.org/wiki/Constructive_solid_geometry) based on [SDF](https://en.wikipedia.org/wiki/Signed_distance_function):

:warning: This project is work in progress!

[![Ray-marching](https://dev.azure.com/marcstanlive/Opensource/_apis/build/status/120)](https://dev.azure.com/marcstanlive/Opensource/_build/definition?definitionId=120)

___

# Setup

Built with Visual Studio 2019 and .Net Core 3.0.

The compute shader implementation is in its [own repository](https://github.com/MarcStan/monogame-framework-computeshader) and referenced via a git submodule. Therefore you must run:

```
git submodule init && git submodule update
```

after checkout to build the solution successfully.

# Details

tbd

# Known issues

* purple screen when using compute shader on mobile chipsets (Intel Graphics 6xx and the likes). The shader simply doesn't run/output anything resulting in the default texture color (purple) being shown
* AccessViolationException on linkProgram on mobile chipsets (Intel Graphis 6xx and the likes)
