using System;
using System.Collections.Generic;
using XRL.Messages;
using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class rr_OwnedBedroll : Bedroll
    {
        public string owner = string.Empty;
        public bool debug = false;

        public rr_OwnedBedroll()
        {
            Name ="rr_OwnedBedroll";
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "IdleQuery")
            {
                GameObject GO = E.GetParameter<GameObject>("Object");

                // Check if the bed has an owner.
                if (owner != string.Empty)
                {
                    bool result = false;

                    // Allow the owner to use 'owner' labeled beds.
                    if (GO.Blueprint == owner) result = true;

                    // Allow the faction to use 'faction' labeled beds.
                    foreach (KeyValuePair<string, int> item in GO.pBrain.FactionMembership)
                    {
                        if (item.Key == owner) result = true;
                    }

                    // Stop the usage of the bedroll if this is not an owner.
                    if (result == false) return result;
                }

                // Debugging
                if (debug)
                {
                    string message = GO.DisplayName + " goes to bed!";
                    MessageQueue.AddPlayerMessage(message);
                }
            }
            if (E.ID == "CommandSmartUse" && !ThePlayer.HasEffect("Sitting"))
            {
                // Check if the bed has an owner.
                if (owner != string.Empty)
                {
                    Popup.Show("The owner of this bed would be very cross if they caught you sleeping in it!");
                    return false;
                }
            }
            return base.FireEvent(E);
        }
    }
}
