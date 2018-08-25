using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Domain
{
    public class CharacterScene
    {
        public Character Character { get; set; }
        public int SceneId { get; set; }
        public int CharacterSceneExitTypeId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Notes { get; set; }
        public string PhysicalGoal { get; set; }
        public string EmotionalGoal { get; set; }
        public string Obstacle { get; set; }
    }
}