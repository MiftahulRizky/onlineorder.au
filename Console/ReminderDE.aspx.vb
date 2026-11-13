Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Security.Cryptography
Partial Class Console_ReminderDE
    Inherits System.Web.UI.Page

    Dim publicCfg As New PublicConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        FindOrders()

    End Sub

    Protected Sub FindOrders()
        Try
            Dim dump As New Text.StringBuilder()
            Dim orders As DataSet = publicCfg.GetListData("SELECT * FROM view_order_headers WHERE Active = '1' AND OrderType IN ('Blinds', 'Door and Window', 'Furnishings') AND Status = 'New Order' AND SubmittedDate <= DATEADD(DAY, -4, GETDATE()) AND DATEDIFF(DAY, SubmittedDate, GETDATE()) - (DATEDIFF(WEEK, SubmittedDate, GETDATE()) * 2) - CASE WHEN DATEPART(WEEKDAY, SubmittedDate) = 1 THEN 1 ELSE 0 END - CASE WHEN DATEPART(WEEKDAY, GETDATE()) = 7 THEN 1 ELSE 0 END >= 4 ORDER BY CreatedDate DESC")
            Dim ordersCount As Integer = orders.Tables(0).Rows.Count

            If ordersCount > 0 Then
                Dim Res As Object = SendReminderMail()
                If Res.error Then 
                    dump.AppendLine("<div style='padding:6px 10px; border-radius:6px; margin-bottom:4px; font-weight:bold; color:red; border:1px solid red;'>")
                    dump.AppendLine("Mail Status: " & Res.message)
                    dump.AppendLine("</div>")
                Else
                    dump.AppendLine("<div style='padding:6px 10px; border-radius:6px; margin-bottom:4px; font-weight:bold; color:green; border:1px solid green;'>")
                    dump.AppendLine(String.Format("Mail Status: {0} | Order Count: {1}", Res.message, ordersCount))
                    dump.AppendLine("</div>")
                End If
            Else
                dump.AppendLine("<div style='padding:6px 10px; border-radius:6px; margin-bottom:4px; font-weight:bold; color: blue; border:1px solid blue;'>")
                dump.AppendLine("Nothing order new on > 4 days.")
                dump.AppendLine("</div>")
            End If


            ltDump.Text = dump.ToString()
        Catch ex As Exception
            ltDump.Text = "<div style='color:red;'>Error: " & ex.Message & "</div>"
        End Try
    End Sub

    Public Function SendReminderMail() As Object
        Try
            Dim Appid As String = "B0165D5B-5B57-499C-8400-63720E216C44" '#Application Sunlight Product
            Dim Mail As String = "wijayayudi@jpm-trading.com,deslina@springfordtrading.com,bayu@jpm-trading.com,pijen@springfordtrading.com,de3@rimbabr.com,miftah@bigblinds.co.id"
            ' Mail = "miftah@bigblinds.co.id"

            Dim mailData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM Mailings WHERE ApplicationId = '{0}' AND Name = 'Reminder Order Draft/Unsubmitted' AND Active=1", Appid))
            If mailData.Tables(0).Rows.Count = 0 Then Throw New Exception("Mailings No Found")
            
            Dim mailDevelopment As DataSet = publicCfg.GetListData("SELECT * From MailConfiguration WHERE Id='FADBA62C-2072-4501-8901-5E071BBF5E67'")

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

            Dim orders As DataSet = publicCfg.GetListData("SELECT * FROM view_order_headers WHERE Active = '1' AND OrderType IN ('Blinds', 'Door and Window', 'Furnishings') AND Status = 'New Order' AND SubmittedDate <= DATEADD(DAY, -4, GETDATE()) AND DATEDIFF(DAY, SubmittedDate, GETDATE()) - (DATEDIFF(WEEK, SubmittedDate, GETDATE()) * 2) - CASE WHEN DATEPART(WEEKDAY, SubmittedDate) = 1 THEN 1 ELSE 0 END - CASE WHEN DATEPART(WEEKDAY, GETDATE()) = 7 THEN 1 ELSE 0 END >= 4 ORDER BY CreatedDate DESC")
            Dim ordersCount As Integer = orders.Tables(0).Rows.Count

            Dim mailBody As New System.Text.StringBuilder()
            mailBody.Append("Hi DE,")
            mailBody.Append("<br><br>")
            mailBody.Append("Could you please check the orders listed below that have not yet been processed, even though they were submitted more than 4 working days ago?")
            mailBody.Append("<br><br>")

            ' Create HTML Table
            mailBody.Append("<table style='width:100%; border-collapse:collapse; font-family:Arial,sans-serif; font-size:12px;'>")

            ' Table Header
            mailBody.Append("<thead>")
            mailBody.Append("<tr style='background-color:#343a40; color:#ffffff;'>")

            Dim headers As String() = {"No", "OrderId", "OrderNumber", "OrderName", "OrderType", "Status", "Delivery", "CreatedBy", "CreatedDate", "SubmittedDate"}

            For Each header As String In headers
                mailBody.Append("<th style='border:1px solid #dee2e6; padding:10px; text-align:left; white-space:nowrap;'>")
                mailBody.Append(header)
                mailBody.Append("</th>")
            Next

            mailBody.Append("</tr>")
            mailBody.Append("</thead>")

            ' Table Body
            mailBody.Append("<tbody>")

            If ordersCount > 0 Then
                For i As Integer = 0 To ordersCount - 1
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
                    Dim SubmittedDate As String = SafeGet(row, "SubmittedDate")
                    Dim CreatedByName As String = publicCfg.GetItemData(String.Format("SELECT FullName FROM CustomerLogins WHERE Id = '{0}'", UCase(CreatedBy).ToString()))

                    Dim Created As String = If(IsDBNull(CreatedDate), "", Convert.ToDateTime(CreatedDate).ToString("dd MMM yyyy"))
                    Dim Submitted As String = If(IsDBNull(SubmittedDate), "", Convert.ToDateTime(SubmittedDate).ToString("dd MMM yyyy"))

                    Dim bgColor As String = If(i Mod 2 = 0, "#ffffff", "#f8f9fa")

                    mailBody.Append("<tr style='background-color:" & bgColor & ";'>")

                    Dim values As String() = {
                        (i + 1).ToString(),
                        OrderId,
                        OrderNumber,
                        OrderName,
                        OrderType,
                        Status,
                        Delivery,
                        CreatedByName,
                        Created,
                        Submitted
                    }

                    For Each value As String In values
                        mailBody.Append("<td style='border:1px solid #dee2e6; padding:8px; white-space:nowrap;'>")
                        mailBody.Append(System.Web.HttpUtility.HtmlEncode(value))
                        mailBody.Append("</td>")
                    Next

                    mailBody.Append("</tr>")

                Next
            Else
                mailBody.Append("<tr>")
                mailBody.Append("<td colspan='10' style='border:1px solid #dee2e6; padding:12px; text-align:center;'>")
                mailBody.Append("No orders found.")
                mailBody.Append("</td>")
                mailBody.Append("</tr>")
            End If

            mailBody.Append("</tbody>")
            mailBody.Append("</table>")

            mailBody.Append("<br><br>")
            mailBody.Append("Please check at <a href='https://www.onlineorder.au'>onlineorder.au</a>")
            mailBody.Append("<br><br>")

            Dim finalMailBody As String = mailBody.ToString()


            Using myMail As New MailMessage()
                myMail.Subject = "Reminder Prosessing Order"
                myMail.From = New MailAddress(mailServer, mailAlias)
                myMail.Body = finalMailBody
                myMail.IsBodyHtml = True

                If mailDevelopment.Tables.Count > 0 Then
                    Dim mDev As String = mailDevelopment.Tables(0).Rows(0).Item("To").ToString()
                    Dim activeDev As String = mailDevelopment.Tables(0).Rows(0).Item("Active").ToString()

                    If activeDev = "True" Or activeDev = "1" Then
                        myMail.To.Add(mdev)
                    Else
                        myMail.To.Add(Mail)
                        If Not String.IsNullOrEmpty(mailTo) Then myMail.To.Add(mailTo)
                        If Not String.IsNullOrEmpty(mailCc) Then myMail.CC.Add(mailCc)
                        If Not String.IsNullOrEmpty(mailBcc) Then myMail.Bcc.Add(mailBcc)
                    End If
                Else
                    myMail.To.Add(Mail)
                    If Not String.IsNullOrEmpty(mailTo) Then myMail.To.Add(mailTo)
                    If Not String.IsNullOrEmpty(mailCc) Then myMail.CC.Add(mailCc)
                    If Not String.IsNullOrEmpty(mailBcc) Then myMail.Bcc.Add(mailBcc)
                End If

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


            Return New With { .error = false, .success = true, .message = "Success sending mail."}
        Catch ex As Exception
            Dim errorMessage As String = "Failure sending mail. " & ex.Message
            If ex.InnerException IsNot Nothing Then
                errorMessage &= " Inner Exception: " & ex.InnerException.Message
            End If
            Return New With { .error = true, .message = errorMessage}
        End Try
    End Function

    Private Function SafeGet(row As DataRow, column As String) As String
        If row.Table.Columns.Contains(column) AndAlso Not IsDBNull(row(column)) Then
            Return row(column).ToString()
        End If
        ' Return "<span style='color:red;'>(null)</span>"
        Return ""
    End Function

    Private Function CalculateWorkingDays(startDate As DateTime, endDate As DateTime) As Integer
        Dim totalDays As Integer = 0
        Dim currentDate As DateTime = startDate.Date
        
        While currentDate <= endDate.Date
            ' Cek apakah hari bukan Sabtu (DayOfWeek.Saturday = 6) dan bukan Minggu (DayOfWeek.Sunday = 0)
            If currentDate.DayOfWeek <> DayOfWeek.Saturday AndAlso currentDate.DayOfWeek <> DayOfWeek.Sunday Then
                totalDays += 1
            End If
            currentDate = currentDate.AddDays(1)
        End While
        
        Return totalDays
    End Function


End Class
