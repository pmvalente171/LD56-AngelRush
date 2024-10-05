using System;
using UnityEngine;

namespace Game
{
    public class EncounterManager : MonoBehaviour
    {
        public CardDeck cardDeck;

        public void Start()
        {
            cardDeck = new CardDeck();
            cardDeck.Shuffle();
        }
    }
}