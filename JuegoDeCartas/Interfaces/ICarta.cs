using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuegoDeCartas.Enumeradores;

namespace JuegoDeCartas.Interfaces
{
    public interface ICarta
    {
        FigurasCartasEnum Figura { get; }
        ValoresCartasEnum Valor { get; }
    }

}
