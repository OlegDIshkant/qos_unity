

namespace Qos.Interaction.ViaGraphicScene.SceneObjects
{
    public interface ICard_ReadOnly : ITransformReadOnly
    {
    }


    public interface ICard : ITransformEditable, ICard_ReadOnly
    {
    }

}