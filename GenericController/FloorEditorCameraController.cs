using UnityEngine;

namespace WIFramework
{
    [CreateAssetMenu(fileName = "FloorEditorCameraController", menuName = "GenericController/FloorEditorCameraController")]

    public partial class FloorEditorCameraController : CustomCameraController
    {
        public override void LastPositioning(bool limit)
        {
            throw new System.NotImplementedException();
        }

        public override void Rewind()
        {
            throw new System.NotImplementedException();
        }

        public override void SetCamOption()
        {
            throw new System.NotImplementedException();
        }

        public override void SetControllerOption()
        {
            throw new System.NotImplementedException();
        }

        protected override void ElevationControl(UserInputData input)
        {
            throw new System.NotImplementedException();
        }

        protected override void Move(UserInputData input)
        {
            throw new System.NotImplementedException();
        }

        protected override void MoveVectorControl(Vector3 moveVector)
        {
            throw new System.NotImplementedException();
        }

        protected override void OrbitalControl(UserInputData input)
        {
            throw new System.NotImplementedException();
        }

        protected override void Rotate(UserInputData input)
        {
            throw new System.NotImplementedException();
        }

        protected override void Zoom(UserInputData input)
        {
            throw new System.NotImplementedException();
        }
    }
}
