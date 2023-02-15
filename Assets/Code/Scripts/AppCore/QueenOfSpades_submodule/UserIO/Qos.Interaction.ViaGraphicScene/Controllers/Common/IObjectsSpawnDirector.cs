using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Immutable;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{

    /// <summary>
    /// C�������, ����� ������� (������������������ ������ <typeparamref name="Key"/>) � ������ ������ ������ ������������ � ����.
    /// </summary>
    public interface IObjectsSpawnDirector<Key>
        where Key : struct
    {
        public ListData<Key> ShouldBeSpawnedNow { get; }
    }



}
