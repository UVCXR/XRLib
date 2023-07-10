using UnityEngine;

namespace WIFramework
{
    public partial class CameraController : MonoBehaviour, ISingle
    {
        public CustomCameraController[] controllers;
        public CustomCameraController currentController;
        SDictionary<System.Type, CustomCameraController> table_Controller = new SDictionary<System.Type, CustomCameraController>();
        private MaxRangeLimitter maxRangeLimitter = new MaxRangeLimitter();
        private UserInputData inputData = new UserInputData();


        public void Awake()
        {
            table_Controller.Clear();

            foreach (var c in controllers)
            {
                table_Controller.Add(c.GetType(), c);
                c.SetController();
            }
            currentController = table_Controller[typeof(Orthographic)];
            maxRangeLimitter.SetRange(FindObjectOfType<Tag_MovementRange>().GetComponent<Collider>());
        }

        public void SetMoveRange(Collider range)
        {
            maxRangeLimitter.SetRange(range);
        }

        public void SetController(System.Type type)
        {
            currentController = table_Controller[type];
            currentController.SetCamOption();
            currentController.Movement(inputData);
        }

        /// <summary>
        /// 매 프레임 마다 controller의 camera와 target의 위치값을 지정하는 Method
        /// 1. 사용자로부터 입력을 받음
        /// 2. 입력받은 값을 바탕으로 controller 의 camera와 target의 위치값을 조작
        /// 2-1. 조작된 값을 리턴 받음
        /// 3. 리턴값 받은 값이 콜라이더 내에 존재하는지에 대한 여부를 검증
        /// 3-1. 검증 성공시 최종 위치 지정
        /// </summary>
        void Update()
        {
            inputData.GetInput();
            var newPosition = currentController.Movement(inputData);
            bool limitCheck = maxRangeLimitter.MoveRangeLimit(newPosition);
            currentController.LastPositioning(limitCheck);
        }

    }
}