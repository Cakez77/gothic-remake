using Unity.Burst;
using Unity.Mathematics;

namespace VelocityStateMachine
{
    public struct VelocityParams
    {
        public float3 linearVelocity;
        public float3 forward;
        public float3 right;
        public float3 normal;
        public float time;
        public float movementSpeed;
        public float jumpForce;
    };

    public unsafe delegate float3* ProcessVelocity(VelocityParams* velocityParams);

    [BurstCompile]
    public static class VelocityFunctions
    {
        private static float _airSpeed = 2f; // TODO: Change speed based on world state(system)
        /**
         * 
         */
        [BurstCompile]
        public unsafe static float3* Run(VelocityParams* velocityParams)
        {
            // check if supplied time is too low
            // TODO: Not working correctly, debug to find the problem
            var t = MakeTimeCorrect(velocityParams->linearVelocity.x, velocityParams->time, velocityParams->forward.x, velocityParams->movementSpeed);

            var vel = WalkOnGround(
                velocityParams->linearVelocity, 
                velocityParams->forward, 
                velocityParams->right, 
                velocityParams->normal, 
                velocityParams->movementSpeed, 
                t);

            return &vel;
        }

        private static float3 WalkOnGround(float3 vel, float3 forward, float3 right, float3 normal, float speed, float t)
        {
            float slope = math.cross(right, normal).y;

            vel.x = forward.x * speed * t * t;
            vel.y = AllignWithSlope(slope, t, speed);
            vel.z = forward.z * speed * t * t;

            return vel;
        }

        /**
         * 
         */
        [BurstCompile]
        public unsafe static float3* Stand(VelocityParams* velocityParams)
        {
            var t = 1 - velocityParams->time; // inverse, this is ease out quad

            var vel = WalkOnGround(
                velocityParams->linearVelocity, 
                velocityParams->forward, 
                velocityParams->right, 
                velocityParams->normal, 
                velocityParams->movementSpeed, 
                t);

            return &vel;
        }

        /**
         * 
         */
        [BurstCompile]
        public unsafe static float3* Jump(VelocityParams* velocityParams)
        {
            // TODO: Implement a nice jump based on time and an easing function.
            if (math.length(velocityParams->normal) > 0)
            {
                velocityParams->linearVelocity.y = velocityParams->jumpForce;
            }

            return &velocityParams->linearVelocity;
        }

        /**
         * 
         */
         //TODO: This has to be additive, x and z should almost stay the same as when leaving the ground
        [BurstCompile]
        public unsafe static float3* Fall(VelocityParams* velocityParams)
        {
            velocityParams->linearVelocity.x = velocityParams->forward.x * _airSpeed * velocityParams->time;
            velocityParams->linearVelocity.z = velocityParams->forward.z * _airSpeed * velocityParams->time;

            return &velocityParams->linearVelocity;
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