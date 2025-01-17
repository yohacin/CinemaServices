using System;

namespace wDualAssistence.Models
{
    public class ErrorViewModel
    {
        public ErrorViewModel() {
            RequestId = "404";
        }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ErrorMessage { get; internal set; }

    }
}