using Microsoft.AspNetCore.Mvc;
using PostMessage.BLL;
using PostMessage.Models;
using Microsoft.AspNetCore.Http;
using NuGet.Protocol;

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
            ViewData ["userId"] = userId;
            var a = await _postMessage.Get10UserMessageAsync(Guid.Parse(userId));

            return View(a);
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

    }
}
