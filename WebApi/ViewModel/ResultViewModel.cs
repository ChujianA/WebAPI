using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModel
{
    public class ResultViewModel
    {
      public bool IsSuccess { get; set; }
      public string Message { get; set; }
      public string Token { get; set; }
    }
}
