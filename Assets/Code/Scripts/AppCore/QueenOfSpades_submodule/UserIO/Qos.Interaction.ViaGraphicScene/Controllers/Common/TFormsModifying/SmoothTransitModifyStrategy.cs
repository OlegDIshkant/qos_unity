using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// Следит за расположениями объектов в простанстве. Если эти рассположения "резко" поменяются, будет модифицировать их так, чтобы визуально "сгладить" эти изменения.
    /// </summary>
    /// <remarks>
    /// Создает плавные анимации перемещений.
    /// </remarks>
    public class SmoothTransitModifyStrategy<Key> : OncePerFrameModifyStrategy<Key>
    {
        private readonly float CHANGE_SPEED;

        private Dictionary<Key, Transform> _knownTransforms = new Dictionary<Key, Transform>();


        public SmoothTransitModifyStrategy(TimeContext timeContext, float changeSpeed) : base(timeContext)
        {
            CHANGE_SPEED = changeSpeed;
        }


        protected override IDictionary<Key, Transform> ModifyOncePerFrame(DictionaryData<Key, Transform> transforms)
        {
            ForgetRemovedItems(transforms);
            return UpdateKnownTForms(changeRate: CHANGE_SPEED * TimeContext.TimeSincePrevUpdate, transforms);
        }


        protected override void OnSameFrameUpdates(DictionaryData<Key, Transform> transforms)
        {
            base.OnSameFrameUpdates(transforms);
            ForgetRemovedItems(transforms);
        }



        private void ForgetRemovedItems(DictionaryData<Key, Transform> transforms)
        {
            if (transforms.HasChanged)
            {
                foreach (var removed in transforms.Removed)
                {
                    _knownTransforms.Remove(removed);
                }
            }
        }


        private IDictionary<Key, Transform> UpdateKnownTForms(float changeRate, DictionaryData<Key, Transform> transforms)
        {
            foreach (var item in transforms.Items)
            {
                if (_knownTransforms.TryGetValue(item.Key, out var smoothTForm))
                {
                    smoothTForm.Towards(item.Value, changeRate, changeRate, changeRate);
                    _knownTransforms[item.Key] = smoothTForm;
                }
                else
                {
                    _knownTransforms[item.Key] = item.Value;
                }
            }

            return _knownTransforms;
        }

    }
}