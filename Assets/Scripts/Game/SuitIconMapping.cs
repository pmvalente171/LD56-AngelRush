using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "SuitIconMap", menuName = "Game/SuitIconMap", order = 0)]
    public class SuitIconMapping : ScriptableObject
    {
        [SerializeField] List<Material> suitIcons;

        private Material GetSuitIcon(int suitIndex)
        {
            return suitIcons[suitIndex];
        }
        
        public Material GetSuitIcon(CardSuit suit)=>
            GetSuitIcon((int)suit);
    }
}