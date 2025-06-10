using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Models.AudioEdit;
using QFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 25/06/06 - mixyao
/// 大幅修改，实现了节拍设置调整标尺布局，并添加了操作按钮精准移动到指定拍的位置。
/// 对标尺的可视化进行了规范。
/// </summary>
public class UIDrawAScale : MonoBehaviour, IController
{
    [SerializeField] RectTransform progressBar;

    [Header("颜色设置")]
    [SerializeField] Color mainBeatColor = Color.red;
    [SerializeField] Color subBeatColor = Color.black;
    [SerializeField]
    Color[] measureColors = new Color[]
    {
        new Color(0.9f, 0.9f, 0.9f),
        new Color(0.75f, 0.75f, 0.75f)
    };

    AudioEditModel editModel;
    List<GameObject> scaleLines = new();
    List<GameObject> measureBGs = new();
    List<GameObject> clickableBlocks = new();

    int _PixelUnitsPerSecond = AudioEditConfig.PixelUnitsPerSecond;
    float scaleHeight = 80f;

    void Start()
    {
        editModel = this.GetModel<AudioEditModel>();
        this.RegisterEvent<BPMChangeValue>(v =>
        {
            GenerateScales();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void ClearAll()
    {
        foreach (var g in scaleLines) Destroy(g);
        foreach (var g in measureBGs) Destroy(g);
        foreach (var g in clickableBlocks) Destroy(g);
        scaleLines.Clear();
        measureBGs.Clear();
        clickableBlocks.Clear();
    }

    public void GenerateScales()
    {
        if (editModel.EditAudioClip == null || editModel.BPM <= 0) return;

        ClearAll();

        int beatA = editModel.BeatA;
        int beatB = editModel.BeatB;
        float beatDuration = 60f / editModel.BPM;
        float measureDuration = beatDuration * beatB;
        float audioLength = editModel.EditAudioClip.length;

        int beatIndex = 0;
        float time = 0f;

        while (time < audioLength)
        {
            bool isMainBeat = (beatIndex % beatA == 0); // 每小节首拍

            if (isMainBeat)
            {
                // 背景按小节交替，每 A 拍为一个小节
                int measureIndex = beatIndex / beatA;
                Color bgColor = measureColors[measureIndex % measureColors.Length];
                CreateMeasureBG(time, measureDuration, bgColor);
            }

            CreateScaleLine(time, isMainBeat);
            time += beatDuration;
            beatIndex++;
        }
    }


    void CreateMeasureBG(float time, float duration, Color color)
    {
        GameObject bg = new GameObject("MeasureBG");
        bg.transform.SetParent(progressBar, false);

        RectTransform rt = bg.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0f, 0f);
        rt.sizeDelta = new Vector2(duration * _PixelUnitsPerSecond, scaleHeight);
        rt.anchoredPosition = new Vector2(time * _PixelUnitsPerSecond, 0);

        Image img = bg.AddComponent<Image>();
        img.color = color;

        measureBGs.Add(bg);
    }

    void CreateScaleLine(float time, bool isMainBeat)
    {
        GameObject line = new GameObject(isMainBeat ? "MainBeat" : "SubBeat");
        line.transform.SetParent(progressBar, false);

        RectTransform rt = line.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(time * _PixelUnitsPerSecond, 0);

        float height = isMainBeat ? scaleHeight : scaleHeight * 0.3f;
        float width = 3f * 1.5f;
        rt.sizeDelta = new Vector2(width, height);

        Image img = line.AddComponent<Image>();
        img.color = isMainBeat ? mainBeatColor : subBeatColor;

        scaleLines.Add(line);

        // 创建点击区域
        CreateClickableBlockAbove(time, width);
    }

    void CreateClickableBlockAbove(float time, float width)
    {
        GameObject block = new GameObject("ClickArea");
        block.transform.SetParent(progressBar, false);

        RectTransform rt = block.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(width * 7.5f, 20f);
        rt.anchoredPosition = new Vector2(time * _PixelUnitsPerSecond, scaleHeight);

        Image img = block.AddComponent<Image>();
        img.color = new Color(1f, 0.5f, 0f, 0.6f);

        var eventTrigger = block.AddComponent<EventTrigger>();
        float tCopy = time;

        var clickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        clickEntry.callback.AddListener((data) =>
        {
            this.SendCommand(new SetAudioEditThisTimeCommand(tCopy));
            FindObjectOfType<CreateDrumsManager>()?.ResetAllActiveCenters(); // 清空标记 局限在谱面编辑中可以重新设置InputMode //2025/06/10 - mixyao
        });
        eventTrigger.triggers.Add(clickEntry);

        var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enterEntry.callback.AddListener((data) =>
        {
            img.color = new Color(1f, 0.7f, 0.2f, 0.8f); // 悬停变亮
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        });
        eventTrigger.triggers.Add(enterEntry);

        var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exitEntry.callback.AddListener((data) =>
        {
            img.color = new Color(1f, 0.5f, 0f, 0.6f); // 恢复
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        });
        eventTrigger.triggers.Add(exitEntry);

        clickableBlocks.Add(block);
    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}
