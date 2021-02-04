using System;
using System.Collections.Generic;
using System.Text;

namespace Body4U.Data.Models.Helper
{
    public class Response
    {
        protected Response(bool isValid, string errorKey = null)
        {
            this.IsValid = isValid;

            if (!isValid)
            {
                Error = new ErrorResponse(errorKey);
            }
        }

        public bool IsValid { get; set; }

        public ErrorResponse Error { get; set; }

        public static Response BadResponse(string errorKey)
        {
            return new Response(false, errorKey);
        }

        public static Response CorrectResponse()
        {
            return new Response(true);
        }
    }
}
