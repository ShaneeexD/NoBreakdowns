using UnityEngine;
using Verse;

namespace ColonyChatter
{
    [StaticConstructorOnStartup]
    public static class ChattyBubbleTextures
    {
        // Texture for the inner part of the bubble
        public static readonly Texture2D Inner = ContentFinder<Texture2D>.Get("Bubbles/Inner");
        
        // Texture for the outer part of the bubble
        public static readonly Texture2D Outer = ContentFinder<Texture2D>.Get("Bubbles/Outer");
        
        // Texture for the bubble icon
        public static readonly Texture2D Icon = ContentFinder<Texture2D>.Get("Bubbles/Icon");
    }
}
