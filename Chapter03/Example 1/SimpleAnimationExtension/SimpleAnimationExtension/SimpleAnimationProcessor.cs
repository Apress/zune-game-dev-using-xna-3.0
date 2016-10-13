using System;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace SimpleAnimationExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// </summary>
    [ContentProcessor(DisplayName = "Simple Animation Processor")]
    public class SimpleAnimationProcessor : 
        ContentProcessor<TextureContent, SimpleAnimationContent>
    {
        private int _numFrames;
        private int _animationInterval;

        [DisplayName("Frame Count")] 
        [Description("The number of animation frames in the texture.")]   
        public int FrameCount
        {
            get { return _numFrames; }
            set { _numFrames = value; }
        }

        [DisplayName("Animation Speed")]
        [Description("The speed of the animation in milliseconds.")]
        [DefaultValue("100")]
        public int AnimationInterval
        {
            get { return _animationInterval; }
            set { _animationInterval = value; }
        }

        public override SimpleAnimationContent Process(TextureContent input, 
            ContentProcessorContext context)
        {
            return new SimpleAnimationContent(input, _numFrames, _animationInterval);            
        }
    }
}