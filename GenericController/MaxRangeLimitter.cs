using UnityEngine;

namespace WIFramework
{
    public partial class MaxRangeLimitter
    {
        public Collider maxRange;

        public void SetRange(Collider maxrRangeCollider)
        {
            maxRange = maxrRangeCollider;
        }

        public bool MoveRangeLimit(Vector3 pos)
        {
            if (maxRange.bounds.Contains(pos))
            {
                return true;
            }
            return false;
        }
    }
}
