English:
"DefaultNoneHotUpdateScene.unity" : The default none hot update update scene contains only one KissGame component in the MainRoot node, which contains as few resources as possible to ensure sufficient initial package size.

"HotUpdateSceneBuildInDemo.unity" : BuildInDemo scene contains no component in the MainRoot node, that suppose you will initial your scene in "CSharpLike.HotUpdateClassBridge.OnLoadDefaultSceneDone()". Of course, you can also directly include the complete scene.

"HotUpdateSceneTreasureBox.unity" : TreasureBox scene contains no component in the MainRoot node and a loading panel, that suppose you will initial your scene in "CSharpLike.HotUpdateClassBridge.OnLoadDefaultSceneDone()". Of course, you can also directly include the complete scene.


中文:
"DefaultNoneHotUpdateScene.unity" : 默认的非热更新的场景,里面的MainRoot节点只包含一个KissGame组件,里面包含尽量精简的资源,确保初始包体足够少.

"HotUpdateSceneBuildInDemo.unity" : 内置演示的热更新的场景,里面只包含空白的基本MainRoot,预期在加载完场景后您再在"CSharpLike.HotUpdateClassBridge.OnLoadDefaultSceneDone()"内初始化你要的场景.当然你也可以直接包含完整的场景.

"HotUpdateSceneTreasureBox.unity" : 疯狂宝箱的热更新的场景,里面只包含空白的基本MainRoot和一个简陋的加载界面,预期在加载完场景后您再在"CSharpLike.HotUpdateClassBridge.OnLoadDefaultSceneDone()"内初始化你要的场景.当然你也可以直接包含完整的场景.

