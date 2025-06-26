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
    /// ����򿪵����ĵ�༭��
    /// </summary>
    public void ShowData()
    {
        timeHand.SetTime(this.ThisTime); //����ĵ�����ƶ�ָ�뵽�ĵ�λ�� 2025/06/10 - mixyao
    }

}
