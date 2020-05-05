using Unity.Burst;
using Unity.Mathematics;

namespace VelocityStateMachine
{
    public delegate float3 ProcessVelocity(float3 linearVelocity, float3 forward, float t, float speed = 0f, float height = 0f);

    [BurstCompile]
    public static class VelocityFunctions
    {
        private static float _airSpeed = 2f; // TODO: Bad, I don't know where to put this
        /**
         * 
         */
        [BurstCompile]
        public static float3 Run(float3 linearVelocity, float3 forward, float t, float speed, float height = 0f)
        {
            // check if supplied time is too low
            float newXVel = forward.x * speed * t * t;
            float newYVel = forward.z * speed * t * t;

            float additionalTime = 0f;
            bool timeIsTooLow = newXVel < linearVelocity.x;

            if (timeIsTooLow)
            {
                additionalTime = linearVelocity.x / (forward.x * speed);
                t += additionalTime;
            }

            if (t > 1)
            {
                t = 1;
            }

            if (forward.y < 0)
            {
                linearVelocity = PushPlayerUp(linearVelocity, forward, t, speed, height);
            }
            else
            {
                linearVelocity = PushPlayerDown(linearVelocity, forward, t, speed, height);
            }

            linearVelocity.x = newXVel;
            linearVelocity.z = newYVel;

            return linearVelocity;
        }

        /**
         * 
         */
        [BurstCompile]
        public static float3 Stand(float3 linearVelocity, float3 forward, float t, float speed = 0f, float height = 0f)
        {
            t = 1 - t; // inverse, this is ease out quad

            linearVelocity.x *= forward.x * t * t;
            linearVelocity.x *= forward.x * t * t;

            return linearVelocity;
        }

        public static float3 Jump(float3 linearVelocity, float3 forward, float t, float speed = 0f, float height = 0f)
        {
            // TODO: Implement a nice jump based on time and an easing function.
            if (t == 0)
            {
                linearVelocity.y = height;
            }
            return linearVelocity;
        }

        public static float3 Fall(float3 linearVelocity, float3 forward, float t, float speed = 0f, float height = 0f)
        {
            linearVelocity.x = forward.x * _airSpeed * t;
            linearVelocity.z = forward.z * _airSpeed * t;

            return linearVelocity;
        }

        public static float3 PushPlayerUp(float3 linearVelocity, float3 forward, float t, float speed = 0f, float height = 0f)
        {
            linearVelocity.y = forward.y * speed * t * t * 6.1f; // Magic number, additional gravity

            return linearVelocity;
        }

        public static float3 PushPlayerDown(float3 linearVelocity, float3 forward, float t, float speed = 0f, float height = 0f)
        {
            linearVelocity.y = forward.y * speed * t * t * 2.1f; // Magic number, additional gravity

            return linearVelocity;
        }
    }
}