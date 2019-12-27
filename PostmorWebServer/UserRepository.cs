using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer
{
    public class UserRepository
    {
        private readonly DbContext context;
        public UserRepository(DbContext context)
        {
            this.context = context;
        }

    }
}
