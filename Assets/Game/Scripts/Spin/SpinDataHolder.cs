using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Game.Scripts.Spin
{
    [CreateAssetMenu]
    public class SpinDataHolder : ScriptableObject
    {
        public int TryMe = 1;
        public SpinResultList spinResultList;
        public List<SpinData> spinDataList;
        [HideInInspector] public int spinIndex;
        public Action onResetState;
    }

    [Serializable]
    public class SpinData
    {
        public SpinResult spinResult;
        [Range(1, 100)] public int percentage;
    }

    [Serializable]
    public struct SpinResult
    {
        public SpinType firstSpin;
        public SpinType secondSpin;
        public SpinType thirdSpin;

        public bool IsFull() => firstSpin == secondSpin && secondSpin == thirdSpin;
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