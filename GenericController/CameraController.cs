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
        /// �� ������ ���� controller�� camera�� target�� ��ġ���� �����ϴ� Method
        /// 1. ����ڷκ��� �Է��� ����
        /// 2. �Է¹��� ���� �������� controller �� camera�� target�� ��ġ���� ����
        /// 2-1. ���۵� ���� ���� ����
        /// 3. ���ϰ� ���� ���� �ݶ��̴� ���� �����ϴ����� ���� ���θ� ����
        /// 3-1. ���� ������ ���� ��ġ ����
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