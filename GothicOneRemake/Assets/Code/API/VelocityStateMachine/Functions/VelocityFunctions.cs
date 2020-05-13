using Unity.Burst;
using Unity.Mathematics;

namespace VelocityStateMachine
{
    public unsafe delegate float3* ProcessVelocity(
        float3* linearVelocity,
        float3* forward,
        float3* right,
        float3* normal,
        float t,
        float speed,
        float height);

    [BurstCompile]
    public static class VelocityFunctions
    {
        private static float _airSpeed = 2f; // TODO: Bad, I don't know where to put this
        /**
         * 
         */
        [BurstCompile]
        public unsafe static float3* Run(
            float3* linearVelocity,
            float3* forward,
            float3* right,
            float3* normal,
            float t,
            float speed,
            float height)
        {
            // check if supplied time is too low
            t = MakeTimeCorrect(linearVelocity->x, t, forward->x, speed);

            return WalkOnGround(linearVelocity, forward, right, normal, speed, t);
        }

        private unsafe static float3* WalkOnGround(float3* vel, float3* forward, float3* right, float3* normal, float speed, float t)
        {
            float slope = math.cross(*right, *normal).y;

            vel->x = forward->x * speed * t * t;
            vel->y = AllignWithSlope(slope, t, speed);
            vel->z = forward->z * speed * t * t;

            return vel;
        }

        /**
         * 
         */
        [BurstCompile]
        public static float3 Stand(
            float3 linearVelocity,
            float3 forward,
            float3 right,
            float3 normal,
            float t,
            float speed,
            float height)
        {
            t = 1 - t; // inverse, this is ease out quad

            return WalkOnGround(linearVelocity, forward, right, normal, speed, t);
        }

        /**
         * 
         */
        [BurstCompile]
        public static float3 Jump(
            float3 linearVelocity,
            float3 forward,
            float3 right,
            float3 normal,
            float t,
            float speed,
            float height)
        {
            // TODO: Implement a nice jump based on time and an easing function.
            if (math.length(normal) > 0)
            {
                linearVelocity.y = height;
            }

            return linearVelocity;
        }

        /**
         * 
         */
        [BurstCompile]
        public static float3 Fall(//TODO: Change in a way that speed will be air speed
            float3 linearVelocity,
            float3 forward,
            float3 right,
            float3 normal,
            float t,
            float speed,
            float height)
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
                pushFactor = 1f;
            }
            return slope * speed * t * t * pushFactor; // Magic number, additional gravity
        }

        private static float MakeTimeCorrect(float currentVelocity, float t, float forward, float speed)
        {
            float additionalTime = 0f;

            float newVelocity = forward * speed * t * t;
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