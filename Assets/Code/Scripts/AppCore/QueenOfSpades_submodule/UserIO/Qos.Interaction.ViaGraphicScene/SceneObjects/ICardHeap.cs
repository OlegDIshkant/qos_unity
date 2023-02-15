


namespace Qos.Interaction.ViaGraphicScene
{
    public interface ICardHeap_ReadOnly : ITransformReadOnly
    {
    }


    public interface ICardHeap : ITransformEditable, ICardHeap_ReadOnly
    {

    }
}