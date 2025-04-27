using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GRA2D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void Wyjdź_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); //Zamyka cała aplikację
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow(); // tworzymy nowe okno 
            gameWindow.Show(); // pokazujemy nowe okno
            this.Close(); // zamykamy aktualne okno
        }
    }
}