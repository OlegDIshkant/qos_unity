using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Utils;
using static CommonTools.Math.NDCPointToPlaneSolver;


namespace GameScene
{
    public class Camera : ICamera
    {
        public bool TransformChanged { get; private set; }
        public bool ParamsChanged { get; private set; }


        private UnityEngine.Camera _camera;


        public Camera()
        {
            CreateCameraIfNeeded();
        }


        private void CreateCameraIfNeeded()
        {
            if (_camera == null)
            {
                _camera = UnityEngine.Camera.main;
                UpdateParams(asInitial: true);
            }
        }


        public CameraParams Params { get; private set; }


        public CommonTools.Math.Transform GetTransform() => _camera.transform.FromUnity();


        public void SetTransform(Transform tform)
        {
            _camera.transform.ToUnity(tform);
            UpdateParams();
            TransformChanged = true;
        }


        private void UpdateParams(bool asInitial = false)
        {
            Params = new CameraParams()
            {
                nearPlaneDist = UnityEngine.Camera.main.nearClipPlane,
                inverseProjMatrix = UnityEngine.Camera.main.projectionMatrix.inverse.FromUnity(),
                inverseViewMatrix = UnityEngine.Camera.main.cameraToWorldMatrix.FromUnity()
            };

            if (!asInitial)
            {
                ParamsChanged = true;
            }
        }
    }
}
