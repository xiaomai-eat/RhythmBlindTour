using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IUIData
{

}

public class MessageTipData : IUIData
{
    public MessageTipData(string message)
    {
        this.message = message;
    }
    public string message;
}
public class UIManager : Singleton<UIManager>
{
    Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();
    Stack<UIElement> activeElements = new Stack<UIElement>();
    class UIElement
    {
        public string AssetName;
        public GameObject Instance;
        public UIWindowBase script;
        public Canvas canvas;
    }
    public UIManager()
    {
        //UIResources[typeof(UITest)] = new UIElement { AssetName = PathConfig.UIPath + "UITest" };
        //RegisterUI<UITest>(PathConfig.UIPath +"UITest");
    }
    void RegisterUI<T>(string UIPath)where T : UIWindowBase
    {
        UIResources[typeof(T)] = new UIElement { AssetName = UIPath };
    }
    public T Show<T>(IUIData data) where T : UIWindowBase
    {
        Type type = typeof(T);
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                GameObject prefab = ResLoader.Load<GameObject>(info.AssetName);
                if (prefab == null)
                {
                    Debug.LogError(info.AssetName + " can not be find");
                    return default;
                }
                info.Instance = GameObject.Instantiate(prefab);
                info.script = info.Instance.GetComponent<T>();
                info.canvas = info.Instance.GetComponent<Canvas>();
                info.Instance.SetActive(true);
            }
            info.canvas.sortingOrder = GetALayer();
            activeElements.Push(info);
            info.script.OnShow(data);
            return info.script as T;
        }
        else
        {
            Debug.LogError(typeof(T).ToString() + " is not in dictionary(人话:UIManager里面没注册)");
            return default;
        }

    }

    public void Close(Type type, bool destroy = false)
    {
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Instance != null)
            {
                if (!destroy)
                {
                    info.script.OnHide();
                    info.canvas.sortingOrder = UILayerConfig.NormalBack;
                    info.Instance.SetActive(false);
                }
                else
                {
                    info.script.OnHide();
                    info.script.OnDestroyClose();
                    GameObject.Destroy(info.Instance);
                    info.Instance = null;
                    info.script = null;
                }
            }
            if (activeElements.Count > 0)
                activeElements.Pop();
        }
    }

    public enum Dirction
    {
        up, down, left, right
    }

    /// <summary>
    /// 弹出ui画布下必须有根节点Root,根据Root的大小和位置判断是否超出屏幕并调整
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    /// <param name="ownerIsUI"> 调用者(caller)是不是UI </param>
    /// <param name="offset"></param>
    /// <param name="offsetDir"></param>
    /// <returns></returns>
    public T ShowPop<T>(IUIData data, Transform ts, bool ownerIsUI = false, float offset = 1f, Dirction offsetDir = Dirction.right) where T : UIWindowBase
    {
        #region
        Type type = typeof(T);
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                GameObject prefab = ResLoader.Load<GameObject>(info.AssetName);
                if (prefab == null)
                {
                    Debug.LogError(info.AssetName + "can not find");
                    return default;
                }
                info.Instance = GameObject.Instantiate(prefab);
                info.script = info.Instance.GetComponent<T>();
                info.canvas = info.Instance.GetComponent<Canvas>();
                info.Instance.SetActive(true);
            }
            info.script.OnShow(data);
            info.canvas.sortingOrder = GetALayer();
            #endregion
            Vector3 dir = offsetDir switch
            {
                Dirction.up => Vector3.up,
                Dirction.down => Vector3.down,
                Dirction.left => Vector3.left,
                Dirction.right => Vector3.right,
                _ => throw new NotImplementedException()
            };

            if (info.Instance.transform.Find("Root") == null)
            {
                Debug.LogError(typeof(T).ToString() + "下没有找到Root节点");
                return default;
            }

            RectTransform rect = info.Instance.transform.Find("Root").GetComponent<RectTransform>();
            if (!ownerIsUI)
                rect.transform.position = Camera.main.WorldToScreenPoint(ts.position + (dir * offset));
            else
                rect.transform.position = ts.position + (dir * offset * 40);
            if (!JudgmentUiInScreen(rect, offsetDir))
            {
                float modificationOffset = ownerIsUI ? offset * 40 : offset;
                rect.transform.position = Camera.main.WorldToScreenPoint(ts.position - (dir * modificationOffset));
            }
            return info.script as T;
        }
        else
        {
            Debug.LogError(typeof(T).ToString() + "is not in dictionary");
            return default;
        }
    }

    bool JudgmentUiInScreen(RectTransform rt, Dirction offsetDir)
    {
        RectTransform rtransform = rt;

        float miAxisPos = offsetDir switch
        {
            Dirction.up => rtransform.transform.position.y - rtransform.sizeDelta.y / 2,
            Dirction.down => rtransform.transform.position.y - rtransform.sizeDelta.y / 2,
            Dirction.left => rtransform.transform.position.x - rtransform.sizeDelta.x / 2,
            Dirction.right => rtransform.transform.position.x - rtransform.sizeDelta.x / 2,
            _ => throw new NotImplementedException(),
        };

        float maAixsPos = offsetDir switch
        {
            Dirction.up => rtransform.transform.position.y + rtransform.sizeDelta.y / 2,
            Dirction.down => rtransform.transform.position.y + rtransform.sizeDelta.y / 2,
            Dirction.left => rtransform.transform.position.x + rtransform.sizeDelta.x / 2,
            Dirction.right => rtransform.transform.position.x + rtransform.sizeDelta.x / 2,
            _ => throw new NotImplementedException(),
        };

        return offsetDir switch
        {
            Dirction.up => (miAxisPos >= 0 && maAixsPos <= Screen.height),
            Dirction.down => (miAxisPos >= 0 && maAixsPos <= Screen.height),
            Dirction.left => (miAxisPos >= 0 && maAixsPos <= Screen.width),
            Dirction.right => (miAxisPos >= 0 && maAixsPos <= Screen.width),
            _ => throw new NotImplementedException(),
        };
    }


    int GetALayer()
    {
        UIElement element = null;
        if (activeElements.Count > 0)
            element = activeElements.Peek();
        return element == null ? 1 : Math.Clamp(element.canvas.sortingOrder + 1, UILayerConfig.NormalBack, UILayerConfig.NomalFornt);
    }
}
