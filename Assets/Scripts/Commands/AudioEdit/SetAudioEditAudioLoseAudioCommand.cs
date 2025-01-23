using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Qf.Commands.AudioEdit
{
    public class SetAudioEditAudioLoseAudioCommand : AbstractCommand
    {
        AudioEditModel editModel;
        AudioClip audioClip;
        public SetAudioEditAudioLoseAudioCommand(AudioClip value)
        {
            audioClip = value;
        }
        protected override void OnExecute()
        {
            editModel = this.GetModel<AudioEditModel>();
            editModel.LoseAudioClip = audioClip;
        }
    }
}