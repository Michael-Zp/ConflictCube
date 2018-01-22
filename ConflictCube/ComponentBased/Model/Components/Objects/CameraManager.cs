﻿using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.View;
using OpenTK;
using System;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class CameraManager : GameObject
    {
        private List<Camera> Cameras;
        private List<Player> Players;

        public CameraManager(string name, Transform transform, List<Camera> cameras, List<Player> players) : base(name, transform)
        {
            Cameras = cameras;
            Players = players;

            foreach(Camera camera in cameras)
            {
                camera.Transform.SetPosition(new Vector2(0f, -1f), WorldRelation.Global);
            }
        }

        public override void OnUpdate()
        {
            Vector2 player0Pos = Players[0].Transform.GetPosition(WorldRelation.Global);
            Vector2 player1Pos = Players[1].Transform.GetPosition(WorldRelation.Global);

            Vector2 averagePlayerPos = (player0Pos + player1Pos) / 2;
            float playerDistance = (float)Math.Sqrt(Math.Pow(player0Pos.X - player1Pos.X, 2) + Math.Pow(player0Pos.Y - player1Pos.Y, 2));

            float zoomFactor = 1f;
            if(playerDistance > 1.8f)
            {
                Console.WriteLine("Zoom");
                zoomFactor = 1.8f / playerDistance;
            }
            

            foreach (Camera camera in Cameras)
            {
                camera.Transform.SetSize(new Vector2(zoomFactor, zoomFactor), WorldRelation.Global);
                camera.Transform.SetPosition(-averagePlayerPos, WorldRelation.Global);
            }
        }
    }
}
