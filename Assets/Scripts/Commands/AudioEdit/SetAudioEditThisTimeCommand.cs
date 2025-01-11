using Qf.Events;
using Qf.Models.AudioEdit;
using QFramework;

namespace Qf.Commands.AudioEdit
{
    /// <summary>
    /// 设置编辑器当前播放时间命令
    /// </summary>
    public class SetAudioEditThisTimeCommand : AbstractCommand
    {
        float value;
        /// <summary>
        /// 设置编辑器当前播放时间命令
        /// </summary>
        /// <param name="time">时间</param>
        public SetAudioEditThisTimeCommand(float time)
        {
            value = time;
        }
        protected override void OnExecute()
        {
            AudioEditModel audioEditModel = this.GetModel<AudioEditModel>();
            if (audioEditModel != null)
            {
                if (audioEditModel.EditAudioClip == null) return;
                
                if(value > audioEditModel.EditAudioClip.length)
                {
                    audioEditModel.ThisTime = audioEditModel.EditAudioClip.length;
                }
                else if(value<0)
                {
                    audioEditModel.ThisTime = 0;
                }
                else
                {
                    audioEditModel.ThisTime = value;
                }
            }
            this.SendEvent<OnUpdateThisTime>();
        }
    }
}
