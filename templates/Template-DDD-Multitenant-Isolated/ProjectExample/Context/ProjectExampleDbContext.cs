using Microsoft.EntityFrameworkCore;
using SharedKernel.Repository;

namespace ProjectExample.Context;
public partial class ProjectExampleDbContext : DbContext, IUnitOfWork
{
    public ProjectExampleDbContext(DbContextOptions<ProjectExampleDbContext> options)
        : base(options)
    {
    }
    
    
}