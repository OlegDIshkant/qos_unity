using System.Numerics;

namespace Qos.Interaction.ViaGraphicScene
{
    public interface ICursor_ReadOnly
    {
        bool PositionChanged { get; }
        Vector2 NdcPosition { get; }
        bool LeftClick { get; }
    }


    public interface ICursor : ICursor_ReadOnly
    {
        void Update();
    }

}
