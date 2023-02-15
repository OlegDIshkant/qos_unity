using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// Контроллер, который получает на вход положения объектов (имющих ключ <see cref="Key"/>), как-либо модифицирует эти положения и подает на выход.
    /// </summary>
    public abstract class TFormsModifier<Key> : AbstractController, IDictionaryDataProvider<Key, Transform>
    {
        private readonly IWhenToModifyStategy _whenToModifyStategy;
        private readonly IModifyStategy<Key> _modifyStategy;
        private readonly IDictionaryDataProvider<Key, Transform> _originalTFormsProvider;

        private DictionaryData<Key, Transform>.Editable _dataEdit;
        public DictionaryData<Key, Transform> DictionaryOutput { get; private set; }

        public TFormsModifier(
            Contexts context,
            IDictionaryDataProvider<Key, Transform> originalTFormsProvider,
            IWhenToModifyStategy whenToModifyStategy,
            IModifyStategy<Key> modifyStategy) :
            base(context)
        {
            _originalTFormsProvider = originalTFormsProvider;
            _whenToModifyStategy = whenToModifyStategy;
            _modifyStategy = modifyStategy;

            DictionaryOutput = new DictionaryData<Key, Transform>(out _dataEdit);

            RememberAvailableObjects();
        }


        public override void Update()
        {
            // В первую очередь мы должны копировать все данные из источника, чтобы всегда мочь передать их следущим контроллерам...
            CopyChangesFromOriginalSource();

            // а при необходимости какие-то из этих данных мы редактируем.
            if (ToModify())
            {
                ModifyTForms();
            }
        }


        private bool ToModify() => _whenToModifyStategy.ToModify();


        protected virtual void ModifyTForms()
        {
            var modifiedTforms = _modifyStategy.Modify(_originalTFormsProvider.DictionaryOutput);
            ApplyModifiedTforms(modifiedTforms);
        }


        private void ApplyModifiedTforms(IEnumerable<KeyValuePair<Key, Transform>> tForsToApply)
        {
            foreach (var item in tForsToApply)
            {
                _dataEdit.SetItem(item.Key, item.Value);
            }
        }


        private void RememberAvailableObjects()
        {
            foreach (var (objectId, Transform) in AvailableObjects())
            {
                _dataEdit.SetItem(objectId, Transform);
            }
        }


        private IEnumerable<(Key, Transform)> AvailableObjects()
        {
            if (_originalTFormsProvider?.DictionaryOutput?.Items != null)
            {
                foreach (var item in _originalTFormsProvider.DictionaryOutput.Items)
                {
                    yield return (item.Key, item.Value);
                }
            }
        }


        private void CopyChangesFromOriginalSource()
        {
            var data = _originalTFormsProvider?.DictionaryOutput;
            if (data?.HasChanged ?? false)
            {
                foreach (var item in data.AddedOrChanged)
                {
                    _dataEdit.SetItem(item.Key, item.Value);
                }

                foreach (var item in data.Removed)
                {
                    _dataEdit.RemoveItem(item);
                }
            }
        }

    }





    public abstract class TFormsModifier<Key, ED> : TFormsModifier<Key>, IDictionaryDataProvider<Key, ED>
        where ED : struct
    {
        private IModifyStategyWithExtraData<Key, ED> _modifyStategy;

        DictionaryData<Key, ED> IDictionaryDataProvider<Key, ED>.DictionaryOutput => _modifyStategy.DictionaryOutput;


        protected TFormsModifier(
            Contexts context,
            IDictionaryDataProvider<Key, Transform> originalTFormsProvider,
            IWhenToModifyStategy whenToModifyStategy,
            IModifyStategyWithExtraData<Key, ED> modifyStategy) :
            base(
                context,
                originalTFormsProvider,
                whenToModifyStategy,
                modifyStategy)
        {
            _modifyStategy = modifyStategy;
        }

    }





    /// <summary>
    /// Определяет, как именно нужно модифицировать те или иные расположения в пространстве.
    /// </summary>
    public interface IModifyStategy<Key>
    {
        /// <returns> Словарь только с изененными <see cref="Transform"/>-ами. Иными словами вполне может быть меньше переданных на вход <paramref name="transforms"/>.  </returns>
        IDictionary<Key, Transform> Modify(DictionaryData<Key, Transform> transforms);
    }


    /// <summary>
    /// <see cref="IModifyStategy"/>, которая помимо решения своих основных задач ещё и предоставляет определенные данные.
    /// </summary>
    public interface IModifyStategyWithExtraData<Key, ExtraData> : IModifyStategy<Key>, IDictionaryDataProvider<Key, ExtraData>
        where ExtraData : struct
    {
    }


}
