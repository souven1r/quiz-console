using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static quizApp.Constants;
namespace quizApp
{
    public interface IUser
    {
        int ID { get; set; }
        string Login { get; set; }
        string Password { get; set; }
        Date BirthDate { get; set; }
        UserType GetUserType();
        List<Result> Results { get; set; }
        void FillData();
        void ShowSettings();
    }
    public abstract class AUser : IUser
    {
        public string Login { get; set; }
        public int ID { get; set; }
        public string Password { get; set; }
        public Date BirthDate { get; set; }
        public List<Result> Results { get; set; }
        public AUser()
        {
            Results = new List<Result>();
        }
        public abstract UserType GetUserType();
        public void ShowSettings()
        {
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("========== НАСТРОЙКИ ==========");
                Console.WriteLine("Введите 1 - чтобы сменить пароль");
                Console.WriteLine("Введите 2 - чтобы сменить дату рождения");
                Console.WriteLine("Введите 3 - чтобы вернуться в главное меню");
                string input = Console.ReadLine();
                int select = Convert.ToInt32(input);
                switch (select)
                {
                    case 1:
                        SetPass();
                        break;
                    case 2:
                        SetBirthDate();
                        break;
                    case 3:
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("[ERROR]: Неверное значение, попробуйте еще раз!");
                        break;
                }
            }

        }
        private void SetPass()
        {
            Console.Write("Введите пароль: ");
            this.Password = Console.ReadLine();
            Console.WriteLine("[SUCCESS]: Вы установили новый пароль");
        }
        private void SetBirthDate()
        {
            Date newBD = new Date();
            string input;
            Console.Write("Введите ДЕНЬ вашего рождения: ");
            input = Console.ReadLine();
            newBD.Day = Convert.ToInt32(input);
            Console.Write("Введите МЕСЯЦ вашего рождения: ");
            input = Console.ReadLine();
            newBD.Month = Convert.ToInt32(input);
            Console.Write("Введите ГОД вашего рождения: ");
            input = Console.ReadLine();
            newBD.Year = Convert.ToInt32(input);
            this.BirthDate = newBD;

        }
        public void FillData()
        {
            string input;
            Console.WriteLine("Введите логин для вашего аккаунта: ");
            Login = Console.ReadLine();
            Console.WriteLine("Введите пароль для вашего аккаунта: ");
            Password = Console.ReadLine();
            Console.WriteLine("Введите ДЕНЬ вашего рождения: ");
            input = Console.ReadLine();
            BirthDate = new Date();
            BirthDate.Day = Convert.ToInt32(input);
            Console.WriteLine("Введите МЕСЯЦ вашего рождения: ");
            input = Console.ReadLine();
            BirthDate.Month = Convert.ToInt32(input);
            Console.WriteLine("Введите ГОД вашего рождения: ");
            input = Console.ReadLine();
            BirthDate.Year = Convert.ToInt32(input);
        }
    }
    public class Result
    {
        public int TestID { get; set; }
        public double Grade { get; set; }
        public int Date { get; set; }
        public string GetDate()
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Date).ToLocalTime();
            string result = dtDateTime.ToString("d") + ", " + dtDateTime.ToShortTimeString();
            return result;
        }
        public Result()
        {
            Date = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }

    public class User : AUser
    {
        public User() : base()
        {
        }
        
        public override UserType GetUserType()
        {
            return UserType.User;
        }

    }
    public class Admin : AUser
    {
        //  private QuizGame quizGame = null;
        public Admin() : base()
        {
        }
        public override UserType GetUserType()
        {
            return UserType.Admin;
        }

    }
}
