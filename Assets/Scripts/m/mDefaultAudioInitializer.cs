using UnityEngine;
using Qf.Events;
using Qf.Commands.AudioEdit;
using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class mDefaultAudioInitializer : MonoBehaviour, IController
{
    [System.Serializable]
    public class AudioAssignment
    {
        [HideInInspector] public string groupName;
        public UIFileAttribute targetAttribute;
        public AudioClip defaultClip;
    }

    [Header("请按顺序填写 12 项音频（主音频 → 提示音）")]
    public List<AudioAssignment> defaultAudioList = new List<AudioAssignment>();

    [Header("初始化使用的 BPM")]
    public int initialBPM = 120;

    [Header("初始化节拍设置")]
    public TMP_Dropdown beatADropdown;
    public TMP_Dropdown beatBDropdown;
    public int defaultBeatA = 4;
    public int defaultBeatB = 4;

    [Header("主音频波形图设置")]
    public Image waveformImage;
    public Color waveformColor = Color.yellow;
    public Color waveformBGColor = Color.clear;
    public int waveformHeight = 80;

    void OnValidate()
    {
#if UNITY_EDITOR
        foreach (var entry in defaultAudioList)
        {
            if (entry.targetAttribute != null)
            {
                var parent = entry.targetAttribute.transform.parent;
                entry.groupName = parent != null ? parent.name : "Unparented";
            }
        }
#endif
    }

    void Start()
    {
        var model = this.GetModel<AudioEditModel>();
        model.BeatA = defaultBeatA;
        model.BeatB = defaultBeatB;

        if (beatADropdown != null)
            beatADropdown.value = Mathf.Clamp(defaultBeatA - 1, 0, beatADropdown.options.Count - 1);

        if (beatBDropdown != null)
        {
            int[] bOptions = { 1, 2, 4, 8 };
            for (int i = 0; i < bOptions.Length; i++)
            {
                if (bOptions[i] == defaultBeatB)
                {
                    beatBDropdown.value = i;
                    break;
                }
            }
        }

        for (int i = 0; i < defaultAudioList.Count; i++)
        {
            var entry = defaultAudioList[i];
            if (entry == null || entry.targetAttribute == null || entry.defaultClip == null)
                continue;

            SelectManager.SetAttribute(entry.targetAttribute);
            entry.targetAttribute.SetShowFileName(entry.defaultClip.name);
            entry.targetAttribute.RunAction(entry.defaultClip);

            switch (i)
            {
                case 0:
                    this.SendCommand(new SetAudioEditAudioCommand(entry.defaultClip));
                    StartCoroutine(DelayedBPMInit(initialBPM));
                    StartCoroutine(GenerateWaveformFrom(entry.defaultClip));
                    break;
                case 1:
                    this.SendCommand(new SetAudioEditAudioLoseAudioCommand(entry.defaultClip));
                    break;
                case 2:
                    this.SendCommand(new SetAudioEditSucceedAudioCommand(TheTypeOfOperation.SwipeUp, entry.defaultClip));
                    break;
                case 3:
                    this.SendCommand(new SetAudioEditSucceedAudioCommand(TheTypeOfOperation.SwipeDown, entry.defaultClip));
                    break;
                case 4:
                    this.SendCommand(new SetAudioEditSucceedAudioCommand(TheTypeOfOperation.SwipeLeft, entry.defaultClip));
                    break;
                case 5:
                    this.SendCommand(new SetAudioEditSucceedAudioCommand(TheTypeOfOperation.SwipeRight, entry.defaultClip));
                    break;
                case 6:
                    this.SendCommand(new SetAudioEditSucceedAudioCommand(TheTypeOfOperation.Click, entry.defaultClip));
                    break;
                case 7:
                    model.UpTipsAudioClip = entry.defaultClip;
                    break;
                case 8:
                    model.DownTipsAudioClip = entry.defaultClip;
                    break;
                case 9:
                    model.LeftTipsAudioClip = entry.defaultClip;
                    break;
                case 10:
                    model.RightTipsAudioClip = entry.defaultClip;
                    break;
                case 11:
                    model.ClickTipsAudioCLip = entry.defaultClip;
                    break;
            }
        }

        // 完成后自动销毁自身（节省运行时内存）
        //Destroy(this);
    }

    IEnumerator DelayedBPMInit(int bpm)
    {
        yield return null;
        this.SendCommand(new SetAudioEditAudioBPMCommand(bpm));
        this.SendEvent(new BPMChangeValue { BPM = bpm });
    }

    IEnumerator GenerateWaveformFrom(AudioClip clip)
    {
        if (clip == null || waveformImage == null)
            yield break;

        while (clip.loadState != AudioDataLoadState.Loaded)
            yield return null;

        int pps = AudioEditConfig.PixelUnitsPerSecond;
        int width = Mathf.CeilToInt(clip.length * pps);
        int height = waveformHeight;

        int dataPerColumn = Mathf.Max(1, clip.frequency / pps);
        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);

        if (data.Length < 10 || Mathf.Approximately(data[0], 0f))
        {
            Texture2D testTex = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color c = (y == height / 2) ? Color.red : waveformBGColor;
                    testTex.SetPixel(x, y, c);
                }
            }
            testTex.Apply();
            waveformImage.sprite = Sprite.Create(testTex, new Rect(0, 0, width, height), new Vector2(0f, 0f));
            waveformImage.color = Color.white;
            yield break;
        }

        float[] waveform = new float[data.Length / dataPerColumn];
        float max = 0f;
        for (int i = 0; i < waveform.Length; i++)
        {
            float sum = 0f;
            for (int j = 0; j < dataPerColumn; j++)
            {
                int idx = i * dataPerColumn + j;
                if (idx >= data.Length) break;
                sum += Mathf.Abs(data[idx]);
            }
            waveform[i] = sum;
            if (sum > max) max = sum;
        }

        Texture2D tex = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = waveformBGColor;
        tex.SetPixels(pixels);

        int halfH = height / 2;
        float scale = (max > halfH) ? (halfH / max) : 1f;
        float hScale = (float)width / waveform.Length;

        for (int i = 0; i < waveform.Length; i++)
        {
            int x = Mathf.Clamp((int)(i * hScale), 0, width - 1);
            int yExtent = Mathf.Clamp((int)(waveform[i] * scale), 0, halfH);
            for (int y = halfH - yExtent; y <= halfH + yExtent; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                    tex.SetPixel(x, y, waveformColor);
            }
        }

        tex.Apply();
        waveformImage.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0f, 0f));
        waveformImage.color = Color.white;
    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}
