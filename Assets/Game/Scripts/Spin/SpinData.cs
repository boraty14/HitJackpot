using System;
using UnityEngine;

namespace Game.Scripts.Spin
{
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