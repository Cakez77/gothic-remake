using UnityEngine;
using Unity.Burst;
using Unity.Entities;

namespace VelocityStateMachine
{
    public struct VelocityState : IComponentData
    {
        public float Time;
        public VelocityStates Name;
        public FunctionPointer<ProcessVelocity> VelocityFunction;
    }

    public struct VelocityEvent : IComponentData
    {
        public VelocityEvents Value;
    }
}