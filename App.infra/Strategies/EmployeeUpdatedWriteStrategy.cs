using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class EmployeeUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public EmployeeUpdatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<EmployeeUpdatedData>(domainEvent.Data);
        if (data == null) return;

        var employeeRead = await _readContext.EmployeesRead.FindAsync(data.Employee_id);
        if (employeeRead == null) return;

        employeeRead.FirstName = data.FirstName;
        employeeRead.LastName = data.LastName;
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("EmployeeRead", data.Employee_id);
    }

    private record EmployeeUpdatedData(string Employee_id, string FirstName, string LastName);
}
