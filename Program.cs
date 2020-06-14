using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quizApp
{
    class Program
    {
        static void Main(string[] args)
        {
            QuizUi quiz = new QuizUi();
            quiz.Start();
            quiz.Stop();
            Environment.Exit(0);
        }
    }
}
