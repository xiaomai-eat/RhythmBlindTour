using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Models.AudioEdit;
using QFramework;

namespace Qf.Commands.AudioEdit
{
    public class SetAudioEditModeCommand : AbstractCommand
    {
        AudioEditModel AudioEditModel;
        SystemModeData CurrentModeData;
        SystemModeData LastModeData;
        public SetAudioEditModeCommand(SystemModeData systemModeData)
        {
            this.CurrentModeData = systemModeData;
        }
        protected override void OnExecute()
        {
            AudioEditModel = this.GetModel<AudioEditModel>();
            LastModeData = AudioEditModel.Mode;
            AudioEditModel.Mode = CurrentModeData;
            if (LastModeData == CurrentModeData) return;
            switch (LastModeData)
            {
                case SystemModeData.EditMode:
                    this.SendEvent<ExitEditMode>();
                    break;
                case SystemModeData.PlayMode:
                    this.SendEvent<ExitPlayMode>();
                    break;
                case SystemModeData.RecordingMode:
                    this.SendEvent<ExitRecordingMode>();
                    break;
            }
            switch (CurrentModeData)
            {
                case SystemModeData.EditMode:
                    this.SendEvent<OnEditMode>();
                    break;
                case SystemModeData.PlayMode:
                    this.SendEvent<OnPlayMode>();
                    break;
                case SystemModeData.RecordingMode:
                    this.SendEvent<OnRecordingMode>();
                    break;
            }
        }
    }
}
