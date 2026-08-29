using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class EmployeeDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public EmployeeDeletedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<EmployeeDeletedData>(domainEvent.Data);
        if (data == null) return;

        var employeeRead = await _readContext.EmployeesRead.FindAsync(data.Employee_id);
        if (employeeRead == null) return;

        _readContext.EmployeesRead.Remove(employeeRead);
        await _readContext.SaveChangesAsync();
    }

    private record EmployeeDeletedData(string Employee_id);
}
