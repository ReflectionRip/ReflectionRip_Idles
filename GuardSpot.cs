using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.Messages;
using XRL.World.AI;

namespace XRL.World.Parts
{
    [Serializable]
    public class rr_GuardSpot : IPart
    {
        public string owner = string.Empty;
        public bool debug = false;
        public int minTurns = 100;
        public int maxTurns = 200;
        private long guardStart = 0;
        private int guardTurns = 0;

        public rr_GuardSpot()
        {
            Name = "rr_GuardSpot";
        }

        public override bool SameAs(IPart p)
        {
            return false;
        }

        public override void Register(GameObject GO)
        {
            GO.RegisterPartEvent(this, "IdleQuery");
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "IdleQuery")
            {
                GameObject GO = E.GetParameter<GameObject>("Object");

                if (owner != string.Empty)
                {
                    bool result = false;

                    // Allow the owner to use the guard spot.
                    if (GO.Blueprint == owner) result = true;

                    // Allow the faction to use 'faction' labeled guard spots.
                    foreach (KeyValuePair<string, int> item in GO.pBrain.FactionMembership)
                    {
                        if (item.Key == owner) result = true;
                    }

                    // Stop the user of the guard post not an owner.
                    if (result == false) return result;
                }

                if (currentTurn <= (guardStart + guardTurns)) return false;

                // Debugging
                if (debug)
                {
                    string message = GO.DisplayName + " starts guarding!";
                    MessageQueue.AddPlayerMessage(message);
                }

                // Guard from 2 to 4 hours. (Default)
                guardStart = currentTurn;
                guardTurns = Stat.Random(minTurns, maxTurns);

                // Move to the guard spot; Up to a range of 1 spot around the post.
                GO.pBrain.PushGoal((GoalHandler)new AI.GoalHandlers.rr_MoveNearTo(ParentObject, 1, false));

                // Wait for the shift to end.
                GO.pBrain.PushGoal((GoalHandler)new AI.GoalHandlers.Dormant(guardTurns));
            }

            return base.FireEvent(E);
        }
    }
}
