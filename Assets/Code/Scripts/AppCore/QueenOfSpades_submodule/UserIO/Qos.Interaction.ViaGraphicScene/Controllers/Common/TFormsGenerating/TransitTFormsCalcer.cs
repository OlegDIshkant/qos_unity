using CommonTools;
using CommonTools.Math;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// Вычисляет промежуточные <see cref="Transform"/> при переходе какого-либо объекта из отной конформации в другую.
    /// </summary>
    public abstract class TransitTFormsCalcer 
    {
        public Transform InitialTForm { get; private set; }
        public Transform TargetTForm { get; private set; }


        public TransitTFormsCalcer(Transform initial, Transform target)
        {
            InitialTForm = initial;
            TargetTForm = target;
        }


        public abstract Transform Calc(NormValue normTime);


        public void ChangeTarget(Transform target)
        {
            TargetTForm = target;
            OnTargetTformChanged();
        }


        protected abstract void OnTargetTformChanged();
    }
}