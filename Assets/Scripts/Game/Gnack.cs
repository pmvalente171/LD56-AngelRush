using System;
using System.Collections;
using System.Collections.Generic;
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
        
        public AudioSource gnackSpawn;
        public AudioSource gnackNotif;
        
        [Space] public float TimeToLive = 120f;
        public Transform gnackVisualTimer;
        public TMP_Text WarningText;
        
        [Space]
        public float spring = 0.6f;
        public float damp = 1f;
        
        [Space] public string gnackName;
        public ArcanaType arcanaType;
        [TextArea] public string gnackDescription;
        
        private Camera mainCamera;
        [HideInInspector] public Collider gnackCollider;
        
        [HideInInspector] public bool isOnCard;
        [HideInInspector] public float timeToLive;
        
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
        
        private Vector3 startBarScale;
        private float f; 
        
        public bool isOnSwap = false;
        public bool isOnWarning = false;
        
        
        public void SpawnSound(float volume = 1f)
        {
            // slight variation in pitch
            var pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            
            // play the sound
            gnackSpawn.volume = volume;
            AudioUtil.PlayOneShot(gnackSpawn, pitch);
        }
        
        public void NotifSound()
        {
            // slight variation in pitch
            var pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            
            // play the sound
            AudioUtil.PlayOneShot(gnackNotif, pitch);
        }
        
        private IEnumerator GnackTimer()
        {
            if (isOnSwap) yield break;
            
            startBarScale = gnackVisualTimer.localScale;
            Vector3 finalScale = new Vector3(0, startBarScale.y, startBarScale.z);
            
            f = 0f;
            while (f < 1f)
            {
                f += Time.deltaTime / timeToLive;
                gnackVisualTimer.localScale = Vector3.Lerp(startBarScale, finalScale, f);
                
                if (f > 0.66f && !isOnWarning)
                {
                    var tempStarScale = WarningText.transform.localScale;
                    var tempF = 0f;
                    
                    while (tempF < 1f)
                    {
                        tempF += Time.deltaTime / 2f;
                        WarningText.transform.localScale = Vector3.Lerp(Vector3.zero, tempStarScale, tempF);
                        yield return null;
                    }
                    
                    isOnWarning = true;
                    NotifSound();
                    WarningText.gameObject.SetActive(true);
                }
                
                yield return null;
            }
            
            gnackTimer = null;
            var encounterManager = FindObjectOfType<EncounterManager>();
            encounterManager.Timeout(gnackId);
            encounterManager.TakeDamage();
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
            
            timeToLive = TimeToLive;
            timeToLive -= 10f * EncounterManager.encouterCount / 4f;
            timeToLive = Mathf.Clamp(timeToLive, 10f, 120f);
            
            this.f = 0f;
            var finalScale = transform.localScale;
            SpawnSound();
            
            float f = 0f;
            while (f < 1f)
            {
                f += Time.deltaTime / 0.3f;
                transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, f);
                yield return null;
            }
            
            transform.localScale = finalScale;
            gnackTimer = StartCoroutine(GnackTimer());
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
            int ammount =  1;
            Gnack knight = null;
            SpawnSound(0.6f);
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
            
            float scaleMultiplier = 0.5f;
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
            
            // if is on warning
            if (isOnWarning)
            {
                float sin = Mathf.Sin(Time.time * 10f) * 0.5f;
                WarningText.transform.localScale = new Vector3(1f + sin, 1f + sin, 1f + sin);
            }
        }
    }
}