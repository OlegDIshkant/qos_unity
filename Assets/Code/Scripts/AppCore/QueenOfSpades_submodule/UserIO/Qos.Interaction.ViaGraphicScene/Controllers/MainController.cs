using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions;
using CommonTools.Math;


namespace Qos.Interaction.ViaGraphicScene.Controllers
{
    /// <summary>
    /// —оздает все контроллеры требуемые дл€ отображени€ игры, выстраива€ их в пор€дке очередности выполнени€ (оновлени€).
    /// </summary>
    public class MainController : ControllersController
    {
        private readonly Contexts _contexts;
        private readonly IGraphicSceneFactory _graphicSceneFactory;
        private readonly PlayersInfo _playersInfo;
        private readonly HighlightSettings _highlightSettings = new HighlightSettings(selectHighlight: HighlightType.LIGHT, overlayHighlight: HighlightType.HEAVY);
        private readonly Func<IEvent> _GetCurrentEventCallBack;
        private readonly Action _OnExitRequested;

        private ICameraController _cam;
        private ICursorController _cursor;
        private IPlayFieldController _playField;

        private IPlayersTFormsDefiner _playersTFormsDefiner;


        public MainController(
            Contexts contexts, 
            IGraphicSceneFactory graphicSceneFactory, 
            Func<IEvent> GetCurrentEventCallBack,
            Action OnExitRequested) :
            base(contexts)
        {
            _contexts = contexts;
            _graphicSceneFactory = graphicSceneFactory;
            _playersInfo = contexts.PlayersInfo;
            _GetCurrentEventCallBack = GetCurrentEventCallBack;
            _OnExitRequested = OnExitRequested;

            PrepareControllersChain();
        }


        public void PrepareControllersChain()
        {
            var controllersQueue = new ControllersQueue();

            IndependentControllers(controllersQueue);
            SceneObjectsControllers(controllersQueue);
            PlayersControllers(controllersQueue);
            CardsControllers(controllersQueue);

            SetControllersQueue(controllersQueue);
        }


        private void IndependentControllers(ControllersQueue c)
        {
            c.AddWithoutTag(new ExitUiController(Contexts, _graphicSceneFactory.CreateExitUi, _OnExitRequested));
            c.AddWithoutTag(new WinLooseAnnouncerController(Contexts, _GetCurrentEventCallBack, _graphicSceneFactory.CreateWinLooseAnnouncer));
        }


        private void SceneObjectsControllers(ControllersQueue c)
        {
            c.AddWithoutTag(new GameExiter(_contexts, _GetCurrentEventCallBack, _OnExitRequested));
            _cursor = c.AddWithoutTag(new CursorController(_contexts, _graphicSceneFactory.CreateCursor));
            _playField = c.AddWithoutTag(new PlayFieldController(_contexts, _graphicSceneFactory.CreatePlayField));
            _cam = c.AddWithoutTag(new CameraController(_contexts, _graphicSceneFactory.CreateCamera, _playField));
        }


        private void PlayersControllers(ControllersQueue c)
        {
            var definingPlayersActions = c.AddWithoutTag(new PlayerActionsProvider(_contexts, _GetCurrentEventCallBack));
            var prevPlayerTFormsProvider = new PromissedData<PlayerId, CommonTools.Math.Transform>();
            var inGamePlayers = c.AddWithoutTag(new InGamePlayersProviders(_contexts, _playersInfo.mainPLayerId, definingPlayersActions));
            _playersTFormsDefiner = c.AddWithoutTag(new DefiningPlayersTForms(_contexts, _cam, _playField, definingPlayersActions, prevPlayerTFormsProvider, inGamePlayers));
            var players = c.AddWithoutTag(new PlayersController(_contexts, _graphicSceneFactory.CreatePlayer, _playersTFormsDefiner, inGamePlayers));
            var playersTFormsRemember = c.AddWithoutTag(new TFormsRemember<PlayerId>(_contexts, players));
            prevPlayerTFormsProvider.AttachDataProvider(playersTFormsRemember);
        }



        private void CardsControllers(ControllersQueue c)
        {
            var deck = c.AddWithoutTag(new DeckController(_contexts, _graphicSceneFactory.CreateDeck, _playersTFormsDefiner));
            var cardHeap = c.AddWithoutTag(new CardHeapController(_contexts, _graphicSceneFactory.CreateCardHeap, _playField));


            var cardsActions = c.AddWithoutTag(new CardActionsProvider(_contexts, _GetCurrentEventCallBack));

            var prevCardTforms = new PromissedData<CardId, CommonTools.Math.Transform>();
            var cardTforms = c.AddWithoutTag(new DefiningCardsTForms(_contexts, cardsActions, deck, cardHeap, _cursor, _cam, _playersTFormsDefiner, prevCardTforms, _highlightSettings));

            var cardsSpawnDirector = c.AddWithoutTag(new CardsSpawnDirector(Contexts, cardsActions));
            var cardsModelsProvider = c.AddWithoutTag(new CardModelsProvider(Contexts, _GetCurrentEventCallBack));
            var cards = c.AddWithoutTag(new CardsController(_contexts, _graphicSceneFactory.CreateCard, cardTforms, cardsSpawnDirector, cardsModelsProvider));
            var cardsTFormsRemember = c.AddWithoutTag(new TFormsRemember<CardId>(_contexts, cards));
            prevCardTforms.AttachDataProvider(cardsTFormsRemember);


            c.AddWithoutTag(new UserInteractionsController(_contexts, _GetCurrentEventCallBack, _playersInfo.mainPLayerId, _graphicSceneFactory.CreateDiscModeUi, cardTforms));

        }
    }



    /// <summary>
    /// ѕрокси дл€ <see cref="IDictionaryDataProvider{K, V}"/>, который ещЄ не создан, но будет создан к моменту востребовани€.
    /// </summary>
    public class PromissedData<K, V> : IDictionaryDataProvider<K, V>
        where V : struct
    {
        private IDictionaryDataProvider<K, V> _dataProvider;
        private Func<DictionaryData<K, V>> _GetData;

        public DictionaryData<K, V> DictionaryOutput => _GetData();


        public PromissedData()
        {
            _GetData = ThrowException; // пока прокси не св€зан с источником данных, любое обращение к данным выкидывает ошибку
        }


        private DictionaryData<K, V> ThrowException() => throw new InvalidOperationException("ќбещанные данные не готовы.");
        private DictionaryData<K, V> ProvideData() =>_dataProvider.DictionaryOutput;


        /// <summary>
        ///  —в€зывает прокси с источником данных.
        /// </summary>
        public void AttachDataProvider(IDictionaryDataProvider<K, V> dataProvider)
        {
            if (_dataProvider != null)
            {
                throw new InvalidOperationException("”же прив€зан к другому источнику данных.");
            }
            _dataProvider = dataProvider;
            _GetData = ProvideData;
        }
    }














    /// <summary>
    ///  онтроллер, запоминающий последние расположени€ объектов в пространстве.
    /// </summary>
    public class TFormsRemember<K> : AbstractController, IDictionaryDataProvider<K, Transform>
    {

        private DictionaryData<K, Transform>.Editable _outputEdit;
        public DictionaryData<K, Transform> DictionaryOutput { get; private set; }

        public TFormsRemember(Contexts contexts, ITFormsProvider<K> tformsProvider) : base(contexts)
        {
            DictionaryOutput = new DictionaryData<K, Transform>(out _outputEdit);
            tformsProvider.OnTransformChanged += TformsProvider_OnTransformChanged;
        }


        private void TformsProvider_OnTransformChanged(K obj, Transform newTForm)
        {
            _outputEdit.SetItem(obj, newTForm);
        }


        public override void Update()
        {

        }

    }



}