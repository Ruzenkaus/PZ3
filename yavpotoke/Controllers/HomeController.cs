using Microsoft.AspNetCore.Mvc;
using System.IO;
using Yavpotoke;

namespace yavpotoke.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string comment)
        {
            if (string.IsNullOrEmpty(comment))
            {
                ViewBag.Result = "Коментар не повинен бути порожнім.";
                return View();
            }

            var sampleData = new MLModel.ModelInput()
            {
                Col0 = comment
            };

            var result = MLModel.Predict(sampleData);

            float predictionScore = result.Score != null && result.Score.Length > 0 ? result.Score[0] : 0f;

            SaveFeedback(comment, float.Parse(result.PredictedLabel), predictionScore);
            ViewBag.Comment = comment;
            ViewBag.Prediction = Convert.ToInt32(result.PredictedLabel) == 1 ? "Позитивний настрій 😊" : "Негативний настрій 😡";
            ViewBag.Accuracy = $"Точність передбачення: {predictionScore:P2}";

            return View();
        }

        [HttpPost]
        public IActionResult Feedback(string comment, int prediction, string feedback)
        {
            var feedbackLine = $"{comment},{prediction},{feedback}";
            System.IO.File.AppendAllText("feedback.csv", feedbackLine + "\n");

            ViewBag.Message = "Дякуємо за ваш відгук!";

            return RedirectToAction("Index");
        }

        private void SaveFeedback(string comment, float prediction, float score)
        {
            string feedbackFilePath = "feedback.csv";

            var line = $"{comment},{prediction},{score}";
            System.IO.File.AppendAllText(feedbackFilePath, line + "\n");
        }
    }
}
