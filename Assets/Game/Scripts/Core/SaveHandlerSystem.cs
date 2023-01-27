using Game.Scripts.Spin;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Core
{
    public class SaveHandlerSystem : IStartable
    {
        private readonly SpinDataHolder _spinDataHolder;
        private readonly SpinResultList _spinResultList;
        private readonly SaveHandler _saveHandler;
        private readonly SpinGenerator _spinGenerator;
        private const string SaveKey = "SAVEKEY";

        [Inject]
        public SaveHandlerSystem(SpinDataHolder spinDataHolder,
            SpinResultList spinResultList,
            SaveHandler saveHandler,
            SpinGenerator spinGenerator)
        {
            _spinDataHolder = spinDataHolder;
            _spinResultList = spinResultList;
            _saveHandler = saveHandler;
            _spinGenerator = spinGenerator;

            _saveHandler.onGameCloseState = SaveSpinData;
            _saveHandler.onLoadState = LoadSpinData;
            Debug.Log(1);
        }

        private void SaveSpinData()
        {
            var spinSave = new SpinSave
            {
                spinResults = _spinResultList.Value,
                spinIndex = _spinDataHolder.spinIndex
            };
            ES3.Save<SpinSave>(SaveKey, spinSave);
        }

        private void LoadSpinData()
        {
            if (ES3.KeyExists(SaveKey))
            {
                var savedSpinData = ES3.Load<SpinSave>(SaveKey);
                _spinResultList.Value = savedSpinData.spinResults;
                _spinDataHolder.spinIndex = savedSpinData.spinIndex;
            }
            else
            {
                ES3.DeleteKey(SaveKey);
                _spinGenerator.GenerateSpinListNew();
            }
        }

        public void Start()
        {
            
        }
    }
}