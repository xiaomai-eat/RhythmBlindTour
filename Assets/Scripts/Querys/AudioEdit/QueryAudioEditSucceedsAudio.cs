using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Querys.AudioEdit
{
    public class QueryAudioEditSucceedsAudio : AbstractQuery<AudioClip>
    {
        AudioEditModel editModel;
        AudioClip audioClip;
        TheTypeOfOperation theTypeOfOperation;
        public QueryAudioEditSucceedsAudio(TheTypeOfOperation theTypeOfOperation)
        {
            this.theTypeOfOperation = theTypeOfOperation;
        }
        protected override AudioClip OnDo()
        {
            editModel = this.GetModel<AudioEditModel>();
            switch (theTypeOfOperation)
            {
                case TheTypeOfOperation.SwipeUp:
                    audioClip = editModel.UpSucceedAudioClip;
                    break;
                case TheTypeOfOperation.SwipeDown:
                    audioClip = editModel.DownSucceedAudioClip;
                    break;
                case TheTypeOfOperation.SwipeRight:
                    audioClip = editModel.RigthSucceedAudioClip;
                    break;
                case TheTypeOfOperation.SwipeLeft:
                    audioClip = editModel.LeftSucceedAudioClip;
                    break;
                case TheTypeOfOperation.Click:
                    audioClip = editModel.ClickSucceedAudioClip;
                    break;
                default:
                    audioClip = null;
                    break;
            };
            return audioClip;
        }
    }
}