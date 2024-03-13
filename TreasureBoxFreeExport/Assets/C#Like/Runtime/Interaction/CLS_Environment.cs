/// C#Light/Evil
/// 作者 疯光无限 版本见ICLS_Environment.version
/// https://github.com/lightszero/CSLightStudio
/// http://crazylights.cnblogs.com
/// 请勿删除此声明


/// Above is the 'C#Light' statement. Don't delete this statement for respect 'C#Light' author.
/// The C#Light is the reference resources of C#Like, and that ICLS_Environment.version is "0.64.1Beta".
/// Thanks to open source C#Light.

/// Here are the differences between C#Light and C#Like. 
/// More detail in https://www.csharplike.com/CSharpLike.html
/// ----------------------------------------------------------------------------------------------------
/// |     feature    |              C#Light              |                   C#Like                    |
/// ----------------------------------------------------------------------------------------------------
/// |    delegate    |              support              |                  support                    |
/// ----------------------------------------------------------------------------------------------------
/// |     lambda     |              support              |                  support                    |
/// ----------------------------------------------------------------------------------------------------
/// |                |                                   | constructor(support this/base);destructor;  |
/// |object-oriented |         interface inherit         | class inherit;interface inherit;            |
/// |                |                                   | virtual function;support partial            |
/// ----------------------------------------------------------------------------------------------------
/// |                | + - * / % += -= *= /= %= > >= <   | adds bit operation '& | ~ ^ &= |= ^= << >>  |
/// |math expression | <= != == && || ! ++ --(support i++|  <<= >>=' and full support '++ --'          |
/// |                | but not support ++i) ?: is as     |                                             |
/// ----------------------------------------------------------------------------------------------------
/// |    keyword     |               this                | this base sizeof typeof unsafe $ @ #pragma  |
/// |                |                                   | #warning #error                             |
/// ----------------------------------------------------------------------------------------------------
/// |   namespace    | Not support namespace.You must    | Support namespace,Don't need to register the|
/// |     using      | register the type in not hot      | type in not hot update script before use it |
/// |                | update script before use it in    | in hot update script.Support 'using command'|
/// |                | hot update script.                | and 'using alias' and 'using sentence'.     |
/// ----------------------------------------------------------------------------------------------------
/// |   exception    |               throw               |          adds try-catch-finally             |
/// ----------------------------------------------------------------------------------------------------
/// |                | var void bool float double char   | Adds support Nullable type and Nullable     |
/// |     type       | string byte sbyte int uint short  | math expression and coalescing operator     |
/// |                | ushort long ulong null            |  '?.' and '??'.                             |
/// ----------------------------------------------------------------------------------------------------
/// |get/set Accessor| Only support automatic implement  |      Adds can custom get set accessor       |
/// |                | get/set accessor                  |                                             |
/// ----------------------------------------------------------------------------------------------------
/// |      loop      | for foreach continue break if-else|         Adds switch-case-default            |
/// |                | return while do-while             |                                             |
/// ----------------------------------------------------------------------------------------------------
/// |                |                                   | In debug mode,you can use breakpoint and    |
/// |                |                                   | step-in to debug your code by VisualStudio. |
/// |     debug      |      Can print error sentence     | In hot update mode,you can get the stack    |
/// |                |                                   | information (include file name,class name,  |
/// |                |                                   | function name,which line) while got error.  |
/// ----------------------------------------------------------------------------------------------------
/// |                |                                   | All compilation processes are completed in  |
/// |                | It may take several seconds or    | the editor and saved as binary file. The    |
/// |                | even more than ten seconds to     |loading time at runtime is almost negligible.|
/// | compile script | compile at runtime, depending on  | Although the compilation time is basically  |
/// |                | the amount of your code,even if it| the same (it even takes more time to build  |
/// |                | has been Precompiled into token.  | cached data). The loading time gives user   |
/// |                |                                   | an excellent experience.                    |
/// ----------------------------------------------------------------------------------------------------
/// |                |                                   | You can write your hot update script just   |
/// | MonoBehaviour  |             not support           | same with in normal script. You can regard  |
/// |                |                                   | LikeBehaviour as MonoBehaviour in hot update|
/// |                |                                   | script. And support the coroutine.          |
/// ----------------------------------------------------------------------------------------------------
/// | multi-threading|             not support           |   support multi-threading and lock syntax   |
/// ----------------------------------------------------------------------------------------------------
/// | code annotation|                //                 |                // and /**/                  |
/// ----------------------------------------------------------------------------------------------------
/// |macro and region|             not support           | #if #elif #else #endif #region #endregion   |
/// ----------------------------------------------------------------------------------------------------
/// |      enum      |             not support           |                  support                    |
/// ----------------------------------------------------------------------------------------------------
/// |    parameter   |             not support           |          support 'ref out in params'        |
/// |    modifier    |                                   |                                             |
/// ----------------------------------------------------------------------------------------------------
/// |    function    |             not support           |                  support                    |
/// |   overloading  |                                   |                                             |
/// ----------------------------------------------------------------------------------------------------
/// |    default     |             not support           |                  support                    |
/// |   parameters   |                                   |                                             |
/// ----------------------------------------------------------------------------------------------------
/// |      CSV       |             not support           |                  KissCSV                    |
/// ----------------------------------------------------------------------------------------------------
/// |     JSON       |             not support           |                  KissJSON                   |
/// ----------------------------------------------------------------------------------------------------
/// |                |                                   | Easy use in hot update script, corresponding|
/// |                |                                   | C# server. This is a most simple and stupid |
/// |                |                                   | IOCP server framework component include     |
/// |                |                                   | WebSocket/Socket/HTTP/MySQL. All your logic |
/// |     Socket     |                                   | work in A single main thread, you don't need|
/// |    WebSocket   |             not support           | to worry about multi-threading problem. All |
/// |                |                                   | the heavy work process by framework in      |
/// |                |                                   | background threads. Easy to use database    |
/// |                |                                   | even never hear about SQL. Automatically    |
/// |                |                                   | synchronize data with client and database.  |
/// ----------------------------------------------------------------------------------------------------
/// | ResourceManager|             not support           | Fully automatic and more convenient         |
/// |                |                                   | management of AssetBundles                  |
/// ----------------------------------------------------------------------------------------------------
/// |    KissUI      |             not support           | The UGUI support sprite atlas now, no worry |
/// |                |                                   | about Drawcall anymore                      |
/// ----------------------------------------------------------------------------------------------------




/// 本文的前5行是'C#Light'作者 疯光无限 的声明,为了尊重'C#Light'作者,不要删除上面的声明.
/// 'C#Like'吸取了'C#Light'(版本ICLS_Environment.version为"0.64.1Beta")精华后,
/// 在巨人肩膀上进行了极大的改进和重构,扭转有人调侃C#Light只是好像(Like)C#而已的局面,
/// 让喜欢C#的程序员得心应手地用C#写出热更代码.
/// 无尽感激开源项目C#Light.

/// 下面整理了C#Light和C#Like之间的差异,
/// 更多详情可以查看 https://www.csharplike.com/CSharpLikeCN.html
/// --------------------------------------------------------------------------------------------------
/// |     功能     |              C#Light              |                  C#Like                     |
/// --------------------------------------------------------------------------------------------------
/// |     委托     |               支持                |                   支持                      |
/// --------------------------------------------------------------------------------------------------
/// |    Lambda    |               支持                |                   支持                      |
/// --------------------------------------------------------------------------------------------------
/// |   面向对象   |          类仅能继承接口           | 构造函数(支持this/base);析构函数;类继承;    |
/// |              |                                   | 虚函数;接口继承;支持partial                 |
/// --------------------------------------------------------------------------------------------------
/// |              | + - * / % += -= *= /= %= > >= <   | 多了位运算 & | ~ ^ &= |= ^= << >> <<= >>=   |
/// |  运算表达式  | <= != == && || ! ++ --(仅支持i++  | 和完全支持++ --                             |
/// |              | 不支持++i) ?: is as               |                                             |
/// --------------------------------------------------------------------------------------------------
/// |    关键字    |               this                | this base sizeof typeof unsafe $ @ #pragma  |
/// |              |                                   | #warning #error                             |
/// --------------------------------------------------------------------------------------------------
/// |   命名空间   | 不支持,有名无实,导致每种在热更代码| 完整的命名空间功能,无需事先注册类型就能直接 |
/// |    using     | 里使用的所有类型必须先非热更注册, | 使用,使用方便;using指令;using别名;using语句 |
/// |              | 非常不便且容易遗漏                |                                             |
/// --------------------------------------------------------------------------------------------------
/// |   异常处理   |               throw               |           多了try-catch-finally             |
/// --------------------------------------------------------------------------------------------------
/// |              | var void bool float double char   |                                             |
/// |     类型     | string byte sbyte int uint short  |   多了支持可空运算?及配套的合并运算?. ??    |
/// |              | ushort long ulong null            |                                             |
/// --------------------------------------------------------------------------------------------------
/// |get/set 访问器|     仅支持自动实现的get/set       |         多了可以自定义实现get/set           |
/// --------------------------------------------------------------------------------------------------
/// |   循环语句   | for foreach continue break if-else|          多了switch-case-default            |
/// |              | return while do-while             |                                             |
/// --------------------------------------------------------------------------------------------------
/// |              |                                   | 在调试模式下可以使用VisualStudio断点/步进调 |
/// |     调试     |           能打印报错语句          | 试,与正常C#一模一样;热更代码模式下当脚本出错|
/// |              |                                   | 可以打印出堆栈数据(文件名/函数名/第几行)    |
/// --------------------------------------------------------------------------------------------------
/// |              | 在运行时编译,视乎你的代码量多少可 | 所有编译过程都在编辑器里完成,保存为二进制文 |
/// |   编译脚本   | 能会花费几秒甚至超过十几秒,即使已 | 件,运行时加载时间几乎可以忽略,虽然编译时间也|
/// |              | 经预编译为Token了                 | 基本一样(首次编译甚至花费更多时间建立缓存数 |
/// |              |                                   | 据),但加载时候给玩家极好的体验              |
/// --------------------------------------------------------------------------------------------------
/// |              |                                   | LikeBehaviour仿照MonoBehaviour,让热更脚本直 |
/// |MonoBehaviour |               不支持              | 接按MonoBehaviour的方式写代码,热更脚本就像是|
/// |              |                                   | 继承MonoBehaviour一样,而且完美支持协程      |
/// --------------------------------------------------------------------------------------------------
/// |    多线程    |               不支持              |             支持且多了lock语法              |
/// --------------------------------------------------------------------------------------------------
/// |     注释     |                //                 |                 //和/**/                    |
/// --------------------------------------------------------------------------------------------------
/// | 宏定义和区域 |               不支持              | #if #elif #else #endif #region #endregion   |
/// --------------------------------------------------------------------------------------------------
/// |     枚举     |               不支持              |                   支持                      |
/// --------------------------------------------------------------------------------------------------
/// |  参数修饰符  |               不支持              |           支持ref out in params             |
/// --------------------------------------------------------------------------------------------------
/// |   函数重载   |               不支持              |                   支持                      |
/// --------------------------------------------------------------------------------------------------
/// |   默认参数   |               不支持              |                   支持                      |
/// --------------------------------------------------------------------------------------------------
/// |      CSV     |               不支持              |                 KissCSV                     |
/// --------------------------------------------------------------------------------------------------
/// |     JSON     |               不支持              |                 KissJSON                    |
/// --------------------------------------------------------------------------------------------------
/// |              |                                   | 超简洁的可热更的接口,提供KissServerFramework|
/// |    Socket    |               不支持              | (最简洁易用的IOCP服务器框架,用户逻辑单线程, |
/// |   WebSocket  |                                   | 其他都多线程,面向对象,包含WebSocket/Socket  |
/// |              |                                   | /HTTP/MySQL,无需了解SQL都可轻松使用数据库,自|
/// |              |                                   | 动和客户端和数据库同步数据)                 |
/// --------------------------------------------------------------------------------------------------
/// |              |                                   | 全自动更加方便管理AssetBundle,设置场景的    |
/// |ResourceManager|             不支持               | AssetBundle后,你只需在C#Like设置面板里配置最|
/// |              |                                   | 终下载地址即可                              |
/// --------------------------------------------------------------------------------------------------
/// |    KissUI    |               不支持              | 支持精灵图集的UGUI,让你不再为Drawcall烦恼   |
/// --------------------------------------------------------------------------------------------------
