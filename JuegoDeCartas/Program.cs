using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuegoDeCartas
{
    class Program
    {
        static void Main(string[] args)
        {

        }

        public interface ICarta
        {
            FigurasCartasEnum Figura { get; }
            ValoresCartasEnum Valor { get; }
        }
        
        public interface IDeckDeCartas
        {
            void BarajearDeck();
            ICarta VerCarta(int indiceCarta);
            ICarta SacarCarta(int indiceCarta);
            void MeterCarta(ICarta carta);
            void MeterCarta(List<ICarta> cartas);
        }

        public interface IComparadorDeManos
        {
            List<ICarta> ObtenerManoGanadora(List<List<ICarta>> manosDeCartas);
        }
        public interface IDealer
        {
            List<ICarta> RepartirCartas(int numeroDeCartas);
            void RecogerCartas(List<ICarta> cartas);
            void BarajearDeck();
        }

        public interface IJuego
        {
            IDealer Dealer { get; }
            IComparadorDeManos ComparadorDeManos { get; }
            void AgregarJugador(IJugador jugador);
            void IniciarJuego();
            void MostrarGanador();
        }

        public interface IJugador
        {
            void RealizarJugada();
            void ObtenerCartas(List<ICarta> cartas);
            ICarta DevolverCarta(int indiceCarta);
            List<ICarta> DevolverTodasLasCartas();
            List<ICarta> MostrarCartas();
            ICarta MostrarCarta(int indiceCarta);
        }

    }
}
