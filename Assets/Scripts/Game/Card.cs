using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game
{
    public class Card : MonoBehaviour
    {
        private static Dictionary<CardSuit, String> SuitNames = new() // TODO: Temporary
        {
            {CardSuit.COINS, "Coins"},
            {CardSuit.CUPS, "Cups"},
            {CardSuit.SWORDS, "Swords"},
            {CardSuit.WANDS, "Wands"}
        };
        
        public static Card CurrentCard;
        
        public float spring = 0.1f;
        public float damping = 0.8f;
        
        public TMP_Text cardValueText;
        public List<Transform> possiblePositions;
        
        [Space]
        public CardData cardData;
        public int index;
        
        [HideInInspector] public bool WasFlipped = false;
        
        private Collider collider;
        private Bounds bounds;
        
        private Vector3 velocity;
        private Vector3 targetPosition;
        private Vector3 startPosition;
        
        private Vector3 targetScale;
        private Vector3 startScale;
        
        private Quaternion targetRotation;
        private Quaternion startRotation;
        
        private int count = 0;
        private bool isHidden = false;
        private bool isBeingRemoved = false;
        
        public event Action<Card> CardFlipped;
        public event Action<Card> CardBurnout;
        public event Action<Card> CardVictory;

        public int Count
        {
            get => count;
            set
            {
                count = value;
                UpdateCount();
            }
        }

        public bool IsHidden
        {
            get => isHidden;
            set
            {
                isHidden = value;
                if (isHidden)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    startRotation = transform.rotation;
                    targetRotation = startRotation;
                }
            }
        }

        private void Start()
        {
            collider = GetComponent<Collider>();
            bounds = collider.bounds;
            count = cardData.cardValue;

            startRotation = transform.rotation;
            startScale = transform.localScale;
            targetScale = startScale;
        }
        
        public void Instantiate(CardData cardData, int index, Vector3 startPosition)
        {
            this.cardData = cardData;
            this.index = index;
            this.startPosition = startPosition;
            
            // set card values
            targetPosition = startPosition;
            targetScale = startScale;
            startRotation = transform.rotation;
            cardValueText.text = cardData.cardValue + " of " + SuitNames[cardData.cardSuit];
        }
        
        public Vector3 GetRandomPosition()
        {
            int posIndex = Random.Range(0, possiblePositions.Count);
            var position = possiblePositions[posIndex].position;
            
            possiblePositions.RemoveAt(posIndex);
            return position;
        }

        private void OnMouseOver()
        {
            if (CurrentCard == this)
                return;
            
            CurrentCard = this;
            targetScale = startScale * 1.15f;
        }
        
        private void OnMouseExit()
        {
            if (CurrentCard == this)
            {
                CurrentCard = null;
                targetScale = startScale;
            }
        }

        private void Update()
        {
            
            // spring motion for position
            transform.position = Util.MovingUtil.SpringDamp(transform.position, targetPosition, ref velocity, spring, damping);
            
            // spring motion for rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            
            if (Mathf.Abs((transform.localScale - targetScale).sqrMagnitude) < 0.01f) return;
            if (isBeingRemoved) return;
            
            // spring motion for scale
            transform.localScale = Util.MovingUtil.SpringDamp(transform.localScale, targetScale, ref velocity, spring, damping);
        }
        
        private void OnCardFlipped() => CardFlipped?.Invoke(this);
        private void OnCardBurnout() => CardBurnout?.Invoke(this);
        private void OnCardVictory() => CardVictory?.Invoke(this);
        
        public void Flip()
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, 0, 180);
            OnCardFlipped();
        }
        
        private IEnumerator RemoveCardRoutine()
        {
            yield return new WaitForSeconds(4f);
            Destroy(gameObject);
        }

        public void RemoveCard()
        {
            targetPosition = startPosition + new Vector3(0, 0f, -9.3f);
            isBeingRemoved = true;
            StartCoroutine(RemoveCardRoutine());
        }
        
        private void UpdateCount()
        {
            cardValueText.text = count + " of " + SuitNames[cardData.cardSuit];

            if (count <= 0 && !WasFlipped)
            {
                WasFlipped = true;
                Flip();
            }
            else if (count <= 0 && isHidden)
            {
                Flip();
                OnCardVictory();
            }
        }

        public void DropGnack(Gnack gnackObject)
        {
            if (isHidden && gnackObject.cardSuit != cardData.cardSuit)
            {
                // take burnout
                OnCardBurnout();
                return;
            }
            
            if (isHidden && gnackObject.cardSuit == cardData.cardSuit && !WasFlipped)
            {
                WasFlipped = true;
                Flip();
                return;
            } 
            
            count--;
            if (gnackObject.cardSuit == cardData.cardSuit) count--;
            UpdateCount();
        }
    }
}