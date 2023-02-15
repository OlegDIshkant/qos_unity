using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene
{
    public interface IPlayField_ReadOnly
    {
        /// <summary>
        /// Центр круга, вокруг которого расположены игроки. 
        /// </summary>
        Vector3 FloorCircleCenter { get; }

        /// <summary>
        /// Радиус круга, вокруг которого расположены игроки. 
        /// </summary>
        float FloorCircleRadius { get; }

        /// <summary>
        /// Центр круга, куда сбрасываются карты.
        /// </summary>
        Vector3 CardHeapCenter { get; }

        /// <summary>
        /// Радиус круга, куда сбрасываются карты.
        /// </summary>
        float CardHeapRadius { get; }
    }


    public interface IPlayField : IPlayField_ReadOnly
    {
        /// <summary>
        /// Задает <see cref="IPlayField_ReadOnly.FloorCircleCenter"/>.
        /// </summary>
        void SetFlorCircleCenter(Vector3 point);

        /// <summary>
        /// Задает <see cref="IPlayField_ReadOnly.FloorCircleRadius"/>.
        /// </summary>
        void SetFlorCircleRadius(float radius);
    }

}
