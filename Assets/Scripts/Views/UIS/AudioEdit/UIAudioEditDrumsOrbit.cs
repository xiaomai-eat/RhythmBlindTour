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
        if (i == 4)
            AddDrwms(TheTypeOfOperation.Click);
        else if (i == 1)
        {
            AddDrwms(TheTypeOfOperation.SwipeDown);
        }
        else if (i == 0)
        {
            AddDrwms(TheTypeOfOperation.SwipeUp);
        }
        else if (i == 2)
        {
            AddDrwms(TheTypeOfOperation.SwipeLeft);
        }
        else if (i == 3)
        {
            AddDrwms(TheTypeOfOperation.SwipeRight);
        }
        else
        {

        }
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
            go = null;
            for (int i = 0; i < a[item].Count; i++)
            {
                if (a[item][i] == null) continue;
                go = Instantiate(DrumsProfabs).transform;
                go.SetParent(DrumsUI[i].transform);
                gorecttransform = go.GetComponent<RectTransform>();
                go.transform.position = new Vector3(transform.position.x, DrumsUI[i].transform.position.y - (_EditHeight / DrumsUI.Length / 2), transform.position.z);
                gorecttransform.anchoredPosition = new Vector2(item * _PixelUnitsPerSecond, gorecttransform.anchoredPosition.y);
                gorecttransform.sizeDelta = new Vector2(a[item][i].DrwmsData.VTimeOfExistence * gorecttransform.sizeDelta.x, gorecttransform.sizeDelta.y);
                uiDrums = go.GetComponent<UIAudioEditDrums>();
                uiDrums.SetColor(new Color(Random.Range(0, 101) / (float)100, Random.Range(0, 101) / (float)100, Random.Range(0, 101) / (float)100, 1));
                uiDrums.ThisTime = item;
                uiDrums.Index = i;
                DrumsUIInDrums.Add(go.gameObject);
            }
        }
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
