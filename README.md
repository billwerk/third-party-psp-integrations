# billwerk Payment Gateway sample

Repository contains the sample of 3-rd party PSP (Payment Provider Service) example integration with billwerk.
This approach allows to implement the layer between billwerk and PSP. The sample is one of the use cases scenario. 
  
More information about the Payment gateway can be found in:

1. [Customer documentation](https://billwerk.gitbook.io/home/)
2. [API documentation](https://payment-gateway.swagger.billwerk.io/)
3. [Postman collection with sample requests](https://www.getpostman.com/collections/bc278c1bea0dc0e440fa)
4. [Postman collection with example workflows](https://www.postman.com/collections/639331ca5fef8d0a23af)
## **Content**

- [_About_](#About)
- [_Code structure_](#Code-structure) 
- [_Setup & Launch_](#Setup-&-Launch)

### About

This repository provides a simple example of the **billwerk 3-rd party PSP implementation** with a help
of ```Billwerk.Payment.SDK``` models and a set of payment-actions endpoints. It is not destined to be a real solution starting point, but the sample represents the main ideas behind functionality, which should be implemented.  
Main functionalities are:

- communication with a specific PSP,

- communication with billwerk by implementing set of predefined endpoints to carry out interactions, in which billwerk is the initiator of this communication,

- storage of an actual state of payment operations (transaction data),

- storage of customer payment data, which are needed to produce new payment requests,

- notification for billwerk about payment transactions and updates through the webhook channel. 

### Code structure

- `PaymentGateway` - a web application, determines a communication level with billwerk. 
- `PaymentGateway.Application` - set of handlers responsible for validation of input data and pass them to PSP-handlers. It has handlers to process PSP webhooks
and apply updates from them to transactions. 
- `PaymentGateway.Domain` - contains `BillwerkSDK`, it is a set of requests and responses of DTO models that must be used together with `PaymentGateway` endpoints. 
In addition, it includes transaction models to store transaction data in strong-typed manner like validation rules, i.e. domain of application. 
- `PaymentGateway.Infrastructure` - an infrastructure related code: 
  - serialization, 
  - specific code for data storages (MongoDb / custom in memory storage),
  - Dependency Injection, 
  - logging etc.

> **Important note** : ```PaymentGateway.Application```, ```PaymentGateway.Domain```, ```PaymentGateway.Infrastructure``` layers are under hood functionality in terms of
billwerk-communication API endpoints. They are contain logic of collecting, validating and processing transaction data and **billwerk doesn't specify by this sample how it
must be implemented**. All of this can be designed in any preferable way, with any other programming language, technologies, packages, libraries, 
architecture, data storage and so on.

The `PaymentGateway` web project contains endpoints that are mostly needed to be implemented in order to achieve correct communication
with billwerk. It works perfectly as an initial point of implementation. 
Requests and responses models used inside `PaymentGateway` web project refer to `PaymentGateway.Domain.BillwerkSDK` folder. 
They are connected together by `PaymentGateway` API endpoints. That is a crucial part of 3-rd party PSP implementation.

Repository also includes two projects related to payment providers:
- `FakeProvider`
    - contains a hardcoded mock responses from a hypothetical PSP. Can be used for any kind of testing purposes.
- `Reepay`
    - a battle-ready implementation of [Reepay's](https://reepay.com/) PSP with a _credit card_ payment role,
  which fully relies on `PaymentGateway.Domain.BillwerkSDK.DTO.Requests` models and it produces responses in
  `PaymentGateway.Domain.BillwerkSDK.DTO.Responses` models.  
  
>"**Battle-ready**" - any kind of billwerk-related payment operations
  that can be processed via this sample application. You can test it if you have Reepay API test account credentials (see Reepay chapter).

### Setup & Launch

To launch the sample, you must have installed [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) on your local environment. No more external dependencies needed.

In `PaymentGateway` there is a `appsettings.json` file, which contains basic application settings. By default, the application's url, specified in this file, is a `localhost:7086`.
This settings file also includes `PlaygroundEnvironment` section with two parameters:

1. `UseInMemoryStorage` - a boolean value: 
   -  `true` (default value): application stores payment transaction data in memory, during application enablement. 
   In this case, you can use `PaymentGateway.InMemoryStorageController` GET https://localhost:7086/api/InMemoryStorage to fetch current stored data. 
   - `false`: application uses MongoDb as data storage. It is required to [install MongoDb](https://www.mongodb.com/docs/manual/installation/) on your local environment.
>If you have a running local MongoDb server with a default port (`mongodb://localhost:27017`), the application automatically creates a`PaymentGatewaySample` database and it uses it for storing data.

>However, if the database with such name exists already (e.g. after the second launch of application), the application **does not** create a new database, it just uses an existing one.
Using local MongoDb as a data storage can be simplified process of testing an application, because it does not disappear after an application shutdown, **but it is a completely optional solution**.

2. `CurrentPaymentProvider` - an integer value:
  - ''2'' (default value): application uses the `FakeProvider` as a payment provider. 
>Using `FakeProvider` doesn't produce any external calls and used hardcoded responses 
  in `Billwerk.SDK` models' structure. Useful for a testing purpose.
  - ''1'' (Reepay): application uses the [Reepay](https://reepay.com/) as a payment provider.
>**Important note:** before setting this parameter, you must have your own Reepay test account. If you already have it, put your Reepay **private API key** and **webhook secret** into `AppBuilder.CreateDefaultSettings` -> `...SaveSettings(new ReepaySettings...` place. 
> It will create Reepay settings with your credentials on application start.






  
