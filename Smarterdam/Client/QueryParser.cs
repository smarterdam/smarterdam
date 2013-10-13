using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Client
{
    //examples of query;

    //create alert "AlertA" where alert.email = "max@gebeus.ru" and alert.type = "txt";

    //select forecast("consumption") from datastream "sensorA" where forecast.horizon = 96 and forecast.frequency = "00:00" initiate alert("AlertA");
    //select outlier "temperature" from datastream "sensorA" initiate alert("AlertA");



    //select forecast (outlier "temperature") from datastream "sensorA" where forecast.horizon = 96 and forecast.frequency = "00:00" initiate alert("AlertA");

    //            <SELECT statement> ::=  
    //    [WITH <common_table_expression> [,...n]]
    //    <query_expression> 
    //    [ ORDER BY { order_by_expression | column_position [ ASC | DESC ] } 
    //  [ ,...n ] ] 
    //    [ <FOR Clause>] 
    //    [ OPTION ( <query_hint> [ ,...n ] ) ] 
    //<query_expression> ::= 
    //    { <query_specification> | ( <query_expression> ) } 
    //    [  { UNION [ ALL ] | EXCEPT | INTERSECT }
    //        <query_specification> | ( <query_expression> ) [...n ] ] 
    //<query_specification> ::= 
    //SELECT [ ALL | DISTINCT ] 
    //    [TOP ( expression ) [PERCENT] [ WITH TIES ] ] 
    //    < select_list > 
    //    [ INTO new_table ] 
    //    [ FROM { <table_source> } [ ,...n ] ] 
    //    [ WHERE <search_condition> ] 
    //    [ <GROUP BY> ] 
    //    [ HAVING < search_condition > ] 
    public class QueryParser : IQueryParser
    {
        private Dictionary<string, List<string>> CommandsSample1;
        private Dictionary<string, List<string>> CommandsSample2;
        private List<string> DB;
        private List<string> Owner;
        private List<string> Stream; 

        public QueryParser()
        {
            CommandsSample1 = new Dictionary<string, List<string>>();
            CommandsSample1.Add("forecast", new List<string>(new string[] { "attr", "horizon", "frq" }));
            CommandsSample1.Add("outlier", new List<string>(new string[] { "attr" }));
            CommandsSample1.Add("SAVE", null);
            CommandsSample1.Add("KILL", null);
            
            CommandsSample2 = new Dictionary<string, List<string>>();
            CommandsSample2.Add("then", new List<string>(new string[] { "alert" }));

            DB = new List<string>();
            DB.Add("db");

            Owner = new List<string>();
            Owner.Add("ownerMax");

            Stream = new List<string>();
            Stream.Add("streamA");
            Stream.Add("streamB");
        }

        private bool FindCommand(string word)
        {
            bool IfFind = false;

            char RazdelitelMain = '(';
            string[] words = word.Split(RazdelitelMain);

            foreach (KeyValuePair<string, List<string>> item in CommandsSample1)
            {
                if (item.Key == words[0])
                    IfFind = true;
            }

            return IfFind;
        }

        private Dictionary<string, string> SplitCommand(string word)
        {
            Dictionary<string, string> readyCommands = new Dictionary<string, string>();
            //List<string> Parameters = new List<string>();
            //List<string> Attributes = new List<string>(); 
            char RazdelitelMain = ';';
            char RazdelitelTemp = ':';
            string[] words = word.Split(RazdelitelMain);

            if (words[0][0] == '{') //идёт проверка на валидность строки
            {
                for (int j = 0; j < words.Length; j++)
                {
                    string[] TempWords = words[j].Split(RazdelitelTemp);

                    if (TempWords[0][0] == '{')
                        TempWords[0] = TempWords[0].Remove(0, 1);

                    //Parameters.Add(TempWords[0]);
                    if (TempWords[1][TempWords[1].Length - 1] == ')')
                    {
                        TempWords[1] = TempWords[1].Remove(TempWords[1].Length - 1, 1);
                        TempWords[1] = TempWords[1].Remove(TempWords[1].Length - 1, 1);
                    }
                    if (TempWords[1][0] == '\"')
                    {
                        TempWords[1] = TempWords[1].Remove(0, 1);
                        TempWords[1] = TempWords[1].Remove(TempWords[1].Length - 1, 1);
                    }
                    //Attributes.Add(TempWords[1]);
                    readyCommands.Add(TempWords[0], TempWords[1]);
                }
                //string[] ParamWords = word.Split(RazdelitelTemp);
            }
            else return null; // ошибка ввода данных!!!
            return readyCommands;
        }

        private List<string> ParseCom(string one_word, int com)
        {
            List<string> ResultList = new List<string>();
            char RazdelitelMain = ';';
            string[] words = one_word.Split(RazdelitelMain);


            return ResultList;
        }

        public Commands Parse(string query)
        {
            char RazdelitelMain = '.';
            Commands ListParseCommands = new Commands();
            int PrevCom = 0; //какая была предыдущая команда
            Dictionary<string, string> dict = new Dictionary<string, string> {
            {"null", "null"},
            };

            //int SecondCom = 0; //какое слово второе
            //int ThirdCom = 0; //какое слово третье
            //int FourthCom = 0; //какое слово четвертое

            int CountCom = 0; //кол-во записанных слов


            string[] words = query.Split(RazdelitelMain);

            for (int i = 0; i < words.Count(); i++)
            {
                if (DB.Contains(words[i]) && i == 0) //ПРОВЕРЯЕМ НА НАЛИЧИЕ ТАКОЙ БД 
                {
                    Command db = new Command(words[i].ToString(), dict);
                    ListParseCommands.ListCommand.Add(db); //добавляем такую БД
                    PrevCom = 0;
                }
                else if (Owner.Contains(words[i]) && PrevCom == 0) //если овнер
                {
                    Command owner = new Command(words[i].ToString(), dict);
                    ListParseCommands.ListCommand.Add(owner); //добавляем такую овнер
                    PrevCom = 1;
                }
                else if ((Stream.Contains(words[i]) && PrevCom == 0) || (Stream.Contains(words[i]) && PrevCom == 1)) //если поток
                {
                    if (PrevCom == 0)
                    {
                        Command owner = new Command(null, dict);
                        ListParseCommands.ListCommand.Add(owner); //добавляем такую овнер
                        Command stream = new Command(words[i].ToString(), dict);
                        ListParseCommands.ListCommand.Add(stream); //добавляем такую поток
                        PrevCom = 2;
                    }
                    else if (PrevCom == 1)
                    {
                        Command stream = new Command(words[i].ToString(), dict);
                        ListParseCommands.ListCommand.Add(stream); //добавляем такой поток 
                        PrevCom = 2;
                    }
                }
                else if ((FindCommand(words[i]) && PrevCom == 0) || (FindCommand(words[i]) && PrevCom == 1) || (FindCommand(words[i]) && PrevCom == 2)) //если первая команда
                {
                    if (PrevCom == 0) //после БД
                    {
                        Command owner = new Command(null, dict);
                        ListParseCommands.ListCommand.Add(owner); //добавляем такой овнер
                        Command stream = new Command(null, dict);
                        ListParseCommands.ListCommand.Add(stream); //добавляем такой поток
                        char Razdelitel = '(';
                        string[] temp_words = words[i].ToString().Split(Razdelitel);
                        if (temp_words[0] == "SAVE" || temp_words[0] == "KILL")
                        {
                            Command command1 = new Command(temp_words[0], null);
                            ListParseCommands.ListCommand.Add(command1); //добавляем такую команду
                        }
                        else
                        {
                            Dictionary<string, string> readyCommands = SplitCommand(temp_words[1]);
                            Command command1 = new Command(temp_words[0], readyCommands);
                            ListParseCommands.ListCommand.Add(command1); //добавляем такую команду
                        }
                        PrevCom = 3;
                    }
                    else if (PrevCom == 1) //после овнера
                    {
                        Command stream = new Command(null, dict);
                        ListParseCommands.ListCommand.Add(stream); //добавляем такую овнер
                        char Razdelitel = '(';
                        string[] temp_words = words[i].ToString().Split(Razdelitel);
                        Dictionary<string, string> readyCommands = SplitCommand(temp_words[1]);
                        Command command1 = new Command(temp_words[0], readyCommands);
                        ListParseCommands.ListCommand.Add(command1); //добавляем такую поток     
                        PrevCom = 3;
                    }
                    else if (PrevCom == 2) //после потока
                    {
                        if (words[i] == "SAVE" || words[i] == "KILL")
                        {
                            Command command1 = new Command(words[i], dict);
                            ListParseCommands.ListCommand.Add(command1); //добавляем такую поток 
                            return ListParseCommands;
                        }
                        else
                        {
                            char Razdelitel = '(';
                            string[] temp_words = words[i].ToString().Split(Razdelitel);
                            Dictionary<string, string> readyCommands = SplitCommand(temp_words[1]);
                            Command command1 = new Command(temp_words[0], readyCommands);
                            ListParseCommands.ListCommand.Add(command1); //добавляем такую поток 
                            PrevCom = 3;
                        }
                    }
                }
                else if ((FindCommand(words[i]) && PrevCom == 3)) //если вторая команда
                {
                    char Razdelitel = '(';
                    string[] temp_words = words[i].ToString().Split(Razdelitel);
                    Dictionary<string, string> readyCommands = SplitCommand(temp_words[1]);
                    Command command2 = new Command(temp_words[0], readyCommands);
                    ListParseCommands.ListCommand.Add(command2); //добавляем такую поток  
                    PrevCom = 4;
                }
                //else return null;
                else if (i != 5)
                    return null;
            }
            return ListParseCommands;
            //return null;
        }
    }
}
