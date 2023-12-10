using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Kursa_Darbs_WBF_ES_DVDZ1
{
    public partial class MainWindow : Window
    {
        private readonly List<int> correctRecipeOrder = new List<int> { 1, 2 };
        private List<BurgerIngredient> ingredients = new List<BurgerIngredient>();
        private BurgerIngredient selectedIngredient;
        private Point offset;

        private Dictionary<string, int> ingredientNumbers = new Dictionary<string, int>
        {
            { "Auksa Maizes", 1 },
            { "Gala", 2 },
            { "Gurki", 3 },
            { "Kecups", 4 },
            { "Mahoneze", 5 },
            { "Maizes Apaksa", 6 },
            { "Salat Lapa", 7 },
            { "Siers", 8 },
            { "Sinepes", 9 },
            { "Sipoli", 10 },
            { "Tomati", 11 }
        };

        public MainWindow()
        {
            InitializeComponent();
            SetUpApp();

            foreach (var ingredient in ingredients)
            {
                ingredient.Image.MouseDown += IngredientImage_MouseDown;
            }

        }


        private void IngredientImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image ingredientImage = sender as Image;

            // Check if the sender is a valid image element
            if (ingredientImage != null)
            {
                selectedIngredient = ingredients.FirstOrDefault(ing => ing.Image == ingredientImage);

                if (selectedIngredient != null)
                {
                    Point mousePosition = e.GetPosition(burgerCanvas);
                    offset = new Point(mousePosition.X - Canvas.GetLeft(ingredientImage), mousePosition.Y - Canvas.GetTop(ingredientImage));
                    ingredientImage.Opacity = 0.7;
                    Panel.SetZIndex(ingredientImage, 1); // Bring to foreground
                }
            }
        }




        private void SetUpApp()
        {
            string imageDirectoryPath = @"C:\Users\nazis\Desktop\atteli";
            string[] imagePaths = Directory.GetFiles(imageDirectoryPath, "*.png");
            foreach (string imagePath in imagePaths)
            {
                string imageName = Path.GetFileNameWithoutExtension(imagePath);
                int ingredientNumber = GetIngredientNumber(imageName);

                BurgerIngredient ingredient = new BurgerIngredient(imagePath, 0, 0, ingredientNumber);
                ingredients.Add(ingredient);
                Image ingredientImage = ingredient.Image; // Change this line

                ingredientImage.MouseDown += IngredientImage_MouseDown;

                ingredientCanvas.Children.Add(ingredientImage);
            }



            StackIngredients();
        }

        private void StackIngredients()
        {
            double currentYPosition = 10; // Start a little down from the top of the canvas
            double xPosition = 10; // Initial X position
            double yIncrement = 30; // Y position increment for each ingredient

            foreach (var ingredient in ingredients)
            {
                Canvas.SetTop(ingredient.Image, currentYPosition);
                Canvas.SetLeft(ingredient.Image, xPosition);

                // Update position for next ingredient
                currentYPosition += yIncrement;

                // Change xPosition if you want to stagger the ingredients horizontally as well
                // For example:
                // xPosition = (xPosition == 10) ? 30 : 10;
            }
        }
        private void BurgerCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(burgerCanvas);

            foreach (BurgerIngredient ingredient in ingredients)
            {
                // Adjust to use the actual bounds of the image
                Rect ingredientBounds = new Rect(Canvas.GetLeft(ingredient.Image), Canvas.GetTop(ingredient.Image), ingredient.Image.Width, ingredient.Image.Height);

                if (ingredientBounds.Contains(mousePosition))
                {
                    selectedIngredient = ingredient;
                    offset = new Point(mousePosition.X - Canvas.GetLeft(ingredient.Image), mousePosition.Y - Canvas.GetTop(ingredient.Image));
                    ingredient.Image.Opacity = 0.7;
                    Panel.SetZIndex(ingredient.Image, 1); // Bring to foreground
                    break;
                }
            }
        }

        // This will be called after dragging the ingredient to set it in place.
        private void DropIngredient(BurgerIngredient ingredient, Point dropPosition)
        {
            // Determine which rectangle the ingredient was dropped in
            // This logic needs to be adjusted based on your actual rectangle positions and sizes
            int index = (int)((dropPosition.Y - 50) / 30); // Example logic
            double newYPosition = 50 + (index * 30); // Y position of the target rectangle

            // Snap the ingredient to the rectangle position
            Canvas.SetTop(ingredient.Image, newYPosition);
            Canvas.SetLeft(ingredient.Image, 200); // X position inside the burgerCanvas, adjust as needed

            // Check if the ingredient is placed in the correct order
            if (index < correctRecipeOrder.Count && ingredient.IngredientNumber == correctRecipeOrder[index])
            {
                // Correct order
                MessageBox.Show($"Correctly placed {ingredientNumbers.FirstOrDefault(x => x.Value == ingredient.IngredientNumber).Key}");
            }
            else
            {
                // Incorrect order, maybe reset the position or give a message
                MessageBox.Show("Incorrect order, try again!");
            }
        }



        private void BurgerCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedIngredient != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePosition = e.GetPosition(burgerCanvas);
                Point newPosition = new Point(mousePosition.X - offset.X, mousePosition.Y - offset.Y);

                Canvas.SetLeft(selectedIngredient.Image, newPosition.X);
                Canvas.SetTop(selectedIngredient.Image, newPosition.Y);
            }
        }

        // Peles atlaiduma notikuma apstrāde
        private void BurgerCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedIngredient != null)
            {
                Point mousePosition = e.GetPosition(burgerCanvas);

                DropIngredient(selectedIngredient, mousePosition);

                selectedIngredient.Image.Opacity = 1.0;
                Panel.SetZIndex(selectedIngredient.Image, 0); // Reset Z-index
                selectedIngredient = null;
            }
        }

        // Pārbaudīt receptes secību
        private void CheckRecipeOrder()
            {
                for (int i = 0; i < Math.Min(ingredients.Count, correctRecipeOrder.Count); i++)
                {
                    if (ingredients[i].IngredientNumber != correctRecipeOrder[i])
                    {
                        MessageBox.Show("Nepareiza receptes secība!", "Kļūda", MessageBoxButton.OK, MessageBoxImage.Error);
                        PlaySound(@"mixkit-arcade-mechanical-bling-210");
                        return;

                    }
                }
                MessageBox.Show("Pareiza receptes secība!", "Veiksmīgi", MessageBoxButton.OK, MessageBoxImage.Information);
                PlaySound(@"mixkit-arcade-mechanical-bling-210"); // Replace "successSound" with the actual sound file name (without extension)
            }

            // McDonald's izvēlnes poga
            private void McDonaldsMenu_Click(object sender, RoutedEventArgs e)
            {
                PlaySound("McDonalds Logo Sound effects (HD).wav");
                IngredientsWindow ingredientsWindow = new IngredientsWindow();
                ingredientsWindow.OnWindowClosed += OpenTimeWindow;
                this.Hide();
                ingredientsWindow.Show();
            }

            // Pabeigt burgera pogas notikums
            private void CompleteBurger_Click(object sender, RoutedEventArgs e)
            {

                PlaySound("Mouse Click Sound Effect.wav");
                CheckRecipeOrder();
            }

       
            // Sastāvdaļas pogas notikums
            private void IngredientButton_Click(object sender, RoutedEventArgs e)
            {

                PlaySound("whoosh sound effect.wav");
                var button = sender as Button;
                if (button != null)
                {
                    string ingredientType = button.Content.ToString();
                    string imagePath = GetImagePathForIngredient(ingredientType);
                    int ingredientNumber = GetIngredientNumber(ingredientType);

                    if (ingredientNumber != -1)
                    {
                        BurgerIngredient newIngredient = new BurgerIngredient(imagePath, 0, 0, ingredientNumber);
                        ingredients.Add(newIngredient);
                        burgerCanvas.Children.Add(newIngredient.Image);
                        StackIngredients();
                    }
                }
            }

            // Atgriezt attēla ceļu sastāvdaļai
            private string GetImagePathForIngredient(string ingredientType)
            {
                string imageDirectoryPath = @"C:\Users\nazis\Desktop\atteli";
                string imagePath = Path.Combine(imageDirectoryPath, ingredientType + ".png");
                if (File.Exists(imagePath))
                {
                    return imagePath;
                }
                return null;
            }

            // Atgriezt sastāvdaļas numuru
            private int GetIngredientNumber(string ingredientType)
            {
                if (ingredientNumbers.TryGetValue(ingredientType, out int number))
                {
                    return number;
                }
                return -1;
            }

        // Laika loga atvēršana
        private void GoToTime_Click(object sender, RoutedEventArgs e)
        {
            PlaySound("yourSoundFileName.wav");  // Replace with the actual sound file name
                                                 // Rest of your existing code...
            timeMainWindow timeWindow = new timeMainWindow();
            timeWindow.Show();
            this.Close();
        }



        // Laika loga atvēršana pēc citu logu aizvēršanas
        private void OpenTimeWindow()
            {
                timeMainWindow timeWindow = new timeMainWindow();
                timeWindow.Show();
                this.Close();
            }


            // Skaņas atskaņošana
            private void PlaySound(string soundName)
            {
                string soundPath = Path.Combine(@"C:\Users\nazis\Desktop\skana", soundName);
                if (File.Exists(soundPath))
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(soundPath);
                    player.Play();
                }
            }

            private void ASomeButton_Click(object sender, RoutedEventArgs e)
            {
                PlaySound("Mouse Click Sound Effect.wav");
            }

        }
    }






