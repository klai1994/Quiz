using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;

namespace Quiz
{
    public partial class Form1 : Form
    {
        QuestionData[] allQuestions;
        int currentQuestion;
        int CurrentQuestion
        {
            get
            {
                return currentQuestion;
            }
            set
            {
                currentQuestion = value;
                label1.Text = "#" + (currentQuestion + 1) + "\n" + allQuestions[value].question;
            }
        }

        int totalQuestions = 0;
        int correctAnswers = 0;

        public Form1()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Wrong by default unless correct answer is entered
            label2.Text = "Incorrect. Right answer:\n" + allQuestions[currentQuestion].GetAnswers();
            foreach (string s in allQuestions[currentQuestion].answers)
            {
                // 'right' is a cheat to always answer correctly
                if (textBox1.Text == s || textBox1.Text == "right" && CurrentQuestion <= totalQuestions)
                {
                    label2.Text = "Correct!";
                    correctAnswers++;
                    break;
                }
            }

            PrintScore();
            // If there are still questions, go to the next one. Otherwise, complete quiz and calculate score.
            if (CurrentQuestion < totalQuestions)
            {
                CurrentQuestion++;
            }
            else
            {
                label2.Text = "Grade: % " + 
                    ((float)correctAnswers / (totalQuestions + 1) * 100).ToString("0.00") + "\nKeep going!";
                textBox1.Enabled = false;
                button1.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Confirm with user to start a new quiz
            DialogResult userResponse = MessageBox.Show("Start new quiz?", "Confirm", MessageBoxButtons.YesNo);
            if (userResponse == DialogResult.No)
            {
                return;
            }

            /* Collects the questions and answers from a text file and inputs them into a struct. 
               Each line is broken down into a question and then one or more answers which is placed into 
               different variables in the struct */
            string path = Directory.GetCurrentDirectory() + "\\questions.txt";
            string[] questions = File.ReadAllLines(path);
            allQuestions = new QuestionData[questions.Length];
            int numQuestions = questions.Length - 1;

            for (int i = 0; i < numQuestions + 1; i++)
            {
                string[] question = questions[i].Trim().Split(' ');
                allQuestions[i].question = question[0];

                string[] acceptableAnswers = question[1].Trim().Split('/');
                allQuestions[i].answers = acceptableAnswers;
            }

            // Shuffle the questions
            Random rand = new Random();
            for (int i = 0; i < numQuestions; i++)
            {
                int r = rand.Next(i, numQuestions);
                QuestionData temp = allQuestions[i];
                allQuestions[i] = allQuestions[r];
                allQuestions[r] = temp;
            }

            // Resets the score and initializes the quiz
            totalQuestions = numQuestions;
            correctAnswers = 0;
            CurrentQuestion = 0;

            PrintScore();
            textBox1.Enabled = true;
            button1.Enabled = true;

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)ConsoleKey.Enter && button1.Enabled)
            {
                e.Handled = true;
                button1_Click(null, null);
            }
        }

        private void PrintScore()
        {
            label3.Text = "Score: " + correctAnswers + '/' + (totalQuestions + 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 'Help' button
            MessageBox.Show("To start a quiz, place the file \"questions.txt\" " 
                + "into the same folder as the program executable. The file must be formatted like so:"
                + "\n\n• Each question and answer(s) should be on a separate line."
                + "\n\n• Separate the question from the answer with one space."
                + "\n\n• For questions with multiple correct answers, place one forward slash \"/\" between the answers."
                + "\n\n• Avoid blank lines, and use an English keyboard for the spaces and slashes.",
                "Help");
        }
    }

    struct QuestionData
    {
        public string question;
        public string[] answers;

        public string GetAnswers()
        {
            string answerList = answers[0];

            if (answers.Length > 1)
            {
                for (int i = 1; i < answers.Length; i++)
                {
                    answerList = answerList + " or " + answers[i];
                }
            }
            return answerList;
        }

    }
}
