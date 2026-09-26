using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System;
using System.Threading.Tasks;
using OrderTracking.Infrastructure;
using OrderTracking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace OrderTracking.Worker;

public class ShipmentSimulatorService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ShipmentSimulatorService> _logger;

    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(2));

    public ShipmentSimulatorService(IServiceScopeFactory serviceScopeFactory, ILogger<ShipmentSimulatorService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(await _timer.WaitForNextTickAsync(stoppingToken))
        {
            //scope for the db context 
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //db queries
            var cutoff = DateTime.UtcNow.AddMinutes(-1);

            

            var toShipped = await db.Orders
            .Where(o => o.Status == OrderStatus.Processing && o.LastStatusChangedAt < cutoff)
            .OrderBy(o => o.LastStatusChangedAt)
            .Take(10)
            .ToListAsync();
            
            var toOutForDelivery = await db.Orders
            .Where(o => o.Status == OrderStatus.Shipped && o.LastStatusChangedAt < cutoff)
            .OrderBy(o => o.LastStatusChangedAt)
            .Take(10)
            .ToListAsync();

            var toDelivered = await db.Orders
            .Where(o => o.Status == OrderStatus.OutForDelivery && o.LastStatusChangedAt < cutoff)
            .OrderBy(o => o.LastStatusChangedAt)
            .Take(10)
            .ToListAsync();


                 foreach(var order in toShipped)
                {
                    
                    try
                    {
                        //random courrier provider list
                        var courrierList = new List<string>{"UPS", "USPS", "FedEx", "DHL"};
                        var randCourrier  = courrierList[Random.Shared.Next(courrierList.Count)];

                        //guid based tracking num
                        var courrierTrackingNumber = Guid.NewGuid();

                        //estimated delivery date
                        int rangeEstimate = Random.Shared.Next(2,6);
                        var courrierEstimatedDate = DateTime.UtcNow.AddDays(rangeEstimate);

                        order.MarkAsShipped(randCourrier, courrierTrackingNumber.ToString(), courrierEstimatedDate);
                        await db.SaveChangesAsync();   
                    }
                    
                    catch(InvalidOperationException ex)
                    {
                        _logger.LogError(ex, "Failed to advance order {OrderId}", order.Id);
                    }
                }

            
                 foreach(var order in toOutForDelivery)
                {
                    try
                    {
                        order.MarkAsOutForDelivery();
                        await db.SaveChangesAsync();
                    }
                    catch(InvalidOperationException ex)
                    {
                        _logger.LogError(ex, "Failed to advance order {OrderId}", order.Id);
                    }
                    
                }

                foreach(var order in toDelivered)
                {
                    try
                    {
                        order.MarkAsDelivered();
                        await db.SaveChangesAsync();   
                    }
                    
                    catch(InvalidOperationException ex)
                    {
                        _logger.LogError(ex, "Failed to advance order {OrderId}", order.Id);
                    }
                }

             
          

           


        }
    }
}