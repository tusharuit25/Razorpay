using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Plugin.Payments.RazorpayPayment.Helper;
using Nop.Plugin.Payments.RazorpayPayment.Models;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RazorpayPayment.Controllers
{
    public class PaymentController : BasePaymentController
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly RazorpaySettings _settings;
        private readonly IWebHelper _webHelper;

        public PaymentController(IHttpContextAccessor httpContextAccessor, RazorpaySettings settings, IWebHelper webHelper)
        {
            _settings = settings;
            _httpContextAccessor = httpContextAccessor;
            _webHelper = webHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Completed()
        {
            RazorpayHelper payment = new RazorpayHelper();
            string PaymentMessage = await payment.Completed(_httpContextAccessor,_settings.ClientId,_settings.SecretKey);
            if (PaymentMessage == "captured")
            {
                return RedirectToAction("Success");
            }
            else
            {
                return RedirectToAction("Failed");
            }
        }
        public IActionResult Success()
        {
            return View("~/Plugins/Payments.RazorpayPayment/Views/Success.cshtml");
        }

        public IActionResult Failed()
        {
            return View("~/Plugins/Payments.RazorpayPayment/Views/Success.cshtml");
        }
    }
}

