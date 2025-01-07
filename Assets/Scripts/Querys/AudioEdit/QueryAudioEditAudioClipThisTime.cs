using Qf.Models.AudioEdit;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Querys.AudioEdit
{
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
