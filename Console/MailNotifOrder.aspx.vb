Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Security.Cryptography
Partial Class Console_MailNotifOrder
     Inherits System.Web.UI.Page
    Dim publicCfg As New PublicConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Koneksi ke database
        Using connection As New SqlConnection(myConn)
            connection.Open()
            Using command As New SqlCommand("MailPushOrder", connection)
                command.CommandType = CommandType.StoredProcedure
                Try
                    Dim reader As SqlDataReader = command.ExecuteReader()
                    If reader.HasRows Then
                        While reader.Read()
                            Dim email As String = reader("Email").ToString()
                            ' Dim email As String = "duokstayl376@gmail.com"
                            Dim cc As String = "duokstayl376@gmail.com"
                            Dim del As String = reader("Delivery").ToString()
                            Dim orderno As String = reader("OrderNo").ToString()
                            Dim orderCust As String = reader("OrderCust").ToString()
                            Dim appId As String = reader("AppId").ToString()
                            '#cek & count day of created
                            Dim createdDate As DateTime
                            If DateTime.TryParse(reader("CreatedDate").ToString(), createdDate) Then
                                Dim currentDate As DateTime = DateTime.Now.Date ' Hanya bagian tanggal
                                Dim daysDifference As Integer = (currentDate - createdDate.Date).Days ' Hitung selisih hari
                                Dim CekMailPush As DataSet = publicCfg.GetListData("SELECT * FROM MailPushOrderDraft WHERE Email = '"+ reader("Email").ToString() +"' AND OrdId = '"+ reader("OrdId").ToString() +"' ")
                                '#SendEmail
                                If daysDifference < 3 Then
                                    If CekMailPush.Tables.Count > 0 AndAlso CekMailPush.Tables(0).Rows.Count < 3 Then
                                        InsertMailPushOrderDraft(reader("OrdId").ToString(),reader("Email").ToString(),reader("CreatedDate").ToString())
                                        ' Kirim email
                                        SendEmail(email, cc, del, orderno, orderCust, appId)
                                    End If
                                End If
                                '#delete OrderDraft after 10 days
                                If daysDifference > 10 Then
                                    deleteOrderDraft(reader("OrdId").ToString())
                                End If
                            Else
                                Console.WriteLine("Tanggal CreatedDate tidak valid.")
                            End If
                        End While
                    End If
                Catch ex As Exception
                    ' Log the error details for troubleshooting
                    Console.WriteLine("Error Query: " & ex.ToString())
                    ' Optionally re-throw with a more informative ToString()
                    Throw New Exception("Error Query: " & ex.ToString())
                End Try
            End Using
        End Using
    End Sub



    Public Sub InsertMailPushOrderDraft(OrdId As String, Email As String, DraftDate As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO MailPushOrderDraft (OrdId, Email, DraftDate) VALUES (@OrdId, @Email, @DraftDate)")
                myCmd.Parameters.AddWithValue("@OrdId", OrdId)
                myCmd.Parameters.AddWithValue("@Email", Email)
                myCmd.Parameters.AddWithValue("@DraftDate", DraftDate)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Sub deleteOrderDraft(OrdId As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Active=0 WHERE Id = @OrdId")
                myCmd.Parameters.AddWithValue("@OrdId", OrdId)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub



    Private Sub SendEmail(toEmail As String, cc As String, del As String, orderno As String, orderCust As String, appId As String)
        Try
            Dim mailData As DataSet = publicCfg.GetListData("SELECT * FROM MailConfiguration WHERE AppId = '" + UCase(appId).ToString() + "' AND Name = 'REMINDER ORDER DRAFT' AND Active=1")

        ' Periksa apakah dataset memiliki tabel dan baris
        If Not mailData.Tables.Count = 0 Then
            Dim mailRow As DataRow = mailData.Tables(0).Rows(0)
            Dim mailServer As String = mailRow.Item("Server").ToString()
            Dim mailAlias As String = mailRow.Item("Alias").ToString()
            Dim mailPort As Integer = Integer.Parse(mailRow.Item("Port").ToString())
            Dim mailHost As String = mailRow.Item("Host").ToString()
            Dim mailEnableSsl As Boolean = Boolean.Parse(mailRow.Item("EnableSsl").ToString())
            Dim mailCredentials As Boolean = Boolean.Parse(mailRow.Item("UseDefaultCredentials").ToString())
            Dim mailAccount As String = mailRow.Item("Account").ToString()
            Dim mailPassword As String = mailRow.Item("Password").ToString()
            Dim mailSubject As String = mailRow.Item("Subject").ToString()
            Dim mailTo As String = mailRow.Item("To").ToString()
            Dim mailCC As String = mailRow.Item("Cc").ToString()


            '#email content
            Dim mailBody As String = "Thank you for placing your order."
            mailBody += "<br>"
            mailBody += "You may have forgotten that you have entered your order, here we just want to remind you that you have not submitted your order with the following information:"
            mailBody += "<br>"
            mailBody += "<br /><br />"
            mailBody += "Store Order No : <b>" & orderno & "</b>"
            mailBody += "<br />"
            mailBody += "Store Customer : <b>" & orderCust & "</b>"
            mailBody += "<br />"
            mailBody += "Delivery / Pick Up : <b>" & del & "</b>"
            mailBody += "<br /><br />"
            mailBody += "<br>"
            mailBody += "Please note that all draft orders will be removed from the system if there are no activities after 10 days.<br>"
            mailBody += "please check at <a href='https://www.onlineorder.au'>onlineorder.au</a>"
            mailBody += "<br>"
            mailBody += "<br>"
            mailBody += "<br /><br />"
            mailBody += "Kind regards,"
            mailBody += "<br /><br />"

            mailBody += "<br /><br />"
            mailBody += "<b>Sunlight Products Pty Ltd</b>"

            Dim myMail As New MailMessage
            myMail.Subject = mailSubject
            myMail.From = New MailAddress(mailServer, mailAlias)
            ' myMail.To.Add(toEmail)
            myMail.To.Add(cc)
            ' myMail.CC.Add(mailCC)
            myMail.CC.Add(cc)
            myMail.Body = mailBody
            myMail.IsBodyHtml = True

            Dim smtpClient As New SmtpClient()
            smtpClient.Host = mailHost
            smtpClient.EnableSsl = mailEnableSsl
            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
            smtpClient.Port = mailPort
            smtpClient.Send(myMail)
        End If
        Catch ex As Exception
            ' Log the error details for troubleshooting
            Console.WriteLine("Error sending email: " & ex.ToString())
            ' Optionally re-throw with a more informative message
            Throw New Exception("Failed to send email notification: " & ex.ToString())
        End Try
    End Sub




End Class
