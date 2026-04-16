using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Infrastructure.Data;

namespace Telemedicina.Api.Services;

public class CreditRenewalBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CreditRenewalBackgroundService> _logger;

    public CreditRenewalBackgroundService(IServiceProvider serviceProvider, ILogger<CreditRenewalBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CreditRenewalBackgroundService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRenewalsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing CreditRenewalBackgroundService.");
            }

            // Aguarda 1 hora antes de rodar novamente
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        _logger.LogInformation("CreditRenewalBackgroundService is stopping.");
    }

    private async Task ProcessRenewalsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        var doctorsToRenew = await dbContext.Doctors
            .Where(d => d.NextRenewalDate.HasValue && d.NextRenewalDate <= now)
            .ToListAsync();

        if (doctorsToRenew.Any())
        {
            _logger.LogInformation($"Found {doctorsToRenew.Count} doctor(s) to renew credits.");
            int countProcessed = 0;

            foreach (var doctor in doctorsToRenew)
            {
                using var transaction = await dbContext.Database.BeginTransactionAsync();
                try
                {
                    doctor.Credits = doctor.PlanCredits;
                    doctor.LastRenewalDate = now;
                    // Avança a próxima renovação sempre para o primeiro dia do próximo mês
                    var nextMonth = now.AddMonths(1);
                    doctor.NextRenewalDate = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                    var tx = new CreditTransaction
                    {
                        DoctorId = doctor.Id,
                        Amount = doctor.PlanCredits, 
                        Description = "Renovação mensal de créditos",
                        CreatedAt = now
                    };

                    dbContext.CreditTransactions.Add(tx);
                    await dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    countProcessed++;
                    _logger.LogInformation($"Renewed credits for Doctor ID: {doctor.Id}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Failed to renew credits for Doctor ID: {doctor.Id}");
                }
            }
            
            _logger.LogInformation($"Successfully processed {countProcessed} renewals.");
        }
    }
}
