using UnityEngine;

namespace CSharpLike// RongRong : Change namespace to "CSharpLike" or add "using CSharpLike;" in the front.
{
    /// <summary>
    /// RongRong : This class include mothed 'Start/Update',
    /// we using 'HotUpdateBehaviourUpdate' to bind prefab and set 'scriptUpdateFPS' value to 10000.
    /// </summary>
    public class UIDirectionControl : LikeBehaviour // RongRong : Change 'MonoBehaviour' to 'LikeBehaviour'
    {
        // This class is used to make sure world space UI
        // elements such as the health bar face the correct direction.

        public bool m_UseRelativeRotation = true;       // Use relative rotation should be used for this gameobject?


        private Quaternion m_RelativeRotation;          // The local rotatation at the start of the scene.


        private void Start ()
        {
            m_RelativeRotation = transform.parent.localRotation;
        }


        private void Update ()
        {
            if (m_UseRelativeRotation)
                transform.rotation = m_RelativeRotation;
        }
    }
}