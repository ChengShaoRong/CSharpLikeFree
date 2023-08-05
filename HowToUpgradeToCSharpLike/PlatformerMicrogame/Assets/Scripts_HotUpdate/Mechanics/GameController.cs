using UnityEngine;
using CSharpLike;

namespace Microgame
{
    /// <summary>
    /// This class exposes the the game model in the inspector, and ticks the
    /// simulation.
    /// </summary> 
    public class GameController : LikeBehaviour
    {
        public static GameController Instance { get; private set; }

        public static PlatformerModel Model { get; private set; }
        void Awake()
        {
            Model = new PlatformerModel();
            Model.virtualCamera = GetGameObject("virtualCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>();
            Model.spawnPoint = GetGameObject("spawnPoint").GetComponent<Transform>();
            Model.jumpModifier = GetFloat("jumpModifier");
            Model.jumpDeceleration = GetFloat("jumpDeceleration");
        }
        void Start()
        {
            Model.player = HotUpdateBehaviour.GetComponentByType(GetGameObject("player"), typeof(PlayerController)) as PlayerController;
        }

        void OnEnable()
        {
            Instance = this;
        }

        void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            //if (Instance == this) Simulation.Tick();
        }
    }
}