using System;
using UnityEngine;
namespace WIFramework
{
    [CreateAssetMenu(fileName = "OrthographicController", menuName = "GenericController/Orthographic")]
    public partial class Orthographic : CustomCameraController
    {
        //TODO : 이름 오류
        [Serializable]
        public partial class OrthographicOption
        {
            public Transform target;
            public float maxDistance;
            public float minDistance;
            public float maxElevation;
            public float minElevation;
            public float elevationSensitivity;
            public float orbitalSensitivity;
            public float moveClamper;

            public float originDistance;
            public float originElevation;
            public float originOrbital;
        }

        public OrthographicOption option;

        #region SetControllerOption
        public override void SetControllerOption()
        {
            commonVariable.camera = FindObjectOfType<Camera>();
            //TODO : 개선 필요
            if (!GameObject.Find("CameraPoint_Perspective"))
            {
                option.target = new GameObject("CameraPoint_Perspective").transform;
            }
            option.target = GameObject.Find("CameraPoint_Perspective").transform;

            originTargetPos = option.target.position;
            originTargetRot = option.target.eulerAngles;
            currentDistance = option.originDistance;
            currentElevation = option.originElevation;
            currentOrbital = option.originOrbital;
        }
        public override void SetCamOption()
        {
            commonVariable.camera.orthographic = true;
            commonVariable.camera.orthographicSize = commonVariable.fieldOfView;
        }
        #endregion

        public override Vector3 Movement(UserInputData input)
        {
            nextPostion = option.target.position;
            if (!commonVariable.moveLimit)
            {
                Rotate(input);
                Move(input);
                Zoom(input);
            }
            return nextPostion;
        }

        #region Move
        protected override void Move(UserInputData input)
        {
            if (input.leftClick)
            {
                moveVector = Vector3.zero;

                moveVector = commonVariable.camera.transform.TransformDirection(input.mouseX, input.mouseY, 0);
                MoveVectorControl(moveVector);
            }
        }

        protected override void MoveVectorControl(Vector3 moveVector)
        {
            var t = currentDistance / option.maxDistance;
            option.moveClamper = Mathf.Min(t, 1f);
            moveVector *= option.moveClamper * commonVariable.moveSpeed;
            nextPostion = option.target.position - moveVector;
        }
        #endregion

        #region Zoom
        protected override void Zoom(UserInputData input)
        {
            if (input.mouseWheel < -0.01f || input.mouseWheel > 0.01f)
            {
                currentDistance -= input.mouseWheel * commonVariable.zoomSpeed;
                currentDistance = Mathf.Clamp(currentDistance, option.minDistance, option.maxDistance);
                commonVariable.camera.orthographicSize = currentDistance;
            }
        }
        #endregion

        #region Rotate
        protected override void Rotate(UserInputData input)
        {
            OrbitalControl(input);
        }

        protected override void OrbitalControl(UserInputData input)
        {
            if (input.rightClick)
            {
                if (input.mouseX > 0 || input.mouseX < 0)
                {
                    currentOrbital += input.mouseX * option.orbitalSensitivity;
                    if (currentOrbital > 360)
                        currentOrbital -= 360;
                    if (currentOrbital < 0)
                        currentOrbital += 360;
                }
            }
        }

        protected override void ElevationControl(UserInputData input)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region LastPositioning
        public override void LastPositioning(bool limit)
        {
            if (!limit)
                return;

            option.target.position = nextPostion;
            cameraPosition = nextPostion + Quaternion.Euler(currentElevation, currentOrbital, 0f) * new Vector3(0, 0, -currentDistance);
            commonVariable.camera.transform.position = cameraPosition;
            commonVariable.camera.transform.LookAt(option.target);
        }
        #endregion

        #region Reset
        public override void Rewind()
        {
            option.target.position = originTargetPos;
            option.target.eulerAngles = originTargetRot;
            currentDistance = option.originDistance;
            currentElevation = option.originElevation;
            currentOrbital = option.originOrbital;

            cameraPosition = option.target.position + Quaternion.Euler(currentElevation, currentOrbital, 0f) * new Vector3(0, 0, -currentDistance);
            commonVariable.camera.transform.position = cameraPosition;
            commonVariable.camera.transform.LookAt(option.target);
        }
        #endregion

    }
}