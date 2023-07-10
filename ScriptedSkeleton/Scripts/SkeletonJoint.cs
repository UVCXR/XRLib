using System;
using System.Collections.Generic;
using UnityEngine;


namespace WIFramework
{

    [Serializable]
    public partial class SkeletonJoint : MonoBehaviour
    {
        [Serializable]
        public struct VectorLocker
        {
            public bool x;
            public bool y;
            public bool z;

            [Range(0, 360)]
            public float xRange;
            [Range(0, 360)]
            public float yRange;
            [Range(0, 360)]
            public float zRange;

            public void Locking(ref Vector3 value)
            {
                if (x)
                    value.x = 0;
                if (y)
                    value.y = 0;
                if (z)
                    value.z = 0;

                if (value.x > 360)
                {
                    value.x %= 360;
                }
            }
        }
        Mesh _jointMesh;
        public Mesh mesh
        {
            get
            {
                if (_jointMesh == null)
                    _jointMesh = GetComponent<MeshFilter>().mesh;
                return _jointMesh;
            }
        }

        Renderer _renderer;
        public new Renderer renderer
        {
            get
            {
                if (_renderer == null)
                    _renderer = GetComponent<Renderer>();
                return _renderer;
            }
        }

        Transform bone;
        List<SkeletonJoint> _childJoints = new List<SkeletonJoint>();
        [SerializeField] VectorLocker rotateLock;
        [SerializeField] VectorLocker positionLock;

        public List<SkeletonJoint> childJoints => _childJoints;

        public VectorLocker GetRotateLocker()
        {
            return rotateLock;
        }
        public VectorLocker GetPositionLocker()
        {
            return positionLock;
        }

        public void SetRotate(Vector3 value)
        {
            rotateLock.Locking(ref value);
            bone.localEulerAngles = value;
        }

        public void SetPosition(Vector3 value)
        {
            positionLock.Locking(ref value);
            bone.localPosition = value;
        }

        void SetChildJoint()
        {
            var c = bone.transform.childCount;
            for (int i = 0; i < c; ++i)
            {
                var child = bone.transform.GetChild(i);
                if (child.TryGetComponent<RectTransform>(out var isUI))
                {
                    //Debug.Log(child.name);
                    continue;
                }
                var joint = child.GetOrAddComponent<SkeletonJoint>();
                joint.SetBone(child);
                _childJoints.Add(joint);
            }
        }

        public void SetBone(Transform child)
        {
            bone = child;
            SetChildJoint();
        }
    }
}