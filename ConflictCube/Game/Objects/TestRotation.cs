using ConflictCube.ComponentBased.Components;
using System;
using System.Drawing;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class TestRotation : GameObject
    {
        public TestRotation(string name, Transform transform) : base(name, transform)
        {
            AddComponent(new Material(Color.Red));
        }

        public override GameObject Clone()
        {
            return base.Clone();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void OnCollision(Collider other)
        {
            base.OnCollision(other);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            Transform.SetRotation(90f * (float)Math.Floor(Time.Time.CurrentTime));
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
