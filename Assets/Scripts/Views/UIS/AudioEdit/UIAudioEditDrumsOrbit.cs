using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Models.AudioEdit;
using Qf.Querys.AudioEdit;
using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioEditDrumsOrbit : MonoBehaviour,IController
{
    [SerializeField]
    GameObject DrumsProfabs;//鼓点预制体
    [SerializeField]
    RectTransform[] DrumsUI;
    [SerializeField]
    List<GameObject> DrumsUIInDrums = new ();
    int _PixelUnitsPerSecond = AudioEditConfig.PixelUnitsPerSecond;//每秒像素单位
    int _EditHeight = AudioEditConfig.EditHeight;//编辑器可编辑范围高度
    AudioEditModel editModel;
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return GameBody.Interface;
    }
    public void AddDrwms()
    {
        Debug.Log($"生成鼓点{editModel.ThisTime}");
        this.SendCommand(new AddAudioEditTimeLineDataCommand(
            editModel.ThisTime,
            new Qf.ClassDatas.AudioEdit.DrumsLoadData()));
    }

    public void RemoveDrwms(int index = -1)
    {
        Debug.Log($"删除鼓点{editModel.ThisTime}");
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
    void UpDateDrwmsUI()//这里需要优化<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<(目前仅暂时实现功能)
    {
        //初始化鼓点数据
        UpDateAllDrwmsUI();
    }
    void UpDateAllDrwmsUI()
    {
        //清除鼓点UI
        foreach(var item in DrumsUIInDrums)
        {
            Destroy(item);
        }
        DrumsUIInDrums.Clear();
        //通过字典重新实例化所有鼓点位置
        var a = this.SendQuery(new QueryAudioEditTimeLineAllData());
        Transform go;
        UIAudioEditDrums uiDrums;
        RectTransform gorecttransform;
        foreach (var item in a.Keys)
        {
            go = null;
            for (int i=0; i<a[item].Count;i++)
            {
                go = Instantiate(DrumsProfabs).transform;
                go.SetParent(DrumsUI[i].transform);
                gorecttransform = go.GetComponent<RectTransform>();
                go.transform.position = new Vector3(transform.position.x, DrumsUI[i].transform.position.y- (_EditHeight / DrumsUI.Length/2),transform.position.z);
                gorecttransform.anchoredPosition = new Vector2(item * _PixelUnitsPerSecond, gorecttransform.anchoredPosition.y);
                uiDrums = go.GetComponent<UIAudioEditDrums>();
                uiDrums.SetColor(new Color(Random.Range(0,101)/(float)100,Random.Range(0, 101) / (float)100, Random.Range(0, 101) / (float)100, 1));
                uiDrums.ThisTime = item;
                uiDrums.Index = i;
                DrumsUIInDrums.Add(go.gameObject);
            }
        }
    }
    void StartLength()
    {
        //初始化轨道长度
        float SongTime = this.SendQuery(new QueryAudioEditAudioClipLength());
        for (int i = 0; i < DrumsUI.Length; i++)
        {
            DrumsUI[i].sizeDelta = new Vector2(SongTime * _PixelUnitsPerSecond, _EditHeight/ DrumsUI.Length);
        }
    }
    void Start()
    {
        Init();
    }

}
