using QFramework;
using UnityEngine;
namespace Qf.Managers
{
    public abstract class ManagerBase : MonoBehaviour,IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameBody.Interface;
        }

        public abstract void Init();
    }
}
