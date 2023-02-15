using CommonTools.Math;
using System;

namespace Qos.Interaction.ViaGraphicScene
{
    public interface ITransformReadOnly
    {
        Transform GetTransform();
        bool TransformChanged { get; }
    }


    public interface ITransformEditable
    {
        void SetTransform(Transform tform);
    }
}
