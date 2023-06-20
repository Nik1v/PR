using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using MimeKit;
using MailKit.Net.Pop3;
using System;

string username = "nichita.vornicov@isa.utm.md";
string password = "bmgflmhganndqran";

bool exit = false;

while (!exit)
{
    Console.WriteLine("\nMenu:");
    Console.WriteLine("     1. Enumerates the list of emails in the mailbox using the protocol POP3");
    Console.WriteLine("     2. Enumerates the list of emails in the mailbox using the protocol IMAP");
    Console.WriteLine("     3. Download an email with attachments");
    Console.WriteLine("     4. Send a text-only email");
    Console.WriteLine("     5. Send an email with an attachment");
    Console.WriteLine("     6. When sending, you can indicate the subject and reply-to details");
    Console.WriteLine("     0. Exit");

    Console.Write("\nEnter the desired option: ");
    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            pop3();
            break;

        case "2":
            imap();
            break;

        case "3":
            downloadEmail();
            break;

        case "4":
            emailText();
            break;

        case "5":
            email();
            break;

        case "6":
            reply();
            break;

        case "0":
            exit = true;
            break;

        default:
            Console.WriteLine("Invalid option. Try again.");
            break;
    }
}

void pop3()
{
    try
    {
        using (TcpClient tcpClient = new TcpClient("pop.gmail.com", 995))
        {
            using (SslStream sslStream = new SslStream(tcpClient.GetStream()))
            {
                sslStream.AuthenticateAsClient("pop.gmail.com");

                using (StreamReader reader = new StreamReader(sslStream, Encoding.ASCII))
                {
                    using (StreamWriter writer = new StreamWriter(sslStream, Encoding.ASCII))
                    {
                        string response = reader.ReadLine() ?? "";
                        Console.WriteLine(response);

                        // Login
                        writer.WriteLine("USER " + username);
                        writer.Flush();
                        response = reader.ReadLine() ?? "";
                        Console.WriteLine(response);

                        writer.WriteLine("PASS " + password);
                        writer.Flush();
                        response = reader.ReadLine() ?? "";
                        Console.WriteLine(response);

                        // Retrieve messages
                        writer.WriteLine("LIST");
                        writer.Flush();
                        response = reader.ReadLine() ?? "";
                        Console.WriteLine(response);

                        while ((response = reader.ReadLine() ?? "") != "." && response != null)
                        {
                            Console.WriteLine(response);
                        }

                        // Logout
                        writer.WriteLine("QUIT");
                        writer.Flush();
                        response = reader.ReadLine() ?? "";
                        Console.WriteLine(response);
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred: " + ex.Message);
    }
}

void imap()
{
    using (var client = new ImapClient())
    {
        client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
        client.Authenticate(username, password);

        var inbox = client.Inbox;
        inbox.Open(FolderAccess.ReadOnly);

        for (int i = 0; i < inbox.Count; i++)
        {
            var message = inbox.GetMessage(i);
            Console.WriteLine("Subject: " + message.Subject);
            Console.WriteLine("From: " + message.From);
            Console.WriteLine();
        }
    }
}

void downloadEmail()
{
    string downloadPath = "C:\\Users\\Nikita\\Downloads\\";
    var pop3Client = new Pop3Client();
    pop3Client.Connect("pop.gmail.com", 995, true);
    pop3Client.Authenticate(username, password);

    for (int i = 0; i < pop3Client.Count; i++)
    {
        var message = pop3Client.GetMessage(i);
        foreach (var attachment in message.Attachments)
        {
            var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
            using (var stream = File.Create(downloadPath + fileName))
            {
                if (attachment is MessagePart)
                {
                    var part = (MessagePart)attachment;

                    part.Message.WriteTo(stream);
                }
                else
                {
                    var part = (MimePart)attachment;

                    part.Content.DecodeTo(stream);
                }
            }
            Console.WriteLine("Downloaded successfully " + fileName);
        }
    }
}

void emailText()
{
    string toEmail;
    do
    {
        Console.Write("Enter the recipient's email address ");
        toEmail = Console.ReadLine() ?? "";

        if (!Regex.IsMatch(toEmail, @"^[^@\s]+@([a-zA-Z0-9]+\.)+[a-zA-Z]{2,}$"))
        {
            Console.WriteLine("The email address is invalid. Enter another address.");
            toEmail = "";
        }
    } while (toEmail == null);

    Console.Write("Enter the email subject ");
    string subject = Console.ReadLine() ?? "";

    Console.Write("Enter the email message ");
    string body = Console.ReadLine() ?? "";

    var smtpClient = new SmtpClient("smtp.gmail.com", 587);
    smtpClient.EnableSsl = true;
    smtpClient.UseDefaultCredentials = false;
    smtpClient.Credentials = new NetworkCredential(username, password);

    var message = new MailMessage(username, toEmail, subject, body);

    try
    {
        smtpClient.Send(message);
        Console.WriteLine("The email was sent successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error sending email " + ex.Message);
    }
}

void email()
{
    string toEmail;
    do
    {
        Console.Write("Enter the recipient's email address ");
        toEmail = Console.ReadLine() ?? "";

        if (!Regex.IsMatch(toEmail, @"^[^@\s]+@([a-zA-Z0-9]+\.)+[a-zA-Z]{2,}$"))
        {
            Console.WriteLine("The email address is invalid. Enter another address.");
            toEmail = "";
        }
    } while (toEmail == null);

    Console.Write("Enter the email subject ");
    string subject = Console.ReadLine() ?? "";

    Console.Write("Enter the email message ");
    string body = Console.ReadLine() ?? "";

    Console.Write("Enter the path to the attached file ");
    string attachmentPath = Console.ReadLine() ?? "";

    Attachment attachment = new Attachment(attachmentPath); // C:\\Users\\Nikita\\photo.jpeg

    MailMessage mailMessage = new MailMessage();
    mailMessage.From = new MailAddress(username);
    mailMessage.To.Add(toEmail);
    mailMessage.Subject = subject;
    mailMessage.Body = body;
    mailMessage.Attachments.Add(attachment);

    var smtpClient = new SmtpClient("smtp.gmail.com", 587);
    smtpClient.EnableSsl = true;
    smtpClient.UseDefaultCredentials = false;
    smtpClient.Credentials = new NetworkCredential(username, password);

    try
    {
        smtpClient.Send(mailMessage);
        Console.WriteLine("The email was sent successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error sending email " + ex.Message);
    }
}

void reply()
{
    string toEmail;
    do
    {
        Console.Write("Enter the recipient's email address ");
        toEmail = Console.ReadLine() ?? "";

        if (!Regex.IsMatch(toEmail, @"^[^@\s]+@([a-zA-Z0-9]+\.)+[a-zA-Z]{2,}$"))
        {
            Console.WriteLine("The email address is invalid. Enter another address.");
            toEmail = "";
        }
    } while (toEmail == null);

    string replyTo;
    do
    {
        Console.WriteLine("Enter your reply email address ");
        replyTo = Console.ReadLine() ?? "";

        if (!Regex.IsMatch(replyTo, @"^[^@\s]+@([a-zA-Z0-9]+\.)+[a-zA-Z]{2,}$"))
        {
            Console.WriteLine("The email address is invalid. Enter another address.");
            replyTo = "";
        }
    } while (replyTo == null);

    Console.Write("Enter the email subject ");
    string subject = Console.ReadLine() ?? "";

    Console.Write("Enter the email message ");
    string body = Console.ReadLine() ?? "";

    MailMessage message = new MailMessage();
    message.To.Add(toEmail);
    message.Subject = subject;
    message.Body = body;
    message.ReplyToList.Add(replyTo);

    var smtpClient = new SmtpClient("smtp.gmail.com", 587);
    smtpClient.EnableSsl = true;
    smtpClient.UseDefaultCredentials = false;
    smtpClient.Credentials = new NetworkCredential(username, password);

    try
    {
        smtpClient.Send(message);
        Console.WriteLine("The email was sent successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error sending email " + ex.Message);
    }
}