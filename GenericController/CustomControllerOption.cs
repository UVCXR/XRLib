using System;
using UnityEngine;

namespace WIFramework
{
    [Serializable]
    public partial class CustomControllerOption
    {
        public Camera camera;
        public float moveSpeed;
        public float zoomSpeed;
        public float fieldOfView;

        public bool moveLimit;
    }
}