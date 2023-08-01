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
    public class PlayerController : LikeBehaviour// Not support inherit class in FREE version
    {
        /// <summary>
        /// The minimum normal (dot product) considered suitable for the entity sit on.
        /// </summary>
        public float minGroundNormalY = 0.65f;

        /// <summary>
        /// A custom gravity coefficient applied to this entity.
        /// </summary>
        public float gravityModifier = 1f;

        /// <summary>
        /// The current velocity of the entity.
        /// </summary>
        public Vector2 velocity;

        /// <summary>
        /// Is the entity currently sitting on a surface?
        /// </summary>
        /// <value></value>
        public bool IsGrounded { get; private set; }

        protected Vector2 targetVelocity = Vector2.zero;
        protected Vector2 groundNormal = Vector2.zero;
        protected Rigidbody2D body;
        protected ContactFilter2D contactFilter = new ContactFilter2D();
        protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

        protected const float minMoveDistance = 0.001f;
        protected const float shellRadius = 0.01f;


        /// <summary>
        /// Bounce the object's vertical velocity.
        /// </summary>
        /// <param name="value"></param>
        public void Bounce(float value)
        {
            velocity.y = value;
        }

        /// <summary>
        /// Bounce the objects velocity in a direction.
        /// </summary>
        /// <param name="dir"></param>
        public void BounceV2(Vector2 dir)
        {
            velocity.y = dir.y;
            velocity.x = dir.x;
        }

        /// <summary>
        /// Teleport to some position.
        /// </summary>
        /// <param name="position"></param>
        public void Teleport(Vector3 position)
        {
            body.position = position;
            velocity *= 0;
            body.velocity *= 0;
        }

        protected virtual void OnEnable()
        {
            body = gameObject.GetComponent<Rigidbody2D>();
            body.isKinematic = true;
        }

        protected virtual void OnDisable()
        {
            body.isKinematic = false;
        }

        protected virtual void FixedUpdate()
        {
            //if already falling, fall faster than the jump speed, otherwise use normal gravity.
            if (velocity.y < 0)
                velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            else
                velocity += Physics2D.gravity * Time.deltaTime;

            velocity.x = targetVelocity.x;

            IsGrounded = false;

            var deltaPosition = velocity * Time.deltaTime;

            var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

            var move = moveAlongGround * deltaPosition.x;

            PerformMovement(move, false);

            move = Vector2.up * deltaPosition.y;

            PerformMovement(move, true);

        }

        void PerformMovement(Vector2 move, bool yMovement)
        {
            //Debug.Log($"PerformMovement IsGrounded={IsGrounded} move={move} yMovement={yMovement}");
            var distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                //Debug.Log("PerformMovement distance > minMoveDistance");
                //check if we hit anything in current direction of travel
                var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
                for (var i = 0; i < count; i++)
                {
                    var currentNormal = hitBuffer[i].normal;

                    //is this surface flat enough to land on?
                    if (currentNormal.y > minGroundNormalY)
                    {
                        IsGrounded = true;
                        //Debug.Log($"PerformMovement IsGrounded={IsGrounded}");
                        // if moving up, change the groundNormal to new surface normal.
                        if (yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }
                    if (IsGrounded)
                    {
                        //how much of our velocity aligns with surface normal?
                        var projection = Vector2.Dot(velocity, currentNormal);
                        if (projection < 0)
                        {
                            //slower velocity if moving against the normal (up a hill).
                            velocity = velocity - projection * currentNormal;
                        }
                    }
                    else
                    {
                        //We are airborne, but hit something, so cancel vertical up and horizontal velocity.
                        velocity.x *= 0;
                        velocity.y = Mathf.Min(velocity.y, 0);
                    }
                    //remove shellDistance from actual move distance.
                    var modifiedDistance = hitBuffer[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }
            body.position = body.position + move.normalized * distance;
        }

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

        public int jumpState = 0;// JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move = Vector2.zero;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        PlatformerModel model;

        public Bounds GetBounds() { return collider2d.bounds; }

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

        protected void Start()
        {
            health = HotUpdateBehaviour.GetComponentByType(gameObject, typeof(Health)) as Health;
            model = GameController.Model;
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            contactFilter.useLayerMask = true;
        }

        protected void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == 0// JumpState.Grounded
                    && Input.GetButtonDown("Jump"))
                    jumpState = 1;// JumpState.PrepareToJump;
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
            targetVelocity = Vector2.zero;
            ComputeVelocity();
        }

        void UpdateJumpState()
        {
            jump = false;
            if (jumpState == 1)// JumpState.PrepareToJump:
            {
                jumpState = 2;// JumpState.Jumping;
                jump = true;
                stopJump = false;
            }
            else if (jumpState == 2)// JumpState.Jumping:
            {
                if (!IsGrounded)
                {
                    (Simulation.Schedule(typeof(PlayerJumped)) as PlayerJumped).player = this;
                    jumpState = 3;// JumpState.InFlight;
                }
            }
            else if (jumpState == 3)// JumpState.InFlight:
            {
                if (IsGrounded)
                {
                    (Simulation.Schedule(typeof(PlayerLanded)) as PlayerLanded).player = this;
                    jumpState = 4;// JumpState.Landed;
                }
            }
            else if (jumpState == 4)// JumpState.Landed:
            {
                jumpState = 0;// JumpState.Grounded;
            }
        }

        protected void ComputeVelocity()
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

        //public enum JumpState
        //{
        //    Grounded,//0
        //    PrepareToJump,//1
        //    Jumping,//2
        //    InFlight,//3
        //    Landed//4
        //}
    }
}