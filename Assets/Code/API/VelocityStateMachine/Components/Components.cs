using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;

namespace VelocityStateMachine
{
    public struct VelocityState : IComponentData
    {
        public float Time;
        public float Duration;
        public VelocityStates Name;
        public FunctionPointer<ProcessVelocity> VelocityFunction;
    }

    public struct VelocityEvent : IComponentData
    {
        public VelocityEvents Value;
    }

    public struct Heading : IComponentData
    {
        public float2 Value;
    }

    public struct JumpHeight : IComponentData
    {
        public float Value;
    }

    public struct JumpForce : IComponentData
    {
        public float Value;
    }

    public struct ColAngle : IComponentData
    {
        public float Value;
    }

    public struct OnGround : IComponentData
    {
        public bool Value;
    }

    public struct GroundNormal : IComponentData
    {
        public float3 Value
        {
            get;
            set;
        }
    }

    public struct TakeoffHeight : IComponentData
    {
        public float Value;
    }

    public struct MovementSpeed : IComponentData
    {
        public float Value;
    }

    public struct Slope : IComponentData
    {
        public float Value;
    }
}