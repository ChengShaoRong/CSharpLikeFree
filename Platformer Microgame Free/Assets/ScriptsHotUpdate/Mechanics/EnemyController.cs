﻿using UnityEngine;
using CSharpLike;

namespace PlatformerMicrogame
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    public class EnemyController : LikeBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;

        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        //public Bounds Bounds => collider2d.bounds;
        public Bounds GetBounds()
        {
            return _collider.bounds;
        }

        void Awake()
        {
            _collider = gameObject.GetComponent<Collider2D>();
            _audio = gameObject.GetComponent<AudioSource>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        void Start()
        {
            control = HotUpdateBehaviour.GetComponentByType(gameObject, typeof(AnimationController)) as AnimationController;
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
                control.move.x = Mathf.Clamp(mover.GetPosition().x - transform.position.x, -1, 1);
            }
        }

    }
}