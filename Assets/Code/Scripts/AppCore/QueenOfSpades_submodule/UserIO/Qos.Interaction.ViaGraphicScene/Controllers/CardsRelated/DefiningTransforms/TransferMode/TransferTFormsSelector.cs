using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, который определяет, результаты работы какого из возможных контроллеров 
    /// (определяющих положения карт игрока, когда другой выбирает, какие из них взять себе) 
    /// должны быть использованы.
    /// </summary>
    public class TransferTFormsSelector : AbstractController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly ISingleDataProvider<TransferProcessInfo> _processInfoProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _player_to_player_TForms;
        private readonly IDictionaryDataProvider<CardId, Transform> _player_to_mainPlayer_TForms;
        private readonly IDictionaryDataProvider<CardId, Transform> _mainPlayer_to_player_TForms;

        private IDictionaryDataProvider<CardId, Transform> _tFormsSource;


        private DictionaryData<CardId, Transform>.Editable _outputEdit;
        public DictionaryData<CardId, Transform> DictionaryOutput { get; private set; }


        public TransferTFormsSelector(
            Contexts contexts,
            ISingleDataProvider<TransferProcessInfo> processInfoProvider,
            IDictionaryDataProvider<CardId, Transform> player_to_player_TForms,
            IDictionaryDataProvider<CardId, Transform> player_to_mainPlayer_TForms,
            IDictionaryDataProvider<CardId, Transform> mainPlayer_to_player_TForms) : 
            base(contexts)
        {
            _processInfoProvider = processInfoProvider;
            _player_to_mainPlayer_TForms = player_to_mainPlayer_TForms;
            _player_to_player_TForms = player_to_player_TForms;
            _mainPlayer_to_player_TForms = mainPlayer_to_player_TForms;

            DictionaryOutput = new DictionaryData<CardId, Transform>(out _outputEdit);
        }


        public override void Update()
        {
            ChangeTFormsSourceIfNeeded();
            UpdateTFormsFromSource();
        }


        private void ChangeTFormsSourceIfNeeded()
        {
            if (ShouldChangeSource())
            {
                ChangeSource();
            }
        }


        private bool ShouldChangeSource() => _processInfoProvider.SingleOutput.HasChanged;


        private void ForgetPrevTForms()
        {
            if (_tFormsSource != null)
            {
                _outputEdit.Clear();
            }
        }


        private void ChangeSource()
        {
            ForgetPrevTForms();

            _tFormsSource = GetNewSource();
            if (_tFormsSource != null)
            {
                SetInitialTFormsFromSource(_tFormsSource);
            }

            Logger.Verbose($"Новый источник положений: {_tFormsSource}");
        }


        private IDictionaryDataProvider<CardId, Transform> GetNewSource()
        {
            if (_processInfoProvider.SingleOutput.Value != null)
            {
                var transferType = _processInfoProvider.SingleOutput.Value.Value.TransferType;
                return
                    transferType == TransferType.FROM_PLAYER_TO_PLAYER ? _player_to_player_TForms :
                    transferType == TransferType.FROM_MAIN_PLAYER_TO_PLAYER ? _mainPlayer_to_player_TForms :
                    transferType == TransferType.FROM_PLAYER_TO_MAIN_PLAYER ? _player_to_mainPlayer_TForms :
                    throw new System.Exception($"Unknown transfer type : {transferType}");
            }
            return null;
        }


        private void SetInitialTFormsFromSource(IDictionaryDataProvider<CardId, Transform> source)
        {
            foreach (var item in source.DictionaryOutput.Items)
            {
                _outputEdit.SetItem(item.Key, item.Value);
            }
        }


        private void UpdateTFormsFromSource()
        {
            if (_tFormsSource != null)
            {
                if (_tFormsSource.DictionaryOutput.HasChanged)
                {
                    foreach (var removed in _tFormsSource.DictionaryOutput.Removed)
                    {
                        _outputEdit.RemoveItem(removed);
                    }

                    foreach (var item in _tFormsSource.DictionaryOutput.AddedOrChanged)
                    {
                        _outputEdit.SetItem(item.Key, item.Value);
                    }
                }
            }
        }
    }
}