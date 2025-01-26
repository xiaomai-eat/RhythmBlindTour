using QFramework;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
public class Storage : IUtility
{
    static string dataPath = Application.persistentDataPath + PathConfig.SavePath;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="fileName">Paths为true时填完成路径</param>
    /// <param name="Paths">保存方式false为默认路径</param>
    public void Save<T>(T obj, string fileName = null, bool Paths = false)
    {
        string path ="";
        string json ="";
        if (!Paths)
        {
            if (!File.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            path = Path.Combine(dataPath, typeof(T).ToString() + fileName);
            json = JsonConvert.SerializeObject(obj, new VectorConverter());//VectorConverter处理坐标信息
            File.WriteAllText(path, json);
        }
        else
        {
            FileStream fs = new(fileName,FileMode.Create);
            path = fileName;
            json = JsonConvert.SerializeObject(obj, new VectorConverter());//VectorConverter处理坐标信息
            StreamWriter sw = new(fs);
            sw.WriteLine(json);
            sw.Close();
            fs.Close();
            //File.WriteAllText(path, json);
        }
#if UNITY_EDITOR
        Debug.Log(path);
#endif
    }
    public T Load<T>(string fileName = null, bool Paths = false)
    {
        string path = "";
        if (!Paths)
        {
            path = Path.Combine(dataPath, typeof(T).ToString() + fileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                T data = JsonConvert.DeserializeObject<T>(json, new VectorConverter());
                return data;
            }
        }
        else
        {
            path = fileName;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                T data = JsonConvert.DeserializeObject<T>(json, new VectorConverter());
                return data;
            }
        }
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
