// ModelTrainer.cs
using Microsoft.ML;

namespace ML_Trend_Prediction_API.Services
{
    public class ModelTrainer
    {
        private const string ModelPath = "./Models/model.zip";
        private const string TrainingDataPath = "./Models/training_data.json";
        private const string LogPath = "./Models/prediction_log.json";

        public ModelTrainer()
        {
            Directory.CreateDirectory("./Models");
        }

        public void TrainModel(TrainingExample[] trainingData)
        {
            var mlContext = new MLContext();
            var data = mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = mlContext.Transforms
                .Concatenate("Features", nameof(TrainingExample.Value))
                .Append(mlContext.Regression.Trainers.LbfgsPoissonRegression());

            var model = pipeline.Fit(data);
            mlContext.Model.Save(model, data.Schema, ModelPath);

            // Save the training data
            File.WriteAllText(TrainingDataPath, System.Text.Json.JsonSerializer.Serialize(trainingData));
        }

        public void ClearModel()
        {
            if (File.Exists(ModelPath)) File.Delete(ModelPath);
            if (File.Exists(TrainingDataPath)) File.Delete(TrainingDataPath);
            if (File.Exists(LogPath)) File.Delete(LogPath);

            Console.WriteLine("[INFO] Model, training data, and logs cleared.");
        }

        public List<float> Forecast(List<float> priceHistory, int forecastDays, float? actualValue = null)
        {
            var mlContext = new MLContext();

            if (!File.Exists(ModelPath))
                throw new FileNotFoundException("No trained model found. Train the model first.");

            var model = mlContext.Model.Load(ModelPath, out _);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TrainingExample, PricePrediction>(model);

            List<float> predictions = new List<float>();

            for (int i = 0; i < forecastDays; i++)
            {
                var lastPrice = priceHistory[^1];
                var prediction = predictionEngine.Predict(new TrainingExample { Value = lastPrice });
                predictions.Add(prediction.Score);
                priceHistory.Add(prediction.Score);
            }

            // Calculate prediction error if actual value is provided
            float? predictionError = null;
            if (actualValue.HasValue)
            {
                var lastPrediction = predictions[^1];
                predictionError = Math.Abs(actualValue.Value - lastPrediction);
            }

            // Log the predictions
            LogPredictions(priceHistory, predictions, actualValue, predictionError);

            return predictions;
        }

        private void LogPredictions(List<float> inputData, List<float> predictions, float? actualValue, float? predictionError)
        {
            var logEntry = new { Date = DateTime.UtcNow, InputData = inputData, Predictions = predictions, Actual = actualValue, Error = predictionError };
            var logs = File.Exists(LogPath) ? System.Text.Json.JsonSerializer.Deserialize<List<object>>(File.ReadAllText(LogPath)) : new List<object>();
            logs.Add(logEntry);
            File.WriteAllText(LogPath, System.Text.Json.JsonSerializer.Serialize(logs));
        }

        public TrainingExample[] GetTrainingData()
        {
            if (File.Exists(TrainingDataPath))
            {
                var json = File.ReadAllText(TrainingDataPath);
                return System.Text.Json.JsonSerializer.Deserialize<TrainingExample[]>(json);
            }

            throw new FileNotFoundException("No training data found. Train the model first.");
        }

        public List<object> GetPredictionLog()
        {
            if (File.Exists(LogPath))
            {
                var json = File.ReadAllText(LogPath);
                return System.Text.Json.JsonSerializer.Deserialize<List<object>>(json);
            }

            return new List<object>();
        }

        public void CorrectModel(float actualValue)
        {
            var trainingData = GetTrainingData().ToList();
            trainingData.Add(new TrainingExample { Value = trainingData[^1].Label, Label = actualValue });
            TrainModel(trainingData.ToArray());
        }
    }

    public class TrainingExample
    {
        public float Value { get; set; }
        public float Label { get; set; }
    }

    public class PricePrediction
    {
        public float Score { get; set; }
    }
}
