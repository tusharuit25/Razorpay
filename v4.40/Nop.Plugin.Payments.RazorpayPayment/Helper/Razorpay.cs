using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.RazorpayPayment.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RazorpayPayment.Helper
{
    class RazorpayHelper
    {
        private readonly RazorpaySettings _settings;
        private string _clientId;
        private string _secretKey;

        public RazorpayHelper()
        {

        }

        public RazorpayHelper(string clientId, string secretKey)
        {
            _clientId = clientId;
            _secretKey = secretKey;
        }

        public OrderModel CreatePaymentRequest(PaymentInitiateModel _requestData, string email,decimal price)
        {
            // Generate random receipt number for order
            Random randomObj = new Random();
            string transactionId = randomObj.Next(10000000, 100000000).ToString();
            OrderModel model = new OrderModel();
            try
            {
                Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient(_clientId, _secretKey);
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", price * 100);  // Amount will in paise
                options.Add("receipt", transactionId);
                options.Add("currency", "USD");
                options.Add("payment_capture", "0"); // 1 - automatic  , 0 - manual
                                                     //options.Add("notes", "-- You can put any notes here --");

                Razorpay.Api.Order orderResponse = client.Order.Create(options);
                string orderId = orderResponse["id"].ToString();


                // Create order model for return on view
                OrderModel orderModel = new OrderModel
                {
                    orderId = orderResponse.Attributes["id"],
                    razorpayKey = _clientId,
                    amount = price * 100,
                    currency = "USD",
                    name = email,
                    email = email,
                    contactNumber = "",
                    address = "",
                };
                return orderModel;
            }
            catch (Exception) { return model; }
        }


        public async Task<string> Completed(IHttpContextAccessor _httpContextAccessor, string keyId, string keySecret)
        {
            try
            {
                string paymentId = _httpContextAccessor.HttpContext.Request.Form["rzp_paymentid"];

                // This is orderId
                string orderId = _httpContextAccessor.HttpContext.Request.Form["rzp_orderid"];

                Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient(keyId, keySecret);

                Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

                // This code is for capture the payment 
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", payment.Attributes["amount"]);
                Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
                string amt = paymentCaptured.Attributes["amount"];
                return paymentCaptured.Attributes["status"];
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}