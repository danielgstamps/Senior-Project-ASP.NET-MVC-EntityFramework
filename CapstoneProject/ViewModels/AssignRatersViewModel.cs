﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class AssignRatersViewModel
    {
        public int? EvalId { get; set; }

        public List<Rater> Raters { get; set; }
    }
}