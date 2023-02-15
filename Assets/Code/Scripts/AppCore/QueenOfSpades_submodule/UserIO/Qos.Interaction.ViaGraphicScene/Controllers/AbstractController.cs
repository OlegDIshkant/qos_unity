using CommonTools;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;

namespace Qos.Interaction.ViaGraphicScene
{
    /// <summary>
    /// Базовый класс для контроллера, который должен выполнять определенную функцию в игре.
    /// </summary>
    public abstract class AbstractController
    {
        protected Contexts Contexts { get; private set; }


        public AbstractController(Contexts contexts)
        {
            Contexts = contexts;
        }


        public abstract void Update();
    }



    /// <summary>
    /// Контроллер, который знает о событиях, происходящих в игровом мире.
    /// </summary>
    public abstract class EventController : AbstractController
    {
        private Func<IEvent> _GetCurrentEvent;
        protected IEvent CurrentEvent => _GetCurrentEvent();

        protected EventController(Contexts contexts, Func<IEvent> GetCurrentEvent) : base(contexts)
        {
            _GetCurrentEvent = GetCurrentEvent; 
        }


    }



}
