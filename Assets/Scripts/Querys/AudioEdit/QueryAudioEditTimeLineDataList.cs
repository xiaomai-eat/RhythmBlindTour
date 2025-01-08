using Qf.ClassDatas.AudioEdit;
using Qf.Models.AudioEdit;
using QFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Qf.Querys.AudioEdit
{
    public class QueryAudioEditTimeLineDataList : AbstractQuery<List<DrumsLoadData>>
    {
        AudioEditModel audioEdit;
        float value;
        /// <summary>
        /// 获取编辑器时间轴上对应时间的数据List
        /// </summary>
        /// <param name="value">对应时间</param>
        public QueryAudioEditTimeLineDataList(float value)
        {
            this.value = value;
        }
        protected override List<DrumsLoadData> OnDo()
        {
            audioEdit = this.GetModel<AudioEditModel>();
            if (audioEdit.TimeLineData.ContainsKey(value))
            {
                if (audioEdit.TimeLineData[value] != null)
                    return audioEdit.TimeLineData[value];
                else
                {
                    Debug.LogError("[QueryAudioEditTimeLineDataList] 数组未被实例化拿不到数据");
                    return null;
                }
            }
            Debug.LogError("[QueryAudioEditTimeLineDataList] 对应时间轴不存在对应鼓点数据");
            return null;
        }
    }
}
