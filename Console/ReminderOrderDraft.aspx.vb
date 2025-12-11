Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Security.Cryptography
Partial Class Console_ReminderOrderDraft
    Inherits System.Web.UI.Page

    Dim publicCfg As New PublicConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        GetOrderDraft()

    End Sub

    Protected Sub GetOrderDraft()
        Try
            Dim orders As DataSet = publicCfg.GetListData("SELECT * FROM view_order_headers WHERE Active = '1' AND Status IN ( 'Draft', 'Unsubmitted' ) ORDER BY CreatedDate DESC")
            Dim dump As New Text.StringBuilder()
            
            For i As Integer = 0 To orders.Tables(0).Rows.Count - 1
                Dim row = orders.Tables(0).Rows(i)

                Dim Id As String = SafeGet(row, "Id")
                Dim OrderId As String = SafeGet(row, "OrderId")
                Dim CustomerId As String = SafeGet(row, "CustomerId")
                Dim OrderNumber As String = SafeGet(row, "OrderNumber")
                Dim OrderName As String = SafeGet(row, "OrderName")
                Dim OrderType As String = SafeGet(row, "OrderType")
                Dim Status As String = SafeGet(row, "Status")
                Dim Delivery As String = SafeGet(row, "Delivery")
                Dim CreatedBy As String = SafeGet(row, "CreatedBy")
                Dim CreatedDate As String = SafeGet(row, "CreatedDate")

                Dim ThisTime As DateTime
                If DateTime.TryParse(CreatedDate, ThisTime) Then
                    Dim CurrentDate As DateTime = DateTime.Now
                    Dim DaysDiff As Integer = (CurrentDate - ThisTime.Date).Days '#Hitung selisih hari
                    Dim Log_OrderDraft As DataSet = publicCfg.GetListData("SELECT * FROM Log_OrderDraft WHERE Id = '" & Id & "' AND OrderType = '" & OrderType & "'")

                    If DaysDiff < 3 Then
                        If Log_OrderDraft.Tables.Count > 0 AndAlso Log_OrderDraft.Tables(0).Rows.Count < 3 Then

                            ' DebugOrderDraft(Id, OrderNumber, OrderType, Status, CreatedBy, CreatedDate)
                            ' Exit Sub

                            '#Insert Log & Return Response
                            Dim ResponseLog As String = InsertLogOrderDraft(Id, OrderId, OrderType, CreatedDate)
                            Dim statusColor As String = If(ResponseLog = "200", "green", "red")
                            Dim statusText As String = If(ResponseLog = "200", "Success", "Failed")
                            dump.AppendLine("<div style='padding:6px 10px; border-radius:6px; margin-bottom:4px; font-weight:bold; color:" & statusColor & "; border:1px solid " & statusColor &";'>")
                            dump.AppendLine("Log Status: " & statusText & " (Code: " & ResponseLog & ")")
                            dump.AppendLine("</div>")


                            Dim ResponseMail As String = SendReminderMail(CustomerId, OrderNumber, OrderName, OrderType, Delivery, CreatedBy)
                            statusColor = If(ResponseMail = "200", "green", "red")
                            statusText = If(ResponseMail = "200", "Success", "Failed")
                            dump.AppendLine("<div style='padding:6px 10px; border-radius:6px; margin-bottom:4px; font-weight:bold; color:" & statusColor & "; border:1px solid " & statusColor &";'>")
                            dump.AppendLine("Mail Status: " & statusText & " (Code: " & ResponseMail & ")")
                            dump.AppendLine("</div>")
                        End If
                    End If

                    If DaysDiff > 10 Then
                        Dim ResponseDelete As String = DeleteOrderDraft(Id, OrderType)
                        Dim statusColor As String = If(ResponseDelete = "200", "green", "red")
                        Dim statusText As String =  If(ResponseDelete = "200", "Success", "Failed")
                        dump.AppendLine("<div style='padding:6px 10px; border-radius:6px; margin-bottom:4px; font-weight:bold; color:" & statusColor & "; border:1px solid " & statusColor &";'>")
                        dump.AppendLine("Delete Status: " & statusText & " (Code: " & ResponseDelete & ")")
                        dump.AppendLine("</div>")
                    End If
                Else
                    ltDump.Text = "<div style='color:red;'>Error: Tanggal CreatedDate tidak valid. </div>"
                End If
                

                
            Next

            ltDump.Text = dump.ToString()
        Catch ex As Exception
            ltDump.Text = "<div style='color:red;'>Error: " & ex.Message & "</div>"
        End Try
    End sub

    


    Public Function InsertLogOrderDraft(Id As String, OrderId As String, OrderType As String, CreatedDate As String) As String
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Log_OrderDraft (Id, OrderId, OrderType, DraftDate) VALUES (@Id, @OrderId, @OrderType, @DraftDate)")
                    myCmd.Parameters.AddWithValue("@Id", Id)
                    myCmd.Parameters.AddWithValue("@OrderId", OrderId)
                    myCmd.Parameters.AddWithValue("@OrderType", OrderType)
                    myCmd.Parameters.AddWithValue("@DraftDate", CreatedDate)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
            Return "200"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function


    Public Function SendReminderMail(CustomerId As String, OrderNumber As String, OrderName As String, OrderType As String, Delivery As String, CreatedBy As String) As String
        Try
            Dim Appid As String = publicCfg.GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(CreatedBy).ToString() + "'")
            Dim Mail As String = publicCfg.GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId = '" + CustomerId + "' AND [Primary] = 1")
            Mail = "miftah@bigblinds.co.id" '#For Testing

            Dim mailData As DataSet = publicCfg.GetListData("SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(Appid).ToString() + "' AND Name = 'Reminder Order Draft/Unsubmitted' AND Active=1")
            If mailData.Tables(0).Rows.Count = 0 Then Return "Mailing No Found"

            Dim mailServer As String = mailData.Tables(0).Rows(0)("Server").ToString()
            Dim mailHost As String = mailData.Tables(0).Rows(0)("Host").ToString()
            Dim mailPort As Integer = CInt(mailData.Tables(0).Rows(0)("Port"))
            Dim mailAccount As String = mailData.Tables(0).Rows(0)("Account").ToString()
            Dim mailPassword As String = mailData.Tables(0).Rows(0)("Password").ToString()
            Dim mailAlias As String = mailData.Tables(0).Rows(0)("Alias").ToString()
            Dim mailTo As String = mailData.Tables(0).Rows(0)("To").ToString()
            Dim mailCc As String = mailData.Tables(0).Rows(0)("Cc").ToString()
            Dim mailBcc As String = mailData.Tables(0).Rows(0)("Bcc").ToString()
            Dim mailNetworkCredentials As Boolean = CBool(mailData.Tables(0).Rows(0)("NetworkCredentials"))
            Dim mailDefaultCredentials As Boolean = CBool(mailData.Tables(0).Rows(0)("DefaultCredentials"))
            Dim mailEnableSSL As Boolean = CBool(mailData.Tables(0).Rows(0)("EnableSSL"))

            Dim mailBody As String = "Thank you for placing your order."
            mailBody += "<br>"
            mailBody += "You may have forgotten that you have entered your order, here we just want to remind you that you have not submitted your order with the following information:"
            mailBody += "<br>"
            mailBody += "<br /><br />"
            mailBody += "Store Order No : <b>" & OrderNumber & "</b>"
            mailBody += "<br />"
            mailBody += "Store Customer : <b>" & OrderName & "</b>"
            mailBody += "<br />"
            mailBody += "Order Type : <b>" & OrderType & "</b>"
            mailBody += "<br />"
            mailBody += "Delivery / Pick Up : <b>" & Delivery & "</b>"
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

            Using myMail As New MailMessage()
                myMail.Subject = "Reminder Order Draft"
                myMail.From = New MailAddress(mailServer, mailAlias)
                myMail.Body = mailBody
                myMail.IsBodyHtml = True

                myMail.To.Add(Mail)
                If Not String.IsNullOrEmpty(mailTo) Then myMail.To.Add(mailTo)
                If Not String.IsNullOrEmpty(mailCc) Then myMail.CC.Add(mailCc)
                If Not String.IsNullOrEmpty(mailBcc) Then myMail.Bcc.Add(mailBcc)

                Using smtpClient As New SmtpClient(mailHost, mailPort)
                    smtpClient.EnableSsl = mailEnableSSL
                    smtpClient.UseDefaultCredentials = mailDefaultCredentials
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network

                    If mailNetworkCredentials Then
                        smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
                    ElseIf mailDefaultCredentials Then
                        smtpClient.UseDefaultCredentials = True
                    Else
                        smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials
                    End If

                    smtpClient.Send(myMail)
                End Using

            End Using


            Return "200"
        Catch ex As Exception
            ' Tambahkan detail Inner Exception untuk debugging
            Dim errorMessage As String = "Failure sending mail. " & ex.Message
            If ex.InnerException IsNot Nothing Then
                errorMessage &= " Inner Exception: " & ex.InnerException.Message
            End If
            Return errorMessage
        End Try
    End Function


    Public Function DeleteOrderDraft(Id As String, OrderType As String) As String
        Try
            Dim query As String = "UPDATE OrderHeaders SET Active=0 WHERE Id = @Id"
            If Not OrderType = "Blinds" Then
                query = "UPDATE OrderHeaders_Shutters SET Active=0 WHERE Id = @Id"
            End If
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand(query)
                    myCmd.Parameters.AddWithValue("@Id", Id)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function


    Public Sub DebugOrderDraft(Id As String, OrderNumber As String, OrderType As String, Status As String, CreatedBy As String, CreatedDate As String)
        Dim dump As New Text.StringBuilder()
        dump.AppendLine("<div style='margin-bottom:10px;'>")
        dump.AppendLine("ID: " & Id & "<br>")
        dump.AppendLine("Order No: " & OrderNumber & "<br>")
        dump.AppendLine("Order Type: " & OrderType  & "<br>")
        dump.AppendLine("Status: " & Status  & "<br>")
        dump.AppendLine("CreatedBy: " & CreatedBy  & "<br>")
        dump.AppendLine("CreatedDate: " & CreatedDate  & "<br>")
        dump.AppendLine("</div>")
        ltDump.Text = dump.ToString()
    End Sub


    Private Function SafeGet(row As DataRow, column As String) As String
        If row.Table.Columns.Contains(column) AndAlso Not IsDBNull(row(column)) Then
            Return row(column).ToString()
        End If
        Return "<span style='color:red;'>(null)</span>"
    End Function


End Class
