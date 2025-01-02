using QFramework;
using UnityEngine;
namespace Qf.Managers
{
    public abstract class ManagerBase : MonoSingleton<ManagerBase>
    {
        public abstract void Init();
    }
}
