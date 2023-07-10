using UnityEngine;

namespace WIFramework
{
    public abstract partial class CustomCameraController : ScriptableObject, IRewind
    {
        public CustomControllerOption commonVariable;

        public Vector3 nextPostion;
        protected Vector3 moveVector;
        protected Vector3 cameraPosition;

        protected Vector3 originTargetPos;
        protected Vector3 originTargetRot;

        protected float currentElevation;
        protected float currentOrbital;
        [HideInInspector] public float currentDistance;

        public void SetController()
        {
            SetControllerOption();
            SetCamOption();
        }


        public abstract void SetControllerOption();

        public abstract void SetCamOption();

        public virtual Vector3 Movement(UserInputData input)
        {
            nextPostion = commonVariable.camera.transform.position;
            if (!commonVariable.moveLimit)
            {
                Rotate(input);
                Move(input);
                Zoom(input);
            }
            return nextPostion;
        }

        // controller의 Camera의 위치값을 조작하는 Method
        protected abstract void Move(UserInputData input);


        /// <summary>
        /// controller의 target 의 위치값을 조작하는 Method
        /// target = controller의 Camera가 바라봐야하는 오브젝트
        /// </summary>
        /// <param name="moveVector">
        /// controllerObject의 위치값
        /// </param>
        protected abstract void MoveVectorControl(Vector3 moveVector);

        /// <summary>
        /// Move, Rotate Method를 통해 조작된 Camera, target의 위치 값을 토대로 
        /// 최종위치, 각도를 지정하는 Method
        /// </summary>
        /// <param name="limit">
        /// camera 와 target의 위치값이 Boundary 라는 콜라이더 범위내에 존재하는지에 대한 여부
        /// </param>
        public abstract void LastPositioning(bool limit);

        protected abstract void Zoom(UserInputData input);

        //controller의 target 의 회전값을 조작하는 메서드들을 통합한 메서드
        //회전과 관련된 Method들은 Rotate 안에 넣어야 합니다.
        protected abstract void Rotate(UserInputData input);

        //controller의 Camera의 고도 값을 조작하는 Method
        protected abstract void ElevationControl(UserInputData input);

        //controller의 Camera의 궤도 값을 조작하는 Method
        protected abstract void OrbitalControl(UserInputData input);

        // controller의 target, Camera 의 위치 값과 회전 값을 초기화 하는 Method
        public abstract void Rewind();
    }
}