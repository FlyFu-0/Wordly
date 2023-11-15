using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wordly
{
    public partial class WordsFinder : Form
    {
        private List<string> words;

        public WordsFinder()
        {
            InitializeComponent();

        }

        private void ButtonLetter_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender; 
            string letter = button.Text;

            //if(textWordBox.Text.Length < textWordBox.MaxLength) {
                textWordBox.Text += letter; 
            //}
        }

        private void buttonBackspace_Click()
        {
            if(textWordBox.Text.Length > 0)
            {
                textWordBox.Text = textWordBox.Text.Substring(0, textWordBox.Text.Length - 1);
            }
        }

        private void getLengthOfCurrentWord()
        {
            if (lengthOfWord.SelectedValue != null)
            {
                string maxLengthOfWord = lengthOfWord.SelectedValue.ToString();
                //textWordBox.MaxLength = int.Parse(maxLengthOfWord);
            }
        }

        private static List<string> loadDataFromFile()
        {
            string filePath = "russian_nouns_with_correct_words.txt";
            List<string> words = new List<string>();

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    words.Add(line);
                }
            }

            return words;
        }

        private async void proccessingFileWords(List<string> words)
        {
            string filepath = "russian_nouns_with_correct_words.txt";
            using (StreamWriter writer = new StreamWriter(filepath, false, System.Text.Encoding.UTF8))
            {
                foreach(var word in words)
                {
                    if(word.Length >= 4 && word.Length <= 11)
                    {
                        await writer.WriteLineAsync(word);
                    }
                }
            }
            MessageBox.Show(words.Count.ToString());
        }

        private bool isExcludedWord(string word)
        {
            string exLetters = exLettersText.Text;

            foreach(var c in exLetters)
            {
                if(word.Contains(c))
                    return false;
            }
            return true;
        }

        private Regex wordToRegex(ref string letterContain)
        {
            string pattern = "^";
            string word = textWordBox.Text;

            for(int i = 0; i < word.Length; i++)
            {
                if (word[i] == '-')
                {
                    pattern += "[а-я]";
                    continue;
                }
                else if (word[i] == '(')
                {
                    pattern += $"[^{word[i + 1]}]";
                    letterContain += word[i + 1];
                    i += 2;
                    continue;
                }

                pattern += word[i];
            }
            //MessageBox.Show(pattern);

            pattern += '$';
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex;
        }

        private bool isLetterContains(string word, string letters)
        {
            foreach(var c in letters)
            {
                if(!word.Contains(c))
                    return false;
            }
            return true;
        }

        private void findMatchingWords()
        {
            string letterContain = "";
            Regex regex = wordToRegex(ref letterContain);
            List<string> matchingWords = words.Where(word => regex.IsMatch(word) && isLetterContains(word, letterContain)).ToList();

            dataGridView1.Rows.Clear();

            foreach (string word in matchingWords)
            {
                if(isExcludedWord(word))
                    dataGridView1.Rows.Add(word);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            words = loadDataFromFile();
            //MessageBox.Show(words.Count.ToString());

            dataGridView1.Columns.Add("WordColumn", "Word");
            dataGridView1.Columns["WordColumn"].ValueType = typeof(string);

            lengthOfWord.SelectedItem = "4";

            foreach (Control control in alphabetLayout.Controls)
            {
                if (control is Button)
                {
                    Button button = (Button)control;
                    button.Click += new EventHandler(ButtonLetter_Click);
                }
            }

        }

        private void backspaceBtn_Click(object sender, EventArgs e)
        {
            buttonBackspace_Click();
        }

        private void lengthOfWord_SelectedValueChanged(object sender, EventArgs e)
        {
            getLengthOfCurrentWord();
        }

        private void findBtn_Click(object sender, EventArgs e)
        {
            findMatchingWords();
        }

        private void textWordBox_TextChanged(object sender, EventArgs e)
        {
            textLengthOfInput.Text = textWordBox.Text.Length.ToString();
        }
    }
}
