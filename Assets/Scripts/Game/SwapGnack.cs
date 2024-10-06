using System;
using UnityEngine;

namespace Game
{
    public class SwapGnack : MonoBehaviour
    {
        public static SwapGnack CurrentGnackSwap;
        
        [HideInInspector] public Gnack gnack;
        [HideInInspector] public Vector3 targetScale;
        [HideInInspector] public Vector3 startScale;
        
        public event Action<Gnack> GnackSwaped;
        
        private void Awake()
        {
            startScale = transform.localScale;
            targetScale = startScale;
        }
        
        public void SetGnack(Gnack newGnack)
        {
            newGnack.isOnSwap = true;
            
            gnack = newGnack;
            newGnack.startPosition = transform.position; // TODO: Fix this
            newGnack.targetPosition = transform.position; // TODO: Fix this
            
            if (newGnack.gnackCollider is null)
                newGnack.gnackCollider = newGnack.GetComponent<Collider>();
            
            newGnack.isOnSwap = true;
            newGnack.gnackCollider.enabled = false;
            newGnack.StopTimer();
        }
        
        public void Swap(Gnack other)
        {
            gnack.startPosition = other.startPosition;
            gnack.targetPosition = other.startPosition;
            gnack.gnackCollider.enabled = true;
            gnack.gnackId = other.gnackId;
            
            gnack.isOnSwap = false;
            gnack.StartTimer();
            
            OnGnackSwapped(gnack);
            SetGnack(other);
        }
        
        private void OnMouseOver()
        {
            if (CurrentGnackSwap == this)
                return;
            
            CurrentGnackSwap = this;
            targetScale = startScale * 1.15f;
        }
        
        private void OnMouseExit()
        {
            if (CurrentGnackSwap == this)
            {
                CurrentGnackSwap = null;
                targetScale = startScale;
            }
        }
        
        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10);
        }

        protected virtual void OnGnackSwapped(Gnack obj) => GnackSwaped?.Invoke(obj);
        
    }
}