﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sample.AspNetCore3.Data;
using Sample.AspNetCore3.Models;
using Sample.AspNetCore3.Models.ViewModels;
using SwedbankPay.Sdk;
using SwedbankPay.Sdk.PaymentOrders;
using SwedbankPay.Sdk.Transactions;

namespace Sample.AspNetCore3.Controllers
{
    public class PaymentController : Controller
    {
        private readonly SwedbankPayOptions _swedbankPayOptions;
        private Cart _cartService;
        private readonly StoreDBContext _context;

        public PaymentController(IOptionsMonitor<SwedbankPayOptions> optionsAccessor, Cart cartService, StoreDBContext context)
        {
            _swedbankPayOptions = optionsAccessor.CurrentValue;
            _cartService = cartService;
            _context = context;
        }


        // GET: Payment
        public async Task<ActionResult> Index(Cart cart)
        {



            return View();
        }

        // GET: Payment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Payment/Create
        public ActionResult Create(Cart cart)
        {


            //var swedbankPayClient.PaymentOrders.CreatePaymentOrder();
            return View();
        }

        // POST: Payment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Payment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }


        [HttpGet]

        public async Task<ActionResult> CancelPayment(string paymentOrderId)
        {
            try
            {
                var swedbankPayClient = new SwedbankPayClient(_swedbankPayOptions);
                var transactionRequestObject = new TransactionRequestContainer(new TransactionRequest
                {
                    PayeeReference = DateTime.Now.Ticks.ToString(),
                    Description = "Cancelling parts of the total amount"

                });
                var response = await swedbankPayClient.PaymentOrders.CancelPaymentOrder(paymentOrderId, transactionRequestObject);

                TempData["CancelMessage"] = $"Payment has been cancelled: {response.Id}";
                _cartService.PaymentOrderLink = null;

                return RedirectToAction(nameof(Details), "Orders");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction(nameof(Details), "Orders");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AbortPaymentOrder(string paymentOrderId)
        {
            try
            {
                var swedbankPayClient = new SwedbankPayClient(_swedbankPayOptions);

                var response = await swedbankPayClient.PaymentOrders.AbortPaymentOrder(paymentOrderId);

                TempData["AbortMessage"] = $"Payment Order: {response.PaymentOrder.Id} has been {response.PaymentOrder.State}";
                _cartService.PaymentOrderLink = null;

                return RedirectToAction(nameof(Index), "Products");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction(nameof(Index), "Orders");
            }

        }

        //public async Task<IActionResult> GetPaymentOrder(string paymentOrderId)
        //{
        //    try
        //    {
        //        var swedbankPayClient = new SwedbankPayClient(_swedbankPayOptions);

        //        var response = await swedbankPayClient.PaymentOrders.AbortPaymentOrder(paymentOrderId);

        //        TempData["AbortMessage"] = $"Payment Order: {response.PaymentOrder.Id} has been {response.PaymentOrder.State}";
        //        _cartService.PaymentOrderLink = null;

        //        return RedirectToAction(nameof(Index), "Products");
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
        //        return RedirectToAction(nameof(Index), "Orders");
        //    }

        //}

        public async Task<IActionResult> CapturePayment(string paymentOrderId)
        {
            try
            {
                var swedbankPayClient = new SwedbankPayClient(_swedbankPayOptions);

                var transActionRequestObject = await GetTransactionRequestContainer("Capturing the authorized payment");

                var response = await swedbankPayClient.PaymentOrders.Capture(paymentOrderId, transActionRequestObject);

                    TempData["CaptureMessage"] = $"{response.Id}, {response.Type}, {response.State}";
                    _cartService.PaymentOrderLink = null;

                    return RedirectToAction(nameof(Details), "Orders");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction(nameof(Details), "Orders");
            }
        }

        public async Task<IActionResult> ReversalPayment(string paymentOrderId)
        {
            try
            {
                var swedbankPayClient = new SwedbankPayClient(_swedbankPayOptions);
                var transActionRequestObject = await GetTransactionRequestContainer("Reversing the capture amount");
                var response = await swedbankPayClient.PaymentOrders.Reversal(paymentOrderId, transActionRequestObject);

                TempData["ReversalMessage"] = $"{response.Id}, {response.Type}, {response.State}";
                _cartService.PaymentOrderLink = null;

                return RedirectToAction(nameof(Details), "Orders");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction(nameof(Details), "Orders");
            }
        }

        private async Task<TransactionRequestContainer> GetTransactionRequestContainer(string description)
        {
            var order = await _context.Orders.Include(l => l.Lines).ThenInclude(p => p.Product).FirstOrDefaultAsync();

            var orderItems = order.Lines.Select(line => new OrderItem
            {
                Amount = (int?)line.CalculateTotal() * 100,
                Quantity = line.Quantity,
                Reference = line.Product.Reference,
                Name = line.Product.Name,
                Type = line.Product.Type,
                Class = line.Product.Class,
                ItemUrl = "https://example.com/products/123",
                ImageUrl = "https://example.com/products/123.jpg",
                Description = "Product 1 description",
                QuantityUnit = "pcs",
                UnitPrice = line.Product.Price * 100,
                VatPercent = 0, //TODO Correct VatPercent
                VatAmount = 0, //TODO Correct VatAmount
            }).ToList();

            var transActionRequestObject = new TransactionRequestContainer(new TransactionRequest
            {
                PayeeReference = DateTime.Now.Ticks.ToString(),
                Amount = order.Lines.Sum(e => e.Quantity * e.Product.Price) * 100,
                VatAmount = 0, //TODO Correct amount
                Description = description,
                OrderItems = orderItems
            });

            return transActionRequestObject;

        }


        [HttpPost]
        public void OnCompleted(string paymentLinkId)
        {
            var products = _cartService.CartLines.Select(p => p.Product);
            _context.Products.AttachRange(products);

            _context.Orders.Add(new Order
            {
                PaymentOrderLink = _cartService.PaymentOrderLink,
                PaymentLink = paymentLinkId,
                Lines = _cartService.CartLines.ToList()
            });
            _context.SaveChanges(true);
        }





    }
}