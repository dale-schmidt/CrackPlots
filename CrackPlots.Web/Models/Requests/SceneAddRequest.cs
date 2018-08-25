using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class SceneAddRequest
    {
        [StringLength(50)]
        public string Title { get; set; }
        [MaxLength]
        public string Summary { get; set; }
        public int ProtagonistId { get; set; }
        public int AntagonistId { get; set; }
        [StringLength(1000)]
        public string Conflict { get; set; }
        [StringLength(1000)]
        public string PhysicalGoal { get; set; }
        [StringLength(1000)]
        public string EmotionalGoal { get; set; }
        [StringLength(1000)]
        public string Turn { get; set; }
        [StringLength(200)]
        public string Setting { get; set; }
        public int SequenceId { get; set; }
    }
}