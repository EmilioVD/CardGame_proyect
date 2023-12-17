using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuegoDeCartas.Interfaces;
using JuegoDeCartas.Enumeradores;
using static JuegoDeCartas.Program;


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

            if (!int.TryParse(Console.ReadLine(), out int opcionJuego) || (opcionJuego != 1 && opcionJuego != 2))
            {
                Console.WriteLine("Opción de juego no válida.");
                return;
            }

            IniciarJuego(opcionJuego);

            Console.WriteLine("Fin del programa. ¡Gracias por jugar!");
            Console.ReadKey();
        }

        static void IniciarJuego(int opcionJuego)
        {
            // Resto del código para configurar el juego según la opción seleccionada
            Console.WriteLine($"Iniciando {(opcionJuego == 1 ? "Blackjack" : "Poker")}...");

            // Crear jugadores y dealer
            List<IJugador> jugadores = new List<IJugador>
            {
            new Jugador(),
             new Jugador(),
                    // Agrega más jugadores según sea necesario
             };

            IDealer dealer = (opcionJuego == 1) ? (IDealer)new DealerBlackJack() : (IDealer)new DealerPokerClasico();
            IDeckDeCartas deck = new DeckDeCartas();
            IJuego juego = new JuegoDePoker(dealer, deck);

            // Limpia las manos de los jugadores
            foreach (var jugador in jugadores)
            {
                jugador.DevolverTodasLasCartas();
            }

            // Barajea el mazo antes de repartir las cartas
            deck.BarajearDeck();

            // Reparte cartas a los jugadores
            foreach (var jugador in jugadores)
            {
                List<ICarta> cartasRepartidas = new List<ICarta>();

                // Sacar 5 cartas del mazo para cada jugador
                for (int i = 0; i < 5; i++)
                {
                    ICarta carta = deck.SacarCarta(0); // Suponiendo que 0 representa la parte superior del mazo
                    cartasRepartidas.Add(carta);
                }

                jugador.ObtenerCartas(cartasRepartidas);
            }

            // Agregar jugadores al juego
            foreach (var jugador in jugadores)
            {
                juego.AgregarJugador(jugador);
            }

            // Simulación de juego
            juego.IniciarJuego();
            juego.JugarRonda();
            juego.MostrarGanador();

            Console.WriteLine("Fin del juego. ¿Desea volver al menú principal? (Sí/No)");
            string respuesta = Console.ReadLine();
            if (respuesta?.ToLower() != "no")
                IniciarJuego(opcionJuego);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public class DeckDeCartas : IDeckDeCartas
        {
            private List<ICarta> cartas;

            public DeckDeCartas()
            {
                // Lógica básica para inicializar un mazo de cartas
                cartas = new List<ICarta>();
                foreach (FigurasCartasEnum figura in Enum.GetValues(typeof(FigurasCartasEnum)))
                {
                    foreach (ValoresCartasEnum valor in Enum.GetValues(typeof(ValoresCartasEnum)))
                    {
                        cartas.Add(new Carta(figura, valor));
                    }
                }
            }

            public ICarta SacarCarta(int indice)
            {
                if (indice >= 0 && indice < cartas.Count)
                {
                    ICarta cartaSacada = cartas[indice];
                    cartas.RemoveAt(indice);
                    return cartaSacada;
                }
                return null;
            }

            public void MeterCarta(ICarta carta)
            {
                cartas.Add(carta);
            }
            public void MeterCarta(List<ICarta> nuevasCartas)
            {
                cartas.AddRange(nuevasCartas);
            }
            public List<ICarta> RepartirCartas(int numeroDeCartas)
            {
                List<ICarta> cartasRepartidas = new List<ICarta>();
                for (int i = 0; i < numeroDeCartas; i++)
                {
                    if (cartas.Count > 0)
                    {
                        cartasRepartidas.Add(cartas[0]);
                        cartas.RemoveAt(0);
                    }
                }
                return cartasRepartidas;
            }

            public void InicializarDeck(int tipoJuego)
            {
                // Limpiamos el mazo actual
                cartas.Clear();
                foreach (FigurasCartasEnum figura in Enum.GetValues(typeof(FigurasCartasEnum)))
                {
                    foreach (ValoresCartasEnum valor in Enum.GetValues(typeof(ValoresCartasEnum)))
                    {
                        cartas.Add(new Carta(figura, valor));
                    }
                }
                if (tipoJuego == 1)
                {
                    // Lógica para inicializar el deck de Blackjack (por ejemplo)
                }
                else if (tipoJuego == 2)
                {
                    foreach (FigurasCartasEnum figura in Enum.GetValues(typeof(FigurasCartasEnum)))
                    {
                        foreach (ValoresCartasEnum valor in Enum.GetValues(typeof(ValoresCartasEnum)))
                        {
                            cartas.Add(new Carta(figura, valor));
                        }
                    }
                }
                else
                {
                    // Otro tipo de juego 
                }
            }
            public void BarajearDeck()
            {
                var random = new Random();
                cartas = cartas.OrderBy(card => random.Next()).ToList();
            }

            public ICarta VerCarta(int indiceCarta)
            {
                if (indiceCarta >= 0 && indiceCarta < cartas.Count)
                {
                    return cartas[indiceCarta];
                }
                return null;
            }


        }
        //______________________________________________________________





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


        public class Jugador : IJugador
        {
            private List<ICarta> mano;

            public Jugador()
            {
                mano = new List<ICarta>();
            }

            public void RealizarJugada()
            {
                // implementar logica

            }

            public void ObtenerCartas(List<ICarta> cartas)
            {
                mano.AddRange(cartas);
            }

            public ICarta DevolverCarta(int indiceCarta)
            {
                if (indiceCarta >= 0 && indiceCarta < mano.Count)
                {
                    ICarta cartaDevuelta = mano[indiceCarta];
                    mano.RemoveAt(indiceCarta);
                    return cartaDevuelta;
                }
                return null;
            }

            public List<ICarta> DevolverTodasLasCartas()
            {
                List<ICarta> cartasDevueltas = new List<ICarta>(mano);
                mano.Clear();
                return cartasDevueltas;
            }

            public List<ICarta> MostrarCartas()
            {
                return new List<ICarta>(mano);
            }

            public ICarta MostrarCarta(int indiceCarta)
            {
                if (indiceCarta >= 0 && indiceCarta < mano.Count)
                {
                    return mano[indiceCarta];
                }
                return null;
            }

            public List<ICarta> ObtenerMano()
            {
                return new List<ICarta>(mano);
            }


        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public class JuegoDePoker : IJuego
        { 

            private List<IJugador> jugadores;
            private IDeckDeCartas deck;
            public bool JuegoTerminado { get; private set; } = false;

            private IDealer dealer;

            public IDealer GetDealer()
            {
                return dealer;
            }

            private void SetDealer(IDealer value)
            {
                dealer = value;
            }

            private enum ManoEnum
            {
                CartaAlta = 1,
                Par = 2,
                DoblePar = 3,
                Trio = 4,
                Escalera = 5,
                Color = 6,
                FullHouse = 7,
                Poker = 8,
                EscaleraDeColor = 9,
                EscaleraReal = 10
            }
            private class ResultadoDeLaMano
            {
                public IJugador Jugador { get; set; } = null;
                public ManoEnum TipoDeMano { get; set; }
                public IEnumerable<ICarta> Cartas { get; set; }

                public ResultadoDeLaMano(ManoEnum tipoDeMano, IEnumerable<ICarta> cartas)
                {
                    TipoDeMano = tipoDeMano;
                    Cartas = cartas;
                }
            }
            public JuegoDePoker(IDealer dealer, IDeckDeCartas deck)
            {
                this.jugadores = new List<IJugador>();
                this.dealer = dealer;
                this.deck = deck;

                // Inicializar el mazo (puedes ajustar esto según tus necesidades)
                deck.InicializarDeck(0); // 0 podría indicar póker, ajusta según sea necesario
            }

            public void AgregarJugador(IJugador jugador)
            {
                jugadores.Add(jugador);
            }

            public void IniciarJuego()
            {
                foreach (var jugador in jugadores)
                {
                    List<ICarta> cartasRepartidas = new List<ICarta>();

                    // RepartirCartas devuelve 5 cartas por jugador
                    for (int i = 0; i < 5; i++)
                    {
                        cartasRepartidas.Add(deck.SacarCarta(0)); // Suponiendo que 0 representa la parte superior del mazo
                    }

                    jugador.ObtenerCartas(cartasRepartidas);
                }

            }

            public void JugarRonda()
            {
                foreach (var jugador in jugadores)
                {
                    jugador.RealizarJugada();
                }

                foreach (var jugador in jugadores)
                {
                    jugador.MostrarCartas();
                }

                MostrarGanador();
            }

            public void MostrarGanador()
            {
                List<ResultadoDeLaMano> resultados = new List<ResultadoDeLaMano>();
                foreach (var jugador in jugadores)
                {
                    ResultadoDeLaMano resultado = ObtenerResultadoDeLaMano(jugador.DevolverTodasLasCartas());
                    resultado.Jugador = jugador;
                    resultados.Add(resultado); // Añadir el resultado a la lista
                }

                resultados = resultados.OrderByDescending(r => r.TipoDeMano).ToList();

                List<ResultadoDeLaMano> resultadosDeMayorAMenor = resultados.Where(r => r.TipoDeMano == resultados[0].TipoDeMano).ToList();

                if (resultadosDeMayorAMenor.Count > 1)
                {
                    ObtenerGanadorPorCartas(resultadosDeMayorAMenor);
                }
                else
                {
                    Console.WriteLine($"El ganador es: {resultados[0].Jugador}");
                }
            }

            private ResultadoDeLaMano ObtenerResultadoDeLaMano(List<ICarta> cartas)

            {
                // Se obtiene el resultado de la mano del jugador
                var resultadoDeLaMano = EscaleraReal(cartas) ?? EscaleraColor(cartas) ?? Poker(cartas) ?? FullHouse(cartas) ?? Color(cartas) ?? //Es para obtener el valor de cada mano 
                                    Escalera(cartas) ?? Trio(cartas) ?? DoblePareja(cartas) ?? Pareja(cartas) ?? CartaAlta(cartas);

                return resultadoDeLaMano;
            }


            private void ObtenerGanadorPorCartas(List<ResultadoDeLaMano> puntajes) 
            {
                List<ResultadoDeLaMano> ganadores = new List<ResultadoDeLaMano>();
                ganadores.Add(puntajes[0]);

                for (int i = 1; i < puntajes.Count; i++)
                {
                    ResultadoDeLaMano Puntajes = puntajes[i];
                    List<ICarta> cartasDelGanador = ganadores[0].Cartas.ToList();
                    List<ICarta> cartasActualesDelJugador = Puntajes.Cartas.ToList();


                    for (int j = 0; j < cartasDelGanador.Count; j++)  //se ve cada carta de los jugadores
                    {
                        if (j == cartasDelGanador.Count - 1)
                        {

                            if (cartasDelGanador[j].Valor == cartasActualesDelJugador[j].Valor) //Define si existe mas de un ganador 
                            {
                                ganadores.Add(Puntajes);
                            }
                        }

                        if (cartasDelGanador[j].Valor > cartasActualesDelJugador[j].Valor)
                        {
                            break;
                        }
                        else if (cartasDelGanador[j].Valor < cartasActualesDelJugador[j].Valor)
                        {
                            ganadores.Clear();
                            ganadores.Add(Puntajes);
                            break;
                        }
                    }
                }

                if (ganadores.Count > 1)   //obtenemos si exiten mas de un ganador 
                {
                    Console.WriteLine("Hubo un empate entre:");
                }
                else  //obtenemos al ganador 
                {
                    Console.WriteLine("El ganador de la ronda es:");
                }

                foreach (var ganador in ganadores)  //imprimimos al jugador 
                {
                    Console.WriteLine($"{ganador.Jugador}");
                }


            }

            private  ResultadoDeLaMano EscaleraReal(List<ICarta> cartas)
            {
                var escaleraDeColor = EscaleraColor(cartas);

                if (escaleraDeColor != null)
                {
                    
                    if ((int)escaleraDeColor.Cartas.First().Valor == 1) // si Escalera Real, se retorna los resultados de la Escalera Real 
                    {
                        return new ResultadoDeLaMano(ManoEnum.EscaleraReal, escaleraDeColor.Cartas);
                    }
                }

                return null;
            }

            private ResultadoDeLaMano EscaleraColor(List<ICarta> cartas)
            {
                var escalera = Escalera(cartas);

                if (escalera != null)
                {
                    var color = Color(cartas); //Si es Escalera de color, se retormna los resultados de la Escalera de color 

                    if (color != null)
                    {
                       
                        return new ResultadoDeLaMano(ManoEnum.EscaleraDeColor, escalera.Cartas);
                    }
                }

                return null;
            }


            private static ResultadoDeLaMano Escalera(List<ICarta> cartas)
            {
                
                var cartasOrdenadas = cartas.OrderByDescending(c => c.Valor).ToList(); //cartas de Mayor a Menor 

                
                foreach (var carta in cartasOrdenadas) //Revisar una por una para ver si son consecutivas 
                {
                    if (carta.Valor != cartasOrdenadas[0].Valor - cartasOrdenadas.IndexOf(carta))
                    {
                        //Mueve al principio de la lista el as si es la ultima carta pero tiene una escalera con todos los valores de 10 
                        bool esUnAs = carta.Valor == ValoresCartasEnum.As;
                        bool esUltimaCarta = cartasOrdenadas.IndexOf(carta) == cartasOrdenadas.Count - 1;
                        if (esUnAs && cartasOrdenadas[0].Valor == ValoresCartasEnum.Rey && esUltimaCarta)
                        {
                            cartasOrdenadas.Insert(0, cartasOrdenadas.Last()); //Se mueve al principio de la lista 
                            cartasOrdenadas.RemoveAt(cartasOrdenadas.Count - 1);

                            return new ResultadoDeLaMano(ManoEnum.Escalera, cartasOrdenadas);
                        }

                        return null;
                    }
                }
                return new ResultadoDeLaMano(ManoEnum.Escalera, cartasOrdenadas);
            }


            private ResultadoDeLaMano FullHouse(List<ICarta> cartas)
            {
                var trio = Trio(cartas);

                if (trio != null)
                {
                    //cartas sin trio (trio devuelve las 5 cartas asi que se quitan las primeras 3)
                    var cartasQueNoSonTrio = trio.Cartas.Skip(3).ToList();

                    if (cartasQueNoSonTrio[0].Valor == cartasQueNoSonTrio[1].Valor)
                    {
                        return new ResultadoDeLaMano(ManoEnum.FullHouse, trio.Cartas); //Si hay parejas en las cartas restantes, lo que se retorna es Full House 
                    }
                }

                return null;
            }

            private ResultadoDeLaMano Trio(List<ICarta> cartas)
            {

                var trio = cartas.GroupBy(c => c.Valor).Where(g => g.Count() == 2).SelectMany(g => g).ToList(); //Aqui se elijen las cartas que son  trio y se ponen al frente de la lista, se agrupan las cartas por su valor corresponeidnte, las que se repiyten 3 veces y se conviente en lista 

                var cartasQueNoSonTrio = cartas.Except(trio).OrderByDescending(c => c.Valor).ToList();

                cartasQueNoSonTrio = MoverAsAlPrincipio(cartasQueNoSonTrio);
                var cartasOrdenadas = trio.Concat(cartasQueNoSonTrio);

                return trio.Count == 3 ? new ResultadoDeLaMano(ManoEnum.Trio, cartasOrdenadas) : null;
            }


            private  ResultadoDeLaMano Poker(List<ICarta> cartas)
            {
                var poker = cartas.GroupBy(c => c.Valor).Where(g => g.Count() == 4).SelectMany(g => g).ToList(); //Aqui se elijen las cartas que son poker y se ponen al frente de la lista, se agrupan las cartas po su valor corresponeidnte, las que se repiyten 4 veces y se conviente en lista 
                var cartasQueNoSonPoker = cartas.Except(poker).OrderByDescending(c => c.Valor).ToList();
                cartasQueNoSonPoker = MoverAsAlPrincipio(cartasQueNoSonPoker);

                var cartasOrdenadas = poker.Concat(cartasQueNoSonPoker).ToList(); //manda al final de nuestra lista esas cartas que nos son poker 


                return poker.Count == 4 ? new ResultadoDeLaMano(ManoEnum.Poker, cartasOrdenadas) : null; //Si hay poker, lo que se retorna es la mano de poker 
            }


            private  ResultadoDeLaMano Color(List<ICarta> cartas)
            {
                // Se seleccionan las cartas que tienen el mismo palo
                var cartasConLaFigura = cartas.GroupBy(c => c.Figura) .Where(g => g.Count() >= 5) .SelectMany(g => g) .OrderByDescending(c => c.Valor).ToList();

                cartasConLaFigura = MoverAsAlPrincipio(cartasConLaFigura); //Se mueve el As al principio dependeindo del valor que tenga 

                return cartasConLaFigura.Count >= 5 ? new ResultadoDeLaMano(ManoEnum.Color, cartasConLaFigura) : null; // si es Color, lo que se retorna es la mano de Color 
            }


            private ResultadoDeLaMano DoblePareja(List<ICarta> cartas)
            {
                var parejas = cartas.GroupBy(c => c.Valor).Where(g => g.Count() == 2).SelectMany(g => g).ToList();
                parejas.OrderByDescending(c => c.Valor);
                parejas = MoverAsAlPrincipio(parejas);

                var cartasOrdenadas = parejas.Concat(cartas.Except(parejas).OrderByDescending(c => c.Valor).ToList());

                return parejas.Count == 4 ? new ResultadoDeLaMano(ManoEnum.DoblePar, cartasOrdenadas) : null;
            }

            private ResultadoDeLaMano Pareja(List<ICarta> cartas)
            {
                var pareja = cartas.GroupBy(c => c.Valor).Where(g => g.Count() == 2).SelectMany(g => g).ToList(); //Aqui se elijen las cartas que son parejas y se ponen al frente de la lista, se agrupan las cartas po su valor corresponeidnte, las que se repiyten 2 veces y se conviente en lista 
                var cartasQueNoSonPareja = cartas.Except(pareja).OrderByDescending(c => c.Valor).ToList();
                cartasQueNoSonPareja = MoverAsAlPrincipio(cartasQueNoSonPareja);

                var cartasOrdenadas = pareja.Concat(cartasQueNoSonPareja).ToList();

                return pareja.Count == 2 ? new ResultadoDeLaMano(ManoEnum.Par, cartasOrdenadas) : null; // si es Pareja, lo que se retorna es el mano Par

            }

            private ResultadoDeLaMano CartaAlta(List<ICarta> cartas)
            {
                var cartasOrdenadas = cartas.OrderByDescending(c => c.Valor).ToList();

                cartasOrdenadas = MoverAsAlPrincipio(cartasOrdenadas); //Se mueve el As al inicio

                return new ResultadoDeLaMano(ManoEnum.CartaAlta, cartasOrdenadas);  // Aquise retorna el puntaje de la primera carta de las cartas ordenas ahora tendiendo el valor mas alto
            }

            private List<ICarta> MoverAsAlPrincipio(List<ICarta> cartas)
            {
                var cartasOrdenadas = cartas.OrderByDescending(c => c.Valor).ToList();
                var listaDeAses = new List<ICarta>();
                var listaSinAses = new List<ICarta>();

                for (int i = 0; i <= cartasOrdenadas.Count - 1; i++)
                {
                    if (cartasOrdenadas[i].Valor == ValoresCartasEnum.As)
                    {
                        listaDeAses.Add(cartasOrdenadas[i]);
                    }
                    else
                    {
                        listaSinAses.Add(cartasOrdenadas[i]);
                    }
                }

                var listaFinal = listaDeAses.Concat(listaDeAses).ToList();

                return listaFinal;
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

            public void jugar(List<IJugador> jugadores) //Desta forma el dealer puede mover las cartas entre jugadores 
            {
                Console.WriteLine("Turno del Dealer");
                RepartirCartasIniciales(jugadores);
                var cartasUsadas = new List<ICarta>();
                foreach (var jugador in jugadores)
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
    }
}




