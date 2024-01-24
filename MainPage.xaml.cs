using AppJuego.Models;
using AppJuego.Services;

namespace AppJuego
{
    public partial class MainPage : ContentPage
    {
        private readonly APIService _apiService;

        private char currentPlayer = 'X';
        private int currentPlayerNumber = 1;

        public MainPage()
        {
            InitializeComponent();
            InitializeGameGrid();
            _apiService = new APIService();
            CargarJuegosGanados();
        }

        private void InitializeGameGrid()
        {
            gameGrid.Children.Clear();
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var button = new Button
                    {
                        Text = "",
                        FontSize = 24,
                        BackgroundColor = Colors.LightGray
                    };
                    button.Clicked += OnCellClicked;
                    gameGrid.Add(button, col, row);
                }
            }
        }

        private void OnStartResetClicked(object sender, EventArgs e)
        {
            InitializeGameGrid();
            CargarJuegosGanados();
            currentPlayer = 'X';
            currentPlayerNumber = 1; // Restablece el número del jugador a 1
            playerTurnLabel.Text = "Turno: jugador 1";
        }

        private bool CheckWinner()
        {
            // Las combinaciones ganadoras en un tablero de tres en raya
            int[,] winningCombinations = new int[,]
            {
                {0, 1, 2}, // Primera fila
                {3, 4, 5}, // Segunda fila
                {6, 7, 8}, // Tercera fila
                {0, 3, 6}, // Primera columna
                {1, 4, 7}, // Segunda columna
                {2, 5, 8}, // Tercera columna
                {0, 4, 8}, // Diagonal principal
                {2, 4, 6}  // Diagonal inversa
            };

            var buttons = gameGrid.Children.Cast<Button>().ToArray();

            for (int i = 0; i < 8; i++)
            {
                int a = winningCombinations[i, 0];
                int b = winningCombinations[i, 1];
                int c = winningCombinations[i, 2];

                string aText = buttons[a].Text;
                string bText = buttons[b].Text;
                string cText = buttons[c].Text;

                // Verifica si las casillas no están vacías y son iguales
                if (!string.IsNullOrEmpty(aText) && aText == bText && bText == cText)
                {
                    return true;
                }
            }

            return false;
        }

        private async void OnCellClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button.Text == "")
            {
                button.Text = currentPlayer.ToString();
                if (CheckWinner())
                {
                    // Cambia el mensaje para que muestre el número del jugador ganador
                    await DisplayAlert("Ganador", $"Jugador {currentPlayerNumber} ha ganado!", "OK");

                    // Registrar el ganador
                    var juegoGanado = new Juego
                    {
                        PlayerNumber = currentPlayerNumber,
                        GameDateTime = DateTime.Now
                    };
                    await _apiService.RegistrarGanador(juegoGanado);

                    OnStartResetClicked(this, EventArgs.Empty);
                    return;
                }
                else if (IsTie())
                {
                    await DisplayAlert("Empate", "¡Es un empate!", "OK");
                    OnStartResetClicked(this, EventArgs.Empty);
                    return;
                }

                // Cambiar el jugador actual y el número del jugador
                if (currentPlayer == 'X')
                {
                    currentPlayer = 'O';
                    currentPlayerNumber = 2;
                }
                else
                {
                    currentPlayer = 'X';
                    currentPlayerNumber = 1;
                }

                // Actualiza esta línea para mostrar el número del jugador en lugar de 'X' o 'O'
                playerTurnLabel.Text = $"Turno: jugador {currentPlayerNumber}";
            }
        }

        private bool IsTie()
        {
            var buttons = gameGrid.Children.Cast<Button>().ToArray();
            foreach (var button in buttons)
            {
                if (string.IsNullOrEmpty(button.Text))
                    return false; // Si hay alguna celda vacía, no es empate
            }
            return true; // Si todas las celdas están llenas y no hay ganador, es empate
        }


        private async void CargarJuegosGanados()
        {
            var juegosGanados = await _apiService.ObtenerGanadores();
            var juegosJugador1 = juegosGanados.Where(j => j.PlayerNumber == 1).ToList();
            var juegosJugador2 = juegosGanados.Where(j => j.PlayerNumber == 2).ToList();

            listaJuegosGanadosJugador1.ItemsSource = juegosJugador1;
            listaJuegosGanadosJugador2.ItemsSource = juegosJugador2;
        }



    }


}
