using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MoviesAPI.Validators
{
    public class AllowedExtensionsValidator : ValidationAttribute
    {
        private readonly string[] extensions;

        public AllowedExtensionsValidator(string[] extensions)
        {
            this.extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile formFile = value as IFormFile;
            if (formFile != null)
            {
                var file = Path.GetExtension(formFile.FileName);
                file = file.Substring(file.Length-4);
                if (! extensions.Contains(file.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return "Bu fotoğraf türü desteklenemiyor";
        }
    }
}
