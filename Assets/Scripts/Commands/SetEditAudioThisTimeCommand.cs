using Qf.Models.AudioEdit;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Commands.AudioEdit
{
    public class SetEditAudioThisTimeCommand : AbstractCommand
    {
        float value;
        public SetEditAudioThisTimeCommand(float time)
        {
            value = time;
        }
        protected override void OnExecute()
        {
            AudioEditModel audioEditModel = this.GetModel<AudioEditModel>();
            if (audioEditModel != null)
            {
                audioEditModel.ThisTime = value;
                if(audioEditModel.ThisTime > audioEditModel.EditAudioClip.length)
                {
                    audioEditModel.ThisTime = audioEditModel.EditAudioClip.length;
                }
                else if(audioEditModel.ThisTime<=0)
                {
                    audioEditModel.ThisTime = 0;
                }
            }
                
        }
    }
}
