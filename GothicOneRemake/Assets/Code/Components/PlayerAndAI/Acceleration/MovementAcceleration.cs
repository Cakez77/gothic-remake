using Unity.Entities;

public struct MovementAcceleration : IComponentData {
    public float ElaspedTime;
    public float AccelerationTime; //TODO Have AcelerationTime be derived from Dexterity
    public float MaxSpeed; //TODO Have a System alter MaxSpeed based on Input
}