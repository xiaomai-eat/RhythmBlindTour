using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Models.AudioEdit;
using QFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Qf.Commands.AudioEdit
{
    /// <summary>
    /// 设置编辑器时间轴上的数据命令
    /// </summary>
    public class SetEditAudioTimeLineDataCommand : AbstractCommand
    {
        float time;
        List<DrumsLoadData> value;
        DrumsLoadData value2;
        int index;
        AudioEditModel AudioEditModel;
        /// <summary>
        /// 设置编辑器时间轴上的数据命令
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="value">值(List)</param>
        public SetEditAudioTimeLineDataCommand(float time, List<DrumsLoadData> value)
        {
            this.time = time;
            this.value = value;
            this.index = -1;
        }
        /// <summary>
        /// 设置编辑器时间轴上的数据命令(通过下标直接修改对应值)
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="index">下标</param>
        /// <param name="value">值(DrumsLoadData)</param>
        public SetEditAudioTimeLineDataCommand(float time, int index, DrumsLoadData value)
        {
            this.time = time;
            this.value2 = value;
            this.index = index;
        }
        protected override void OnExecute()
        {
            AudioEditModel = this.GetModel<AudioEditModel>();
            if (index == -1)
            {
                if(AudioEditModel.TimeLineData.ContainsKey(time))
                    AudioEditModel.TimeLineData[time] = value;
                else
                    AudioEditModel.TimeLineData.Add(time, value);
            }
            else
            {
                if (AudioEditModel.TimeLineData[time].Count > index)
                    AudioEditModel.TimeLineData[time][index] = value2;
                else
                    Debug.LogError("[SetEditAudioTimeLineDataCommand] 超出下标位置");
            }
            this.SendEvent<OnUpdateAudioEditDrumsUI>();
        }
    }
}
