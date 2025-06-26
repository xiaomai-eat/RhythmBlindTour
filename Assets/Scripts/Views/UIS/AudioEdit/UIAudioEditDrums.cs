using Qf.ClassDatas.AudioEdit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAudioEditDrums : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    Image image;
    public float ThisTime;
    public int Index;
    public bool IsTip = false;

    UIAudioEditTimeHand timeHand;

    void Awake()
    {
        timeHand = FindObjectOfType<UIAudioEditTimeHand>();
    }

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
        timeHand.SetTime(this.ThisTime); //点击鼓点快速移动指针到鼓点位置 2025/06/10 - mixyao
    }

}
