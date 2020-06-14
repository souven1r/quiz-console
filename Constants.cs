using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quizApp
{
    /// <summary>
    /// Class which contains global constants;
    /// </summary>
    public static class Constants
    {
        public enum UserType { User, Admin };
        public const string USERS_BASE_PATH = "../../database/users.xml";
        public const string GAME_BASE_PATH = "../../database/gameData.xml";
        public const int ANSWERS_COUNT = 4;
        public const int MAX_GRADE = 12;
    }
    public class Answer
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
    public class Question
    {
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
        public Question() : this("Без названия")
        { }
        public Question(string name)
        {
            Text = name;
            Answers = new List<Answer>();
        }
    }
    public class Date
    {
        private int day;
        private int month;
        public int Year { get; set; }

        public int Day
        {
            get
            {
                return day;
            }
            set
            {
                if (value <= 0)
                    day = 1;
                else if (value > 31)
                    day = 31;
                else
                    day = value;
            }
        }

        public int Month
        {
            get
            {
                return month;
            }
            set
            {
                if (value < 1)
                    month = 1;
                else if (value > 12)
                    month = 12;
                else
                    month = value;
            }
        }
        public Date(int d, int m, int y)
        {
            Day = d;
            Month = m;
            Year = y;
        }
        public Date() : this(0,0,0)
        { }
        public override string ToString()
        {
            return $"{Day}/{Month}/{Year}";
        }

    }
}
