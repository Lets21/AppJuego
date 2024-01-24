using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppJuego.Models
{
    public class Juego
    {
        int Id { get; set; }
        public int PlayerNumber { get; set; }
        public DateTime GameDateTime { get; set; }
    }
}
