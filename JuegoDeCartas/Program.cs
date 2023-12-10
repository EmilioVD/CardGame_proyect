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

        public interface IDealer
        {
            List<ICarta> RepartirCartas(int numeroDeCartas);
            void RecogerCartas(List<ICarta> cartas);
            void BarajearDeck();
        }

        public interface IJuego
        {
            IDealer Dealer { get; }
            bool JuegoTerminado { get; }
            void AgregarJugador(IJugador jugador);
            void IniciarJuego();
            void JugarRonda();
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

          
        }
        public class Juego : IJuego
        {

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
                else
                {
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
       
        

        public class Dealer : IDealer
        {
            private List<ICarta> deck;

            public Dealer()
            {
                deck = new List<ICarta>();
            }

            public List<ICarta> RepartirCartas(int numeroDeCartas)
            {
                List<ICarta> cartasRepartidas = new List<ICarta>();
                for (int i = 0; i < numeroDeCartas; i++)
                {
                    if (deck.Count > 0)
                    {
                        cartasRepartidas.Add(deck[0]);
                        deck.RemoveAt(0);
                    }
                }
                return cartasRepartidas;
            }

            public void RecogerCartas(List<ICarta> cartas)
            {
                deck.AddRange(cartas);
            }

            public void BarajearDeck()
            {
                var random = new Random();
                deck = deck.OrderBy(card => random.Next()).ToList();
            }

            public void JugarBlackjack()
            {
                while (CalcularPuntuacion() < 17)
                {
                    TomarCarta();
                }
            }

            private void TomarCarta()
            {
                List<ICarta> cartasRepartidas = RepartirCartas(1);
                Console.WriteLine($"El Dealer toma una carta: {cartasRepartidas[0].Valor} de {cartasRepartidas[0].Figura}");
            }

            private int CalcularPuntuacion()
            {
                int puntuacion = 0;
                foreach (var carta in deck)
                {
                    puntuacion += (int)carta.Valor;
                }

                // Ajustar la puntuación por los Ases
                foreach (var carta in deck.Where(c => c.Valor == ValoresCartasEnum.As))
                {
                    if (puntuacion + 10 <= 21)
                    {
                        puntuacion += 10;
                    }
                }

                return puntuacion;
            }
        }



    }

}


