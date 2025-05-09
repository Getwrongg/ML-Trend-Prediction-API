// PredictionController.cs
using Microsoft.AspNetCore.Mvc;
using ML_Trend_Prediction_API.Services;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly ModelTrainer _modelTrainer;

    public PredictionController(ModelTrainer modelTrainer)
    {
        _modelTrainer = modelTrainer;
    }

    [HttpPost("train")] // Train or Update Model
    public IActionResult TrainCustomModel([FromBody] float[] prices)
    {
        var trainingData = prices.Select((value, index) => new TrainingExample
        {
            Value = value,
            Label = (index < prices.Length - 1) ? prices[index + 1] : value
        }).ToArray();

        _modelTrainer.TrainModel(trainingData);
        return Ok(new { Message = "Model trained or updated.", ModelPath = "./Models/model.zip" });
    }

    [HttpPost("forecast")] // Forecast using existing model
    public IActionResult Forecast([FromBody] List<float> priceHistory, int forecastDays, float? actualValue = null)
    {
        if (priceHistory.Count == 0)
            return BadRequest("Price history cannot be empty.");

        var predictions = _modelTrainer.Forecast(priceHistory, forecastDays, actualValue);
        return Ok(new
        {
            Predictions = predictions,
            Actual = actualValue,
            Error = actualValue.HasValue ? Math.Abs(predictions[^1] - actualValue.Value) : (float?)null
        });
    }

    [HttpPost("correct")] // Correct model with actual value
    public IActionResult CorrectModel([FromBody] float actualValue)
    {
        _modelTrainer.CorrectModel(actualValue);
        return Ok(new { Message = "Model corrected with actual value." });
    }

    [HttpGet("log")] // Get prediction logs
    public IActionResult GetPredictionLog()
    {
        var logs = _modelTrainer.GetPredictionLog();
        return Ok(logs);
    }

    [HttpGet("training-data")] // Get current training data
    public IActionResult GetTrainingData()
    {
        try
        {
            var data = _modelTrainer.GetTrainingData();
            return Ok(data);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("clear")] // Clear the existing model and training data
    public IActionResult ClearModel()
    {
        _modelTrainer.ClearModel();
        Console.WriteLine("[INFO] Model and training data cleared by API request.");
        return Ok(new { Message = "Model cleared. A new model can now be created." });
    }
}
