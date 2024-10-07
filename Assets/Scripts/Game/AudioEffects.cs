using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class AudioEffects : MonoBehaviour
    {
        public AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            // randomize the pitch of the audio source
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.Play();
        }
        
        public void PlayOneShot()
        {
            // randomize the pitch of the audio source
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}