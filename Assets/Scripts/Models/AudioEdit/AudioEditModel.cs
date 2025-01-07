using Qf.ClassDatas;
using Qf.ClassDatas.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Qf.Models.AudioEdit
{
    public class AudioEditModel : AbstractModel
    {
        public SystemModeData Mode = SystemModeData.EditMode;//模式
        public AudioClip EditAudioClip;//编辑的音频
        public float ThisTime;//当前时间
        public Dictionary<float, DrumsLoadData> TimeLineData = new();//时间线数据
        //public DrumsLoadData GetThisTimeLoadData()
        //{
        //    if (!TimeLineData.ContainsKey(ThisTime))
        //    {
        //        return null;
        //    }
        //    return TimeLineData[ThisTime];
        //}
        //public bool EqueryThisTimeLoadData()
        //{
        //    if (!TimeLineData.ContainsKey(ThisTime))
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        protected override void OnInit()
        {

        }

    }

}
