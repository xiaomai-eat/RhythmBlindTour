using Qf.Events;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIDrawAScale : MonoBehaviour, IController
{
    /// <summary>
    /// 需求：方向键单击切换小节中的位置,CTRL+方向建在小节之间切换
    /// </summary>
    [SerializeField]
    RectTransform progressBar; // 进度条的RectTransform
    [SerializeField]
    Color scaleColor = Color.black; // 刻度的颜色
    [SerializeField]
    float _JInterval = 1;
    int numberOfScales = 10; // 刻度的数量
    float scaleHeight = 80f; // 每个刻度的高度
    List<GameObject> _JGameObjects = new();//节数
    List<GameObject> _XJGameObjects = new();//小节数
    int _PixelUnitsPerSecond = AudioEditConfig.PixelUnitsPerSecond;//每秒像素单位
    int _EditHeight = AudioEditConfig.EditHeight;//编辑器可编辑范围高度
    void Start()
    {
        this.RegisterEvent<MainAudioChangeValue>(v =>
        {
            numberOfScales = (int)v.Length + 2;
            GenerateScales();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
    void GenerateScales()
    {
        foreach(var i in _JGameObjects)
        {
            Destroy(i.gameObject);
        }
        _JGameObjects.Clear();
        for (int i = 0; i < numberOfScales; i++)
        {
            GameObject scale = new GameObject("Scale" + i);
            _JGameObjects.Add(scale);
            scale.transform.SetParent(progressBar, false);
            RectTransform rectTransform = scale.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(5f, scaleHeight); // 宽度和高度
            rectTransform.anchoredPosition = new Vector2(i * _PixelUnitsPerSecond * _JInterval, 0); // 垂直位置
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0); // 轴心点
            Image image = scale.AddComponent<Image>();
            image.color = scaleColor;
        }
    }
    public void SetColor(Color color)
    {
        scaleColor = color;
    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}