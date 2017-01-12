using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kurs0._7.Models
{
    public class ClientDetails
    {
        [Required(ErrorMessage = "Укажите номер телефона")]
        [RegularExpression(@"^\+[7]\(\d{3}\)\d{3}-\d{2}-\d{2}$", ErrorMessage = "Некорректный номер")]
        public string Telephone { get; set; }

        public string EMail { get; set; }
    }
}