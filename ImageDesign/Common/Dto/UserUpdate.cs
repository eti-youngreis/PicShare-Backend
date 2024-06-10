using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class UserUpdate
    {
        public string? FullName { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
