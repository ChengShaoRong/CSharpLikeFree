# Directory structure
## CSharpLikeFreeDemo
-  Used for exporting [C#LikeFree Demo](https://www.csharplike.com/CSharpLikeFreeDemo/index.html), the initial package can load the hot update project.
- Guide:
> * Open the project, and switch to WebGL platform
> * Menu "File"->"C#Like"->"Build Settings..."->"Build"
> * Export WebGL project to folder "CSharpLikeFreeDemo/CSharpLikeFreeDemo"
> * Put the AssetBundles to the folder "CSharpLikeFreeDemo/CSharpLikeFreeDemo", the AssetBundles is load from the below 3 projects
> * Put the final WebGL folder "CSharpLikeFreeDemo" to your HTTP server. e.g. The folder is "D:\xampp\htdocs\CSharpLikeFreeDemo" in Apache Web server. The folder is "C:\MyServer\wwwroot\CSharpLikeFreeDemo" in KissServerFramework Web server.
> * Config the files 'CSharpLikeFreeDemo\AssetBundles\gamesWebGL.json','CSharpLikeFreeDemo\AssetBundles\CsharpLike\WebGL\config.json','CSharpLikeFreeDemo\AssetBundles\MicrogameFree\WebGL\config.json','CSharpLikeFreeDemo\AssetBundles\Tank\WebGL\config.json' to your web link

## CSharpLikeFree
-  Used for exporting the AssetBundle in [C#LikeFree Demo](https://www.csharplike.com/CSharpLikeFreeDemo/index.html), "C#Like Demo",include hot update binary file and other source.
- Guide:
> * Open the project, and switch to WebGL platform
> * Menu "Menu"->"C#Like"->"C#Like Setting"->"Rebuild Scripts"
> * Finally export to the folder "CSharpLikeFree\AssetBundles\CsharpLike", include 3 files, and put them to "CSharpLikeFreeDemo\CSharpLikeFreeDemo\AssetBundles"

## TankFree
-  Used for exporting the AssetBundle in [C#LikeFree Demo](https://www.csharplike.com/CSharpLikeFreeDemo/index.html), [Tanks! Tutorial](https://assetstore.unity.com/packages/essentials/tutorial-projects/tanks-tutorial-46209), include hot update binary file and other source. 
- Guide:
> * Open the project, and switch to WebGL platform
> * Menu "Menu"->"C#Like"->"C#Like Setting"->"Rebuild Scripts"
> * Finally export to the folder "TankFree\AssetBundles\Tank", include 3 files, and put them to "CSharpLikeFreeDemo\CSharpLikeFreeDemo\AssetBundles"

## PlatformerMicrogameFree
-  Used for exporting the AssetBundle in [C#LikeFree Demo](https://www.csharplike.com/CSharpLikeFreeDemo/index.html), [Platformer Microgame](https://assetstore.unity.com/packages/templates/platformer-microgame-151055), include hot update binary file and other source. 
- Guide:
> * Open the project, and switch to WebGL platform
> * Menu "Menu"->"C#Like"->"C#Like Setting"->"Rebuild Scripts"
> * Finally export to the folder "PlatformerMicrogameFree\AssetBundles\MicrogameFree", include 3 files, and put them to "CSharpLikeFreeDemo\CSharpLikeFreeDemo\AssetBundles"