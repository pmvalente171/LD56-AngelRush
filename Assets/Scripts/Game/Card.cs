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
        
        [Space]
        public CardData cardData;
        public int index;
        
        private Collider collider;
        private Bounds bounds;
        
        private Vector3 velocity;
        private Vector3 targetPosition;
        private Vector3 startPosition;
        
        private Quaternion targetRotation;
        private Quaternion startRotation;
        
        private int count = 0;
        private bool isHidden = false;
        
        public event Action<Card> CardFlipped;
        
        public int Count => count;

        public bool IsHidden
        {
            set => isHidden = value;
        }

        private void Start()
        {
            collider = GetComponent<Collider>();
            bounds = collider.bounds;
            count = cardData.cardValue;
            
            startRotation = transform.rotation;
        }
        
        public void Instantiate(CardData cardData, int index, Vector3 startPosition)
        {
            this.cardData = cardData;
            this.index = index;
            this.startPosition = startPosition;
            targetPosition = startPosition;
            
            cardValueText.text = cardData.cardValue + " of " + SuitNames[cardData.cardSuit];
        }

        private void OnMouseOver()
        {
            if (CurrentCard == this)
                return;
            
            CurrentCard = this;
        }
        
        public List<Vector3> GetPoints(int nbOfPoints=10)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            
            List<Vector3> points = new();
            for (int i = 0; i < nbOfPoints; i++)
            {
                float x = Random.Range(center.x - extents.x, center.x + extents.x);
                float y = Random.Range(center.y - extents.y, center.y + extents.y);
                points.Add(new Vector3(x, y, 0));
            }
            
            return points;
        }
        
        private void OnMouseExit()
        {
            if (CurrentCard == this)
                CurrentCard = null;
        }

        private void Update()
        {
            // spring motion for position
            transform.position = Util.MovingUtil.SpringDamp(transform.position, targetPosition, ref velocity, spring, damping);
            
            // spring motion for rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        
        private void OnCardFlipped() => CardFlipped?.Invoke(this);
        
        public void Flip()
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, 0, 180);
            OnCardFlipped();
        }
        
        private IEnumerator RemoveCardRoutine()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

        public void RemoveCard()
        {
            targetPosition = startPosition + new Vector3(0, 0, -20f);
            StartCoroutine(RemoveCardRoutine());
        }

        public void DropGnack(Gnack gnackObject)
        {
            if (CurrentCard != this)
                return;
            
            count--;
            if (gnackObject.cardSuit == cardData.cardSuit) count--;
            if (count == 0) Flip();
        }
    }
}