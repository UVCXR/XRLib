using System;
using WIFramework;


[Serializable]
public partial class MonoBehaviour : UnityEngine.MonoBehaviour
{
    public virtual void Initialize()
    {
    }
    public virtual void AfterInitialize()
    {
    }

    public MonoBehaviour()
    {
        if (this.GetType() != typeof(TrashBehaviour))
            Hooker.RegistReady(this);
    }

    private void OnDestroy()
    {
        if (this.GetType() != typeof(TrashBehaviour))
            WIManager.Unregist(this);
    }
}