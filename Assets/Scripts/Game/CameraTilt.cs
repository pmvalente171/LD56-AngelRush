using System;
using UnityEngine;

namespace Game
{
    public class CameraTilt : MonoBehaviour
    {
        private Quaternion initialRotation;
        private Quaternion targetRotation;

        private void Awake()
        {
            initialRotation = transform.localRotation;
        }

        private void LateUpdate()
        {
            // follow the cursor slightly to give a parallax effect
            Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            
            float x = Mathf.Lerp(-1, 1, mousePos.x);
            float y = Mathf.Lerp(-1, 1, mousePos.y);
            
            targetRotation = initialRotation * Quaternion.Euler(y * 1, x * 1, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5);
            
        }
    }
}