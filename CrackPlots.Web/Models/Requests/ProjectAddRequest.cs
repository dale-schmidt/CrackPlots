using ForeSight.Web.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class ProjectAddRequest
    {
        [StringLength(200)]
        public string Title { get; set; }
        [StringLength(4000)]
        public string Logline { get; set; }
        public int StoryTypeId { get; set; }
        [MaxLength]
        public string Notes { get; set; }
        public bool AutoStructured { get; set; }
        public int ProjectId { get; set; }
        public int SeasonId { get; set; }
        public int ActsCount { get; set; }
        public List<Plot> Plots { get; set; }
    }
}