using System;
using System.Collections.Generic;

namespace EBanking.Data.Interfaces
{
    public interface IEntitySet<TPK, TEntity>
    {
        IEnumerable<TEntity> All { get; }

        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TPK id);
    }
}
