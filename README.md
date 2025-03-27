# Payment Gateway Challenge

A simple payment processing API that allows merchants to initiate and retrieve card-based payments. The service consists of two endpoints:

    GET /api/payments/{id} – Retrieve a previously processed payment

    POST /api/payments – Process a payment

This API acts as the gateway between the merchant and the acquiring bank, handling validation, forwarding requests, and storing/retrieving payment details.

## Initial Assumptions
I have assumed that the POST endpoint doesn't also add the payment to the repository as this isn't stated as necessary in the requirements.

Further, I have also assumed that the serialisation of the JSON for the database document is case-sensitive.

A BadRequest (400) is returned when validation fails. If call to the acquiring bank fails then Status field of PostPaymentResponse is Rejected.

Null is returned from GetPayment(id) in the service layer to allow the controller to return a NotFound (404).

Authorisation codes returned from the acquiring bank are used as the payment ID.

## Solution Design
The solution follows a clean, layered architecture separating Controllers, Services, Repositories, HttpClients, Validators, and Models
with folders for each.

## Controllers
The controller exposes the GET and POST endpoints and are responsible for validating the incoming request and returning the required response codes.
New controllers can be added depending on function, new endpoints can be added for payments related exposure.

## Services
The service layer sits underneath the controllers and contains all the business logic related to the service being called.
The payments service handles all payment related business logic and can be reused accordingly in various controller endpoints.

## Repositories
The repositories provide the specific methods that enable interaction with the chosen database. 
An interface was used to inject the repository where needed to ensure classes don't rely on the concrete implementation and to make testing easier.
Any logic related to interacting directly with the database is handled here and is specific to the specific database it is using (internal static, external context etc)

## Http Clients
The clients handle any logic pertaining to requests out to an external service (in this case a mocked acquiring bank service). The response is deserialised and status code checked.
A fallback response is produced for failed calls instead of throwing exception here, this is then handled in the service layer.

## Validators
Utilisation of FluentValidation ensures requests are valid. Specific rules can be set up, as seen in the PostPaymentRequestValidator.
Both rule-level and class-level cascade modes are employed.

## Models
I have separated the models into requests and responses, while having the repository documents have their own schemas.
This allows for the service to wrap the responses/requests with extra information and metadata if required in the future and ensures the serialisation is expected by using Newtonsoft.

## Testing Strategy
Unit testing validates each layer and logical pathway within each method in each class in isolation.

Integration tests spins up a test server in-memory using WebApplicationFactory. Integration tests include testing of validation of the full request to response pipeline and common scenarios and use cases.

## Future modifications
Add authentication via FromHeader on each endpoint to ensure only verifed users can hit the endpoint.

The in-memory repository would be replaced with an external database. The returned document can then be mapped into a response object or mapped from a request object.
Currently, the mapping is one-to-one so has been handled in the service layer, but a mapper class can be created if the mapping get more complicated or varied.

Add OpenTelemetry tracing and metrics for observability for insights and debugging and general health checks of the service.

Add logging to various points (ex after external calls, null returns etc) to aid debugging and observability.

Containerise app for easier deployment on different machines via docker, configure deployments with helm charts.

Add contract testing for requests/responses between external services, with minor versioning for non-breaking changes and major versioning for breaking changes.

Add retry policies to external call made from http clients, writes/reads to external databases if added.

Change the response of failed validation on POST endpoint to an Ok response, but with Status field of response to Rejected as in requirement.

On retrieval of payment, restrict the Status value to be only Authorized or Declined.

Add GUID parsing to ensure ids are guids.
