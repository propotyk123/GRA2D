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
        //Zmienne do obsługi Generowania mapy
        private int IlesegmentowX = 5;
        private int IlesegmentowY = 5;
        //zmiene do obsługi XP levelu
        private int iloscXP = 0;
        private int potrzebnailoscXP = 100;
        //Zmiene do obslugi sklepu za level
        private int iloscPunktow_Level = 0;
        private int MnoznikXP = 1;
        private int MnoznikPieniedzy = 1;
        //Zmienne do obslugi sklepu za pieniadze
        private int PoziomKilofa = 1;
        private int KosztKilofa = 50;
        private int PoziomWielkosciMapy = 0;
        private int KosztWielkosciMapy = 200;
        public GameWindow()
        {
            InitializeComponent();
            GenerujMape(IlesegmentowX, IlesegmentowY); //generuje mape o podanych wymiarach
            WczytajObrazyTerenu();
            
            

            // Inicjalizacja obrazka gracza
            ObrazGracza = new Image
            {
                Width = RozmiarSegmentu,
                Height = RozmiarSegmentu
            };
            BitmapImage bmpGracza = new BitmapImage(new Uri("assets/GameWindow/Gracz/gracz.png", UriKind.Relative)); 
            ObrazGracza.Source = bmpGracza;


            WczytajMape("mapa.txt");
        }

        private void Powrot_Menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(); // tworzymy nowe okno 
            mainWindow.Show(); // pokazujemy nowe okno
            this.Close(); // zamykamy aktualne okno
        }
        private void WczytajObrazyTerenu() //przypisuje dane grafiki do tablicy
        {
            obrazyTerenu[KAMIEN] = new BitmapImage(new Uri("assets/GameWindow/Teren/Kamien.png", UriKind.Relative));
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
            } //koniec try
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message); //jeżeli wystąpił błąd to wyświetlamy komunikat
            } //koniec catch
        }
        private void GenerujMape(int IlesegmentowX , int IlesegmentowY)
        {
            IlesegmentowX = IlesegmentowX + PoziomWielkosciMapy; //dodajemy poziom wielkosci mapy do ilosci segmentow X
            IlesegmentowY = IlesegmentowY + PoziomWielkosciMapy; //dodajemy poziom wielkosci mapy do ilosci segmentow Y
            Random rnd = new Random();
            StreamWriter writer = new StreamWriter("mapa.txt"); //tworzy plik tekstowy
            for (int i = 0; i < IlesegmentowX; i++)
            {
                for (int j = 0; j < IlesegmentowY; j++)
                {
                    int szansa = rnd.Next(100); //generuje wartosc od 0 do 99 
                    if (PoziomKilofa == 1)
                    {
                        if (szansa < 80) //jesli liczba jest mniejsza od 80 dodaje 1 czyli liczba 1 ma 80% szans na wylosowanie
                        {
                            writer.Write(1 + " ");
                        }
                        else if (szansa >= 80) //jesli liczba jest wieksza lub równa 80 dodaje 2 czyli liczba 2 ma 20% szans na wylosowanie
                        {
                            writer.Write(2 + " ");
                        }
                    }
                    if (PoziomKilofa == 2)
                    {
                        if (szansa < 70) //jesli liczba jest mniejsza od 70 dodaje 1 czyli liczba 1 ma 70% szans na wylosowanie
                        {
                            writer.Write(1 + " ");
                        }
                        else if (szansa >= 70 && szansa < 90) //jesli liczba jest wieksza lub równa 70 i mniejsza od 90 dodaje 2 czyli liczba 2 ma 20% szans na wylosowanie
                        {
                            writer.Write(2 + " ");
                        }
                        else if (szansa >= 90) //jesli liczba jest wieksza lub równa 90 dodaje 3 czyli liczba 3 ma 10% szans na wylosowanie
                        {
                            writer.Write(3 + " ");
                        }

                    }
                    if (PoziomKilofa == 3)
                    {
                        if (szansa < 60) //jesli liczba jest mniejsza od 60 dodaje 1 czyli liczba 1 ma 60% szans na wylosowanie
                        {
                            writer.Write(1 + " ");
                        }
                        else if (szansa >= 60 && szansa < 80) //jesli liczba jest wieksza lub równa 60 i mniejsza od 80 dodaje 2 czyli liczba 2 ma 20% szans na wylosowanie
                        {
                            writer.Write(2 + " ");
                        }
                        else if (szansa >= 80 && szansa < 95) //jesli liczba jest wieksza lub równa 80 i mniejsza od 95 dodaje 3 czyli liczba 3 ma 15% szans na wylosowanie
                        {
                            writer.Write(3 + " ");
                        }
                        else if (szansa >= 95) //jesli liczba jest wieksza lub równa 95 dodaje 4 czyli liczba 4 ma 5% szans na wylosowanie
                        {
                            writer.Write(4 + " ");
                        }
                    }
                    if(PoziomKilofa == 4)
                    {
                        if (szansa < 50) //jesli liczba jest mniejsza od 50 dodaje 1 czyli liczba 1 ma 50% szans na wylosowanie
                        {
                            writer.Write(1 + " ");
                        }
                        else if (szansa >= 50 && szansa < 70) //jesli liczba jest wieksza lub równa 50 i mniejsza od 70 dodaje 2 czyli liczba 2 ma 20% szans na wylosowanie
                        {
                            writer.Write(2 + " ");
                        }
                        else if (szansa >= 70 && szansa < 90) //jesli liczba jest wieksza lub równa 70 i mniejsza od 90 dodaje 3 czyli liczba 3 ma 20% szans na wylosowanie
                        {
                            writer.Write(3 + " ");
                        }
                        else if (szansa >= 90 && szansa < 98) //jesli liczba jest wieksza lub równa 90 i mniejsza od 98 dodaje 4 czyli liczba 4 ma 8% szans na wylosowanie
                        {
                            writer.Write(4 + " ");
                        }
                        else if (szansa >= 98) //jesli liczba jest wieksza lub równa 98 dodaje 5 czyli liczba 5 ma 2% szans na wylosowanie
                        {
                            writer.Write(5 + " ");
                        }
                    }
                    if(PoziomKilofa == 5)
                    {
                        if (szansa < 30) //jesli liczba jest mniejsza od 30 dodaje 1 czyli liczba 1 ma 30% szans na wylosowanie
                        {
                            writer.Write(1 + " ");
                        }
                        else if (szansa >= 30 && szansa < 50) //jesli liczba jest wieksza lub równa 30 i mniejsza od 50 dodaje 2 czyli liczba 2 ma 20% szans na wylosowanie
                        {
                            writer.Write(2 + " ");
                        }
                        else if (szansa >= 50 && szansa < 70) //jesli liczba jest wieksza lub równa 50 i mniejsza od 70 dodaje 3 czyli liczba 3 ma 20% szans na wylosowanie
                        {
                            writer.Write(3 + " ");
                        }
                        else if (szansa >= 70 && szansa < 85) //jesli liczba jest wieksza lub równa 70 i mniejsza od 85 dodaje 4 czyli liczba 4 ma 15% szans na wylosowanie
                        {
                            writer.Write(4 + " ");
                        }
                        else if (szansa >= 85 && szansa < 95) //jesli liczba jest wieksza lub równa 85 i mniejsza od 95 dodaje 5 czyli liczba 5 ma 4% szans na wylosowanie
                        {
                            writer.Write(5 + " ");
                        }
                        else if (szansa >= 95) //jesli liczba jest wieksza lub równa 99 dodaje 6 czyli liczba 6 ma 1% szans na wylosowanie
                        {
                            writer.Write(6 + " ");
                        }
                    }
                }
                writer.WriteLine(); //dodaje nową linię
            }
            writer.Close(); //zamyka plik
        }
        private void AktualizujPozycjeGracza() //Aktualizuje pozycję obrazka gracza w siatce
        {
            Grid.SetRow(ObrazGracza, pozycjaGraczaY); //ustawiamy wiersz
            Grid.SetColumn(ObrazGracza, pozycjaGraczaX); //ustawiamy kolumnę
        }
        private void OknoGry_KeyDown(object sender, KeyEventArgs e)
        {
            //przypisujemy obecna pozycje gracza 
            int nowyX = pozycjaGraczaX;
            int nowyY = pozycjaGraczaY;
            //Zmiana pozycji gracza w zależności od wciśniętego klawisza
            if (e.Key == Key.W ||e.Key == Key.Up) //jesli wcisnięto strzałke w góre albo w
            {
                nowyY--; //zmniejszamy Y dzieki czemu postac przesuwa sie o 1 w gore
            }
            else if (e.Key == Key.S || e.Key == Key.Down) //jesli wcisnieto strzałke w dół albo s 
            {
                nowyY++; //zwiekszamy Y dzieki czemu postac przesuwa sie o 1 w dół
            }
            else if (e.Key == Key.A || e.Key == Key.Left) //jesli wcisnieto strzałke w lewo albo a
            {
                nowyX--; //zmniejszamy X dzieki czemu postac przesuwa sie o 1 w lewo
            }
            else if (e.Key == Key.D || e.Key == Key.Right) //jesli wcisnieto strzałke w prawo albo d
            {
                nowyX++; //zwiekszamy X dzieki czemu postac przesuwa sie o 1 w prawo
            }
            //Sprawdzamy czy nowa pozycja gracza jest w granicach mapy
            if (nowyX >= 0 && nowyX < szerokoscMapy && nowyY >= 0 && nowyY < wysokoscMapy)
            {
                //przypisujemy nowa pozycje gracza
                pozycjaGraczaX = nowyX;
                pozycjaGraczaY = nowyY;
                AktualizujPozycjeGracza(); //aktualizujemy pozycje gracza
            }

                //Automatyczne resetowanie mapy
                bool reset = true; //zmienna ktora odpowiada czy trzeba resetowac mape czy nie
                for(int i = 0 ; i < mapa.GetLength(0); i++)
                {
                    for(int j = 0; j < mapa.GetLength(1); j++)
                    {
                        if(mapa[i, j] != KAMIEN) //jesli mapa nie jest kamieniem to zmieniamy zmienna na false
                        {
                            reset = false; //zmieniamy zmienna na false
                            break; //przerywamy petle
                        }
                    }
                }
                if(reset == true)
                {
                    GenerujMape(IlesegmentowX, IlesegmentowY); //generuje mape o podanych wymiarach
                    WczytajMape("mapa.txt");
                }


            //Obsluga kopania - naciskamy spacje
            if (e.Key == Key.Space)
            {
                if (mapa[pozycjaGraczaY, pozycjaGraczaX] == WEGIEL) //jesli gracz stoi na weglu
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = KAMIEN; //po wykopaniu zmieniamy rodzaj terenu na kamien
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[KAMIEN]; //zmieniamy obrazek terenu na kamien
                    DodajPieniadze(1); //dodajemy pieniadze
                    DodajXP(10); //dodajemy XP
                }
                if (mapa[pozycjaGraczaY , pozycjaGraczaX] == ZELAZO) //jesli gracz stoi na zelazie
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = KAMIEN; //po wykopaniu zmieniamy rodzaj terenu na kamien
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[KAMIEN]; //zmieniamy obrazek terenu na kamien
                    DodajPieniadze(4); //dodajemy pieniadze
                    DodajXP(20); //dodajemy XP
                }
                if(mapa[pozycjaGraczaY, pozycjaGraczaX] == ZLOTO) //jesli gracz stoi na zloto
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = KAMIEN; //po wykopaniu zmieniamy rodzaj terenu na kamien
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[KAMIEN]; //zmieniamy obrazek terenu na kamien
                    DodajPieniadze(8); //dodajemy pieniadze
                    DodajXP(40); //dodajemy XP
                }
                if(mapa[pozycjaGraczaY, pozycjaGraczaX] == DIAMENT) //jesli gracz stoi na diament
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = KAMIEN; //po wykopaniu zmieniamy rodzaj terenu na kamien
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[KAMIEN]; //zmieniamy obrazek terenu na kamien
                    DodajPieniadze(16); //dodajemy pieniadze
                    DodajXP(80); //dodajemy XP
                }
                if (mapa[pozycjaGraczaY, pozycjaGraczaX] == EMERALD) //jesli gracz stoi na emerald
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = KAMIEN; //po wykopaniu zmieniamy rodzaj terenu na kamien
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[KAMIEN]; //zmieniamy obrazek terenu na kamien
                    DodajPieniadze(40); //dodajemy pieniadze
                    DodajXP(160); //dodajemy XP
                }

            }

        }
        private void DodajPieniadze(int ilosc) //dodaje pieniadze
        {
            iloscPieniedzy += ilosc * MnoznikPieniedzy; //dodajemy pieniadze
            EtykietaPieniadzy.Content = "Pieniądze: " + iloscPieniedzy; //aktualizujemy etykiete pieniedzy
        }
        private void AktualizujPieniadze() //aktualizuje pieniadze
        {
            EtykietaPieniadzy.Content = "Pieniądze: " + iloscPieniedzy; //aktualizuje etykiete z iloscia pieniedzy
        }
        private void DodajXP(int ilosc) //dodaje XP
        {
            iloscXP += ilosc * MnoznikXP; //dodajemy XP
            XPText.Text = iloscXP + "/" + potrzebnailoscXP; //aktualizujemy etykiete XP
            if (iloscXP >= potrzebnailoscXP)
            {
                level++; //zwiekszamy level
                EtykiataLevel.Content = "Level: " + level; //aktualizujemy etykiete levelu
                iloscXP = 0; //resetujemy XP
                potrzebnailoscXP = potrzebnailoscXP * 2; //zwiekszamy potrzebna ilosc XP do nastepnego levelu
                ProgressXP.Maximum = potrzebnailoscXP; //ustawiamy maksymalna wartosc paska XP
                iloscPunktow_Level++; //dodajemy punkty za kazdy zdobyty poziom
                AktualizujPunkty(); //aktualizujemy punkty
                XPText.Text = iloscXP + "/" + potrzebnailoscXP; //aktualizujemy etykiete XP
            }
            ProgressXP.Value = iloscXP; //aktualizujemy pasek XP
        }
        private void AktualizujPunkty() //aktualizuje punkty
        {
            IloscPunktow.Content = $"Masz {iloscPunktow_Level} punkty"; //aktualizuje etykiete z iloscia punktow w sklepie
        }
        //Funkcję do obsługi otwierania i zamykania sklepów
        private void Sklep_punkty_Click(object sender, RoutedEventArgs e)
        {
            SklepZaPunkty.Visibility = Visibility.Visible; //otwiera sklep za punkty
        }

        private void ZamknijSklepLevel_Click(object sender, RoutedEventArgs e)
        {
            SklepZaPunkty.Visibility = Visibility.Collapsed; //zamyka sklep za punkty
        }
        private void Sklep_pieniadze_Click(object sender, RoutedEventArgs e)
        {
            SklepZaPieniadze.Visibility = Visibility.Visible; //otwiera sklep za pieniądze
        }
        private void ZamknijSklepPieniadze_Click(object sender, RoutedEventArgs e)
        {
            SklepZaPieniadze.Visibility = Visibility.Collapsed; //zamyka sklep za pieniądze
        }

        //Funkcje do obsługi kupowania ulepszeń
        private void MnożnikXPUlepszenie_Click(object sender, RoutedEventArgs e)
        {
            if (iloscPunktow_Level >= 1)
            {
                MnoznikXP++; //zwiekszamy mnoznik XP
                iloscPunktow_Level--; //zmniejszamy ilosc punktow
                Mnożnik_XP.Content = $"Mnożnik XP x{MnoznikXP + 1} - Zdobywaj {MnoznikXP + 1}x więcej xp";
                AktualizujPunkty(); //aktualizujemy punkty
            }
           
        }

        private void MnożnikPieniedzyUlepszenie_Click(object sender, RoutedEventArgs e)
        {
            if (iloscPunktow_Level >= 1)
            {
                MnoznikPieniedzy++; //zwiekszamy mnoznik pieniedzy
                iloscPunktow_Level--; //zmniejszamy ilosc punktow
                Mnożnik_Pieniedzy.Content = $"Mnożnik Pieniędzy x{MnoznikPieniedzy + 1} - Zdobywaj {MnoznikPieniedzy + 1}x więcej pieniędzy";
                AktualizujPunkty(); //aktualizujemy punkty
            }
        }

        private void UlepszKilof_Click(object sender, RoutedEventArgs e)
        {
            if(iloscPieniedzy >= KosztKilofa)
            {
                    iloscPieniedzy = iloscPieniedzy - KosztKilofa; //zmniejszamy ilosc pieniedzy
                    AktualizujPieniadze(); //aktualizujemy pieniadze
                    PoziomKilofa++; //zwiekszamy poziom kilofa
                    PoziomKilofa_label.Content = $"Ulepsz kilof - {PoziomKilofa}/5 - pozwala kopać lepsze minerały";
                    KosztKilofa = KosztKilofa * 2; //zwiekszamy koszt kilofa
                    PoziomKilofKoszt.Content = $"Koszt ulepszenia: {KosztKilofa}";
                    
                if (PoziomKilofa == 5)
                {
                    PoziomKilofKoszt.Content = $"Osiagnięto maksymalny poziom kilofa";
                    UlepszKilof.IsEnabled = false; //wyłącza przycisk ulepszania kilofak
                }

            }
        }
        private void UlepszWielkoscMapy_Click(object sender, RoutedEventArgs e)
        {
            if (iloscPieniedzy >= KosztWielkosciMapy)
            {
                iloscPieniedzy = iloscPieniedzy - KosztWielkosciMapy; //zmniejszamy ilosc pieniedzy
                AktualizujPieniadze(); //aktualizujemy pieniadze
                PoziomWielkosciMapy++; //zwiekszamy poziom wielkosci mapy
                WielkoscMapy_Label.Content = $"Ulepsz wielkość mapy - {PoziomWielkosciMapy}/5";
                KosztWielkosciMapy = KosztWielkosciMapy * 2; //zwiekszamy koszt wielkosci mapy
                PoziomMapyKoszt.Content = $"Koszt ulepszenia: {KosztWielkosciMapy}";
            }
            if(PoziomWielkosciMapy == 5)
            {
                PoziomMapyKoszt.Content = $"Osiagnięto maksymalny poziom mapy";
                UlepszWielkoscMapy.IsEnabled = false; //wyłącza przycisk ulepszania wielkosci mapy
            }
        }
    }
}
