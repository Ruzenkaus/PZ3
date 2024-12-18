using Microsoft.ML;
using Microsoft.ML.Data;
using yavpotoke.Models;

namespace yavpotoke.Trainer
{
    public class ModelTrainer
    {
        private static readonly string _feedbackFilePath = "C:\\Users\\suvor\\source\\repos\\yavpotoke\\yavpotoke\\feedback.csv";
        private static readonly string _modelPath = "C:\\Users\\suvor\\source\\repos\\yavpotoke\\yavpotoke\\MLModel.zip";

        public static void RetrainModel()
        {
            var mlContext = new MLContext();

            var data = mlContext.Data.LoadFromTextFile<ModelInput>(
                path: _feedbackFilePath,
                hasHeader: false,
                separatorChar: ',');

            var preview = data.Preview();
            if (!preview.Schema.Select(s => s.Name).Contains("Col0"))
            {
                throw new InvalidOperationException("Файл даних не містить очікуваний стовпець 'Col0'.");
            }

            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(ModelInput.Col0))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ModelInput.Label))) 
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(data);
            mlContext.Model.Save(model, data.Schema, _modelPath);
        }
    }
}
