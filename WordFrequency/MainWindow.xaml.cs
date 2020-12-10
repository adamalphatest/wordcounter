using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordFrequency
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

        private void Ofd_Button_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog 
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            // Filter for text files only 
            dialog.DefaultExt = ".txt";

            // Show OFD
            Nullable<bool> result = dialog.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Get the filename of the file selected via the OFD
                string filename = dialog.FileName;

                int totalUnique;

                List<Unique> words = getOrderedWordList(filename, out totalUnique);

                // Set the datagrid item source to display in UI
                dgWords.ItemsSource = words;
                total_words_box.Text = $"Total unique words: \n {totalUnique.ToString()}";
            }
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            dgWords.ItemsSource = null;
            total_words_box.Text = "Total unique words:";
        }

        private List<Unique> getOrderedWordList(string file, out int totalUnique)
        {
            var fileContents = File.ReadAllText(file);
            var wordRegex = new Regex(@"\w+");

            MatchCollection words = wordRegex.Matches(fileContents);

            // Cast regex MatchCollection object to a list of strings
            var list = words.Cast<Match>().Select(x => x.Value).ToList();

            // Group by unique words + take top 10 ordered by frequency count desc
            var uniques = list
            .GroupBy(x => x)
            .Select(group => new Unique() { Word = group.Key, Count = group.Count() })
            .OrderByDescending(o => o.Count);

            totalUnique = uniques.Count();

            var topUniques = uniques.Take(10).ToList();

            // Set the Rank prop based on index + 1
            foreach (Unique item in topUniques)
            {
                int itemIndex = topUniques.FindIndex(x => x.Word == item.Word);
                item.Rank = itemIndex + 1;
            }

            return topUniques;
        }

    }


}
