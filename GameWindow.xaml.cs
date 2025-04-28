using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GRA2D
{
    public partial class GameWindow : Window
    {
        // Stałe reprezentujące rodzeje terenu
        public const int KAMIEN = 1;
        public const int WEGIEL = 2;
        public const int ZELAZO = 3;
        public const int ZLOTO = 4;
        public const int DIAMENT = 5;
        public const int EMERALD = 6;
        public const int ILE_TERENOW = 7;
        // Mapa przechowywana jako tablica dwywymiarowa int
        private int[,] mapa;
        private int szerokoscMapy;
        private int wysokoscMapy;
        //Dwuwymiarowa tablica kontrolek IMAGE reprezentująca segmenty mapy
        private Image[,] tablicaTerenu;
        //Rozmiar jednego segmentu mapy w pikselach
        private const int RozmiarSegmentu = 64;
        //Tablica obrazków terenu - indeks odpowiada rodzajowi terenu
        //INdeks 1: kamien, 2:wegiel, 3: zelazo, 4: zloto, 5: diament, 6: emerald
        private BitmapImage[] obrazyTerenu = new BitmapImage[ILE_TERENOW];
        //Pozycja gracza na mapie
        private int pozycjaGraczaX = 0;
        private int pozycjaGraczaY = 0;
        //Obrazek reprezentujący gracza
        private Image ObrazGracza;
        //Licznik pieniedzy i levelu
        public int iloscPieniedzy = 0;
        public int level = 0;
        public GameWindow()
        {
            InitializeComponent();
            WczytajObrazyTerenu();
            // Inicjalizacja obrazka gracza
            ObrazGracza = new Image
            {
                Width = RozmiarSegmentu,
                Height = RozmiarSegmentu
            };
            BitmapImage bmpGracza = new BitmapImage(new Uri("assets/GameWindow/Gracz/gracz.png", UriKind.Relative));
            ObrazGracza.Source = bmpGracza;
        }

        private void Powrot_Menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(); // tworzymy nowe okno 
            mainWindow.Show(); // pokazujemy nowe okno
            this.Close(); // zamykamy aktualne okno
        }
        private void WczytajObrazyTerenu()
        {
            obrazyTerenu[KAMIEN] = new BitmapImage(new Uri("assets/GameWindow/Teren/kamien.png", UriKind.Relative));
            obrazyTerenu[WEGIEL] = new BitmapImage(new Uri("assets/GameWindow/Teren/wegiel.png", UriKind.Relative));
            obrazyTerenu[ZELAZO] = new BitmapImage(new Uri("assets/GameWindow/Teren/zelazo.png", UriKind.Relative));
            obrazyTerenu[ZLOTO] = new BitmapImage(new Uri("assets/GameWindow/Teren/zloto.png", UriKind.Relative));
            obrazyTerenu[DIAMENT] = new BitmapImage(new Uri("assets/GameWindow/Teren/diament.png", UriKind.Relative));
            obrazyTerenu[EMERALD] = new BitmapImage(new Uri("assets/GameWindow/Teren/emerald.png", UriKind.Relative));
        }

    }
}
