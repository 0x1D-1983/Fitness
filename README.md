# Heart Rate Zone Fitness App
Two microservices apps to demonstrate async data communication implemented with Kafka topics using Confluent .NET
consumer and producer libraries and transaction based commit logic.

## Client Gateway
REST API serivce to submit fitness tracker data into a Kafka topic.

## Client Gateway Tests
Test project for the API service.

## Heart Rate Zone Service
Kafka stream dotnet hosted service to calculate heart rate zone ranges and push it to a new topic.

## Heart Rate Zone Service Tests
Test project for the hosted service.

## Run the project
To run the solution spin up a local Kafka cluster with Docker. Sample Docker Compose files can be found on the Confluent website.

### Create topics
Input topic where fitness tracking data is sent (by the API endpoint): BiometricsImported
Ouput topic where the HeartRateZone hosted services sends the calculated range data: HeartRateZoneReached

### Schema Registy

Schemas for the input and output topics can be found in the [/schemas](./schemas/) folder.

### API data
Some API data can be found in the [sample-data](./sample-data/) folder.