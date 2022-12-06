using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RazorpayPayment
{
    public class RazorpaySettings : ISettings
    {
        /// <summary>
        /// Gets or sets client identifier
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets client secret
        /// </summary>
        public string SecretKey { get; set; }
    }
}
