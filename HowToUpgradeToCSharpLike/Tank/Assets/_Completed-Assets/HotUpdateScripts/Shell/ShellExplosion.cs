using UnityEngine;

namespace CSharpLike// RongRong : Change namespace to "CSharpLike" or add "using CSharpLike;" in the front.
{
    /// <summary>
    /// RongRong : This class include mothed 'Start/OnTriggerEnter',
    /// we using 'HotUpdateBehaviourTrigger' to bind prefabe.
    /// We add 'Game Objects' name "m_ExplosionParticles"/"m_ExplosionAudio" in prefab.
    /// We add 'Floats' name "m_MaxDamage"/"m_ExplosionForce"/"m_MaxLifeTime"/"m_ExplosionRadius" in prefab.
    /// We add 'Strings' name "m_TankMask" in prefab.
    /// </summary>
    public class ShellExplosion : LikeBehaviour // RongRong : Change 'MonoBehaviour' to 'LikeBehaviour'
    {
        public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
        public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
        public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
        public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
        public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
        public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
        public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.


        // RongRong : Bind value MUST in Awake before use it due to execute order : Awake -> OnEnable -> Start.
        void Awake()
        {
            // RongRong : Bind value from prefab.
            m_MaxDamage = GetFloat("m_MaxDamage", 100f);
            m_ExplosionForce = GetFloat("m_ExplosionForce", 1000f);
            m_MaxLifeTime = GetFloat("m_MaxLifeTime", 2f);
            m_ExplosionRadius = GetFloat("m_ExplosionRadius", 5f);
            m_TankMask = LayerMask.NameToLayer(GetString("m_TankMask", "Players"));// RongRong : Not support bind struct, we using string instead, and then covert to struct! 
            m_ExplosionParticles = GetComponent<ParticleSystem>("m_ExplosionParticles");
            m_ExplosionAudio = GetComponent<AudioSource>("m_ExplosionAudio");
#if UNITY_WEBGL
            ResourceManager.LoadAudioClipAsync("https://www.csharplike.com/CSharpLikeDemo/AssetBundles/Tank/WebGL/ShellExplosion.wav", AudioType.WAV, (AudioClip audioClip) =>
            {
                m_ExplosionAudio.clip = audioClip;
            });
#endif
        }
        private void Start ()
        {
            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            // RongRong : Add prefix "GameObject." 
            GameObject.Destroy (gameObject, m_MaxLifeTime);
        }


        private void OnTriggerEnter (Collider other)
        {
            // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
            // RongRong : Change 'm_TankMask' to '1 << m_TankMask.value'. I don't know why, just debug value is 512(equal to 1 << 9).
            Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, 512);
            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++)
            {
                // ... and find their rigidbody.
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

                // If they don't have a rigidbody, go on to the next collider.
                if (targetRigidbody == null)
                    continue;

                // Add an explosion force.
                targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

                // Find the TankHealth script associated with the rigidbody.
                // RongRong : Change 'targetRigidbody.GetComponent<TankHealth> ();' to 'HotUpdateBehaviour.GetComponentByType(targetRigidbody, typeof(TankHealth)) as TankHealth;'
                TankHealth targetHealth = HotUpdateBehaviour.GetComponentByType(targetRigidbody, typeof(TankHealth)) as TankHealth;

                // If there is no TankHealth script attached to the gameobject, go on to the next collider.
                // RongRong : Change '!targetHealth' to 'targetHealth == null'
                if (targetHealth == null)
                    continue;

                // Calculate the amount of damage the target should take based on it's distance from the shell.
                float damage = CalculateDamage (targetRigidbody.position);

                // Deal this damage to the tank.
                targetHealth.TakeDamage (damage);
            }

            // Unparent the particles from the shell.
            m_ExplosionParticles.transform.parent = null;

            // Play the particle system.
            m_ExplosionParticles.Play();

            // Play the explosion sound effect.
            m_ExplosionAudio.Play();

            // Once the particles have finished, destroy the gameobject they are on.
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            // RongRong : Add prefix "GameObject." 
            GameObject.Destroy (m_ExplosionParticles.gameObject, mainModule.duration);

            // Destroy the shell.
            // RongRong : Add prefix "GameObject." 
            GameObject.Destroy (gameObject);
        }


        private float CalculateDamage (Vector3 targetPosition)
        {
            // Create a vector from the shell to the target.
            Vector3 explosionToTarget = targetPosition - transform.position;

            // Calculate the distance from the shell to the target.
            float explosionDistance = explosionToTarget.magnitude;

            // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
            float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

            // Calculate damage as this proportion of the maximum possible damage.
            float damage = relativeDistance * m_MaxDamage;

            // Make sure that the minimum damage is always 0.
            damage = Mathf.Max (0f, damage);

            return damage;
        }
    }
}