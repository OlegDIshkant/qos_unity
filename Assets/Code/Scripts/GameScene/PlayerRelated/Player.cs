using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using UnityEditor;
using UnityEngine;
using Utils;


namespace GameScene
{
    internal class Player : DisposableClass, IPlayer
    {
        private PlayerAppearenceSettings _appearence;
        private string _name;

        private bool _transformChanged;
        public bool TransformChanged
        {
            get { ThrowErrorIfDisposed();  return _transformChanged; }
            private set { _transformChanged = value; }
        }

        private GameObject GO => _appearence?.gameObject;


        public Player(PlayerModel model, PlayerAppearence appearence)
        {
            _appearence = SpawnAppearence();
            SetUp(model, appearence);
        }


        private PlayerAppearenceSettings SpawnAppearence()
        {
            var prototype = Resources.Load("Prefabs/GameSceneElements/Player", typeof(UnityEngine.Object));
            var go = (GameObject)UnityEngine.Object.Instantiate(prototype, Vector3.zero, Quaternion.identity);
            return go.GetComponent<PlayerAppearenceSettings>();
        }


        private void SetUp(PlayerModel model, PlayerAppearence appearence)
        {
            _appearence.SetUp(appearence);
            TryRenameGameObject(_name);

        }


        public string GetName()
        {
            ThrowErrorIfDisposed();
            return _name;
        }


        private bool TryRenameGameObject(string name)
        {
            if (GO == null)
            {
                return false;
            }

            GO.name = $"Player with name: '{name}'";
            return true;
        }


        public void SetTransform(CommonTools.Math.Transform tForm)
        {
            ThrowErrorIfDisposed();
            GO.transform.ToUnity(tForm);
            TransformChanged = true;
        }


        public CommonTools.Math.Transform GetTransform()
        {
            ThrowErrorIfDisposed();
            return GO.transform.FromUnity();
        }


        public void ShowCreating(NormValue normTime)
        {
            ThrowErrorIfDisposed();
        }


        protected override void OnDisposed()
        {
            GameObject.Destroy(GO);
            _appearence = null;
        }
    }
}
