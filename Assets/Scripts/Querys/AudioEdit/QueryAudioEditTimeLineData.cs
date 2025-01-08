using Qf.ClassDatas.AudioEdit;
using Qf.Models.AudioEdit;
using QFramework;
using UnityEngine;

namespace Qf.Querys.AudioEdit
{
    /// <summary>
    /// 查询音频编辑器时间轴上的数据
    /// </summary>
    public class QueryAudioEditTimeLineData : AbstractQuery<DrumsLoadData>
    {
        AudioEditModel audioEdit;
        float value;
        int index;
        /// <summary>
        /// 查询对应时间轴上信息
        /// </summary>
        /// <param name="value">时间</param>
        /// <param name="index">对应轨道的索引</param>
        public QueryAudioEditTimeLineData(float value,int index)
        {
            this.value = value;
            this.index = index;
        }
        protected override DrumsLoadData OnDo()
        {
            audioEdit = this.GetModel<AudioEditModel>();
            if (audioEdit.TimeLineData.ContainsKey(value))
            {
                if (audioEdit.TimeLineData[value] != null && audioEdit.TimeLineData[value].Count > index)
                    return audioEdit.TimeLineData[value][index];
                else if(audioEdit.TimeLineData[value] == null)
                {
                    Debug.LogError("[QueryAudioEditTimeLineData] 数据未被实例化");
                    return null;
                }
                else
                {
                    Debug.LogError("[QueryAudioEditTimeLineData] 索引超出范围");
                    return null;
                }
            }
            Debug.LogError("[QueryAudioEditTimeLineData] 对应时间轴不存在对应鼓点数据");
                return null;
        }
    }
}
