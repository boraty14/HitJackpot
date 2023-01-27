using System.Collections.Generic;
using Game.Scripts.Effects;
using UnityEngine;

namespace Game.Scripts.SlotElement
{
    public class SlotMachine : MonoBehaviour
    {
        public List<Slot> slots;
        public CoinEffect coinEffectPrefab;
        public Transform coinEffectTransform;
    }
}