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
using GoblinXNA.UI.UI2D;

namespace AWDataEntry
{
    /// <summary>
    /// This tutorial demonstrates how to use Goblin XNA's networking capabilities with
    /// server-client model. In order to run both server and client on the same machine
    /// you need to copy the generated .exe file to other folder, and set one of them
    /// to be the server, and the other to be the client (isServer = false). If you're
    /// running the server and client on different machines, then you can simply run the
    /// project (of course, you will need to set one of them to be client).
    /// </summary>
    public class AWDataEntry : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        // A Goblin XNA scene graph
        Scene scene;

        Boolean fullScreen = true;

        TransformNode shipTransNode;

        // Indicates whether this is a server
        bool isServer = false;

        LidgrenClient client;

        G2DTextField textF;

        // A network object 
        MessageNetworkObject networkObj;

        String response;
            

        public AWDataEntry(bool iserver)
        {
            this.isServer = false;
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

            response = "";

            // Initialize the GoblinXNA framework
            State.InitGoblin(graphics, Content, "");

            // Initialize the scene graph
            scene = new Scene(this);

            State.EnableNetworking = true;
            State.IsServer = false;

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
            networkObj = new MessageNetworkObject();

            // When a mouse press event is sent from the other side, then call "ShootBox"
            // function
            networkObj.CallbackFunc = UpdateMessage;
                      

            // Create a network handler for handling the network transfers
            NetworkHandler networkHandler = new NetworkHandler();

            if (3==4)
                networkHandler.NetworkServer = new LidgrenServer("InfoOverload", 14242);
            else
            {
                // Create a client that connects to the local machine assuming that both
                // the server and client will be running on the same machine. In order to 
                // connect to a remote machine, you need to either pass the host name or
                // the IP address of the remote machine in the 3rd parameter. 
                client = new LidgrenClient("InfoOverload", 14242, "Localhost");

                // If the server is not running when client is started, then wait for the
                // server to start up.
                client.WaitForServer = true;                

                networkHandler.NetworkClient = client;
            }

            scene.BackgroundColor = Color.Black;

            // Assign the network handler used for this scene
            scene.NetworkHandler = networkHandler;

            // Add the mouse network object to the scene graph, so it'll be sent over network
            // whenever ReadyToSend is set to true.
            scene.NetworkHandler.AddNetworkObject(networkObj);
            
            //scene.PhysicsEngine = new NewtonPhysics();
            //Set to false for window
            graphics.IsFullScreen = fullScreen;
            graphics.ApplyChanges();

            // Add a keyboard press handler for user input
            KeyboardInput.Instance.KeyPressEvent += new HandleKeyPress(KeyPressHandler);

            State.ShowNotifications = false;
            base.Initialize();
        }

        private void UpdateMessage(String msg)
        {
            GoblinXNA.UI.Notifier.AddMessage("UpdateMsg");
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
            G2DPanel frame = new G2DPanel();
            frame.Bounds = new Rectangle(0, 0, 800, 600);
            frame.Border = GoblinEnums.BorderFactory.LineBorder;
            frame.Transparency = 1.0f;  // Ranges from 0 (fully transparent) to 1 (fully opaque)
            frame.BackgroundColor = Color.Black;

            SpriteFont textBoxFont = Content.Load<SpriteFont>("TextBoxFont");
            SpriteFont textFont = Content.Load<SpriteFont>("TextFont");

            
           
            G2DLabel label = new G2DLabel();
            label.TextFont = textFont;
            label.TextColor = Color.Yellow;
            label.Bounds = new Rectangle(10, 100, 780, 200);
            label.Text = "Type Target ID and Press Enter";

            textF = new G2DTextField();
            textF.TextFont = textBoxFont;
            textF.TextColor = Color.Black;
            textF.Bounds = new Rectangle(10, 200, 780, 200);
            textF.Editable = false;
            textF.Text = "";
            
            
            frame.AddChild(label);
            frame.AddChild(textF);
            scene.UIRenderer.Add2DComponent(frame);
            
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


        private void sendResponse(String response)
        {
            GoblinXNA.UI.Notifier.AddMessage("User typed response: " + response);
           
            networkObj.ReadyToSend = true;
            networkObj.Message = response;
        }

        private void KeyPressHandler(Keys keys, KeyModifier modifier)
        {

            
            if (keys == Keys.Enter)
            {
                sendResponse(response);       
                textF.Clear();
                response = "";
            }
            else
            {
                Boolean ignore = false;
                                
                if (keys == Keys.Left) ignore = true;
                if (keys == Keys.Right) ignore = true;
                if (keys == Keys.Space) ignore = true;
                if (ignore) return;

                if (keys == Keys.Back)
                {
                    int i = response.Length;
                    if (i > 0)
                    {
                        response = response.Substring(0, i - 1);                        
                        textF.Text = response;
                        return;
                    }
                }
                String nr = keys.ToString();

                if (keys == Keys.D1) nr = "1";
                if (keys == Keys.D2) nr = "2";
                if (keys == Keys.D3) nr = "3";
                if (keys == Keys.D4) nr = "4";
                if (keys == Keys.D5) nr = "5";
                if (keys == Keys.D6) nr = "6";
                if (keys == Keys.D7) nr = "7";
                if (keys == Keys.D8) nr = "8";
                if (keys == Keys.D9) nr = "9";
                if (keys == Keys.D0) nr = "10";

                if (keys == Keys.Space) nr = " ";
                nr = nr.ToUpper();
                char c = nr[0];
                if ((c >= '0') && (c <= 'Z'))
                {
                    response = response + nr;
                    textF.Text = response;
                }
            }
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
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
          

           base.Draw(gameTime);
                    
        }
    }
}
