﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestXmlConfig.Models;
using XmlConfigInitialization;

namespace TestXmlConfig.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly XmlConfig _xmlConfig;

        public HomeController(ILogger<HomeController> logger, XmlConfig xmlConfig)
        {
            _logger = logger;
            this._xmlConfig = xmlConfig;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetXml(string key, string node = "default")
        {
            return Ok(new
            {
                value = _xmlConfig.GetValue(key, node)
            });
        }

        [HttpGet]
        public IActionResult AllTestXml()
        {
            return Ok(new
            {
                value1 = _xmlConfig.GetValue("aa", "default"),
                value2 = _xmlConfig.GetNodes(),
                value3 = _xmlConfig.GetAllValue()
            });
        }

        [HttpGet]
        public IActionResult SetXml(string key, string value, string node = "default")
        {
            _xmlConfig.SetValue(key, value, node);
            return Ok(new
            {
                value = _xmlConfig.GetValue(key, node)
            });
        }

        [HttpGet]
        public IActionResult DeleteNodeXml(string node)
        {            
            _xmlConfig.DeleteNode(node);
            return Ok(new
            {
                value = "ok"
            });
        }

        [HttpGet]
        public IActionResult SetSingletonXml(string key, string value, string node = "default")
        {
            var xmlConfig = XmlConfigFactory.GetSingletonInstance();
            xmlConfig.SetValue(key, value, node);
            return Ok(new
            {
                value = xmlConfig.GetValue(key, node)
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
