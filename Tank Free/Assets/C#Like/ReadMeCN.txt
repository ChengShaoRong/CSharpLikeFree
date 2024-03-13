----------------------------------------------
                 C#Like
  Copyright © 2022-2024 RongRong. All right reserved.
  https://www.csharplike.com/
----------------------------------------------

欢迎试用C#Like免费版!

C#Like免费版是C#Like的阉割版,你可以在网络上任何地方下载C#Like免费版,这是一个免费资源.
C#Like免费版我已上传到 https://github.com/ChengShaoRong/CSharpLikeFree 和 https://gitee.com/cheng-shaorong/CSharpLikeFree

无尽感激开源项目C#Light : https://github.com/lightszero/CSLightStudio

C#Like免费版在Unity商店的网页链接 : https://assetstore.unity.com/packages/slug/222880
C#Like在Unity商店的网页链接 : https://assetstore.unity.com/packages/slug/222256

C#Like免费版的示例导出WebGL平台的Demo网页链接:
https://www.csharplike.com/CSharpLikeFreeDemo/index.html
C#Like的示例导出WebGL平台的Demo网页链接: 
https://www.csharplike.com/CSharpLikeDemo/index.html

在线文档网页链接 : https://www.csharplike.com/
如果你没有找到你的答案请邮件联系我,我会尽快回复你 : csharplike@qq.com


-----------------
 版本历史
-----------------
v 2024.1
功能
- 全新的绑定热更新脚本的序列化字段方式,采用和MonoBehaviour完全一样方式,绑定数据类型含:
    1 内置类型:byte/sbyte/short/ushort/int/uint/long/ulong/char/string/float/bool/double.
    2 所有基类为'LikeBehaviour'类的组件. 例如你的热更新脚本类.
    3 所有基类为'UnityEngine.Object'类. 例如MonoBehaviour/GameObject/TextAsset/Material/Texture/AudioClip/等等.
    4 部分常用的UnityEngine结构体. 暂时只添加Vector2/Vector3/Vector4/Vector2Int/Vector3Int/Matrix4x4/Rect/RectInt/Bounds/BoundsInt/Color/Color32/Quaternion/LayerMask.
    5 List<上述1~4的类型>. 例如List<int>/List<GameObject>/List<Vector3>/,等等.
- 序列化字段支持属性:
    1 [Range(1,10)]. 例如支持'byte/sbyte/short/ushort/int/uint/long/ulong/float/double'和List<byte/sbyte/short/ushort/int/uint/long/ulong/float/double>.
    2 [HideInInspector] 不显示在编辑器面板
    3 [SerializeField] 强制序列化私有字段
    4 [System.NonSerialized] 强制不序列化共有字段
    5 [Tooltip("自定义提示")] 自定义在编辑器里的悬浮提示
- 新增KissUI系列组件,支持TextMeshPro,支持精灵图集,非常方便地搭建UI系统,让你的UI的Drawcall进行质的优化.
- 新增全热更Demo项目"疯狂宝箱",为完整版和免费版打造的,包含完整功能的客户端代码和服务器代码:
    1 登录界面(含游客账号/注册/登录/修改密码/选区/选语言).
    2 主界面(含基本信息和各系统按钮)..
    3 邮件界面.
    4 背包界面.
    5 签到界面.
    6 通用获取道具界面.
修正
- 只修改场景或预制体,现在编辑器下点运行按钮,可以自动导出AssetBundle了(原来需要手动点导出或有热更新代码修改).
- 现在Lambda可以省略参数的类型了
- 修正委托不可带参数的异常
- 修正KissCSV里属性异常

v 1.8
功能
- 新增移动组件KissTween,满足简单的UI移动和路径移动需求.
修正
- 修正脚本"AOTForMerge.txt"报错.

v 1.7
功能
- 增加由非热更新代码快速调用热更新代码的函数.

v 1.6
功能
- ResourceManager现在更加全自动更加方便管理AssetBundle了,你只需在C#Like设置面板里配置最终下载地址即可.
- 编辑器里直接运行可以直接调用StreamingAssets内自动打包好的AssetBundle资源了,测试更加方便了.
修正
- 修正LikeBehaviour的behaviour异常,现在内置Demo的'飞机大战'正常了.
- 'AheadOfTime.cs'和'link.xml'现在能自动合并了.

v 1.5
功能
- 增加Sprite的绑定.
- 增加对象的null的判断.
- 把Unity出品的Platformer Microgame改造成全热更项目.
- 增加Simulation的事件.
修正
- 修正as语句异常情况.
- 允许Array出现0的情况.

v 1.4
功能
- 新增ResourceManager,最精简方式自动导出AssetBundle和读取资源.
- 新增link.xml配置.
- 使用C#Like免费版把Unity出品的Tanks! Tutorial改造成全热更项目,并列出详尽过程.
- 获取对应热更脚本类的对象实例,无需再借助HotUpdateBehaviour.
修正
- 修正LikeBehaviour修正Color和Vector3类型的数据获取.
- 非内置类型的数学计算发生异常,例如Vector3的加减乘除运算.
- 热更脚本里的类对象的数组异常.

v 1.3
功能
- KissServerFramework可以轻松架构网页了.
修正
- 修正无法从热更代码里访问非热更代码里的放在类里面的枚举.
- KissEditor编辑器修正部分错误.

v 1.2
功能
- KissJSON格式化JSON增加更可读性的接口.
- HotUpdateBehaviour绑定新增类型'Vector3/Color'.
修正
- KissServerFramework使用KissEditor编辑器,可以更方便编辑网络对象.

v 1.1
功能
- 支持CSV文件.
- 全平台支持Socket/WebSocket.
- 支持KissFramework, 一个超级简单易用的服务器框架. 更多详情请移步 https://github.com/ChengShaoRong/KissServerFramework
- 支持数字后缀'f/F/u/U/l/L'.
- 重构和优化代码.
修正
- 修复错误: KissJSON里类型为long/ulong的报错.
- 修复错误: if-else语句如果出现空白的语句会报错.

v 1.0
功能
- 支持JSON库. KissJson是唯一可同时在热更和非热更代码都可以使用的JSON库.
- 支持委托.
- 支持lambda语法.
- 支持IL2CPP.
- 面向对象. 支持partial和继承接口.
- 运算表达式. 支持+ - * / % += -= *= /= %= > >= < <= != == && || ! ++ -- ?:
- 支持命名空间, using 指令, using 别名, using static.
- 循环语句: for foreach continue break if-else return while do-while throw
- 支持自动实现的get/set访问器
- 支持Nullable外的类型.
- 支持多线程(不支持'lock'语法).
- 支持注释方式: //
- 支持自定义目标二进制文件加密.
- 其他: 支持关键字this typeof as is
