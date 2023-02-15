namespace Qos.Interaction.ViaGraphicScene
{
    public interface IDeck_ReadOnly : ITransformReadOnly
    {
    }


    public interface IDeck : ITransformEditable, IDeck_ReadOnly
    {
    }

}