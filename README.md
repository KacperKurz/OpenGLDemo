# OpenGL Project  
Demo of a graphics engine written in C#  

Since the scene files weigh over 1GB, I am not including them in the repository but instead providing a link to the source and import instructions.  

## Features:  
* Loading models along with textures from `.obj` files  
* Camera movement system in 3D space  
* Dynamic adaptation of the application to window width and height  
* Cubemap textures (Skyboxes)  
* Blinn-Phong shading model  
* Textures  
* Specular Mapping  
* Normal Mapping  
* Gamma correction  
* Dynamic engine settings toggling using keyboard shortcuts  
* A scene consisting of:  
  * 2,832,120 triangles  
  * 203 albedo, normal, and specular textures  
  * 1,591 unique meshes  
* Compatibility with multiple systems (tested on Windows and Linux distributions)  
* Compatibility with NVIDIA and AMD graphics cards/drivers  

## Installation:  
* Download the repository  
* Install the required NuGet packages  
* Download the [scene](https://developer.nvidia.com/orca/amazon-lumberyard-bistro)  
* Import the scene into Blender  
* Export the scene to a `"bistro.obj"` file with the correct settings and place it in the `"models"` directory  
* ![settings](settings.png)  
* Convert each `.dds` file to a `.png` file with the same name and place them in the `"Textures"` directory (I recommend using [ImageMagick](https://imagemagick.org/index.php))  
* Run the project  

## Controls  
| Key                  | Action                                 |  
|----------------------|---------------------------------------|  
| W                    | Move forward                         |  
| S                    | Move backward                        |  
| A                    | Move left                            |  
| D                    | Move right                           |  
| Space               | Move up                              |  
| LCtrl               | Move down                            |  
| LShift              | Speed modifier                       |  
| Mouse scroll up     | Decrease camera FOV                  |  
| Mouse scroll down   | Increase camera FOV                  |  
| TAB                 | Release cursor and pause simulation  |  
| T                   | Toggle texture usage                 |  
| N                   | Toggle normal maps usage             |  
| M                   | Toggle specular maps usage           |  
| B                   | Toggle skybox usage                  |  
| L                   | Toggle lighting and shading          |  