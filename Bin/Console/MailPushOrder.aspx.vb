Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Security.Cryptography
Partial Class Console_MailPushOrder
    Inherits System.Web.UI.Page
    Dim PublicConfig As New PublicConfig
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Koneksi ke database
        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
        PublicConfig.MailError("9E15AD5B-93D4-4021-96D9-27C5AFD612B4","","","")
        exit Sub

        Using connection As New SqlConnection(myConn)
            connection.Open()
            Using command As New SqlCommand("MailPushOrder", connection)
                command.CommandType = CommandType.StoredProcedure
                Try
                    Dim reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        ' Dim email As String = reader("Email").ToString()
                        Dim email As String = "duokstayl376@gmail.com"
                        Dim orderId As String = reader("OrdId").ToString()
                        Dim orderno As String = reader("OrderNo").ToString()
                        Dim orderCust As String = reader("OrderCust").ToString()
                        Dim appId As String = reader("AppId").ToString()
                        ' Kirim email
                        SendEmail(email, orderId, orderno, orderCust, appId)
                    End While
                Catch ex As Exception
                    ' Log the error details for troubleshooting
                    Console.WriteLine("Error Query: " & ex.ToString())
                    ' Optionally re-throw with a more informative ToString()
                    Throw New Exception("Error Query: " & ex.ToString())
                End Try
            End Using
        End Using
    End Sub



    Private Sub SendEmail(toEmail As String, orderId As String, orderno As String, orderCust As String, appId As String)
        Try
            Dim mail As New MailMessage()
            mail.From = New MailAddress("noreply@onlineorder.au")
            mail.To.Add(toEmail)  ' Use the provided recipient email here
            mail.Subject = "Reminder Order Draft"
            mail.Body = "Halo, order Anda dengan ID " & orderId & " dengan nomor " & orderno & " untuk pelanggan " & orderCust & " masih dalam status draft. Silakan selesaikan order Anda."

            ' Replace with your email provider's SMTP server details
            Dim smtpClient As New SmtpClient("127.0.0.1", 25)
            smtpClient.Credentials = New System.Net.NetworkCredential("noreply@onlineorder.au", "Kp56~g8v2")
            smtpClient.EnableSsl = False
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            smtpClient.UseDefaultCredentials = True
            smtpClient.Timeout = 100000 ' Set timeout ke 100 detik (dalam milidetik)
            smtpClient.Send(mail)
        Catch ex As Exception
            ' Log the error details for troubleshooting
            Console.WriteLine("Error sending email: " & ex.ToString())
            ' Optionally re-throw with a more informative message
            Throw New Exception("Failed to send email notification: " & ex.ToString())
        End Try
    End Sub

End Class
