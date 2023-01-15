using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.GameElements
{
    [CreateAssetMenu]
    public class SpinGenerator : ScriptableObject
    {
        public SpinResultList spinResultList;
        public List<SpinData> spinDataList;
        [HideInInspector] public int spinIndex;
        private const string SaveKey = "SAVEKEY";
        public Dictionary<SpinData, int> RemainExtensionCountDictionary { get; private set; }
    
        public void GenerateSpinListNew()
        {
            ES3.DeleteKey(SaveKey);
            spinIndex = 0;
            spinResultList.Value = new SpinResult[100];
            bool[] resultOccupiedArray = new bool[100];
            Dictionary<SpinData, int> resultAppearCountDictionary = new Dictionary<SpinData, int>();
            //remain ext dictionary is for selecting the intervals more precisely.
            //For example if we have a %13 probability, 9 interval will be 8 element long and 4 interval
            // will be 7 element long. This dictionary holds how many times a spin result is increased its
            // interval by 1.
            Dictionary<SpinData, int> remainExtensionCountDictionary = new Dictionary<SpinData, int>();
            Dictionary<SpinData, int> startIndexDictionary = new Dictionary<SpinData, int>();
        
            var sortedResults = spinDataList.OrderByDescending(result => result.percentage).ToList();
            //set dictionaries
            foreach (var result in spinDataList)
            { 
                resultAppearCountDictionary[result] = 0;
                startIndexDictionary[result] = 0;
                remainExtensionCountDictionary[result] = 0;
            }

        
            //set 100 spin
            for (int i = 0; i < 100; i++)
            {
                // so basically this part is for selecting spin with the at least interval limit.
                // According to their appearance count and last interval start index, which both
                // hold in dictionaries, I select the interval with minimal upper limit. For example
                // we can have possibilities that should appear between 17-33, 20-39,24-29 ...
                // In these intervals we chose the last one, 24-29 because it needs to placed sooner than others.
                // After placing it, we increase its startIndex, appear count and remainderExtension count if we extended
                // its interval by 1, according to its remainder after division with 100.
                // The point is for probabilities that have remainder after dividing 100, not all the intervals are same.
                // By checking if spin interval should be extended according to extension count and empty indices, we extend
                // the interval or not. 
            
                //get minimal interval limit
                var currentResult = sortedResults[0];
                for (int j = 1; j < sortedResults.Count; j++)
                {
                    var comparingResult = sortedResults[j];
                    int comparingResultInterval = 100 / comparingResult.percentage;
                    int comparingResultRemainFactor = (100 % comparingResult.percentage != 0) &&
                                                      remainExtensionCountDictionary[comparingResult] < (100 % comparingResult.percentage) ?
                        1:0;

                    int currentResultInterval = 100 / currentResult.percentage;
                    int currentResultRemainFactor = (100 % currentResult.percentage != 0) &&
                                                    remainExtensionCountDictionary[currentResult] <
                                                    (100 % currentResult.percentage)
                        ? 1 : 0;
                
                    int comparingResultLimit = startIndexDictionary[comparingResult] + comparingResultInterval +
                                               comparingResultRemainFactor;

                    int currentResultLimit = startIndexDictionary[currentResult] + currentResultInterval +
                                             currentResultRemainFactor;
                

                    if (comparingResultLimit == currentResultLimit)
                    {
                        if (startIndexDictionary[comparingResult] < startIndexDictionary[currentResult])
                        {
                            currentResult = comparingResult;
                        }

                        //add randomness for the same weighted probabilities
                        else if(comparingResult.percentage == currentResult.percentage)
                        {
                            int randomSelectIndex = Random.Range(0, 2);
                            if (randomSelectIndex == 1) currentResult = comparingResult;
                        }
                    
                    }
                
                    else if (comparingResultLimit < currentResultLimit) currentResult = comparingResult;
                }

                // after getting result set its index in the first available space in interval
                // and check if we should and if we will extend the interval
                int resultInterval = 100 / currentResult.percentage;
                int resultRemainFactor = (100 % currentResult.percentage != 0) &&
                                         remainExtensionCountDictionary[currentResult] < (100 % currentResult.percentage) ?
                    1:0;
                int placementIndex = startIndexDictionary[currentResult];
                int placementIndexOffset = 0;
                while (resultOccupiedArray[placementIndex + placementIndexOffset])
                {
                    placementIndexOffset++;
                    if (placementIndex + placementIndexOffset >= 100)
                    {
                        // SHOULD NOT REACH HERE BUT JUST IN CASE IF I MADE A MISTAKE
                        Debug.LogError("placement index " + placementIndex + " percentage " + currentResult.percentage); 
                        return;
                    }
                }

                // set the result and update dictionaries
                resultOccupiedArray[placementIndex + placementIndexOffset] = true;
                startIndexDictionary[currentResult] += resultInterval;
                if ((placementIndexOffset >= resultInterval + resultRemainFactor - 1 && resultRemainFactor == 1) ||
                    ((100 % currentResult.percentage) - remainExtensionCountDictionary[currentResult]) ==
                    (currentResult.percentage) - resultAppearCountDictionary[currentResult] && 
                    100 % currentResult.percentage != 0)
                {
                    startIndexDictionary[currentResult]++;
                    remainExtensionCountDictionary[currentResult]++;
                }
                resultAppearCountDictionary[currentResult]++;
                spinResultList.Value[placementIndex + placementIndexOffset] = currentResult.spinResult;

            }

            RemainExtensionCountDictionary = remainExtensionCountDictionary;
        
        
            // Note: In general code should not contain this much comment and explain itself and a bit cleaner maybe.
            // But this function in the whole project represents the algorithm used for probability distribution and
            // it needs explaining. Optimization can be made but since the system itself does not trigger on the
            // gameplay spinning and sets before game starts, I think it is acceptable. Also, other than
            // this function this project does not have any other comments. I believe rest of the project
            // is self explanatory.
        }

        public SpinResult Spin()
        {
            var result = spinResultList.Value[spinIndex];
            spinIndex = (spinIndex + 1) % 100;
            return result;
        }

        public void ResetStates()
        {
            ES3.DeleteKey(SaveKey);
            spinResultList.Value = null;
            spinIndex = 0;
        }

        public void SaveSpinData()
        {
            var spinSave = new SpinSave
            {
                spinResults = spinResultList.Value,
                spinIndex = this.spinIndex
            };
            ES3.Save<SpinSave>(SaveKey,spinSave);
        }

        public void LoadSpinData()
        {
            if (ES3.KeyExists(SaveKey))
            {
                var savedSpinData = ES3.Load<SpinSave>(SaveKey);
                spinResultList.Value = savedSpinData.spinResults;
                spinIndex = savedSpinData.spinIndex;
            }
            else if (spinResultList.Value == null || spinResultList.Value.Length == 0)
            {
                GenerateSpinListNew();        
            }
        
        }
    }

    [Serializable]
    public class SpinData
    {
        public SpinResult spinResult;
        [Range(1,100)]public int percentage;
    }

    [Serializable]
    public struct SpinResult
    {
        public SpinType firstSpin;
        public SpinType secondSpin;
        public SpinType thirdSpin;
    }

    [Serializable]
    public enum SpinType 
    {
        Jackpot = 0,
        Wild = 1,
        Seven = 2,
        Bonus = 3,
        A = 4
    }

    [Serializable]
    public class SpinSave
    {
        public SpinResult[] spinResults;
        public int spinIndex;
    }
}