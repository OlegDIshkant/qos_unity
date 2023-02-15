using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Immutable;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{

    /// <summary>
    /// Cообщает, какие объекты (идентифицирующиеся ключом <typeparamref name="Key"/>) в данный момент должны существовать в игре.
    /// </summary>
    public interface IObjectsSpawnDirector<Key>
        where Key : struct
    {
        public ListData<Key> ShouldBeSpawnedNow { get; }
    }



}
