using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using System.IO;
using System.Xml.Linq;
using yavpotoke.Trainer;
using Yavpotoke;

namespace yavpotoke.Controllers
{
    public class HomeController : Controller
    {
        private readonly string feedbackFilePath = "feedback.csv";
        private readonly string modelPath = "MLModel.zip";


        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
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

            ViewBag.Comment = comment;
            ViewBag.Prediction = int.Parse(result.PredictedLabel) == 1 ? "Позитивний настрій 😊" : "Негативний настрій 😡";
            ViewBag.Accuracy = $"Точність передбачення: {predictionScore:P2}";

            return View();
        }

        [HttpPost]
        public IActionResult Retrain()
        {
            try
            {
                ModelTrainer.RetrainModel();

            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Помилка донавчання: {ex.Message}";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Feedback(string comment, int feedback)
        {
            string line = $"{comment},{feedback}";
            System.IO.File.AppendAllText(feedbackFilePath, line + "\n");

            ViewBag.Message = "Дякуємо за зворотний зв'язок! Дані збережено.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult FeedbackData()
        {
            if (!System.IO.File.Exists(feedbackFilePath))
            {
                return Content("Файл фідбеку порожній або відсутній.");
            }

            var lines = System.IO.File.ReadAllLines(feedbackFilePath);
            return Content(string.Join("\n", lines));
        }
    }

}