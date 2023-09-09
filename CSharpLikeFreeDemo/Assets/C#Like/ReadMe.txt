----------------------------------------------
                 C#Like
  Copyright © 2022-2023 RongRong. All right reserved.
  https://www.csharplike.com/
----------------------------------------------

Thank you for on trial C#LikeFree!

PLEASE NOTE that C#LikeFree can be downloaded from anywhere, is a FREE asset. 
If you do not meet the conditions of Unity's personal license, you cannot use it for commercial purposes. 

C#LikeFree is the lite version of the C#Like, it missing some features from full version.
The file tree and usage guide of the C#LikeFree is same with C#Like, so the upgrade is very convenient.
The C#LikeFree I had uploaded to https://github.com/ChengShaoRong/CSharpLikeFree and https://gitee.com/cheng-shaorong/CSharpLikeFree

C#LikeFree on Unity AssetStore web page : https://assetstore.unity.com/packages/slug/222880
C#Like on Unity AssetStore web page : https://assetstore.unity.com/packages/slug/222256

C#LikeFree Demo: The sample of usage guide export to WebGL platform:
https://www.csharplike.com/CSharpLikeFreeDemo/index.html
C#Like Demo: The sample of usage guide export to WebGL platform:
https://www.csharplike.com/CSharpLikeDemo/index.html

Documentation can be found here: https://www.csharplike.com/CSharpLikeFree.html

If you have any questions, please browse this web page : https://www.csharplike.com/
If you don't find an answer, mail to me: csharplike@qq.com

Specail thanks to open source C#Light : https://github.com/lightszero/CSLightStudio


--------------------
 How To Update C#Like
--------------------

1. In Unity, File -> New Scene
2. Import C#Like from the updated Unity Package.

-----------------
 Version History
-----------------
-----------------
v 1.6
Features
- ResourceManager is now more fully automated and convenient to manager AssetBundle, you only need to configure the final download address in the C#Like settings panel.
- Running directly in the editor will direct invocation of automatically packaged AssetBundle resources in StreamingAssets folder, making testing more convenient.
Fixes
- Fix the error while using behaviour in LikeBehaviour. The demo 'AircraftBattle' is OK now.

v 1.5
Features
- HotUpdateBehaviour binding type add 'Sprite'
- Adding null judgments to objects
- Turn 'Platformer Microgame' into hot update script project with C#Like.
- Adding simulation events.
Fixes
- Fix 'as' statement error..
- Allowing the occurrence of 0 in the array.

v 1.4
Features
- ResourceManager automatically export AssetBundle and read resources in the most streamlined way
- Add config link.xml
- Turn 'Tanks! Tutorial' into hot update script project with C#LikeFree, and provide a detailed process.
- Get the class instance that inherited from LikeBehaviour in hot update script, before just using HotUpdateBehaviour instance instead of LikeBehaviour instance.
Fixes
- LikeBehaviour fix Color and Vector3 type.
- Math calc will be error if none build-in type. e.g. Vector3 addition operation
- The hotupdate class array objects in hotupdate scripts will be error

v 1.3
Features
- KissServerFramework now can easy setup a webside.
Fixs
- Fixed inability to access enumerations placed in classes in non hotupdate script from hotupdate script.
- Fixed KissEditor some error.

v 1.2
Features
- KissJSON formatting JSON string more readability.
- HotUpdateBehaviour binding type add 'Vector3/Color'.
Fixs
- KissServerFramework using a KissEditor, can visualize edit net object.

v 1.1
Features
- support CSV file.
- support Socket/WebSocket in all platform.
- support KissFramework that simple use of server framework. More detail in github
- support suffix with 'f/F/u/U/l/L' for number.
- refactoring and optimizing code.
Fixs
- fix issue: KissJSON long/ulong type number.
- fix issue: got error while if-else have empty content.

v 1.0
Features
- support JSON library. KissJson easy use in normal script and in hot update script.
- support delegate.
- support lambda.
- support IL2CPP.
- object-oriented. support partial and interface inherit.
- math expression. + - * / % += -= *= /= %= > >= < <= != == && || ! ++ -- ?:
- support namespace, using command, using alias, using static.
- loop: for foreach continue break if-else return while do-while throw
- support automatic implement get/set Accessor
- support type: exclude Nullable type.
- support multi-threading (exclude 'lock').
- support code annotation: //
- support custom encrypt the binary file.
- other: this typeof as is
