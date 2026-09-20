using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class EmployeeCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public EmployeeCreatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<EmployeeCreatedData>(domainEvent.Data);
        if (data == null) return;

        var employeeRead = new EmployeeRead
        {
            Employee_id = data.Employee_id,
            FirstName = data.FirstName,
            LastName = data.LastName
        };

        _readContext.EmployeesRead.Add(employeeRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("EmployeeRead", data.Employee_id);
    }

    private record EmployeeCreatedData(string Employee_id, string FirstName, string LastName);
}
