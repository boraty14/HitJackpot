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
            var sortedSpinDataList = _spinDataHolder.spinDataList.OrderByDescending(result => result.percentage).ToList();

            //set dictionaries
            _resultAppearCountDictionary = new Dictionary<SpinData, int>();
            //remain ext dictionary is for selecting the intervals more precisely.
            //For example if we have a %13 probability, 9 interval will be 8 element long and 4 interval
            // will be 7 element long. This dictionary holds how many times a spin result is increased its
            // interval by 1. Also in general reason we have multiple dictionaries is printing a detailed console
            // log and test the algorithm easily. We print each interval of result, their appearing indices, appear count,
            // and remainder count to see how many times we extend the intervals. It makes it really easy to see if
            // anything is wrong with the algorithm.
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


            //set 100 spin
            for (int i = 0; i < 100; i++)
            {
                // so basically this part is for selecting spin with the minimum interval limit.
                // According to their appearance count and last interval start index, which both
                // hold in dictionaries, I select the interval with minimal upper limit. For example
                // we can have possibilities that should appear between 17-33, 20-39, 30-39, 24-29 ...
                // In these intervals we chose the last one, 24-29 because it needs to placed sooner than others.
                // We can decide it by checking its intervals limit index. It has the smallest index.
                // After placing it, we increase its startIndex, appear count and remainderExtension count if we extended
                // its interval by 1, according to its remainder after division with 100.
                // The point is for probabilities that have remainder after dividing 100, not all the intervals are same.
                // By checking if spin interval should be extended according to extension count and empty indices, we extend
                // the interval or not. 

                //get minimal interval limit
                var currentSpinData = sortedSpinDataList[0];
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

                // after getting result set its index in the first available space in interval
                // and check if we should and if we will extend the interval
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
                    // so a quick note this one looks like a good candidate for out of range exception
                    // in the while check but actually it is not. Since after checking interval upper limit,
                    // we place the result in the first available index, algorithm first places the empty indices from
                    // the start, (not in a perfect manner like from 0-1-2...99) but once an interval upper limit hits 99,
                    // it will never be called again and other spin results will be placed to empty places before the
                    // last placed index. Therefore before while check index becomes 100, it will be placed for sure.
                    placementIndexOffset++;
                }

                // set the result and update dictionaries
                resultOccupiedArray[placementIndex + placementIndexOffset] = true;
                Vector2Int intervalVector = new Vector2Int(_startIndexDictionary[currentSpinData], 0);
                _startIndexDictionary[currentSpinData] += resultInterval;

                // what I check here is if I had to extend interval to find an available index for placement, or
                // if the other intervals were not extended and from this point I have to extend this interval no matter
                // what to fill all the interval up to index 99. If it is the case, extend the interval
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
                _spinDataHolder.spinResultList.Value[placementIndex + placementIndexOffset] = currentSpinData.spinResult;
            }

            // In short this algorithm works perfectly for the given probabilities in the case, all %13 results shared in their
            // 9 interval by 8 length and 4 interval by 7 length (also correction is 9x8 + 7x4 = 100)
            // This applies for all the other results. For the given results and percentages in the case, algorithm
            // distributes perfectly each result. Though in other experiments, when I try the algorithm with less result count
            // and higher percentages, -for example 50,35,10,5- or with an extreme example like 94,2,2,2 I observed algorithm
            // can extend some intervals by 2, instead of 1. Result count is still perfect but distribution becomes almost perfect.
            // This basically happens because algorithm checks for the result that should be placed most early. Since
            // extreme percentages more than 50, are almost always need to be placed first, this can cause stacking for
            // other possibilities. Especially when you have a case like 94,2,2,2. Since %94 is almost always prior, and also
            // the other 3 have the same percentages, in the check point they cause stacking and one of them misses its first interval
            // by 1. This can be fixed by optimizing the algorithm to check such stackings for extreme cases and percentages.
            // Other than that, I tried the algorithm for many cases. Apart from the ones
            // I mentioned, for every other possibilities I observed perfect distribution in the intervals. Though
            // in the given extreme examples, 1-2 interval have offset by +-1 for mentioned stacking conditions,
            // other than that they work as expected.
            // I wanted to write this comment to explain clearly how algorithm works, and also to let you know that I am
            // aware of the little flaw for extreme cases, though I could not figure out a rational way to eliminate it at the moment.

            // Note: In general code should not contain this much comment and explain itself and be a bit cleaner maybe.
            // But this function in the whole project represents the algorithm used for probability distribution and
            // it needs explaining. Optimization can be made but since the system itself does not trigger on the
            // gameplay spinning and sets before game starts, I think it is acceptable. Also, other than
            // this function this project does not have any other comments. I believe rest of the project
            // is self explanatory.
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