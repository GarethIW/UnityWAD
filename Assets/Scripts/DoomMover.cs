using UnityEngine;

namespace UnityWAD
{
    // Data-only marker attached to a separated door/lift GameObject. It records what the mover is
    // and where it would travel, so a controller can be added later without recomputing anything.
    // This component contains NO movement logic by design.
    public class DoomMover : MonoBehaviour
    {
        public MoverKind Kind;
        public int SectorIndex;

        // DOOM heights (unscaled map units) of the surface that moves (a door's ceiling / a lift's floor).
        public int ClosedHeight;
        public int OpenHeight;

        // Local-space offset from the built (closed/resting) position to the fully-open position,
        // already scaled to Unity units. To open: transform.localPosition += OpenOffset.
        public Vector3 OpenOffset;
    }
}
