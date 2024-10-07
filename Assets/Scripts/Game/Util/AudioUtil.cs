using UnityEngine;

namespace Game.Util
{
    public static class AudioUtil
    {
        public static void PlayOneShot(AudioSource audioSource, float pitch = 1f)
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}