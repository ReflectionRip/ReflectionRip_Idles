using System;
using XRL.Rules;
using XRL.World.AI.Pathfinding;

namespace XRL.World.AI.GoalHandlers
{
    [Serializable]
    public class rr_MoveNearTo : GoalHandler
    {
        public static string[] DirectionList = new string[8]
        {
            "NW",
            "N",
            "NE",
            "E",
            "SE",
            "S",
            "SW",
            "W"
        };
        private int MaxTurns = -1;
        private string dZone;
        private int dCx;
        private int dCy;

        public rr_MoveNearTo(GameObject go, int range = 1, bool avoidCenter = true)
        {
            dZone = go.pPhysics.CurrentCell.ParentZone.ZoneID;

            if (range > 0)
            {
                int tempX = 0;
                int tempY = 0;

                // Avoiding the center can be useful for standing around a fire, table, or any object.
                if (avoidCenter == true)
                {
                    int count = 0;
                    do
                    {
                        tempX = Stat.Random(-range, range);
                        tempY = Stat.Random(-range, range);
                        count++;
                    } while ((tempX == 0 && tempY == 0) || count < 10);
                }
                else
                {
                    tempX = Stat.Random(-range, range);
                    tempY = Stat.Random(-range, range);
                }
                dCx = go.pPhysics.CurrentCell.X + tempX;
                dCy = go.pPhysics.CurrentCell.Y + tempY;

                // Keep the point inside the map.
                if (dCx < 0) dCx = 0;
                if (dCx > 79) dCx = 79;
                if (dCy < 0) dCy = 0;
                if (dCy > 24) dCy = 24;
            }
            else
            {
                dCx = go.pPhysics.CurrentCell.X;
                dCy = go.pPhysics.CurrentCell.Y;
            }
        }

        public override bool Finished()
        {
            Cell myCell = ParentBrain.pPhysics.CurrentCell;
            return !ParentBrain.isMobile() || myCell.X == dCx && myCell.Y == dCy;
        }

        public override void TakeAction()
        {
            // 1: Don't do anything if the object isn't mobile. (Can't move)
            // 2: Pop? if the ZoneID is NULL?
            // 3: Pop? if the destination Zone is NULL?
            if (!ParentBrain.isMobile()) FailToParent();
            else if (ParentBrain.pPhysics.CurrentCell.ParentZone.ZoneID == null) Pop();
            else if (dZone == null) Pop();
            else
            {
                FindPath findPath = new FindPath(
                    ParentBrain.pPhysics.CurrentCell.ParentZone.ZoneID, 
                    ParentBrain.pPhysics.CurrentCell.X, 
                    ParentBrain.pPhysics.CurrentCell.Y, 
                    dZone, 
                    dCx, 
                    dCy, 
                    ParentBrain.limitToAquatic(),
                    false, 
                    ParentBrain.ParentObject);
                ParentBrain.ParentObject.UseEnergy(1000);
                if (findPath.bFound)
                {
                    findPath.Directions.Reverse();
                    int num = 0;
                    if (MaxTurns > -1) Pop();
                    foreach (string direction in findPath.Directions)
                    {
                        PushGoal((GoalHandler)new rr_PublicStep(direction));
                        ++num;
                        if (MaxTurns > -1 && num >= MaxTurns) break;
                    }
                }
                else
                {
                    FailToParent();
                }
            }
        }
    }
}
