using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class CharacterAddRequest
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Want { get; set; }
        [StringLength(1000)]
        public string Need { get; set; }
        [StringLength(4000)]
        public string Biography { get; set; }
        public int ProjectId { get; set; }
    }
}