using UnityEngine;
namespace Qf.ClassDatas.AudioEdit
{
    [CreateAssetMenu(fileName = "DrwmsDataSet", menuName = "ScriptableObjects/AudioEdit/DrwmsData", order = 1)]
    public class DrwmsData:ScriptableObject
    {
        public float TriggerTimeRange;//触发时间范围
        public float DelayTheTriggerTime = 0.1f;//延迟触发时间
    }
}
