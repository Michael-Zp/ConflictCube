using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.View;
using OpenTK;
using System;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class CameraManager : GameObject
    {
        private List<Camera> Cameras;
        private Floor Floor {
            set {
                float xSize = value.FloorTileSize.X * value.FloorColumns;
                float ySize = value.FloorTileSize.Y * value.FloorRows;

                float xZoomFactor = .9f / xSize;
                float yZoomFactor = .9f / ySize;

                float zoomFactor = Math.Min(xZoomFactor, yZoomFactor);

                ZoomFactor = MathHelper.Clamp(zoomFactor, 0, 1);
            }
        }
        private float ZoomFactor = 0;
        private bool WasZoomedOut = false;
        private bool IsZoomedOut = false;
        private List<Player> Players;

        public CameraManager(string name, Transform transform, List<Camera> cameras, Floor floor, List<Player> players) : base(name, transform)
        {
            Cameras = cameras;
            Floor = floor;
            Players = players;
        }

        public override void OnUpdate()
        {
            if (Input.OnButtonDown(InputKey.Zoom, 0))
            {
                IsZoomedOut = !IsZoomedOut;
            }

            if (IsZoomedOut)
            {
                if(!WasZoomedOut)
                {
                    foreach (Camera camera in Cameras)
                    {
                        camera.Transform.SetSize(new Vector2(ZoomFactor, ZoomFactor), WorldRelation.Global);
                        camera.Transform.SetPosition(new Vector2(0f, -camera.Transform.GetSize(WorldRelation.Global).Y), WorldRelation.Local);
                    }
                }
            }
            else
            {
                if(WasZoomedOut)
                {
                    foreach (Camera camera in Cameras)
                    {
                        camera.Transform.SetSize(new Vector2(1, 1), WorldRelation.Global);
                    }
                }
                for (int i = 0; i < Cameras.Count && i < Players.Count; i++)
                {
                    UpdateCameraPositionToCenterOnPlayer(Cameras[i], Players[i]);
                }
            }

            WasZoomedOut = IsZoomedOut;
        }


        private void UpdateCameraPositionToCenterOnPlayer(Camera camera, Player player)
        {
            Vector2 playerPos = player.Transform.GetPosition(WorldRelation.Global);

            camera.Transform.SetPosition(new Vector2(-playerPos.X, -playerPos.Y), WorldRelation.Global);
        }
    }
}
