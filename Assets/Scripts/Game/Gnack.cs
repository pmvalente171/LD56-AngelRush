using System;
using System.Collections;
using Game.UI;
using Game.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [Serializable]
    public enum ArcanaType
    {
        NONE,
        KNIGHT,
        QUEEN,
        KING,
        PAGE
    }
    
    public class Gnack : MonoBehaviour
    {
        public static Gnack CurrentGnack;
     
        [HideInInspector] public int gnackId;
        [HideInInspector] public Card currentCard;
        
        public float TimeToLive = 120f;
        public TMP_Text gnackTimerText;
        public TMP_Text gnackNameText;
        [Space]
        public float spring = 0.6f;
        public float damp = 1f;
        
        [Space] public string gnackName;
        public CardSuit cardSuit;
        public ArcanaType arcanaType;
        [TextArea] public string gnackDescription;
        
        private Camera mainCamera;
        [HideInInspector] public Collider gnackCollider;
        
        [HideInInspector] public bool isOnCard;
        
        [HideInInspector] public Vector3 startPosition;
        [HideInInspector] public Vector3 targetPosition;
        private Vector3 velocity;
        
        [HideInInspector] public Vector3 startScale;
        [HideInInspector] public Vector3 targetScale;
        
        private Quaternion startRotation;
        private Quaternion targetRotation;
        
        private bool isDragging;
        private Coroutine gnackTimer;
        private UnitDescription unitDescription;
        private float f; 
        
        public bool isOnSwap = false;
        
        private IEnumerator GnackTimer()
        {
            if (isOnSwap) yield break;
            
            f = 0f;
            while (f < 1f)
            {
                f += Time.deltaTime / TimeToLive;
                gnackTimerText.text = $"{(int) (TimeToLive - TimeToLive * f)}";
                yield return null;
            }
            
            gnackTimer = null;
            var encounterManager = FindObjectOfType<EncounterManager>();
            encounterManager.Burnout(gnackId);
            encounterManager.VerifyDeath();
        }
        
        public void StopTimer()
        {
            if (gnackTimer == null)
                return;
            
            StopCoroutine(gnackTimer);
            gnackTimer = null;
        }
        
        public void StartTimer()
        {
            gnackTimer = StartCoroutine(GnackTimer());
        }
        
        private IEnumerator Start()
        {
            mainCamera = FindObjectsOfType<Camera>()[0];
            unitDescription = FindObjectOfType<UnitDescription>();
            gnackCollider = GetComponent<Collider>();
            
            startPosition = transform.position;
            startRotation = transform.rotation;
            
            targetPosition = startPosition;
            targetRotation = startRotation;
            
            startScale = transform.localScale;
            targetScale = startScale;
            
            this.f = 0f;
            gnackTimerText.text = $"{(int)TimeToLive}";
            
            var finalScale = transform.localScale;
            float f = 0f;
            while (f < 1f)
            {
                f += Time.deltaTime * 2f;
                transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, f);
                yield return null;
            }
            
            transform.localScale = finalScale;
            gnackTimer = StartCoroutine(GnackTimer());
        }

        public void UpdateName() =>
            gnackNameText.text = Card.SuitNames[cardSuit];

        private Vector3 GrabMousePosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 7f;
            mousePos = mainCamera.ScreenToWorldPoint(mousePos);
            return mousePos;
        }
        
        private void DropGnack(Card card)
        {
            int ammount = currentCard?.cardData.cardSuit == cardSuit ? 2 : 1;
            Gnack knight = null;
            bool isKnightOnCard = currentCard != null && currentCard.IsKnightOnCard(out knight);
            
            if (currentCard != null && arcanaType == ArcanaType.KNIGHT)
                ammount += currentCard.activeGnacks.Count - 1; // minus 1 because the knight itself is already on the card
            else if (arcanaType == ArcanaType.PAGE)
                ammount *= 2;
            else if (arcanaType == ArcanaType.QUEEN)
                ammount *= 0;
            else if (arcanaType == ArcanaType.KING) // need to verify if this makes sense
                ammount *= 0;
            
            if (isKnightOnCard && knight != this)
                ammount += 1;
            
            if (card == null || (card.WasFlipped && !card.IsHidden))
            {
                if (currentCard is null)
                    return;
                
                currentCard.Count += ammount; 
                currentCard.activeGnacks.Remove(this);
                
                isOnCard = false;
                currentCard = null;
                return;
            }
            
            if (card.IsHidden && !card.WasFlipped && card.cardData.cardSuit != cardSuit)
            {
                card.OnCardBurnout();
                if (currentCard is null)
                    return;
                
                currentCard.Count += ammount;
                currentCard.activeGnacks.Remove(this);
                
                isOnCard = false;
                currentCard = null;
                return;
            }
            
            if (currentCard is not null)
            {
                currentCard.Count += ammount; 
                currentCard.activeGnacks.Remove(this);
            }
            
            isOnCard = true;
            card.activeGnacks.Add(this);
            
            Vector3 cardPosition = card.GetRandomPosition();
            targetPosition = cardPosition;
            currentCard = card;
            
            float scaleMultiplier = card.cardData.cardSuit == cardSuit ? 0.8f : 0.5f;
            targetScale = startScale * scaleMultiplier;
            card.DropGnack(this);
        }
        
        // on cursor exit
        public void OnMouseUp()
        {
            if (CurrentGnack != this)
                return;
            
            // reset the scale and position
            targetScale = startScale;
            targetPosition = startPosition;
            unitDescription.Hide();
            
            // if (Card.CurrentCard != null)
            DropGnack(Card.CurrentCard);
            
            // reset the rotation
            targetRotation = startRotation;
            
            // reset the current gnack
            CurrentGnack = null;
            isDragging = false;
            gnackCollider.enabled = !isDragging;
            
            // verify gnack swap
            if (SwapGnack.CurrentGnackSwap != null)
                SwapGnack.CurrentGnackSwap.Swap(this);
        }

        // on mouse down
        public void OnMouseDown()
        {
            CurrentGnack = this;
            isDragging = true;
            gnackCollider.enabled = !isDragging;
            
            // update the the unit description
            unitDescription.SetValue(gnackName.ToUpper(), gnackDescription);
            
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