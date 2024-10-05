using System;
using Game.Util;
using UnityEngine;

namespace Game
{
    public class Gnack : MonoBehaviour
    {
        public static Gnack CurrentGnack;

        public float spring = 0.6f;
        public float damp = 1f;
        
        private Camera mainCamera;
        
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private Vector3 velocity;
        
        private Quaternion startRotation;
        private Quaternion targetRotation;
        
        private bool isDragging;
        
        private void Start()
        {
            mainCamera = FindObjectsOfType<Camera>()[0];
            
            startPosition = transform.position;
            startRotation = transform.rotation;
            
            targetPosition = startPosition;
            targetRotation = startRotation;
        }

        private Vector3 GrabMousePosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 7f;
            mousePos = mainCamera.ScreenToWorldPoint(mousePos);
            return mousePos;
        }
        
        
        // on cursor exit
        public void OnMouseUp()
        {
            if (CurrentGnack != this)
                return;
            
            // reset the position
            targetPosition = startPosition;
            
            // reset the rotation
            targetRotation = startRotation;
            
            CurrentGnack = null;
            isDragging = false;
        }
        
        // on mouse down
        public void OnMouseDown()
        {
            CurrentGnack = this;
            isDragging = true;
            
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
        }
    }
}