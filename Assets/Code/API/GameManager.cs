using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Scenes;
using Unity.Physics;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;
    private EntityManager entityManager;
    private BlobAssetStore assetStore;


    void Start() {

        CreateOptions();

        CreatePlayer();
    }

    void Awake() {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        assetStore = new BlobAssetStore();
    }

    void OnDestroy() {
        assetStore.Dispose();
    }


    //=========================================================================================
    //                  Helper functions
    //=========================================================================================

    private void CreateOptions() {
        // Create the options Archetype
        EntityArchetype options = entityManager.CreateArchetype(
            typeof(MouseSensitivity),
            typeof(RotationSmothnes)
        );

        // TODO: Read up on default values for components
        // Create the options entity and set default values
        Entity optionsEntity = entityManager.CreateEntity(options);
        entityManager.SetComponentData(optionsEntity, new MouseSensitivity { Value = 0.5f });
        entityManager.SetComponentData(optionsEntity, new RotationSmothnes { Value = 1f });
        entityManager.SetName(optionsEntity, "Options");
    }

    private void CreatePlayer() {
        // Instantiate a prefab entity
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, assetStore));

        var player = entityManager.Instantiate(prefab);
        entityManager.SetName(player, "Player");

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
            EntityA = player,
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

        //// Adding buffer to entity
        //entityManager.AddBuffer<BufferCollisionDetails>(player);
        //entityManager.AddComponent<ColAngle>(player);
        //entityManager.AddComponent<BaseSpeed>(player);
        //entityManager.SetComponentData(player, new BaseSpeed { Value = 8f });
        //entityManager.AddComponent<JumpHeight>(player);
        //entityManager.SetComponentData(player, new JumpHeight { Value = 15f });

        //var entity = entityManager.Instantiate(player);
    }
}