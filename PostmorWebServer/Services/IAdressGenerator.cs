using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Services
{
    public interface IAdressGenerator
    {
        Task<List<string>> GenerateAdresses(int amount);
    }
}
