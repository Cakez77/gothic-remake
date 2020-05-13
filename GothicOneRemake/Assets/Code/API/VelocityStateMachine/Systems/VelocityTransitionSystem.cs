using Unity.Transforms;
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

        protected unsafe override void OnStartRunning()
        {
            base.OnStartRunning();

            Stand = BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Stand);

            Run = BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Run);

            Jump = BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Jump);

            Fall = BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Fall);


            Standing = new VelocityState
            {
                Name = VelocityStates.Standing,
                VelocityFunction = Stand,
                Time = 0f,
                Duration = 0.3f
            };

            Running = new VelocityState
            {
                Name = VelocityStates.Running,
                VelocityFunction = Run,
                Time = 0f,
                Duration = 1.5f
            };

            Jumping = new VelocityState
            {
                Name = VelocityStates.Jumping,
                VelocityFunction = Jump,
                Time = 0f,
                Duration = 0.01f
            };

            Falling = new VelocityState
            {
                Name = VelocityStates.Falling,
                VelocityFunction = Fall,
                Time = 0f,
                Duration = 0.5f
            };

            _transitions = new NativeArray<VelocityState>(
                new VelocityState[4]
                {
                    Standing, Running, Jumping, Falling,
                },
                Allocator.Persistent);
        }


        protected override void OnUpdate()
        {
            var numberOfEvents = Enum.GetNames(typeof(VelocityEvents)).Length;
            var velTransitions = _transitions;

            Entities.ForEach((ref VelocityState currentState, ref TakeoffHeight takeoffHeight, in LocalToWorld ltw, in VelocityEvent velocityEvent) =>
            {
                var nextState = velTransitions[(int) velocityEvent.Value];

                if (nextState.Name != currentState.Name) // transition
                {
                    currentState = nextState;
                }

            }).Schedule();
        }

        private unsafe FunctionPointer<ProcessVelocity> CompileFunction(VelocityStates state)
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

        protected override void OnStopRunning()
        {
            base.OnStopRunning();

            _transitions.Dispose();
        }
    }
}