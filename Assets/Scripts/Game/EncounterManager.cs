﻿using System;
using System.Collections;
using System.Collections.Generic;
using Game.Encounters;
using Game.UI;
using Game.Util;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public struct DestroyedSlot
    {
        public int slotId;
        public Transform slot;
        public GameObject cross;
    }
    
    public class EncounterManager : MonoBehaviour
    {
        public int maxHp = 3;

        [Space] public TMP_Text hpText;
        public ScoreManager scoreManager;
        
        [Space] public GameObject CrossPrefab;
        public Card CardPrefab;
        public List<Gnack> GnackPrefabs;
        public List<Gnack> GnackArcanaPrefabs;

        [Space] public Board board;
        public SwapGnack swapGnack;

        [Space] public Encounter InitialEncounter;
        public List<Encounter> encounters = new();
        public Encounter FinalEncounter;

        private Queue<Encounter> encounterQueue = new();
        private CardDeck cardDeck;

        private List<CardData> activeCardData = new();
        private Encounter currentEncounter;

        private List<Gnack> activeGnacks = new();
        private List<Card> activeCards = new();


        private int flipCount = 0;
        private int encouterCount = 0;
        private int currentHp = 3;

        public void Start()
        {
            cardDeck = new CardDeck();
            encounterQueue = new Queue<Encounter>();

            int count = Mathf.Min(encounters.Count, 7);
            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, encounters.Count);
                encounterQueue.Enqueue(encounters[index]);
                encounters.RemoveAt(index);
            }

            SwitchEncounter(InitialEncounter);
            currentHp = maxHp;
            
            // instance a gnack on the swap gnack
            Gnack gnackToSwap = Instantiate(GetRandomGnack(), swapGnack.transform.position, Quaternion.identity);
            gnackToSwap.gnackId = -1;
            gnackToSwap.gnackCollider = gnackToSwap.GetComponent<Collider>();
            
            swapGnack.SetGnack(gnackToSwap);
            swapGnack.GnackSwaped += gnack =>
            {
                activeGnacks[gnack.gnackId] = gnack;
            };
        }
        
        public void RestoreHp()
        {
            currentHp = Mathf.Min(maxHp, currentHp + 1);
            hpText.text = $"HP: {currentHp}";
        }

        private Gnack GetRandomGnack()
        {
            // TODO: Implement Arcana gnacks
            int chance = 30 + encouterCount * 2;
            chance = Mathf.Clamp(chance, 15, 30);
            var gnackList = DiceUtil.D100() > chance ? GnackPrefabs : GnackArcanaPrefabs; // BALANCING

            int index = Random.Range(0, gnackList.Count);
            return gnackList[index];
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
                card.CardFlipped += OnCardFlipped;
                activeCards.Add(card);

                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator SpawnGnacks(int amount = 7)
        {
            // why am i getting index out of range errors?
            int max = Mathf.Min(activeGnacks.Count + amount, board.gnacks.Count);

            for (int i = activeGnacks.Count; i < max; i++)
            {
                Gnack gnack = Instantiate(GetRandomGnack(), board.gnacks[i].position, Quaternion.identity);
                gnack.gnackId = i;

                activeGnacks.Add(gnack);
                max = Mathf.Min(max, board.gnacks.Count); // TODO: cop out
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
            if (gnack is null)
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

        public void Timeout(int gnackId)
        {
            // get the gnack
            Gnack gnack = activeGnacks.Find(g => g.gnackId == gnackId);
            if (gnack is null)
                return;
            
            // kill the gnack
            KillGnack(gnackId);
            
            // Spawn a new gnack
            StartCoroutine(SpawnGnacks(1));
        }

        public void Timeout() => Timeout(Random.Range(0, activeGnacks.Count));

        private IEnumerator DrawCardsRoutine()
        {
            int count = activeCards is null ? 0 : activeCards.Count;
            for (int i = 0; i < count; i++)
            {
                activeCards[i].RemoveCard();
            }

            activeCardData.Clear();
            activeCards.Clear();

            // reset flip count
            flipCount = 0;

            yield return new WaitForSeconds(2f);

            // BALANCING
            if (encouterCount % 2 == 0)
            {
                cardDeck.cardValueRange++; // increase the card value range every 2 encounters
            }

            for (int i = 0; i < 4; i++)
            {
                activeCardData.Add(cardDeck.Draw());
            }

            StartCoroutine(SpawnCards());
            if (currentEncounter == InitialEncounter)
                StartCoroutine(SpawnGnacks()); // BALANCING

        }

        private void DrawCards()
        {
            StartCoroutine(DrawCardsRoutine());
        }

        public void SwitchEncounter(Encounter newEncounter)
        {
            encouterCount++;
            currentEncounter = newEncounter;
            activeCardData.Clear();
            DrawCards();
        }

        public void NextEncounter()
        {
            if (encounterQueue.Count == 0)
            {
                SwitchEncounter(FinalEncounter); // TODO: Fix this lmao
                return;
            }

            SwitchEncounter(encounterQueue.Dequeue());
        }

        private void ResetGnacks()
        {
            foreach (var gnack in activeGnacks)
            {
                gnack.targetPosition = gnack.startPosition;
                gnack.targetScale = gnack.startScale;
                gnack.isOnCard = false;
                gnack.currentCard = null;
            }
        }

        public void OnCardVictory(Card card)
        {
            int gnacksInCard = card.activeGnacks.Count;

            // kill them all!!!
            foreach (var gnack in card.activeGnacks)
                KillGnack(gnack.gnackId);

            // aaand a new gnack appears :X
            StartCoroutine(SpawnGnacks(gnacksInCard));

            // reset all the gnacks
            ResetGnacks();
            NextEncounter();
        }

        public void OnCardFlipped(Card card)
        {
            flipCount++;
            scoreManager.AddScore("Card Flipped", 1, 4f); // BALANCING
            scoreManager.VerifyAndStartCombo("CARD_FLIPPED", new ComboString("Flipping COMBO!", 1, 4f));

            if (card.IsHidden)
                return;

            bool isQueenOnCard = card.IsQueenOnCard(out var queen);
            int gnacksInCard = card.activeGnacks.Count;

            // if there is a queen on the card save a gnack
            if (isQueenOnCard)
            {
                RestoreHp();
            }

            // kill them all!!!
            foreach (var gnack in card.activeGnacks)
            {
                if (gnack.arcanaType == ArcanaType.KING)
                {
                    // damage all the other cards
                    foreach (var otherCard in activeCards)
                    {
                        if (otherCard == card)
                            continue;

                        if (otherCard.Count <= 0)
                            continue;

                        int ammout = 1; 
                        otherCard.Count -= ammout;
                    }
                }
                
                // kill the gnack
                KillGnack(gnack.gnackId);
            }

            // aaand a new gnack appears :X
            StartCoroutine(SpawnGnacks(gnacksInCard));

            if (flipCount == 4)
            {
                scoreManager.AddScore("Victory", 3, 10f); // BALANCING
                scoreManager.VerifyAndStartCombo("COMPLETE", new ComboString("Vicotry COMBO!", 5, 10f));
                NextEncounter();
            }
        }
        
        public void TakeDamage(int damage)
        {
            currentHp -= damage;
            hpText.text = $"HP: {currentHp}";
            if (currentHp <= 0)
            {
                // game over
                currentHp = 0;
                print("Game Over");
                hpText.text = $"HP: {currentHp}";
            }
        }
        
        public void TakeDamage() => TakeDamage(1);

    }
}