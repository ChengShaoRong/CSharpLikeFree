using PlatformerMicrogame;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CSharpLike;
namespace Platformer
{
    [CustomEditor(typeof(PatrolPath))]
    public class PatrolPathGizmo : Editor
    {
        public void OnSceneGUI()
        {
            var hub = target as HotUpdateBehaviour;
            if (hub == null)
                return;
            KissSerializableObject objStartPosition = hub.GetKissSerializableObject("startPosition");
            KissSerializableObject objEndPosition = hub.GetKissSerializableObject("endPosition");
            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                var sp = hub.transform.InverseTransformPoint(Handles.PositionHandle(hub.transform.TransformPoint((Vector3)objStartPosition.Value), hub.transform.rotation));
                var ep = hub.transform.InverseTransformPoint(Handles.PositionHandle(hub.transform.TransformPoint((Vector3)objEndPosition.Value), hub.transform.rotation));
                if (cc.changed)
                {
                    sp.y = 0;
                    ep.y = 0;
                    objStartPosition.Value = sp;
                    objEndPosition.Value = ep;
                    hub.UpdateFieldInEditor(objStartPosition);
                    hub.UpdateFieldInEditor(objEndPosition);
                }
            }
            Handles.Label(hub.transform.position, ((Vector3)objStartPosition.Value - (Vector3)objEndPosition.Value).magnitude.ToString());
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        static void OnDrawGizmo(PatrolPath path, GizmoType gizmoType)
        {
            var start = path.transform.TransformPoint(path.startPosition);
            var end = path.transform.TransformPoint(path.endPosition);
            Handles.color = Color.yellow;
            Handles.DrawDottedLine(start, end, 5);
            Handles.DrawSolidDisc(start, path.transform.forward, 0.1f);
            Handles.DrawSolidDisc(end, path.transform.forward, 0.1f);
        }
    }
}
