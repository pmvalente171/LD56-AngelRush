using System;
using System.Collections.Generic;
using Game.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class EncounterManager : MonoBehaviour
    {
        
        public Encounter InitialEncounter;
        public List<Encounter> encounters;
        public Encounter FinalEncounter;
        
        private Queue<Encounter> encounterQueue;
        private CardDeck cardDeck;
        
        private List<Card> activeCards;
        private Encounter currentEncounter;
        
        public void Start()
        {
            cardDeck = new CardDeck();
            cardDeck.Shuffle();
            
            encounterQueue = new Queue<Encounter>();
            for (int i = 0; i < 7; i++)
            {
                int index = Random.Range(0, encounters.Count);
                encounterQueue.Enqueue(encounters[index]);
                encounters.RemoveAt(index);
            }
            
            SwitchEncounter(InitialEncounter);
        }
        
        private void DrawCards()
        {
            for (int i = 0; i < 4; i++)
            {
                activeCards.Add(cardDeck.Draw());
            }
        }
        
        public void SwitchEncounter(Encounter newEncounter)
        {
            currentEncounter = newEncounter;
            
            activeCards.Clear();
            DrawCards();
            
            // other stuff:
            // - update UI
            // - update game state
        }
        
        
    }
}