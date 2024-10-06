using System;
using System.Collections;
using System.Collections.Generic;
using Game.Encounters;
using Game.UI;
using Game.Util;
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
        public GameObject CrossPrefab;
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
        private List<DestroyedSlot> destroyedSlots = new();
        private Card hiddenCard;

        private Queue<Hint> hintQueue = new();

        private int flipCount = 0;
        private int encouterCount = 0;

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
        
        public void RestoreRandomSlot()
        {
            if (destroyedSlots.Count == 0)
                return;

            int index = Random.Range(0, destroyedSlots.Count);
            var slot = destroyedSlots[index];
            destroyedSlots.RemoveAt(index);
            
            slot.cross.SetActive(false);
            board.gnacks.Insert(index, slot.slot);
            
            // reorganize gnack ids
            for (int i = 0; i < activeGnacks.Count; i++)
            {
                activeGnacks[i].gnackId = i;
                activeGnacks[i].startPosition = board.gnacks[i].position;
                if (!activeGnacks[i].isOnCard)
                    activeGnacks[i].targetPosition = board.gnacks[i].position;
            }
            
            // spawn a new gnack
            StartCoroutine(SpawnGnacks(1));
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
                card.CardBurnout += OnCardBurnout;
                card.CardFlipped += OnCardFlipped;
                activeCards.Add(card);

                yield return new WaitForSeconds(0.5f);
            }

            // instance a flipped card
            card = Instantiate(CardPrefab, board.hidenCard.position + Vector3.forward * 7f,
                Quaternion.Euler(0, 0f, 180f));
            card.Instantiate(currentEncounter.hiddenCardData, 4, board.hidenCard.position);
            card.IsHidden = true;
            card.CardVictory += OnCardVictory;
            card.CardBurnout += OnCardBurnout;
            hiddenCard = card;
        }

        private IEnumerator SpawnGnacks(int amount = 7)
        {
            // why am i getting index out of range errors?
            int max = Mathf.Min(activeGnacks.Count + amount, board.gnacks.Count);

            for (int i = activeGnacks.Count; i < max; i++)
            {
                Gnack gnack = Instantiate(GetRandomGnack(), board.gnacks[i].position, Quaternion.identity);
                gnack.gnackId = i;
                if (gnack.arcanaType != ArcanaType.NONE)
                {
                    gnack.cardSuit = DiceUtil.DCardSuit();
                    gnack.UpdateName();
                }

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

        public void Burnout(int gnackId)
        {
            // permanently remove the gnacks location
            Instantiate(CrossPrefab, board.gnacks[gnackId].position, Quaternion.identity);
            board.gnacks.RemoveAt(gnackId);

            // get the gnack
            Gnack gnack = activeGnacks.Find(g => g.gnackId == gnackId);
            if (gnack is null)
                return;
            
            // kill the gnack
            KillGnack(gnackId);
            bool flipCard = gnack.isOnCard;
            if (gnack.isOnCard)
            {
                // remove the gnacks from the card
                // gnack.currentCard.Count = 0;
                
                // reset the gnacks
                for (int i = 0; i < activeGnacks.Count; i++)
                {
                    activeGnacks[i].targetPosition = activeGnacks[i].startPosition;
                    activeGnacks[i].targetScale = activeGnacks[i].startScale;
                    activeGnacks[i].isOnCard = false;
                    activeGnacks[i].currentCard = null;
                }
            }

            if (flipCard) gnack.currentCard.Count = 0;
            VerifyDeath();
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
            hintQueue = newEncounter.GetHints();

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
            // int halfCardValue = Mathf.CeilToInt(card.cardData.cardValue / 2f); // BALANCING
            StartCoroutine(SpawnGnacks(gnacksInCard));

            // reset all the gnacks
            ResetGnacks();
            NextEncounter();
        }

        public void OnCardBurnout(Card card) => Burnout();

        public void OnCardFlipped(Card card)
        {
            flipCount++;

            if (card.IsHidden)
                return;

            bool isQueenOnCard = card.IsQueenOnCard(out var queen);
            int gnacksInCard = card.activeGnacks.Count;

            // if there is a queen on the card save a gnack
            if (isQueenOnCard)
            {
                // TODO HEAL THE PLAYER
                RestoreRandomSlot();
                
                // if the queen as the same suit as the card, heal the player
                if (queen.cardSuit == card.cardData.cardSuit)
                    RestoreRandomSlot();
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

                        int ammout = card.cardData.cardSuit == gnack.cardSuit ? 2 : 1; // BALANCING
                        otherCard.Count -= ammout;
                    }

                    // damage the hidden card
                    if (hiddenCard.Count > 0 && hiddenCard.WasFlipped)
                    {
                        int ammout = card.cardData.cardSuit == gnack.cardSuit ? 2 : 1; // BALANCING
                        hiddenCard.Count -= ammout;
                    }
                }
                
                // kill the gnack
                KillGnack(gnack.gnackId);
            }

            // aaand a new gnack appears :X
            // int halfCardValue = Mathf.CeilToInt(card.cardData.cardValue / 2f); // BALANCING
            StartCoroutine(SpawnGnacks(gnacksInCard));

            if (flipCount == 4)
            {
                hiddenCard.Flip(true);
                NextEncounter();
            }
        }

        public void VerifyDeath()
        {
            if (activeGnacks.Count == 0)
            {
                // game over
                print("Game Over");
            }
        }
    }
}