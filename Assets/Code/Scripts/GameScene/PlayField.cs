using CommonTools;
using Qos.Interaction.ViaGraphicScene;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace GameScene
{
    internal class PlayField : DisposableClass, IPlayField
    {

        private GameObject _go;

        public System.Numerics.Vector3 FloorCircleCenter { get; private set; } = new System.Numerics.Vector3(0, 15, 0);
        public float FloorCircleRadius { get; private set; } = 10f;
        public System.Numerics.Vector3 CardHeapCenter => new System.Numerics.Vector3(0, 1.36f, 0);
        public float CardHeapRadius => 3;


        public PlayField()
        {
            var prototype = Resources.Load("Prefabs/GameSceneElements/Arena", typeof(UnityEngine.Object));
            _go = (GameObject)UnityEngine.Object.Instantiate(prototype, Vector3.zero, Quaternion.identity);
        }


        public void SetFlorCircleCenter(System.Numerics.Vector3 point)
        {
            ThrowErrorIfDisposed();
            FloorCircleCenter = point;
        }


        public void SetFlorCircleRadius(float radius)
        {
            ThrowErrorIfDisposed();
            FloorCircleRadius = radius;
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();

            GameObject.Destroy(_go);
            _go = null;
        }
    }
}