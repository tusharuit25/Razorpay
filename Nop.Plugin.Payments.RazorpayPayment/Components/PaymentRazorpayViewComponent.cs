using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.RazorpayPayment.Helper;
using Nop.Plugin.Payments.RazorpayPayment.Models;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RazorpayPayment.Components
{
    [ViewComponent(Name = "PaymentRazorpay")]
    public class PaymentRazorpayViewComponent : NopViewComponent
    {
        private readonly RazorpaySettings _settings;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;

        public PaymentRazorpayViewComponent(RazorpaySettings settings, IWorkContext workContext, IShoppingCartService shoppingCartService, IStoreContext storeContext, OrderSettings orderSettings, IOrderTotalCalculationService orderTotalCalculationService)
        {
            _settings = settings;
            _workContext = workContext;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _orderSettings = orderSettings;
            _orderTotalCalculationService = orderTotalCalculationService;
        }

        public IViewComponentResult Invoke()
        {
            var total = GetCartSubTotal();
            decimal amount = total.Result.subTotalWithDiscount;
            RazorpayHelper payment = new RazorpayHelper(_settings.ClientId, _settings.SecretKey);
            var customer = _workContext.GetCurrentCustomerAsync();
            PaymentInitiateModel paymentInitiateModel = new PaymentInitiateModel();
            OrderModel orderModel = payment.CreatePaymentRequest(paymentInitiateModel, customer.Result.Email, amount);
            return View("~/Plugins/Payments.RazorpayPayment/Views/PaymentInfo.cshtml", orderModel);
        }

        public async Task<(decimal discountAmount, List<Discount> appliedDiscounts, decimal subTotalWithoutDiscount, decimal subTotalWithDiscount, SortedDictionary<decimal, decimal> taxRates)> GetCartSubTotal()
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);
            var total = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, _orderSettings.MinOrderSubtotalAmountIncludingTax);
            return total;
        }
    }
}
