using Qf.Models.AudioEdit;
using QFramework;

namespace Qf.Querys.AudioEdit
{
    /// <summary>
    /// 查询音频编辑器音频当前的时间(播放时间<秒>)
    /// </summary>
    public class QueryAudioEditAudioClipThisTime : AbstractQuery<float>
    {
        AudioEditModel audioEditModel;
        protected override float OnDo()
        {
            audioEditModel = this.GetModel<AudioEditModel>();
            if (audioEditModel.EditAudioClip != null)
            {
                return audioEditModel.ThisTime;
            }
            return 0;
        }
    }
}
