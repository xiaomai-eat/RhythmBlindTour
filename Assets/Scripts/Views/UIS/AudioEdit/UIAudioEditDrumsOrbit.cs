using Assets.Scripts.Querys.AudioEdit;
using Qf.ClassDatas.AudioEdit;
using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Models.AudioEdit;
using Qf.Querys.AudioEdit;
using Qf.Systems;
using QFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioEditDrumsOrbit : MonoBehaviour, IController
{
    [System.Serializable]
    public class TrackStyle
    {
        public TheTypeOfOperation Operation;
        public Color DrumColor;
        public Color PreTipColor;
        public Sprite DrumSprite;
        public Sprite PreTipSprite;
    }

    [SerializeField]
    private List<TrackStyle> trackStyles = new(); // 轨道样式
    [SerializeField]
    GameObject DrumsProfabs;
    [SerializeField]
    RectTransform[] DrumsUI;
    [SerializeField]
    List<GameObject> DrumsUIInDrums = new();
    [Header("鼓点布局设置")]
    [Tooltip("是否以中心时间为创建位置锚点（默认偏移为鼓点宽度一半）")]
    [SerializeField]
    private bool isCenterCreate = false;    // 添加开关，可应需求设置创建点为提示音位置，或回答音位置。 -- mixyao/06/24

    private bool isNextDrumDemo = false;     // 控制是否为 Demo 鼓点（存在时间=0）
    private bool isAutoTipOffset = false;    // 控制是否自动设置 Tip 偏移时间 = existence / 2

    public void ToggleNextDrumDemo(bool isOn) => isNextDrumDemo = isOn;
    public void ToggleAutoTipOffset(bool isOn) => isAutoTipOffset = isOn;

    int _PixelUnitsPerSecond = AudioEditConfig.PixelUnitsPerSecond;
    int _EditHeight = AudioEditConfig.EditHeight;
    AudioEditModel editModel;
    Dictionary<TheTypeOfOperation, int> operationToTrackIndex;

    void Start()
    {
        Init();
    }

    void Update() => InputContller();

    IArchitecture IBelongToArchitecture.GetArchitecture() => GameBody.Interface;

    void Init()
    {
        editModel = this.GetModel<AudioEditModel>();
        InitOperationTracks();
        StartLength();
        this.RegisterEvent<OnUpdateAudioEditDrumsUI>(v => UpDateDrwmsUI()).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<MainAudioChangeValue>(v => StartLength()).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void InitOperationTracks()
    {
        operationToTrackIndex = new();
        for (int i = 0; i < trackStyles.Count; i++)
        {
            if (!operationToTrackIndex.ContainsKey(trackStyles[i].Operation))
                operationToTrackIndex[trackStyles[i].Operation] = i;
        }
    }

    #region 外部设置开关，Button or Toggle 
    public void ND_SetNextDrumAsDemo()
    {
        isNextDrumDemo = true;
    }

    public void ND_SetNextDrumAsNormal()
    {
        isNextDrumDemo = false;
    }
    public void ND_ToggleNextDrumDemo(bool isOn)
    {
        isNextDrumDemo = isOn;
    }
    #endregion

    public void AddDrwms(int i = 0)
    {
        TheTypeOfOperation operation = i switch
        {
            0 => TheTypeOfOperation.SwipeUp,
            1 => TheTypeOfOperation.SwipeDown,
            2 => TheTypeOfOperation.SwipeLeft,
            3 => TheTypeOfOperation.SwipeRight,
            4 => TheTypeOfOperation.Click,
            _ => TheTypeOfOperation.Click
        };
        AddDrwms(operation);
    }

    public void AddDrwms(TheTypeOfOperation theTypeOfOperation)
    {
        if (editModel.EditAudioClip == null) return;

        var tipClip = this.SendQuery(new QueryAudioEditComeTipAudio(theTypeOfOperation));
        var succeedClip = this.SendQuery(new QueryAudioEditSucceedsAudio(theTypeOfOperation));
        var loseClip = editModel.LoseAudioClip;
        float tipOffset = editModel.TipOffset.Value;
        float existence = isNextDrumDemo ? 0f : editModel.TimeOfExistence.Value;
        float centerTime = isCenterCreate ? editModel.ThisTime : editModel.ThisTime + tipOffset;
        tipOffset = isAutoTipOffset ? editModel.TimeOfExistence.Value / 2 : editModel.TipOffset.Value;
        centerTime = isAutoTipOffset ? editModel.ThisTime + tipOffset : editModel.ThisTime;

        var newDrums = new DrumsLoadData()
        {
            DrwmsData = new DrwmsData()
            {
                DtheTypeOfOperation = theTypeOfOperation,
                FPreAdventAudioClipPath = tipClip?.name,
                FSucceedAudioClipPath = succeedClip?.name,
                FLoseAudioClipPath = loseClip?.name,
                VPreAdventAudioClipOffsetTime = tipOffset,
                VTimeOfExistence = existence,
                CenterTime = centerTime,
            },
            MusicData = new MusicData()
            {
                SPreAdventVolume = editModel.PreAdventVolume.Value,
                SLoseVolume = editModel.LoseAudioVolume.Value,
                SSucceedVolume = editModel.SucceedAudioVolume.Value
            }
        };

        this.SendCommand(new AddAudioEditTimeLineDataCommand(centerTime, newDrums));
    }


    void UpDateDrwmsUI() => UpDateAllDrwmsUI();

    void UpDateAllDrwmsUI()
    {
        foreach (var item in DrumsUIInDrums)
            Destroy(item);
        DrumsUIInDrums.Clear();

        var dataDict = this.SendQuery(new QueryAudioEditTimeLineAllData());

        foreach (var item in dataDict.Keys)
        {
            for (int i = 0; i < dataDict[item].Count; i++)
            {
                if (dataDict[item][i] == null) continue;
                CreateDrumItemUI(item, i, dataDict);
            }
        }
    }

    void CreateDrumItemUI(float item, int i, Dictionary<float, List<DrumsLoadData>> dataDict)
    {
        var drumData = dataDict[item][i];
        var op = drumData.DrwmsData.DtheTypeOfOperation;

        if (!operationToTrackIndex.TryGetValue(op, out int trackIndex))
        {
            Debug.LogWarning($"操作 {op} 未绑定轨道");
            return;
        }

        var style = trackStyles[trackIndex];
        float tipOffset = drumData.DrwmsData.VPreAdventAudioClipOffsetTime;
        float existence = drumData.DrwmsData.VTimeOfExistence;
        float drumX = item * _PixelUnitsPerSecond;
        float pixelExistence = existence * _PixelUnitsPerSecond;

        // 创建根节点
        var drumRoot = new GameObject("DrumRoot");
        drumRoot.transform.SetParent(DrumsUI[trackIndex].transform);
        var rootRect = drumRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = rootRect.anchorMax = new Vector2(0, 0.5f);
        rootRect.pivot = new Vector2(0, 0.5f);
        rootRect.anchoredPosition = Vector2.zero;
        rootRect.localScale = Vector3.one;

        // 创建 Drum UI
        var drumGO = Instantiate(DrumsProfabs, drumRoot.transform);
        drumGO.name = "Drum";
        var drumRect = drumGO.GetComponent<RectTransform>();
        drumRect.anchorMin = drumRect.anchorMax = new Vector2(0, 0.5f);
        drumRect.pivot = new Vector2(0, 0.5f);
        drumRect.localScale = Vector3.one;
        drumRect.anchoredPosition = new Vector2(drumX - pixelExistence / 2f, 0);
        drumRect.sizeDelta = new Vector2(pixelExistence, drumRect.sizeDelta.y);

        var drumImage = drumGO.GetComponent<Image>();
        if (drumImage && style.DrumSprite) drumImage.sprite = style.DrumSprite;
        drumImage.color = style.DrumColor;

        var uiDrums = drumGO.GetComponent<UIAudioEditDrums>();
        uiDrums.ThisTime = item; // 鼓点点击跳转时间 = CenterTime
        uiDrums.Index = i;
        uiDrums.IsTip = false;

        // 创建 PreTip UI
        var preTipGO = Instantiate(DrumsProfabs, drumRoot.transform);
        preTipGO.name = "PreTipArea";
        var preRect = preTipGO.GetComponent<RectTransform>();
        preRect.anchorMin = preRect.anchorMax = new Vector2(0, 0.5f);
        preRect.pivot = new Vector2(1, 0.5f);
        preRect.localScale = Vector3.one;
        preRect.anchoredPosition = new Vector2(drumX, 0);
        preRect.sizeDelta = new Vector2(tipOffset * _PixelUnitsPerSecond, drumRect.sizeDelta.y);

        var preImage = preTipGO.GetComponent<Image>();
        if (preImage && style.PreTipSprite) preImage.sprite = style.PreTipSprite;

        // ✅ 设置 Tip 区域颜色（含 Demo 鼓点高亮处理）
        Color tipColor;
        if (Mathf.Approximately(existence, 0f))
        {
            // 使用 DrumColor + 降亮度 + 透明度 200
            tipColor = style.DrumColor;
            float darken = 0.75f;
            tipColor.r *= darken;
            tipColor.g *= darken;
            tipColor.b *= darken;
            tipColor.a = 200f / 255f;
        }
        else
        {
            tipColor = style.PreTipColor;
        }
        preImage.color = tipColor;

        var preUI = preTipGO.GetComponent<UIAudioEditDrums>();
        preUI.ThisTime = Mathf.Approximately(existence, 0f) ? item : item - tipOffset; // 鼠标点击时跳转到中心时间
        preUI.Index = i;
        preUI.IsTip = true;

        // 创建红线起始标
        GameObject startBar = new GameObject("StartBar", typeof(Image));
        startBar.transform.SetParent(preTipGO.transform, false);
        var startRect = startBar.GetComponent<RectTransform>();
        startRect.anchorMin = startRect.anchorMax = new Vector2(0, 0.5f);
        startRect.pivot = new Vector2(0, 0.5f);
        startRect.anchoredPosition = Vector2.zero;
        startRect.sizeDelta = new Vector2(3f, drumRect.sizeDelta.y);
        startBar.GetComponent<Image>().color = Color.red;

        // 创建红线结束标
        GameObject endBar = new GameObject("EndBar", typeof(Image));
        endBar.transform.SetParent(preTipGO.transform, false);
        var endRect = endBar.GetComponent<RectTransform>();
        endRect.anchorMin = endRect.anchorMax = new Vector2(1, 0.5f);
        endRect.pivot = new Vector2(1, 0.5f);
        endRect.anchoredPosition = Vector2.zero;
        endRect.sizeDelta = new Vector2(3f, drumRect.sizeDelta.y);
        endBar.GetComponent<Image>().color = Color.red;

        DrumsUIInDrums.Add(drumRoot.gameObject);
    }


    void StartLength()
    {
        float SongTime = this.SendQuery(new QueryAudioEditAudioClipLength());
        for (int i = 0; i < DrumsUI.Length; i++)
        {
            DrumsUI[i].sizeDelta = new Vector2(SongTime * _PixelUnitsPerSecond, _EditHeight / DrumsUI.Length);
        }
    }

    [SerializeField]
    private mUIDrumsInspectorPanel drumsInspectorPanel;

    private void InputContller()
    {
        if (!editModel.Mode.Equals(SystemModeData.RecordingMode)) return;

        if (InputSystems.Click) AddDrwms(TheTypeOfOperation.Click);
        if (InputSystems.SwipeUp) AddDrwms(TheTypeOfOperation.SwipeUp);
        if (InputSystems.SwipeDown) AddDrwms(TheTypeOfOperation.SwipeDown);
        if (InputSystems.SwipeLeft) AddDrwms(TheTypeOfOperation.SwipeLeft);
        if (InputSystems.SwipeRight) AddDrwms(TheTypeOfOperation.SwipeRight);

        // 添加刷新列表的方法
        if (drumsInspectorPanel != null)
        {
            drumsInspectorPanel.RefreshList();
        }
    }

    public void RemoveDrwms(int index = -1)
    {
        this.SendCommand(new RemoveAudioEditTimeLineDataCommand(editModel.ThisTime, index));
    }

    public void PlayAllDrwmsUI() { }
}
