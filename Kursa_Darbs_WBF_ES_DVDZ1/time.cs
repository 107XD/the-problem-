using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Kursa_Darbs_WBF_ES_DVDZ1
{
    public partial class timeMainWindow : Window
    {
        private Point lastMousePosition;
        private int score;
        private DispatcherTimer timer;
        private Random random = new Random();
        private int gameDuration = 10;
        private int elapsedTime = 0;

        public timeMainWindow()
        {
            Cub_Image();
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset score and elapsed time, then start the timer
            score = 0;
            elapsedTime = 0;
            timer.Start();
            MoveObjectRandomly(movingRectangle);
            UpdateScore();

            // Play sound on start
            PlaySound("mixkit-arcade-mechanical-bling-210.wav");
        }




        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            timerTextBlock.Text = "00:" + (gameDuration - elapsedTime).ToString("00");

            if (elapsedTime >= gameDuration)
            {
                timer.Stop();
                PlaySound("Simple game countdown - Sound Effect.wav"); // Play sound when time is up
                MessageBox.Show("Laiks beidzies");
                MessageBox.Show("Jūsu rezultāts ir " + score + " Vēlaties spelēt velreiz?");

                // Ask the user if they want to play again
                if (MessageBox.Show("Vēlaties spelēt velreiz?", "Spēle", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ResetGame();
                }
                else
                {
                    this.Close();
                }
            }
        }

        //Poga Play Again
        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseOverObject(movingRectangle))
            {
                MoveObjectRandomly(movingRectangle);
                score += CalculatePoints();
                UpdateScore();
            }
        }

        private int CalculatePoints()
        {
            // Example scoring logic
            return Math.Max(10 - (elapsedTime / 3), 1);
        }

        private void UpdateScore()
        {
            Title = "Score: " + score;
        }

        private void MoveObjectRandomly(UIElement element)
        {
            double newX = random.NextDouble() * (canvas.ActualWidth - element.RenderSize.Width);
            double newY = random.NextDouble() * (canvas.ActualHeight - element.RenderSize.Height);

            Canvas.SetLeft(element, newX);
            Canvas.SetTop(element, newY);
        }

        // Reset the game to its initial state
        private void ResetGame()
        {
            score = 0;
            elapsedTime = 0;
            timer.Start();
            MoveObjectRandomly(movingRectangle);
            UpdateScore();
            PlaySound("mixkit-arcade-mechanical-bling-210.wav"); // Play sound on reset
        }

        // Play a sound from the specified file
        private void PlaySound(string soundFileName)
        {
            string soundPath = System.IO.Path.Combine(@"C:\Users\nazis\Desktop\skana", soundFileName);
            if (System.IO.File.Exists(soundPath))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(soundPath);
                player.Play();
            }
        }


        private bool IsMouseOverObject(UIElement element)
        {
            Point mousePosition = Mouse.GetPosition(element);
            return new Rect(0, 0, element.RenderSize.Width, element.RenderSize.Height).Contains(mousePosition);
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastMousePosition = e.GetPosition(canvas);
            Canvas.SetLeft(movingRectangle, lastMousePosition.X - movingRectangle.Width / 2);
            Canvas.SetTop(movingRectangle, lastMousePosition.Y - movingRectangle.Height / 2);
            
           
        }


        private void Cub_Image()
        {
            try
            {
                // Apply the ImageBrush to the Fill property of the Rectangle
                movingRectangle.Fill = Bildes_uzstadisana(@"C:\Users\nazis\Desktop\Stack - Burger - Toppings - 1.jpg");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public ImageBrush Bildes_uzstadisana(string links)
        {
            ImageBrush img = new ImageBrush(new BitmapImage(new Uri(links, UriKind.Absolute)));
            return img;
        }

    }
}