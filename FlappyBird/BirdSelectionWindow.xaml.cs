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

namespace FlappyBird    
{
   public partial class BirdSelectionWindow : Window
    {
        private string selectedBirdPath = "Resources/bird1.png";

        public BirdSelectionWindow()
        {
            InitializeComponent();
        }

        private void Bird_Click(object sender, MouseButtonEventArgs e)
        {
            Bird1Border.BorderBrush = Brushes.Transparent;
            Bird2Border.BorderBrush = Brushes.Transparent;
            Bird3Border.BorderBrush = Brushes.Transparent;
            Bird4Border.BorderBrush = Brushes.Transparent;
            
            if (sender == Bird1)
            {
                Bird1Border.BorderBrush = Brushes.Blue;
                selectedBirdPath = "Resources/bird.png";
            }
            else if (sender == Bird2)
            {
                Bird2Border.BorderBrush = Brushes.Blue;
                selectedBirdPath = "Resources/bird2.png";
            }
            else if (sender == Bird3)
            {
                Bird3Border.BorderBrush = Brushes.Blue;
                selectedBirdPath = "Resources/bird3.png";
            }
            else if (sender == Bird4)
            {
                Bird3Border.BorderBrush = Brushes.Blue;
                selectedBirdPath = "Resources/bird4.png";
            }
            
        }

        private void ConfirmSelection_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new GameWindow(selectedBirdPath);
            gameWindow.Show();
            this.Close();
        }
    }
}
