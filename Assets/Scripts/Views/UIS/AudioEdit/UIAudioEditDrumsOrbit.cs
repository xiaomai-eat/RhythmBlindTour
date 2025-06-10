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
    [SerializeField]
    GameObject DrumsProfabs;//�ĵ�Ԥ����
    [SerializeField]
    RectTransform[] DrumsUI;
    [SerializeField]
    List<GameObject> DrumsUIInDrums = new();
    int _PixelUnitsPerSecond = AudioEditConfig.PixelUnitsPerSecond;//ÿ�����ص�λ
    int _EditHeight = AudioEditConfig.EditHeight;//�༭���ɱ༭��Χ�߶�
    AudioEditModel editModel;
    void Start()
    {
        Init();
    }
    void Update()
    {
        InputContller();
    }
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return GameBody.Interface;
    }
    public void AddDrwms(int i = 0)
    {
        TheTypeOfOperation operation;
        switch (i)
        {
            case 0:
                operation = TheTypeOfOperation.SwipeUp;
                break;
            case 1:
                operation = TheTypeOfOperation.SwipeDown;
                break;
            case 2:
                operation = TheTypeOfOperation.SwipeLeft;
                break;
            case 3:
                operation = TheTypeOfOperation.SwipeRight;
                break;
            case 4:
                operation = TheTypeOfOperation.Click;
                break;
            default:
                Debug.LogWarning($"δ��������Ʊ��: {i}");
                return;
        }
        AddDrwms(operation);
    }

    public void AddDrwms(TheTypeOfOperation theTypeOfOperation)
    {
        if (editModel.EditAudioClip == null) return;

        var tipClip = this.SendQuery(new QueryAudioEditComeTipAudio(theTypeOfOperation));
        var succeedClip = this.SendQuery(new QueryAudioEditSucceedsAudio(theTypeOfOperation));
        var loseClip = editModel.LoseAudioClip;

        var newDrums = new DrumsLoadData()
        {
            DrwmsData = new DrwmsData()
            {
                DtheTypeOfOperation = theTypeOfOperation,
                FPreAdventAudioClipPath = tipClip?.name,
                FSucceedAudioClipPath = succeedClip?.name,
                FLoseAudioClipPath = loseClip?.name,
                VPreAdventAudioClipOffsetTime = editModel.TipOffset.Value,
                VTimeOfExistence = editModel.TimeOfExistence.Value
            },
            MusicData = new MusicData()
            {
                SPreAdventVolume = editModel.PreAdventVolume.Value,
                SLoseVolume = editModel.LoseAudioVolume.Value,
                SSucceedVolume = editModel.SucceedAudioVolume.Value
            }
        };
        
        this.SendCommand(new AddAudioEditTimeLineDataCommand(editModel.ThisTime + editModel.TipOffset.Value, newDrums)); // 指针位置设置提示音位置 偏移鼓点到提示音后 2025/06/10 - mixyao 
    }


    public void RemoveDrwms(int index = -1)
    {
        Debug.Log($"ɾ���ĵ�{editModel.ThisTime}");
        this.SendCommand(new RemoveAudioEditTimeLineDataCommand(
            editModel.ThisTime,
            index
            ));
    }
    public void PlayAllDrwmsUI()
    {

    }
    void Init()
    {
        editModel = this.GetModel<AudioEditModel>();
        StartLength();
        this.RegisterEvent<OnUpdateAudioEditDrumsUI>(v => UpDateDrwmsUI()).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<MainAudioChangeValue>(v => StartLength()).UnRegisterWhenGameObjectDestroyed(gameObject);

    }
    void UpDateDrwmsUI()//������Ҫ�Ż�<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<(Ŀǰ����ʱʵ�ֹ���)
    {
        //��ʼ���ĵ�����
        UpDateAllDrwmsUI();
    }
    void UpDateAllDrwmsUI()
    {
        //����ĵ�UI
        foreach (var item in DrumsUIInDrums)
        {
            Destroy(item);
        }
        DrumsUIInDrums.Clear();
        //ͨ���ֵ�����ʵ�������йĵ�λ��
        var a = this.SendQuery(new QueryAudioEditTimeLineAllData());
        Transform go;
        UIAudioEditDrums uiDrums;
        RectTransform gorecttransform;
        foreach (var item in a.Keys)
        {
            for (int i = 0; i < a[item].Count; i++)
            {
                if (a[item][i] == null) continue;
                CreateDrumItemUI(item, i, a);
            }
        }

    }

void CreateDrumItemUI(float item, int i, Dictionary<float, List<DrumsLoadData>> a)
{
    var drumData = a[item][i];
    float tipOffset = drumData.DrwmsData.VPreAdventAudioClipOffsetTime;
    float existence = drumData.DrwmsData.VTimeOfExistence;
    float preStart = item - tipOffset;
    float start = item;

    float pixelExistence = existence * _PixelUnitsPerSecond;
    float drumX = start * _PixelUnitsPerSecond;

    // 创建 drumRoot
    var drumRoot = new GameObject("DrumRoot");
    drumRoot.transform.SetParent(DrumsUI[i].transform);
    RectTransform rootRect = drumRoot.AddComponent<RectTransform>();
    rootRect.anchorMin = new Vector2(0, 0.5f);
    rootRect.anchorMax = new Vector2(0, 0.5f);
    rootRect.pivot = new Vector2(0, 0.5f);
    rootRect.anchoredPosition = Vector2.zero;
    rootRect.localScale = Vector3.one;

    // 创建 drum 本体
    var drumGO = Instantiate(DrumsProfabs, drumRoot.transform);
    drumGO.name = "Drum";
    RectTransform drumRect = drumGO.GetComponent<RectTransform>();
    drumRect.anchorMin = new Vector2(0, 0.5f);
    drumRect.anchorMax = new Vector2(0, 0.5f);
    drumRect.pivot = new Vector2(0, 0.5f);
    drumRect.localScale = Vector3.one;

    // drum 左移一半长度
    drumRect.anchoredPosition = new Vector2(drumX - pixelExistence / 2f, 0);
    drumRect.sizeDelta = new Vector2(pixelExistence, drumRect.sizeDelta.y);

    var uiDrums = drumGO.GetComponent<UIAudioEditDrums>();
    uiDrums.ThisTime = item;
    uiDrums.Index = i;

    // 创建 preTip 区域
    var preTipGO = Instantiate(DrumsProfabs, drumRoot.transform);
    preTipGO.name = "PreTipArea";
    RectTransform preRect = preTipGO.GetComponent<RectTransform>();
    preRect.anchorMin = new Vector2(0, 0.5f);
    preRect.anchorMax = new Vector2(0, 0.5f);
    preRect.pivot = new Vector2(1, 0.5f); // 右对齐 drum 左侧
    preRect.localScale = Vector3.one;
    preRect.anchoredPosition = new Vector2(drumX, 0); // 不变
    preRect.sizeDelta = new Vector2(tipOffset * _PixelUnitsPerSecond, drumRect.sizeDelta.y);

    var preUI = preTipGO.GetComponent<UIAudioEditDrums>();
    preUI.SetColor(new Color(40f / 255f, 40f / 255f, 40f / 255f, 0.8f));
    preUI.ThisTime = item - tipOffset;
    preUI.Index = i;
    uiDrums.SetColor(new Color(211f / 255f, 84f / 255f, 0f, 1f));

    // 添加红色起始柱
    GameObject startBar = new GameObject("StartBar", typeof(Image));
    startBar.transform.SetParent(preTipGO.transform, false);
    RectTransform startRect = startBar.GetComponent<RectTransform>();
    startRect.anchorMin = new Vector2(0, 0.5f);
    startRect.anchorMax = new Vector2(0, 0.5f);
    startRect.pivot = new Vector2(0, 0.5f);
    startRect.anchoredPosition = Vector2.zero;
    startRect.sizeDelta = new Vector2(3f, drumRect.sizeDelta.y); // 3f 为红柱宽度
    startBar.GetComponent<Image>().color = Color.red;

    // 添加红色结束柱
    GameObject endBar = new GameObject("EndBar", typeof(Image));
    endBar.transform.SetParent(preTipGO.transform, false);
    RectTransform endRect = endBar.GetComponent<RectTransform>();
    endRect.anchorMin = new Vector2(1, 0.5f);
    endRect.anchorMax = new Vector2(1, 0.5f);
    endRect.pivot = new Vector2(1, 0.5f);
    endRect.anchoredPosition = Vector2.zero;
    endRect.sizeDelta = new Vector2(3f, drumRect.sizeDelta.y);
    endBar.GetComponent<Image>().color = Color.red;

    // 添加 root 到管理列表
    DrumsUIInDrums.Add(drumRoot.gameObject);
}



    void StartLength()
    {
        //��ʼ���������?
        float SongTime = this.SendQuery(new QueryAudioEditAudioClipLength());
        for (int i = 0; i < DrumsUI.Length; i++)
        {
            DrumsUI[i].sizeDelta = new Vector2(SongTime * _PixelUnitsPerSecond, _EditHeight / DrumsUI.Length);
        }
    }


    private void InputContller()
    {
        if (!editModel.Mode.Equals(SystemModeData.RecordingMode)) return;
        if (InputSystems.Click)
        {
            AddDrwms(TheTypeOfOperation.Click);
        }
        if (InputSystems.SwipeUp)
        {
            AddDrwms(TheTypeOfOperation.SwipeUp);
        }
        if (InputSystems.SwipeDown)
        {
            AddDrwms(TheTypeOfOperation.SwipeDown);
        }
        if (InputSystems.SwipeLeft)
        {
            AddDrwms(TheTypeOfOperation.SwipeLeft);
        }
        if (InputSystems.SwipeRight)
        {
            AddDrwms(TheTypeOfOperation.SwipeRight);
        }
    }
}
