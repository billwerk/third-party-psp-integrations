// Copyright (c) billwerk GmbH. All rights reserved

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Infrastructure.InMemoryStorage;
using PaymentGateway.Shared;

namespace PaymentGateway;

/// <summary>
/// Sample only controller.
/// Provide possibility to view current in-memory storage state.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
[Route("api/InMemoryStorage")]
public class InMemoryStorageController : ControllerBase
{
    [HttpGet]
    public IActionResult GetCurrentDbState()
    {
        var currentDbState = new CurrentDbState
        {
            PreauthTransactions = InMemoryStorage.Transactions.Where(x => x is PreauthTransaction)
                .OfType<PreauthTransaction>()
                .ToList(),
            PaymentTransactions = InMemoryStorage.Transactions.Where(x => x is PaymentTransaction)
                .OfType<PaymentTransaction>()
                .ToList(),
            RefundTransactions = InMemoryStorage.Transactions.Where(x => x is RefundTransaction)
                .OfType<RefundTransaction>()
                .ToList(),
            PspSettings = InMemoryStorage.PspSettings.ToList(),
        };
        
        //JSON serialization needed to avoid expanded structure of ObjectId structure 
        return Content( currentDbState.To(JsonConvert.SerializeObject), "application/json");
    }
    
   private class CurrentDbState
   {
       public List<PreauthTransaction> PreauthTransactions = new();
       
       public List<PaymentTransaction> PaymentTransactions = new();
       
       public List<RefundTransaction> RefundTransactions = new();

       public List<Domain.Modules.PSP.Settings.PspSettings> PspSettings = new();
   }
}
