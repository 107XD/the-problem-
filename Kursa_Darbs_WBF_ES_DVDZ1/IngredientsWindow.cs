using System;
using System.Windows;
using System.Windows.Controls;

namespace Kursa_Darbs_WBF_ES_DVDZ1
{
    public partial class IngredientsWindow : Window
    {

        public event Action OnWindowClosed;
        public IngredientsWindow()
        {
            InitializeComponent();
        }

        // Event handler for ingredient buttons
        private void IngredientButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string ingredientName = button.Content.ToString();
                // Implement the logic for when an ingredient is selected.
                
                MessageBox.Show($"Selected ingredient: {ingredientName}");
            }
        }

        // Event handler for the Complete Burger button
        private void CompleteBurger_Click(object sender, RoutedEventArgs e)
        {
            // Implement the logic for completing the burger.
       
            MessageBox.Show("Burger completed!");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            OnWindowClosed?.Invoke();
        }
    }
}