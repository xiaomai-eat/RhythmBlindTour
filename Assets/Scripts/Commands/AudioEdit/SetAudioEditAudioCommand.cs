using Qf.Events;
using Qf.Models.AudioEdit;
using QFramework;
using UnityEngine;
namespace Qf.Commands.AudioEdit
{
    /// <summary>
    ///…Ë÷√±‡º≠∆˜“Ù∆µ√¸¡Ó
    /// </summary>
    public class SetAudioEditAudioCommand : AbstractCommand
    {
        AudioClip _Clip;
        AudioClip _Last;
        AudioClip _Current;
        AudioEditModel audioEditModel;
        /// <summary>
        /// …Ë÷√±‡º≠∆˜“Ù∆µ√¸¡Ó
        /// </summary>
        /// <param name="audioClip">“Ù∆µ</param>
        public SetAudioEditAudioCommand(AudioClip audioClip)
        {
            _Clip = audioClip;
        }
        protected override void OnExecute()
        {
            audioEditModel = this.GetModel<AudioEditModel>();
            _Last = audioEditModel.EditAudioClip;
            _Current = _Clip;
            if (_Current == null) return;
            else if (_Current.Equals(_Last)) return;
            audioEditModel.EditAudioClip = _Clip;
            this.SendCommand(new SetAudioEditThisTimeCommand(0));
            this.SendCommand(new RemoveAudioEditTimeLineDataCommand(-1));
            this.SendEvent(new MainAudioChangeValue() { Name = _Clip.name,Length = _Clip.length }) ;
        }
    }

}
