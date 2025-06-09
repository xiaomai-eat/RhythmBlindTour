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

        this.SendCommand(new AddAudioEditTimeLineDataCommand(editModel.ThisTime, newDrums));
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
        float start = item + tipOffset;

        var drumRoot = new GameObject("DrumRoot");
        drumRoot.transform.SetParent(DrumsUI[i].transform);
        RectTransform rootRect = drumRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0, 0.5f);
        rootRect.anchorMax = new Vector2(0, 0.5f);
        rootRect.pivot = new Vector2(0, 0.5f);
        rootRect.anchoredPosition = Vector2.zero;
        rootRect.localScale = Vector3.one;

        var preTipGO = new GameObject("PreTipArea");
        preTipGO.transform.SetParent(drumRoot.transform);
        RectTransform preRect = preTipGO.AddComponent<RectTransform>();
        preRect.anchorMin = new Vector2(0, 0.5f);
        preRect.anchorMax = new Vector2(0, 0.5f);
        preRect.pivot = new Vector2(0, 0.5f);
        preRect.anchoredPosition = new Vector2(preStart * _PixelUnitsPerSecond, 0);
        preRect.sizeDelta = new Vector2(tipOffset * _PixelUnitsPerSecond, _EditHeight / DrumsUI.Length);

        var preImage = preTipGO.AddComponent<UnityEngine.UI.Image>();
        preImage.color = new Color(192f / 255f, 192f / 255f, 192f / 255f, 0.5f); // RGB(192,192,192)

        var drumGO = Instantiate(DrumsProfabs, drumRoot.transform);
        RectTransform drumRect = drumGO.GetComponent<RectTransform>();

        drumRect.anchoredPosition = new Vector2(start * _PixelUnitsPerSecond, 0);

        drumRect.sizeDelta = new Vector2(existence * _PixelUnitsPerSecond, drumRect.sizeDelta.y);

        var uiDrums = drumGO.GetComponent<UIAudioEditDrums>();
        uiDrums.SetColor(new Color(211f / 255f, 84f / 255f, 0f, 1f)); uiDrums.ThisTime = item;
        uiDrums.Index = i;

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
