using static CommonTools.Math.NDCPointToPlaneSolver;


namespace Qos.Interaction.ViaGraphicScene.SceneObjects
{
    public interface ICamera_ReadOnly : ITransformReadOnly
    {
        CameraParams Params { get; }
        bool ParamsChanged { get; }
    }


    public interface ICamera : ICamera_ReadOnly, ITransformEditable
    {

    }

}