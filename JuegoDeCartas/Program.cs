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
            Console.WriteLine("Bienvenido al programa de cartas");

        // Menú para seleccionar el juego
        Console.WriteLine("Seleccione el juego:");
        Console.WriteLine("1. Blackjack");
        Console.WriteLine("2. Poker");
        int opcionJuego = int.Parse(Console.ReadLine());

        // Menú para ingresar el número de jugadores
        Console.Write("Ingrese el número de jugadores: ");
        int numJugadores = int.Parse(Console.ReadLine());

        IDeckDeCartas deck = new DeckDeCartas(opcionJuego);
        IComparadorDeManos comparador;

            switch (opcionJuego)
            {
                case 1:
                comparador = new ComparadorDeManosBlackjack();
                break;

                case 2:
                comparador = new ComparadorDeManosPoker();
                break;

            }

            // Crear jugadores
            List<IJugador> jugadores = new List<IJugador>();
            for (int i = 0; i < numJugadores; i++)
            {
                jugadores.Add(new Jugador());
            }
         
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Mano del Jugador: {string.Join(", ", jugador.MostrarCartas().Select(carta => $"{carta.Figura} {carta.Valor}"))}");
            }

            // Crear dealer
            IDealer dealer = new Dealer(deck);

        
            dealer.BarajearDeck();
            Console.WriteLine("Comienza el juego...");

            

            // cartas al dealer
            List<ICarta> manoDealer = dealer.RepartirCartas(2);

            
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Mano del Jugador: {string.Join(", ", jugador.MostrarCartas().Select(carta => $"{carta.Figura} {carta.Valor}"))}");
            }
            Console.WriteLine($"Carta visible del Dealer: {manoDealer[0].Figura} {manoDealer[0].Valor}");

            Console.WriteLine("Fin del juego. ¡Gracias por jugar!");
            Console.ReadKey();
        }
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

            void InicializarDeck(int tipoJuego);


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

            List<ICarta> ObtenerMano();
        }
    public class Jugador : IJugador
    {
        private List<ICarta> mano;

        public Jugador()
        {
            mano = new List<ICarta>();
        }

       

        public List<ICarta> ObtenerCartas()
        {
            return mano;
        }
        public void RealizarJugada()
        {
            // Implementar logica
        }

        public void ObtenerCartas(List<ICarta> cartas)
        {
            mano.AddRange(cartas);
        }

        public ICarta DevolverCarta(int indiceCarta)
        {
            ICarta carta = mano[indiceCarta];
            mano.RemoveAt(indiceCarta);
            return carta;
        }

        public List<ICarta> DevolverTodasLasCartas()
        {
            List<ICarta> todasLasCartas = new List<ICarta>(mano);
            mano.Clear();
            return todasLasCartas;
        }

        public List<ICarta> MostrarCartas()
        {
            return new List<ICarta>(mano);
        }

        public ICarta MostrarCarta(int indiceCarta)
        {
            return mano[indiceCarta];
        }

        public List<ICarta> ObtenerMano()
        {
            return mano;
        }
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
            public Carta(ValoresCartasEnum valor)
            {
                Figura = FigurasCartasEnum.Diamantes;
                Valor = valor;
            }
        }
        public class DeckDeCartas : IDeckDeCartas
        {
            private List<ICarta> deck;

            public bool blackjack { get; private set; }

            public DeckDeCartas()
            {
                deck = new List<ICarta>();
                if (blackjack)
                {
                    InicializarDeckBlackjack();
                }
                else {
                    InicializarDeckPoker();
                }

            }

        public DeckDeCartas(int tipoJuego)
        {
            deck = new List<ICarta>();
            InicializarDeck(tipoJuego);
        }

        public void InicializarDeck(int tipoJuego)
        {
            switch (tipoJuego)
            {
                case 1: // Blackjack
                    InicializarDeckBlackjack();
                    break;
                case 2: // Poker 
                    InicializarDeckPoker();
                    break;
                default:
                    throw new ArgumentException("Tipo de juego no válido");
            }
        }

        public void BarajearDeck()
            {
                var random = new Random();
                deck = deck.OrderBy(card => random.Next()).ToList();
            }

            public ICarta VerCarta(int indiceCarta)
            {
                return deck[indiceCarta];
            }

            public ICarta SacarCarta(int indiceCarta)
            {
                ICarta carta = deck[indiceCarta];
                deck.RemoveAt(indiceCarta);
                return carta;
            }

            public void MeterCarta(ICarta carta)
            {
                deck.Add(carta);
            }
            public void MeterCarta(List<ICarta> cartas)
            {
                deck.AddRange(cartas);
            }

            private void InicializarDeckBlackjack()
            {
                foreach (FigurasCartasEnum figura in Enum.GetValues(typeof(FigurasCartasEnum)))
                {
                    foreach (ValoresCartasEnum valor in Enum.GetValues(typeof(ValoresCartasEnum)))
                    {
                        deck.Add(new Carta(figura, valor));
                    }
                }
            }

            private void InicializarDeckPoker()
            {
                foreach (ValoresCartasEnum valor in Enum.GetValues(typeof(ValoresCartasEnum)))
                {
                    deck.Add(new Carta(FigurasCartasEnum.Diamantes, valor));
                    deck.Add(new Carta(FigurasCartasEnum.Espadas, valor));
                    deck.Add(new Carta(FigurasCartasEnum.Treboles, valor));
                    deck.Add(new Carta(FigurasCartasEnum.Corazones, valor));
                }
            }
        }

        public class ComparadorDeManosBlackjack : IComparadorDeManos
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

        public class ComparadorDeManosPoker : IComparadorDeManos
        {
            public List<ICarta> ObtenerManoGanadora(List<List<ICarta>> manosDeCartas)
            {
                throw new NotImplementedException();
            }

            public List<ICarta> ObtenerManoGnadora (List<List<ICarta>> manosDeCartas)
            {
                return manosDeCartas.OrderByDescending(mano => CalcularPuntuacionPoker(mano)).FirstOrDefault();
            }
            private int CalcularPuntuacionPoker(List<ICarta> mano)
            {
                return mano.Sum(carta => (int)carta.Valor);
            }
        }

        public class ComparadorDeManosBlackJack : IComparadorDeManos
        {
            public List<ICarta>  ObtenerManoGanadora(List<List<ICarta>> manosDeCartas)
            {
                return manosDeCartas.OrderByDescending(mano => CalcularPuntuacionBlackJack(mano)).FirstOrDefault();
            }
            private int CalcularPuntuacionBlackJack(List<ICarta> mano)
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

        public class Dealer : IDealer
        {
            private IDeckDeCartas deck;
            public Dealer(IDeckDeCartas deck)
            {
                this.deck = deck;
            }
            public List<ICarta> RepartirCartas(int numeroDeCartas)
            {
                List<ICarta> cartasRepartidas = new List<ICarta>();
                for (int i = 0; i < numeroDeCartas; i++)
                {
                    cartasRepartidas.Add(deck.SacarCarta(0));
                }
                return cartasRepartidas;
            }

            public void RecogerCartas(List<ICarta> cartas)
            {
                deck.MeterCarta(cartas);
            }

            public void BarajearDeck()
            {
                deck.BarajearDeck();
            }

        }

    }


