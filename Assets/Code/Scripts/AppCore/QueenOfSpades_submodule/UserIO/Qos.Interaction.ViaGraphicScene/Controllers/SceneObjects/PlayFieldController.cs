using CommonTools;
using System;


namespace Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects
{
    public interface IPlayFieldController
    {
        IPlayField_ReadOnly PlayFieldInfo { get; }
    }


    public class PlayFieldController : AbstractController, IPlayFieldController
    {
        private IPlayField _playField;

        public IPlayField_ReadOnly PlayFieldInfo => _playField;
        public TimeContext TimeContext { set; private get; }


        public PlayFieldController(Contexts contexts, Func<IPlayField> PlayFieldFactoryMethod) : base(contexts)
        {
            _playField = PlayFieldFactoryMethod();

        }


        public override void Update()
        {

        }
    }
}