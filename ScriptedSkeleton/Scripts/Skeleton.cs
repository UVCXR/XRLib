using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace WIFramework
{

    public partial class Skeleton : MonoBehaviour
    {
        public SkeletonJoint Bone;
        public SkeletonJoint[] bones;
        public SDictionary<int, List<SkeletonJoint>> depthBone = new SDictionary<int, List<SkeletonJoint>>();
        public new Collider collider;

        private void Awake()
        {
            SetSkeleton();
        }
        void BoneActionTest()
        {
            var randomBone = depthBone[UnityEngine.Random.Range(0, depthBone.Count)];
            var randomRotation = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));

            foreach (var j in randomBone)
            {
                j.SetRotate(randomRotation);
            }
        }

        #region public interface
        public void SetSkeleton()
        {
            Bone = transform.GetOrAddComponent<SkeletonJoint>();
            SetDepthBone();
            bones = GetComponentsInChildren<SkeletonJoint>();
        }
        public SkeletonJoint GetRootBone()
        {
            return Bone;
        }
        public bool GetBoneFromDepth(int depth, out List<SkeletonJoint> result)
        {
            if (depthBone.TryGetValue(depth, out result))
                return true;
            else
            {
                result = new List<SkeletonJoint>();
                return false;
            }
        }

        #endregion

        #region private
        void SetDepthBone()
        {
            depthBone.Clear();
            depthBone.Add(0, new List<SkeletonJoint> { Bone });
            Bone.SetBone(transform);
            foreach (var j in Bone.childJoints)
                GetDepthBone(j, 1);
        }

        void GetDepthBone(SkeletonJoint joint, int depth)
        {
            if (!depthBone.TryGetValue(depth, out var list))
                depthBone.Add(depth, new List<SkeletonJoint>());

            depthBone[depth].Add(joint);
            if (joint.childJoints == null)
            {
                return;
            }
            depth++;
            foreach (var j in joint.childJoints)
            {
                GetDepthBone(j, depth);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("A");
        }
        #endregion
    }

}