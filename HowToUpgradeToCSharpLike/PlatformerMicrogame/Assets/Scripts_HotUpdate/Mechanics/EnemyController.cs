using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSharpLike;

namespace Microgame
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    public class EnemyController : LikeBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;

        public PatrolPath.Mover mover;
        public AnimationController control;
        public Collider2D _collider;
        public AudioSource _audio;
        SpriteRenderer spriteRenderer;

        public Bounds bounds
        {
            get
            {
                return _collider.bounds;
            }
        }

        void Awake()
        {
            _collider = gameObject.GetComponent<Collider2D>();
            _audio = gameObject.GetComponent<AudioSource>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            ouch = GetAudioClip("ouch");
        }
        void Start()
        {
            control = HotUpdateBehaviour.GetComponentByType(gameObject, typeof(AnimationController)) as AnimationController;
            path = HotUpdateBehaviour.GetComponentByType(GetGameObject("path"), typeof(PatrolPath)) as PatrolPath;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = HotUpdateBehaviour.GetComponentByType(collision.gameObject, typeof(PlayerController)) as PlayerController;
            if (player != null)
            {
                var ev = Simulation.Schedule(typeof(PlayerEnemyCollision)) as PlayerEnemyCollision;
                ev.player = player;
                ev.enemy = this;
            }
        }

        void Update()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

    }
}