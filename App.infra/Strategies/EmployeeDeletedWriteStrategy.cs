using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class EmployeeDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public EmployeeDeletedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<EmployeeDeletedData>(domainEvent.Data);
        if (data == null) return;

        var employeeRead = await _readContext.EmployeesRead.FindAsync(data.Employee_id);
        if (employeeRead == null) return;

        _readContext.EmployeesRead.Remove(employeeRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("EmployeeRead", data.Employee_id);
    }

    private record EmployeeDeletedData(string Employee_id);
}
