using System.Collections.Generic;

namespace Engine.Components
{
    public static class CollisionLayerExtensionMethods
    {
        public static bool AreLayersColliding(this string layer, string other)
        {
            return CollisionLayers.GetCollidesWithListOfCollisionLayer(layer).Contains(other);
        }
    }


    public static class CollisionLayers
    {
        private static Dictionary<string, List<string>> CollidesWith = new Dictionary<string, List<string>>();

        public static void AddLayer(string layer, List<string> layersToCollideWith)
        {
            CollidesWith.Add(layer, layersToCollideWith);
        }

        public static List<string> GetCollidesWithListOfCollisionLayer(string layer)
        {
            CollidesWith.TryGetValue(layer, out List<string> collidesWith);

            return collidesWith;
        }
    }
}
