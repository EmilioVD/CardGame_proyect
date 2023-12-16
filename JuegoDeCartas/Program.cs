using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuegoDeCartas.Interfaces;
using JuegoDeCartas.Enumeradores;


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
            IDealer dealer = new Dealer();

            Console.WriteLine("Comienza el juego...");

            // Crear el deck según el tipo de juego seleccionado
            IDeckDeCartas deck = new DeckDeCartas(opcionJuego);
            deck.BarajearDeck();

            // Repartir cartas iniciales
            RepartirCartasIniciales(jugadores, dealer, deck);

            // Mostrar estado inicial
            MostrarEstado(jugadores);

            // Jugadores devuelven cartas y reciben nuevas
            foreach (var jugador in jugadores)
            {
                DevolverYRecibirCartas(jugador, deck);
                MostrarEstado(jugadores);
            }

            // Mostrar manos finales y determinar ganador
            MostrarManosFinales(jugadores);
            DeterminarGanador(jugadores);

            Console.WriteLine("Fin del juego. ¡Gracias por jugar!");
            Console.ReadKey();
        }
        static void RepartirCartasIniciales(List<IJugador> jugadores, IDealer dealer, IDeckDeCartas deck)
        {
            // Repartir cinco cartas a cada jugador y al dealer
            for (int i = 0; i < 5; i++)
            {
                foreach (var jugador in jugadores)
                {
                    jugador.ObtenerCartas(new List<ICarta> { deck.SacarCarta(0) });
                }

                dealer.RecogerCartas(new List<ICarta> { deck.SacarCarta(0) });
            }
        }


        static void DevolverYRecibirCartas(IJugador jugador, IDeckDeCartas deck)
        {
            // Jugador devuelve todas las cartas
            List<ICarta> cartasDevueltas = jugador.DevolverTodasLasCartas();

            // Devolver cartas al mazo
            foreach (var cartaDevuelta in cartasDevueltas)
            {
                deck.MeterCarta(cartaDevuelta);
            }

            // Dealer da nuevas cartas al jugador
            List<ICarta> nuevasCartas = new List<ICarta>();
            foreach (var cartaDevuelta in cartasDevueltas)
            {
                nuevasCartas.Add(deck.SacarCarta(0));  // Suponemos que el índice 0 es la parte superior del mazo
            }
            jugador.ObtenerCartas(nuevasCartas);

        }

        static void MostrarEstado(List<IJugador> jugadores)
        {
            // Mostrar el estado actual del juego, incluyendo las manos de los jugadores
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Mano del Jugador: {string.Join(", ", jugador.MostrarCartas().Select(carta => $"{carta.Figura} {carta.Valor}"))}");
            }

            Console.WriteLine();
        }

        static void MostrarManosFinales(List<IJugador> jugadores)
        {
            // Mostrar las manos finales de todos los jugadores al final de la ronda
            Console.WriteLine("Manos Finales:");
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Mano del Jugador: {string.Join(", ", jugador.MostrarCartas().Select(carta => $"{carta.Figura} {carta.Valor}"))}");
            }

            Console.WriteLine();
        }

        static void DeterminarGanador(List<IJugador> jugadores)
        {



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

        public class DealerBlackJack : IDealer
        {
            private List<ICarta> deck;

            public DealerBlackJack()
            {
                deck = new List<ICarta>(); //Creamos un nuvo deck de cartas
            }

            public List<ICarta> RepartirCartas(int numeroDeCartas)
            {
                List<ICarta> cartasRepartidas = new List<ICarta>();
                for (int i = 0; i < numeroDeCartas; i++)
                {
                    if (deck.Count > 0)
                    {
                        cartasRepartidas.Add(deck[0]);
                        deck.RemoveAt(0); //Quitamos cartas de la lista cartas repartidas 
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

            public void Jugar()
            {
                Console.WriteLine("Turno del Dealer");
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
                int ases = 0;
                foreach (var carta in deck)
                {
                    int valorCarta;

                    // Ajustamos el valor de la carta según las reglas de 21 Blackjack
                    switch (carta.Valor)
                    {
                        case ValoresCartasEnum.As:
                            valorCarta = 11; // Se asume inicialmente el valor de 11 para ayudar al jugador en todo caso sera 1 
                            ases++;
                            break;
                        case ValoresCartasEnum.Dos:
                        case ValoresCartasEnum.Tres:
                        case ValoresCartasEnum.Cuatro:
                        case ValoresCartasEnum.Cinco:
                        case ValoresCartasEnum.Seis:
                        case ValoresCartasEnum.Siete:
                        case ValoresCartasEnum.Ocho:
                        case ValoresCartasEnum.Nueve:
                            valorCarta = (int)carta.Valor;
                            break;
                        case ValoresCartasEnum.Diez:
                        case ValoresCartasEnum.Jota:
                        case ValoresCartasEnum.Reina:
                        case ValoresCartasEnum.Rey:
                            valorCarta = 10;
                            break;
                        default:
                            valorCarta = 0; // En caso de error, se asigna 0
                            break;
                    }

                    puntuacion += valorCarta;
                }

                // Ajustar la puntuación por los Ases
                while (ases > 0 && puntuacion > 21)
                {
                    puntuacion -= 10;
                    ases--;
                }

                return puntuacion;

            }

            public class DealerPokerClasico : IDealer
            {
                private List<ICarta> deck;

                public DealerPokerClasico()
                {
                    deck = new List<ICarta>(); //Creamos un nuevo deck de cartas
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

                public void jugar(List<IJugador> jugadores ) //Desta forma el dealer puede mover las cartas entre jugadores 
                {
                    Console.WriteLine("Turno del Dealer");
                    RepartirCartasIniciales(jugadores);
                    var cartasUsadas = new List<ICarta>();
                    foreach (var jugador in jugadores )
                    {
                        cartasUsadas.AddRange(jugador.DevolverTodasLasCartas());
                    }
                    RecogerCartas(cartasUsadas);
                    BarajearDeck();
                }

                private void RepartirCartasIniciales(List<IJugador> jugadores)
                {
                    foreach (var jugador in jugadores)
                    {
                        jugador.ObtenerCartas(RepartirCartas(5));
                    }
                }

            }

        }



    }

}


