using CommonTools;
using System;


namespace Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects
{
    public interface ICursorController
    {
        ICursor_ReadOnly CursorInfo { get; }
    }

    public class CursorController : AbstractController, ICursorController
    {
        private readonly ICursor _cursor;
        
        private TimeContext.OncePerFrame _oncePerFrameExecutor;

        public ICursor_ReadOnly CursorInfo => _cursor;



        public CursorController(Contexts contexts, Func<ICursor> CursorFactoryMethod) : base(contexts)
        {
            _cursor = CursorFactoryMethod();
            _oncePerFrameExecutor = contexts.TimeContext.NewOncePerFrameExecutor();
        }


        public override void Update()
        {
            _oncePerFrameExecutor.Execute(() => _cursor.Update());
        }
    }
}