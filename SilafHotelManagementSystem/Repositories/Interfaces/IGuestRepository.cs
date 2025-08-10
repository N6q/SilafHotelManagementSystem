using SilafHotelManagementSystem.Models;
using System.Collections.Generic;

namespace SilafHotelManagementSystem.Repositories.Interfaces
{
    /// <summary>
    /// Synchronous repository contract for Guest entities.
    /// </summary>
    public interface IGuestRepository
    {
        List<Guest> GetAll();
        Guest? GetById(int id);
        void Add(Guest guest);
        void Update(Guest guest);
        void Delete(int id);
    }
}
