

namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// —тратеги€, определ€юща€, нужно ли в данный момент модифицировать нечто.
    /// </summary>
    public interface IWhenToModifyStategy
    {
        bool ToModify();
    }


    public class AllwaysModifyStrategy : IWhenToModifyStategy
    {
        public bool ToModify() => true;
    }


    /// <summary>
    /// —тратеги€, котора€ позвол€ет переключатьс€ между двум€ режимами работы:
    /// в одном мы всегда модифицируем положени€ карт,
    /// в другом - никогда.
    /// </summary>
    public abstract class SwitchableWhenToModifyStategy : IWhenToModifyStategy
    {
        protected bool InModifiesAllowedState { get; private set; } = false;

        public bool ToModify()
        {
            if (InModifiesAllowedState)
            {
                if (IfExitModifiesAllowedState())
                {
                    InModifiesAllowedState = false;
                }
            }
            else
            {
                if (IfEnterModifiesAllowedState())
                {
                    InModifiesAllowedState = true;
                }
            }

            return InModifiesAllowedState;
        }


        protected abstract bool IfEnterModifiesAllowedState();


        protected abstract bool IfExitModifiesAllowedState();
    }


}
