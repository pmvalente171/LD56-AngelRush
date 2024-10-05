using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "SuitIconMap", menuName = "Game/SuitIconMap", order = 0)]
    public class SuitIconMapping : ScriptableObject
    {
        [SerializeField] List<Sprite> suitIcons;

        private Sprite GetSuitIcon(int suitIndex)
        {
            return suitIcons[suitIndex];
        }
        
        public Sprite GetSuitIcon(CardSuit suit)=>
            GetSuitIcon((int)suit);
    }
}