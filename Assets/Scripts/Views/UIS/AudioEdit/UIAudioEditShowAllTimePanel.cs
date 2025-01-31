using Qf.Querys.AudioEdit;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class UIAudioEditShowAllTimePanel :UIWindowBase
{
    [SerializeField]
    GameObject _UITimeShowButtonProfabs;
    [SerializeField]
    Transform _ParentLoadPath;
    List<UIAudioEditTimeDataShowButton> _AllGameObjects = new();
    public override void OnShow(IUIData showData)
    {
        UpdateShow();
    }

    void UpdateShow()
    {
        var ls = this.SendQuery(new QueryAudioEditTimeLineAllData());
        if (_AllGameObjects.Count >= ls.Count)
        {
            int i = 0;
            foreach (var t in ls.Keys)
            {
                _AllGameObjects[i].gameObject.SetActive(true);
                _AllGameObjects[i].SetShowText($"{t}");
                _AllGameObjects[i].SetShowPlane(t);
                i++;
            }
            for(int j=i;j<_AllGameObjects.Count; j++)
            {
                _AllGameObjects[j].gameObject.SetActive(false);
            }
        }
        else
        {
            int i;
            for (i = _AllGameObjects.Count; i < ls.Count; i++)
            {
                CreateButton();
            }
            i = 0;
            foreach(var t in ls.Keys)
            {
                _AllGameObjects[i].gameObject.SetActive(true);
                _AllGameObjects[i].SetShowText($"{t}");
                _AllGameObjects[i].SetShowPlane(t);
                i++;
            }
        }
       
    }
    Transform go;
    void CreateButton()
    {
        go = Instantiate(_UITimeShowButtonProfabs).transform;
        go.SetParent(_ParentLoadPath);
        _AllGameObjects.Add(go.GetComponent<UIAudioEditTimeDataShowButton>());
    }
}

