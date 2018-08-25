using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class CharacterSceneRequest
    {
        [Required]
        public int CharacterId { get; set; }
        [Required]
        public int SceneId { get; set; }
        public int CharacterSceneExitTypeId { get; set; }
        [StringLength(200)]
        public string Start { get; set; }
        [StringLength(200)]
        public string End { get; set; }
        [StringLength(4000)]
        public string Notes { get; set; }
        [StringLength(200)]
        public string PhysicalGoal { get; set; }
        [StringLength(200)]
        public string EmotionalGoal { get; set; }
        [StringLength(200)]
        public string Obstacle { get; set; }
    }
}