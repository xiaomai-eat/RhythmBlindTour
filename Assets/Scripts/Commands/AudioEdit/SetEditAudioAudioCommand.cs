using Qf.Models.AudioEdit;
using QFramework;
using UnityEngine;
namespace Qf.Commands.AudioEdit
{
    /// <summary>
    ///…Ë÷√±‡º≠∆˜“Ù∆µ√¸¡Ó
    /// </summary>
    public class SetEditAudioAudioCommand : AbstractCommand
    {
        AudioClip clip;
        /// <summary>
        /// …Ë÷√±‡º≠∆˜“Ù∆µ√¸¡Ó
        /// </summary>
        /// <param name="audioClip">“Ù∆µ</param>
        public SetEditAudioAudioCommand(AudioClip audioClip)
        {
            clip = audioClip;
        }
        protected override void OnExecute()
        {
            this.GetModel<AudioEditModel>().EditAudioClip = clip;
        }
    }

}
