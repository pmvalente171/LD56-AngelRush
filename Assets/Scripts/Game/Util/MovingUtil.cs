using UnityEngine;

namespace Game.Util
{
    public static class MovingUtil
    {
        // spring damping between two floats
        public static float SpringDamp(float current, float target, float spring, float damping)
        {
            // Velocity is tracked internally to control the rate of change
            float velocity = 0f;

            // Calculate the force applied by the spring
            float force = (target - current) * spring;

            // Update velocity based on damping and spring force
            velocity += force * Time.deltaTime;
            velocity *= Mathf.Clamp01(1.0f - damping * Time.deltaTime);

            // Calculate the new position
            float newPosition = current + velocity;

            return newPosition;
        }
        
        public static Vector3 SpringDamp(Vector3 current, Vector3 target, ref Vector3 velocity, float spring, float damping)
        {
            // Calculate the force applied by the spring
            Vector3 force = (target - current) * spring;

            // Update velocity based on damping and spring force
            velocity += force * Time.deltaTime;
            velocity *= Mathf.Clamp01(1.0f - damping * Time.deltaTime);

            // Calculate the new position
            Vector3 newPosition = current + velocity;

            return newPosition;
        }
    }
}