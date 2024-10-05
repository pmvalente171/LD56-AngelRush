using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class CardObject : MonoBehaviour
    {
        public static CardObject CurrentCardObject;
        
        public Card card;
        public int index;
        
        private Collider collider;
        private Bounds bounds;
        
        private Vector3 velocity;
        private Vector3 targetPosition;
        private Vector3 startPosition;
        
        private Quaternion targetRotation;
        private Quaternion startRotation;
        
        
        private int count = 0;
        
        public event Action<CardObject> CardFlipped;
        
        public int Count
        {
            get => count;
        }
        
        private void Start()
        {
            collider = GetComponent<Collider>();
            bounds = collider.bounds;
            count = card.cardValue;
            
            startRotation = transform.rotation;
        }
        
        public void Instantiate(Card card, int index, Vector3 startPosition)
        {
            this.card = card;
            this.index = index;
            this.startPosition = startPosition;
            targetPosition = startPosition;
        }

        private void OnMouseOver()
        {
            if (CurrentCardObject == this)
                return;
            
            CurrentCardObject = this;
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
            if (CurrentCardObject == this)
                CurrentCardObject = null;
        }

        private void Update()
        {
            print(CurrentCardObject);
            
            // spring motion for position
            transform.position = Util.MovingUtil.SpringDamp(transform.position, targetPosition, ref velocity, 1f, 1f);
            
            // spring motion for rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        
        private void OnCardFlipped() => CardFlipped?.Invoke(this);
        
        public void Flip()
        {
            targetRotation = Quaternion.Euler(0, 180, 0);
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
            if (CurrentCardObject != this)
                return;
            
            count--;
            if (gnackObject.cardSuit == card.cardSuit) count--;
            if (count == 0) Flip();
        }
    }
}