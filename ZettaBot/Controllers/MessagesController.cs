///@author = Cristian Ioan Niculae 
///@email = cristianniculae29@gmail.com

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using MySql.Data.MySqlClient;

namespace ZettaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                MySql.Data.MySqlClient.MySqlConnection conn;
                string myConnectionString;
                string serverIp = "46.101.34.121";
                string serverPort = "3306";
                string id = "zetta";
                string pass = "zettaBot";
                string database = "databaseName";
                myConnectionString = "server="+serverIp+";port="+serverPort+";uid="+id+";" +
                    "pwd="+pass+";database="+database+";Allow User Variables=True;";
                string result = "";
                try
                {
                    conn = new MySql.Data.MySqlClient.MySqlConnection();
                    conn.ConnectionString = myConnectionString;
                    conn.Open();
                    String message = activity.Text.ToLower().Trim();
                    if (message.IndexOf(" ") != -1)
                    {
                        if(message.IndexOf("booty") > 0) {

                            for (int i=0; i<10; ++i)
                            {
                                Activity booty = activity.CreateReply($"Booty incoming");
                                await connector.Conversations.ReplyToActivityAsync(booty);
                            }

                            Activity bootyA = activity.CreateReply($"Booty arrived: (‿ˠ‿)");
                            await connector.Conversations.ReplyToActivityAsync(bootyA);
                        }else if (message.Substring(0, message.IndexOf(" ")).Equals("register"))
                        {
                            String courseCode = message.Substring(message.IndexOf(" ") + 1).Trim();
                            int id = -1;
                            try
                            {
                                using (MySqlCommand cmd1 = new MySqlCommand("SELECT id FROM modules WHERE code = \"" + courseCode + "\"", conn))
                                {
                                    var reader1 = cmd1.ExecuteReader();
                                    if (reader1.Read())
                                    {
                                        id = reader1.GetInt32(0);
                                    }
                                }
                                if (id != -1)
                                {
                                    bool hasModule = false;
                                    try
                                    {
                                        conn.Close();
                                        conn = new MySql.Data.MySqlClient.MySqlConnection();
                                        conn.ConnectionString = myConnectionString;
                                        conn.Open();
                                        using (MySqlCommand cmd2 = new MySqlCommand("SELECT * FROM usertimes WHERE id=\"" + activity.From.Id.ToString() + "\" AND moduleId=" + id.ToString(), conn))
                                        {
                                            var reader2 = cmd2.ExecuteReader();
                                            if (reader2.Read())
                                            {
                                                hasModule = true;
                                            }
                                        }
                                        if (!hasModule)
                                        {
                                            try
                                            {
                                                conn.Close();
                                                conn = new MySql.Data.MySqlClient.MySqlConnection();
                                                conn.ConnectionString = myConnectionString;
                                                conn.Open();
                                                using (MySqlCommand cmd3 = new MySqlCommand("INSERT INTO usertimes(id,moduleId) VALUES (\"" + activity.From.Id.ToString() + "\"," + id + ")", conn))
                                                {
                                                    //cmd.Parameters.AddWithValue("?id", MySqlDbType.VarChar).Value = activity.From.Id.ToString();
                                                    //cmd.Parameters.AddWithValue("?moduleId", MySqlDbType.Int32).Value = id;
                                                    //cmd.CommandText = "INSERT INTO usertimes(id,moduleId) VALUES (\"" + activity.From.Id.ToString() + "\"," + id + ");";
                                                    cmd3.ExecuteNonQuery();
                                                    result = "You registered succesfully.";
                                                }
                                            }
                                            catch (Exception e3)
                                            {
                                                result = "SELECT * FROM usertimes";
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception();
                                        }
                                    }
                                    catch (Exception e2)
                                    {
                                        result = "You are already registered to this module.";
                                    }
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                            catch (Exception e1)
                            {
                                result = "Course not found.";
                            }

                        }
                        else if (message.Substring(0, message.IndexOf(" ")).Equals("remove"))
                        {
                            String courseCode = message.Substring(message.IndexOf(" ") + 1).Trim();
                            int id = -1;

                            try
                            {
                                using (MySqlCommand cmd1 = new MySqlCommand("SELECT id FROM modules WHERE code = \"" + courseCode + "\"", conn))
                                {
                                    var reader1 = cmd1.ExecuteReader();
                                    if (reader1.Read())
                                    {
                                        id = reader1.GetInt32(0);
                                    }
                                }

                                conn.Close();
                                conn = new MySql.Data.MySqlClient.MySqlConnection();
                                conn.ConnectionString = myConnectionString;
                                conn.Open();

                                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM usertimes WHERE id=\"" + activity.From.Id + "\" AND moduleId = " + id.ToString(), conn))
                                {
                                    cmd.ExecuteNonQuery();
                                    result = "Remove succesful";
                                }
                            }
                            catch (Exception e)
                            {
                                result = "Remove unsuccesful.";
                            }

                        }
                    }
                    else
                    {
                        if (message.Equals("help"))
                        {
                            Activity helpReply1 = activity.CreateReply($"Zetta bot guide:");
                            Activity helpReply2 = activity.CreateReply($"register <Course Code> - to register a course code on your account");
                            Activity helpReply3 = activity.CreateReply($"remove <Course Code> - to remove a course code on your account");
                            Activity helpReply4 = activity.CreateReply($"modules - shows all your course codes");
                            Activity helpReply5 = activity.CreateReply($"next - shows your next lecture");
                            Activity helpReply6 = activity.CreateReply($"map - shows your next lecture and gives you an image with the location of your next lecture");
                            Activity helpReply7 = activity.CreateReply($"Disclaimer: you can't have more than 10 course codes registered to your account.");
                            await connector.Conversations.ReplyToActivityAsync(helpReply1);
                            await connector.Conversations.ReplyToActivityAsync(helpReply2);
                            await connector.Conversations.ReplyToActivityAsync(helpReply3);
                            await connector.Conversations.ReplyToActivityAsync(helpReply4);
                            await connector.Conversations.ReplyToActivityAsync(helpReply5);
                            await connector.Conversations.ReplyToActivityAsync(helpReply6);
                            await connector.Conversations.ReplyToActivityAsync(helpReply7);

                        }
                        else if (message.Equals("next"))
                        {
                            try
                            {
                                using (MySqlCommand cmd = new MySqlCommand("SELECT modules.code, modules.name, timetable.starttime, timetable.location FROM timetable INNER JOIN modules ON (modules.id = timetable.moduleId) INNER JOIN usertimes ON (usertimes.moduleId = modules.id) WHERE timetable.starttime > NOW() AND usertimes.id = \"" + activity.From.Id + "\" ORDER BY timetable.starttime LIMIT 1", conn))
                                {
                                    //result = "SELECT modules.code, modules.name, timetable.starttime, timetable.location FROM timetable INNER JOIN modules ON (modules.id = timetable.moduleId) INNER JOIN usertimes ON (usertimes.moduleId = modules.id) WHERE timetable.starttime > NOW() AND usertimes.id = \"" + activity.From.Id + "\" ORDER BY timetable.starttime LIMIT 1";
                                    var nextReader = cmd.ExecuteReader();

                                    if (nextReader.Read())
                                    {
                                        result = nextReader.GetString(0) + " - " + nextReader.GetString(1) + ". Starting at: " + nextReader.GetDateTime(2).ToString() + ", location: " + nextReader.GetString(3);
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                result = "There was a problem. Please try again later.";
                            }
                        }else if (message.Equals("modules"))
                        {
                            try
                            {
                                using (MySqlCommand cmd = new MySqlCommand("SELECT modules.name, modules.code FROM modules INNER JOIN usertimes ON (modules.id = usertimes.moduleId) WHERE usertimes.id=\"" + activity.From.Id + "\"",conn))
                                {
                                    var showReader = cmd.ExecuteReader();
                                    
                                    while (showReader.Read())
                                    {
                                        String showResult = "";
                                        showResult += showReader.GetString(0) + " " + showReader.GetString(1);
                                        Activity showAnswer = activity.CreateReply(showResult);
                                        await connector.Conversations.ReplyToActivityAsync(showAnswer);
                                    }
                                    result = "<------------------------------->";
                                }
                            }catch(Exception e)
                            {
                                result = "You are not registered to any modules.";
                            }
                        }else if (message.Equals("update"))
                        {

                        }
                    }
                }catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply($"Can't connect to database {result}"));
                }

                Activity registerAnswer = activity.CreateReply(result);
                await connector.Conversations.ReplyToActivityAsync(registerAnswer);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}