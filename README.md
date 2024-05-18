# Project Overview

This project is a Lambda function designed to import data for onboarding clients. Client data is provided as binary files through SFTP. This Lambda function listens for S3 file upload events, uploads those files to an internal document service, and then creates a client database stub containing the file information.

Terraform scripts and custom libraries have been removed from this implementation for privacy reasons. As such, this project will not compile and serves only an example of non-proprietary code developed in real-world scenario with tight deadlines and shifting requirements. 

## Application Workflow
The `Program.cs` file serves as the standard entry point, which wires up the event listener. Incoming messages are directed to `DocumentImportLambda.Aws.Services.LambdaService` where the events are parsed and mapped to domain specific objects. After mapping occurs, the message is enumerated and each event is processed individually within the `DocumentImportLambda.Aws.Services.LambdaService.LambdaEventHandler` class. This two step message parsing/processing design pattern was deliberate, to provide a tightly scoped "integration specific" class as well as an easily testable "business logic" class, by separating out external dependencies from internal objects.

Once within the `LambdaEventHandler` class, default implementations for each specific domain are constructed if none are provided by the constructor (See notes: 4)

The event is processed within the handler class by parsing the file data, and recursing through the file if it is an archive. Once the individual file contents have been identified/enumerated, the actual file processing occurs within the method `ProcessSingleFile`. The majority of the rest of the class serves only to perform initial setup, validate the messages, and recurse through the file content

## Organization
Due to a large number of distinct domains being required, the project is organized using a "domain first" approach with each relevant files for each domain existing within their own folder. The domains are as follows

1. Archive: Code specific to managing archive data (See notes: 5)
2. AWS: Code specific to integrating with AWS services
3. Document: Client specific integrations for managing document imports
4. Database: Data abstraction layer code
5. Authentication: OAuth specific code for wiring together services 

In a perfect world, each domain could be further expanded into its own library for re-use, however that was not within the scope of this project.

# Notes

1. Environment restrictions prevented running a full end-to-end execution locally, as there was no development infrastructure to integrate with for some services. As a result, the application was developed using a test driven pattern. Some tests included in the project served only to initialize functions for the sake of development. These are stored under "Integration Tests"
2. As a result of tight deadlines caused by changing business requirements, Unit Tests are scoped to specific "Problem Areas"
3. Due to apparent connection limits on RDS Proxies within AWS, connections needed to be pooled. Since local development did not use an RDS Proxy, the database access layer needed to be abstracted. The combination of connection pooling requirements,  TDD requirements, and limitations on generic inheritance lead to what can only be described as a "non-standard" approach to connection pooling, where the abstracted object had to represent an individual query instead of a connection. This has proven to be a functionally reliable and easily unit-testable method of achieving this pooling, however I would consider it unsatisfactory from an architectural perspective
4. Dependency injection could have been used at this point, however a combination of changing business requirements and client-specific application framework code has pushed this to "V2". Dependency injection in this context provides no additional benefit beyond the sake of the design pattern which has taken a back seat to providing an immediate functional project, and since the project is already architected in an abstracted/decoupled manner using proper interfaces, all of the usual flexibility and testability is provided already.
5. There were very specific requirements around memory usage due to the volume of data to be processed, and the limitations of Lambda functions. No library fitting all business needs was available.  As a result, the archive integration (usually a simple, single library) had to be broken out into its own abstracted area allowing the process to agnostically leverage the "best available" library for each format. This is functional, unit testable, and serves the business need, however its not the "ideal" solution I would have liked.