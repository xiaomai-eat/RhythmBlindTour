using QFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Qf.Systems
{
    /// <summary>
    /// 输入系统(通过统一的接口获取输入)
    /// </summary>
    public class InputSystems : AbstractSystem
    {
        public static bool Quit { get; protected set; }
        public static bool Sure { get; protected set; }
        public static bool SwipeUp { get; protected set; }
        public static bool SwipeDown { get; protected set; }
        public static bool SwipeLeft { get; protected set; }
        public static bool SwipeRight { get; protected set; }
        public static bool Click { get; protected set; }
        public static bool PlayClick { get; protected set; }//点击或使用了交互按键
        public static float Horizontal { get; protected set; }
        public static float Vertical { get; protected set; }
        static Dictionary<string, List<KeyCode>> keyValuePairs = new();//用于切换触发事件的物理按键
        protected override void OnInit()
        {
            InputInit();
            Managers.Managers.instance.AddUpdate(() => Pc());
        }
        void InputInit()
        {
            AddKey("Quit", KeyCode.Escape);
            AddKey("Sure", KeyCode.Space);
            AddKey("SwipeUp", KeyCode.W);
            AddKey("SwipeDown", KeyCode.S);
            AddKey("SwipeLeft", KeyCode.A);
            AddKey("SwipeRight", KeyCode.D);
            AddKey("Click", KeyCode.Space);
            List<KeyCode> ls = new();
            ls .AddRange(GetKeys("SwipeUp"));
            ls.AddRange(GetKeys("SwipeDown"));
            ls.AddRange(GetKeys("SwipeLeft"));
            ls.AddRange(GetKeys("SwipeRight"));
            ls.AddRange(GetKeys("Click"));
            AddKey("PlayClick", ls.ToArray());
        }
        void Pc()
        {
            Quit = InputQuery("Quit");
            Sure = InputQuery("Sure");
            SwipeUp = InputQuery("SwipeUp");
            SwipeDown = InputQuery("SwipeDown");
            SwipeLeft = InputQuery("SwipeLeft");
            SwipeRight = InputQuery("SwipeRight");
            Click = InputQuery("Click");
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
            PlayClick = InputQuery("PlayClick");
        }
        /// <summary>
        /// 多按键解决方法
        /// </summary>
        /// <param name="keyName">操作名称</param>
        /// <param name="and">启用与运算:所有按键同时触发才为true</param>
        /// <returns></returns>
        bool InputQuery(string keyName,bool and = false)
        {
            if (keyValuePairs[keyName].Count <= 1)//减少查找导致的相应时间变慢非必要不要使用多按键
            {
                if (Input.GetKeyDown(keyValuePairs[keyName][0])){
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (and)
                {
                    foreach (var key in keyValuePairs[keyName])
                    {
                        if (!Input.GetKey(key))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                foreach (var key in keyValuePairs[keyName])
                {
                    if (Input.GetKeyDown(key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static List<KeyCode> GetKeys(string KeyName)
        {
            if (keyValuePairs.ContainsKey(KeyName))
            {
                return keyValuePairs[KeyName];
            }
            return null;
        }
        public static void AddKey(string KeyName,params KeyCode[] keyCode)
        {
            if (!keyValuePairs.ContainsKey(KeyName))
            {
                List<KeyCode> keys;
                keys= keyCode.ToList();
                keyValuePairs[KeyName] = keys;
            }
            else
            {
                foreach(var i in keyCode)
                {
                    if (keyValuePairs[KeyName].Contains(i))
                    {
                        Debug.Log($"{i}该按键已经存在");
                        continue;
                    }
                    keyValuePairs[KeyName].Add(i);
                }
                
            }
        }
        public static void RemoveKey(string KeyName, KeyCode keyCode)
        {
            if (!keyValuePairs.ContainsKey(KeyName))
            {
                Debug.LogError("不存在该操作请使用AddKey方法或其它方法创建");
                return;
            }
            else
            {
                keyValuePairs[KeyName].Remove(keyCode);
            }
        }
        public static void ClearKey(string KeyName)
        {
            if (!keyValuePairs.ContainsKey(KeyName))
            {
                Debug.LogError("不存在该操作请使用AddKey方法或其它方法创建");
                return;
            }
            else
            {
                keyValuePairs[KeyName].Clear();
            }
        }

    }
}

