namespace Qf.Events
{
    public struct SucceedTrigger { }
    public struct LoseTrigger { }
    /// <summary>
    /// 在鼓点出现时提供的事件
    /// </summary>
    public struct DrumsGenerate
    {
        public InputMode InputMode;
    }
}