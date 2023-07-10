using UnityEngine;
namespace WIFramework
{
    public partial class MouseTracker : MonoBehaviour, ISingle
    {
        Vector3 mousePosition;

        void FixedUpdate()
        {
            mousePosition = Input.mousePosition;
            WIManager.trackers.RemoveWhere(t => t == null);
            foreach (var t in WIManager.trackers)
            {
                t.Tracking(mousePosition);
            }
        }
    }
}