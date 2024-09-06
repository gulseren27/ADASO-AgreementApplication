﻿namespace AgreementReminderService
{
    using System;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Mail;
    using System.ServiceProcess;
    using System.Timers;

    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        private string connectionString = "Server=DESKTOP-172N3BR\\SQLEXPRESS;Database=ADASO;Integrated Security=True;MultipleActiveResultSets=True;";

        public Service1()
        {
            InitializeComponent();
            timer = new Timer(24 * 60 * 60 * 1000);
            timer.Elapsed += Timer_Elapsed;
        }

        protected override void OnStart(string[] args)
        {
            timer.Start();
            Timer_Elapsed(this, null);
        }

        protected override void OnStop()
        {
            timer.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CheckAgreementsAndSendEmails();
            
        }

        private void CheckAgreementsAndSendEmails()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                    SELECT a.Title, a.Email, DATEDIFF(day, GETDATE(), a.EndDate) AS DaysLeft, a.ReminderSentFor, a.Content,
                           m.MailAdress, m.Name, m.Surname
                    FROM Agreementt a
                    LEFT JOIN Maill m ON a.Id = m.AId
                    WHERE DATEDIFF(day, GETDATE(), a.EndDate) <= 30";



                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string title = reader["Title"].ToString();
                        string email = reader["Email"].ToString();
                        string content = reader["Content"].ToString();
                        int daysLeft = Convert.ToInt32(reader["DaysLeft"]);
                        string reminderSentFor = reader["ReminderSentFor"].ToString();

                        string partyEmail = reader["MailAdress"].ToString();
                        string partyName = reader["Name"].ToString();
                        string partySurname = reader["Surname"].ToString();

                        if (daysLeft <= 30 && daysLeft > 15 && reminderSentFor != "1")
                        {
                            SendReminderEmail(email, title, daysLeft, content);
                            UpdateReminderStatus(conn, title, "1", content);
                        }
                        else if (daysLeft <= 15 && daysLeft > 7 && reminderSentFor != "2")
                        {
                            SendReminderEmail(email, title, daysLeft, content);
                            UpdateReminderStatus(conn, title, "2", content);
                        }
                        else if (daysLeft <= 7 && daysLeft >= 1 && reminderSentFor != "3")
                        {
                            SendReminderEmail(email, title, daysLeft, content);
                            UpdateReminderStatus(conn, title, "3", content);
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Error checking agreements: " + ex.Message, EventLogEntryType.Error);
            }
        }

        private void UpdateReminderStatus(SqlConnection conn, string title, string reminderFor, string content)
        {
            string updateQuery = @"
            UPDATE Agreementt
            SET ReminderSentFor = @ReminderFor
            WHERE Title = @Title";

            SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@ReminderFor", reminderFor);
            updateCmd.Parameters.AddWithValue("@Title", title);
            updateCmd.Parameters.AddWithValue("@Content", content);

            updateCmd.ExecuteNonQuery();
        }

        private void SendReminderEmail(string recipientEmail, string title, int daysLeft, string content)
        {
            string emailBody = $@"
<html lang=""tr"">
<head>
    <meta charset=""utf-8"" />
    <style type=""text/css"">
        .rollover:hover.rollover-first {{
            max-height: 0px!important;
            display: none!important;
        }}
        .rollover:hover.rollover-second {{
            max-height: none!important;
            display: block!important;
        }}
        .rollover span {{
            font-size: 0px;
        }}
        u + .body img ~div div {{
            display: none;
        }}
        #outlook a {{
            padding: 0;
        }}
        span.MsoHyperlink,
        span.MsoHyperlinkFollowed {{
            color: inherit;
        }}
        a.es-button {{
            text-decoration: none!important;
        }}
        a[x-apple-data-detectors],
        #MessageViewBody a {{
            color: inherit!important;
            text-decoration: none!important;
            font-size: inherit!important;
            font-family: inherit!important;
            font-weight: inherit!important;
            line-height: inherit!important;
        }}
        .es-desk-hidden {{
            display: none;
            float: left;
            overflow: hidden;
            width: 0;
            max-height: 0;
            line-height: 0;
        }}
        .es-button-border:hover {{
            border-color: #3d5ca3 !important;
            background: #ffffff !important;
        }}
        .es-button-border:hover a.es-button,
        .es-button-border:hover button.es-button {{
            background: #ffffff !important;
        }}
        @media only screen and (max-width:600px) {{
            .es-m-p20b {{
                padding-bottom: 20px!important;
            }}
            .es-m-p0l {{
                padding-left: 0px!important;
            }}
            .es-m-txt-c {{
                text-align: center!important;
            }}
            a.es-button, button.es-button {{
                font-size: 14px!important;
                padding: 10px 20px!important;
                line-height: 120% !important;
            }}
            .es-header-body, .es-content-body, .es-footer-body {{
                max-width: 600px!important;
            }}
            .adapt-img {{
                width: 100% !important;
                height: auto!important;
            }}
            .es-mobile-hidden, .es-hidden {{
                display: none!important;
            }}
            .es-desk-hidden {{
                width: auto!important;
                overflow: visible!important;
                float: none!important;
                max-height: inherit!important;
                line-height: inherit!important;
            }}
        }}
        @media screen and (max-width:384px) {{
            .mail-message-content {{
                width: 414px!important;
            }}
        }}
    </style>
</head>
<body style=""width:100%;height:100%;padding:0;margin:0;background-color:#FAFAFA"">
    <div class=""es-wrapper-color"" style=""background-color:#FAFAFA"">
        <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""width:100%;height:100%;background-color:#FAFAFA"">
            <tr>
                <td valign=""top"" style=""padding:0;margin:0"">
                    <table align=""center"" style=""background-color:transparent;width:600px;"">
                        <tr>
                            <td align=""center"" style=""padding:20px;background-color:#3d5ca3"">
                                <img src=""http://www.adaso.org.tr/Content/Files/fileMenager/2022/2//ADASO_Logo_Alternatif_1.png"" alt=""ADASO Logo"" width=""183"" style=""display:block;border:0;"" />
                            </td>
                        </tr>
                    </table>
                    <table align=""center"" style=""background-color:#fafafa;width:600px;"">
                        <tr>
                            <td align=""center"" style=""background-color:#ffffff;padding:40px 20px 0;"">
                                <img src=""https://w7.pngwing.com/pngs/989/91/png-transparent-scalable-graphics-ico-icon-alarm-clock-electronics-digital-clock-fire-alarm-thumbnail.png"" alt=""Icon"" width=""175"" style=""display:block;border:0;"" />
                                <h1 style=""font-family:arial, 'helvetica neue', helvetica, sans-serif;font-size:20px;line-height:24px;color:#333333;""><strong>SÖZLEŞME BİTİŞ HATIRLATMASI</strong></h1>
                                <p style=""font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:24px;color:#666666;font-size:16px;text-align:center;"">Merhaba,<b> {title}</b> başlıklı <b>{content} </b> konulu  sözleşmenizin bitmesine <b> {daysLeft} </b> gün kaldığını hatırlatır , iyi günler dileriz. </p>
                                <p style=""font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:24px;color:#666666;font-size:16px;text-align:center;"">Sözleşmeyi yenilemek için aşağıdaki butona tıklayın!</p>
                               <span class=""es-button border"">
                                    <button>
                                        <a href=""https://localhost:44302/Agreement"" class=""font-family: helvetica, 'helvetica neue', arial, verdana, sans-serif;
                                        -webkit-text-size-adjust: 100%;
                                        border-spacing: 0px;
                                        text-align: -webkit-center;
                                        scrollbar-width: thin;
                                        scrollbar-color: #888 transparent;
                                        Margin: 0;
                                        z-index: 9999;
                                        user-select: none;
                                        padding-left: 10px;
                                        padding-right: 10px;
                                        padding-top: 40px;
                                        padding-bottom: 40px;
                                        position: relative;
                                        border-collapse: separate;
                                        display: table-cell;
                                        outline: 2px solid #666666 !important;"" target=""_blank"">Sözleşme Yenile</a>
                                    </button>
                                </span>
</td>
                        </tr>
  <tr>
                            <td align=""center"" style=""padding:20px;background-color:#3d5ca3"">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
              
        </table>
    </div>
</body>
</html>";
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("adasoexample@gmail.com");
                mail.To.Add(recipientEmail);
                mail.Subject = "SÖZLEŞME BİTİŞ HATIRLATMASI";
                mail.IsBodyHtml = true;

                


                mail.Body = emailBody;
                smtpServer.Port = 587;
                smtpServer.Credentials = new NetworkCredential("adasoexample@gmail.com", "bcqh zxtl wioc bzsb");
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
                EventLog.WriteEntry($"Reminder email sent to {recipientEmail}.", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Error sending reminder email: " + ex.Message, EventLogEntryType.Error);
            }


        }
        }

       
    }
