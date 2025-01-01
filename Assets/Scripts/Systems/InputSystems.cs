using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;
namespace Qf.Systems
{
    public class InputSystems : AbstractSystem
    {
        public static bool Quit { get; protected set; }
        public static bool Sure { get; protected set; }
        public static bool SwipeUp { get; protected set; }
        public static bool SwipeDown { get; protected set; }
        public static bool SwipeLeft { get; protected set; }
        public static bool SwipeRight { get; protected set; }
        public static float Click { get; protected set; }
        public static float Horizontal { get; protected set; }
        public static float Vertical { get; protected set; }
        static Dictionary<string, List<KeyCode>> keyValuePairs = new();
        protected override void OnInit()
        {
            InputInit();
            Managers.Instance.AddUpdate(() => Pc());
        }
        void InputInit()
        {
            AddKey("Quit",KeyCode.Escape);
            AddKey("Sure", KeyCode.Space);
        }
        void Pc()
        {
            Quit = InputQuery("Quit");
            Sure = InputQuery("Sure");
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
        }
        /// <summary>
        /// 多按键解决方法
        /// </summary>
        /// <param name="keyName">操作名称</param>
        /// <param name="and">启用与运算:所有按键同时触发才为true</param>
        /// <returns></returns>
        bool InputQuery(string keyName, bool and = false)
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
                if (Input.GetKey(key))
                {
                    return true;
                }
            }
            return false;
        }
        public static void AddKey(string KeyName,KeyCode keyCode)
        {
            if (!keyValuePairs.ContainsKey(KeyName))
            {
                List<KeyCode> keys = new();
                keys.Add(keyCode);
                keyValuePairs[KeyName] = keys;
            }
            else
            {
                keyValuePairs[KeyName].Add(keyCode);
            }
        }
        public static void RemoveKey(string KeyName,KeyCode keyCode) 
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

