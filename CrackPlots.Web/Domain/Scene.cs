using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Domain
{
    public class Scene
    {
        public int Id { get; set; }
        public int SequenceId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int ProtagonistId { get; set; }
        public int AntagonistId { get; set; }
        public string Conflict { get; set; }
        public string PhysicalGoal { get; set; }
        public string EmotionalGoal { get; set; }
        public string Turn { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string Setting { get; set; }
        public List<CharacterScene> Characters { get; set; }
    }
}