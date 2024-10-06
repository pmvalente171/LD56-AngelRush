using System;
using System.Collections;
using Game.Util;
using UnityEngine;

namespace Game
{
    public class Gnack : MonoBehaviour
    {
        public static Gnack CurrentGnack;
     
        [HideInInspector] public int gnackId;
        
        public float spring = 0.6f;
        public float damp = 1f;
        
        [Space] public CardSuit cardSuit;
        
        private Camera mainCamera;
        private Collider collider;
        
        [HideInInspector] public Vector3 startPosition;
        [HideInInspector] public Vector3 targetPosition;
        private Vector3 velocity;
        
        private Vector3 startScale;
        private Vector3 targetScale;
        
        private Quaternion startRotation;
        private Quaternion targetRotation;
        
        private Card currentCard;
        
        private bool isDragging;
        
        private IEnumerator Start()
        {
            mainCamera = FindObjectsOfType<Camera>()[0];
            collider = GetComponent<Collider>();
            
            startPosition = transform.position;
            startRotation = transform.rotation;
            
            targetPosition = startPosition;
            targetRotation = startRotation;
            
            startScale = transform.localScale;
            targetScale = startScale;
            
            var finalScale = transform.localScale;
            float f = 0f;
            while (f < 1f)
            {
                f += Time.deltaTime * 2f;
                transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, f);
                yield return null;
            }
            
            transform.localScale = finalScale;
            collider.enabled = true;
        }

        private Vector3 GrabMousePosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 7f;
            mousePos = mainCamera.ScreenToWorldPoint(mousePos);
            return mousePos;
        }
        
        private void DropGnack(Card card)
        {
            if (card == null || (card.WasFlipped && !card.IsHidden))
            {
                if (currentCard is null)
                    return;
                
                currentCard.Count = currentCard.cardData.cardSuit == cardSuit ? 
                    currentCard.Count + 2 : currentCard.Count + 1;
                
                currentCard = null;
                return;
            }
            
            if (card.IsHidden && card.cardData.cardSuit != cardSuit)
            {
                if (currentCard is null)
                    return;
                
                currentCard.Count = currentCard.cardData.cardSuit == cardSuit ? 
                    currentCard.Count + 2 : currentCard.Count + 1;
                
                currentCard = null;
                return;
            }
            
            if (currentCard is not null)
            {
                currentCard.Count = currentCard.cardData.cardSuit == cardSuit ? 
                    currentCard.Count + 2 : currentCard.Count + 1;
            }
            
            card.DropGnack(this);
            
            Vector3 cardPosition = card.GetRandomPosition();
            targetPosition = cardPosition;
            currentCard = card;
            
            float scaleMultiplier = card.cardData.cardSuit == cardSuit ? 0.8f : 0.5f;
            targetScale = startScale * scaleMultiplier;
        }
        
        // on cursor exit
        public void OnMouseUp()
        {
            if (CurrentGnack != this)
                return;
            
            // reset the scale and position
            targetScale = startScale;
            targetPosition = startPosition;
            
            // if (Card.CurrentCard != null)
            DropGnack(Card.CurrentCard);
            
            // reset the rotation
            targetRotation = startRotation;
            
            // reset the current gnack
            CurrentGnack = null;
            isDragging = false;
            collider.enabled = !isDragging;
        }

        // on mouse down
        public void OnMouseDown()
        {
            CurrentGnack = this;
            isDragging = true;
            collider.enabled = !isDragging;
            
            // rotate the gnack towards the camera
            targetRotation = Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up);
        }
        
        private void Update()
        {
            if (isDragging)
            {
                targetPosition = GrabMousePosition();
            }
            
            // spring to target position
            transform.position = MovingUtil.SpringDamp(transform.position, targetPosition, ref velocity, spring, damp);
            
            // spring to target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            
            // spring to target scale
            if (Math.Abs((transform.localScale - targetScale).sqrMagnitude) < 0.005f) return;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
        }
    }
}