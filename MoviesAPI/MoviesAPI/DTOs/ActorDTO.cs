using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class ActorDTO : ActorCreationDTO
    {
        public string PersonName { get; set; }
    }
}
