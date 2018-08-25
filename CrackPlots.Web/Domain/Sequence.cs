using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Domain
{
    public class Sequence
    {
        public int Id { get; set; }
        public int ActId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Notes { get; set; }
        public string CentralQuestion { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public List<Scene> Scenes { get; set; }
    }
}