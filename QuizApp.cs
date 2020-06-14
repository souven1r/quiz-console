using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static quizApp.Constants;

namespace quizApp
{
    public class QuizUi
    {
        private QuizApp quiz { get; set; }
        public QuizUi()
        {
            quiz = new QuizApp();
        }
        public void Start()
        {
            quiz.LoadUsers();
            quiz.LoadGame();
            ShowAppMenu();
        }
        public void Stop()
        {
            quiz.SaveUsers();
            quiz.SaveGame();
            Console.WriteLine("QuizApp has been closed");
        }
        public void ShowAppMenu()
        {
            bool loop = true;
            while(loop)
            { 
                quiz.PrintCurrentUserMessage();
                Console.WriteLine("============== MENU ==============");
                Console.WriteLine("Введите 1 - для авторизации");
                Console.WriteLine("Введите 2 - для регистрации");
                Console.WriteLine("Введите 3 - чтобы открыть меню");
                Console.WriteLine("Введите 4 - чтобы открыть настройки");
                Console.WriteLine("Введите 5 - чтобы выйти");
                Console.Write("Ваш выбор: ");
                string input = Console.ReadLine();
                int select = Convert.ToInt32(input);
                switch(select)
                {
                    case 1:
                        quiz.ShowLoginMenu();
                        break;
                    case 2:
                        quiz.ShowRegisterMenu();
                        break;
                    case 3:
                        quiz.ShowUserMenu();
                        break;
                    case 4:
                        quiz.ShowSettings();
                        break;
                    case 5:
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("ERROR: Вы допустили ошибку при вводе номера, попробуйте еще раз!");
                        break;
                }

             }

        }


    }
    public class QuizApp
    {
        private static List<IUser> users = new List<IUser>();
        private static QuizData gameData = new QuizData();
        private static IGame game = null;
        private IDatabase Data = null;
        public IUser CurrentUser = null;
        public void LoadGame()
        {
            Data = new GameBase(gameData);
            Data.Read();
            Data = null;
            Console.WriteLine($"[XML]: Загружено {gameData.Categories.Count} разделов из XML файла");
        }
        public void SaveGame()
        {
            Data = new GameBase(gameData);
            Data.Write();
            Data = null;
        }
        public void LoadUsers()
        {
            Data = new UsersBase(users);
            Data.Read();
            Data = null;
            Console.WriteLine($"[XML]: Загружено {users.Count} аккаунта из XML файла");
        }
        public void SaveUsers()
        {
            Data = new UsersBase(users);
            Data.Write();
            Data = null;
        }
        public void ShowLoginMenu()
        {
            if (!UnLogin())
                return;
            string login, pass;
            int uncorrectCount = 0;
            while (true)
            {
                if(uncorrectCount >=5)
                {
                    Console.WriteLine("[ERROR]: Вы ввели неверный пароль или логин больше чем 5 раз..");
                    break;
                }
                Console.Write("Введите логин: ");
                login = Console.ReadLine();
                Console.Write("Введите пароль: ");
                pass = Console.ReadLine();
                if(TryLogin(login,pass))
                {
                    Console.WriteLine("[SUCCESS]: Вы успешно авторизовались!");
                    break;
                }
                else
                {
                    Console.WriteLine("[ERROR]: Неверный пароль или логин!");
                    uncorrectCount++;
                }
            }

        }
        public void ShowRegisterMenu()
        {
            if (!UnLogin())
                return;
            IUser newUser;
            if (users.Count == 0)
                newUser = new Admin();
            else
                newUser = new User();
            while (true)
            {
                newUser.FillData();
                if (!TryReg(newUser.Login))
                {
                    Console.WriteLine("[ERROR]: Уже существует аккаунт с таким логином");
                }
                else
                    break;
            }
            users.Add(newUser);
            newUser.ID = users.Count;
        }
        public void ShowUserMenu()
        {
            if (CurrentUser == null)
            {
                Console.WriteLine("[ERROR]: Чтобы открыть меню игрока вы должны авторизоваться");
                return;
            }
            if(CurrentUser.GetUserType() == UserType.Admin)
            {
                while(true)
                {
                    Console.WriteLine("Вы хотите войти в игру как администатор или как игрок? (a/u): ");
                    string input = Console.ReadLine();
                    char c = Convert.ToChar(input);
                    if(c == 'a' || c == 'A')
                    {
                        game = new QuizForAdmin(gameData);
                        break;
                    }
                    else if(c == 'u' || c == 'U')
                    {
                        game = new QuizForUser(gameData, CurrentUser);
                        break;
                    }
                }
            }
            else
                game = new QuizForUser(gameData, CurrentUser);

            game.ShowGameMenu();
           // CurrentUser.ShowMenu();
        }
        public void ShowSettings()
        {
            if(CurrentUser == null)
            {
                Console.WriteLine("[ERROR]: Чтобы открыть меню настроек вы должны авторизоваться");
                return;
            }
            CurrentUser.ShowSettings();
        }
        public void PrintCurrentUserMessage()
        {
            if (CurrentUser == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("QuizApp: Вы не авторизованы, пожалуйста, авторизуйтесь или зарегистрируйтесь!");
            }
            else
            {
                if (CurrentUser.GetUserType() == UserType.Admin)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"QuizApp: Вы ({CurrentUser.Login}) авторизованы как администратор!");
                }
                else if (CurrentUser.GetUserType() == UserType.User)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"QuizApp: Вы ({CurrentUser.Login}) авторизованы как обычный пользователь!");
                }
            }
            Console.ResetColor();

        }
        private bool TryReg(string login)
        {
            IUser user = users.Find(u => u.Login == login);
            if (user == null)
                return true;
            else
                return false;
        }
        private bool TryLogin(string login, string pass)
        {
            IUser user = users.Find(u => u.Login == login && u.Password == pass);
            if (user == null)
                return false;
            CurrentUser = user;
            return true;
        }
        private bool UnLogin()
        {
            if (CurrentUser != null)
            {
                string input;
                char select;
                while (true)
                {
                    Console.Write("ERROR: Вы сейчас авторизованы. Вы хотите выйти из аккаунта и продолжить регистрацию? (Y/N): ");
                    input = Console.ReadLine();
                    select = Convert.ToChar(input);
                    if (select == 'y' || select == 'Y')
                    {
                        CurrentUser = null;
                        game = null;
                        return true;
                    }
                    else if (select == 'n' || select == 'N')
                        return false;
                    else
                        Console.WriteLine("ERROR: Вы допустили ошибку при вводе, попробуйте еще раз!");
                }
            }
            else
                return true;
        }
       
    }
} 
