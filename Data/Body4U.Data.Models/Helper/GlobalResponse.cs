using System;
using System.Collections.Generic;
using System.Text;

namespace Body4U.Data.Models.Helper
{
    public class GlobalResponse
    {
        protected GlobalResponse(bool isValid, string errorKey = null)
        {
            this.IsValid = isValid;

            if (!isValid)
            {
                Error = new ErrorResponse(errorKey);
            }
        }

        public bool IsValid { get; set; }

        public ErrorResponse Error { get; set; }

        public static GlobalResponse BadResponse(string errorKey)
        {
            return new GlobalResponse(false, errorKey);
        }

        public static GlobalResponse CorrectResponse()
        {
            return new GlobalResponse(true);
        }
    }
}
