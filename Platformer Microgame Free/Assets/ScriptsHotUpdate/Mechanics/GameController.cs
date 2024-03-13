using UnityEngine;
using CSharpLike;

namespace PlatformerMicrogame
{
    /// <summary>
    /// This class exposes the the game model in the inspector, and ticks the
    /// simulation.
    /// </summary> 
    public class GameController : LikeBehaviour
    {
        public static GameController Instance { get; private set; }

        //This model field is public and can be therefore be modified in the 
        //inspector.
        //The reference actually comes from the InstanceRegister, and is shared
        //through the simulation and events. Unity will deserialize over this
        //shared reference when the scene loads, allowing the model to be
        //conveniently configured inside the inspector.
        public static PlatformerModel Model { get; private set; }

        /// <summary>
        /// The virtual camera in the scene.
        /// </summary>
        public Cinemachine.CinemachineVirtualCamera virtualCamera;

        /// <summary>
        /// The main component which controls the player sprite, controlled 
        /// by the user.
        /// </summary>
        public PlayerController player;

        /// <summary>
        /// The spawn point in the scene.
        /// </summary>
        public Transform spawnPoint;

        /// <summary>
        /// A global jump modifier applied to all initial jump velocities.
        /// </summary>
        public float jumpModifier = 1.5f;

        /// <summary>
        /// A global jump modifier applied to slow down an active jump when 
        /// the user releases the jump input.
        /// </summary>
        public float jumpDeceleration = 0.5f;

        void Awake()
        {
            //Because not support attribute [Serializable], we new a object here
            //Model = new PlatformerModel()
            //{
            //    virtualCamera = virtualCamera,
            //    spawnPoint = spawnPoint,
            //    jumpModifier = jumpModifier,
            //    jumpDeceleration = jumpDeceleration
            //};
            Model = new PlatformerModel();
            Model.virtualCamera = virtualCamera;
            Model.spawnPoint = spawnPoint;
            Model.jumpModifier = jumpModifier;
            Model.jumpDeceleration = jumpDeceleration;
            Debug.LogError("Test:" + Test(gameObject));
        }

        string Test<T>(T a) where T : UnityEngine.Object
        {
            return a.name;
        }

        void Start()
        {
            //LikeBehaviour May be can't get object in 'Awake' function
            Model.player = player;
        }

        void OnEnable()
        {
            Instance = this;
        }

        void OnDisable()
        {
            if (Instance == this)
                Instance = null;
        }

        void Update()
        {
            if (Instance == this)
                Simulation.Tick();
        }
    }
}