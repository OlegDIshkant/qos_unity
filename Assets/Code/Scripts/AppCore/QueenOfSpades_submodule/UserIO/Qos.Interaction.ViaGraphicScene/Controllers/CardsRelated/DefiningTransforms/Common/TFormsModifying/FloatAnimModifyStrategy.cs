using CommonTools;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Модификация, меняющая расположение объектов в пространстве так, чтобы создать визуальный эффект будто они слегка парят в воздухе.
    /// </summary>
    public class FloatAnimModifyStrategy<Key> : OncePerFrameModifyStrategy<Key>
    {
        private readonly float T_SPEED = 0.001f;
        private readonly float PHASE_DIFFERENCE = 0.05f;
        private readonly float FLOAT_AMPL = 0.01f;

        private float _t = 0;


        public FloatAnimModifyStrategy(TimeContext timeContext)
            : base(timeContext)
        {
        }


        protected override IDictionary<Key, Transform> ModifyOncePerFrame(DictionaryData<Key, Transform> transforms)
        {
            IncrParams();

            int index = 0;
            Dictionary<Key, Transform> result = new Dictionary<Key, Transform>();
            foreach (var key in transforms.Items.Keys)
            {
                result[key] = Modify(transforms.Items[key], index++);
            }
            return result;
        }


        private void IncrParams()
        {
            _t += T_SPEED * TimeContext.TimeSincePrevUpdate;
            while (_t > 1f)
            {
                _t %= 1f;
            }
        }


        private Transform Modify(Transform tForm, int index)
        {
            tForm.Position = tForm.Position + PositionJitter(index);
            return tForm;
        }


        private Vector3 PositionJitter(int index)
        {
            var sinArg = (_t + index * PHASE_DIFFERENCE) * Math.PI * 2;
            return Vector3.Multiply((float)Math.Sin(sinArg) * FLOAT_AMPL, Vector3.UnitY);
        }

    }
}