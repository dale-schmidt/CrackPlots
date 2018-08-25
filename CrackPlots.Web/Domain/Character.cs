using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Domain
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Want { get; set; }
        public string Need { get; set; }
        public string Biography { get; set; }
        public int ProjectId { get; set; }
    }
}