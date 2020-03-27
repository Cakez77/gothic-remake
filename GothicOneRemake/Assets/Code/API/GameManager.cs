using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Scenes;
using Unity.Physics;

public class GameManager : MonoBehaviour {
    public SubScene scene;

    public GameObject playerPrefab;

    private EntityManager entityManager;

    private Entity playerEntity;
    // public SubScene Scene
    // {
    //     get
    //     {
    //         return scene;
    //     }
    // }

    public static GameManager instance;

    void Awake() {
        // Singleton
        instance = this;

        // Get the EntityManager to instantiate Entities
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype options = entityManager.CreateArchetype(
            typeof(MouseSensitivity),
            typeof(PitchYaw),
            typeof(Gravity)
        );

        Entity optionsEntity = entityManager.CreateEntity(options);
        entityManager.SetComponentData(optionsEntity, new MouseSensitivity { Value = 0.5f });
        entityManager.SetComponentData(optionsEntity, new Gravity { Value = -1.2f });
        entityManager.SetName(optionsEntity, "Options");


        Entity playerInput = entityManager.CreateEntity();
        entityManager.AddComponent(playerInput, typeof(PlayerInput));
        entityManager.SetComponentData(playerInput, new PlayerInput());
        entityManager.SetName(playerInput, "Input");

        BlobAssetStore assetStore = new BlobAssetStore();

        // Instantiate a prefab entity
        playerEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, assetStore));

        // Adding buffer to entity
        entityManager.AddBuffer<BufferCollisionDetails>(playerEntity);
        var entity = entityManager.Instantiate(playerEntity);

        // Trying to create a limited joint to limit rotation along certain axes

        // Creating Constraints
        var angularConstraint = new Constraint {
            ConstrainedAxes = new bool3(true, false, true),
            Type = ConstraintType.Angular,
            Min = 0,
            Max = 0,
            SpringFrequency = Constraint.DefaultSpringFrequency,
            SpringDamping = Constraint.DefaultSpringDamping
        };

        Constraint[] constraints = new Constraint[] { angularConstraint };

        // Creating JointData
        var jointData = JointData.Create(
            new Math.MTransform(float3x3.identity, float3.zero),
            new Math.MTransform(float3x3.identity, float3.zero),
            constraints
            );

        // Creating a PhysicsJoint this will constrain EntityA and EntityB from rotating or moving 
        // along certain Axes based on the above defined constraints.
        var componentData = new PhysicsJoint {
            JointData = jointData,
            EntityA = entity,
            EntityB = Entity.Null,
            EnableCollision = 0
        };

        // Create the ComponentType for the PhysicsJoint
        ComponentType[] componentTypes = new ComponentType[1];
        componentTypes[0] = typeof(PhysicsJoint);

        // Create an entity to hold the PhysicsJoint component
        var jointEntity = entityManager.CreateEntity(componentTypes);
        // TODO: Check if this is needed
        // Setting the name of the jointEntity
        entityManager.SetName(jointEntity, "Joint Entity");

        // Add the component data to the jointEntity
        entityManager.AddComponentData(jointEntity, componentData);

    }
}