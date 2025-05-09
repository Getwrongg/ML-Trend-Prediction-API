# ML Trend Prediction API

## 📌 Project Overview
ML Trend Prediction API is a robust and scalable API for predicting financial prices using machine learning. It allows you to train a model with custom price data, make predictions, and continuously improve the model through manual correction.

## 🚀 Features
- Train a model with your own price data.
- Make future price predictions with the trained model.
- Manually correct the model with actual values to improve accuracy.
- View all past predictions and compare them with actual values.
- Clear the model and training data at any time.
- Error tracking for predictions (difference between prediction and actual value).

## 📂 Project Structure
```
ML_Trend_Prediction_API/
├── Controllers/
│   └── PredictionController.cs   # API Endpoints
├── Services/
│   └── ModelTrainer.cs           # ML Model Training & Prediction Logic
├── Models/
│   └── TrainingExample.cs        # Data Model for Training
│   └── PricePrediction.cs        # Model Prediction Output
└── Program.cs                    # API Configuration
```

## ⚙️ API Endpoints
### 1. Train the Model
- URL: `POST /api/prediction/train`
- Body (JSON Array of Prices):
```json
[0, 4, 3, 2, 5, 4, 3, 2, 5]
```
- Response:
```json
{
  "Message": "Model trained or updated.",
  "ModelPath": "./Models/model.zip"
}
```

### 2. Make a Forecast
- URL: `POST /api/prediction/forecast?forecastDays=1`
- Body (JSON Array of Prices):
```json
[0, 4, 3, 2, 5, 4, 3, 2, 5]
```
- Optional Query Parameter: `actualValue=15.0`
- Response:
```json
{
  "Predictions": [15.2],
  "Actual": 15.0,
  "Error": 0.2
}
```

### 3. Correct the Model with Actual Value
- URL: `POST /api/prediction/correct`
- Body:
```json
15.0
```
- Response:
```json
{
  "Message": "Model corrected with actual value."
}
```

### 4. View Prediction Logs
- URL: `GET /api/prediction/log`
- Response:
```json
[
  {
    "Date": "2025-05-09T12:00:00Z",
    "InputData": [0, 4, 3, 2, 5, 4, 3, 2, 5],
    "Predictions": [15.2],
    "Actual": 15.0,
    "Error": 0.2
  }
]
```

### 5. View Training Data
- URL: `GET /api/prediction/training-data`
- Response:
```json
[
  { "Value": 0, "Label": 4 },
  { "Value": 4, "Label": 3 }
]
```

### 6. Clear Model and Training Data
- URL: `DELETE /api/prediction/clear`
- Response:
```json
{
  "Message": "Model cleared. A new model can now be created."
}
```

## ⚡ How It Works
- Train the model using a set of prices.
- Use the model to make predictions.
- Correct the model with actual values when available.
- Repeat to continuously improve accuracy.

## 🛡️ Security Considerations
- Currently, the API is open without authentication.
- For production, consider adding API Key or JWT authentication.

## ✅ Future Improvements
- Multi-feature model (Price, Volume, RSI, MACD, etc.).
- Dynamic model selection (allowing multiple models).
- Model versioning.
- Advanced error analysis.

## ⚡ How to Run Locally
```bash
# Clone the repository
git clone https://github.com/yourusername/ML_Trend_Prediction_API.git

# Navigate to the project directory
cd ML_Trend_Prediction_API

# Build and run the project
dotnet build
dotnet run
```

## 📌 License
MIT License
