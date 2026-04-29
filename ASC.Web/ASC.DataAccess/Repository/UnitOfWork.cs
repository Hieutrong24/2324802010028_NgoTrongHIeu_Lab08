using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASC.Model;
using Microsoft.EntityFrameworkCore;

namespace ASC.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public UnitOfWork(DbContext context)
        {
            _context = context;

            ServiceRequestRepository = new Repository<ServiceRequest>(_context);
            MasterDataKeyRepository = new Repository<MasterDataKey>(_context);
            MasterDataValueRepository = new Repository<MasterDataValue>(_context);
        }

        public IRepository<ServiceRequest> ServiceRequestRepository { get; }

        public IRepository<MasterDataKey> MasterDataKeyRepository { get; }

        public IRepository<MasterDataValue> MasterDataValueRepository { get; }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}