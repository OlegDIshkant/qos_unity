using System.Collections;


namespace Qos.GameLogic.GameWorld.Stages
{
    /// <summary>
    /// Та или иная стадия матча и соответствующие ей действия.
    /// </summary>
   internal abstract class AbstractStage
    {
        public abstract IEnumerator Complete();
    }

}
