using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;

namespace TestGeneratorWPF
{
    public enum CorrectAnswer
    {
        Option1,
        Option2,
        Option3
    }

    [Serializable]
    public class TestModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public CorrectAnswer CorrectAnswer { get; set; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EditTestButton_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            if (password == "your_password_here")
            {
                AuthorizedWindow authorizedWindow = new AuthorizedWindow();
                authorizedWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect password.");
            }
        }

        private void PassTestButton_Click(object sender, RoutedEventArgs e)
        {
            TestPassingPage testPassingPage = new TestPassingPage();
            testPassingPage.Show();
            this.Close();
        }
    }

    public partial class AuthorizedWindow : Window
    {
        private List<TestModel> tests = new List<TestModel>();

        public AuthorizedWindow()
        {
            InitializeComponent();
            LoadTests();
        }

        private void LoadTests()
        {
            if (File.Exists("tests.dat"))
            {
                using (FileStream fs = new FileStream("tests.dat", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    tests = (List<TestModel>)bf.Deserialize(fs);
                }
            }
        }

        private void SaveTests()
        {
            using (FileStream fs = new FileStream("tests.dat", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tests);
            }
        }

        private void AddTestButton_Click(object sender, RoutedEventArgs e)
        {
            tests.Add(new TestModel());
            TestEditorFrame.Navigate(new TestEditorPage(tests[tests.Count - 1]));
        }

        private void TestEditorFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            SaveTests();
        }
    }

    public partial class TestPassingPage : Page
    {
        private List<TestModel> tests = new List<TestModel>();
        private int currentTestIndex = 0;
        private int correctAnswers = 0;

        public TestPassingPage()
        {
            InitializeComponent();
            LoadTests();
            if (tests.Count == 0)
            {
                NoTestLabel.Visibility = Visibility.Visible;
                PassTestButton.IsEnabled = false;
            }
            else
            {
                DisplayTest();
            }
        }

        private void LoadTests()
        {
            if (File.Exists("tests.dat"))
            {
                using (FileStream fs = new FileStream("tests.dat", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    tests = (List<TestModel>)bf.Deserialize(fs);
                }
            }
        }

        private void DisplayTest()
        {
            TestModel currentTest = tests[currentTestIndex];
            TitleLabel.Content = currentTest.Title;
            DescriptionTextBlock.Text = currentTest.Description;
            Option1RadioButton.Content = currentTest.Option1;
            Option2RadioButton.Content = currentTest.Option2;
            Option3RadioButton.Content = currentTest.Option3;
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Option1RadioButton.IsChecked == true && tests[currentTestIndex].CorrectAnswer == CorrectAnswer.Option1 ||
                Option2RadioButton.IsChecked == true && tests[currentTestIndex].CorrectAnswer == CorrectAnswer.Option2 ||
                Option3RadioButton.IsChecked == true && tests[currentTestIndex].CorrectAnswer == CorrectAnswer.Option3)
            {
                correctAnswers++;
            }

            currentTestIndex++;
            if (currentTestIndex < tests.Count)
            {
                DisplayTest();
            }
            else
            {
                MessageBox.Show($"You answered {correctAnswers} out of {tests.Count} questions correctly.");
                NavigationService.Navigate(new Uri("MainWindow.xaml", UriKind.Relative));
            }
        }
    }

    public partial class TestEditorPage : Page
    {
        private TestModel test;

        public TestEditorPage(TestModel test)
        {
            InitializeComponent();
            this.test = test;
            DataContext = test;
        }
    }
}
