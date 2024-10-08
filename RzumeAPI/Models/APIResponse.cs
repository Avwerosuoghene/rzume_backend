﻿using System;
using System.Net;

namespace RzumeAPI.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = [];
        }
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public List<string> ErrorMessages { get; set; }

        public ResultObject? Result { get; set; }

       
    }

      public class ResultObject
        {
            public required string Message { get; set; }
            public object? Content { get; set; }
        }

    
}

