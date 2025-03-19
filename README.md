# FoodDelivey
This project calculates the delivery cost for food orders based on the city, vehicle type, and real-time weather conditions. It includes a CronJob that fetches weather data every hour from the Ilmateenistus. The API interface allows clients to request the delivery fee, and the system calculates the cost dynamically using the city, vehicle type, and weather conditions.

Key features:
Delivery Fee Calculation: Calculates the delivery cost based on multiple factors, including city, vehicle type, and weather.
Weather Data Integration: Automatically fetches weather data every hour from Ilmateenistus via a scheduled cron job.
API Interface: Exposes an API for clients to request the delivery fee, ensuring easy integration with external systems.
Unit Testing: Comprehensive unit tests ensure the reliability and correctness of the application.
