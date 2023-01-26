using Game.Scripts.Spin;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Core
{
    public class SaveHandler : MonoBehaviour
    {
        [SerializeField] private SpinDataHolder _spinDataHolder;
        [SerializeField] private SpinResultList _spinResultList;
        private const string SaveKey = "SAVEKEY";

        private void OnEnable()
        {
            _spinDataHolder.onResetState += SpinGenerator_OnResetState;
            LoadSpinData();
        }

        private void OnDisable()
        {
            _spinDataHolder.onResetState -= SpinGenerator_OnResetState;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            SaveSpinData();
        }

        private void OnApplicationQuit()
        {
            SaveSpinData();
        }

        private void SpinGenerator_OnResetState()
        {
            ES3.DeleteKey(SaveKey);
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
                _spinDataHolder.GenerateSpinListNew();
            }
        }
    }
}