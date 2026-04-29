using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASC.Model;

namespace ASC.DataAccess.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<ServiceRequest> ServiceRequestRepository { get; }

        IRepository<MasterDataKey> MasterDataKeyRepository { get; }

        IRepository<MasterDataValue> MasterDataValueRepository { get; }

        Task<int> SaveAsync();
    }
}