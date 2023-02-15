using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;

namespace Qos.Interaction.ViaGraphicScene.Controllers
{

    /// <summary>
    ///  онтроллер, содержащий в себе суб-контроллеры.
    /// </summary>
    public abstract class ControllersController : AbstractController
    {
        private ControllersQueue _controllersQueue;


        public ControllersController(Contexts contexts) : base(contexts)
        {
        }


        protected void SetControllersQueue(ControllersQueue controllersQueue)
        {
            if (_controllersQueue != null)
            {
                throw new InvalidOperationException("ќчередь контроллеров уже назначена.");
            }
            _controllersQueue = controllersQueue;
        }


        public override sealed void Update()
        {
            foreach (var controller in _controllersQueue.GetControllersInExecOrder())
            {
                Logger.Verbose($"#region  онтроллер '{controller}' начал обновление");
                Logger.AddLevel();

                controller.Update();

                Logger.RemoveLevel();
                Logger.Verbose($"#endrerion");
            }
        }
    }
}