using Qf.ClassDatas;
using Qf.ClassDatas.AudioEdit;
using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Querys.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Qf.Models.AudioEdit
{
    /// <summary>
    /// 音频编辑数据
    /// </summary>
    public class AudioEditModel : AbstractModel
    {
        public SystemModeData Mode = SystemModeData.EditMode;//模式
        public AudioClip EditAudioClip;//编辑的音频
        public AudioClip SucceedAudioClip;//交互成功的音频
        public AudioClip LoseAudioClip;//交互失败的音频
        public BindableProperty<float> TipOffset = new();//交互提示的偏移量(正数为在N秒前触发负数则相反)
        public float ThisTime;//当前时间
        public Dictionary<float, List<DrumsLoadData>> TimeLineData = new();//时间线数据(存储的为时间轴对应鼓点数据)
        public int BPM = 60;
        public int BeatA = 1;//B/A节拍
        public int BeatB = 1;
        protected override void OnInit()
        {

        }
        public void Load()
        {
            AudioSaveData audioSaveData = this.GetUtility<Storage>().Load<AudioSaveData>(".Save");
            DataCachingModel s = this.GetModel<DataCachingModel>();
            this.SendCommand(new SetAudioEditAudioCommand(this.SendQuery(new QueryAudioEditLoadAudio(audioSaveData.EditAudioClip))));
            this.SendCommand(new SetAudioEditSucceedAudioCommand(this.SendQuery(new QueryAudioEditLoadAudio(audioSaveData.SucceedAudioClip))));
            this.SendCommand(new SetAudioEditAudioLoseAudioCommand(this.SendQuery(new QueryAudioEditLoadAudio(audioSaveData.LoseAudioClip))));
            TipOffset.Value = audioSaveData.TipOffset;
            this.SendCommand(new SetAudioEditThisTimeCommand(audioSaveData.ThisTime));
            TimeLineData = audioSaveData.TimeLineData;
            this.SendEvent<OnUpdateAudioEditDrumsUI>();

        }
        public void Save()
        {
            AudioSaveData audioSaveData = new AudioSaveData();
            audioSaveData.EditAudioClip = EditAudioClip.name;
            audioSaveData.SucceedAudioClip = SucceedAudioClip.name;
            audioSaveData.LoseAudioClip = LoseAudioClip.name;
            audioSaveData.TipOffset = TipOffset.Value;
            audioSaveData.ThisTime = ThisTime;
            audioSaveData.TimeLineData = TimeLineData;
            this.GetUtility<Storage>().Save(audioSaveData,".Save");
        }
    }
    public class AudioSaveData
    {
        public string EditAudioClip;
        public string SucceedAudioClip;
        public string LoseAudioClip;
        public float TipOffset;//交互提示的偏移量(正数为在N秒前触发负数则相反)
        public float ThisTime;//当前时间
        public Dictionary<float, List<DrumsLoadData>> TimeLineData = new();//时间线数据(存储的为时间轴对应鼓点数据)
    }
}
