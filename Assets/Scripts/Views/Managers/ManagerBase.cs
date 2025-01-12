using QFramework;
using UnityEngine;
namespace Qf.Managers
{
    public abstract class ManagerBase : MonoSingleton<ManagerBase>,IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameBody.Interface;
        }

        public abstract void Init();
    }
}
