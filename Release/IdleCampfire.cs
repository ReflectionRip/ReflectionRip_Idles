using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.Messages;
using XRL.World.AI;

namespace XRL.World.Parts
{
    [Serializable]
    public class rr_IdleCampfire : Campfire
    {
        public string owner = string.Empty;
        public int idleChance = 5;
        public bool debug = false;

        public rr_IdleCampfire()
        {
            Name = "rr_IdleCampfire";
        }

        public override void Register(GameObject GO)
        {
            GO.RegisterPartEvent(this, "IdleQuery");

            base.Register(GO);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "IdleQuery")
            {
                // 5% chance to go to campfire.
                if (Stat.Random(1, 100) > idleChance) return false;

                GameObject GO = E.GetParameter<GameObject>("Object");

                if (owner != string.Empty)
                {
                    bool result = false;

                    // Allow the owner to use the campfire.
                    if (GO.Blueprint == owner) result = true;

                    // Allow the faction to use 'faction' labeled campfires.
                    foreach (KeyValuePair<string, int> item in GO.pBrain.FactionMembership)
                    {
                        if (item.Key == owner) result = true;
                    }

                    // Stop the user of the guard post not an owner.
                    if (result == false) return result;
                }

                // Debugging
                if (debug)
                {
                    string message = GO.DisplayName + " uses the campfire!";
                    MessageQueue.AddPlayerMessage(message);
                }

                // Stand 1 or 2 steps from the campfire.
                GO.pBrain.PushGoal((GoalHandler)new AI.GoalHandlers.rr_MoveNearTo(ParentObject, 2));

                // Rest for 10 to 20 rounds.
                GO.pBrain.PushGoal((GoalHandler)new AI.GoalHandlers.Dormant(Stat.Random(10, 20)));

                return true;
            }
            return base.FireEvent(E);
        }
    }
}
