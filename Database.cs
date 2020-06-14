using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using static quizApp.Constants;

namespace quizApp
{
    public interface IDatabase
    {
        void Read();
        void Write();
    }
    /// <summary>
    /// class for load/save IUser items from/to XML file
    /// </summary>
    public class UsersBase : IDatabase
    {
        private List<IUser> Users = null;
        public UsersBase(List<IUser> users)
        {
            Users = users;
        }
        public void Read()
        {
            XmlTextReader reader = new XmlTextReader(Constants.USERS_BASE_PATH);
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "user")
                    {
                        IUser user;
                        UserType type;
                        type = (UserType)Convert.ToInt32(reader.GetAttribute("type"));
                        if (type == UserType.Admin)
                            user = new Admin();
                        else
                            user = new User();

                        user.ID = Convert.ToInt32(reader.GetAttribute("id"));
                        user.Login = reader.GetAttribute("login");
                        user.Password = reader.GetAttribute("password");
                        reader.ReadToDescendant("birthdate");
                        Date birthDate = new Date();
                        birthDate.Day = Convert.ToInt32(reader.GetAttribute("day"));
                        birthDate.Month = Convert.ToInt32(reader.GetAttribute("month"));
                        birthDate.Year = Convert.ToInt32(reader.GetAttribute("year"));
                        user.BirthDate = birthDate;
                        while (reader.ReadToNextSibling("result"))
                        {
                            Result result = new Result();
                            result.TestID = Convert.ToInt32(reader.GetAttribute("testID"));
                            result.Grade = Convert.ToDouble(reader.GetAttribute("grade"));
                            result.Date = Convert.ToInt32(reader.GetAttribute("date"));

                            user.Results.Add(result);
                        }
                        Users.Add(user);
                    }

                }
            }
            catch(System.Xml.XmlException e)
            {
                Console.WriteLine($"Exception(Users.Read): {e.Message}");
            }
            reader.Close();
        }
        public void Write()
        {
            XmlTextWriter writer = new XmlTextWriter(Constants.USERS_BASE_PATH,Encoding.UTF8);
            try
            {
                writer.WriteStartDocument();
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = '\t';
                writer.Indentation = 1;
                writer.WriteStartElement("users");
                foreach (IUser user in Users)
                {
                    writer.WriteStartElement("user");

                    writer.WriteStartAttribute("id", null);
                    writer.WriteString(user.ID.ToString());
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("login", null);
                    writer.WriteString(user.Login);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("password", null);
                    writer.WriteString(user.Password);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("type", null);
                    writer.WriteString(((int)user.GetUserType()).ToString());
                    writer.WriteEndAttribute();

                    writer.WriteStartElement("birthdate");

                    writer.WriteStartAttribute("day", null);
                    writer.WriteString(user.BirthDate.Day.ToString());
                    writer.WriteEndAttribute();
                    writer.WriteStartAttribute("month", null);
                    writer.WriteString(user.BirthDate.Month.ToString());
                    writer.WriteEndAttribute();
                    writer.WriteStartAttribute("year", null);
                    writer.WriteString(user.BirthDate.Year.ToString());
                    writer.WriteEndAttribute();

                    writer.WriteEndElement();
                    foreach (Result result in user.Results)
                    {
                        writer.WriteStartElement("result");

                        writer.WriteStartAttribute("testID", null);
                        writer.WriteString(result.TestID.ToString());
                        writer.WriteEndAttribute();

                        writer.WriteStartAttribute("grade", null);
                        writer.WriteString(result.Grade.ToString());
                        writer.WriteEndAttribute();

                        writer.WriteStartAttribute("date", null);
                        writer.WriteString(result.Date.ToString());
                        writer.WriteEndAttribute();

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
            }
            catch (System.Xml.XmlException e)
            {
                Console.WriteLine($"Exception(Users.Write): {e.Message}");
            }
            writer.Close();


        }
    }

    public class GameBase : IDatabase
    {
        private QuizData data = null;
        public GameBase(QuizData data)
        {
            this.data = data;
        }
        public void Read()
        {
            XmlTextReader reader = new XmlTextReader(Constants.GAME_BASE_PATH);
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "game")
                    {
                        Test.LAST_ID = Convert.ToInt32(reader.GetAttribute("testLastID"));
                        if (!reader.ReadToDescendant("category"))
                            continue;
                        do
                        {
                            Category cat = new Category();
                            cat.Text = reader.GetAttribute("text");
                            if (reader.ReadToDescendant("test"))
                            {
                                do
                                {
                                    int test_id = Convert.ToInt32(reader.GetAttribute("id"));
                                    Test test = new Test(test_id);
                                    test.Text = reader.GetAttribute("text");
                                    if (reader.ReadToDescendant("question"))
                                    {
                                        do
                                        {
                                            Question question = new Question();
                                            question.Text = reader.GetAttribute("text");
                                            if (reader.ReadToDescendant("answer"))
                                            {
                                                do
                                                {
                                                    Answer answer = new Answer();
                                                    answer.Text = reader.GetAttribute("text");
                                                    answer.IsCorrect = Convert.ToBoolean(reader.GetAttribute("isCorrect"));
                                                    question.Answers.Add(answer);
                                                } while (reader.ReadToNextSibling("answer"));
                                            }
                                            test.Questions.Add(question);
                                        } while (reader.ReadToNextSibling("question"));
                                    }
                                    cat.Tests.Add(test);
                                } while (reader.ReadToNextSibling("test"));
                            }
                            data.Categories.Add(cat);
                        } while (reader.ReadToNextSibling("category"));
                    }
                }
            }
            catch (System.Xml.XmlException e)
            {
                Console.WriteLine($"Exception(Game.Read): {e.Message}");
            }
            reader.Close();
        }

        public void Write()
        {
            XmlTextWriter writer = new XmlTextWriter(Constants.GAME_BASE_PATH, Encoding.UTF8);
            try
            {
                writer.WriteStartDocument();
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = '\t';
                writer.Indentation = 1;
                writer.WriteStartElement("game");

                writer.WriteStartAttribute("testLastID", null);
                writer.WriteString(Test.LAST_ID.ToString());
                writer.WriteEndAttribute();

                foreach (Category cat in data.Categories)
                {
                    writer.WriteStartElement("category");

                    writer.WriteStartAttribute("text", null);
                    writer.WriteString(cat.Text);
                    writer.WriteEndAttribute();
                    foreach (Test test in cat.Tests)
                    {
                        writer.WriteStartElement("test");

                        writer.WriteStartAttribute("id", null);
                        writer.WriteString(test.ID.ToString());
                        writer.WriteEndAttribute();

                        writer.WriteStartAttribute("text", null);
                        writer.WriteString(test.Text);
                        writer.WriteEndAttribute();

                        foreach (Question question in test.Questions)
                        {
                            writer.WriteStartElement("question");

                            writer.WriteStartAttribute("text", null);
                            writer.WriteString(question.Text);
                            writer.WriteEndAttribute();
                            foreach (Answer answer in question.Answers)
                            {

                                writer.WriteStartElement("answer");

                                writer.WriteStartAttribute("text", null);
                                writer.WriteString(answer.Text);
                                writer.WriteEndAttribute();

                                writer.WriteStartAttribute("isCorrect", null);
                                writer.WriteString(answer.IsCorrect.ToString());
                                writer.WriteEndAttribute();

                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
            }
            catch (System.Xml.XmlException e)
            {
                Console.WriteLine($"Exception(Game.Write): {e.Message}");
            }
            writer.Close();
        }
    }
}
