# TinyPlanetGenerator
<img src="https://github.com/Arghonot/TinyPlanetGenerator/tree/master/Assets/Textures/ReadmeTextures/resizedgolfplanet.png" alt="golfplanet.png" width="100" height="50">

![](https://github.com/Arghonot/TinyPlanetGenerator/tree/master/Assets/Textures/ReadmeTextures/resizedgolfplanet.png?s=100)

This is a WIP Spore like planet generator.
It is intended as a demo for a nodal procedural texture generator.

This generator is based on a fork of ricardo mendez's [libnoise port](https://github.com/ricardojmendez/LibNoise.Unity) in unity and Siccity's [Xnode](https://github.com/Siccity/xNode) project to provide an editor interface as well as a node's behavior foundation.

The following papers were used :
http://www.twandegraaf.nl/Art/Documents/Procedural%20planets%20into%20detail,%20Twan%20de%20Graaf%202012.pdf
http://jacobkeane.co.uk/wp-content/uploads/2014/06/Procedural-Planet-Generation.pdf

Sebastian Lague's videos about [procedural landmasses](https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3) and [planets](https://www.youtube.com/watch?v=QN39W020LqU&list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8) where used for the mesh generation.

## Installation
To work with this repository you will need :

* My [fork](https://github.com/Arghonot/LibNoise.Unity) of ricardo mendez's libnoise port.
* My [repository](https://github.com/Arghonot/My_CustomBehaviorTree/tree/XnodeEnhancement) about node's and graphs that comes on top of Xnode.
* [XNode](https://github.com/Siccity/xNode).
* ShaderGraph.
* [Unity 2019.3.0f6](https://unity3d.com/unity/whats-new/2019.3.0).

## Use
Since this project is still a work in progress a lot of the code is subject to a deep refactoring in the upcoming commits.
### Main scene
There are two important scenes at the moment :
* PlanetTester
It is mainly a quick way to test a planet using only a RGB [splatmap](https://en.wikipedia.org/wiki/Texture_splatting#:~:text=In%20computer%20graphics%2C%20texture%20splatting,is%20partially%20or%20completely%20transparent) (remember to alway tick the texture's "Read/write" checkbox).
* SolarSytemTest
It is the main scene that you can try [here](https://arghonot.github.io/). It generate a complete set of planets.

### Planet profile
It is a Scriptable Object that contains all the data needed for the planet generation such as the libnoise graph used or the ground/water material. 
### Planet graph
It is a Xnode graph that contain the logic for the texture generation like the amount of generator / combiners / ... and how they combine their output.


## Next updates
The next commits will contain : 
* A completely independant repository for the planet generation tool
* a character controller to move on the planet
* improved shaders
* a rich environment
* more complex planets

## Gallery
