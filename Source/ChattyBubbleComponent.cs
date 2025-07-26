using UnityEngine;
using Verse;

namespace ColonyChatter
{
    /// <summary>
    /// Component that handles drawing speech bubble text during the OnGUI event
    /// </summary>
    public class ChattyBubbleComponent : MonoBehaviour
    {
        public ChattyBubbleComponent()
        {
            // Constructor
        }

        public void OnGUI()
        {
            // Only draw bubbles when in a map view
            if (Find.CurrentMap != null)
            {
                // Call the manager to draw bubbles using the custom textures
                ChattyBubbleManager.DrawBubbles();
                
                // Also update the bubbles on each frame
                ChattyBubbleManager.Update();
            }
        }
    }
    
    /// <summary>
    /// Initializes the ChattyBubbleComponent when the game starts
    /// </summary>
    [StaticConstructorOnStartup]
    public static class ChattyBubbleInitializer
    {
        static ChattyBubbleInitializer()
        {
            // Create a GameObject to hold our component
            GameObject bubbleGameObject = new GameObject("ChattyBubbleComponent");
            // Add our component to the GameObject
            bubbleGameObject.AddComponent<ChattyBubbleComponent>();
            // Make the GameObject persist between scene changes
            Object.DontDestroyOnLoad(bubbleGameObject);
            
            Log.Message("Colony Chatter: Speech bubble renderer initialized");
        }
    }
}
