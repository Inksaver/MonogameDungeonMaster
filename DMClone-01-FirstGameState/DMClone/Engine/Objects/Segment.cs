﻿using Microsoft.Xna.Framework;
namespace DMClone.Engine.Objects
{
    internal class Segment
    {
        public Vector2 P1 { get; private set; }
        public Vector2 P2 { get; private set; }
        public Segment(Vector2 p1, Vector2 p2)
        {
            P1 = p1;
            P2 = p2;
        }
    }
}