using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Models.AudioEdit;
using QFramework;

namespace Qf.Commands.AudioEdit
{

    /// <summary>
    /// 设置BPM值
    /// </summary>
    public class SetAudioEditAudioBPMCommand : AbstractCommand
    {
        AudioEditModel audioEditModel;
        int value;
        public SetAudioEditAudioBPMCommand(int value)
        {
            this.value = value;
        }
        protected override void OnExecute()
        {
            audioEditModel = this.GetModel<AudioEditModel>();
            if (audioEditModel.EditAudioClip == null || audioEditModel.Mode.Equals(SystemModeData.PlayMode) || value<0) return;
            audioEditModel.BPM = value;
            this.SendEvent(new BPMChangeValue() { BPM = value });
        }
    }
}
