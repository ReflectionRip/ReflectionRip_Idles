using System;
using XRL.World.Parts;

namespace XRL.World.AI.GoalHandlers
{
    [Serializable]
    public class rr_PublicStep : GoalHandler
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
        public string Dir;

        public rr_PublicStep(string Direction)
        {
            Dir = Direction;
        }

        public override bool Finished()
        {
            return false;
        }

        private GameObject CellHasFriendly(Cell TargetCell)
        {
            if (TargetCell == null) return null;
            foreach (GameObject GO in TargetCell.GetObjectsWithPart("Combat"))
            {
                if (ParentBrain != null && GO != ParentBrain.Target)
                {
                    bool flag = true;
                    if ((GO.IsPlayer() || GO.Blueprint != ParentBrain.ParentObject.Blueprint) && !ParentBrain.IsHostileTowards(GO))
                    {
                        flag = false;
                    }
                    if (flag) return GO;
                }
            }
            return null;
        }

        private bool CellHasHostile(Cell TargetCell)
        {
            if (TargetCell == null) return false;
            bool flag = false;
            foreach (GameObject GO in TargetCell.GetObjectsWithPart("Combat"))
            {
                if ((GO.IsPlayer() || GO.Blueprint != ParentBrain.ParentObject.Blueprint) && (ParentBrain.IsHostileTowards(GO) && ParentBrain.ParentObject.SamePhaseAs(GO)))
                {
                    flag = true;
                }
            }
            return flag;
        }

        private void MoveDirection(string Direction)
        {
            Cell cellFromDirection = ParentBrain.pPhysics.CurrentCell.GetCellFromDirection(Direction, false);
            if (cellFromDirection == null || CellHasFriendly(cellFromDirection) != null) return;
            ParentBrain.ParentObject.FireEvent(Event.New("CommandMove", nameof (Direction), Direction));
        }

        public override void TakeAction()
        {
            if (!ParentBrain.isMobile())
            {
                Pop();
            }
            else
            {
                GameObject gameObject = CellHasFriendly(ParentBrain.pPhysics.CurrentCell.GetCellFromDirection(Dir, true));
                if (gameObject != null && gameObject.HasPart("Brain"))
                {
                    Brain part = gameObject.GetPart("Brain") as Brain;
                    if (gameObject.IsPlayer() || part.Target != null && part.Target == ParentBrain.Target) return;
                }
                if (CellHasHostile(ParentBrain.pPhysics.CurrentCell.GetCellFromDirection(Dir, true)))
                {
                    Think("There's something in my way!");
                    FailToParent();
                }
                else
                {
                    MoveDirection(Dir);
                    Pop();
                }
            }
        }
    }
}
