using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class EmployeeUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public EmployeeUpdatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
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
    }

    private record EmployeeUpdatedData(string Employee_id, string FirstName, string LastName);
}
