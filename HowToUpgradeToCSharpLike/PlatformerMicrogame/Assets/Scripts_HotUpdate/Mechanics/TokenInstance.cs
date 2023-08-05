using CSharpLike;
using UnityEngine;

namespace Microgame
{
    /// <summary>
    /// This class contains the data required for implementing token collection mechanics.
    /// It does not perform animation of the token, this is handled in a batch by the 
    /// TokenController in the scene.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class TokenInstance : LikeBehaviour
    {
        public AudioClip tokenCollectAudio;
        [Tooltip("If true, animation will start at a random position in the sequence.")]
        public bool randomAnimationStartTime = false;
        [Tooltip("List of frames that make up the animation.")]
        public Sprite[] idleAnimation;
        public Sprite[] collectedAnimation;

        public Sprite[] sprites = new Sprite[0];

        public SpriteRenderer _renderer;

        //unique index which is assigned by the TokenController in a scene.
        public int tokenIndex = -1;
        public TokenController controller;
        //active frame in animation, updated by the controller.
        public int frame = 0;
        public bool collected = false;

        void Start()
        {
            tokenCollectAudio = GetAudioClip("tokenCollectAudio");
            randomAnimationStartTime = GetBoolean("randomAnimationStartTime");
            _renderer = gameObject.GetComponent<SpriteRenderer>();
            if (randomAnimationStartTime)
                frame = Random.Range(0, sprites.Length);
            int idleAnimationCount = GetInt("idleAnimationCount");
            idleAnimation = new Sprite[idleAnimationCount];
            for (int i=0; i<idleAnimationCount; i++)
            {
                idleAnimation[i] = GetSprite("idleAnimation"+i);
            }
            int collectedAnimationCount = GetInt("collectedAnimationCount");
            collectedAnimation = new Sprite[collectedAnimationCount];
            for (int i = 0; i < collectedAnimationCount; i++)
            {
                collectedAnimation[i] = GetSprite("collectedAnimation" + i);
            }
            sprites = idleAnimation;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            //only exectue OnPlayerEnter if the player collides with this token.
            var player = HotUpdateBehaviour.GetComponentByType(other.gameObject, typeof(PlayerController)) as PlayerController;
            if (player != null) OnPlayerEnter(player);
        }

        void OnPlayerEnter(PlayerController player)
        {
            if (collected) return;
            //disable the gameObject and remove it from the controller update list.
            frame = 0;
            sprites = collectedAnimation;
            if (controller != null)
                collected = true;
            //send an event into the gameplay system to perform some behaviour.
            var ev = Simulation.Schedule(typeof(PlayerTokenCollision)) as PlayerTokenCollision;
            ev.token = this;
            ev.player = player;
        }
    }
}