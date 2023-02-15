using CommonTools;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, выбирающий среди нескольких <see cref="TransferProcessInfo"/> тот, что необходим для текущего процесса.
    /// </summary>
    public class TransferParamsProvider : AbstractController, ISingleDataProvider<TransferProcessInfo>
    {
        private readonly int _processIndex;
        private readonly IDictionaryDataProvider<int, TransferProcessInfo> _transferInfoProvider;

        private bool _isIdling = true;

        private SingleData<TransferProcessInfo>.Editable _outputEdit;
        public SingleData<TransferProcessInfo> SingleOutput { get; private set; }


        public TransferParamsProvider(
            Contexts contexts, 
            int processIndex,
            IDictionaryDataProvider<int, TransferProcessInfo> transferInfoProvider) : 
            base(contexts)
        {
            _processIndex = processIndex;
            _transferInfoProvider = transferInfoProvider;   

            SingleOutput = new SingleData<TransferProcessInfo>(out _outputEdit);
        }


        public override void Update()
        {
            if (_isIdling)
            {
                Logger.Verbose("Ожидаем начала процесса передачи карт.");
                if (TryStartProcess())
                {
                    Logger.Verbose("Считаем процесс передачи карт начатым.");
                    _isIdling = false;
                }
            }
            else
            {
                Logger.Verbose("Ожидаем окончание процесса передачи карт.");
                if (TryFinishProcess())
                {
                    Logger.Verbose("Считаем процесс передачи карт завершенным.");
                    _isIdling = true;
                }
            }
        }


        private bool TryStartProcess()
        {
            if (_transferInfoProvider.DictionaryOutput.HasChanged &&
                _transferInfoProvider.DictionaryOutput.AddedOrChanged.TryGetValue(_processIndex, out var processInfo))
            {
                _outputEdit.Set(processInfo);
                return true;
            }

            return false;
        }


        private bool TryFinishProcess()
        {
            if (_transferInfoProvider.DictionaryOutput.HasChanged &&
                _transferInfoProvider.DictionaryOutput.Removed.Contains(_processIndex))
            {
                _outputEdit.Clear();
                return true;
            }
            return false;
        }
    }
}