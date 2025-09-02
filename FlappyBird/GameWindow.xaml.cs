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
using System.Windows.Threading;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.IconPacks;


namespace FlappyBird
{

    public partial class GameWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        double gravity = 0.3;
        double velocity = 0;
        int score = 0;
        DateTime startTime;
        List<UIElement> pipeList = new List<UIElement>();
        List<UIElement> scoredPipes = new List<UIElement>();

        //zvukvi
        SoundPlayer jumpSound = new SoundPlayer("Resources/jump1.wav");
        SoundPlayer hitSound = new SoundPlayer("Resources/hit1.wav");
        SoundPlayer pointSound = new SoundPlayer("Resources/point1.wav");

        private int gameElapsedSeconds = 0;
        private bool isDay = false;
        private double obstacleSpeed = 3; // Početna brzina
        DispatcherTimer themeTimer = new DispatcherTimer();


        UIElement exists;
        int highScore = 0;
        List<int> topScores = new List<int>();
        string topScoresFilePath = "topscores.txt";
            
        Random rand = new Random();
        bool gameOver = false;
        bool gameStarted = false;

        private double baseSpeed = 3.0;
        private double maxSpeed = 7.5;

        private GradientStop backgroundStop1;
        private GradientStop backgroundStop2;
        private string birdImagePath = "Resources/bird1.png";

        private double initialGap = 150;
        private int difficultyStep = 10; 
        private int lastDifficultyIncrease = 0;


        public GameWindow()
        {
            InitializeComponent();
            ToggleTheme();
            Bird.Source = new BitmapImage(new Uri(birdImagePath, UriKind.Relative));

            if (GameCanvas.Background is LinearGradientBrush brush)
            {
                backgroundStop1 = brush.GradientStops[0];
                backgroundStop2 = brush.GradientStops[1];
            }


            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;

            themeTimer.Interval = TimeSpan.FromSeconds(10);
            themeTimer.Tick += ThemeTimer_Tick;
            StartGame();

        }
        public GameWindow(string selectedBirdPath)
        {
            InitializeComponent();
            birdImagePath = selectedBirdPath;

            ToggleTheme(); 

            Bird.Source = new BitmapImage(new Uri(birdImagePath, UriKind.Relative)); // postavlja sliku ptice

            if (GameCanvas.Background is LinearGradientBrush brush)
            {
                backgroundStop1 = brush.GradientStops[0];
                backgroundStop2 = brush.GradientStops[1];
            }

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;

            themeTimer.Interval = TimeSpan.FromSeconds(30); 
            themeTimer.Tick += ThemeTimer_Tick;

            //StartGame();
        }


        private void StartGame()
        {
            velocity = 0;
            gameOver = false;
            Canvas.SetTop(Bird, 200);
            score = 0;
            startTime = DateTime.Now;

            themeTimer.Stop(); 
            themeTimer.Start();
            

            foreach (var pipe in pipeList)
            {
                GameCanvas.Children.Remove(pipe);
            }
            pipeList.Clear();

            scoredPipes.Clear(); 

            AddPipe(initialX: 1000);
            AddPipe(initialX: 1300);

            IncreaseDifficulty();
            gameTimer.Start();

        }

        private void IncreaseDifficulty()
        {
            if (score > 0 && score % difficultyStep == 0 && score != lastDifficultyIncrease)
            {
                obstacleSpeed += 0.5;
                initialGap = Math.Max(80, initialGap - 10); 
                lastDifficultyIncrease = score;

                System.Diagnostics.Debug.WriteLine($"📈 Težina porasla! Brzina: {obstacleSpeed}, Razmak: {initialGap}");
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var cityStoryboard = (Storyboard)this.Resources["CityGlowStoryboard"];
            cityStoryboard?.Begin();
        }


        private void GameLoop(object sender, EventArgs e)
        {
            velocity += gravity;
            double birdY = Canvas.GetTop(Bird);
            Canvas.SetTop(Bird, birdY + velocity);

            TimeSpan elapsed = DateTime.Now - startTime;
            ScoreText.Text = score.ToString();
            TimeText.Text = elapsed.ToString(@"mm\:ss");

            double elapsedSeconds = elapsed.TotalSeconds;
            double currentSpeed = baseSpeed + Math.Min(elapsedSeconds / 15.0 * 0.5, maxSpeed - baseSpeed);


            for (int i = 0; i < pipeList.Count; i++)
            {
                FrameworkElement pipe = (FrameworkElement)pipeList[i];
                double left = Canvas.GetLeft(pipe);
                Canvas.SetLeft(pipe, left - currentSpeed);

                
                Rect birdHitBox = new Rect(Canvas.GetLeft(Bird), Canvas.GetTop(Bird), Bird.Width, Bird.Height);
                Rect pipeHitBox = new Rect(Canvas.GetLeft(pipe), Canvas.GetTop(pipe), pipe.Width, pipe.Height);

                if (birdHitBox.IntersectsWith(pipeHitBox))
                {
                    EndGame();
                    return;
                }

                if (left + pipe.Width < 0)
                {
                    GameCanvas.Children.Remove(pipe);
                    pipeList.Remove(pipe);
                    i--;
                }

                if (!scoredPipes.Contains(pipe)
                    && Canvas.GetLeft(pipe) + pipe.Width < Canvas.GetLeft(Bird)
                    && Canvas.GetTop(pipe) < 5) 
                {
                    if (MainWindow.IsSoundOn)
                        pointSound.Play();

                    score++;
                    scoredPipes.Add(pipe);
                    IncreaseDifficulty();
                }

            }

            if (pipeList.Count < 6)
            {
                AddPipe();
            }

            if (Canvas.GetTop(Bird) + Bird.Height > Canvas.GetTop(Ground))
            {
                EndGame();
                return;
            }

            if (Canvas.GetTop(Bird) < 0)
            {
                EndGame();
                return;
            }

            ScoreText.Text = score.ToString();
            TimeText.Text = elapsed.ToString(@"mm\:ss");
        }




        private void AddPipe(double initialX = 800)
        {
            double gap = initialGap;
            
            double pipeWidth = 80;
            double minHeight = 80;
            double maxHeight = 450 - gap - 20; 

            double pipeHeight = rand.Next((int)minHeight, (int)maxHeight);
            double topPipeHeight = pipeHeight;
            double bottomPipeY = pipeHeight + gap;
            double bottomPipeHeight = 450 - bottomPipeY;


            var topPipe = new Border
            {
                Width = pipeWidth,
                Height = topPipeHeight,
                Background = new SolidColorBrush(Color.FromRgb(34, 139, 34)),
                CornerRadius = new CornerRadius(5),
                BorderBrush = Brushes.DarkOliveGreen,
                BorderThickness = new Thickness(2),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 5,
                    ShadowDepth = 2
                }
            };

            Canvas.SetLeft(topPipe, initialX);
            Canvas.SetTop(topPipe, 0);
            GameCanvas.Children.Add(topPipe);
            pipeList.Add(topPipe);

            // Donja cijev
            var bottomPipe = new Border
            {
                Width = pipeWidth,
                Height = bottomPipeHeight,
                Background = new SolidColorBrush(Color.FromRgb(34, 139, 34)),
                CornerRadius = new CornerRadius(5),
                BorderBrush = Brushes.DarkOliveGreen,
                BorderThickness = new Thickness(2),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 5,
                    ShadowDepth = 2
                }
            };

            Canvas.SetLeft(bottomPipe, initialX);
            Canvas.SetTop(bottomPipe, bottomPipeY);
            GameCanvas.Children.Add(bottomPipe);
            pipeList.Add(bottomPipe);
        }


        private void EndGame()
        {
            if (MainWindow.IsSoundOn)
                hitSound.Play();

            gameTimer.Stop();
            gameOver = true;
            themeTimer.Stop();

            if (File.Exists("highscore.txt"))
            {
                string content = File.ReadAllText("highscore.txt");
                int.TryParse(content, out highScore);
            }

            if (score > highScore)
            {
                highScore = score;
                File.WriteAllText("highscore.txt", highScore.ToString());
            }

            SaveTopScores();
            SaveScore(score);

            FinalScoreText.Text = $"Tvoj rezultat: {score} \nNajbolji: {highScore}";
            GameOverCard.Visibility = Visibility.Visible;
        }


        private void SaveScore(int score)
        {
            try
            {
                string filePath = "topscores.txt";

                if (!System.IO.File.Exists(filePath))
                    System.IO.File.Create(filePath).Close();

                using (var writer = System.IO.File.AppendText(filePath))
                {
                    writer.WriteLine(score);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri čuvanju rezultata: {ex.Message}");
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!gameStarted)
                {
                    StartGame();
                    gameStarted = true;
                }
                else if (gameOver)
                {
                    StartGame();
                }
                else
                {
                    velocity = -5;
                    if (MainWindow.IsSoundOn)
                        jumpSound.Play();
                }
            }
        }


        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            GameOverCard.Visibility = Visibility.Collapsed;
            StartGame();
        }

        private void ToggleTheme()
        {
            isDay = !isDay;

            Color toColor1 = isDay ? Color.FromRgb(137, 207, 240) : Color.FromRgb(10, 10, 50);
            Color toColor2 = isDay ? Color.FromRgb(176, 224, 230) : Color.FromRgb(30, 30, 80);

            if (backgroundStop1 != null && backgroundStop2 != null)
            {
                backgroundStop1.BeginAnimation(GradientStop.ColorProperty, new ColorAnimation
                {
                    To = toColor1,
                    Duration = TimeSpan.FromSeconds(2)
                });

                backgroundStop2.BeginAnimation(GradientStop.ColorProperty, new ColorAnimation
                {
                    To = toColor2,
                    Duration = TimeSpan.FromSeconds(2)
                });

                System.Diagnostics.Debug.WriteLine($"✔ ToggleTheme: {(isDay ? "Dan" : "Noć")} @ {DateTime.Now:T}");
                
                Sun.Visibility = !isDay ? Visibility.Collapsed : Visibility.Visible;

                Star1.Visibility = isDay ? Visibility.Collapsed : Visibility.Visible;
                Star2.Visibility = isDay ? Visibility.Collapsed : Visibility.Visible;
                Star3.Visibility = isDay ? Visibility.Collapsed : Visibility.Visible;
                Star4.Visibility = isDay ? Visibility.Collapsed : Visibility.Visible;

                Cloud1.Visibility = !isDay ? Visibility.Collapsed : Visibility.Visible;
                Cloud2.Visibility = !isDay ? Visibility.Collapsed : Visibility.Visible;
                Cloud3.Visibility = !isDay ? Visibility.Collapsed : Visibility.Visible;

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠ backgroundStop1 ili backgroundStop2 je null!");
            }


        }

        private void ThemeTimer_Tick(object sender, EventArgs e)
        {
            ToggleTheme();
        }

        private void BackToStart_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); 
        }

        private void LoadTopScores()
        {
            if (File.Exists(topScoresFilePath))
            {
                var lines = File.ReadAllLines(topScoresFilePath);
                topScores = lines.Select(l => int.TryParse(l, out var s) ? s : 0).ToList();
            }
            else
            {
                topScores = new List<int>();
            }
        }

        private void SaveTopScores()
        {
            topScores.Add(score);
            File.AppendAllText(topScoresFilePath, score + Environment.NewLine);

            topScores = File.ReadAllLines(topScoresFilePath)
                            .Select(line => int.TryParse(line, out int s) ? s : 0)
                            .OrderByDescending(s => s)
                            .ToList();

        }


    }
}   
