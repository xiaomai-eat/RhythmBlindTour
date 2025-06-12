using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Qf.Models.AudioEdit;
using QFramework;
using Qf.ClassDatas.AudioEdit;
using Qf.Commands.AudioEdit;
using Qf.Events;

public class mUIDrumsInspectorPanel : MonoBehaviour, IController
{
    public GameObject ItemPrefab;
    public Transform ContentRoot;
    private AudioEditModel editModel;
    private List<DrumRow> rows = new();

    void Start()
    {
        StartCoroutine(InitEditModelIfNeeded());

        // 注册时间针变更事件（仅非PlayMode生效）
        this.RegisterEvent<OnUpdateThisTime>(OnTimeNeedCheck)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    IEnumerator InitEditModelIfNeeded()
    {
        yield return new WaitUntil(() => GameBody.Interface != null);

        while (editModel == null)
        {
            try
            {
                editModel = this.GetModel<AudioEditModel>();
            }
            catch { }
            yield return null;
        }
    }

    public void EnsureReadyAndRefresh()
    {
        if (editModel == null)
        {
            try
            {
                editModel = this.GetModel<AudioEditModel>();
            }
            catch
            {
                Debug.LogError("[mUIDrumsInspectorPanel] editModel 获取失败");
                return;
            }
        }

        if (editModel != null)
        {
            RefreshList();
        }
    }

    public void RefreshList()
    {
        if (ItemPrefab == null || ContentRoot == null || editModel == null)
        {
            Debug.LogError("[mUIDrumsInspectorPanel] 组件未就绪，刷新失败");
            return;
        }

        foreach (var row in rows)
            Destroy(row.GameObject);
        rows.Clear();

        int counter = 1;
        var timeDict = editModel.TimeLineData;

        // 统计重复时间（以 0.00 格式）
        Dictionary<string, int> timeTextCount = new();
        foreach (var time in timeDict.Keys)
        {
            string timeStr = Math.Round(time, 2).ToString("0.00");
            if (!timeTextCount.ContainsKey(timeStr))
                timeTextCount[timeStr] = 0;
            timeTextCount[timeStr] += timeDict[time].Count;
        }

        HashSet<string> duplicatedTimeStrings = new();
        foreach (var kv in timeTextCount)
        {
            if (kv.Value > 1)
                duplicatedTimeStrings.Add(kv.Key);
        }

        foreach (var time in new List<float>(timeDict.Keys))
        {
            float roundedTime = (float)Math.Round(time, 2, MidpointRounding.ToEven);
            string timeStr = roundedTime.ToString("0.00");

            var drumList = timeDict[time];

            for (int i = 0; i < drumList.Count; i++)
            {
                var data = drumList[i];
                var go = Instantiate(ItemPrefab, ContentRoot);
                int localIndex = i;

                var row = new DrumRow(go, counter++, roundedTime, data.DrwmsData.DtheTypeOfOperation);

                row.Init(
                    onDelete: () => row.SetPendingDelete(true),
                    onUndo: () => row.SetPendingDelete(false),
                    onConfirm: () =>
                    {
                        this.SendCommand(new RemoveAudioEditTimeLineDataCommand(roundedTime, localIndex));
                        RefreshList();
                    }
                );

                if (duplicatedTimeStrings.Contains(timeStr))
                {
                    row.MarkAsDuplicatedTime();
                }

                rows.Add(row);
            }
        }

        // 初次刷新时同步一次当前高亮（如果不是PlayMode）
        if (editModel.Mode != SystemModeData.PlayMode)
        {
            OnTimeNeedCheck(new OnUpdateThisTime() { ThisTime = editModel.ThisTime });
        }
    }

    void OnTimeNeedCheck(OnUpdateThisTime evt)
    {
        if (editModel == null || editModel.Mode == SystemModeData.PlayMode) return;

        string currentTime = evt.ThisTime.ToString("0.00");

        foreach (var row in rows)
        {
            row.SetHighlighted(row.GetTimeTextValue() == currentTime);
        }
    }

    class DrumRow
    {
        public GameObject GameObject;
        TMP_Text IndexText, TimeText, TypeText;
        Button ActionButton, UndoButton, ConfirmButton;
        Image Background;

        bool isDuplicatedTime = false;
        bool isDeleted = false;
        bool isHighlighted = false;

        public DrumRow(GameObject go, int index, float time, TheTypeOfOperation type)
        {
            GameObject = go;

            Transform tIndex = go.transform.Find("Index");
            Transform tTime = go.transform.Find("Time");
            Transform tType = go.transform.Find("Type");
            Transform tDel = go.transform.Find("DeleteBtn");
            Transform tUndo = go.transform.Find("UndoBtn");
            Transform tConfirm = go.transform.Find("ConfirmBtn");

            IndexText = tIndex?.GetComponent<TMP_Text>();
            TimeText = tTime?.GetComponent<TMP_Text>();
            TypeText = tType?.GetComponent<TMP_Text>();

            ActionButton = tDel?.GetComponent<Button>();
            UndoButton = tUndo?.GetComponent<Button>();
            ConfirmButton = tConfirm?.GetComponent<Button>();
            Background = go.GetComponent<Image>();

            IndexText.text = index.ToString("D3");
            TimeText.text = time.ToString("0.00");
            TypeText.text = ConvertType(type);

            UpdateTextColor();

            ActionButton?.gameObject.SetActive(true);
            UndoButton?.gameObject.SetActive(false);
            ConfirmButton?.gameObject.SetActive(false);

            var button = go.GetComponent<Button>() ?? go.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                var timeHand = GameObject.FindObjectOfType<UIAudioEditTimeHand>();
                if (timeHand != null)
                    timeHand.SetTime(time);
            });
        }

        public void Init(Action onDelete, Action onUndo, Action onConfirm)
        {
            ActionButton?.onClick.AddListener(() =>
            {
                isDeleted = true;
                onDelete?.Invoke();
                UpdateTextColor();
            });

            UndoButton?.onClick.AddListener(() =>
            {
                isDeleted = false;
                onUndo?.Invoke();
                UpdateTextColor();
            });

            ConfirmButton?.onClick.AddListener(() =>
            {
                isDeleted = false;
                onConfirm?.Invoke();
            });
        }

        public void SetPendingDelete(bool pending)
        {
            isDeleted = pending;
            ActionButton?.gameObject.SetActive(!pending);
            UndoButton?.gameObject.SetActive(pending);
            ConfirmButton?.gameObject.SetActive(pending);
            UpdateTextColor();
        }

        public void MarkAsDuplicatedTime()
        {
            isDuplicatedTime = true;
            UpdateTextColor();
        }

        public void SetHighlighted(bool highlight)
        {
            isHighlighted = highlight;
            UpdateTextColor();
        }

        void UpdateTextColor()
        {
            if (isHighlighted)
            {
                IndexText.color = Color.green;
                TimeText.color = Color.green;
                TypeText.color = Color.green;
                return;
            }

            Color color = isDeleted ? Color.red :
                           isDuplicatedTime ? Color.yellow :
                           Color.white;

            IndexText.color = color;
            TimeText.color = color;
            TypeText.color = color;
        }

        public string GetTimeTextValue() => TimeText.text;

        string ConvertType(TheTypeOfOperation op) => op switch
        {
            TheTypeOfOperation.Click => "单击",
            TheTypeOfOperation.SwipeUp => "上滑",
            TheTypeOfOperation.SwipeDown => "下滑",
            TheTypeOfOperation.SwipeLeft => "左滑",
            TheTypeOfOperation.SwipeRight => "右滑",
            _ => "未知"
        };
    }

    public IArchitecture GetArchitecture() => GameBody.Interface;
}
