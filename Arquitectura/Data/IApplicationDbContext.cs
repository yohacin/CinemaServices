using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public interface IApplicationDbContext
    {
        IEnumerable<dynamic> CollectionFromSql(string Sql);

        //IDbContextTransaction _currentTransaction { get; set; }

        DbContext dbContext { get; }
        bool HasActiveTransaction { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync(IDbContextTransaction transaction);

        Task RollbackTransactionAsync();
    }
}