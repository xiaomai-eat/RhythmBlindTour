using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Qf.Systems;

/// <summary>
/// 替代 Unity InputSystem 的手动输入映射配置器（支持 Inspector 自定义初始映射）
/// </summary>
public class mInputMappingConfigurator : MonoBehaviour
{
    [Header("建议自定义映射（可在 Inspector 中设置）")]
    public KeyCode mKey_Quit = KeyCode.Escape;
    public KeyCode mKey_Sure = KeyCode.Return;
    public KeyCode mKey_SwipeUp = KeyCode.UpArrow;
    public KeyCode mKey_SwipeDown = KeyCode.DownArrow;
    public KeyCode mKey_SwipeLeft = KeyCode.LeftArrow;
    public KeyCode mKey_SwipeRight = KeyCode.RightArrow;
    public KeyCode mKey_Click = KeyCode.Space;

    // [默认键位字典] -- mixyao/25/07/02
    private Dictionary<string, List<KeyCode>> mDefaultMappingSet;

    void Awake()
    {
        mDefaultMappingSet = new Dictionary<string, List<KeyCode>>()
        {
            { "Quit", new List<KeyCode>{ mKey_Quit } },
            { "Sure", new List<KeyCode>{ mKey_Sure } },
            { "SwipeUp", new List<KeyCode>{ mKey_SwipeUp } },
            { "SwipeDown", new List<KeyCode>{ mKey_SwipeDown } },
            { "SwipeLeft", new List<KeyCode>{ mKey_SwipeLeft } },
            { "SwipeRight", new List<KeyCode>{ mKey_SwipeRight } },
            { "Click", new List<KeyCode>{ mKey_Click } }
        };

        PrepareInputMap();
        LoadMappingsFromJson();
    }

    /// <summary>
    /// 第一次运行时从 StreamingAssets 拷贝 inputMap.json 到 persistentDataPath
    /// </summary>
    void PrepareInputMap()
    {
        string dst = RuntimeJsonPath;
        if (!File.Exists(dst))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            string src = DefaultJsonPath;
            var www = new UnityEngine.Networking.UnityWebRequest(src);
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            www.SendWebRequest();
            while (!www.isDone) { }
            if (string.IsNullOrEmpty(www.error))
                File.WriteAllText(dst, www.downloadHandler.text);
#else
            string src = DefaultJsonPath;
            if (File.Exists(src))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dst));
                File.Copy(src, dst);
            }
#endif
        }
    }

    /// <summary>
    /// 从 JSON 文件加载键位映射配置，并补齐缺失键为默认 -- mixyao/25/07/02
    /// </summary>
    public void LoadMappingsFromJson()
    {
        string path = RuntimeJsonPath;
        InputKeyMap finalMap = new InputKeyMap();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var map = JsonUtility.FromJson<InputKeyMap>(json);

            foreach (var kv in map.KeyBindings)
                finalMap.KeyBindings[kv.Key] = kv.Value;
        }

        // [补充缺失操作键] -- mixyao/25/07/02
        foreach (var def in mDefaultMappingSet)
        {
            if (!finalMap.KeyBindings.ContainsKey(def.Key) || finalMap.KeyBindings[def.Key] == null || finalMap.KeyBindings[def.Key].Count == 0)
            {
                finalMap.KeyBindings[def.Key] = def.Value.ConvertAll(k => (int)k);
            }
        }

        foreach (var kv in finalMap.KeyBindings)
        {
            List<KeyCode> keyCodes = kv.Value.ConvertAll(i => (KeyCode)i);
            InputSystems.ClearKey(kv.Key);
            InputSystems.AddKey(kv.Key, keyCodes.ToArray());
        }

        UpdatePlayClickComposite();
    }

    /// <summary>
    /// 保存当前 InputSystems 的键位配置为 JSON 文件（UTF-8 编码）
    /// </summary>
    public void SaveAllMappingsToJson()
    {
        var map = new InputKeyMap();
        foreach (var key in InputSystems.GetAllKeyNames())
        {
            var codes = InputSystems.GetKeys(key);
            if (codes != null)
                map.KeyBindings[key] = codes.ConvertAll(k => (int)k);
        }

        var json = JsonUtility.ToJson(map, true);
        Directory.CreateDirectory(Path.GetDirectoryName(RuntimeJsonPath));
        File.WriteAllText(RuntimeJsonPath, json, System.Text.Encoding.UTF8);

        Debug.Log($"已保存输入映射至 JSON: {RuntimeJsonPath}");
    }

    /// <summary>
    /// 将 Inspector 设置应用为当前配置（不读写 JSON）
    /// </summary>
    public void ApplyInitialMappingsFromInspector()
    {
        foreach (var kv in mDefaultMappingSet)
        {
            InputSystems.ClearKey(kv.Key);
            InputSystems.AddKey(kv.Key, kv.Value.ToArray());
        }

        UpdatePlayClickComposite();
    }

    /// <summary>
    /// 设置指定操作的新按键，并保存
    /// </summary>
    public void SetKey(string action, KeyCode key)
    {
        InputSystems.ClearKey(action);
        InputSystems.AddKey(action, key);
        SaveAllMappingsToJson();

        if (action is "SwipeUp" or "SwipeDown" or "SwipeLeft" or "SwipeRight" or "Click")
            UpdatePlayClickComposite();
    }

    /// <summary>
    /// 给指定操作添加一个附加按键，并保存
    /// </summary>
    public void AddKey(string action, KeyCode extraKey)
    {
        InputSystems.AddKey(action, extraKey);
        SaveAllMappingsToJson();
    }

    /// <summary>
    /// 重置为 Inspector 默认映射并保存
    /// </summary>
    public void ResetAllToInspectorValues()
    {
        ApplyInitialMappingsFromInspector();
        SaveAllMappingsToJson();
    }

    /// <summary>
    /// 组合 PlayClick 的复合按键映射（确保各键存在） -- mixyao/25/07/02
    /// </summary>
    void UpdatePlayClickComposite()
    {
        var keys = new List<KeyCode>();

        void AddIfExists(string name)
        {
            var k = InputSystems.GetKeys(name);
            if (k != null)
                keys.AddRange(k);
        }

        AddIfExists("SwipeUp");
        AddIfExists("SwipeDown");
        AddIfExists("SwipeLeft");
        AddIfExists("SwipeRight");
        AddIfExists("Click");

        InputSystems.ClearKey("PlayClick");
        InputSystems.AddKey("PlayClick", keys.ToArray());
    }
    void Update()
    {
        // [按 R 键强制应用当前映射并保存] -- mixyao/25/07/02
        if (Input.GetKeyDown(KeyCode.R))
        {
            ApplyCurrentMappings();
            Debug.Log("当前映射已应用并保存 (R)");
        }
    }

    /// <summary>
    /// [将当前所有按键设置应用到 InputSystems 并保存] -- mixyao/25/07/02
    /// </summary>
    public void ApplyCurrentMappings()
    {
        foreach (var kv in mDefaultMappingSet)
        {
            InputSystems.AddKey(kv.Key); // 确保注册
            InputSystems.ClearKey(kv.Key);
            InputSystems.AddKey(kv.Key, kv.Value.ToArray());
        }

        UpdatePlayClickComposite();
        SaveAllMappingsToJson();
    }

    // === 路径定义 ===
    public static string DefaultJsonPath =>
        Path.Combine(Application.streamingAssetsPath, "KeyMap/inputMap.json");

    public static string RuntimeJsonPath =>
        Path.Combine(Application.persistentDataPath, "KeyMap/inputMap.json");
}

