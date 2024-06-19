# Billwerk+ Transform Payment Gateway sample

The repository contains a sample of 3-rd party PSP (Payment Provider Service) example integration with billwerk.
This approach allows the implementation of the layer between billwerk and PSP. The sample is one of the use case scenarios. 
  
More information about the Payment gateway can be found in:

1. [Customer documentation](https://transform-docs.billwerk.com/docs/introduction)
2. [API documentation](https://transform-docs.billwerk.com/reference/post_api-payment)

## **Content**

- [_About_](#About)
- [_Code structure_](#Code-structure) 
- [_Setup & Launch_](#Setup-&-Launch)

### About

This repository provides a simple example of the **Billwerk+ Transform 3-rd party PSP implementation** with the help
of ```Billwerk.Payment.SDK``` models and a set of payment-actions endpoints. It is not destined to be a real solution starting point, but the sample represents the main ideas behind functionality, which should be implemented.  
The main functionalities are:

- communication with a specific PSP,

- communication with billwerk by implementing a set of predefined endpoints to carry out interactions, in which billwerk is the initiator of this communication,

- storage of an actual state of payment operations (transaction data),

- storage of customer payment data, which are needed to produce new payment requests,

- notification for billwerk about payment transactions and updates through the webhook channel. 

### Code structure

- `PaymentGateway` - a web application that determines a communication level with billwerk. 
- `PaymentGateway.Application` - set of handlers responsible for validation of input data and passing them to PSP handlers. It has handlers to process PSP webhooks
and apply updates from them to transactions. 
- `PaymentGateway.Domain` - contains `BillwerkSDK`, it is a set of requests and responses of DTO models that must be used together with `PaymentGateway` endpoints. 
In addition, it includes transaction models to store transaction data in a strong-typed manner like validation rules, i.e., the domain of the application. 
- `PaymentGateway.Infrastructure` - an infrastructure-related code: 
  - serialization, 
  - specific code for data storages (MongoDb / custom in memory storage),
  - Dependency Injection, 
  - logging etc.

> **Important note** : ```PaymentGateway.Application```, ```PaymentGateway.Domain```, ```PaymentGateway.Infrastructure``` layers are under hood functionality in terms of
Billwerk+ Transform communication API endpoints. They contain logic of collecting, validating and processing transaction data and **Billwerk+ Transform doesn't specify by this sample how it
must be implemented**. All of this can be designed in any way that is preferable, including any other programming language, technologies, packages, libraries,
architecture, data storage, etc.

The `PaymentGateway` web project contains endpoints that are mostly needed to be implemented in order to achieve correct communication
with billwerk. It works perfectly as an initial point of implementation. 
Requests and response models used inside the `PaymentGateway` web project refer to the `PaymentGateway.Domain.BillwerkSDK` folder. 
They are connected together by `PaymentGateway` API endpoints. That is a crucial part of 3-rd party PSP implementation.

The repository also includes two projects related to payment providers:
- `FakeProvider`
    - contains hardcoded mock responses from a hypothetical PSP. Can be used for any kind of testing purposes.
- `Reepay` (Former name of Billwerk+ Pay)
    - a battle-ready implementation of [Billwerk+ Pay](https://www.billwerk.plus/pay-payment-gateway/) PSP with a _credit card_ payment role,
  which fully relies on `PaymentGateway.Domain.BillwerkSDK.DTO.Requests` models and it produces responses in
  `PaymentGateway.Domain.BillwerkSDK.DTO.Responses` models.  
  
>"**Battle-ready**" - any kind of Billwerk+ Transform-related payment operations
  that can be processed via this sample application. You can test it if you have Billwerk+ Pay API test account credentials (see Reepay chapter).

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
Using local MongoDB as a data storage can be a simplified the process of testing an application because it does not disappear after an application shutdown, **but it is a completely optional solution**.

2. `CurrentPaymentProvider` - an integer value:
  - ''2'' (default value): the application uses the `FakeProvider` as a payment provider. 
>Using `FakeProvider` doesn't produce any external calls and uses hardcoded responses 
  in `Billwerk.SDK` models' structure. Useful for a testing purpose.
  - ''1'' (Billwerk+ Pay): application uses the [Billwerk+ Pay](https://www.billwerk.plus/pay-payment-gateway/) as a payment provider.
>**Important note:** Before setting this parameter, you must have your own Reepay test account. If you already have it, put your Reepay **private API key** and **webhook secret** into `AppBuilder.CreateDefaultSettings` -> `...SaveSettings(new ReepaySettings...` place. 
> It will create Reepay settings with your credentials on application start.






  
