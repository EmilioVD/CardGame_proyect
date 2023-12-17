using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuegoDeCartas.Interfaces
{
    public interface IJuego
    {
        IDealer GetDealer();

        bool JuegoTerminado { get; }
        void AgregarJugador(IJugador jugador);
        void IniciarJuego();
        void JugarRonda();
        void MostrarGanador();
    }
}
