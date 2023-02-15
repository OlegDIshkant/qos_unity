using CommonTools;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects
{
    public interface ICameraController
    {
        ICamera_ReadOnly CameraInfo { get; }
    }



    public class CameraController : AbstractController, ICameraController
    {
        private readonly IPlayFieldController _playField;


        private ICamera _camera;

        public ICamera_ReadOnly CameraInfo => _camera;
        public TimeContext TimeContext { set; private get; }


        private bool _cameraSetUp = false;

        public CameraController(
            Contexts contexts, 
            Func<ICamera> CameraFactoryMethod,
            IPlayFieldController playField) : 
            base(contexts)
        {
            _playField = playField;

            _camera = CameraFactoryMethod();
        }


        public override void Update()
        {
            if (!_cameraSetUp)
            {
                SetUpCam();
                _cameraSetUp = true;
            }
        }


        private void SetUpCam()
        {
            _camera.SetTransform(CalcTForm());
        }


        private Transform CalcTForm()
        {
            var playFieldCenter = _playField.PlayFieldInfo.FloorCircleCenter;

            var position = playFieldCenter + new Vector3(0, 3, -12);
            var lookDir = playFieldCenter - position;
            var rotation = GeometryUtils.InitialRotation(lookDir);

            return new Transform(position, rotation);
        }


    }
}