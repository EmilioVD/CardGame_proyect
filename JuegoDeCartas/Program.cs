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
        public enum FigurasCartasEnum
        {
            Diamantes,
            Espadas,
            Treboles,
            Corazones
        }
        public enum ValoresCartasEnum
        {
            As = 1,
            Dos,
            Tres,
            Cuatro,
            Cinco,
            Seis,
            Siete,
            Ocho,
            Nueve,
            Diez,
            Jota,
            Reina,
            Rey
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

        public class Carta : ICarta
        {
            public FigurasCartasEnum Figura { get; }
            public ValoresCartasEnum Valor { get; }
            public Carta(FigurasCartasEnum figura, ValoresCartasEnum valor)
            {
                Figura = figura;
                Valor = valor;
            }
        }
        public class DeckDeCartas : IDeckDeCartas;
        {
            private List<ICarta> deck;
            public DeckDeCartas()
            {
                deck = new List<ICarta>();
                InicializarDeck();
            }

            public void BarajearDeck()
            {
                var random = new Random();
                deck = deck.OrderBy(card => random.Next()).ToList();
            }

            public  ICarta VerCarta(int indiceCarta)
            {
                return deck[indiceCarta];
            }

            public ICarta SacarCarta(int indiceCarta)
            {
                ICarta carta = deck[indiceCarta];
                deck.RemoveAt(indiceCarta);
                return carta;
            }

            public void MeterCarta(List<ICarta> cartas)
            {
                deck.AddRange(cartas);
            }

            private void InicializarDeck()
            {
                foreach (FigurasCartasEnum figura in Enum.GetValues(typeof(FigurasCartasEnum)))
                {
                    foreach (ValoresCartasEnum valor in Enum.GetValues(typeof(ValoresCartasEnum)))
                    {
                        deck.Add(new Carta(figura, valor));
                    }
                }
            }
        }

        public class ComparadorDeManos : IComparadorDeManos
        {
            public List<ICarta> ObtenerManoGanadora(List<List<ICarta>> manosDeCartas)
            {
                return manosDeCartas.OrderByDescending(mano => CalcularPuntuacion(mano)).FirstOrDefault();
            }

            private int CalcularPuntuacion(List<ICarta> mano)
            {
                int puntuacion = mano.Sum(carta => (int)carta.Valor);
                foreach (var carta in mano.Where(carta => carta.Valor == ValoresCartasEnum.As)) 
                {
                    if (puntuacion > 21)
                    {
                        puntuacion -= 10;
                    }
                }
                return puntuacion;
            }
        }

    }
}
