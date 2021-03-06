using System.Collections.Generic;
using System.Threading.Tasks;

namespace COMMO.Domain.Interfaces.Services
{
    using Domain.Entities;

    public interface IPlayerService : IServiceBase<Player>
    {
        IEnumerable<string> GetCharactersListByAccountId(int id);
    }
}
