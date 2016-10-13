using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SimpleAnimationExtension
{
    /// <summary>
    /// Contains a texture object and custom properties.
    /// </summary>
    public class SimpleAnimationContent
    {
        public int FrameCount;
        public int AnimationInterval;
        public TextureContent TextureData;

        public SimpleAnimationContent(TextureContent data, int frameCount, 
            int animationInterval)
        {
            TextureData = data;
            FrameCount = frameCount;
            AnimationInterval = animationInterval;
        }
    }

    /// <summary>
    /// Implements the reader for this content type.
    /// </summary>
    public class SimpleAnimationContentReader : ContentTypeReader
    {
        public SimpleAnimationContentReader()
            : base(typeof(SimpleAnimationContent))
        {

        }

        protected override object Read(ContentReader input, 
            object existingInstance)
        {
            int animationInterval = input.ReadInt32();
            int frameCount = input.ReadInt32();
            Texture2D texture = input.ReadObject<Texture2D>();

            return new SimpleAnimation(texture, frameCount, animationInterval);
        }
    }
}
