using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static quizApp.Constants;

namespace quizApp
{
    #region Interfaces
    /// <summary>
    /// Game main inteface
    /// </summary>
    public interface IGame
    {
        void ShowGameMenu();
    }
    /// <summary>
    /// Abstract class with constructor
    /// </summary>
    public abstract class AGame : IGame
    {
        protected QuizData gameData = null;
        protected AGame(QuizData data)
        {
            gameData = data;
        }
        public abstract void ShowGameMenu();
    }
    /// <summary>
    /// Interface that implements addition, deletion,editing
    /// </summary>
    public interface IEditable
    {
        void Add();
        void Remove();
        void EditMenu();
   }
    /// <summary>
    /// Interface that implements printing all data
    /// </summary>
    public interface IPrint
    {
        void ShowAll();
    }
    #endregion

    #region Test
    /// <summary>
    /// class which contains list of Questions 
    /// </summary>
    public class Test : IEditable,IPrint
    {
        public static int LAST_ID = 0;
        public int ID { get; private set; }
        public string Text { get; set; }
        public List<Question> Questions { get; set; }
        public Test(int id) : this("Без названия",id)
        {

        }
        public Test() : this("Без названия")
        {

        }
        public Test(string name, int id = -1)
        {
            Text = name;
            Questions = new List<Question>();
            if (id == -1)
            {
                LAST_ID++;
                ID = LAST_ID;
            }
            else
                ID = id;
        }
        public void PassTestByUser(IUser user)
        {
            int correct = 0;
            int count = 0;
            string input = null;
            int select = 0;
            foreach(Question question in Questions)
            {
                Console.WriteLine($"============ ВОПРОС #{count + 1} ============");
                Console.WriteLine(question.Text);
                Console.WriteLine("Варианты ответа: ");
                for(int i = 0; i < question.Answers.Count; i++)
                {
                    Console.WriteLine($"Введите '{i + 1}' - {question.Answers[i].Text}");
                }
                while (true)
                {
                    Console.Write("Ваш ответ: ");
                    input = Console.ReadLine();
                    select = Convert.ToInt32(input)-1;
                    if(select < 0 || select >= question.Answers.Count)
                    {
                        Console.WriteLine("[ERROR]: Ошибка ввода..");
                        continue;
                    }
                    if(question.Answers[select].IsCorrect)
                    {
                        correct++;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[SUCCESS]: Вы верно ответили на этот вопрос. Всего верных ответов: " + correct);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[FAIL]: Вы НЕверно ответили на этот вопрос. Всего верных ответов: " + correct);
                    }
                    Console.ResetColor();
                    break;
                }
                Console.WriteLine();
                count++;
            }
            Result result = new Result();
            result.Grade = correct * MAX_GRADE / count;
            result.TestID = this.ID;
            Console.WriteLine($"Вы верно ответили на {correct} вопросов из {count}.");
            Console.WriteLine($"Ваша оценка: {result.Grade}");
            user.Results.Add(result);

        }
        public void Add() // Add new question
        {
            Console.Write("Введите текст вопроса: ");
            string input = Console.ReadLine();
            Question question = new Question(input);
            bool isCorrectAnswerPresent = false; // true - if there is already a correct answer
            for(int i = 0; i < ANSWERS_COUNT; i++)
            {
                Console.Write($"Введите вариант ответа #{i + 1}: ");
                Answer a = new Answer();
                a.Text = Console.ReadLine();
                if(!isCorrectAnswerPresent && i == ANSWERS_COUNT-1) // if this is the last question but correct answer have not yet
                {
                    a.IsCorrect = true;
                    isCorrectAnswerPresent = true;
                }
                if(!isCorrectAnswerPresent) // ask if its a correct answer
                {
                    Console.Write("Это правильный ответ? (y/n)");
                    input = Console.ReadLine();
                    char result = Convert.ToChar(input);
                    if (result == 'y' || result == 'Y')
                    {
                        a.IsCorrect = true;
                        isCorrectAnswerPresent = true;
                    }
                    else if (result == 'N' || result == 'n')
                    {
                        a.IsCorrect = false;
                    }
                    else
                        Console.WriteLine("[ERROR]: Можно вводить только 'Y' или 'N'.");
                }
                question.Answers.Add(a);
            }
            Questions.Add(question);
            Console.WriteLine($"[SUCCESS]: Вы успешно добавили тест \"{input}\".");
        }

        public void Remove() // remove question
        {
            if(Questions.Count <= 0)
            {
                Console.WriteLine("[ERROR]: В данный момент список вопросов пуст.");
                return;
            }
            Console.WriteLine("Введите порядковый номер вопроса для удаления: ");
            string input = Console.ReadLine();
            int id = Convert.ToInt32(input) - 1;
            if (id < 0 || id >= Questions.Count)
            {
                Console.WriteLine("[ERROR]: Вы ввели неверный порядковый номер.");
                return;
            }
            if (Questions.Remove(Questions[id]))
                Console.WriteLine($"[SUCCESS]: Вы успешно удалили вопрос по номеру {id + 1}.");
            else
                Console.WriteLine($"[SUCCESS]: Произошла ошибка при удалении вопроса(#{id + 1}).");
        }

        public void EditMenu()
        {
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("============ Меню теста ============");
                Console.WriteLine("Введите 1 - чтобы посмотреть все вопросы");
                Console.WriteLine("Введите 2 - чтобы добавить вопрос");
                Console.WriteLine("Введите 3 - чтобы удалить вопрос");
                Console.WriteLine("Введите 4 - чтобы вернуться");
                string input = Console.ReadLine();
                int select = Convert.ToInt32(input);
                switch (select)
                {
                    case 1:
                        ShowAll();
                        break;
                    case 2:
                        Add();
                        break;
                    case 3:
                        Remove();
                        break;
                    case 4:
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("[ERROR]: Неверное значение, попробуйте еще раз!");
                        break;
                }
            }
        }

        public void ShowAll()
        {
            if (Questions.Count < 1)
            {
                Console.WriteLine("[ERROR]: В данный вопрос список вопросов пуст.");
                return;
            }
            Console.WriteLine("========== Список вопросов ==========");
            for (int i = 0; i < Questions.Count; i++)
            {
                Console.WriteLine($"Вопрос #{i+1}: {Questions[i].Text}");
                Console.WriteLine("Варианты ответа: ");
                for(int j = 0; j < Questions[i].Answers.Count; j++)
                {
                    Console.Write($"({j + 1}) - {Questions[i].Answers[j].Text}. ");
                    if ((j+1) % 2 == 0)
                        Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Всего вопрсов: {Questions.Count}.");
        }
    }
    #endregion

    #region Category
    /// <summary>
    /// class which contains list of tests
    /// </summary>
    public class Category : IEditable,IPrint
    { 
        public string Text { get; set; }
        public List<Test> Tests { get; set; }
        public Category() : this("Без названия")
        {
        }
        public Category(string name)
        {
            Text = name;
            Tests = new List<Test>();
        }

        public void Add() // add new test
        {
            Console.Write("Введите название для теста: ");
            string input = Console.ReadLine();
            Test test = new Test(input);
            Tests.Add(test);
            Console.WriteLine($"[SUCCESS]: Вы успешно добавили тест \"{input}\".");
        }

        public void Remove() //remove test
        {
            if (Tests.Count <= 0)
            {
                Console.WriteLine("[ERROR]: В данный момент список тестов пуст.");
                return;
            }
            Console.WriteLine("Введите порядковый номер теста для удаления: ");
            string input = Console.ReadLine();
            int id = Convert.ToInt32(input) -1;
            if (id < 0 || id >= Tests.Count)
            {
                Console.WriteLine("[ERROR]: Вы ввели неверный порядковый номер.");
                return;
            }
            if (Tests.Remove(Tests[id]))
                Console.WriteLine($"[SUCCESS]: Вы успешно удалили тест по номеру {id+1}.");
            else
                Console.WriteLine($"[SUCCESS]: Произошла ошибка при удалении теста(#{id+1}).");
        }

        public void EditMenu()
        {
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("============ Меню раздела ============");
                Console.WriteLine("Введите 1 - чтобы посмотреть все тесты");
                Console.WriteLine("Введите 2 - чтобы добавить тест");
                Console.WriteLine("Введите 3 - чтобы удалить тест");
                Console.WriteLine("Введите 4 - чтобы редактировать тест");
                Console.WriteLine("Введите 5 - чтобы вернуться");
                string input = Console.ReadLine();
                int select = Convert.ToInt32(input);
                switch (select)
                {
                    case 1:
                        ShowAll();
                        break;
                    case 2:
                        Add();
                        break;
                    case 3:
                        Remove();
                        break;
                    case 4:
                        EditMenuNextStep();
                        break;
                    case 5:
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("[ERROR]: Неверное значение, попробуйте еще раз!");
                        break;
                }
            }
        }
        //select of test to edit
        public void EditMenuNextStep()
        {
            if(Tests.Count <= 0)
            {
                Console.WriteLine("[ERROR]: В данный момент список тестов пуст.");
                return;
            }
            Console.WriteLine("Введите порядковый номер теста для редактирования: ");
            string input = Console.ReadLine();
            int id = Convert.ToInt32(input)-1;
            if (id < 0 || id >= Tests.Count)
            {
                Console.WriteLine("[ERROR]: Вы ввели неверный порядковый номер.");
                return;
            }
            Tests[id].EditMenu();
        }

        public void ShowAll()
        {
            if (Tests.Count < 1)
            {
                Console.WriteLine("[ERROR]: В данный момент список тестов пуст.");
                return;
            }
            Console.WriteLine("========== Тесты ==========");
            for (int i = 0; i < Tests.Count; i++)
            {
                Console.WriteLine($"- {i + 1} - {Tests[i].Text}.");
            }
            Console.WriteLine($"Всего тестов: {Tests.Count}.");
        }
    }
    #endregion

    #region QuizData
    /// <summary>
    /// class which contains all game data
    /// </summary>
    public class QuizData : IEditable, IPrint
    {
        public List<Category> Categories { get; set; }
        public Test GetTestByID(int ID)
        {
            Test result = null;
            foreach(Category cat in Categories)
            {
                foreach(Test test in cat.Tests)
                {
                    if (test.ID == ID)
                    {
                        result = test;
                        break;
                    }
                }
            }
            return result;
        }
        public QuizData()
        {
            Categories = new List<Category>();
        }
        public void ShowAll()
        {
            if (Categories.Count < 1)
            {
                Console.WriteLine("[ERROR]: Список разделов пуст.");
                return;
            }
            Console.WriteLine("========== Разделы викторины ==========");
            for (int i = 0; i < Categories.Count; i++)
            {
                Console.WriteLine($"- {i + 1} - {Categories[i].Text}.");
            }
            Console.WriteLine($"Всего разделов: {Categories.Count}.");
        }
        public void Add() // add new category
        {
            Console.Write("Введите название для раздела: ");
            string input = Console.ReadLine();
            Category category = new Category(input);
            Categories.Add(category);
            Console.WriteLine($"[SUCCESS]: Вы успешно добавили раздел \"{input}\".");
        }
        public void Remove() // remove category
        {
            if (Categories.Count <= 0)
            {
                Console.WriteLine("[ERROR]: В данный момент список разделов пуст.");
                return;
            }
            Console.WriteLine("Введите порядковый номер раздела для удаления: ");
            string input = Console.ReadLine();
            int id = Convert.ToInt32(input) -1;
            if (id < 0 || id >= Categories.Count)
            {
                Console.WriteLine("[ERROR]: Вы ввели неверный порядковый номер.");
                return;
            }
            if (Categories.Remove(Categories[id]))
                Console.WriteLine($"[SUCCESS]: Вы успешно удалили раздел по номеру {id+1}.");
            else
                Console.WriteLine($"[SUCCESS]: Произошла ошибка при удалении раздела(#{id+1}).");
        }
        public void EditMenu()
        {
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("============ Админ-меню ============");
                Console.WriteLine("Введите 1 - чтобы посмотреть все разделы");
                Console.WriteLine("Введите 2 - чтобы добавить раздел");
                Console.WriteLine("Введите 3 - чтобы удалить раздел");
                Console.WriteLine("Введите 4 - чтобы редактировать раздел");
                Console.WriteLine("Введите 5 - чтобы вернуться");
                string input = Console.ReadLine();
                int select = Convert.ToInt32(input);
                switch (select)
                {
                    case 1:
                        ShowAll();
                        break;
                    case 2:
                        Add();
                        break;
                    case 3:
                        Remove();
                        break;
                    case 4:
                        EditMenuNextStep();
                        break;
                    case 5:
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("[ERROR]: Неверное значение, попробуйте еще раз!");
                        break;
                }
            }
        }
        //select of category to edit
        public void EditMenuNextStep()
        {
            if (Categories.Count <= 0)
            {
                Console.WriteLine("[ERROR]: В данный момент список разделов пуст.");
                return;
            }
            Console.WriteLine("Введите порядковый номер раздела для редактирования: ");
            string input = Console.ReadLine();
            int id = Convert.ToInt32(input)-1;
            if (id < 0 || id >= Categories.Count)
            {
                Console.WriteLine("[ERROR]: Вы ввели неверный порядковый номер.");
                return;
            }
            Categories[id].EditMenu();
        }
    }
    #endregion

    #region QuizGame
    /// <summary>
    /// class to play quizgame for simple user
    /// </summary>
    public class QuizForUser : AGame
    {
        private IUser user = null;
        public QuizForUser(QuizData data, IUser user) : base(data)
        {
            this.user = user;
        }
        public override void ShowGameMenu()
        {
            if (gameData == null)
            {
                Console.WriteLine("Ошибка...");
            }
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("============ QuizApp ============");
                Console.WriteLine("Введите 1 - чтобы посмотреть все разделы");
                Console.WriteLine("Введите 2 - чтобы выбрать раздел");
                Console.WriteLine("Введите 3 - чтобы посмотреть результаты");
                Console.WriteLine("Введите 4 - чтобы вернуться");
                string input = Console.ReadLine();
                int select = Convert.ToInt32(input);
                switch (select)
                {
                    case 1:
                        gameData.ShowAll();
                        break;
                    case 2:
                        SelectCategories();
                        break;
                    case 3:
                        PrintUserResults();
                        break;
                    case 4:
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("[ERROR]: Неверное значение, попробуйте еще раз!");
                        break;
                }
            }
        }
        public void PrintUserResults()
        {
            if (user.Results.Count <= 0)
            {
                Console.WriteLine($"[ERROR]: Вы еще не проходили ни одного теста.");
                return;
            }
            Console.WriteLine("============= Оценки =============");
            int count = 0;
            foreach (Result r in user.Results)
            {
                Test test = gameData.GetTestByID(r.TestID);
                if (test == null)
                    continue;
                count++;
                Console.WriteLine($"[{count}] Тест: {test.Text}, оценка: {r.Grade}, дата прохождения: {r.GetDate()}");
            }
            Console.WriteLine("Всего пройденных тестов: "+ count);
            Console.WriteLine();
        }
        public void SelectCategories()
        {
            if (gameData.Categories.Count <= 0)
            {
                Console.WriteLine("[ERROR]: В данный момент список разделов пуст.");
                return;
            }
            Console.WriteLine("Введите порядковый номер раздела: ");
            string input = Console.ReadLine();
            int id = Convert.ToInt32(input) - 1;
            if (id < 0 || id >= gameData.Categories.Count)
            {
                Console.WriteLine("[ERROR]: Вы ввели неверный порядковый номер.");
                return;
            }
            ChooseCategory(id);
        }
        public void ChooseCategory(int id)
        {
            if(gameData.Categories[id].Tests.Count <= 0)
            {
                Console.WriteLine("[ERROR]: В данном разделе нет тестов для прохождения");
                return;
            }
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("============ QuizApp ============");
                Console.WriteLine("Введите 1 - чтобы посмотреть все тесты");
                Console.WriteLine("Введите 2 - чтобы выбрать тест для прохождения");
                Console.WriteLine("Введите 3 - чтобы вернуться");
                string input = Console.ReadLine();
                int select = Convert.ToInt32(input);
                switch (select)
                {
                    case 1:
                        gameData.Categories[id].ShowAll();
                        break;
                    case 2:
                        SelectTest(id);
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
        public void SelectTest(int catID)
        {
            var data = gameData.Categories[catID];

            if (data.Tests.Count <= 0)
            {
                Console.WriteLine("[ERROR]: В данный момент список тестов пуст.");
                return;
            }
            Console.WriteLine("Введите порядковый номер теста: ");
            string input = Console.ReadLine();
            int id = Convert.ToInt32(input) - 1;
            if (id < 0 || id >= data.Tests.Count)
            {
                Console.WriteLine("[ERROR]: Вы ввели неверный порядковый номер.");
                return;
            }
            data.Tests[id].PassTestByUser(user);
        }
    }
    /// <summary>
    /// class to edit quizgame for administrator
    /// </summary>
    public class QuizForAdmin : AGame
    {
        public QuizForAdmin(QuizData data) : base(data)
        {
        }
        public override void ShowGameMenu()
        {
            if(gameData == null)
            {
                Console.WriteLine("Ошибка...");
            }
            gameData.EditMenu();
        }        
    }
    #endregion
}
