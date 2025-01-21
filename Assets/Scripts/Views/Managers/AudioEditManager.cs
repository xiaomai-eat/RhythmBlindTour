using Qf.ClassDatas;
using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Models;
using Qf.Models.AudioEdit;
using QFramework;
using RhythmTool;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
namespace Qf.Managers
{
    public class AudioEditManager : MonoBehaviour , IController
    {
        [SerializeField]
        AudioSource audioSource;//音频源
        [SerializeField]
        RhythmPlayer rhythmPlayer;//音频处理器
        [SerializeField]
        RhythmAnalyzer rhythmAnalyzer;//音频分析器
        AudioEditModel editModel;
        int Mode;
        private void Awake()
        {

        }
        void Start()
        {
            Init();
            this.RegisterEvent<MainAudioChangeValue>(v => {
                UpdateData();
                GetBPM();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        public void EnterPlayMode()
        {
            this.SendCommand(new SetAudioEditModeCommand(ClassDatas.AudioEdit.SystemModeData.PlayMode));
        }
        public void EnterEditMode()
        {
            this.SendCommand(new SetAudioEditModeCommand(ClassDatas.AudioEdit.SystemModeData.EditMode));
        }
        public void EnterRecordingMode()
        {
            this.SendCommand(new SetAudioEditModeCommand(ClassDatas.AudioEdit.SystemModeData.RecordingMode));
        }
        float sum;
        public async void GetBPM()
        {
            if(rhythmPlayer.rhythmData == null)
            {
                Debug.Log("[AudioEditManager] 无分析对象");
                return;
            }
            Debug.Log("[AudioEditManager] 等待分析");
            await Task.Delay(3000);
            if (rhythmPlayer.rhythmData == null)
            {
                Debug.Log("[AudioEditManager] 无分析对象");
                return;
            }
            Track<Beat> ls = rhythmPlayer.rhythmData.GetTrack<Beat>();
            sum = 0;
            for (int i = 0; i < ls.count; i++)
            {
                sum += ls[i].bpm;
            }
            sum /= ls.count;
            Debug.Log($"数据组{ls.count},{sum}");
            this.SendCommand(new SetAudioEditAudioBPMCommand((int)Mathf.Round(sum)));
            return;
        }
        void Init()
        {
            editModel = this.GetModel<AudioEditModel>();
            if(editModel.EditAudioClip != null)
            {
                audioSource.clip = editModel.EditAudioClip;
            }
            this.RegisterEvent<OnEditMode>(v =>
            {
                Debug.Log("编辑模式");
                Mode = 0;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnPlayMode>(v =>
            {
                Debug.Log("游玩模式");
                Mode = 1;
                PlayMode();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnRecordingMode>(v =>
            {
                Debug.Log("录制模式");
                Mode = 2;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ExitPlayMode>(v =>
            {
                Debug.Log("退出游玩模式");
                ExitPlayMode();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ExitRecordingMode>(v =>
            {
                Debug.Log("退出录制模式");
                ExitPlayMode();
            });
        }
        void UpdateAll()
        {
            this.SendCommand(new SetAudioEditThisTimeCommand(audioSource.time)); //这里可以使用字段优化因为会一直产生垃圾
        }
        void PlayMode()
        {
            UpdateData();
            audioSource.Play();
        }
        void ExitPlayMode()
        {
            audioSource.Pause();
        }
        /// <summary>
        /// 控制运行,适用于开关
        /// </summary>
        public void ControlRun()
        {
            if (editModel.EditAudioClip == null) return;
            if (audioSource.isPlaying)
            {
                ExitPlayMode();
                return;
            }
            PlayMode();
        }
        void UpdateData()
        {
            audioSource.time = editModel.ThisTime;
            audioSource.clip = editModel.EditAudioClip;
            rhythmPlayer.rhythmData = rhythmAnalyzer.Analyze(editModel.EditAudioClip);
        }
        private void Update()
        {
            if(audioSource.isPlaying)
                UpdateAll();
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameBody.Interface;
        }
    }
}