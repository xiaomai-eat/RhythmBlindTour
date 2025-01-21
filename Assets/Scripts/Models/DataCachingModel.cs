
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Qf.Models
{
    public class DataCachingModel : AbstractModel
    {
        public List<AudioClip> ResourceAudioDatas = new();
        protected override void OnInit()
        {
            ResourceAudioDatas = GetResourceAudioClipData();
        }
        List<AudioClip> GetResourceAudioClipData()
        {
            AudioClip[] ls = Resources.LoadAll<AudioClip>("Audios");
            return ls.ToList();
        }
    }
}