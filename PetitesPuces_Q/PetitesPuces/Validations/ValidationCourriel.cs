using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PetitesPuces.Validations
{
    public class ValidationCourriel:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var expr =  new Regex("^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$"); 
            return expr.IsMatch(value.ToString());
        }

        public override string FormatErrorMessage(string name)
        {
            return "Fk";
        }
    }
}