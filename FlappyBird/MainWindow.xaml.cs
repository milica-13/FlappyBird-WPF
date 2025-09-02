using Aspose.Pdf.Annotations;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.IconPacks;
using System.Windows.Media.Animation;

namespace FlappyBird
{
    public partial class MainWindow : Window
    {
        private string birdImagePath = "Resources/bird1.png";
        public static bool IsSoundOn { get; set; } = true;  

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var cloudStoryboard = (Storyboard)this.Resources["CloudFloatStoryboard"];
            cloudStoryboard?.Begin();

            var cityStoryboard = (Storyboard)this.Resources["CityGlowStoryboard"];
            cityStoryboard?.Begin();
        }



        private void ShowInstructions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Odaberi svoju pticu \n⬆️ Pritisni SPACE da ptica skoči\n🕳️ Izbjegavaj prepreke\n✅ Osvoji što više poena!",
                            "Uputstvo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SoundToggle_Checked(object sender, RoutedEventArgs e)
        {
            var icon = FindChild<PackIconMaterial>(SoundToggleButton, "SoundIcon");
            if (icon != null)
                icon.Kind = PackIconMaterialKind.VolumeHigh;

            var label = FindChild<TextBlock>(SoundToggleButton, null);
            if (label != null)
                label.Text = "Zvuk: On";
                
            IsSoundOn = true;
        }

        private void SoundToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            var icon = FindChild<PackIconMaterial>(SoundToggleButton, "SoundIcon");
            if (icon != null)
                icon.Kind = PackIconMaterialKind.VolumeMute;

            var label = FindChild<TextBlock>(SoundToggleButton, null);
            if (label != null)
                label.Text = "Zvuk: Off";

            IsSoundOn = false;
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow(birdImagePath);

            gameWindow.Show();
            this.Close();
        }


        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T childType)
                {
                    if (childName == null)
                        return childType;

                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                        return childType;
                }

                T result = FindChild<T>(child, childName);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void SelectBird_Click(object sender, RoutedEventArgs e)
        {
            var birdSelectionWindow = new BirdSelectionWindow();
            birdSelectionWindow.Show();
            //this.Close();
        }

        private void ShowTopScores_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "topscores.txt";
            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show("Nema spremljenih rezultata.", "Top rezultati", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var lines = System.IO.File.ReadAllLines(filePath);
            var topScores = lines.Select(line => int.TryParse(line, out int score) ? score : 0)
                .OrderByDescending(s => s)
                .Take(3)
                .ToList();

            string message = "🏆 Top 3 rezultata:\n\n";
            for (int i = 0; i < topScores.Count; i++)
            {
                message += $" {i + 1}. {topScores[i]} poena\n";
            }

            MaterialDesignThemes.Wpf.DialogHost.Show(message, "RootDialog");


            MessageBox.Show(message, "Top rezultati", MessageBoxButton.OK, MessageBoxImage.Information);
        }



    }
}
