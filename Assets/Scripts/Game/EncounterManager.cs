using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class EncounterManager : MonoBehaviour
    {
        public Card CardPrefab;
        public List<Gnack> GnackPrefabs;
        
        [Space]
        public Board board;
        
        [Space]
        public Encounter InitialEncounter;
        public List<Encounter> encounters = new ();
        public Encounter FinalEncounter;
        
        private Queue<Encounter> encounterQueue = new ();
        private CardDeck cardDeck;
        
        private List<CardData> activeCardData = new ();
        private Encounter currentEncounter;
        
        
        private List<Gnack> activeGnacks = new ();
        private List<Card> activeCards = new ();
        private Card hiddenCard;
        
        public void Start()
        {
            cardDeck = new CardDeck();
            cardDeck.Shuffle();
            
            encounterQueue = new Queue<Encounter>();
            
            int count = Mathf.Min(encounters.Count, 7);
            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, encounters.Count);
                encounterQueue.Enqueue(encounters[index]);
                encounters.RemoveAt(index);
            }
            
            SwitchEncounter(InitialEncounter);
        }
        
        private Gnack GetRandomGnack()
        {
            int index = Random.Range(0, GnackPrefabs.Count);
            return GnackPrefabs[index];
        }
        
        private IEnumerator SpawnCards()
        {
            Card card;
            for (int i = 0; i < activeCardData.Count; i++)
            {
                CardData cardData = activeCardData[i];
                Vector3 position = board.cards[i].position;
                card = Instantiate(CardPrefab, position + Vector3.forward * 7f, Quaternion.identity);
                card.Instantiate(cardData, i, position);
                activeCards.Add(card);
                
                yield return new WaitForSeconds(0.5f);
            }
            
            // instance a flipped card
            card = Instantiate(CardPrefab, board.hidenCard.position + Vector3.forward * 7f, Quaternion.Euler(0, 0f, 180f));
            card.Instantiate(currentEncounter.hiddenCardData, 4, board.hidenCard.position);
            card.IsHidden = true;
            hiddenCard = card;
        }
        
        private IEnumerator SpawnGnacks()
        {
            for (int i = activeGnacks.Count; i < 7; i++)
            {
                Gnack gnack = Instantiate(GetRandomGnack(), board.gnacks[i].position, Quaternion.identity);
                gnack.gnackId = i;
                activeGnacks.Add(gnack);
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        private IEnumerator DestroyGnack(Gnack gnack)
        {;
            yield return new WaitForSeconds(1f);
            Destroy(gnack.gameObject);
        }
        
        public void KillGnack(int gnackId)
        {
            Gnack gnack = activeGnacks.Find(g => g.gnackId == gnackId);
            if (gnack == null)
                return;
            
            activeGnacks.Remove(gnack);
            
            // reorganize gnack ids
            for (int i = 0; i < activeGnacks.Count; i++)
            {
                activeGnacks[i].gnackId = i;
                activeGnacks[i].startPosition = board.gnacks[i].position;
                activeGnacks[i].targetPosition = board.gnacks[i].position;
            }
            
            StartCoroutine(DestroyGnack(gnack));
        }
        
        private IEnumerator DrawCardsRoutine()
        {
            int count = activeCards is null ? 0 : activeCards.Count;
            for (int i = 0; i < count; i++)
            {
                activeCards[i].RemoveCard();
            }
            
            activeCardData.Clear();
            activeCards.Clear();
            
            yield return new WaitForSeconds(2f);
            
            for (int i = 0; i < 4; i++)
            {
                activeCardData.Add(cardDeck.Draw());
            }
            
            StartCoroutine(SpawnCards());
            StartCoroutine(SpawnGnacks());
        }

        private void DrawCards()
        {
            StartCoroutine(DrawCardsRoutine());
        }
        
        public void SwitchEncounter(Encounter newEncounter)
        {
            currentEncounter = newEncounter;
            
            activeCardData.Clear();
            DrawCards();
            
            // other stuff:
            // - update UI
            // - update game state
        }
        
        
    }
}