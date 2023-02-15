using Qos.Interaction.ViaGraphicScene;


namespace GameScene
{
    public class CardHeap : ICardHeap
    {
        private CommonTools.Math.Transform _transform;

        public bool TransformChanged { get; private set; }

        public CommonTools.Math.Transform GetTransform() => _transform;

        public void SetTransform(CommonTools.Math.Transform tform) => _transform = tform;
    }
}