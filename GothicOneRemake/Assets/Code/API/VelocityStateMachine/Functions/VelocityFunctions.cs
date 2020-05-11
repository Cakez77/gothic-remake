using Unity.Burst;
using Unity.Mathematics;

namespace VelocityStateMachine
{
    public delegate float3 ProcessVelocity(float3 linearVelocity, float3 forward, float3 right, float3 normal, float t, float speed = 0f, float height = 0f);

    [BurstCompile]
    public static class VelocityFunctions
    {
        private static float _airSpeed = 2f; // TODO: Bad, I don't know where to put this
        /**
         * 
         */
        [BurstCompile]
        public static float3 Run(float3 linearVelocity, float3 forward, float3 right, float3 normal, float t, float speed, float height = 0f)
        {
            float slope = math.cross(right, normal).y;
            float newXVel = forward.x * speed * t * t;
            float newZVel = forward.z * speed * t * t;

            // check if supplied time is too low
            t = MakeTimeCorrect(linearVelocity.x, newXVel, t, forward.x, speed);

            linearVelocity.x = newXVel;
            linearVelocity.y = AllignWithSlope(slope, t, speed);
            linearVelocity.z = newZVel;

            return linearVelocity;
        }

        /**
         * 
         */
        [BurstCompile]
        public static float3 Stand(float3 linearVelocity, float3 forward, float3 right, float3 normal, float t, float speed = 0f, float height = 0f)
        {
            t = 1 - t; // inverse, this is ease out quad

            float slope = math.cross(right, normal).y;

            linearVelocity.x = forward.x * t * t;
            linearVelocity.y = AllignWithSlope(slope, t, speed);
            linearVelocity.z = forward.z * t * t;

            return linearVelocity;
        }

        /**
         * 
         */
        [BurstCompile]
        public static float3 Jump(float3 linearVelocity, float3 forward, float3 right, float3 normal, float t, float speed = 0f, float height = 0f)
        {
            // TODO: Implement a nice jump based on time and an easing function.
            if (t == 1)
            {
                linearVelocity.y = height;
            }
            return linearVelocity;
        }

        /**
         * 
         */
        [BurstCompile]
        public static float3 Fall(float3 linearVelocity, float3 forward, float3 right, float3 normal, float t, float speed = 0f, float height = 0f)
        {
            linearVelocity.x = forward.x * _airSpeed * t;
            linearVelocity.z = forward.z * _airSpeed * t;

            return linearVelocity;
        }

        public static float AllignWithSlope(float slope, float t, float speed)
        {
            float pushFactor = 6.1f;
            if (slope > 0)
            {
                pushFactor = 2.1f;
            }
            return slope * speed * t * t * pushFactor; // Magic number, additional gravity
        }

        private static float MakeTimeCorrect(float currentVelocity, float newVelocity, float t, float forward, float speed)
        {
            float additionalTime = 0f;
            bool timeIsTooLow = newVelocity < currentVelocity;

            if (timeIsTooLow)
            {
                additionalTime = currentVelocity / (forward * speed);
                t += additionalTime;
            }

            if (t > 1)
            {
                t = 1;
            }

            return t;
        }
    }
}