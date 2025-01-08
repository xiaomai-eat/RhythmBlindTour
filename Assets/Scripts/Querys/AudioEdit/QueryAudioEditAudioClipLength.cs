using Qf.Models.AudioEdit;
using QFramework;
namespace Qf.Querys.AudioEdit
{
    /// <summary>
    /// 获取当前音频编辑器音频长度
    /// </summary>
    public class QueryAudioEditAudioClipLength : AbstractQuery<float>
    {
        AudioEditModel audioEditModel;
        protected override float OnDo()
        {
            audioEditModel = this.GetModel<AudioEditModel>();
            if (audioEditModel.EditAudioClip != null)
            {
                return audioEditModel.EditAudioClip.length;
            }
            return 0;
        }
    }
}

