using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DMClone.Engine.Objects.Collisions
{
    internal class SegmentAABBCollisionDetector<A>
        where A : BaseGameObject
    {
        private A passiveObject;

        public SegmentAABBCollisionDetector(A passiveObject)
        {
            this.passiveObject = passiveObject;
        }
        public void DetectCollisions(Segment segment, Action<A> collisionHandler)
        {
            if (DetectCollision(passiveObject, segment))
            {
                collisionHandler(passiveObject);
            }
        }
        public void DetectCollisions(List<Segment> segments, Action<A> collisionHandler)
        {
            foreach (var segment in segments)
            {
                if (DetectCollision(passiveObject, segment))
                {
                    collisionHandler(passiveObject);
                }
            }
        }
        private bool DetectCollision(A passiveObject, Segment segment)
        {
            foreach (var activeBB in passiveObject.BoundingBoxes)
            {
                if (DetectCollision(segment.P1, activeBB) || DetectCollision(segment.P2, activeBB))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
        private bool DetectCollision(Vector2 p, BoundingBox bb)
        {
            if (p.X < bb.Position.X + bb.Width &&
                p.X > bb.Position.X &&
                p.Y < bb.Position.Y + bb.Height &&
                p.Y > bb.Position.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
