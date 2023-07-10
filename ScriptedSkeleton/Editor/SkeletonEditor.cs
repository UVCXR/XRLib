using UnityEditor;
using UnityEngine;

namespace WIFramework
{
    [CustomEditor(typeof(Skeleton))]
    public partial class SkeletonEditor : Editor
    {
        Skeleton model;
        public void OnEnable()
        {
            model = target as Skeleton;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Set Skeleton"))
                model.SetSkeleton();
        }
    }
}