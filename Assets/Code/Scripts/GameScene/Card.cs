using CommonTools;
using Qos.Domain;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using UnityEditor;
using UnityEngine;
using Utils;
using static Qos.Domain.StandartPlayingCardModel;
using Object = UnityEngine.Object;


namespace GameScene
{
    internal class StandartPlayingCard : DisposableClass, ICard
    {
        private readonly string PATH_TO_SUIT_DECAL_GO = "card/decal_suit";
        private readonly string PATH_TO_VALUE_DECAL_GO = "card/decal_value";

        private GameObject _go;

        public bool TransformChanged { get; private set; } // Без проверки на Disposed, ради производительности

        public StandartPlayingCard(StandartPlayingCardModel cardModel, IStandartPlayingCardTexturesProvider textureProvider)
        {
            var prototype = Resources.Load("Prefabs/GameSceneElements/StandartPlayingCard", typeof(Object));
            _go = (GameObject)Object.Instantiate(prototype, Vector3.zero, Quaternion.identity);
            SetAppearence(cardModel, textureProvider);
        }


        private void SetAppearence(StandartPlayingCardModel cardModel, IStandartPlayingCardTexturesProvider textureProvider)
        {
            var valDecal = textureProvider.GetValueDecal(cardModel.Value);
            var suitDecal = textureProvider.GetSuitDecal(cardModel.Suit);
            SetTextureToChildMaterial(_go.transform, PATH_TO_SUIT_DECAL_GO, suitDecal);
            SetTextureToChildMaterial(_go.transform, PATH_TO_VALUE_DECAL_GO, valDecal);
            
            if (IsRedSuit(cardModel.Suit))
            {
                ChangeTintToRed(_go.transform, PATH_TO_VALUE_DECAL_GO);
            }
        }


        private bool IsRedSuit(Suits suit) =>
            suit == Suits.HEARTS || suit == Suits.DIAMONDS;


        private void SetTextureToChildMaterial(Transform parentGo, string pathToChild, Texture texture)
        {
            DoActionToChild(parentGo, pathToChild,
                (child) =>
                {
                    var meshRenderer = child.GetComponent<MeshRenderer>();
                    meshRenderer.material.mainTexture = texture;
                });
        }


        private void ChangeTintToRed(Transform parentGo, string pathToChild)
        {
            DoActionToChild(parentGo, pathToChild,
                (child) =>
                {
                    var meshRenderer = child.GetComponent<MeshRenderer>();
                    meshRenderer.material.color = Color.red;
                });
        }


        private void DoActionToChild(Transform parentGo, string pathToChild, Action<Transform> actionOnChild)
        {
            var subpath = pathToChild.Split('/')[0];

            Transform child = null;
            for (int i = 0; i < parentGo.transform.childCount; i++)
            {
                child = parentGo.transform.GetChild(i);
                if (child.name == subpath)
                {
                    break;
                }
            }
            if (child == null)
            {
                throw new System.Exception();
            }


            if (subpath.Length == pathToChild.Length)
            {
                actionOnChild(child);
            }
            else
            {
                var restOfPath = pathToChild.Substring(subpath.Length + 1);
                DoActionToChild(child, restOfPath, actionOnChild);
            }
        }


        public CommonTools.Math.Transform GetTransform() => _go.transform.FromUnity();// Без проверки на Disposed, ради производительности


        public void SetTransform(CommonTools.Math.Transform tform)
        {
            // Без проверки на Disposed, ради производительности
            _go.transform.ToUnity(tform);
            TransformChanged = true;
        }


        public void PrepToRememberNewChanges()
        {
            // Без проверки на Disposed, ради производительности
            TransformChanged = false;
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();

            GameObject.Destroy(_go);
            _go = null;
        }
    }
}
