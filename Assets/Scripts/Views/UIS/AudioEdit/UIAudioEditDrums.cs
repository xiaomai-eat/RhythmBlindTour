using Qf.ClassDatas.AudioEdit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAudioEditDrums : MonoBehaviour,IPointerClickHandler
{
    [SerializeField]
    Image image;
    public float ThisTime;
    public int Index;

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowData();
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }
    /// <summary>
    /// 这里打开单独鼓点编辑器
    /// </summary>
    public void ShowData()
    {
        Debug.Log("时间轴中的数据");
    }
}
