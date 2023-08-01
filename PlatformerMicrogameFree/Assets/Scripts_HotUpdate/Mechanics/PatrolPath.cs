using CSharpLike;
using UnityEngine;

namespace Microgame
{
    /// <summary>
    /// This component is used to create a patrol path, two points which enemies will move between.
    /// </summary>
    public partial class PatrolPath : LikeBehaviour
    {
        /// <summary>
        /// One end of the patrol path.
        /// </summary>
        public Vector2 startPosition;
        public Vector2 endPosition;

        void Awake()
        {
            startPosition = GetVector3("startPosition");
            endPosition = GetVector3("endPosition");
        }

        /// <summary>
        /// Create a Mover instance which is used to move an entity along the path at a certain speed.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public Mover CreateMover(float speed)
        {
            return new Mover(this, speed);
        }

        void Reset()
        {
            startPosition = Vector3.left;
            endPosition = Vector3.right;
        }
    }
}