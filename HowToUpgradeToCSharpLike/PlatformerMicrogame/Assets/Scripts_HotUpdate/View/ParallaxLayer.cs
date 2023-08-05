using CSharpLike;
using UnityEngine;

namespace Microgame
{
    /// <summary>
    /// Used to move a transform relative to the main camera position with a scale factor applied.
    /// This is used to implement parallax scrolling effects on different branches of gameobjects.
    /// </summary>
    public class ParallaxLayer : LikeBehaviour
    {
        /// <summary>
        /// Movement of the layer is scaled by this value.
        /// </summary>
        public Vector3 movementScale = Vector3.one;

        Transform _camera;

        void Awake()
        {
            _camera = Camera.main.transform;
            movementScale = GetVector3("movementScale");
        }

        void LateUpdate()
        {
            transform.position = Vector3.Scale(_camera.position, movementScale);
        }

    }
}