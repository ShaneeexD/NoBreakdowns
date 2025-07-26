using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ColonyChatter
{
    public class InteractionWorker_EnthusiasticChat : InteractionWorker
    {
        // Higher chance of this interaction occurring between chatty colonists
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            // Base chance for the interaction
            float baseChance = 0.6f;
            
            // Check if either pawn has the chatty trait
            bool initiatorIsChatty = HasChattyTrait(initiator);
            bool recipientIsChatty = HasChattyTrait(recipient);
            
            // Boost chance if either or both pawns are chatty
            if (initiatorIsChatty && recipientIsChatty)
            {
                // Both are chatty - highest chance
                return baseChance * 3f;
            }
            else if (initiatorIsChatty || recipientIsChatty)
            {
                // One is chatty - medium chance
                return baseChance * 1.5f;
            }
            
            // Neither is chatty - very low chance
            return baseChance * 0.2f;
        }
        
        // Additional social impact when chatty colonists interact
        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
        {
            // Default values
            letterText = null;
            letterLabel = null;
            letterDef = null;
            lookTargets = null;
            
            // Check if either pawn has the chatty trait
            bool initiatorIsChatty = HasChattyTrait(initiator);
            bool recipientIsChatty = HasChattyTrait(recipient);
            
            // Show joke bubbles only
            if (initiatorIsChatty)
            {
                // Show joke bubble above initiator
                if (Rand.Chance(0.7f))
                {
                    ChattyBubbleManager.ShowJokeBubble(initiator);
                }
            }
            
            if (recipientIsChatty && Rand.Chance(0.3f))
            {
                // Occasionally show joke bubble above recipient too
                ChattyBubbleManager.ShowJokeBubble(recipient);
            }
            
            // Check if both pawns have the chatty trait
            if (initiatorIsChatty && recipientIsChatty)
            {
                // Both are chatty, give additional opinion boost
                // Use the built-in social thought from our interaction
                // The opinion boost is defined in the ThoughtDef in the XML
                
                // Add a small social skill gain for both
                initiator.skills.Learn(SkillDefOf.Social, 10f);
                recipient.skills.Learn(SkillDefOf.Social, 10f);
            }
        }
        
        // Helper method to check if a pawn has the chatty trait
        private bool HasChattyTrait(Pawn pawn)
        {
            if (pawn?.story?.traits == null)
                return false;
            
            return pawn.story.traits.HasTrait(ColonyChatterDefOf.ColonyChatter);
        }
    }
}
