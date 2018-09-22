using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Domain
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Logline { get; set; }
        public StoryType StoryType { get; set; }
        public string Notes { get; set; }
        public int ProjectId { get; set; }
        public int SeasonId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public List<Act> Acts { get; set; }
        public List<Project> Seasons { get; set; }
        public List<Project> Episodes { get; set; }
        public List<Plot> Plots { get; set; }
        public List<Person> Users { get; set; }
        public List<int> EpisodeIds { get; set; }
    }
}