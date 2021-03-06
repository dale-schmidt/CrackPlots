﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class ActAddRequest
    {
        [StringLength(50)]
        public string Title { get; set; }
        [MaxLength]
        public string Summary { get; set; }
        [MaxLength]
        public string Notes { get; set; }
        [StringLength(200)]
        public string CentralQuestion { get; set; }
        [Required]
        public int ProjectId { get; set; }
    }
}