
using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Models.AudioEdit;
using QFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Qf.Commands.AudioEdit
{
    /// <summary>
    /// 添加编辑器时间轴上的数据命令
    /// </summary>
    public class AddAudioEditTimeLineDataCommand : AbstractCommand
    {
        float time;
        DrumsLoadData value;
        AudioEditModel AudioEditModel;
        /// <summary>
        /// 添加编辑器时间轴上的数据命令
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="value">值</param>
        public AddAudioEditTimeLineDataCommand(float time, DrumsLoadData value)
        {
            this.time = time;
            this.value = value;
        }

        int Count;
        protected override void OnExecute()
        {
            AudioEditModel = this.GetModel<AudioEditModel>();
            if (AudioEditModel.EditAudioClip == null) return;
            if (AudioEditModel.TimeLineData.ContainsKey(time))
            {
                if (AudioEditModel.TimeLineData[time] != null)
                    Count = AudioEditModel.TimeLineData[time].Count;
                else
                    AudioEditModel.TimeLineData[time] = new List<DrumsLoadData>();
                if (Count >= 5)
                {
                    Debug.LogError("[AddEditAudioTimeLineDataCommand] 当前时间节点鼓点已满");
                    return;
                }
                AudioEditModel.TimeLineData[time].Add(value);
            }
            else
            {
                AudioEditModel.TimeLineData.Add(time, new List<DrumsLoadData>());
                AudioEditModel.TimeLineData[time].Add(value);
            }
            this.SendEvent<OnUpdateAudioEditDrumsUI>();
        }
    }
}
