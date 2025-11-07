Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail

Public Class MailConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetListData(thisString As String) As DataSet
        Dim thisCmd As New SqlCommand(thisString)
        Using thisConn As New SqlConnection(myConn)
            Using thisAdapter As New SqlDataAdapter()
                thisCmd.Connection = thisConn
                thisAdapter.SelectCommand = thisCmd
                Using thisDataSet As New DataSet()
                    thisAdapter.Fill(thisDataSet)
                    Return thisDataSet
                End Using
            End Using
        End Using
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetItemData_Boolean(thisString As String) As Boolean
        Dim result As Boolean = False
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Sub MailSubmit(HeaderId As String, FileDirectory As String, FileName As String)
        Dim fullPath As String = Path.Combine(FileDirectory, FileName)
        Dim myMail As MailMessage = Nothing
        Dim attachment As Attachment = Nothing

        Try
            Dim myData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + HeaderId + "'")
            If myData.Tables(0).Rows.Count = 0 Then Exit Sub

            Dim createdBy As String = myData.Tables(0).Rows(0).Item("CreatedBy").ToString()
            Dim customerId As String = myData.Tables(0).Rows(0).Item("CustomerId").ToString()
            Dim orderId As String = myData.Tables(0).Rows(0).Item("OrderId").ToString()
            Dim orderNumber As String = myData.Tables(0).Rows(0).Item("OrderNumber").ToString()
            Dim orderName As String = myData.Tables(0).Rows(0).Item("OrderName").ToString()
            Dim orderNote As String = myData.Tables(0).Rows(0).Item("OrderNote").ToString()
            Dim orderType As String = myData.Tables(0).Rows(0).Item("OrderType").ToString()
            Dim approved As Integer = myData.Tables(0).Rows(0).Item("Approved")

            Dim customerName As String = GetItemData("SELECT Name FROM Customers WHERE Id = '" + customerId + "'")
            Dim customerCashSale As Boolean = GetItemData_Boolean("SELECT CashSale FROM Customers WHERE Id = '" + customerId + "'")
            Dim production As String = GetItemData("SELECT TOP 1 Production FROM OrderDetails WHERE HeaderId = '" + HeaderId + "' AND Active = 1 ORDER BY Id ASC")

            Dim product As String = If(orderType = "Blinds", orderType & " - " & production, orderType)
            Dim appId As String = GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(createdBy).ToString() + "'")

            Dim queryMailings As String =
            If(orderType = "Panorama" OrElse orderType = "Evolve",
               "SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId).ToString() + "' AND Name = 'New Order Shutters' AND Active = 1",
               "SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId).ToString() + "' AND Name = 'New Order' AND Active = 1")

            If customerId = "DEFAULT" Then Exit Sub

            Dim mailData As DataSet = GetListData(queryMailings)
            If mailData.Tables(0).Rows.Count = 0 Then Exit Sub

            Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
            Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
            Dim mailPort As Integer = mailData.Tables(0).Rows(0).Item("Port")
            Dim mailAccount As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
            Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()
            Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
            Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
            Dim mailCc As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()
            Dim mailBcc As String = mailData.Tables(0).Rows(0).Item("Bcc").ToString()
            Dim mailNetworkCredentials As Boolean = mailData.Tables(0).Rows(0).Item("NetworkCredentials")
            Dim mailDefaultCredentials As Boolean = mailData.Tables(0).Rows(0).Item("DefaultCredentials")
            Dim mailEnableSSL As Boolean = mailData.Tables(0).Rows(0).Item("EnableSSL")

            ' Isi konten email
            Dim mailContent As String =
            "<span style='font-family: Lucida Sans Unicode, Times New Roman, sans-serif; font-size: 14px;'>A new order has been submitted by customer.</span>"

            If customerCashSale Then
                mailContent &= "<br /><br />Hi Team,<br />This is a <b><u>CASH SALE</u></b> customer.<br />Please send a <b><u>DEPOSIT REQUEST</u></b>."
            End If

            If approved = 2 Then
                mailContent &= "<br /><b><u>Out of Spec - Authorisation Needed</u></b><br /><br />Hi Norika & Galih,<br />Please check and discuss with customer."
            End If

            mailContent &= "<br /><br /><table cellpadding='3' cellspacing='0' style='font-size: 14px;'>"
            If orderType = "RBR" OrElse orderType = "Blinds" OrElse orderType = "Curtain" OrElse orderType = "Veri Shades" OrElse orderType = "Zebra Blinds" Then
                mailContent &= "<tr><td>Company</td><td><b>Lifestyle Blinds & Shutters</b></td></tr>"
            ElseIf orderType = "Panorama" OrElse orderType = "Evolve" Then
                mailContent &= "<tr><td>Company</td><td><b>Sunlight Products</b></td></tr>"
            End If

            mailContent &= "<tr><td>Retailer Name</td><td><b>" & customerName & "</b></td></tr>"
            mailContent &= "<tr><td>Order #</td><td><b>" & orderId & "</b></td></tr>"
            mailContent &= "<tr><td>Retailer Order</td><td><b>" & orderNumber & "</b></td></tr>"
            mailContent &= "<tr><td>Customer Name</td><td><b>" & orderName & "</b></td></tr>"
            mailContent &= "<tr><td>Product Type</td><td><b>" & product & "</b></td></tr>"
            mailContent &= "<tr><td>Order Note</td><td><b>" & orderNote & "</b></td></tr>"
            mailContent &= "</table><br />Please login to online ordering website to process it further.<br /><br /><br />Kind Regards,<br />Reza Andika Pratama"

            ' Siapkan MailMessage
            myMail = New MailMessage()
            If Not String.IsNullOrEmpty(mailTo) Then
                For Each thisMail In mailTo.Split(";"c)
                    myMail.To.Add(thisMail)
                Next
            Else
                myMail.To.Add("reza@bigblinds.co.id")
            End If

            If Not String.IsNullOrEmpty(mailCc) Then
                For Each thisMail In mailCc.Split(";"c)
                    myMail.CC.Add(thisMail)
                Next
            End If

            If Not String.IsNullOrEmpty(mailBcc) Then
                For Each thisMail In mailBcc.Split(";"c)
                    myMail.Bcc.Add(thisMail)
                Next
            End If

            myMail.Subject = String.Format("{0} - {1} - {2} - New Order #{3}", customerName, orderName, orderNumber, orderId)
            myMail.From = New MailAddress(mailServer, mailAlias)
            myMail.Body = mailContent
            myMail.IsBodyHtml = True
            Dim smtpClient As New SmtpClient() With {
                .Host = mailHost,
                .EnableSsl = mailEnableSSL,
                .UseDefaultCredentials = mailDefaultCredentials,
                .DeliveryMethod = SmtpDeliveryMethod.Network,
                .Port = mailPort
            }
            If mailNetworkCredentials Then
                smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
            ElseIf mailDefaultCredentials Then
                smtpClient.UseDefaultCredentials = True
            Else
                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials
            End If

            If File.Exists(fullPath) Then
                attachment = New Attachment(fullPath)
                myMail.Attachments.Add(attachment)
            End If

            smtpClient.Send(myMail)

        Catch ex As Exception
        Finally
            If myMail IsNot Nothing Then
                myMail.Attachments.Clear()
                myMail.Dispose()
            End If
            If attachment IsNot Nothing Then
                attachment.Dispose()
            End If
            If File.Exists(fullPath) Then
                Try
                    File.Delete(fullPath)
                Catch
                End Try
            End If
        End Try
    End Sub

    Public Sub MailDeposit(Id As String, FilePDF As String, StrTo As String, StrCC As String)
        Dim fs As FileStream = Nothing
        Try
            Dim myData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" & Id & "'")
            If myData.Tables(0).Rows.Count = 0 Then Exit Sub

            Dim orderId As String = myData.Tables(0).Rows(0)("OrderId").ToString()
            Dim createdBy As String = myData.Tables(0).Rows(0)("CreatedBy").ToString()
            Dim orderType As String = myData.Tables(0).Rows(0)("OrderType").ToString()

            Dim appId As String = GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" & UCase(createdBy) & "'")

            Dim queryMailings As String = "SELECT * FROM Mailings WHERE ApplicationId = '" & UCase(appId) & "' AND Name = 'Deposit Request' AND Active = 1"
            If orderType = "Panorama" OrElse orderType = "Evolve" Then
                queryMailings = "SELECT * FROM Mailings WHERE ApplicationId = '" & UCase(appId) & "' AND Name = 'Deposit Request Shutters' AND Active = 1"
            End If

            Dim mailData As DataSet = GetListData(queryMailings)
            If mailData.Tables(0).Rows.Count = 0 Then Exit Sub

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

            Dim mailContent As New StringBuilder()
            mailContent.AppendLine("<span style='font-family: Lucida Sans Unicode, sans-serif; font-size: 14px;'>Thank you for your order, please see attached Deposit Request Information.</span>")
            mailContent.AppendLine("<br /><br />A minimum deposit of 40% is required prior to your job starting production.<br /><br />")
            mailContent.AppendLine("<b><u>Direct Deposit Details</u></b><br />")

            If orderType = "RBR" OrElse orderType = "Blinds" OrElse orderType = "Curtain" OrElse orderType = "Veri Shades" OrElse orderType = "Zebra Blinds" Then
                mailContent.AppendLine("BSB: 084-209<br />")
                mailContent.AppendLine("ACC No: 299 226 433<br />")
                mailContent.AppendLine("Account Name: Lifestyle Blinds and Shutters Unit Trust<br /><br />")
                mailContent.AppendLine("Please email the remittance of all payments made to <a href='mailto:accountsrec@lifestyleshutters.com.au'>accountsrec@lifestyleshutters.com.au</a> with your order number as the reference.<br /><br />")
            End If

            If orderType = "Panorama" OrElse orderType = "Evolve" Then
                mailContent.AppendLine("BSB: 084-209<br />")
                mailContent.AppendLine("ACC No: 32 893 1386<br />")
                mailContent.AppendLine("Account Name: Sunlight Products Unit Trust<br /><br />")
                mailContent.AppendLine("Please email the remittance of all payments made to <a href='mailto:accountreceivable@sunlight.com.au'>accountreceivable@sunlight.com.au</a> with your order number as the reference.<br /><br />")
            End If

            If orderType = "RBR" OrElse orderType = "Blinds" OrElse orderType = "Curtain" OrElse orderType = "Veri Shades" OrElse orderType = "Zebra Blinds" Then
                mailContent.AppendLine("<b>Kind Regards,<br /><br />Customer Service<br />Lifestyle Blinds & Shutters</b>")
            End If
            If orderType = "Panorama" OrElse orderType = "Evolve" Then
                mailContent.AppendLine("<b>Kind Regards,<br /><br />Customer Service<br />Sunlight Products</b>")
            End If

            Using myMail As New MailMessage()
                myMail.Subject = "Deposit Request - " & orderId
                myMail.From = New MailAddress(mailServer, mailAlias)
                myMail.Body = mailContent.ToString()
                myMail.IsBodyHtml = True

                If Not String.IsNullOrEmpty(StrTo) AndAlso IsValidEmail(StrTo) Then
                    myMail.To.Add(StrTo)
                ElseIf Not String.IsNullOrEmpty(mailTo) Then
                    For Each thisMail In mailTo.Split(";"c)
                        If IsValidEmail(thisMail.Trim()) Then myMail.To.Add(thisMail.Trim())
                    Next
                Else
                    myMail.To.Add("reza@bigblinds.co.id")
                End If

                If Not String.IsNullOrEmpty(mailCc) Then
                    For Each thisMail In mailCc.Split(";"c)
                        If IsValidEmail(thisMail.Trim()) Then myMail.CC.Add(thisMail.Trim())
                    Next
                End If

                If Not String.IsNullOrEmpty(StrCC) AndAlso IsValidEmail(StrCC) Then
                    myMail.CC.Add(StrCC)
                End If

                If Not String.IsNullOrEmpty(mailBcc) Then
                    For Each thisMail In mailBcc.Split(";"c)
                        If IsValidEmail(thisMail.Trim()) Then myMail.Bcc.Add(thisMail.Trim())
                    Next
                End If

                If File.Exists(FilePDF) Then
                    fs = New FileStream(FilePDF, FileMode.Open, FileAccess.Read)
                    Dim attach As New Attachment(fs, Path.GetFileName(FilePDF))
                    myMail.Attachments.Add(attach)
                Else
                    Throw New FileNotFoundException("File PDF tidak ditemukan: " & FilePDF)
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

            Threading.Thread.Sleep(500)

            If File.Exists(FilePDF) Then File.Delete(FilePDF)

        Catch ex As Exception
            Console.WriteLine("Gagal mengirim email deposit: " & ex.Message)
        Finally
            If fs IsNot Nothing Then fs.Dispose()
        End Try
    End Sub

    Public Sub MailQuote(Id As String, QFile As String, PFile As String, MTo As String, MCc As String)
        Try
            Dim myData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + Id + "'")
            If myData.Tables(0).Rows.Count = 0 Then Exit Sub

            Dim orderId As String = myData.Tables(0).Rows(0).Item("OrderId").ToString()
            Dim createdBy As String = myData.Tables(0).Rows(0).Item("CreatedBy").ToString()
            Dim customerId As String = myData.Tables(0).Rows(0).Item("CustomerId").ToString()
            Dim orderType As String = myData.Tables(0).Rows(0).Item("OrderType").ToString()

            Dim customerName As String = GetItemData("SELECT Name FROM Customers WHERE Id = '" + customerId + "'")

            Dim appId As String = GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(createdBy) + "'")

            Dim queryMailing As String = "SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId) + "' AND Name = 'Quote Order' AND Active = 1"
            If orderType = "Panorama" OrElse orderType = "Evolve" Then
                queryMailing = "SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId) + "' AND Name = 'Quote Order Shutters' AND Active = 1"
            End If

            Dim mailData As DataSet = GetListData(queryMailing)
            If mailData.Tables(0).Rows.Count = 0 Then Exit Sub

            Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
            Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
            Dim mailPort As Integer = mailData.Tables(0).Rows(0).Item("Port")
            Dim mailAccount As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
            Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()
            Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
            Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
            Dim mailCc As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()
            Dim mailBcc As String = mailData.Tables(0).Rows(0).Item("Bcc").ToString()
            Dim mailNetworkCredentials As Boolean = mailData.Tables(0).Rows(0).Item("NetworkCredentials")
            Dim mailDefaultCredentials As Boolean = mailData.Tables(0).Rows(0).Item("DefaultCredentials")
            Dim mailEnableSSL As Boolean = mailData.Tables(0).Rows(0).Item("EnableSSL")

            Dim mailContent As String = "<span style='font-family: Lucida Sans Unicode, sans-serif; font-size: 14px;'>Hi <b>" & customerName & ",</b></span>"
            mailContent &= "<br /><br />Please see the files we have attached.<br /><br />"

            If orderType = "RBR" OrElse orderType = "Blinds" OrElse orderType = "Curtain" OrElse orderType = "Veri Shades" OrElse orderType = "Zebra Blinds" Then
                mailContent &= "<span style='font-weight: bold;'>Kind Regards,<br /><br />Customer Service<br />Lifestyle Blinds & Shutters</span>"
            End If
            If orderType = "Panorama" OrElse orderType = "Evolve" Then
                mailContent &= "<span style='font-weight: bold;'>Kind Regards,<br /><br />Customer Service<br />Sunlight Products</span>"
            End If

            Using myMail As New MailMessage()
                myMail.Subject = "Quote Order - " & orderId
                myMail.From = New MailAddress(mailServer, mailAlias)
                myMail.Body = mailContent
                myMail.IsBodyHtml = True

                If Not String.IsNullOrEmpty(MTo) AndAlso IsValidEmail(MTo) Then
                    myMail.To.Add(MTo)
                ElseIf Not String.IsNullOrEmpty(mailTo) Then
                    For Each addr In mailTo.Split(";"c)
                        myMail.To.Add(addr)
                    Next
                Else
                    myMail.To.Add("reza@bigblinds.co.id")
                End If

                If Not String.IsNullOrEmpty(mailCc) Then
                    For Each thisMail In mailCc.Split(";"c)
                        If IsValidEmail(thisMail.Trim()) Then myMail.CC.Add(thisMail.Trim())
                    Next
                End If
                If Not String.IsNullOrEmpty(mailBcc) Then
                    For Each thisMail In mailBcc.Split(";"c)
                        If IsValidEmail(thisMail.Trim()) Then myMail.Bcc.Add(thisMail.Trim())
                    Next
                End If

                If Not String.IsNullOrEmpty(MCc) AndAlso IsValidEmail(MCc) Then
                    myMail.CC.Add(MCc)
                End If

                Using attach1 As New Attachment(QFile), attach2 As New Attachment(PFile)
                    myMail.Attachments.Add(attach1)
                    myMail.Attachments.Add(attach2)

                    Dim smtpClient As New SmtpClient() With {
                        .Host = mailHost,
                        .Port = mailPort,
                        .EnableSsl = mailEnableSSL,
                        .UseDefaultCredentials = mailDefaultCredentials,
                        .DeliveryMethod = SmtpDeliveryMethod.Network
                    }

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

            If File.Exists(QFile) Then File.Delete(QFile)
            If File.Exists(PFile) Then File.Delete(PFile)

            Threading.Thread.Sleep(3000)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub MailProduction()
        Try
            Dim headerData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Status = 'In Production' AND CONVERT(DATE, JobDate) = '" + Now.ToString("yyyy-MM-dd") + "'")
            If headerData.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To headerData.Tables(0).Rows.Count - 1
                    Dim headerId As String = headerData.Tables(0).Rows(i).Item("Id").ToString()
                    Dim createdBy As String = headerData.Tables(0).Rows(i).Item("CreatedBy").ToString()
                    Dim customerId As String = headerData.Tables(0).Rows(i).Item("CustomerId").ToString()
                    Dim orderType As String = headerData.Tables(0).Rows(i).Item("OrderType").ToString()
                    Dim orderNumber As String = headerData.Tables(0).Rows(i).Item("OrderNumber").ToString()
                    Dim orderName As String = headerData.Tables(0).Rows(i).Item("OrderName").ToString()
                    Dim jobId As String = headerData.Tables(0).Rows(i).Item("JobId").ToString()

                    Dim appId As String = GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(createdBy).ToString() + "'")
                    Dim customerName As String = GetItemData("SELECT Name FROM Customers WHERE Id = '" + customerId + "'")

                    Dim customerMail As String = GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId = '" + customerId + "' AND [Primary] = 1")
                    If String.IsNullOrEmpty(customerMail) Then Continue For

                    Dim queryMailings As String = "SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId).ToString() + "' AND Name = 'Production Order' AND Active = 1"
                    If orderType = "Panorama" OrElse orderType = "Evolve" Then
                        queryMailings = "SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId).ToString() + "' AND Name = 'Production Order Shutters' AND Active = 1"
                    End If

                    Dim mailData As DataSet = GetListData(queryMailings)
                    If Not mailData.Tables(0).Rows.Count = 0 Then
                        Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
                        Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
                        Dim mailPort As Integer = mailData.Tables(0).Rows(0).Item("Port")

                        Dim mailAccount As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
                        Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()
                        Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
                        Dim mailSubject As String = mailData.Tables(0).Rows(0).Item("Subject").ToString()

                        Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
                        Dim mailCc As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()
                        Dim mailBcc As String = mailData.Tables(0).Rows(0).Item("Bcc").ToString()

                        Dim mailNetworkCredentials As Boolean = mailData.Tables(0).Rows(0).Item("NetworkCredentials")
                        Dim mailDefaultCredentials As Boolean = mailData.Tables(0).Rows(0).Item("DefaultCredentials")
                        Dim mailEnableSSL As Boolean = mailData.Tables(0).Rows(0).Item("EnableSSL")

                        Dim mailContent As String = String.Empty

                        mailContent &= "<h2 style='font-family: Lucida Sans Unicode, sans-serif; font-size: 18px;'>Orders Acknowledgement</h2>"
                        mailContent &= "<div style='font-family: Lucida Sans Unicode, sans-serif; font-size: 16px;'>Date: " & Now.ToString("dd MMM yyyy") & "</div>"
                        mailContent &= "<div style='font-family: Lucida Sans Unicode, sans-serif; font-size: 16px;'>Store: " & customerName & "</div>"
                        mailContent &= "<br />"
                        mailContent &= "<div style='font-family: Lucida Sans Unicode, sans-serif;font-size: 16px;'><b><u>Dear valued customer</u></b></div>"
                        If orderType = "RBR" OrElse orderType = "Blinds" OrElse orderType = "Curtain" OrElse orderType = "Veri Shades" OrElse orderType = "Zebra Blinds" Then
                            mailContent &= "<div style='font-family: Lucida Sans Unicode, sans-serif; font-size: 16px;'>This acknowledgement confirms the orders listed below have been received by Lifestyle Blinds & Shutters and have been entered for processing.</div>"
                        End If
                        If orderType = "Panorama" OrElse orderType = "Evolve" Then
                            mailContent &= "<div style='font-family: Lucida Sans Unicode, sans-serif; font-size: 16px;'>This acknowledgement confirms the orders listed below have been received and have been entered for processing.</div>"
                        End If

                        mailContent &= "<br />"

                        mailContent &= "<table style='font-family: Arial, sans-serif; font-size: 16px; border-collapse: collapse;'>"
                        mailContent &= "<tr><td style='padding: 4px;'>Order Number</td><td style='padding: 4px;'>:</td><td style='padding: 4px;'>" & orderNumber & "</td></tr>"
                        mailContent &= "<tr><td style='padding: 4px;'>Client Name</td><td style='padding: 4px;'>:</td><td style='padding: 4px;'>" & orderName & "</td></tr>"
                        mailContent &= "<tr><td style='padding: 4px;'>Job No</td><td style='padding: 4px;'>:</td><td style='padding: 4px;'>" & jobId & "</td></tr>"
                        mailContent &= "</table>"

                        mailContent &= "<br />"

                        mailContent &= "<table border='1' width='90%' cellpadding='5' cellspacing='0' style='border-collapse: collapse; font-family: Lucida Sans Unicode, sans-serif; font-size: 14px;'>"
                        mailContent &= "<thead><tr style='background-color: #f2f2f2;'>"
                        mailContent &= "<th>#</th>"
                        mailContent &= "<th>Product</th>"
                        mailContent &= "<th>Qty</th>"
                        mailContent &= "</tr></thead>"

                        mailContent &= "<tbody>"
                        Dim detailQuery As String = "SELECT * FROM OrderDetails WHERE HeaderId = '" + headerId + "' AND Active = 1 ORDER BY Id ASC"
                        Dim detailData As DataSet = GetListData(detailQuery)
                        If Not detailData.Tables(0).Rows.Count = 0 Then
                            For ls As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                                Dim productId As String = detailData.Tables(0).Rows(ls).Item("ProductId").ToString()
                                Dim designId As String = GetItemData("SELECT DesignId FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")
                                Dim blindId As String = GetItemData("SELECT BlindId FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")

                                Dim productName As String = GetItemData("SELECT Name FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")
                                Dim designName As String = GetItemData("SELECT Name FROM Designs WHERE Id = '" + UCase(designId).ToString() + "'")
                                Dim blindName As String = GetItemData("SELECT Name FROM Blinds WHERE Id = '" + UCase(designId).ToString() + "'")

                                Dim width As String = detailData.Tables(0).Rows(ls).Item("Width").ToString()
                                Dim widthB As String = detailData.Tables(0).Rows(ls).Item("WidthB").ToString()
                                Dim widthC As String = detailData.Tables(0).Rows(ls).Item("WidthC").ToString()
                                Dim widthD As String = detailData.Tables(0).Rows(ls).Item("WidthD").ToString()
                                Dim widthE As String = detailData.Tables(0).Rows(ls).Item("WidthE").ToString()
                                Dim widthF As String = detailData.Tables(0).Rows(ls).Item("WidthF").ToString()

                                Dim drop As String = detailData.Tables(0).Rows(ls).Item("Drop").ToString()
                                Dim dropB As String = detailData.Tables(0).Rows(ls).Item("DropB").ToString()
                                Dim dropC As String = detailData.Tables(0).Rows(ls).Item("DropC").ToString()
                                Dim dropD As String = detailData.Tables(0).Rows(ls).Item("DropD").ToString()
                                Dim dropE As String = detailData.Tables(0).Rows(ls).Item("DropE").ToString()
                                Dim dropF As String = detailData.Tables(0).Rows(ls).Item("DropE").ToString()

                                Dim size As String = width & " x " & drop
                                Dim sizeB As String = widthB & " x " & dropB
                                Dim sizeC As String = "(" & widthC & " x " & dropC & ")"
                                Dim sizeD As String = "(" & widthD & " x " & dropD & ")"
                                Dim sizeE As String = "(" & widthE & " x " & dropE & ")"
                                Dim sizeF As String = "(" & widthF & " x " & dropF & ")"

                                Dim fabricColourId As String = detailData.Tables(0).Rows(ls).Item("FabricColourId").ToString()
                                Dim fabricColourIdB As String = detailData.Tables(0).Rows(ls).Item("FabricColourIdB").ToString()
                                Dim fabricColourIdC As String = detailData.Tables(0).Rows(ls).Item("FabricColourIdC").ToString()

                                Dim fabricColourName As String = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourId + "'")
                                Dim fabricColourNameB As String = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdB + "'")
                                Dim fabricColourNameC As String = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdC + "'")

                                Dim itemDescription As String = productName

                                If designId = "6C0B3347-9730-45CA-905C-5EF682CD06EA" Then Continue For

                                If designName = "Aluminium Blind" Or designName = "LS Venetian" Or designName = "Venetian Blind" Then
                                    itemDescription = productName & " - " & size
                                End If

                                If designName = "Cellular Shades" Or designName = "Curtain" Or designName = "Roman Blind" Or designName = "Skin Only" Or designName = "Veri Shades" Or designName = "Zebra Blind" Then
                                    itemDescription = productName & " #" & fabricColourName & " - " & size
                                End If

                                If designName = "Panel Glide" Then
                                    Dim tubeType As String = GetItemData("SELECT TubeType FROM Products WHERE Id = '" + productId + "'")

                                    itemDescription = productName & " #" & fabricColourName & " - " & size
                                    If tubeType = "Track Only" Then
                                        itemDescription = productName & " - " & width
                                    End If
                                End If

                                If designName = "Panorama PVC Parts" Then
                                    Dim partCategory As String = detailData.Tables(0).Rows(ls).Item("PartCategory").ToString()
                                    Dim partComponent As String = detailData.Tables(0).Rows(ls).Item("PartComponent").ToString()
                                    Dim partColour As String = detailData.Tables(0).Rows(ls).Item("PartColour").ToString()
                                    Dim partLength As String = detailData.Tables(0).Rows(ls).Item("PartLength").ToString()

                                    itemDescription = productName & " " & partComponent
                                    If partCategory = "Louvres" Then
                                        itemDescription = productName & " " & partComponent & " " & partColour & " - " & partLength
                                        If partComponent = "Louvre Repair Pin" Or partComponent = "Standard Louvre Pin" Then
                                            itemDescription = productName & " " & partComponent & " " & partColour
                                        End If
                                    End If

                                    If partCategory = "Framing | Hinged" Or partCategory = "Framing | Bi-fold or Sliding" Or partCategory = "Framing | Fixed" Or partCategory = "Extrusion" Then
                                        itemDescription = productName & " " & partComponent & " " & partColour & " " & partLength
                                    End If

                                    If partCategory = "Post" Then
                                        itemDescription = productName & " " & partComponent
                                        If partComponent = "T-Post" Or partComponent = "90° Corner Post" Or partComponent = "135° Bay Post" Then
                                            itemDescription = productName & " " & partComponent & " " & partColour & " - " & partLength
                                        End If
                                    End If

                                    If partCategory = "Hinges" Then
                                        itemDescription = productName & " " & partComponent
                                        If partComponent = "76mm Non-Mortise Hinge" Or partComponent = "76mm Rabbet Hinge" Then
                                            itemDescription = productName & " " & partComponent & " " & partColour
                                        End If
                                    End If

                                    If partCategory = "Magnets and Strikers" Then
                                        itemDescription = productName & " " & partComponent & " " & partColour
                                        If partComponent = "Magnet" Then
                                            itemDescription = productName & " " & partComponent
                                        End If
                                    End If

                                    If partCategory = "Track Hardware" Then
                                        itemDescription = productName & " " & partComponent
                                        If partComponent = "Top Track" Or partComponent = "Bottom M Track" Or partComponent = "Bottom U Track" Then
                                            itemDescription = productName & " " & partComponent & " - " & partLength
                                        End If
                                    End If

                                    If partCategory = "Mist" Then
                                        itemDescription = productName & " " & partComponent & " " & partColour
                                    End If
                                End If

                                If designName = "Panorama PVC Shutters" Or designName = "Evolve Shutters" Then
                                    itemDescription = productName & " - " & size
                                End If

                                If designName = "Pelmet" Then
                                    itemDescription = productName & " #" & productName & " - " & size
                                    If String.IsNullOrEmpty(fabricColourName) Then
                                        itemDescription = productName & " - " & width
                                    End If
                                End If

                                If designName = "Vertical" Then
                                    itemDescription = productName & " #" & fabricColourName & " - " & size
                                    If blindId = "Track Only" Then
                                        itemDescription = productName & " - " & size
                                    End If
                                    If blindName = "Blades Only" Then
                                        itemDescription = productName & " #" & fabricColourName & " - " & drop
                                    End If
                                End If

                                If designName = "Roller Blind" Then
                                    itemDescription = productName & " #" & fabricColourName & " - " & size

                                    If blindName = "Double Bracket" Or blindName = "Link 2 Blinds Ind" Then
                                        itemDescription = productName
                                        itemDescription &= "<br />"
                                        itemDescription &= "First Blind #" & fabricColourName & " - " & size
                                        itemDescription &= "<br />"
                                        itemDescription &= "Second Blind #" & fabricColourNameB & " - " & sizeB
                                    End If

                                    If blindName = "Link 2 Blinds Dep" Then
                                        itemDescription = productName
                                        itemDescription &= "<br />"
                                        itemDescription &= "Control Blind #" & fabricColourName & " - " & size
                                        itemDescription &= "<br />"
                                        itemDescription &= "End Blind #" & fabricColourNameB & " - " & sizeB
                                    End If

                                    If blindName = "Link 3 Blinds Dep" Then
                                        itemDescription = productName
                                        itemDescription &= "<br />"
                                        itemDescription &= "Control Blind #" & fabricColourName & " - " & size
                                        itemDescription &= "<br />"
                                        itemDescription &= "Middle Blind #" & fabricColourNameB & " - " & sizeB
                                        itemDescription &= "<br />"
                                        itemDescription &= "End Blind #" & fabricColourNameC & " - " & sizeC
                                    End If

                                    If blindName = "Link 3 Blinds Ind with Dep" Then
                                        itemDescription = productName
                                        itemDescription &= "<br />"
                                        itemDescription &= "Ind Blind #" & fabricColourName & " - " & size
                                        itemDescription &= "<br />"
                                        itemDescription &= "Middle Blind #" & fabricColourNameB & " - " & sizeB
                                        itemDescription &= "<br />"
                                        itemDescription &= "End Blind #" & fabricColourNameC & " - " & sizeC
                                    End If
                                End If

                                mailContent &= "<tr>"
                                mailContent &= "<td style='text-align: center;'>" & (ls + 1).ToString() & "</td>"
                                mailContent &= "<td>" & itemDescription & "</td>"
                                mailContent &= "<td style='text-align: center;'>" & detailData.Tables(0).Rows(ls).Item("Qty").ToString() & "</td>"
                                mailContent &= "</tr>"
                            Next
                        End If

                        mailContent &= "</tbody>"

                        mailContent &= "<tfoot>"
                        mailContent &= "<tr style='font-weight: bold;'>"
                        mailContent &= "<td colspan='2' style='text-align: right;padding-right: 20px;'>TOTAL ORDER</td>"
                        mailContent &= "<td style='text-align: center;'>" & detailData.Tables(0).Rows.Count & "</td>"
                        mailContent &= "</tr>"
                        mailContent &= "</tfoot>"
                        mailContent &= "</table>"

                        mailContent &= "<br /><br />"
                        mailContent &= "<span style='font-family: Lucida Sans Unicode, sans-serif; font-size:16px;'>Please note that no changes can be made to an order once manufacturing has commenced.</span>"
                        mailContent &= "<br /><br /><br />"

                        mailContent &= "<span style='font-family: Lucida Sans Unicode, sans-serif; font-size:16px; font-weight: bold;'>Kind Regards,</span>"
                        mailContent &= "<br /><br />"
                        mailContent &= "<span style='font-family: Lucida Sans Unicode, sans-serif; font-size:16px; font-weight: bold;'>Customer Service</span>"
                        mailContent &= "<br />"
                        If orderType = "RBR" OrElse orderType = "Blinds" OrElse orderType = "Curtain" OrElse orderType = "Veri Shades" OrElse orderType = "Zebra Blinds" Then
                            mailContent &= "<span style='font-family: Lucida Sans Unicode, sans-serif; font-size:16px; font-weight: bold;'>Lifestyle Blinds & Shutters</span>"
                        End If

                        mailContent &= "<br />"

                        Dim myMail As New MailMessage
                        myMail.Subject = mailSubject
                        myMail.From = New MailAddress(mailServer, mailAlias)

                        If Not customerMail = "" Then
                            Dim thisArray() As String = customerMail.Split(";")
                            Dim thisMail As String = String.Empty
                            For Each thisMail In thisArray
                                If IsValidEmail(thisMail) Then
                                    myMail.To.Add(thisMail)
                                End If
                            Next
                        End If

                        If Not mailCc = "" Then
                            Dim thisArray() As String = mailCc.Split(";")
                            Dim thisMail As String = String.Empty
                            For Each thisMail In thisArray
                                myMail.CC.Add(thisMail)
                            Next
                        End If

                        If Not mailBcc = "" Then
                            Dim thisArray() As String = mailBcc.Split(";")
                            Dim thisMail As String = String.Empty
                            For Each thisMail In thisArray
                                myMail.Bcc.Add(thisMail)
                            Next
                        End If

                        myMail.Body = mailContent
                        myMail.IsBodyHtml = True
                        Dim smtpClient As New SmtpClient()
                        smtpClient.Host = mailHost
                        smtpClient.EnableSsl = mailEnableSSL
                        Dim NetworkCredl As New NetworkCredential()
                        NetworkCredl.UserName = mailAccount
                        NetworkCredl.Password = mailPassword
                        smtpClient.UseDefaultCredentials = mailDefaultCredentials
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network

                        If mailNetworkCredentials = True Then
                            smtpClient.Credentials = NetworkCredl
                        ElseIf mailDefaultCredentials Then
                            smtpClient.UseDefaultCredentials = True
                        Else
                            smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials
                        End If

                        smtpClient.Port = mailPort
                        smtpClient.Send(myMail)

                        Threading.Thread.Sleep(500)
                    End If
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub MailSuplierShutters(Id As String, Lampiran As String)
        Dim myData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" & Id & "'")
        If myData.Tables(0).Rows.Count = 0 Then Exit Sub

        Dim row As DataRow = myData.Tables(0).Rows(0)
        Dim orderId As String = row("OrderId").ToString()
        Dim customerId As String = row("CustomerId").ToString()
        Dim orderNumber As String = row("OrderNumber").ToString()
        Dim orderName As String = row("OrderName").ToString()
        Dim orderDate As String = Convert.ToDateTime(row("SubmittedDate")).ToString("dd MMM yyyy")
        Dim loginId As String = row("CreatedBy").ToString().ToUpper()

        Dim customerName As String = GetItemData("SELECT Name FROM Customers WHERE Id = '" & customerId & "'")

        Dim addressData As DataSet = GetListData("SELECT Street AS Address1, CONVERT(VARCHAR, Suburb) + ', ' + CONVERT(VARCHAR, States) + ' ' + CONVERT(VARCHAR, PostCode) AS Address2 FROM CustomerAddress WHERE CustomerId = '" & customerId & "' AND [Primary] = 1")
        If addressData.Tables(0).Rows.Count = 0 Then Exit Sub

        Dim appId As String = GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" & loginId & "'")
        If String.IsNullOrEmpty(appId) Then Exit Sub

        Dim mailData As DataSet = GetListData("SELECT * FROM Mailings WHERE ApplicationId = '" & appId & "' AND Name = 'Supplier Shutters' AND Active = 1")
        If mailData.Tables(0).Rows.Count = 0 Then Exit Sub

        Dim mailRow As DataRow = mailData.Tables(0).Rows(0)
        Dim mailServer As String = mailRow("Server").ToString()
        Dim mailHost As String = mailRow("Host").ToString()
        Dim mailPort As Integer = Convert.ToInt32(mailRow("Port"))
        Dim mailAccount As String = mailRow("Account").ToString()
        Dim mailPassword As String = mailRow("Password").ToString()
        Dim mailAlias As String = mailRow("Alias").ToString()
        Dim mailTo As String = mailRow("To").ToString()
        Dim mailCc As String = mailRow("Cc").ToString()
        Dim mailBcc As String = mailRow("Bcc").ToString()
        Dim mailNetworkCredentials As Boolean = Convert.ToBoolean(mailRow("NetworkCredentials"))
        Dim mailDefaultCredentials As Boolean = Convert.ToBoolean(mailRow("DefaultCredentials"))
        Dim mailEnableSSL As Boolean = Convert.ToBoolean(mailRow("EnableSSL"))

        Dim thisText As String = String.Format("{0} - {1} - {2} - New Job # {3}", customerName, orderNumber, orderName, orderId)

        Dim mailContent As String =
        "<div style='font-family: Lucida Sans Unicode, sans-serif; font-size: 14px; color: #000000;'>" &
        "Hi Lisa,<br><br>" &
        "Please see the attached new shutter order for " & thisText & ".<br><br>" &
        "Please let us know if you have any questions or need clarification.<br><br>" &
        "Kind Regards,<br>Customer Service<br><br><br>" &
        "<strong>Sunlight Products</strong><br>" &
        "28 Stoddart Road, Prospect NSW 2148<br><br>" &
        "<span style='color: red;'>P</span>: 02 9688 1555<br>" &
        "<span style='color: red;'>E</span>: customerservice@sunlight.com.au<br>" &
        "<span style='color: red;'>W</span>: https://sunlightproducts.com.au<br>" &
        "</div>"

        Try
            Using myMail As New MailMessage()
                myMail.Subject = thisText
                myMail.From = New MailAddress(mailServer, mailAlias)
                myMail.Body = mailContent
                myMail.IsBodyHtml = True

                ' To
                If Not String.IsNullOrWhiteSpace(mailTo) Then
                    For Each email In mailTo.Split(";"c)
                        If IsValidEmail(email) Then myMail.To.Add(email)
                    Next
                Else
                    myMail.To.Add("reza@bigblinds.co.id")
                End If

                ' Cc
                If Not String.IsNullOrWhiteSpace(mailCc) Then
                    For Each email In mailCc.Split(";"c)
                        If IsValidEmail(email) Then myMail.CC.Add(email)
                    Next
                End If

                ' Bcc
                If Not String.IsNullOrWhiteSpace(mailBcc) Then
                    For Each email In mailBcc.Split(";"c)
                        If IsValidEmail(email) Then myMail.Bcc.Add(email)
                    Next
                End If

                ' Attachment tanpa FileStream agar tidak lock
                If File.Exists(Lampiran) Then
                    myMail.Attachments.Add(New Attachment(Lampiran))
                Else
                    Throw New FileNotFoundException("File lampiran tidak ditemukan: " & Lampiran)
                End If

                ' Kirim email
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

            ' Setelah terkirim, hapus lampiran
            If File.Exists(Lampiran) Then File.Delete(Lampiran)

        Catch ex As Exception
            Console.WriteLine("Gagal mengirim email: " & ex.Message)
        End Try
    End Sub

    Public Sub MailNewShipment(HeaderId As String)
        Try
            Dim myData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + HeaderId + "'")

            Dim orderId As String = myData.Tables(0).Rows(0).Item("OrderId").ToString()
            Dim customerId As String = myData.Tables(0).Rows(0).Item("CustomerId").ToString()
            Dim orderNumber As String = myData.Tables(0).Rows(0).Item("OrderNumber").ToString()
            Dim orderName As String = myData.Tables(0).Rows(0).Item("OrderName").ToString()
            Dim createdBy As String = myData.Tables(0).Rows(0).Item("CreatedBy").ToString()
            Dim shipmentId As String = myData.Tables(0).Rows(0).Item("ShipmentId").ToString()

            Dim shipmentNumber As String = GetItemData("SELECT ShipmentNumber FROM OrderShipments WHERE Id='" + shipmentId + "'")
            Dim strEtaCustomer As String = GetItemData("SELECT ETACustomer FROM OrderShipments WHERE Id='" + shipmentId + "'")
            Dim etaCustomer As String = Convert.ToDateTime(strEtaCustomer).ToString("dd MMM yyyy")

            Dim customerName As String = GetItemData("SELECT Name FROM Customers WHERE Id = '" + customerId + "'")
            Dim customerEmail As String = GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId = '" + customerId + "' AND [Primary]=1")

            If String.IsNullOrEmpty(customerEmail) Then Exit Sub

            Dim appId As String = GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(createdBy).ToString() + "'")

            If Not myData.Tables(0).Rows.Count = 0 Then
                Dim mailData As DataSet = GetListData("SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId).ToString() + "' AND Name = 'New Shipment' AND Active = 1")
                If Not mailData.Tables(0).Rows.Count = 0 Then
                    Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
                    Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
                    Dim mailPort As Integer = mailData.Tables(0).Rows(0).Item("Port")

                    Dim mailAccount As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
                    Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()
                    Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
                    Dim mailSubject As String = mailData.Tables(0).Rows(0).Item("Subject").ToString()

                    Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
                    Dim mailCc As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()
                    Dim mailBcc As String = mailData.Tables(0).Rows(0).Item("Bcc").ToString()

                    Dim mailNetworkCredentials As Boolean = mailData.Tables(0).Rows(0).Item("NetworkCredentials")
                    Dim mailDefaultCredentials As Boolean = mailData.Tables(0).Rows(0).Item("DefaultCredentials")
                    Dim mailEnableSSL As Boolean = mailData.Tables(0).Rows(0).Item("EnableSSL")

                    Dim mailContent As String = "Dear <b>" & UCase(customerName).ToString() & "</b>,"

                    mailContent &= "<br /><br />"
                    mailContent &= "The below shutter order has now completed manufacture and has been allocated to container " & "<b>" & shipmentNumber & "</b>" & " for shipping."
                    mailContent &= "<br /><br />"
                    mailContent &= "Order No : " & "<b>" & orderNumber & "</b>"
                    mailContent &= "<br />"
                    mailContent &= "Customer : " & "<b>" & orderName & "</b>"
                    mailContent &= "<br />"
                    mailContent &= "under " & "<b>" & orderId & "</b>"
                    mailContent &= "<br />"
                    mailContent &= "The estimated date of arrival is " & "<b>" & etaCustomer & "</b>."
                    mailContent &= "<br /><br />"
                    mailContent &= "Please be aware that this is an estimated date and may change. We will advise of any changes as they arise."

                    mailContent &= "<br /><br /><br />"
                    mailContent &= "<span style='font-size:16px; font-weight: bold;'>Kind Regards,</span>"
                    mailContent &= "<br /><br /><br />"
                    mailContent &= "<span style='font-size:16px; font-weight: bold;'>Customer Service</span>"
                    mailContent &= "<br />"
                    mailContent &= "<span style='font-size:16px; font-weight: bold;'>Sunlight Products</span>"
                    mailContent &= "<br />"

                    Dim myMail As New MailMessage

                    If Not customerEmail = "" Then
                        myMail.To.Add(customerEmail)
                    Else
                        Dim thisArray() As String = mailTo.Split(";")
                        Dim thisMail As String = String.Empty
                        For Each thisMail In thisArray
                            myMail.To.Add(thisMail)
                        Next
                    End If

                    If Not mailCc = "" Then
                        Dim thisArray() As String = mailCc.Split(";")
                        Dim thisMail As String = String.Empty
                        For Each thisMail In thisArray
                            myMail.CC.Add(thisMail)
                        Next
                    End If

                    If Not mailBcc = "" Then
                        Dim thisArray() As String = mailBcc.Split(";")
                        Dim thisMail As String = String.Empty
                        For Each thisMail In thisArray
                            myMail.Bcc.Add(thisMail)
                        Next
                    End If

                    myMail.Subject = "Order " & orderId & " | " & mailSubject
                    myMail.From = New MailAddress(mailServer, mailAlias)
                    myMail.Body = mailContent
                    myMail.IsBodyHtml = True
                    Dim smtpClient As New SmtpClient()
                    smtpClient.Host = mailHost
                    smtpClient.EnableSsl = mailEnableSSL
                    Dim NetworkCredl As New NetworkCredential()
                    NetworkCredl.UserName = mailAccount
                    NetworkCredl.Password = mailPassword
                    smtpClient.UseDefaultCredentials = mailDefaultCredentials
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
                    If mailNetworkCredentials = True Then
                        smtpClient.Credentials = NetworkCredl
                    End If

                    smtpClient.Port = mailPort
                    smtpClient.Send(myMail)

                    Threading.Thread.Sleep(3000)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub MailAmendedShipment(HeaderId As String)
        Try
            Dim myData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + HeaderId + "'")

            Dim orderId As String = myData.Tables(0).Rows(0).Item("OrderId").ToString()
            Dim customerId As String = myData.Tables(0).Rows(0).Item("CustomerId").ToString()
            Dim orderNumber As String = myData.Tables(0).Rows(0).Item("OrderNumber").ToString()
            Dim orderName As String = myData.Tables(0).Rows(0).Item("OrderName").ToString()
            Dim createdBy As String = myData.Tables(0).Rows(0).Item("CreatedBy").ToString()
            Dim shipmentId As String = myData.Tables(0).Rows(0).Item("ShipmentId").ToString()

            Dim shipmentNumber As String = GetItemData("SELECT ShipmentNumber FROM OrderShipments WHERE Id='" + shipmentId + "'")
            Dim strEtaCustomer As String = GetItemData("SELECT ETACustomer FROM OrderShipments WHERE Id='" + shipmentId + "'")
            Dim etaCustomer As String = Convert.ToDateTime(strEtaCustomer).ToString("dd MMM yyyy")

            Dim customerName As String = GetItemData("SELECT Name FROM Customers WHERE Id = '" + customerId + "'")
            Dim customerEmail As String = GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId = '" + customerId + "' AND [Primary]=1")
            If String.IsNullOrEmpty(customerEmail) Then
                Exit Sub
            End If

            Dim appId As String = GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(createdBy).ToString() + "'")

            If Not myData.Tables(0).Rows.Count = 0 Then
                Dim mailData As DataSet = GetListData("SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId).ToString() + "' AND Name = 'Amended Shipment' AND Active = 1")
                If Not mailData.Tables(0).Rows.Count = 0 Then
                    Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
                    Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
                    Dim mailPort As Integer = mailData.Tables(0).Rows(0).Item("Port")

                    Dim mailAccount As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
                    Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()
                    Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
                    Dim mailSubject As String = mailData.Tables(0).Rows(0).Item("Subject").ToString()

                    Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
                    Dim mailCc As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()
                    Dim mailBcc As String = mailData.Tables(0).Rows(0).Item("Bcc").ToString()

                    Dim mailNetworkCredentials As Boolean = mailData.Tables(0).Rows(0).Item("NetworkCredentials")
                    Dim mailDefaultCredentials As Boolean = mailData.Tables(0).Rows(0).Item("DefaultCredentials")
                    Dim mailEnableSSL As Boolean = mailData.Tables(0).Rows(0).Item("EnableSSL")

                    Dim mailContent As String = "Dear <b>" & UCase(customerName).ToString() & "</b>,"

                    mailContent &= "<br /><br />"
                    mailContent &= "The below shutter order which is allocated to container " & "<b>" & shipmentNumber & "</b>" & " for shipping has had a change in the estimated date of arrival."
                    mailContent &= "<br /><br />"
                    mailContent &= "Order No : " & "<b>" & orderNumber & "</b>"
                    mailContent &= "<br />"
                    mailContent &= "Customer : " & "<b>" & orderName & "</b>"
                    mailContent &= "<br />"
                    mailContent &= "under " & "<b>" & orderId & "</b>"
                    mailContent &= "<br />"
                    mailContent &= "The updated estimated date of arrival is " & "<b>" & etaCustomer & "</b>."
                    mailContent &= "<br /><br /><br />"
                    mailContent &= "<span style='font-size:16px; font-weight: bold;'>Kind Regards,</span>"
                    mailContent &= "<br /><br /><br />"
                    mailContent &= "<span style='font-size:16px; font-weight: bold;'>Customer Service</span>"
                    mailContent &= "<br />"
                    mailContent &= "<span style='font-size:16px; font-weight: bold;'>Sunlight Products</span>"
                    mailContent &= "<br />"

                    Dim myMail As New MailMessage

                    If Not customerEmail = "" Then
                        myMail.To.Add(customerEmail)
                    Else
                        Dim thisArray() As String = mailTo.Split(";")
                        Dim thisMail As String = String.Empty
                        For Each thisMail In thisArray
                            myMail.To.Add(thisMail)
                        Next
                    End If

                    If Not mailCc = "" Then
                        Dim thisArray() As String = mailCc.Split(";")
                        Dim thisMail As String = String.Empty
                        For Each thisMail In thisArray
                            myMail.CC.Add(thisMail)
                        Next
                    End If

                    If Not mailBcc = "" Then
                        Dim thisArray() As String = mailBcc.Split(";")
                        Dim thisMail As String = String.Empty
                        For Each thisMail In thisArray
                            myMail.Bcc.Add(thisMail)
                        Next
                    End If

                    myMail.Subject = "Order " & orderId & " | " & mailSubject
                    myMail.From = New MailAddress(mailServer, mailAlias)
                    myMail.Body = mailContent
                    myMail.IsBodyHtml = True
                    Dim smtpClient As New SmtpClient()
                    smtpClient.Host = mailHost
                    smtpClient.EnableSsl = mailEnableSSL
                    Dim NetworkCredl As New NetworkCredential()
                    NetworkCredl.UserName = mailAccount
                    NetworkCredl.Password = mailPassword
                    smtpClient.UseDefaultCredentials = mailDefaultCredentials
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
                    If mailNetworkCredentials = True Then
                        smtpClient.Credentials = NetworkCredl
                    End If

                    smtpClient.Port = mailPort
                    smtpClient.Send(myMail)

                    Threading.Thread.Sleep(3000)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub MailLogin(LoginId As String, CustomerId As String)
        Try
            Dim custData As DataSet = GetListData("SELECT * FROM CustomerLogins WHERE CustomerId = '" + CustomerId + "'")
            If custData.Tables(0).Rows.Count > 0 Then
                Dim appId As String = GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(LoginId).ToString() + "'")
                Dim mailData As DataSet = GetListData("SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId).ToString() + "' AND Name = 'Login Detail' AND Active = 1")

                Dim customerName As String = GetItemData("SELECT Name FROM Customers WHERE Id = '" + CustomerId + "'")
                Dim customerMail As String = GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId = '" + CustomerId + "' AND [Primary] = 1")

                If Not mailData.Tables(0).Rows.Count = 0 Then
                    Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
                    Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
                    Dim mailPort As Integer = mailData.Tables(0).Rows(0).Item("Port")

                    Dim mailAccount As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
                    Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()
                    Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
                    Dim mailSubject As String = mailData.Tables(0).Rows(0).Item("Subject").ToString()

                    Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
                    Dim mailCc As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()

                    Dim mailNetworkCredentials As Boolean = mailData.Tables(0).Rows(0).Item("NetworkCredentials")
                    Dim mailDefaultCredentials As Boolean = mailData.Tables(0).Rows(0).Item("DefaultCredentials")
                    Dim mailEnableSSL As Boolean = mailData.Tables(0).Rows(0).Item("EnableSSL")

                    Dim mailContent As New StringBuilder()
                    mailContent.AppendLine("<html>")
                    mailContent.AppendLine("<head>")
                    mailContent.AppendLine("<style>")
                    mailContent.AppendLine("table { font-family: Arial, sans-serif; border-collapse: collapse; width: 60%; }")
                    mailContent.AppendLine("th, td { border: 1px solid #dddddd; text-align: left; padding: 8px; font-size: 18px; }")
                    mailContent.AppendLine("th { background-color: #f2f2f2; }")
                    mailContent.AppendLine("</style>")
                    mailContent.AppendLine("</head>")
                    mailContent.AppendLine("<body>")
                    mailContent.AppendLine("Hi <b>" & customerName & "</b><br /><br />")
                    mailContent.AppendLine("This is your detail account logins.<br /><br />")
                    mailContent.AppendLine("<table>")
                    mailContent.AppendLine("<tr>")
                    mailContent.AppendLine("<th>Full Name</th>")
                    mailContent.AppendLine("<th>Username</th>")
                    mailContent.AppendLine("<th>Password</th>")
                    mailContent.AppendLine("</tr>")

                    For i As Integer = 0 To custData.Tables(0).Rows.Count - 1
                        Dim settingCfg As New SettingConfig
                        Dim password As String = settingCfg.Decrypt(custData.Tables(0).Rows(i)("Password").ToString())
                        mailContent.AppendLine("<tr>")
                        mailContent.AppendLine("<td>" & custData.Tables(0).Rows(i)("FullName").ToString() & "</td>")
                        mailContent.AppendLine("<td>" & custData.Tables(0).Rows(i)("UserName").ToString() & "</td>")
                        mailContent.AppendLine("<td>" & password & "</td>")
                        mailContent.AppendLine("</tr>")
                    Next

                    mailContent.AppendLine("</table>")
                    mailContent.AppendLine("Please login and change your password at <a href='https://shutters.onlineorder.au/account/login'>shutters.onlineorder.au</a><br /><br /><br />")
                    mailContent.AppendLine("<br /><br /><br />")
                    mailContent.AppendLine("Kind Regards,<br /><br /><br />")
                    mailContent.AppendLine("Customer Service<br />")
                    mailContent.AppendLine("Sunlight Products")
                    mailContent.AppendLine("</body>")
                    mailContent.AppendLine("</html>")

                    Dim myMail As New MailMessage
                    myMail.Subject = mailSubject
                    myMail.From = New MailAddress(mailServer, mailAlias)

                    If Not customerMail = "" Then
                        If IsValidEmail(customerMail) Then
                            myMail.To.Add(customerMail)
                        Else
                            myMail.To.Add(mailTo)
                        End If
                    Else
                        myMail.To.Add(mailTo)
                    End If

                    If Not mailCc = "" Then
                        Dim ccArray() As String = mailCc.Split(";")
                        Dim thisMail As String = ""
                        For Each thisMail In ccArray
                            myMail.CC.Add(thisMail)
                        Next
                    End If

                    myMail.Body = mailContent.ToString()
                    myMail.IsBodyHtml = True
                    Dim smtpClient As New SmtpClient()
                    smtpClient.Host = mailHost
                    smtpClient.EnableSsl = mailEnableSSL
                    Dim NetworkCredl As New NetworkCredential()
                    NetworkCredl.UserName = mailAccount
                    NetworkCredl.Password = mailPassword
                    smtpClient.UseDefaultCredentials = mailDefaultCredentials
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network

                    If mailNetworkCredentials = True Then
                        smtpClient.Credentials = NetworkCredl
                    End If

                    smtpClient.Port = mailPort
                    smtpClient.Send(myMail)
                    Threading.Thread.Sleep(3000)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub MailError(Page As String, Action As String, LoginId As String, exError As String)
        Try
            Dim appId As String = GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(LoginId).ToString() + "'")

            Dim mailData As DataSet = GetListData("SELECT * FROM Mailings WHERE ApplicationId = '" + UCase(appId).ToString() + "' AND Name = 'Web Error' AND Active = 1")

            If Not mailData.Tables(0).Rows.Count = 0 Then
                Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
                Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
                Dim mailPort As Integer = mailData.Tables(0).Rows(0).Item("Port")

                Dim mailUserName As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
                Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()
                Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
                Dim mailSubject As String = mailData.Tables(0).Rows(0).Item("Subject").ToString()

                Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
                Dim mailCc As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()

                Dim mailNetworkCredentials As Boolean = mailData.Tables(0).Rows(0).Item("NetworkCredentials")
                Dim mailDefaultCredentials As Boolean = mailData.Tables(0).Rows(0).Item("DefaultCredentials")
                Dim mailEnableSSL As Boolean = mailData.Tables(0).Rows(0).Item("EnableSSL")

                Dim userName As String = GetItemData("SELECT UserName FROM CustomerLogins WHERE Id = '" + UCase(LoginId).ToString() + "'")

                Dim mailBody As String = String.Empty

                mailBody = "Hi Team, there's an error."
                mailBody &= "<br /><br />"
                mailBody &= "Web Page : " & Page
                mailBody &= "<br />"
                mailBody &= "Action : " & Action
                mailBody &= "<br />"
                mailBody &= "Users : " & UCase(LoginId).ToString() & " | " & userName
                mailBody &= "<br /><br />"
                mailBody &= "Error Message : "
                mailBody &= "<br />"
                mailBody &= exError

                Dim myMail As New MailMessage

                myMail.Subject = mailSubject
                myMail.From = New MailAddress(mailServer, mailAlias)
                myMail.To.Add(mailTo)

                If Not mailCc = "" Then
                    Dim thisArray() As String = mailCc.Split(";")
                    Dim thisMail As String = ""
                    For Each thisMail In thisArray
                        myMail.CC.Add(thisMail)
                    Next
                End If

                myMail.Body = mailBody
                myMail.IsBodyHtml = True
                Dim smtpClient As New SmtpClient()
                smtpClient.Host = mailHost
                smtpClient.EnableSsl = mailEnableSSL
                Dim NetworkCredl As New NetworkCredential()
                NetworkCredl.UserName = mailUserName
                NetworkCredl.Password = mailPassword
                smtpClient.UseDefaultCredentials = mailDefaultCredentials
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network

                If mailNetworkCredentials = True Then
                    smtpClient.Credentials = NetworkCredl
                End If

                smtpClient.Port = mailPort
                smtpClient.Send(myMail)
                Threading.Thread.Sleep(3000)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub MailTest(EmailName As String, EmailTo As String)
        'Try
        Dim mailData As DataSet = GetListData("SELECT * FROM Mailings WHERE ApplicationId = 'DE067459-832C-47E5-A13F-2C29B3E26D06' AND Name = '" & EmailName & "' AND Active = 1")
        If mailData Is Nothing OrElse mailData.Tables.Count = 0 OrElse mailData.Tables(0).Rows.Count = 0 Then Exit Sub

        Dim row As DataRow = mailData.Tables(0).Rows(0)
        Dim mailServer As String = row("Server").ToString()
        Dim mailHost As String = row("Host").ToString()
        Dim mailPort As Integer = Convert.ToInt32(row("Port"))
        Dim mailAccount As String = row("Account").ToString()
        Dim mailPassword As String = row("Password").ToString()
        Dim mailAlias As String = row("Alias").ToString()
        Dim mailSubject As String = row("Subject").ToString()
        Dim mailTo As String = row("To").ToString()
        Dim mailCc As String = row("Cc").ToString()
        Dim mailBcc As String = row("Bcc").ToString()
        Dim mailNetworkCredentials As Boolean = row("NetworkCredentials")
        Dim mailDefaultCredentials As Boolean = row("DefaultCredentials")
        Dim mailEnableSSL As Boolean = row("EnableSSL")

        Using myMail As New MailMessage()
            myMail.Subject = mailSubject
            myMail.From = New MailAddress(mailServer, mailAlias)
            myMail.Body = "TEST EMAIL"
            myMail.IsBodyHtml = True

            If Not String.IsNullOrWhiteSpace(EmailTo) AndAlso IsValidEmail(EmailTo) Then
                myMail.To.Add(EmailTo)
            Else
                myMail.To.Add("reza@bigblinds.co.id")
            End If

            If Not String.IsNullOrWhiteSpace(mailCc) Then
                For Each ccEmail As String In mailCc.Split(";"c)
                    If IsValidEmail(ccEmail.Trim()) Then myMail.CC.Add(ccEmail.Trim())
                Next
            End If

            If Not String.IsNullOrWhiteSpace(mailBcc) Then
                For Each bccEmail As String In mailBcc.Split(";"c)
                    If IsValidEmail(bccEmail.Trim()) Then myMail.Bcc.Add(bccEmail.Trim())
                Next
            End If

            Using smtpClient As New SmtpClient(mailHost, mailPort)
                smtpClient.EnableSsl = mailEnableSSL
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
                smtpClient.UseDefaultCredentials = mailDefaultCredentials

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

        Threading.Thread.Sleep(3000)

        'Catch ex As Exception
        'End Try
    End Sub

    Public Function IsValidEmail(email As String) As Boolean
        Dim emailPattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
        Dim regex As New Regex(emailPattern)
        Return regex.IsMatch(email)
    End Function
End Class
