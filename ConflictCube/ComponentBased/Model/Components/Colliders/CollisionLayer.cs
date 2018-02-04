using System;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Model.Components.Colliders
{
    public enum CollisionLayer
    {
        Default,
        Orange,
        Blue
    }

    public static class CollisionLayerExtensionMethods
    {
        public static bool AreLayersColliding(this CollisionLayer layer, CollisionLayer other)
        {
            return CollisionLayers.GetCollidesWithListOfCollisionLayer(layer).Contains(other);
        }
    }


    public static class CollisionLayers
    {
        private static Dictionary<CollisionLayer, List<CollisionLayer>> CollidesWith = new Dictionary<CollisionLayer, List<CollisionLayer>>();

        static CollisionLayers()
        {
            CollidesWith.Add(CollisionLayer.Default, new List<CollisionLayer> { CollisionLayer.Default, CollisionLayer.Orange, CollisionLayer.Blue });
            CollidesWith.Add(CollisionLayer.Orange,  new List<CollisionLayer> { CollisionLayer.Default, CollisionLayer.Blue   });
            CollidesWith.Add(CollisionLayer.Blue,    new List<CollisionLayer> { CollisionLayer.Default, CollisionLayer.Orange });
        }

        public static List<CollisionLayer> GetCollidesWithListOfCollisionLayer(CollisionLayer layer)
        {
            CollidesWith.TryGetValue(layer, out List<CollisionLayer> collidesWith);

            return collidesWith;
        }
    }
}
