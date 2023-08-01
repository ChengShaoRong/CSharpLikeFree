# 目录结构
## CSharpLikeFreeDemo
-  用于导出[C#LikeFree演示](https://www.csharplike.com/CSharpLikeFreeDemo/index.html)的里面的初始包体环境
- 步骤:
> * Unity打开项目,切换到WebGL平台
> * 菜单"File"->"C#Like"->"Build Settings..."->"Build"
> * 最终导出WebGL项目到目录"CSharpLikeFreeDemo/CSharpLikeFreeDemo"
> * 依次把下面3个项目导出的放到刚刚导出的目录内
> * 把最终WebGL目录"CSharpLikeFreeDemo"放入你的网页服务器里,例如Apache服务器的放"D:\xampp\htdocs\CSharpLikeFreeDemo",例如我用KissServerFramework服务器的放"C:\MyServer\wwwroot\CSharpLikeFreeDemo"
> * 配置'CSharpLikeFreeDemo\AssetBundles\gamesWebGL.json','CSharpLikeFreeDemo\AssetBundles\CsharpLike\WebGL\config.json','CSharpLikeFreeDemo\AssetBundles\MicrogameFree\WebGL\config.json','CSharpLikeFreeDemo\AssetBundles\Tank\WebGL\config.json'里面的下载链接为你自己的.

## CSharpLikeFree
-  用于导出[C#LikeFree演示](https://www.csharplike.com/CSharpLikeFreeDemo/index.html)的里面的"C#Like Demo"用的AssetBundle, 包含热更新代码二进制文件及其他资源文件.
- 步骤:
> * Unity打开项目,切换到WebGL平台
> * 菜单"Menu"->"C#Like"->"C#Like Setting"->"Rebuild Scripts"
> * 最终生成最终目录"CSharpLikeFree\AssetBundles\CsharpLike",里面包含3个文件, 最后将放到目录"CSharpLikeFreeDemo\CSharpLikeFreeDemo\AssetBundles"

## TankFree
-  用于导出[C#LikeFree演示](https://www.csharplike.com/CSharpLikeFreeDemo/index.html)的里面的[Tanks! Tutorial](https://assetstore.unity.com/packages/essentials/tutorial-projects/tanks-tutorial-46209)用的AssetBundle, 包含热更新代码二进制文件及其他资源文件.
- 步骤:
> * Unity打开项目,切换到WebGL平台
> * 菜单"Menu"->"C#Like"->"C#Like Setting"->"Rebuild Scripts"
> * 最终生成最终目录"TankFree\AssetBundles\Tank",里面包含3个文件, 最后将放到目录"CSharpLikeFreeDemo\CSharpLikeFreeDemo\AssetBundles"

## PlatformerMicrogameFree
-  用于导出[C#LikeFree演示](https://www.csharplike.com/CSharpLikeFreeDemo/index.html)的里面的[Platformer Microgame](https://assetstore.unity.com/packages/templates/platformer-microgame-151055)用的AssetBundle, 包含热更新代码二进制文件及其他资源文件.
- 步骤:
> * Unity打开项目,切换到WebGL平台
> * 菜单"Menu"->"C#Like"->"C#Like Setting"->"Rebuild Scripts"
> * 最终生成最终目录"PlatformerMicrogameFree\AssetBundles\MicrogameFree",里面包含3个文件, 最后将放到目录"CSharpLikeFreeDemo\CSharpLikeFreeDemo\AssetBundles"