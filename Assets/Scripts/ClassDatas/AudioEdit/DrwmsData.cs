using System.Drawing;

namespace Qf.ClassDatas.AudioEdit
{
    public class DrwmsData
    {
        public Color Color;//颜色
        public TheTypeOfOperation theTypeOfOperation;//鼓点类型
        public float TriggerTimeRange;//触发时间范围
        public float DelayTheTriggerTime = 0.1f;//延迟触发时间
        public float Speed = 1;
    }
}
