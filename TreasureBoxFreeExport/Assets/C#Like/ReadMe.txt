----------------------------------------------
                 C#Like
  Copyright © 2022-2024 RongRong. All right reserved.
  https://www.csharplike.com/
----------------------------------------------

Thank you for buying C#Like!

PLEASE NOTE that C#Like can only be legally downloaded from the Unity Asset Store.
If you've obtained C#Like via some other means then note that your license is effectively invalid,
We cannot provide support for pirated and/or potentially modified software.

PLEASE NOTE that C#LikeFree can be downloaded from anywhere, is a FREE asset. 
The C#LikeFree I had uploaded to https://github.com/ChengShaoRong/CSharpLikeFree and https://gitee.com/cheng-shaorong/CSharpLikeFree

The file tree and usage guide of the C#LikeFree is same with C#Like, so the upgrade is very convenient.

C#LikeFree on Unity AssetStore web page : https://assetstore.unity.com/packages/slug/222880
C#Like on Unity AssetStore web page : https://assetstore.unity.com/packages/slug/222256

C#LikeFree Demo: The sample of usage guide export to WebGL platform:
https://www.csharplike.com/CSharpLikeFreeDemo/index.html
C#Like Demo: The sample of usage guide export to WebGL platform:
https://www.csharplike.com/CSharpLikeDemo/index.html

Documentation can be found here: https://www.csharplike.com/

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
v 2024.3
Features
- Print tips in console panel when some features that are not supported by the free version but are supported by the full version have been used.
- New demo show how to convert none hot update project to hot update project:'Tanks! Tutorial' and 'Platformer Microgame' (Projects found in GitHub).
Fixes
- Fixes the bug that serialize field error in debug mode.
- Now can serialize field that inherited from LikeBehaviour before 'Awake' function, original only serialize it before 'Start' function.

v 2024.2
Features
- Serialize field support array. e.g. 'Transform[] transforms'.
Fixes
- Fixes the bug that can't breakpoint debug features.
- Fixes the bug that serialize field values changed not sync to memory immediately in editor.
- Fixes the bug that type is List and base class 'UnityEngine.Object'/'LikeBehaviour'. e.g. 'List<Transform> transforms'.
- Fixes the script error CS0023.

v 2024.1
Features
- New way to serialize field in hot update script, complete SAME with MonoBehaviour, support types:
    1 Build-in type : byte/sbyte/short/ushort/int/uint/long/ulong/char/string/float/bool/double.
    2 All class inherited from 'LikeBehaviour', e.g. your hot update class.
    3 All class inherited from 'UnityEngine.Object'. e.g. MonoBehaviour/GameObject/TextAsset/Material/Texture/AudioClip/ etc.
    4 Some common structs in UnityEngine. e.g. Vector2/Vector3/Vector4/Vector2Int/Vector3Int/Matrix4x4/Rect/RectInt/Bounds/BoundsInt/Color/Color32/Quaternion/LayerMask.
    5 List<Above type1~4>. e.g. List<int>/List<GameObject>/List<Vector3>/,etc.
- Serialize field support attribute:
    1 [Range(1,10)]. e.g. for 'byte/sbyte/short/ushort/int/uint/long/ulong/float/double' and List<byte/sbyte/short/ushort/int/uint/long/ulong/float/double>.
    2 [HideInInspector] Hide in Inspector panel.
    3 [SerializeField] Force serialize private field.
    4 [System.NonSerialized] Force not serialize public field.
    5 [Tooltip("Custom tips")] Custom tips in editor.
- New KissUI series components, support TextMeshPro, support SpriteAtlas, it's very convenient to build a UI system and optimize the quality of your UI's Drawcall.
- New hot update demo project "TreasureBox", designed for both the full and free versions, include FULL client and server code:
    1 Login panel (include guest/register/login/change password/select server/select language).
    2 Main scene panel(include base information and some buttons).
    3 Mail panel.
    4 Bag panel.
    5 Sign in panel.
    6 Common reward item panel.
Fixes
- Only modify scene or prefab, will auto export AssetBundle while you click run button in editor now. (Originally, it was necessary to manually export or modify hot update script)
- The type of the parameter in Lambda expression can be omitted now.
- Fix the bug that delegation cannot have parameters.
- Fix the script error about property in KissCSV in some case.

v 1.8
Features
- New component KissTween to meet the needs of simple UI mobility and path mobility.
Fixes
- Fix the script error about 'AOTForMerge.txt'.

v 1.7
Features
- User can easy to call functions in the hot update script from none hot update script now.
- Support use object initializers.
- Support inline variable declaration.
- Support expression body for constructors and methods and properties and accessors.
- Support switch expression and discard '_'.
- Support collection initializers.

v 1.6
Features
- ResourceManager is now more fully automated and convenient to manager AssetBundle, you only need to configure the final download address in the C#Like settings panel.
- Running directly in the editor will direct invocation of automatically packaged AssetBundle resources in StreamingAssets folder, making testing more convenient.
Fixes
- Fix the error while using behaviour in LikeBehaviour. The demo 'AircraftBattle' is OK now.
- Fix the error that pass the null as param to function will error.
- Fix the error that using nullable type will occur.
- 'AheadOfTime.cs' and 'link.xml' can merge automatically now.

v 1.5
Features
- HotUpdateBehaviour binding type add 'Sprite'
- Adding null judgments to objects
- Turn 'Platformer Microgame' into hot update script project with C#Like.
- Adding simulation events.
Fixes
- Fix 'as' statement error..
- Fix the override function exception to the class of Multiple inheritance LikeBehaviour.
- Allowing the occurrence of 0 in the array.
- Fix '$' syntactic sugar error when more than 2 params, support special symbol : '{{' and '}}' and '\' now..

v 1.4
Features
- ResourceManager automatically export AssetBundle and read resources in the most streamlined way
- Add config link.xml
- Turn 'Tanks! Tutorial' into hot update script project with C#LikeFree, and provide a detailed process.
- Get the class instance that inherited from LikeBehaviour in hot update script, before just using HotUpdateBehaviour instance instead of LikeBehaviour instance.
Fixes
- If it has 'yield return' inside 'while' loop in Coroutine, it'll be error.
- LikeBehaviour fix Color and Vector3 type.
- Math calc will be error if none build-in type. e.g. Vector3 addition operation
- The hotupdate class array objects in hotupdate scripts will be error

v 1.3
Features
- KissServerFramework now can easy setup a webside.
Fixs
- Fixed inability to access enumerations placed in classes in non hotupdate script from hotupdate script.
- Fix KissEditor some error.

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
- fix issue: custom get set accessor may be have error content.

v 1.0
Features
- support coroutine.
- support JSON library. KissJson easy use in normal script and in hot update script.
- support delegate.
- support lambda.
- support IL2CPP.
- object-oriented. constructor(support this/base);destructor;class inherit;interface inherit;virtual function;support partial.
- math expression. + - * / % += -= *= /= %= > >= < <= != == && || ! ++ -- & | ~ ^ &= |= ^= << >> <<= >>= ?: ?? ?.
- support namespace, using command, using alias, using sentence, using static.
- loop: for foreach continue break if-else return while do-while throw switch-case-default lock try-catch-finally yield.
- support get/set Accessor. include custom implement and automatic implement.
- support type: build-in type and Nullable type any other type.
- support enum.
- support macro and region. #if #elif #else #endif #region #endregion.
- support multi-threading.
- support code annotation: // and /**/
- support parameter modifier: ref out in params.
- support function overloading.
- support default parameters.
- support custom encrypt the binary file.
- other: this base sizeof typeof unsafe as is $ @ #pragma #warning #error volatile abstract.
