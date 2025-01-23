using QFramework;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEditor;
public class Storage : IUtility
{
    static string dataPath = Application.persistentDataPath + PathConfig.SavePath;
    public void Save<T>(T obj, string fileName = null)
    {
        if (!File.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        string path = Path.Combine(dataPath, typeof(T).ToString() + fileName);
        string json = JsonConvert.SerializeObject(obj, new VectorConverter());//VectorConverter处理坐标信息
        File.WriteAllText(path, json);
#if UNITY_EDITOR
        Debug.Log(path);
#endif
    }

    public T Load<T>(string fileName = null)
    {
        string path = Path.Combine(dataPath, typeof(T).ToString() + fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            T data = JsonConvert.DeserializeObject<T>(json, new VectorConverter());
            return data;
        }
        else
            return default;
    }

    public void RemoveSave<T>()
    {
        string path = Path.Combine(dataPath, nameof(T));
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
