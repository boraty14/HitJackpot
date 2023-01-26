using Game.Scripts.Spin;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class SaveHandler : MonoBehaviour
    {
        [SerializeField] private SpinGenerator _spinGenerator;
        [SerializeField] private SpinResultList _spinResultList;
        private const string SaveKey = "SAVEKEY";

        private void OnEnable()
        {
            _spinGenerator.onResetState += SpinGenerator_OnResetState;
            LoadSpinData();
        }

        private void OnDisable()
        {
            _spinGenerator.onResetState -= SpinGenerator_OnResetState;
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
                spinIndex = _spinGenerator.spinIndex
            };
            ES3.Save<SpinSave>(SaveKey, spinSave);
        }

        private void LoadSpinData()
        {
            if (ES3.KeyExists(SaveKey))
            {
                var savedSpinData = ES3.Load<SpinSave>(SaveKey);
                _spinResultList.Value = savedSpinData.spinResults;
                _spinGenerator.spinIndex = savedSpinData.spinIndex;
            }
            else
            {
                _spinGenerator.GenerateSpinListNew();
            }
        }
    }
}