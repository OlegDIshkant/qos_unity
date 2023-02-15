using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;

namespace Qos.Interaction.ViaGraphicScene.Controllers
{

    /// <summary>
    /// ����������, ���������� � ���� ���-�����������.
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
                throw new InvalidOperationException("������� ������������ ��� ���������.");
            }
            _controllersQueue = controllersQueue;
        }


        public override sealed void Update()
        {
            foreach (var controller in _controllersQueue.GetControllersInExecOrder())
            {
                Logger.Verbose($"#region ���������� '{controller}' ����� ����������");
                Logger.AddLevel();

                controller.Update();

                Logger.RemoveLevel();
                Logger.Verbose($"#endrerion");
            }
        }
    }
}