using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace Game.Scripts.Spin
{
    public class SpinGenerator
    {
        private readonly SpinDataHolder _spinDataHolder;

        private Dictionary<SpinData, int> _resultAppearCountDictionary;
        private Dictionary<SpinData, int> _remainExtensionCountDictionary;
        private Dictionary<SpinData, int> _startIndexDictionary;
        private Dictionary<SpinData, List<Vector2Int>> _intervalListDictionary;

        public Dictionary<SpinData, int> GetRemainExtensionCountDictionary() => _remainExtensionCountDictionary;
        public Dictionary<SpinData, List<Vector2Int>> GetIntervalListDictionary() => _intervalListDictionary;

        [Inject]
        public SpinGenerator(SpinDataHolder spinDataHolder)
        {
            _spinDataHolder = spinDataHolder;
        }

        public void GenerateSpinListNew()
        {
            ResetStates();
            _spinDataHolder.spinResultList.Value = new SpinResult[100];
            bool[] resultOccupiedArray = new bool[100];
            var sortedSpinDataList =
                _spinDataHolder.spinDataList.OrderByDescending(result => result.percentage).ToList();

            GenerateSpinDictionaries();

            //set 100 spin
            for (int i = 0; i < 100; i++)
            {
                var currentSpinData = GetClosestSpin(sortedSpinDataList);

                int resultInterval = 100 / currentSpinData.percentage;
                int resultRemainFactor = (100 % currentSpinData.percentage != 0) &&
                                         _remainExtensionCountDictionary[currentSpinData] <
                                         (100 % currentSpinData.percentage)
                    ? 1
                    : 0;
                int placementIndex = _startIndexDictionary[currentSpinData];
                int placementIndexOffset = 0;
                while (resultOccupiedArray[placementIndex + placementIndexOffset])
                {
                    placementIndexOffset++;
                }

                resultOccupiedArray[placementIndex + placementIndexOffset] = true;
                Vector2Int intervalVector = new Vector2Int(_startIndexDictionary[currentSpinData], 0);
                _startIndexDictionary[currentSpinData] += resultInterval;

                if ((placementIndexOffset >= resultInterval + resultRemainFactor - 1 && resultRemainFactor == 1) ||
                    ((100 % currentSpinData.percentage) - _remainExtensionCountDictionary[currentSpinData]) ==
                    (currentSpinData.percentage) - _resultAppearCountDictionary[currentSpinData] &&
                    100 % currentSpinData.percentage != 0)
                {
                    _startIndexDictionary[currentSpinData]++;
                    _remainExtensionCountDictionary[currentSpinData]++;
                }

                intervalVector.y = _startIndexDictionary[currentSpinData] - 1;
                _resultAppearCountDictionary[currentSpinData]++;
                _intervalListDictionary[currentSpinData].Add(intervalVector);
                _spinDataHolder.spinResultList.Value[placementIndex + placementIndexOffset] =
                    currentSpinData.spinResult;
            }
        }

        private SpinData GetClosestSpin(List<SpinData> sortedSpinDataList)
        {
            var currentSpinData =  sortedSpinDataList[0];
            for (int j = 1; j < sortedSpinDataList.Count; j++)
            {
                var comparingSpinData = sortedSpinDataList[j];
                int comparingSpinDataLimit = GetCurrentIntervalLimit(comparingSpinData);
                int currentSpinDataLimit = GetCurrentIntervalLimit(currentSpinData);

                if (comparingSpinDataLimit == currentSpinDataLimit)
                {
                    if (_startIndexDictionary[comparingSpinData] < _startIndexDictionary[currentSpinData])
                    {
                        currentSpinData = comparingSpinData;
                    }
                    else if (comparingSpinData.percentage < currentSpinData.percentage)
                    {
                        currentSpinData = comparingSpinData;
                    }

                    //add randomness for the same weighted probabilities
                    else if (comparingSpinData.percentage == currentSpinData.percentage)
                    {
                        int randomSelectIndex = Random.Range(0, 2);
                        if (randomSelectIndex == 1) currentSpinData = comparingSpinData;
                    }
                }

                else if (comparingSpinDataLimit < currentSpinDataLimit) currentSpinData = comparingSpinData;
            }

            return currentSpinData;
        }

        private void GenerateSpinDictionaries()
        {
            _resultAppearCountDictionary = new Dictionary<SpinData, int>();
            _remainExtensionCountDictionary = new Dictionary<SpinData, int>();
            _startIndexDictionary = new Dictionary<SpinData, int>();
            _intervalListDictionary = new Dictionary<SpinData, List<Vector2Int>>();
            foreach (var result in _spinDataHolder.spinDataList)
            {
                _resultAppearCountDictionary[result] = 0;
                _startIndexDictionary[result] = 0;
                _remainExtensionCountDictionary[result] = 0;
                _intervalListDictionary[result] = new List<Vector2Int>();
            }
        }

        private int GetCurrentIntervalLimit(SpinData spinData)
        {
            int spinInterval = 100 / spinData.percentage;
            int spinRemainFactor = (100 % spinData.percentage != 0) &&
                                   _remainExtensionCountDictionary[spinData] <
                                   (100 % spinData.percentage)
                ? 1
                : 0;

            return _startIndexDictionary[spinData] + spinInterval + spinRemainFactor;
        }

        public SpinResult Spin()
        {
            var result = _spinDataHolder.spinResultList.Value[_spinDataHolder.spinIndex];
            _spinDataHolder.spinIndex = (_spinDataHolder.spinIndex + 1) % 100;
            return result;
        }

        private void ResetStates()
        {
            _spinDataHolder.spinResultList.Value = null;
            _spinDataHolder.spinIndex = 0;
        }
    }
}