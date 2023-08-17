using Microsoft.AspNetCore.Mvc;
using PostMessage.BLL;
using PostMessage.Models;
using Microsoft.AspNetCore.Http;
using NuGet.Protocol;
using System.Collections.Generic;

namespace PostMessage.Controllers
{
    public class MessageController : Controller
    {
        private readonly IPostMessage _postMessage;

        public MessageController(IPostMessage postMessage)
        {
            _postMessage = postMessage;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = Request.Cookies["userId"];  //Отримуємо айді з куків

            //Якщо немає, то генеруємо і записуємо в куки
            if (String.IsNullOrEmpty(userId))
            {
                userId = Guid.NewGuid().ToString();
                Response.Cookies.Append("userId", userId);
            }
            ViewData["userId"] = userId;

            return View();
        }
        
        [HttpPost]
        public async Task<ActionResult> PostMessage(string message, string userId)
        {
            var a = userId;
            try
            {
                if (String.IsNullOrEmpty(message))
                {
                    return RedirectToAction("Index");
                }

                var rezult = await _postMessage.PostUserMessageAsync(message, Guid.Parse(userId));

                if (rezult == true)
                {
                    TempData[WC.Success] = WC.Success;
                }
                else
                {
                    TempData[WC.Error] = WC.Error;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return RedirectToAction("Index");
        }

        public async Task<JsonResult> Get10UserMessages(string userId)
        {
            FetchMessageParams fetchParams = new FetchMessageParams(Guid.Parse(userId), 10);
            var result = await _postMessage.GetMessages(fetchParams);
            return Json(result);
        }


        public async Task<IActionResult> Get20UsersMessages()
        {
            FetchMessageParams fetchParams = new FetchMessageParams(20);
            var result = await _postMessage.GetMessages(fetchParams);
            return Json(result);
        }
    }
}
