/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace CSharpLikeEditor
{
    [InitializeOnLoad]
    public class CSL_Setting : EditorWindow, IActiveBuildTargetChanged
    {
        [MenuItem("Window/C#Like")]
        static void ShowWindow()
        {
            GetWindow<CSL_Setting>(true, "C#Like Setting");

            Init();
        }
        static bool compileSuccess = false;
        /// <summary>
        /// You should custom here
        /// </summary>
        static BuildTarget GetDefaultBuildTarget()
        {
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
#if UNITY_STANDALONE_OSX
            buildTarget = BuildTarget.StandaloneOSX;
#endif
#if UNITY_STANDALONE_WIN
            buildTarget = BuildTarget.StandaloneWindows;
#endif
#if UNITY_STANDALONE_LINUX
            buildTarget = BuildTarget.StandaloneLinux64;
#endif
#if UNITY_WII
            buildTarget = BuildTarget.WiiU;
#endif
#if UNITY_IOS
            buildTarget = BuildTarget.iOS;
#endif
#if UNITY_ANDROID
            buildTarget = BuildTarget.Android;
#endif
#if UNITY_PS4
            buildTarget = BuildTarget.PS4;
#endif
#if UNITY_PS5
            buildTarget = BuildTarget.PS5;
#endif
#if UNITY_XBOXONE
            buildTarget = BuildTarget.XboxOne;
#endif
#if UNITY_TIZEN
            buildTarget = BuildTarget.Tizen;
#endif
#if UNITY_TVOS
            buildTarget = BuildTarget.tvOS;
#endif
#if UNITY_WSA || UNITY_WSA_10_0 || UNITY_WINRT
            buildTarget = BuildTarget.WSAPlayer;
#endif
#if UNITY_WEBGL
            buildTarget = BuildTarget.WebGL;
#endif
            return buildTarget;
        }

        static string GetCopyToPath()
        {
            string path = Application.streamingAssetsPath + "/AssetBundles";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/" + strProductName + "/" + buildTarget.ToString();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        static string GetScriptOutputFileName()
        {
            int i = strScriptOutputFile.LastIndexOf('/');
            if (strScriptOutputFile.LastIndexOf('\\') > i)
                i = strScriptOutputFile.LastIndexOf('\\');
            if (i >= 0)
                return strScriptOutputFile.Substring(i + 1);
            return strScriptOutputFile;
        }
        static void GenerateScriptOutputFileMeta()
        {
            if (File.Exists(strScriptOutputFile + ".meta"))
            {
                string strOrg = File.ReadAllText(strScriptOutputFile + ".meta");
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (string str in strOrg.Split('\n'))
                {
                    if (str.StartsWith("  assetBundleName: "))
                    {
                        sb.Append("  assetBundleName: code");
                    }
                    else if (str.StartsWith("  assetBundleVariant: "))
                    {
                        sb.Append("  assetBundleVariant: ab");
                    }
                    else
                        sb.Append(str);
                    sb.Append('\n');
                }
                string strNew = sb.ToString();
                if (strOrg != strNew)
                    File.WriteAllText(strScriptOutputFile + ".meta", strNew, new System.Text.UTF8Encoding(false));
            }
        }
        static void CopyToStreamingAssets()
        {
            //CopyTo(strScriptOutputFile, GetCopyToPath() + "/" + GetScriptOutputFileName());//This will copy 'code.bytes' to StreamingAssets

            GenerateScriptOutputFileMeta();
            forceExportAssetBundle = true;
        }
        static bool forceExportAssetBundle = false;
        class MyProfile : Profile
        {
            CSharpLike.Internal.CSL_VirtualMachine vm;
            public MyProfile()
            {
                vm = new CSharpLike.Internal.CSL_VirtualMachine();
                vm.onRegType += onRegType;
                CSharpLike.HotUpdateManager.vm = vm;
                CSharpLike.HotUpdateManager.RegisterBuildIn();
                dataPath = Application.dataPath;
                foreach (var item in vm.typesss)
                {
                    types.Add(item.Value, item.Key.type);
                }
            }
            public override object Execute(byte[] buff)
            {
                try
                {
                    var stream = new CSharpLike.Internal.CSL_StreamRead(buff);
                    var code = stream.ReadCode();
                    var content = vm.CreateContent();
                    var value = code.Execute(content);
                    return value?.value;
                }
                catch (Exception err)
                {
                    Debug.LogError(err);
                }
                return null;
            }

            public override void OnSuccess()
            {
                compileSuccess = true;
                CopyToStreamingAssets();
            }
            public override Type GetMyType(string typeName)
            {
                Type t = CSharpLike.Internal.CSL_VirtualMachine.GetMyType(typeName);
                if (t != null)
                    return t;
                t = Type.GetType(typeName);
                if (t != null)
                    return t;
                return base.GetMyType(typeName);
            }
            public override List<string> GetAssemblys()
            {
                List<string> list = new List<string>();
                for (int i = 0; i < assemblysBuildIn.Count; i++)
                    list.Add(assemblysBuildIn[i]);
                for (int i = 0; i < assemblys.Count; i++)
                {
                    if (assemblysValue[i])
                        list.Add(assemblys[i]);
                }
                return list;
            }

            public override byte[] Compress(byte[] buff)
            {
                return CSharpLike.CSL_Utils.Compress(buff);
            }
            public override byte[] Decompress(byte[] buff)
            {
                return CSharpLike.CSL_Utils.Decompress(buff);
            }
            public override byte[] Encrypt(byte[] key, byte[] iv, byte[] buff)
            {
                return CSharpLike.CSL_Utils.Encrypt(key, iv, buff);
            }
            public override byte[] Decrypt(byte[] key, byte[] iv, byte[] buff)
            {
                return CSharpLike.CSL_Utils.Decrypt(key, iv, buff);
            }

            public override CultureInfo cultureInfoForConvertSingleAndDouble
            {
                get
                {
                    return CSharpLike.MyCustomConfig.cultureInfoForConvertSingleAndDouble;
                }
            }
        }

        static CSL_Setting()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            OnConfigChange();
        }
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                Init();
                OnConfigChange();
                if (!isDebug && isAutoPackScriptWhenPlay)
                {
                    OnBuildScripts(false, false);
                    if (!compileSuccess)//stop run mode if got error in compiling
                        EditorApplication.ExitPlaymode();
                }
            }
        }
        static string dataPath;
        static CSL_Config config;
        static void CheckFolder(string strFile)
        {
            int i = strFile.LastIndexOf('/');
            if (i > 0)
                strFile = strFile.Substring(0, i);
            if (!Directory.Exists(strFile))
                Directory.CreateDirectory(strFile);
        }
        static void Init()
        {
            if (config != null)
                return;
            dataPath = Application.dataPath;
            config = new CSL_Config(Application.dataPath + "/C#Like/Editor/Config.txt");
            strScriptSourceDir = config.GetString("ScriptSourceDir", "Assets/C#Like/HotUpdateScripts");
            if (string.IsNullOrEmpty(strScriptSourceDir))
                strScriptSourceDir = "Assets/C#Like/HotUpdateScripts";
            strScriptOutputFile = config.GetString("ScriptOutputFile", "Assets/C#Like/output/code.bytes");
            if (string.IsNullOrEmpty(strScriptOutputFile))
                strScriptOutputFile = "Assets/C#Like/output/code.bytes";
            CheckFolder(strScriptOutputFile);
            strDefaultScene = config.GetString("DefaultScene", "Assets/C#Like/Scenes/BuildInDemoScene.unity");
            if (string.IsNullOrEmpty(strDefaultScene))
                strDefaultScene = "Assets/C#Like/Scenes/BuildInDemoScene.unity";
            isDebug = config.GetBoolean("*isDebug", false);
            isAutoPackScriptWhenPlay = config.GetBoolean("*isAutoPackScriptWhenPlay", true);
            buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), config.GetString("*BuildTarget", GetDefaultBuildTarget().ToString()));
            strProductName = config.GetString("ProductName");
            if (string.IsNullOrEmpty(strProductName))
                strProductName = Application.productName;
            strVersion = config.GetString("Version");
            if (string.IsNullOrEmpty(strVersion))
                strVersion = Application.version;
            strDisplayName = config.GetString("DisplayName");
            if (string.IsNullOrEmpty(strDisplayName))
                strDisplayName = strProductName;
            strDownloadPath = config.GetString("DownloadPath", "");
            isAssetBundleLZ = config.GetBoolean("*isAssetBundleLZ", false);
            int customJSONCount = config.GetInt("*customJSONCount", 0);
            for (int i = 0; i < customJSONCount; i++)
            {
                string key = config.GetString("customJSONKey" + i, "");
                string value = config.GetString("customJSONValue" + i, "");
                if (!string.IsNullOrEmpty(key))
                    customJSONs[key] = value;
            }
            SaveDownloadConfig();
            RefreshAssemblys();
            OnConfigChange();
        }
        static void SaveDownloadConfig()
        {
            string strFile = Application.dataPath + "/C#Like/Resources/CSharpLikeConfig.json";
            CSharpLike.JSONData json = null;
            string strOld = null;
            if (File.Exists(strFile))
            {
                strOld = File.ReadAllText(strFile);
                json = CSharpLike.KissJson.ToJSONData(strOld);
            }
            if (json == null)
                json = CSharpLike.JSONData.NewDictionary();
            string _strDownloadPath = strDownloadPath;
            if (string.IsNullOrEmpty(_strDownloadPath))
                _strDownloadPath = "StreamingAssets";
            else if (_strDownloadPath.EndsWith("/"))
                _strDownloadPath = _strDownloadPath.Substring(0, _strDownloadPath.Length - 1);
            json[buildTarget.ToString()] = _strDownloadPath + "/games" + buildTarget + ".json";
            string strNew = json.ToJson(true);
            if (strNew != strOld)
                File.WriteAllText(strFile, strNew, new System.Text.UTF8Encoding(false));
        }
        static void RefreshAssemblys()
        {
            assemblys = GetAssemblys();
            assemblysValue = new List<bool>();
            foreach (string str in GetAssemblys())
                assemblysValue.Add(config.GetBoolean("*" + str, true));
        }
        static BuildTarget buildTarget;
        public static string strScriptSourceDir;
        public static string strScriptOutputFile;
        public static string strProductName;
        static string strVersion;
        static string strDisplayName;
        static string strDefaultScene;
        static string strDownloadPath;
        static Dictionary<string, string> customJSONs = new Dictionary<string, string>();
        static bool isAssetBundleLZ;
        static bool isDebug;
        static bool isAutoPackScriptWhenPlay;
        static List<string> assemblysBuildIn;
        static List<string> assemblys;
        static List<bool> assemblysValue;
        Vector2 scvPos = Vector2.zero;
        void OnGUI()
        {
            Init();
            BuildTarget buildTarget = GetDefaultBuildTarget();
            EditorGUILayout.BeginVertical();
            Rect rect = EditorGUILayout.BeginHorizontal();
            strScriptSourceDir = EditorGUILayout.TextField(new GUIContent("Hot Update Script Folder:", "All scripts in this folder will consider as hot update scripts.Default folder is 'Assets/C#Like/HotUpdateScripts'"), strScriptSourceDir, GUILayout.MaxWidth(500f));
            EditorGUILayout.EndHorizontal();
            if (mouseOverWindow == this && (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited))
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.DragUpdated)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    else
                    {
                        Focus();
                        if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                        {
                            string strPath = DragAndDrop.paths[0];
                            if (Directory.Exists(strPath))
                                strScriptSourceDir = strPath;
                            else if (File.Exists(strPath))
                            {
                                int index = strPath.LastIndexOf("/");
                                if (index > 0)
                                    strScriptSourceDir = strPath.Substring(0, index);
                            }
                            config.SetString("ScriptSourceDir", strScriptSourceDir);
                            config.Save();
                        }
                    }
                }
            }
            EditorGUILayout.Space();

            rect = EditorGUILayout.BeginHorizontal();
            strScriptOutputFile = EditorGUILayout.TextField(new GUIContent("Hot Update Script Save File:", "The hot update scripts will finally compiler and save to this path."), strScriptOutputFile, GUILayout.MaxWidth(500f));
            EditorGUILayout.EndHorizontal();
            if (mouseOverWindow == this && (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited))
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.DragUpdated)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    else
                    {
                        Focus();
                        if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                        {
                            string strPath = DragAndDrop.paths[0];
                            if (File.Exists(strPath))
                                strScriptOutputFile = strPath;
                            else if (Directory.Exists(strPath))
                            {
                                strScriptOutputFile = strPath + "/code.bytes";
                            }
                            CheckFolder(strScriptOutputFile);
                            config.SetString("ScriptOutputFile", strScriptOutputFile);
                            config.Save();
                        }
                    }
                }
            }
            EditorGUILayout.Space();

            rect = EditorGUILayout.BeginHorizontal();
            List<GUIContent> btShows = new List<GUIContent>();
            List<int> targets = new List<int>();
            foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
            {
                targets.Add((int)target);
                btShows.Add(new GUIContent(target.ToString()));
            }
            buildTarget = (BuildTarget)EditorGUILayout.IntPopup(new GUIContent("Build Target", "Choose target platform to build for.\n Will create AssetBundle in folder 'AssetBundles/[platform]/'.\nPlease use 'AssetBundle Browser' if you want more function."), (int)buildTarget, btShows.ToArray(), targets.ToArray(), GUILayout.MaxWidth(500f));
            EditorGUILayout.EndHorizontal();

            rect = EditorGUILayout.BeginHorizontal();
            bool _isAssetBundleLZ = EditorGUILayout.Toggle(new GUIContent("Compression LZ4", "Using chunk base compression(LZ4), otherwide using standard compression(LZMA)."), isAssetBundleLZ);
            if (_isAssetBundleLZ != isAssetBundleLZ)
            {
                isAssetBundleLZ = _isAssetBundleLZ;
                config.SetBoolean("*isAssetBundleLZ", isAssetBundleLZ);
                config.Save();
            }
            EditorGUILayout.EndHorizontal();
            rect = EditorGUILayout.BeginHorizontal();
            string _strProductName = EditorGUILayout.TextField(new GUIContent("Product Name:", "Product name of the AssetBundle, for distinguish between different games."), strProductName, GUILayout.MaxWidth(500f));
            if (_strProductName != strProductName)
            {
                strProductName = _strProductName;
                config.SetString("ProductName", strProductName);
                config.Save();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            bool bValue = EditorGUILayout.Toggle(new GUIContent("is debug mode", "you can direct debug just like the native C# script if in debug mode, is the quick debug way in development stage."), isDebug);
            if (bValue != isDebug)
            {
                isDebug = bValue;
                config.SetBoolean("*isDebug", isDebug);
                config.Save();
                if (EditorApplication.isPlaying)
                    EditorApplication.isPlaying = false;
                OnConfigChange();
            }
            if (!isDebug)
            {
                bValue = EditorGUILayout.Toggle(new GUIContent("   automatic compile", "automatic compile hot update script when enter play mode, only take effect while not in debug mode. That equal to automatic click 'Compile Scripts' button before run. And it will stop running mode if get error while compiling."), isAutoPackScriptWhenPlay);
                if (bValue != isAutoPackScriptWhenPlay)
                {
                    isAutoPackScriptWhenPlay = bValue;
                    config.SetBoolean("*isAutoPackScriptWhenPlay", isAutoPackScriptWhenPlay);
                    config.Save();
                }
            }
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Compile Scripts", "Compile the hot update script with the local cache info which create by last success compile,that can speed up compile."), GUILayout.MaxWidth(120f)))
                OnBuildScripts(true, false);
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Rebuild Scripts", "Compile the hot update script ignore the local cache info which create by last success compile"), GUILayout.MaxWidth(120f)))
                OnBuildScripts(true, true);
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Generate RSA", "Generate RSA private key and public key for socket security"), GUILayout.MaxWidth(120f)))
                OnGenerateRSA();
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Export AssetBundle", "Export AssetBundle files"), GUILayout.MaxWidth(120f)))
                OnExportAssetBundle();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //JSON
            EditorGUILayout.BeginVertical();
            EditorGUILayout.SelectableLabel("Config JSON:");
            rect = EditorGUILayout.BeginHorizontal();
            string _strVersion = EditorGUILayout.TextField(new GUIContent("Version:", "Display name of the AssetBundle, for distinguish between different games."), strVersion, GUILayout.MaxWidth(500f));
            if (_strVersion != strVersion)
            {
                strVersion = _strVersion;
                config.SetString("Version", strVersion);
                config.Save();
            }
            EditorGUILayout.EndHorizontal();
            rect = EditorGUILayout.BeginHorizontal();
            string _strDisplayName = EditorGUILayout.TextField(new GUIContent("Display Name:", "Display name of the AssetBundle, for distinguish between different games."), strDisplayName, GUILayout.MaxWidth(500f));
            if (_strDisplayName != strDisplayName)
            {
                strDisplayName = _strDisplayName;
                config.SetString("DisplayName", strDisplayName);
                config.Save();
            }
            EditorGUILayout.EndHorizontal();
            rect = EditorGUILayout.BeginHorizontal();
            string _strDefaultScene = EditorGUILayout.TextField(new GUIContent("Default Scene:", "When you loaded the AssetBundle, it will use this scene as default scene."), strDefaultScene, GUILayout.MaxWidth(500f));
            if (mouseOverWindow == this && (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited))
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.DragUpdated)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    else
                    {
                        Focus();
                        if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                        {
                            string strPath = DragAndDrop.paths[0];
                            if (File.Exists(strPath) && strPath.EndsWith(".unity"))
                            {
                                _strDefaultScene = strPath;
                            }
                        }
                    }
                }
            }
            if (_strDefaultScene != strDefaultScene)
            {
                strDefaultScene = _strDefaultScene;
                config.SetString("DefaultScene", strDefaultScene);
                config.Save();
            }
            EditorGUILayout.EndHorizontal();
            rect = EditorGUILayout.BeginHorizontal();
            string _strDownloadPath = EditorGUILayout.TextField(new GUIContent("Download Path:", $"Download path for the 'games{buildTarget}.json', default value is 'StreamingAssets/AssetBundles'."), strDownloadPath, GUILayout.MaxWidth(500f));
            if (_strDownloadPath != strDownloadPath)
            {
                strDownloadPath = _strDownloadPath;
                config.SetString("DownloadPath", strDownloadPath);
                config.Save();
                SaveDownloadConfig();
            }
            EditorGUILayout.EndHorizontal();
            rect = EditorGUILayout.BeginHorizontal();
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (var one in customJSONs)
            {
                keys.Add(one.Key);
                values.Add(one.Value);
            }
            bool bModifyCustomJSON = false;
            rect = EditorGUILayout.BeginVertical();
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                string value = values[i];
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                string _key = EditorGUILayout.TextField(new GUIContent("Modify Key:", $"You can edit the JSON key '{key}' and corresponding value is '{value}'.\nIt'll be removed if you set it empty."), key, GUILayout.MaxWidth(200f));
                if (_key != key)
                {
                    keys[i] = _key;
                    bModifyCustomJSON = true;
                }
                string _value = EditorGUILayout.TextField(new GUIContent("Modify Value:", $"You can edit the JSON value '{value}' and corresponding key is '{key}'"), value, GUILayout.MaxWidth(300f));
                if (_value != value)
                {
                    values[i] = _value;
                    bModifyCustomJSON = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            rect = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("New JSON Key", $"New a custom JSON, and then you can edit it"), GUILayout.MaxWidth(150f)))
            {
                string newKey = "CustomKey";
                int count = 0;
                while (customJSONs.ContainsKey(newKey))
                {
                    count++;
                    newKey = "CustomKey" + count;
                }
                keys.Add(newKey);
                values.Add("");
                bModifyCustomJSON = true;
            }
            if (bModifyCustomJSON)
            {
                customJSONs.Clear();
                for (int i = 0; i < keys.Count; i++)
                {
                    string key = keys[i];
                    if (!string.IsNullOrEmpty(key))
                        customJSONs[key] = values[i];
                }
                config.SetInt("*customJSONCount", customJSONs.Count);
                int count = 0;
                foreach (var one in customJSONs)
                {
                    config.SetString("customJSONKey" + count, one.Key);
                    config.SetString("customJSONValue" + count, one.Value);
                    count++;
                }
                config.Save();
            }
            if (GUILayout.Button(new GUIContent("Merge JSON manually", $"Merge config JSON to 'AssetBundles/games{buildTarget}.json'. \nYou need click this button manually when you has some games and merge them into ONE JSON file."), GUILayout.MaxWidth(200f)))
                OnMergeJSON(Application.dataPath.Substring(0, Application.dataPath.Length - 7));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();


            EditorGUILayout.Space();
            EditorGUILayout.SelectableLabel("Assembly for hot update script call:");
            scvPos = EditorGUILayout.BeginScrollView(scvPos);
            EditorGUI.BeginDisabledGroup(true);
            for (int i = 0; i < assemblysBuildIn.Count; i++)
            {
                EditorGUILayout.ToggleLeft(new GUIContent(assemblysBuildIn[i] + ".dll", "You can't remove this build-in dll"), true);
            }
            EditorGUI.EndDisabledGroup();
            for (int i = 0; i < assemblys.Count; i++)
            {
                bValue = EditorGUILayout.ToggleLeft(new GUIContent(assemblys[i] + ".dll", "We strongly recommend remove the dll you never direct use in hot update script, that can reduce a lot of compile time,it's just use while compiling(We'll use all of them in runtime.).We include all of them by default because we don't expect compile error, although that will take must more compile time."), assemblysValue[i]);
                if (bValue != assemblysValue[i])
                {
                    assemblysValue[i] = bValue;
                    config.SetBoolean("*" + assemblys[i], bValue);
                    config.Save();
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            if (forceExportAssetBundle)
            {
                forceExportAssetBundle = false;
                if (!File.Exists(strScriptOutputFile + ".meta"))
                    AssetDatabase.Refresh();
                GenerateScriptOutputFileMeta();
                OnExportAssetBundle();
            }
        }
        static void OnConfigChange()
        {
            string[] playerSettings;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out playerSettings);
            List<string> defines = new List<string>();
            string strDefine = "_CSHARP_LIKE_";
            string strAllowUnsafe = "_CSHARP_LIKE_ALLOW_UNSAFE_CODE_";// PlayerSettings.allowUnsafeCode
            foreach (var one in playerSettings)
            {
                defines.Add(one);
            }
            bool bChanged = false;
            if (isDebug)
            {
                if (defines.Contains(strDefine))
                {
                    defines.Remove(strDefine);
                    bChanged = true;
                }
            }
            else
            {
                if (!defines.Contains(strDefine))
                {
                    defines.Add(strDefine);
                    bChanged = true;
                }
            }
            if (PlayerSettings.allowUnsafeCode)
            {
                if (!defines.Contains(strAllowUnsafe))
                {
                    defines.Add(strAllowUnsafe);
                    bChanged = true;
                }
            }
            else
            {
                if (defines.Contains(strAllowUnsafe))
                {
                    defines.Remove(strAllowUnsafe);
                    bChanged = true;
                }
            }
            if (bChanged)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines.ToArray());
            }
        }
        static void OnMergeJSON(string path)
        {
            string[] strDirectories = Directory.GetDirectories(path + "/AssetBundles");
            CSharpLike.JSONData games = null;
            foreach (string strDirectorie in strDirectories)
            {
                if (File.Exists(strDirectorie + "/" + buildTarget + "/config.json"))
                {
                    CSharpLike.JSONData json = CSharpLike.KissJson.ToJSONData(File.ReadAllText(strDirectorie + "/" + buildTarget + "/config.json"));
                    if (json != null)
                    {
                        if (games == null)
                            games = CSharpLike.JSONData.NewList();
                        CSharpLike.JSONData game = CSharpLike.JSONData.NewDictionary();
                        game["version"] = json["version"];
                        game["sumSize"] = json["sumSize"];
                        game["name"] = json["name"];
                        game["displayName"] = json["displayName"];
                        game["url"] = json["url"] + "config.json";
                        foreach (var one in json["custom"].Value as Dictionary<string, CSharpLike.JSONData>)
                            game[one.Key] = one.Value;
                        games.Add(game);
                    }
                }
            }
            if (games != null)
            {
                string fileName = path + "/AssetBundles/games" + buildTarget + ".json";
                string strOld = File.Exists(fileName) ? File.ReadAllText(fileName) : null;
                string strNew = games.ToJson(true);
                if (strOld != strNew)
                    File.WriteAllText(fileName, strNew, new System.Text.UTF8Encoding(false));
            }
        }
        static void OnExportAssetBundle()
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length-7) + "/AssetBundles";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/" + strProductName + "/" + buildTarget.ToString();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string copyToPath = GetCopyToPath();
            Debug.Log("OnExportAssetBundle : start export AssetBundle to : " + path);
            BuildAssetBundleOptions babo = BuildAssetBundleOptions.CollectDependencies;
            if (isAssetBundleLZ)
                babo |= BuildAssetBundleOptions.ChunkBasedCompression;
            AssetBundleManifest abm = BuildPipeline.BuildAssetBundles(path, babo, buildTarget);
            Debug.Log("OnExportAssetBundle : finish export AssetBundle to : " + path);
            CSharpLike.JSONData configJSON = CSharpLike.JSONData.NewDictionary();
            configJSON["version"] = strVersion;
            string _strDownloadPath = strDownloadPath;
            if (string.IsNullOrEmpty(_strDownloadPath))
                _strDownloadPath = "StreamingAssets/AssetBundles";
            if (_strDownloadPath.EndsWith("/"))
                _strDownloadPath = _strDownloadPath.Substring(0, _strDownloadPath.Length-1);
            configJSON["url"] = _strDownloadPath + "/" + strProductName + "/" + buildTarget.ToString()+"/";
            long sumSize = 0;
            configJSON["sumSize"] = sumSize;
            configJSON["codeFile"] = strScriptOutputFile;
            configJSON["name"] = strProductName;
            configJSON["displayName"] = strDisplayName;
            configJSON["defaultScene"] = strDefaultScene;
            configJSON["custom"] = CSharpLike.JSONData.NewDictionary();
            foreach (var one in customJSONs)
                configJSON["custom"][one.Key] = one.Value;
            configJSON["files"] = CSharpLike.JSONData.NewList();
            foreach (var one in abm.GetAllAssetBundles())
            {
                CSharpLike.JSONData file = CSharpLike.JSONData.NewDictionary();
                file["fileName"] = one;
                file["hash128"] = abm.GetAssetBundleHash(one).ToString();
                file["size"] = new FileInfo(path + "/" + one).Length;
                sumSize += file["size"];
                CSharpLike.JSONData assets = CSharpLike.JSONData.NewList();
                CSharpLike.JSONData dependencies = CSharpLike.JSONData.NewList();
                file["assets"] = assets;
                file["dependencies"] = dependencies;

                foreach (var two in abm.GetAllDependencies(one))
                {
                    dependencies.Add(two);
                }
                AssetBundle ab = AssetBundle.LoadFromFile(path+"/"+one);
                foreach (var two in ab.GetAllAssetNames())
                {
                    assets.Add(two);
                }
                foreach (var two in ab.GetAllScenePaths())
                {
                    assets.Add(two);
                }
                ab.Unload(false);
                configJSON["files"].Add(file);
                //File.WriteAllText(copyToPath + "/" + "config.json", configJSON.ToJson(true), new System.Text.UTF8Encoding(false));
            }
            configJSON["sumSize"] = sumSize;
            //folder "/AssetBundles"
            string strJson = configJSON.ToJson(true);
            if (File.Exists(path + "/" + "config.json"))
            {
                if (strJson != File.ReadAllText(path + "/" + "config.json"))
                    File.WriteAllText(path + "/" + "config.json", strJson, new System.Text.UTF8Encoding(false));
            }
            else
                File.WriteAllText(path + "/" + "config.json", strJson, new System.Text.UTF8Encoding(false));
            //folder "StreamingAssets/AssetBundles"
            configJSON["url"] = "StreamingAssets/AssetBundles/" + strProductName + "/" + buildTarget.ToString() + "/";
            strJson = configJSON.ToJson(true);
            if (File.Exists(copyToPath + "/" + "config.json"))
            {
                if (strJson != File.ReadAllText(copyToPath + "/" + "config.json"))
                    File.WriteAllText(copyToPath + "/" + "config.json", strJson, new System.Text.UTF8Encoding(false));
            }
            else
                File.WriteAllText(copyToPath + "/" + "config.json", strJson, new System.Text.UTF8Encoding(false));
            //CopyTo(path + "/" + "config.json", copyToPath + "/" + "config.json");
            foreach (var one in abm.GetAllAssetBundles())
                CopyTo(path + "/" + one, copyToPath + "/" + one);
            //CopyTo(path + "/" + configJSON["codeFile"], copyToPath + "/" + configJSON["codeFile"]);
            Debug.Log("OnExportAssetBundle : finish copy AssetBundle to : " + copyToPath);
            OnMergeJSON(Application.dataPath.Substring(0, Application.dataPath.Length - 7));
            OnMergeJSON(Application.streamingAssetsPath);
            //System.Diagnostics.Process.Start(path);
        }
        static void CopyTo(string fileName1, string fileName2)
        {
            if (File.Exists(fileName1))
            {
                byte[] buff = File.ReadAllBytes(fileName1);
                if (File.Exists(fileName2))
                {
                    byte[] buffTarget = File.ReadAllBytes(fileName2);
                    if (buff.Length == buffTarget.Length
                        && CSharpLike.CSL_Utils.GetMD5(buff) == CSharpLike.CSL_Utils.GetMD5(buffTarget))
                    {
                        return;
                    }    
                }
                File.WriteAllBytes(fileName2, buff);
            }
        }
        static void OnGenerateRSA()
        {
            RSACryptoServiceProvider crypt = new RSACryptoServiceProvider(2048);
            string strPublicKey = crypt.ToXmlString(false);
            string strPrivateKey = crypt.ToXmlString(true);
            crypt.Clear();
            string strPublicPath = Application.dataPath + "/C#Like/Editor/RSAPublicKey.txt";
            string strPrivatePath = Application.dataPath + "/C#Like/Editor/RSAPrivateKey.txt";
            File.WriteAllText(strPublicPath, strPublicKey, new System.Text.UTF8Encoding(false));
            File.WriteAllText(strPrivatePath, strPrivateKey, new System.Text.UTF8Encoding(false));
            Debug.LogError($"Public key save to :{strPublicPath} , and Private key save to : {strPrivatePath} . ");
        }
        static Thread thread;
        static void OnBuildScripts(bool bForce, bool bRebuild)
        {
            compileSuccess = false;
            Debug.Log("OnBuildScripts:bForce =" + bForce + ",bRebuild=" + bRebuild);
            Init();
            if (!bForce)
            {
                EditorUtility.DisplayProgressBar("C#Like", "check script is modify", 0f);
                string strFile = strScriptOutputFile;
                if (!File.Exists(strFile))
                    strFile = Application.dataPath + "/C#Like/output/code.bytes";
                CheckFolder(strScriptOutputFile);
                if (File.Exists(strFile) && !IsHotUpdateScriptModify())
                {
                    Debug.Log("OnPackScripts:no hot update script files(*.cs) modify");
                    EditorUtility.ClearProgressBar();
                    compileSuccess = true;
                    CopyToStreamingAssets();
                    return;
                }
            }
            if (bRebuild)
            {
                string strFile = dataPath + "/C#Like/Editor/AssemblyCache.dat";
                if (File.Exists(strFile))
                    File.Delete(strFile);
            }
            try
            {
                if (thread != null && thread.IsAlive)
                    thread.Abort();
            }
            catch
            {
            }
            MyProfile myProfile = new MyProfile();
            myProfile.strDefine = MyDefineSymbols.GetAllScriptDefineSymbols();
            myProfile.strScriptSourceDir = Application.dataPath + strScriptSourceDir.Substring(6);
            myProfile.strScriptOutputFile = Application.dataPath + strScriptOutputFile.Substring(6);
            thread = new Thread(new ThreadStart(myProfile.Run));
            thread.Start();
            int count = 0;
            do
            {
                Thread.Sleep(100);
                EditorUtility.DisplayProgressBar("C#Like", "compiling script", myProfile.progress);
            } while (myProfile.progress >= 0f && ++count < 3000);//time out in 300 seconds.
            EditorUtility.ClearProgressBar();
            if (compileSuccess)
            {
                SaveConfigFile();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("not compile success");
            }
        }
        static CSL_Config configFile;

        public int callbackOrder{ get { return 1000; } }

        static void InitConfigFile()
        {
            if (configFile != null)
                return;
            configFile = new CSL_Config(Application.dataPath + "/C#Like/Editor/ConfigFile.txt");
        }
        static void SaveConfigFile()
        {
            InitConfigFile();
            configFile.Clear();
            string strDir = Application.dataPath + strScriptSourceDir.Substring(6);
            string[] files = Directory.GetFiles(strDir, "*.cs", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string strFileNameShort = file.Replace(strDir, "");
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    configFile.SetString(strFileNameShort + "*MD5", CSharpLike.CSL_Utils.GetMD5(fs));
                    configFile.SetLong(strFileNameShort + "*Length", fs.Length);
                }
            }
            configFile.Save();
        }
        static bool IsHotUpdateScriptModify()
        {
            InitConfigFile();
            string strDir = Application.dataPath + strScriptSourceDir.Substring(6);
            string[] files = Directory.GetFiles(strDir, "*.cs", SearchOption.AllDirectories);
            foreach(string file in files)
            {
                string strFileNameShort = file.Replace(strDir, "");
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Length != configFile.GetLong(strFileNameShort + "*Length"))//length not match
                    return true;
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if (CSharpLike.CSL_Utils.GetMD5(fs) != configFile.GetString(strFileNameShort + "*MD5"))
                        return true;
                }
            }
            return false;
        }
        public static List<string> GetAssemblys()
        {
            List<string> list = new List<string>();

            //don't remove this
            assemblysBuildIn = new List<string>();
            assemblysBuildIn.Add("mscorlib");//.Net API
            assemblysBuildIn.Add("Assembly-CSharp");//default user custom c# script assembly
            assemblysBuildIn.Add("UnityEngine");//unity engine core
            assemblysBuildIn.Add("UnityEngine.UI");//unity engine UI core

            //you may config here
            List<string> listIgnoreKeyWord = new List<string>();
            listIgnoreKeyWord.Add("editor");//we ignore the dll which name with the 'editor' keyword 

            //auto add other *.dll in '/Assets/' folder,exclude in the 'StreamingAssets' folder
            string[] files = Directory.GetFiles(dataPath, "*.dll", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (!file.Contains("/StreamingAssets"))//not in StreamingAssets folder
                {
                    string strFileName = file.Substring(Mathf.Max(file.LastIndexOf('/'), file.LastIndexOf('\\')) + 1);
                    strFileName = strFileName.Substring(0, strFileName.Length - 4);//remove '.dll'
                    if (!list.Contains(strFileName) && !assemblysBuildIn.Contains(strFileName))
                    {
                        bool bIgnore = false;
                        foreach (string strIgnore in listIgnoreKeyWord)
                        {
                            if (strFileName.ToLower().Contains(strIgnore))
                            {
                                bIgnore = true;
                                break;
                            }
                        }
                        if (bIgnore)
                            continue;
                        list.Add(strFileName);
                    }
                }
            }
            //auto add other *.dll in '/Library/ScriptAssemblies/' folder
            files = Directory.GetFiles(dataPath.Substring(0, dataPath.Length - 6) + "/Library/ScriptAssemblies/", "*.dll", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string strFileName = file.Substring(Mathf.Max(file.LastIndexOf('/'), file.LastIndexOf('\\')) + 1);
                strFileName = strFileName.Substring(0, strFileName.Length - 4);
                if (!list.Contains(strFileName) && !assemblysBuildIn.Contains(strFileName))
                {
                    bool bIgnore = false;
                    foreach (string strIgnore in listIgnoreKeyWord)
                    {
                        if (strFileName.ToLower().Contains(strIgnore))
                        {
                            bIgnore = true;
                            break;
                        }
                    }
                    if (bIgnore)
                        continue;
                    list.Add(strFileName);
                }
            }
            return list;
        }
    }
}
