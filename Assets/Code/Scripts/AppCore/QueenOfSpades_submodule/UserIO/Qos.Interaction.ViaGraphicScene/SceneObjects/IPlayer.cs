using CommonTools;
using System;

namespace Qos.Interaction.ViaGraphicScene.SceneObjects
{
    public interface IPlayer_ReadOnly : ITransformReadOnly
    {
        //string GetName();
    }


    public interface IPlayer : ITransformEditable, IPlayer_ReadOnly, IDisposable
    {
        //void SetName(string name);
        void ShowCreating(NormValue normTime);
    }
}