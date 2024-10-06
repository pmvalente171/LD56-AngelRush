﻿using System;
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
        
        private int flipCount = 0;
        
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
                card.CardBurnout += OnCardBurnout;
                card.CardFlipped += OnCardFlipped;
                activeCards.Add(card);
                
                yield return new WaitForSeconds(0.5f);
            }
            
            // instance a flipped card
            card = Instantiate(CardPrefab, board.hidenCard.position + Vector3.forward * 7f, Quaternion.Euler(0, 0f, 180f));
            card.Instantiate(currentEncounter.hiddenCardData, 4, board.hidenCard.position);
            card.IsHidden = true;
            card.CardVictory += OnCardVictory;
            hiddenCard = card;
        }
        
        private IEnumerator SpawnGnacks(int ammount = 7)
        {
            int max = Mathf.Min(activeGnacks.Count + ammount, board.gnacks.Count);
            for (int i = activeGnacks.Count; i < max; i++)
            {
                Gnack gnack = Instantiate(GetRandomGnack(), board.gnacks[i].position, Quaternion.identity);
                gnack.gnackId = i;
                activeGnacks.Add(gnack);
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator DestroyGnack(Gnack gnack)
        {
            Destroy(gnack.gameObject);
            yield return new WaitForSeconds(1f);
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
                if (!activeGnacks[i].isOnCard)
                    activeGnacks[i].targetPosition = board.gnacks[i].position;
            }
            
            StartCoroutine(DestroyGnack(gnack));
        }
        
        private void Burnout(int gnackId)
        {
            // permanently remove the gnacks location
            board.gnacks.RemoveAt(gnackId);
            
            if (board.gnacks.Count == 0)
            {
                // end the game
                Debug.Log("Game Over");
            }
            
            // kill the gnack
            KillGnack(gnackId);   
        }

        public void Burnout() => Burnout(Random.Range(0, activeGnacks.Count));
        
        private IEnumerator DrawCardsRoutine()
        {
            int count = activeCards is null ? 0 : activeCards.Count;
            for (int i = 0; i < count; i++)
            {
                activeCards[i].RemoveCard();
            }
            
            hiddenCard?.RemoveCard();
            activeCardData.Clear();
            activeCards.Clear();
            
            // reset flip count
            flipCount = 0;
            
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
        
        public void NextEncounter()
        {
            if (encounterQueue.Count == 0)
            {
                SwitchEncounter(FinalEncounter);
                return;
            }
            
            SwitchEncounter(encounterQueue.Dequeue());
        }
        
        public void OnCardVictory(Card card) => NextEncounter();
        
        public void OnCardBurnout(Card card) => Burnout();
        
        public void OnCardFlipped(Card card)
        {            
            flipCount++;
            
            if (card.IsHidden)
                return;

            // kill them all!!!
            foreach (var gnack in card.activeGnacks)
                KillGnack(gnack.gnackId);
            
            // aaand a new gnack appears :X
            StartCoroutine(SpawnGnacks(1));
            
            if (flipCount == 4)
            {
                hiddenCard.Flip();
                NextEncounter();
            }
        }
    }
}