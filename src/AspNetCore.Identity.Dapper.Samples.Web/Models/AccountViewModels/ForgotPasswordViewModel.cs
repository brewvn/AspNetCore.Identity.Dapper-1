﻿using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Identity.Dapper.Samples.Web.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
