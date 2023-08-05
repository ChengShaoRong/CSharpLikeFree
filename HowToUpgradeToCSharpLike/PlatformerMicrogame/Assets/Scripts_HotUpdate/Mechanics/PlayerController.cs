using CSharpLike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microgame
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move = Vector2.zero;
        SpriteRenderer spriteRenderer;
        public Animator animator;
        PlatformerModel model;

        public Bounds bounds { get { return collider2d.bounds; } }

        void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            collider2d = gameObject.GetComponent<Collider2D>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            animator = gameObject.GetComponent<Animator>();
            jumpAudio = GetAudioClip("jumpAudio");
            respawnAudio = GetAudioClip("respawnAudio");
            ouchAudio = GetAudioClip("ouchAudio");
            maxSpeed = GetFloat("maxSpeed");
            jumpTakeOffSpeed = GetFloat("jumpTakeOffSpeed");
            minGroundNormalY = GetFloat("minGroundNormalY");
            gravityModifier = GetFloat("gravityModifier");
            velocity = GetVector3("velocity");
        }

        protected override void Start()
        {
            health = HotUpdateBehaviour.GetComponentByType(gameObject, typeof(Health)) as Health;
            model = GameController.Model;
            base.Start();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    (Simulation.Schedule(typeof(PlayerStopJump)) as PlayerStopJump).player = this;
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        (Simulation.Schedule(typeof(PlayerJumped)) as PlayerJumped).player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        (Simulation.Schedule(typeof(PlayerLanded)) as PlayerLanded).player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}