using UnityEngine;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace VelocityStateMachine
{
    public class VelocityTransitionSystem : SystemBase
    {
        private NativeArray<VelocityState> _transitions;

        private FunctionPointer<ProcessVelocity> Stand;
        private FunctionPointer<ProcessVelocity> Run;
        private FunctionPointer<ProcessVelocity> Jump;
        private FunctionPointer<ProcessVelocity> Fall;

        private VelocityState Standing;
        private VelocityState Running;
        private VelocityState Jumping;
        private VelocityState Falling;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            Stand = CompileFunction(VelocityStates.Standing);
            Run = CompileFunction(VelocityStates.Running);
            Jump = CompileFunction(VelocityStates.Running);
            Fall = CompileFunction(VelocityStates.Standing);

            Standing = new VelocityState
            {
                Name = VelocityStates.Standing,
                VelocityFunction = Stand
            };

            Running = new VelocityState
            {
                Name = VelocityStates.Running,
                VelocityFunction = Run
            };

            Jumping = new VelocityState
            {
                Name = VelocityStates.Jumping,
                VelocityFunction = Jump
            };

            Falling = new VelocityState
            {
                Name = VelocityStates.Falling,
                VelocityFunction = Fall

            };

            _transitions = new NativeArray<VelocityState>(new VelocityState[16]
                {// Run         Stand       Jump        Fall
                    Running,    Standing,   Jumping,    Falling,// Standing
                    Running,    Standing,   Jumping,    Falling,// Running
                    Jumping,    Standing,   Jumping,    Falling,// Jumping
                    Falling,    Standing,   Falling,    Falling // Falling
                }, Allocator.Persistent);
        }


        protected override void OnUpdate()
        {
            var numberOfEvents = Enum.GetNames(typeof(VelocityEvents)).Length;
            var velTransitions = _transitions;

            Entities.ForEach((ref VelocityState currentState, in VelocityEvent velocityEvent) =>
            {
                var nextState = velTransitions[(int) currentState.Name * numberOfEvents + (int) velocityEvent.Value];

                if (nextState.Name != currentState.Name) // transition
                {
                    currentState = nextState;
                }
            }).Schedule();
        }

        private FunctionPointer<ProcessVelocity> CompileFunction(VelocityStates state)
        {
            switch (state)
            {
                case VelocityStates.Standing:
                    return BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Stand);

                case VelocityStates.Running:
                    return BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Run);

                case VelocityStates.Jumping:
                    return BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Jump);

                case VelocityStates.Falling:
                    return BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Fall);

                default:
                    return BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Stand);

            }
        }
    }
}