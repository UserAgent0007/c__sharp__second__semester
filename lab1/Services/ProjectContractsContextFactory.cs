using lab1.Models;
using Microsoft.EntityFrameworkCore;

namespace lab1.Services;

// Контекст має конструктор зі строкою підключення, тому стандартний AddDbContextFactory
// не підходить. Ця фабрика створює новий короткоживучий контекст на кожну операцію
// (у Blazor Server один circuit живе довго, тому спільний DbContext використовувати не можна).
public class ProjectContractsContextFactory(string connectionString) : IDbContextFactory<ProjectContractsContext>
{
    public ProjectContractsContext CreateDbContext() => new(connectionString);
}
