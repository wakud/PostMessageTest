using Microsoft.AspNetCore.Mvc;
using PostMessage.BLL;
using PostMessage.Models;
using Microsoft.AspNetCore.Http;

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
        public IActionResult Index()
        {
            string userId = Request.Cookies["userId"];  //Отримуємо айді з куків

            //Якщо немає, то генеруємо
            if (String.IsNullOrEmpty(userId))
            {
                userId = Guid.NewGuid().ToString();
                Response.Cookies.Append("userId", userId);
            }

            ViewData ["userId"] = userId;
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

                await _postMessage.PostUserMessageAsync(message, Guid.Parse(userId));
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
