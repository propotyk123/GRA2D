using System;
using System.Collections.Generic;
using System.IO;
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
            //string sciezka = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "GameWindow", "Mapa", "mapa.txt");
            WczytajMape("mapa.txt");

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
        private void WczytajMape(string sciezkaPliku)
        {
            try
            {
                var linie = File.ReadAllLines(sciezkaPliku); //zwraca tablicę stringów, np. linie[0] to pierwsza linia pliku
                wysokoscMapy = linie.Length; //liczba linii w pliku
                szerokoscMapy = linie[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length; //zwraca liczbe elementów w tablicy czyli liczbe kolumn w pliku
                mapa = new int[wysokoscMapy, szerokoscMapy]; //przypisujemy do mapy ile ma wierszy i kolumn
                for (int y = 0; y < wysokoscMapy; y++)
                {
                    var czesci = linie[y].Split(' ', StringSplitOptions.RemoveEmptyEntries); //zwraca tablice stringów np. czesci[0] to pierwszy element linii
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        mapa[y, x] = int.Parse(czesci[x]); //wczytywanie mapy z pliku i przypisywanie do tablicy dwuwymiarowej
                    }
                }
                //przygotwanie kontenera SiatkaMapy - czyszczenie elementów i definicja wierszy/kolumn
                SiatkaMapy.Children.Clear(); //czyści kontener
                SiatkaMapy.RowDefinitions.Clear(); //czyści wiersze
                SiatkaMapy.ColumnDefinitions.Clear(); //czyści kolumny

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    SiatkaMapy.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(RozmiarSegmentu) }); //dodaje nowy wiersz do siatki o wysokosci 64 pikseli
                }
                for (int x = 0; x < szerokoscMapy; x++)
                {
                    SiatkaMapy.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(RozmiarSegmentu) }); //dodaje nową kolumnę do siatki o szerokości 64 pikseli
                }
                //Tworzenie tablicy kontrolek Image i dodawanie ich do siatki
                tablicaTerenu = new Image[wysokoscMapy, szerokoscMapy]; //przypisujemy do tablicy ile ma wierszy i kolumn
                for (int y = 0; y < wysokoscMapy; y++)
                {
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        Image obraz = new Image //przypisujemy szerokosc i wysokosc obrazu
                        {
                            Width = RozmiarSegmentu,
                            Height = RozmiarSegmentu
                        };
                        int rodzaj = mapa[y, x];
                        if (rodzaj >= 1 && rodzaj < ILE_TERENOW) //sprawdzamy czy rodzaj terenu jest poprawny
                        {
                            obraz.Source = obrazyTerenu[rodzaj]; //Wczytywanie obrazka terenu
                        }
                        else
                        {
                            obraz.Source = null; //jeżeli rodzaj terenu jest niepoprawny to ustawiamy obrazek na null
                        }
                        Grid.SetRow(obraz, y); //ustawiamy wiersz
                        Grid.SetColumn(obraz, x); //ustawiamy kolumnę
                        SiatkaMapy.Children.Add(obraz); //dodajemy obrazek do siatki
                        tablicaTerenu[y, x] = obraz; //przypisujemy obrazek do tablicy terenu
                    }
                }
                //Dodawanie obrazka gracza - ustawiamy go na wierzchu
                SiatkaMapy.Children.Add(ObrazGracza); //dodajemy obrazek gracza do siatki
                Panel.SetZIndex(ObrazGracza, 1); //ustawiamy z-index obrazka gracza na 1, aby był na wierzchu
                //Ustawiamy pozycję gracza na 0,0
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();

                iloscPieniedzy = 0;
                level = 0;
                EtykiataLevel.Content = "Level: " + level; //resetuje level
                EtykietaPieniadzy.Content = "Pieniądze: " + iloscPieniedzy; //resetuje pieniądze
            } //koniec try
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message); //jeżeli wystąpił błąd to wyświetlamy komunikat
            } //koniec catch
        }
        private void AktualizujPozycjeGracza()
        {

        }
    }
}
