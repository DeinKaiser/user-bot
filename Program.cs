using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace UserInfoBot
{
    class Program
    {
        static TelegramBotClient botClient = new TelegramBotClient("BOT_TOKEN");
        public static MySqlConnection connection = DBUtils.GetDBConnection();
        static void Main()
        {
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message \"{e.Message.Text}\" in chat {e.Message.Chat.Id}.");

               if(e.Message.Text == "/start")
                {
                    string result = "";
                    try
                    {
                        connection.Open();

                        MySqlCommand cmd = connection.CreateCommand();
                        string sql = "SELECT ID from user WHERE Telegram_ID = " + e.Message.Chat.Id.ToString() ;
                        cmd.CommandText = sql;
                        int id = -1;

                        MySqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            id = rdr.GetInt32(0);
                        }
                        rdr.Close();

                        if (id == -1)
                        { 

                            sql = "INSERT into user (Telegram_Id, User_Name, First_Name, Last_Name) "
                                                            + " values (@telegramId, @userName, @firstName, @lastName) ";

                            cmd = connection.CreateCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.Add("@telegramId", MySqlDbType.VarChar).Value = e.Message.Chat.Id.ToString();
                            cmd.Parameters.Add("@userName", MySqlDbType.VarChar).Value = e.Message.Chat.Username;
                            cmd.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = e.Message.Chat.FirstName;
                            cmd.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = e.Message.Chat.LastName;
                            int rowCount = cmd.ExecuteNonQuery();

                            Console.WriteLine("Row Count affected = " + rowCount);
                            result = " added";
                        }
                        else
                        {

                            sql = "UPDATE user SET User_Name = @userName, First_Name = @firstName," +
                                " Last_Name = @lastName WHERE Telegram_Id =" + e.Message.Chat.Id.ToString();

                            cmd = connection.CreateCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.Add("@userName", MySqlDbType.VarChar).Value = e.Message.Chat.Username;
                            cmd.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = e.Message.Chat.FirstName;
                            cmd.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = e.Message.Chat.LastName;
                            int rowCount = cmd.ExecuteNonQuery();

                            Console.WriteLine("Row Count affected = " + rowCount);
                            result = " updated";
                        }
                    }
                    catch (Exception ex)
                    {
                        result = "..... failed";
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                    finally
                    {
                        connection.Close();
                        await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"Successfuly{result}!"
                        );
                    }
                    
                }
                if (e.Message.Text == "/info")
                {
                    try
                    {
                        connection.Open();

                        MySqlCommand cmd = connection.CreateCommand();
                        string sql = "SELECT ID from user WHERE Telegram_ID = " + e.Message.Chat.Id.ToString();
                        cmd.CommandText = sql;
                        int id = -1;

                        MySqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            id = rdr.GetInt32(0);
                        }
                        rdr.Close();

                        if (id == -1)
                        {

                            await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: "You're not registered yet!"
                        );

                        }
                        else
                        {
                            sql = "SELECT * from user WHERE Telegram_ID = " + e.Message.Chat.Id.ToString();
                            cmd = connection.CreateCommand();
                            cmd.CommandText = sql;
                            rdr = cmd.ExecuteReader();
                            string tg_id = "";
                            string user_name = "";
                            string first_name = "";
                            string last_name = "";
                            while (rdr.Read())
                            {
                                id = rdr.GetInt32(0);
                                tg_id = rdr.GetString(1);
                                user_name = rdr.GetString(2);
                                first_name = rdr.GetString(3);
                                last_name = rdr.GetString(4);
                            }
                            rdr.Close();
                            await botClient.SendTextMessageAsync(
                                chatId: e.Message.Chat,
                                text: $"ID: {id},\nTelegram ID: {tg_id}\nUsername: {user_name},\nFirst Name: {first_name},\nLast Name: {last_name}"
                            );

                        }
                    }
                    catch (Exception ex)
                    {
                        
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
