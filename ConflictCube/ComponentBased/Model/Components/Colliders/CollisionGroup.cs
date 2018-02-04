using System.Collections.Generic;
using ConflictCube.ComponentBased.Model.Components.Colliders;
using OpenTK;

namespace ConflictCube.ComponentBased.Components
{
    public class CollisionGroup
    {
        private Vector2 CellSize;

        private Dictionary<string, List<Collider>> CollidersInGroupGrid = new Dictionary<string, List<Collider>>();

        public static CollisionGroup DefaultCollisionGroup { get; private set; } = new CollisionGroup();


        public CollisionGroup() : this(new Vector2(1))
        { }

        public CollisionGroup(Vector2 cellSize)
        {
            CellSize = cellSize;
        }

        public void AddCollider(Collider collider)
        {
            collider.Group = this;


            foreach (string cell in ColliderGridCell.GetGridCells(collider, CellSize))
            {
                if (CollidersInGroupGrid.ContainsKey(cell))
                {
                    CollidersInGroupGrid.TryGetValue(cell, out List<Collider> gridCell);
                    gridCell.Add(collider);
                }
                else
                {
                    CollidersInGroupGrid.Add(cell, new List<Collider>() { collider });
                }
            }
        }

        public void AddRangeColliders(IEnumerable<Collider> colliders)
        {
            foreach (Collider collider in colliders)
            {
                AddCollider(collider);
            }
        }

        public void CheckCollisions(Collider collider, Vector2 movement)
        {
            if (!collider.Enabled)
            {
                return;
            }

            ColliderGridCell gridCell = ColliderGridCell.GetGridCells(collider, CellSize);

            if (!collider.Owner.EnabledInHierachy)
            {
                return;
            }


            //Check for collision

            for (int x = gridCell.MinXCoord; x <= gridCell.MaxXCoord; x++)
            {
                for (int y = gridCell.MinYCoord; y <= gridCell.MaxYCoord; y++)
                {
                    string key = ColliderGridCell.GetCoordString(x, y);

                    CollidersInGroupGrid.TryGetValue(key, out List<Collider> collidersInCell);

                    if (collidersInCell == null || collidersInCell?.Count == 0)
                    {
                        continue;
                    }

                    for (int i = 0; i < collidersInCell.Count; i++)
                    {
                        if (collidersInCell[i] == collider || !collidersInCell[i].Owner.EnabledInHierachy)
                        {
                            continue;
                        }

                        if (!collidersInCell[i].Enabled)
                        {
                            continue;
                        }

                        if (!collider.Layer.AreLayersColliding(collidersInCell[i].Layer))
                        {
                            continue;
                        }

                        if (collider.IgnoreCollisionsWith.Contains(collidersInCell[i].Type))
                        {
                            continue;
                        }

                        if (collider.IsCollidingWith(collidersInCell[i]))
                        {
                            collider.CollidesWith(collidersInCell[i], movement);

                            if (collidersInCell[i].IsCollidingWith(collider))
                            {
                                collidersInCell[i].CollidesWith(collider, new Vector2(0, 0));
                            }
                        }
                    }
                }
            }

            //TODO: Could go wrong if the collider is removing itself in OnCollision();
            //Collision cell could have changed
            RemoveCollider(collider);
            AddCollider(collider);
        }

        public void RemoveCollider(Collider colliderToRemove)
        {
            ColliderGridCell gridCells = ColliderGridCell.GetGridCells(colliderToRemove, CellSize);

            foreach (string cellString in gridCells)
            {
                CollidersInGroupGrid.TryGetValue(cellString, out List<Collider> colliders);

                if (colliders == null)
                {
                    continue;
                }

                if (colliders.Count == 1)
                {
                    CollidersInGroupGrid.Remove(cellString);
                }
                else
                {
                    colliders.Remove(colliderToRemove);
                }
            }
        }
    }
}
