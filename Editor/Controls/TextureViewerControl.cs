#region Using Statements
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

using XnaColor = Microsoft.Xna.Framework.Color;

namespace AweEditor
{
    class TextureViewerControl : GraphicsDeviceControl
    {
        /// <summary>
        /// The texture to render
        /// </summary>
        public Texture2D Texture {
            get { return texture;}
            set { texture = value; }
        }
        
        Texture2D texture;

        SpriteBatch spriteBatch;

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Application.Idle += delegate { Invalidate(); };
        }

        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            if (texture != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, new Rectangle(0,0,ClientRectangle.Width, ClientRectangle.Height), XnaColor.White);
                spriteBatch.End();
            }

        }
    }
}
