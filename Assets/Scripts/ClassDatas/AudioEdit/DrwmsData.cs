using System.Drawing;

namespace Qf.ClassDatas.AudioEdit
{
    public class DrwmsData
    {
        public Color Color;//颜色
        public TheTypeOfOperation theTypeOfOperation;//鼓点类型
        public float DelayTheTriggerTime = 0.1f;//延迟触发时间
        public float Speed = 1;
        public string PreAdventAudioClipPath;//来临前的音频数据路径
        public string SucceedAudioClipPath;//成功时的音频数据路径
        public string FailAudioClipPath;//失败时的音频路径
    }
}
