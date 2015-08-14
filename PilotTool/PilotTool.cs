using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using GoblinXNA;
using GoblinXNA.SceneGraph;
using GoblinXNA.Helpers;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Network;
using GoblinXNA.Physics;

namespace PilotTool
{
    /// <summary>
    /// This tutorial demonstrates how to use Goblin XNA's networking capabilities with
    /// server-client model. In order to run both server and client on the same machine
    /// you need to copy the generated .exe file to other folder, and set one of them
    /// to be the server, and the other to be the client (isServer = false). If you're
    /// running the server and client on different machines, then you can simply run the
    /// project (of course, you will need to set one of them to be client).
    /// </summary>
    public class PilotTool : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        // A Goblin XNA scene graph
        Scene scene;

        TransformNode shipTransNode;

        // A network object 
        ApacheNetworkObject networkObj;

        // Indicates whether this is a server
        bool isServer;

        public PilotTool(bool isServer)
        {
            this.isServer = isServer;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            // Initialize the GoblinXNA framework
            State.InitGoblin(graphics, Content, "");

            // Initialize the scene graph
            scene = new Scene(this);

            State.EnableNetworking = true;
            State.IsServer = isServer;

            // Set up the lights used in the scene
            CreateLights();

            // Set up the camera, which defines the eye location and viewing frustum
            CreateCamera();

            // Create 3D objects
            CreateObject();

            // Use per pixel lighting for better quality (If you using non NVidia graphics card,
            // setting this to true may reduce the performance significantly)
            scene.PreferPerPixelLighting = true;

            // Create a network object that contains mouse press information to be
            // transmitted over network
            networkObj = new ApacheNetworkObject();

            // When a mouse press event is sent from the other side, then call "ShootBox"
            // function
            networkObj.CallbackFunc = UpdateApache;

            //scene.PhysicsEngine = new NewtonPhysics();

            if (State.IsServer)
                scene.NetworkServer = new LidgrenServer("apache", 14242);
            

            // Add the mouse network object to the scene graph, so it'll be sent over network
            // whenever ReadyToSend is set to true.
            scene.AddNetworkObject(networkObj);

            State.ShowNotifications = true;
            base.Initialize();
        }

        private void CreateLights()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(-1, -1, 0);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = Color.White.ToVector4();

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.LightSources.Add(lightSource);

            // Add this light node to the root node
            scene.RootNode.AddChild(lightNode);
        }

        private void CreateCamera()
        {
            // Create a camera 
            Camera camera = new Camera();

            if (State.IsServer)
            {
                camera.Translation = new Vector3(0, 5, 10);
                camera.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(-25));
            }
            else
            {
                camera.Translation = new Vector3(0, 0, -30);
                camera.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi);
            }
            // Set the vertical field of view to be 60 degrees
            camera.FieldOfViewY = MathHelper.ToRadians(60);
            // Set the near clipping plane to be 0.1f unit away from the camera
            camera.ZNearPlane = 0.1f;
            // Set the far clipping plane to be 1000 units away from the camera
            camera.ZFarPlane = 1000;

            // Now assign this camera to a camera node, and add this camera node to our scene graph
            CameraNode cameraNode = new CameraNode(camera);
            scene.RootNode.AddChild(cameraNode);

            // Assign the camera node to be our scene graph's current camera node
            scene.CameraNode = cameraNode;
        }

        private void CreateObject()
        {
            // Loads a textured model of a ship
            ModelLoader loader = new ModelLoader();
            Model shipModel = (Model)loader.Load("", "p1_wedge");

            // Create a geometry node of a loaded ship model
            GeometryNode shipNode = new GeometryNode("Ship");
            shipNode.Model = shipModel;
            // This ship model has material definitions in the model file, so instead
            // of creating a material node for this ship model, we simply use its internal materials
            shipNode.Model.UseInternalMaterials = true;

            // Create a transform node to define the transformation for the ship
            shipTransNode = new TransformNode();
            shipTransNode.Translation = new Vector3(0, 0, 0);
            shipTransNode.Scale = new Vector3(0.002f, 0.002f, 0.002f); // It's huge!
            shipTransNode.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0),
                MathHelper.ToRadians(0));

            scene.RootNode.AddChild(shipTransNode);
            shipTransNode.AddChild(shipNode);
           
            
        }

        
        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void  UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Shoot a box from the clicked mouse location
        /// </summary>
        /// <param name="near"></param>
        /// <param name="far"></param>
        private void UpdateApache(String cmd, String response, double x, double y, double z, double pitch, double yaw, double roll)
        {
            String s = String.Format("{0},{1},{2},{3},{4},{5},{6}", cmd, x, y, z, pitch, yaw, roll);
            GoblinXNA.UI.Notifier.AddMessage(s);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
