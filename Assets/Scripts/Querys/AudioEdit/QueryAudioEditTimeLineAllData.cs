using Qf.ClassDatas.AudioEdit;
using Qf.Models.AudioEdit;
using QFramework;
using System.Collections.Generic;

namespace Qf.Querys.AudioEdit
{
    /// <summary>
    /// 获取编辑器时间轴上的所有数据
    /// </summary>
    public class QueryAudioEditTimeLineAllData : AbstractQuery<Dictionary<float,List<DrumsLoadData>>>
    {
        AudioEditModel audioEdit;
        Dictionary<float,List<DrumsLoadData>> allData = new();
        bool siftOutNull;
        /// <summary>
        /// 获取编辑器时间轴上的所有数据
        /// </summary>
        /// <param name="SiftOutNull">筛除空列表(没有数据的表会不存在)</param>
        public QueryAudioEditTimeLineAllData(bool SiftOutNull = true)
        {
            this.siftOutNull = SiftOutNull;
        }
        protected override Dictionary<float,List<DrumsLoadData>> OnDo()
        {
            audioEdit = this.GetModel<AudioEditModel>();
            if (siftOutNull)
            {
                foreach (var i in audioEdit.TimeLineData.Keys)
                {
                    if (audioEdit.TimeLineData[i] != null)
                        allData.Add(i,audioEdit.TimeLineData[i]);
                }
            }
            else
            {
                foreach (var i in audioEdit.TimeLineData.Keys)
                {
                    allData.Add(i,audioEdit.TimeLineData[i]);
                }
            }
            return allData;
        }
    }
}
