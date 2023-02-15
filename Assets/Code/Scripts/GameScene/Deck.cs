using CommonTools;
using Qos.Interaction.ViaGraphicScene;
using UnityEditor;
using UnityEngine;
using Utils;


namespace GameScene
{
    internal class Deck : DisposableClass, IDeck
    {        

        private GameObject _go;

        private bool p_transformChanged;

        public bool TransformChanged 
        {
            get { ThrowErrorIfDisposed(); return p_transformChanged; }
        }


        public Deck()
        {
            var prototype = Resources.Load("Prefabs/GameSceneElements/Player", typeof(UnityEngine.Object));
            _go = (GameObject)Object.Instantiate(prototype, Vector3.zero, Quaternion.identity);
        }

        public CommonTools.Math.Transform GetTransform() { ThrowErrorIfDisposed(); return _go.transform.FromUnity(); }


        public void SetTransform(CommonTools.Math.Transform tform)
        {
            ThrowErrorIfDisposed();

            _go.transform.ToUnity(tform);
            p_transformChanged = true;
        }


        public void PrepToRememberNewChanges()
        {
            ThrowErrorIfDisposed();
            p_transformChanged = false;
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();
            GameObject.Destroy(_go);
            _go = null;
        }
    }
}