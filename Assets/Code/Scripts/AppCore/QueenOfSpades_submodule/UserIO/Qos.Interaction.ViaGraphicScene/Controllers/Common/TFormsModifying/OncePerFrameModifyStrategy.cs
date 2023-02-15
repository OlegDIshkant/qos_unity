using CommonTools;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// <see cref="IModifyStategy"/>, которая выполняет вычесления не чаще, чем раз в кадр (и кэширует результат до следующего кадра).
    /// </summary>
    public abstract class OncePerFrameModifyStrategy<Key> : IModifyStategy<Key>
    {
        private readonly TimeContext _timeContext;
        private readonly TimeContext.OncePerFrame _oncePerFrameExecuter;

        private IDictionary<Key, Transform> _cachedTForms;


        protected TimeContext TimeContext => _timeContext;


        public OncePerFrameModifyStrategy(TimeContext timeContext)
        {
            _timeContext = timeContext;
            _oncePerFrameExecuter = _timeContext.NewOncePerFrameExecutor();
        }


        public IDictionary<Key, Transform> Modify(DictionaryData<Key, Transform> transforms)
        {
            _oncePerFrameExecuter.Execute(
                OncePerFrameAction: () => _cachedTForms = ModifyOncePerFrame(transforms),
                IfSameFrameAction: () => UpdateCachedTForms(transforms));
            return _cachedTForms;
        }


        protected abstract IDictionary<Key, Transform> ModifyOncePerFrame(DictionaryData<Key, Transform> transforms);


        private void UpdateCachedTForms(DictionaryData<Key, Transform> transforms)
        {
            RemoveDeletedKeysFromCacheData(transforms);
            OnSameFrameUpdates(transforms);
        }


        private void RemoveDeletedKeysFromCacheData(DictionaryData<Key, Transform> transforms)
        {
            if (transforms.HasChanged)
            {
                foreach (var deletedKey in transforms.Removed)
                {
                    _cachedTForms.Remove(deletedKey);
                }
            }
        }


        protected virtual void OnSameFrameUpdates(DictionaryData<Key, Transform> transforms)
        {

        }

    }




    /// <summary>
    /// <see cref="IModifyStategy"/>, которая выполняет вычесления не чаще, чем раз в кадр (и кэширует результат до следующего кадра).
    /// </summary>
    public abstract class OncePerFrameModifyStrategy<Key, ExtraData> : OncePerFrameModifyStrategy<Key>, IModifyStategyWithExtraData<Key, ExtraData>
        where ExtraData : struct
    {
        public abstract DictionaryData<Key, ExtraData> DictionaryOutput { get; }

        protected OncePerFrameModifyStrategy(TimeContext timeContext) : base(timeContext)
        {
        }

    }
}