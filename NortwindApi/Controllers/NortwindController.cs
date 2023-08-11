﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NortwindApi.Models;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace NortwindApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NortwindController : ControllerBase
    {
        NortwindContext _context;
        public NortwindController(NortwindContext nortwindContext) => _context = nortwindContext;

        [HttpGet("TopSelling3")]
        public IActionResult TopSelling3()
        {
            var topShipCountries = _context.Orders
     .GroupBy(o => o.ShipCountry)
     .Select(group => new
     {
         ShipCountry = group.Key,
         OrderCount = group.Count()
     })
     .OrderByDescending(result => result.OrderCount)
     .Take(3)
     .ToList();

            return Ok(topShipCountries);
        }

        [HttpGet("GetBeveragesCategories")]
        public IActionResult GetBeveragesCategories()
        {
            var productsInCategory1 = _context.Products
           .Where(p => p.CategoryId == 1)
           .OrderBy(p => p.ProductId)
           .Select(p => new
           {
               p.ProductId,
               p.ProductName,
               p.SupplierId,
               p.QuantityPerUnit,
               p.UnitPrice,
               p.UnitsInStock,
               p.UnitsOnOrder,
               p.ReorderLevel,
               p.Discontinued
           })
           .ToList();

            return Ok(productsInCategory1);
        }

        [HttpGet("TotalRevenues1917")]
        public IActionResult TotalRevenues1917()
        {
            var totalRevenues = _context.ProductSalesFor1997s
            .Sum(ps => ps.ProductSales);

            return Ok(totalRevenues);
        }

        [HttpGet("GetTop3Suppliers")]
        public IActionResult GetTop3Suppliers()
        {
            var topSuppliers = _context.Suppliers
    .Join(
        _context.AlphabeticalListOfProducts,
        s => s.SupplierId,
        p => p.SupplierId,
        (s, p) => new
        {
            SupplierId = s.SupplierId,
            CompanyName = s.CompanyName,
            TotalRevenue = p.UnitPrice * p.UnitsInStock
        }
    )
    .GroupBy(result => new { result.SupplierId, result.CompanyName })
    .Select(group => new
    {
        group.Key.SupplierId,
        group.Key.CompanyName,
        TotalRevenue = group.Sum(result => result.TotalRevenue)
    })
    .OrderByDescending(result => result.TotalRevenue)
    .Take(3)
    .ToList();

            return Ok(topSuppliers);
        }

        [HttpGet("ShipperOrderCounts")]
        public IActionResult ShipperOrderCounts()
        {

            var shipperOrderCounts = _context.Shippers
    .Join(
        _context.Orders,
        s => s.ShipperId,
        o => o.ShipVia,
        (s, o) => new
        {
            ShipperId = s.ShipperId,
            CompanyName = s.CompanyName,
            ShipVia = o.ShipVia
        }
    )
    .GroupBy(result => new { result.ShipperId, result.CompanyName })
    .Select(group => new
    {
        group.Key.ShipperId,
        group.Key.CompanyName,
        OrderCount = group.Count()
    })
    .OrderByDescending(result => result.OrderCount)
    .ToList();

            return Ok(shipperOrderCounts);
        }

        [HttpGet("CustomerOrdersLeast15")]
        public IActionResult CustomerOrdersLeast15()
        {
            var customersWithTotalOrderCount = _context.Customers
            .Join(
                _context.Orders,
                c => c.CustomerId,
                o => o.CustomerId,
                (c, o) => new
                {
                    CustomerId = c.CustomerId,
                    ContactName = c.ContactName,
                    CompanyName = c.CompanyName,
                    City = c.City,
                    OrderCount = o.CustomerId
                }
            )
            .GroupBy(result => new { result.CustomerId, result.ContactName, result.CompanyName, result.City })
            .Where(group => group.Count() >= 15)
            .Select(group => new
            {
                CustomerId = group.Key.CustomerId,
                ContactName = group.Key.ContactName,
                CompanyName = group.Key.CompanyName,
                City = group.Key.City,
                TotalOrderCount = group.Count()
            })
            .OrderByDescending(result => result.TotalOrderCount)
            .ToList();

            return Ok(customersWithTotalOrderCount);
        }

        [HttpGet("CustomerNameAfter1917")]
        public IActionResult CustomerNameAfter1917()
        {

            var customersWithRecentOrders = _context.Customers
        .Join(
            _context.Orders,
            c => c.CustomerId,
            o => o.CustomerId,
            (c, o) => new
            {
                CustomerId = c.CustomerId,
                ContactName = c.ContactName,
                OrderDate = o.OrderDate
            }
        )
        .Where(result => result.OrderDate > new DateTime(1998, 1, 1))
        .OrderBy(result => result.OrderDate)
        .Select(result => new
        {
            result.CustomerId,
            result.ContactName,
            result.OrderDate
        })
        .ToList();

            return Ok(customersWithRecentOrders);

        }
        [HttpGet("ShippingWithFederal")]
        public IActionResult ShippingWithFederal()
        {
            try
            {
                var query = from s in _context.Shippers
                            join o in _context.Orders on s.ShipperId equals o.ShipVia
                            where s.ShipperId == 3 && o.ShipVia == 3
                            select s;


                // JSON döngüsel referansları yönetmek için JsonSerializerOptions kullanın
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 32
                };

                // JSON olarak serileştirilmiş veriyi döndürün
                return Ok(query);
            }
            catch (Exception ex)
            {
                // Hata durumunda 500 kodu ile cevap oluşturun
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet("Steven97Report")]
        public IActionResult Steven97Report()
        {
            try
            {
                var orders = _context.Orders
                    .Where(o => o.EmployeeId == 5 && o.OrderDate.Value.Year == 1997 && o.OrderDate.Value.Month == 03)
                    .ToList();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }

        }

        [HttpGet("speedy")]
        public IActionResult SpeedyExpress()
        {

            var speedy = _context.Orders.Where(t => t.EmployeeId == 1 && (t.CustomerId == "DUMON" || t.CustomerId == "ALFKI") && t.ShipVia == 1).ToList();

            return Ok(speedy);
        }


        [HttpGet("GetGermanyCustomer")]

        public IActionResult GetGermanyCustomer()
        {


            return Ok(_context.Customers.Where(i => i.Country == "Germany").ToList());

        }

        [HttpGet("Seafood")]

        public IActionResult Seafood()
        {
            var query = from p in _context.Products
                        join s in _context.Suppliers on p.SupplierId equals s.SupplierId
                        join c in _context.Categories on p.CategoryId equals c.CategoryId
                        join d in _context.OrderDetails on p.ProductId equals d.ProductId
                        join o in _context.Orders on d.OrderId equals o.OrderId
                        join e in _context.Employees on o.EmployeeId equals e.EmployeeId
                        where c.CategoryId != 8 && s.PostalCode == "33007"
                        select new
                        {
                            EmployeeInfo = new
                            {
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                HomePhone = e.HomePhone
                            }
                        };

            var distinctEmployeeInfo = query.Distinct().ToList();

            return Ok(distinctEmployeeInfo);


        }

        //[HttpGet("Eastren")]


        //public IActionResult Eastren()
        //{

        //    var customers = from cst in _context.Customers
        //                    join ord in _context.Orders on cst.CustomerId equals ord.CustomerId
        //                    join emp in _context.Employees on ord.EmployeeId equals emp.EmployeeId
        //                    join tr in _context.Territories on empt.Territory.TerritoryId equals tr.TerritoryId
        //                    join r in _context.Regions on tr.Region.RegionId equals r.RegionId
        //                    join shp in _context.Shippers on ord.ShipVia equals shp.ShipperId
        //                    join ordt in _context.OrderDetails on ord.OrderId equals ordt.OrderId
        //                    join prd in _context.Products on ordt.ProductId equals prd.ProductId
        //                    join cts in _context.Categories on prd.CategoryId equals cts.CategoryId
        //                    where r.RegionDescription == "Eastern" && shp.ShipperId == 3
        //                    select cts;

        //    var result = customers.ToList();

        //    return Ok(customers);


        //}


    }



}