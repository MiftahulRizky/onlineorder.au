Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Security.Cryptography

Public Class PublicConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    ' VARIABLE PRINT

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
        Dim result As String = ""
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
        Return result
    End Function

    Public Function GetItemData_Integer(thisString As String) As Integer
        Dim result As Double = 0.00
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
        Return result
    End Function

    Public Function GetItemData_Boolean(thisString As String) As Boolean
        Dim result As Double = 0.00
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
        Return result
    End Function

    Public Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Double = 0.00
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
        Return result
    End Function

    Public Function GetConfig(AppId As String) As Boolean
        Dim result As Boolean = False
        If Not AppId = "" Then
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT Active FROM Applications WHERE Id='" + UCase(AppId).ToString() + "'", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        End If
        Return result
    End Function

    Public Function GetDesignName(DesignId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT Name FROM Designs WHERE Id = '" + DesignId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetDesignPage(DesignId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT Page FROM Designs WHERE Id = '" + DesignId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetDesignActive(Id As String) As Boolean
        Dim result As Boolean = False
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT Active FROM Designs WHERE Id='" + Id + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0)
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetBlindId(KitId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT BlindId FROM HardwareKits WHERE Id='" + KitId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetBlindName(BlindId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT Name FROM Blinds WHERE Id = '" + BlindId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetSoeKitId(KitId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT SoeId FROM HardwareKits WHERE Id='" + UCase(KitId).ToString() + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetKitName(KitId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT Name FROM HardwareKits WHERE Id = '" + KitId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetBracketType(KitId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT BracketType FROM HardwareKits WHERE Id = '" + KitId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetControlType(KitId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT ControlType FROM HardwareKits WHERE Id = '" + KitId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetTubeType(KitId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT TubeType FROM HardwareKits WHERE Id = '" + KitId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetFabricName(FabricId As String) As String
        Dim result As String = String.Empty
        If Not FabricId = "" Then
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT Name FROM Fabrics WHERE Id = '" + FabricId + "'", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        End If
        Return result
    End Function

    Public Function GetFabricType(FabricId As String) As String
        Dim result As String = String.Empty
        If Not FabricId = "" Then
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT Type FROM Fabrics WHERE Id = '" + FabricId + "'", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        End If
        Return result
    End Function

    Public Function GetFabricGroup(FabricId As String) As String
        Dim result As String = 0
        If Not FabricId = "" Then
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT [Group] FROM Fabrics WHERE Id = '" + FabricId + "'", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item("Group").ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        End If
        Return result
    End Function

    Public Function GetOrderNo(HeaderId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT OrderNo FROM OrderHeaders WHERE Id = '" + HeaderId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetOrderCust(HeaderId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT OrderCust FROM OrderHeaders WHERE Id = '" + HeaderId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetStoreId(HeaderId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT StoreId FROM OrderHeaders WHERE Id='" + HeaderId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetStoreCompany(HeaderId As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT Stores.Company FROM Stores INNER JOIN OrderHeaders ON Stores.Id = OrderHeaders.StoreId WHERE OrderHeaders.Id='" + HeaderId + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function CreateOrderHeaderId() As String
        Dim result As String = String.Empty
        Dim idDetail As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderHeaders ORDER BY Id DESC", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        idDetail = rdResult.Item("Id").ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        If idDetail = "" Then : result = 1
        Else : result = CInt(idDetail) + 1
        End If
        Return result
    End Function

    Public Function CreateOrderItemId() As String
        Dim result As String = String.Empty
        Dim idDetail As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderDetails ORDER BY Id DESC", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        idDetail = rdResult.Item("Id").ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        If idDetail = "" Then : result = 1
        Else : result = CInt(idDetail) + 1
        End If
        Return result
    End Function

    




    Public Sub InsertItemEdited(UserId As String, ItemId As String)
        Dim myQuery As String = "INSERT INTO OrderDetailsEdited SELECT NEWID(), @UserId, GETDATE(), * FROM OrderDetails WHERE Id=@ItemId"

        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand(myQuery, thisConn)
                myCmd.CommandType = CommandType.Text
                myCmd.Parameters.AddWithValue("@UserId", UserId)
                myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                myCmd.ExecuteNonQuery()
            End Using
            thisConn.Close()
        End Using
    End Sub

    Public Function InsertActivity(UserId As String, Page As String, Desc As String) As String
        Dim result As String = ""
        Dim rowsAffected As Integer = 0

        Dim myQuery As String = "INSERT INTO MemberActivity VALUES (NEWID(), @UserId, GETDATE(), @Page, @Description, 1)"

        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand(myQuery, thisConn)
                myCmd.CommandType = CommandType.Text
                myCmd.Parameters.AddWithValue("@UserId", UserId)
                myCmd.Parameters.AddWithValue("@Page", Page)
                myCmd.Parameters.AddWithValue("@Description", Desc)
                rowsAffected = myCmd.ExecuteNonQuery()
            End Using
            thisConn.Close()
            result = "1"
            If rowsAffected = 0 Then : result = "0" : End If
        End Using
        Return result
    End Function

    Public Sub MailError(UserId As String, Page As String, Action As String, Content As String)
        Dim appId As String = GetItemData("SELECT ApplicationId FROM Memberships WHERE UserId = '" + UCase(UserId).ToString() + "'")
        Dim mailData As DataSet = GetListData("SELECT * FROM MailConfiguration WHERE AppId = '" + UCase(appId).ToString() + "' AND Name = 'ERROR WEB' AND Active=1")

        If Not mailData.Tables.Count = 0 AndAlso mailData.Tables(0).Rows.Count > 0 Then
            Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
            Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
            Dim mailPort As String = mailData.Tables(0).Rows(0).Item("Port").ToString()
            Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
            Dim mailEnableSsl As String = mailData.Tables(0).Rows(0).Item("EnableSsl")
            Dim mailCredentials As String = mailData.Tables(0).Rows(0).Item("UseDefaultCredentials")
            Dim nailAccount As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
            Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()
            Dim mailSubject As String = mailData.Tables(0).Rows(0).Item("Subject").ToString()
            Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
            Dim mailCC As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()

            Dim vBody As String = ""
            vBody += "1. PAGE : <b>" & Page & "</b>"
            vBody += "<br />"
            vBody += "2. ACTION : <b>" & Action & "</b>"
            vBody += "<br />"
            vBody += "3. USERS : <b>" & UCase(UserId).ToString() & "</b>"
            vBody += "<br /><br />"
            vBody += "4. MESSAGE : "
            vBody += "<br />"
            vBody += Content

            Dim myMail As New MailMessage
            myMail.Subject = mailSubject
            myMail.From = New MailAddress(mailServer, mailAlias)
            myMail.To.Add(mailTo)
            myMail.Body = vBody
            myMail.IsBodyHtml = True
            Dim smtpClient As New SmtpClient()
            smtpClient.Host = mailHost
            smtpClient.EnableSsl = mailEnableSsl
            Dim NetworkCredl As New NetworkCredential()
            NetworkCredl.UserName = nailAccount
            NetworkCredl.Password = mailPassword
            smtpClient.UseDefaultCredentials = mailCredentials
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            smtpClient.Credentials = NetworkCredl
            smtpClient.Port = mailPort
            smtpClient.Send(myMail)
        End If
    End Sub

    Public Sub MailOrder(HeaderId As String, FileDirectory As String)
        Dim myData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + HeaderId + "'")

        Dim userId As String = myData.Tables(0).Rows(0).Item("UserId").ToString()
        Dim storeId As String = myData.Tables(0).Rows(0).Item("StoreId").ToString()
        Dim orderNo As String = myData.Tables(0).Rows(0).Item("OrderNo").ToString()
        Dim orderCust As String = myData.Tables(0).Rows(0).Item("OrderCust").ToString()
        Dim delivery As String = myData.Tables(0).Rows(0).Item("Delivery").ToString()

        Dim appId As String = GetItemData("SELECT ApplicationId FROM Memberships WHERE UserId = '" + UCase(userId).ToString() + "'")
        Dim mailData As DataSet = GetListData("SELECT * FROM MailConfiguration WHERE AppId = '" + UCase(appId).ToString() + "' AND Name = 'SUBMIT ORDER' AND Active=1")

        If Not mailData.Tables.Count = 0 Then
            Dim storeName As String = GetItemData("SELECT Name FROM Stores WHERE Id = '" + storeId + "'")
            Dim userMail As String = GetItemData("SELECT Email FROM Users WHERE UserId = '" + UCase(userId).ToString() + "'")
            Dim storeMail As String = GetItemData("SELECT Email FROM Stores WHERE Id = '" + storeId + "'")

            Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
            Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
            Dim mailPort As String = mailData.Tables(0).Rows(0).Item("Port").ToString()
            Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
            Dim mailEnableSsl As String = mailData.Tables(0).Rows(0).Item("EnableSsl")
            Dim mailCredentials As String = mailData.Tables(0).Rows(0).Item("UseDefaultCredentials")
            Dim nailAccount As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
            Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()

            Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
            Dim mailCc As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()
            Dim mailBcc As String = mailData.Tables(0).Rows(0).Item("Bcc").ToString()

            Dim vBody As String = "Thank you for your order."
            vBody += "<br />"
            vBody += "This is an automated message confirming the receipt of your order. Your order has been successfully registered and has been forwarded directly to our production system for processing. Please note that due to this streamlined process, we regret to inform you that we are unable to accept cancellations or modifications for this order. For any inquiries or assistance, kindly contact our office.<br /><b>Please do not reply to this email as it is unattended. We appreciate your understanding and trust in our products & services</b>."
            If userMail = "" And storeMail = "" Then
                vBody = "NEW ORDER FROM <b>" & UCase(storeName).ToString() & "</b>"
            End If
            vBody += "<br /><br />"
            vBody += "Store Order No : " & orderNo
            vBody += "<br />"
            vBody += "Store Customer : " & orderCust
            vBody += "<br />"
            vBody += "Delivery / Pick Up : " & delivery
            vBody += "<br /><br />"
            vBody += "Detail order as attached PDF."

            vBody += "<br /><br />"
            vBody += "Kind regards,"
            vBody += "<br /><br />"

            vBody += "<br /><br />"
            vBody += "<b>Sunlight Products Pty Ltd</b>"

            Dim myMail As New MailMessage
            myMail.Subject = "Order No " & orderNo & " | " & orderCust & " Confirmed"
            myMail.From = New MailAddress(mailServer, mailAlias)

            'START VALIDASI MAIL TO
            myMail.To.Add(userMail)
            If Not storeMail = "" Then
                myMail.To.Add(storeMail)
            End If
            'END VALIDASI MAIL TO

            'START VALIDASI MAIL CC
            If Not mailCc = "" Then
                Dim ccArray() As String = mailCc.Split(";")
                Dim thisMail As String = ""
                For Each thisMail In ccArray
                    myMail.CC.Add(thisMail)
                Next
            End If
            'END VALIDASI MAIL CC

            'START VALIDASI MAIL BCC
            If Not mailBcc = "" Then
                Dim bccArray() As String = mailBcc.Split(";")
                Dim thisMail As String = ""
                For Each thisMail In bccArray
                    myMail.Bcc.Add(thisMail)
                Next
            End If
            'END VALIDASI MAIL BCC

            myMail.Body = vBody
            ' SETUP ATTACMENT FILE
            Dim fileName As String = Trim("-ORDER-" & orderNo.Replace(" ", "") & "-" & storeId & ".pdf")
            myMail.Attachments.Add(New Attachment(FileDirectory & "/" & fileName))
            myMail.IsBodyHtml = True
            Dim smtpClient As New SmtpClient()
            smtpClient.Host = mailHost
            smtpClient.EnableSsl = mailEnableSsl
            Dim NetworkCredl As New NetworkCredential()
            NetworkCredl.UserName = nailAccount
            NetworkCredl.Password = mailPassword
            smtpClient.UseDefaultCredentials = mailCredentials
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            smtpClient.Credentials = NetworkCredl
            smtpClient.Port = mailPort
            smtpClient.Send(myMail)
        End If
    End Sub

    Public Sub MailVerify(UserId As String, ForgotCode As String)
        Dim appId As String = GetItemData("SELECT ApplicationId FROM Memberships WHERE UserId = '" + UCase(UserId).ToString() + "'")
        Dim mailData As DataSet = GetListData("SELECT * FROM MailConfiguration WHERE AppId = '" + UCase(appId).ToString() + "' AND Name = 'VERIFY CODE' AND Active=1")

        If Not mailData.Tables.Count = 0 Then
            Dim mailServer As String = mailData.Tables(0).Rows(0).Item("Server").ToString()
            Dim mailAlias As String = mailData.Tables(0).Rows(0).Item("Alias").ToString()
            Dim mailPort As String = mailData.Tables(0).Rows(0).Item("Port").ToString()
            Dim mailHost As String = mailData.Tables(0).Rows(0).Item("Host").ToString()
            Dim mailEnableSsl As String = mailData.Tables(0).Rows(0).Item("EnableSsl")
            Dim mailCredentials As String = mailData.Tables(0).Rows(0).Item("UseDefaultCredentials")
            Dim nailAccount As String = mailData.Tables(0).Rows(0).Item("Account").ToString()
            Dim mailPassword As String = mailData.Tables(0).Rows(0).Item("Password").ToString()
            Dim mailSubject As String = mailData.Tables(0).Rows(0).Item("Subject").ToString()
            Dim mailTo As String = mailData.Tables(0).Rows(0).Item("To").ToString()
            Dim mailCC As String = mailData.Tables(0).Rows(0).Item("Cc").ToString()

            Dim userMail As String = GetItemData("SELECT Email FROM Users WHERE UserId = '" + UCase(UserId).ToString() + "'")

            Dim myMail As New MailMessage
            myMail.Subject = mailSubject
            myMail.From = New MailAddress(mailServer, mailAlias)
            myMail.To.Add(userMail)
            myMail.Body = "Your verification code is " & ForgotCode
            myMail.IsBodyHtml = True
            Dim smtpClient As New SmtpClient()
            smtpClient.Host = mailHost
            smtpClient.EnableSsl = mailEnableSsl
            Dim NetworkCredl As New NetworkCredential()
            NetworkCredl.UserName = nailAccount
            NetworkCredl.Password = mailPassword
            smtpClient.UseDefaultCredentials = mailCredentials
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            smtpClient.Credentials = NetworkCredl
            smtpClient.Port = mailPort
            smtpClient.Send(myMail)
        End If
    End Sub

    Public Function Encrypt(clearText As String) As String
        Dim EncryptionKey As String = "BUM11ND4H9L084L"
        Dim clearBytes As Byte() = Encoding.Unicode.GetBytes(clearText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)
                    cs.Write(clearBytes, 0, clearBytes.Length)
                    cs.Close()
                End Using
                clearText = Convert.ToBase64String(ms.ToArray())
            End Using
        End Using
        Return clearText
    End Function

    Public Function Decrypt(cipherText As String) As String
        Dim EncryptionKey As String = "BUM11ND4H9L084L"
        Dim cipherBytes As Byte() = Convert.FromBase64String(cipherText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)
                    cs.Write(cipherBytes, 0, cipherBytes.Length)
                    cs.Close()
                End Using
                cipherText = Encoding.Unicode.GetString(ms.ToArray())
            End Using
        End Using
        Return cipherText
    End Function

    ' PRICING

    Public Function GetPriceGroupId(Design As String, Name As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT Id FROM PricesGroup WHERE DesignId = '" + UCase(Design).ToString() + "' AND Name = '" + Name + "'", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetGridCost(G As String, Del As String, D As Integer, W As Integer) As Decimal
        Dim result As Decimal = 0.00
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand(String.Format("SELECT TOP 1 [Cost] FROM Prices WHERE [PriceGroupId] = '{0}' AND [Type] = '{1}' AND [Drop] >= '{2}' AND Width >= '{3}' AND [Cost] > 0 ORDER BY [Drop], Width, [Cost] ASC", UCase(G).ToString(), Del, D, W), thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0)
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Sub UpdateDetailHarga(ItemId As String, Cost As Decimal)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET [Cost]=@Cost WHERE Id=@ItemId")
                myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                myCmd.Parameters.AddWithValue("@Cost", Cost)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Sub UpdateMatrix(Item As String, Qty As Integer, Matrix As Decimal)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Matrix=@Matrix, TotalMatrix=@TotalMatrix WHERE Id=@ItemId")
                Dim totalMatrix As Decimal = Qty * Matrix
                myCmd.Parameters.AddWithValue("@ItemId", Item)
                myCmd.Parameters.AddWithValue("@Matrix", Matrix)
                myCmd.Parameters.AddWithValue("@TotalMatrix", totalMatrix)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Sub UpdateDiscount(Item As String, Qty As Integer, Discount As Decimal)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Discount=@Discount, TotalDiscount=@TotalDiscount WHERE Id=@ItemId")
                Dim totalDiscount As Decimal = Qty * Discount
                myCmd.Parameters.AddWithValue("@ItemId", Item)
                myCmd.Parameters.AddWithValue("@Discount", Discount)
                myCmd.Parameters.AddWithValue("@TotalDiscount", totalDiscount)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Sub UpdateCharge(ItemId As String, Qty As Integer, Charge As Decimal)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Charge=@Charge, TotalCharge=@TotalCharge WHERE Id=@ItemId")
                Dim totalCharge As Decimal = Qty * Charge
                myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                myCmd.Parameters.AddWithValue("@Charge", Charge)
                myCmd.Parameters.AddWithValue("@TotalCharge", totalCharge)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Sub ResetPriceDetail(ItemId As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderDetailsPrice WHERE ItemId=@ItemId")
                myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Function HitungDiscount(StoreId As String, PriceGroupId As String, Matrix As Decimal) As Decimal
        Dim result As Decimal = 0.00
        Dim thisData As DataSet = GetListData("SELECT Discount FROM Discounts WHERE StoreId = '" + StoreId + "' AND PriceGroupId = '" + PriceGroupId + "' AND Active=1")
        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim discount As Integer = thisData.Tables(0).Rows(0).Item("Discount").ToString()
            result = Matrix * discount / 100
        End If
        Return result
    End Function

    Public Function HitungCustomDiscount(headerId As String, itemId As String, matrix As Decimal) As Decimal
        Dim result As Decimal = 0.00
        Dim createdDate As String = GetItemData("SELECT CreatedDate FROM OrderHeaders WHERE Id= '" + headerId + "'")
        Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE Id='" + itemId + "' AND Active=1 ORDER BY Id ASC")

        If thisData.Tables(0).Rows.Count > 0 Then
            Dim qty As String = thisData.Tables(0).Rows(0).Item("Qty").ToString()
            Dim designId As String = thisData.Tables(0).Rows(0).Item("DesignId").ToString()
            Dim blindId As String = thisData.Tables(0).Rows(0).Item("BlindId").ToString()
            Dim blindNo As String = thisData.Tables(0).Rows(0).Item("BlindNo").ToString()
            
            Dim customDisData As DataSet = GetListData("SELECT * FROM CustomDiscount WHERE DesignId='" + UCase(designId).ToString() + "' AND BlindId='" + blindId + "' AND BlindNo = '" + blindNo + "' AND Active=1 ORDER BY Id ASC")
            ' result = result + 1.00 'debug
            If customDisData.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To customDisData.Tables(0).Rows.Count - 1
                    Dim fieldName As String = customDisData.Tables(0).Rows(i).Item("FieldName").ToString()
                    Dim formula As String = customDisData.Tables(0).Rows(i).Item("Formula").ToString()
                    Dim chargeResult As String = customDisData.Tables(0).Rows(i).Item("Charge").ToString()
                    Dim fromDate As String = customDisData.Tables(0).Rows(i).Item("FromDate").ToString()
                    Dim toDate As String = customDisData.Tables(0).Rows(i).Item("ToDate").ToString()
                    Dim cekFormula As String = GetItemData("SELECT " + fieldName + " FROM view_details WHERE Id='" + itemId + "' AND " + formula)
                    Dim description As String = customDisData.Tables(0).Rows(i).Item("Description").ToString()
                    ' result = result + 2.00 'debug
                    If Not cekFormula = "" Then

                        If fromDate = "" And toDate = "" Then
                            result = matrix * chargeResult / 100
                        End If

                        '# discount start only
                        If fromDate <> "" AndAlso toDate = "" Then
                            If DateTime.Parse(createdDate) >= DateTime.Parse(fromDate) Then
                                result = matrix * chargeResult / 100
                            End If
                        End If

                        '# discount start and end
                        If fromDate <> "" AndAlso toDate <> "" Then 
                            If DateTime.Parse(createdDate) >= DateTime.Parse(fromDate) AndAlso DateTime.Parse(createdDate) < DateTime.Parse(toDate) Then
                                result = matrix * chargeResult / 100
                            End If
                        End If
                        ' Call PriceDetail(headerId, itemId, qty, description, result)
                    End If
                    Call UpdateDiscount(itemId, qty, result)
                Next
            End If
        End If
        Return result
    End Function

    
    Public Sub HitungHarga(HeaderId As String, ItemId As String)
        Dim delivery As String = GetItemData("SELECT Delivery FROM OrderHeaders WHERE Id='" + HeaderId + "'")
        Dim storeId As String = GetItemData("SELECT StoreId FROM OrderHeaders WHERE Id= '" + HeaderId + "'")

        Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE Id='" + ItemId + "' AND Active=1 ORDER BY Id ASC")
        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim kitName As String = thisData.Tables(0).Rows(0).Item("KitName").ToString()
            Dim designName As String = thisData.Tables(0).Rows(0).Item("DesignName").ToString()
            Dim blindName As String = thisData.Tables(0).Rows(0).Item("BlindName").ToString()
            Dim bracketType As String = thisData.Tables(0).Rows(0).Item("BracketType").ToString()

            Dim priceGroupId As String = thisData.Tables(0).Rows(0).Item("PriceGroupId").ToString()
            Dim qty As String = thisData.Tables(0).Rows(0).Item("Qty").ToString()
            Dim width As String = thisData.Tables(0).Rows(0).Item("Width").ToString()
            Dim drop As String = thisData.Tables(0).Rows(0).Item("Drop").ToString()
            Dim fabricType As String = thisData.Tables(0).Rows(0).Item("FabricType").ToString()
            Dim ControlType As String = thisData.Tables(0).Rows(0).Item("ControlType").ToString()
            Dim SlatQty As String = thisData.Tables(0).Rows(0).Item("SlatQty").ToString()
            Dim doorCutOut As String = thisData.Tables(0).Rows(0).Item("DoorCutOut").ToString()
            Dim sqm As String = thisData.Tables(0).Rows(0).Item("SquareMetre").ToString()
            Dim lnm As String = thisData.Tables(0).Rows(0).Item("LinearMetre").ToString()

            Dim size As String = "(" & width & " x " & drop & ")"

            '#---------------------|| Panorama Shutter Only ||---------------------#
            Dim findMetre As String = lnm
            ' If designName = "Panorama PVC Shutters" Then 
            If designName = "Panorama PVC Shutters" Then 
                width = "0" 
                drop = "0" 

                If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
                    findMetre = sqm
                End If
            End If
            '#---------------------|| /Panorama Shutter Only ||---------------------#

            Dim thisMatrix As Decimal = 0.00

            If Not priceGroupId = "" Then
                Dim getMatrix As Decimal = GetGridCost(priceGroupId, delivery, drop, width)
                
                '#---------------------For Vertical Slat Only---------------------#
                If designName = "Vertical Blinds" AndAlso blindName = "Slat Only" Then
                    getMatrix = getMatrix * Convert.ToDecimal(SlatQty)
                End If
                '#---------------------/For Vertical Slat Only---------------------#

                '#---------------------|| Panorama Shutter Only ||---------------------#
                If designName = "Panorama PVC Shutters" Then 

                    If blindName = "Hinged" And doorCutOut = "Yes" Then '#PVC Hinged with French Cut-Out
                        getMatrix = getMatrix + 30.00
                    End If

                    getMatrix = getMatrix * Convert.ToDecimal(findMetre)
                End If
                '#---------------------|| /Panorama Shutter Only ||---------------------#

                '#---------------------Discount For Store Account---------------------#
                Dim thisDiscount As Decimal = HitungDiscount(storeId, priceGroupId, getMatrix)
                thisMatrix = getMatrix - thisDiscount
                '#---------------------/Discount For Store Account---------------------#


                '#---------------------Custom Discount for Extra Discount---------------------#
                Dim thisCustomDiscount As Decimal = HitungCustomDiscount(HeaderId,ItemId, thisMatrix)
                If thisCustomDiscount > 0  Then
                    thisMatrix = thisMatrix - thisCustomDiscount
                End If
                '#---------------------/Custom Discount for Extra Discount---------------------#

                Dim description As String = kitName & " " & size

                If designName = "Roller Blinds" Then
                    description = "Roller #" & fabricType & " " & size
                End If

                If designName = "Veri Shades" Or designName = "Vertical Track Only" Then
                    description = kitName & " #" & fabricType
                    If blindName = "Veri Shades Track Only" Or blindName = "Vertical Track Only" Then
                        description = kitName
                    End If
                End If

                '#---------------------|| Panorama Shutter Only ||---------------------#
                If designName = "Panorama PVC Shutters" Then
                    description = kitName & " " & size
                    If blindName = "Hinged" And doorCutOut = "Yes"  Then
                        description = kitName & " " & size & " (Door Cut Out)"
                    End If
                End If
                '#---------------------|| /Panorama Shutter Only ||---------------------#

                Call PriceDetail(HeaderId, ItemId, qty, description, thisMatrix)
            End If
            Call UpdateMatrix(ItemId, qty, thisMatrix)
        End If
    End Sub

    Public Sub HitungSurcharge(headerId As String, itemId As String)
        Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE Id='" + itemId + "' AND Active=1 ORDER BY Id ASC")

        Dim surcharge As Decimal = 0.00

        If thisData.Tables(0).Rows.Count > 0 Then
            Dim qty As String = thisData.Tables(0).Rows(0).Item("Qty").ToString()
            Dim designId As String = thisData.Tables(0).Rows(0).Item("DesignId").ToString()
            Dim blindId As String = thisData.Tables(0).Rows(0).Item("BlindId").ToString()
            Dim cassetteExtraId As String = thisData.Tables(0).Rows(0).Item("CassetteExtraId").ToString()
            Dim brackettype As String = thisData.Tables(0).Rows(0).Item("BracketType").ToString()
            Dim blindNo As String = thisData.Tables(0).Rows(0).Item("BlindNo").ToString()
            Dim width As String = thisData.Tables(0).Rows(0).Item("Width").ToString()
            Dim drop As String = thisData.Tables(0).Rows(0).Item("Drop").ToString()
            Dim sqm As String = thisData.Tables(0).Rows(0).Item("SquareMetre").ToString()
            Dim lnm As String = thisData.Tables(0).Rows(0).Item("LinearMetre").ToString()

            Dim surchargeData As DataSet = GetListData("SELECT * FROM Surcharges WHERE DesignId='" + UCase(designId).ToString() + "' AND BlindId='" + blindId + "' AND BlindNo = '" + blindNo + "' AND Active=1 ORDER BY Id ASC")
            If surchargeData.Tables(0).Rows.Count > 0 Then
                Dim subCharge As Decimal = 0.00
                For i As Integer = 0 To surchargeData.Tables(0).Rows.Count - 1
                    Dim id As String = surchargeData.Tables(0).Rows(i).Item("Id").ToString()
                    Dim name As String = surchargeData.Tables(0).Rows(i).Item("Name").ToString()
                    Dim fieldName As String = surchargeData.Tables(0).Rows(i).Item("FieldName").ToString()
                    Dim formula As String = surchargeData.Tables(0).Rows(i).Item("Formula").ToString()
                    Dim charge As String = surchargeData.Tables(0).Rows(i).Item("Charge").ToString()
                    Dim description As String = surchargeData.Tables(0).Rows(i).Item("Description").ToString()
                    Dim thisCharge As Decimal = 0.00

                    Dim cekFormula As String = GetItemData("SELECT " + fieldName + " FROM view_details WHERE Id='" + itemId + "' AND " + formula)



                    If Not cekFormula = "" Then
                        Dim queryCharge As String = "SELECT " + charge + " FROM view_details WHERE Id='" + itemId + "'"
                        If charge = "Extra Surcharge" Then
                            queryCharge = String.Format("SELECT TOP 1 [Cost] FROM CassetteExtra WHERE [PriceGroupId] = '{0}' AND [Drop] >= '{1}' AND Width >= '{2}' AND [Cost] > 0 ORDER BY [Drop], Width, [Cost] ASC", UCase(cassetteExtraId).ToString(), drop, width)

                            If brackettype = "Headbox Only" Then
                                queryCharge = String.Format("SELECT TOP 1 [Cost] FROM CassetteExtra WHERE [PriceGroupId] = '{0}' AND Width >= '{1}' AND [Cost] > 0 ORDER BY [Drop], Width, [Cost] ASC", UCase(cassetteExtraId).ToString(), width)
                            End If
                        End If
                        'thisCharge = GetItemData(queryCharge) 'Default Code Result
                        Dim chargeResult As String = GetItemData(queryCharge)
                        Dim chargeValue As Decimal = 0D
                        If Decimal.TryParse(chargeResult, chargeValue) Then
                            thisCharge = chargeValue
                        Else
                            thisCharge = 0D ' Set default jika parsing gagal
                        End If

                        Call PriceDetail(headerId, itemId, qty, description, thisCharge)
                    End If
                    surcharge = surcharge + thisCharge
                Next
            End If
            Call UpdateCharge(itemId, qty, surcharge)
        End If
    End Sub

    Private Sub PriceDetail(Header As String, Item As String, Qty As Integer, Desc As String, Cost As Decimal)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetailsPrice VALUES(NEWID(), @HeaderId, @ItemId, @Qty, @Description, @Cost, @FinalCost)")
                Dim FinalCost As Decimal = Cost * Qty
                myCmd.Parameters.AddWithValue("@HeaderId", Header)
                myCmd.Parameters.AddWithValue("@ItemId", Item)
                myCmd.Parameters.AddWithValue("@Qty", Qty)
                myCmd.Parameters.AddWithValue("@Description", Desc)
                myCmd.Parameters.AddWithValue("@Cost", Cost)
                myCmd.Parameters.AddWithValue("@FinalCost", FinalCost)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub


    '#Create Jobs Id
    Public Function CreateJobId() As String
        Dim result As String = String.Empty
        Dim idDetail As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM Jobs ORDER BY Id DESC", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        idDetail = rdResult.Item("Id").ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        If idDetail = "" Then : result = 1
        Else : result = CInt(idDetail) + 1
        End If
        Return result
    End Function

    '#Create JoNumber
    Public Function CreateJobNumber(HeaderId As String) As String
        Dim result As String = String.Empty
        Dim jobId As Integer = 1
        Dim idDetail As String = String.Empty

        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            ' Mendapatkan JobNumber terbaru dari database
            Using myCmd As New SqlCommand("SELECT TOP 1 JoNumber FROM Jobs ORDER BY JoNumber DESC", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    If rdResult.Read() Then
                        idDetail = rdResult.Item("JoNumber").ToString()
                    End If
                End Using
            End Using
            thisConn.Close()
        End Using

        If String.IsNullOrEmpty(idDetail) Then
            ' Jika tidak ada JobNumber, buat JobNumber baru "J000001"
            result = "J" & jobId.ToString("D6")
        Else
            ' Jika JobNumber ada, ambil angka dari JobNumber dan tambah 1
            jobId = Integer.Parse(idDetail.Substring(1)) + 1
            result = "J" & jobId.ToString("D6")
        End If

        Return result
    End Function

    '#Create Jobs
    Public Sub CreateJobSheet(HeaderId As String)
        Dim JobId As String = CreateJobId()
        Dim JoNumber As String = CreateJobNumber(HeaderId)
        Call UpdateOrderHeader(HeaderId, JoNumber)
        Call CreateJobs(JoNumber, HeaderId)
        Call CreateJobDetails(HeaderId)
    End Sub


    '#Update Order Header For Jobs
    Public Sub UpdateOrderHeader(HeaderId As String, JoNumber As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET JoNumber=@JoNumber WHERE Id=@HeaderId")
                myCmd.Parameters.AddWithValue("@JoNumber", JoNumber)
                myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub


    '#Update PriceGroup On reloadPricing
    Public Sub UpdatePriceGroup(itemId As String, PriceGroupId As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET PriceGroupId=@PriceGroupId WHERE Id=@Id")
                myCmd.Parameters.AddWithValue("@Id", itemId)
                myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    '#Create Jobs
    Public Sub CreateJobs(JoNumber As String, HeaderId As String)
         Using thisConn As New SqlConnection(myConn)
            Using myCmd As New SqlCommand("InsertJobs", thisConn)
                myCmd.CommandType = CommandType.StoredProcedure
                ' Tambahkan parameter HeaderId
                myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                Try
                    ' Buka koneksi dan eksekusi perintah
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                Catch ex As Exception
                    ' Tangani error
                    Throw New Exception("Terjadi kesalahan saat memasukkan data: " & ex.Message)
                End Try
            End Using
        End Using
    End Sub

    '#Create Job Details
    Public Sub CreateJobDetails(HeaderId As String)
        Using thisConn As New SqlConnection(myConn)
            Using myCmd As New SqlCommand("InsertJobDetails", thisConn)
                myCmd.CommandType = CommandType.StoredProcedure
                myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                Try
                    ' Buka koneksi dan eksekusi perintah
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                Catch ex As Exception
                    ' Tangani error
                    Throw New Exception("Terjadi kesalahan saat memasukkan data: " & ex.Message)
                End Try
            End Using
        End Using
    End Sub


    Public Function CheckStringLayoutD(ByVal layout As String) As Boolean
        For i As Integer = 0 To layout.Length - 1
            If layout(i) = "D"c Then
                Dim hasDashBefore As Boolean = (i > 0 AndAlso layout(i - 1) = "-"c)
                Dim hasDashAfter As Boolean = (i < layout.Length - 1 AndAlso layout(i + 1) = "-"c)
                If Not (hasDashBefore Or hasDashAfter) Then
                    Return False
                End If
            End If
        Next
        Return True
    End Function



    Public Function GetPanelQty(data As String()) As Integer
        Dim result As Integer = 0

        Dim blindName As String = data(0)
        Dim panelQty As String = data(1)
        Dim layout As String = data(2)
        Dim horizontalHeight As String = data(3)

        Dim countL As Integer = 0
        Dim countR As Integer = 0
        Dim countF As Integer = 0
        Dim countM As Integer = 0
        Dim countB As Integer = 0

        countL = layout.Split("L").Length - 1
        countR = layout.Split("R").Length - 1
        countF = layout.Split("F").Length - 1
        countM = layout.Split("M").Length - 1
        If blindName = "Track Sliding" Then
            countB = layout.Split("B").Length - 1
        End If

        Dim hitung As Integer = countL + countR + countF + countM + countB
        If horizontalHeight > 0 Then
            hitung = (countL + countR + countF + countM + countB) * 2
        End If

        If blindName = "Panel Only" Then
            hitung = panelQty
        End If

        result = hitung

        Return result
    End Function


    Public Function WidthDeductPanorama(data As Object()) As Decimal
        Dim result As Decimal = 0.00

        Dim blindName As String = Convert.ToString(data(0))
        Dim type As String = Convert.ToString(data(1))
        Dim width As Integer = Convert.ToInt32(data(2))

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim hingeDeduction As Decimal = 2.5
            Dim frameDeduction As Decimal = 0
            Dim frameLDeduction As Decimal = 0
            Dim frameRDeduction As Decimal = 0
            Dim tPostDeduction As Decimal = 0
            Dim bPostDeduction As Decimal = 0
            Dim cPostDeduction As Decimal = 0
            Dim csDeduction As Decimal = 0
            Dim crDeduction As Decimal = 0
            Dim ccDeduction As Decimal = 0

            Dim jumlahT As Integer = layoutCode.Split("T").Length - 1
            Dim jumlahB As Integer = layoutCode.Split("B").Length - 1
            Dim jumlahC As Integer = layoutCode.Split("C").Length - 1
            Dim jumlahD As Integer = layoutCode.Split("D").Length - 1

            If mounting = "Inside" Then
                crDeduction = -2
                If frameType.Contains("Z Frame") Then
                    crDeduction = -3.2
                End If
            End If

            If frameType = "Beaded L 48mm" Then frameDeduction = 25.4
            If frameType = "Insert L 50mm" Then frameDeduction = 22.2
            If frameType = "Insert L 63mm" Then frameDeduction = 22.2
            If frameType = "Small Bullnose Z Frame" Then frameDeduction = 19.6
            If frameType = "Large Bullnose Z Frame" Then frameDeduction = 22.7
            If frameType = "Colonial Z Frame" Then frameDeduction = 18.4

            If Not frameLeft = "No" Then
                frameLDeduction = frameDeduction
            End If
            If Not frameRight = "No" Then
                frameRDeduction = frameDeduction
            End If

            If layoutCode.Contains("D") Then csDeduction = 3.5 / 2
            If layoutCode.Contains("T") Then tPostDeduction = 25.4 / 2

            If layoutCode.Contains("B") Then
                If frameType = "Beaded L 48mm" Or frameType = "Insert L 50mm" Or frameType = "Insert L 63mm" Or frameType = "No Frame" Then
                    bPostDeduction = 30 / 2
                End If
                If mounting = "Inside" And frameType = "Small Bullnose Z Frame" Then
                    bPostDeduction = 38 / 2
                End If
                If mounting = "Inside" And frameType = "Large Bullnose Z Frame" Then
                    bPostDeduction = 40 / 2
                End If
                If mounting = "Inside" And frameType = "Colonial Z Frame" Then
                    bPostDeduction = 34 / 2
                End If
                If mounting = "Outside" And frameType = "Beaded L 48mm" Then
                    bPostDeduction = 34.7 / 2
                End If
                If mounting = "Outside" And frameType = "Insert L 50mm" Then
                    bPostDeduction = 34.7 / 2
                End If
                If mounting = "Outside" And frameType = "Insert L 63mm" Then
                    bPostDeduction = 40.4 / 2
                End If
            End If

            If layoutCode.Contains("C") Then
                If mounting = "Inside" Then
                    If frameType = "Beaded L 48mm" Or frameType = "Insert L 50mm" Or frameType = "Insert L 63mm" Or frameType = "No Frame" Then cPostDeduction = 30 / 2
                    If frameType = "Small Bullnose Z Frame" Then cPostDeduction = 42 / 2
                    If frameType = "Large Bullnose Z Frame" Then cPostDeduction = 44 / 2
                    If frameType = "Colonial Z Frame" Then cPostDeduction = 36 / 2
                End If
                If mounting = "Outside" Then
                    If frameType = "Beaded L 48mm" Then cPostDeduction = 62.5 / 2
                    If frameType = "Insert L 50mm" Then cPostDeduction = 65 / 2
                    If frameType = "Insert L 63mm" Then cPostDeduction = 78 / 2
                End If
            End If

            If type = "All" Then
                result = width - (hingeDeduction * panelQty) - ((frameLDeduction + frameRDeduction) * panelQty) - (tPostDeduction * jumlahT) - (bPostDeduction * jumlahB) - (cPostDeduction * jumlahC) - (csDeduction * jumlahD) - (crDeduction * panelQty) - (ccDeduction * panelQty)
            End If

            If type = "Gap" Then
                result = width - (hingeDeduction * panelQty) - (frameLDeduction + frameRDeduction) - tPostDeduction - bPostDeduction - cPostDeduction - csDeduction - crDeduction - ccDeduction
            End If
        End If

        If blindName = "Track Bi-fold" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim insideDeductions As Decimal = 0.00
            Dim pivotDeductions As Decimal = 0.00
            Dim closingDeductions As Decimal = 5
            Dim frameLDeductions As Decimal = 0.00
            Dim frameRDeductions As Decimal = 0.00
            Dim hingedDeductions As Decimal = 0.00

            Dim result1 As Integer = 0
            Dim parts As String() = layoutCode.Split("/"c)
            If parts.Length > 0 Then
                result1 = CountMultiLayout(parts(0), New String() {"L", "R", "F"}) - 1
            End If

            Dim result2 As Integer = 0
            If layoutCode.Contains("/") Then
                Dim partss As String() = layoutCode.Split("/"c)
                If partss.Length > 1 Then
                    result2 = CountMultiLayout(partss(1), New String() {"L", "R", "F"}) - 1
                End If
            End If

            hingedDeductions = (result1 + result2) * 5

            If mounting = "Inside" Then insideDeductions = 2

            Dim uniqueLetters As New HashSet(Of Char)
            For Each c As Char In layoutCode.ToLower()
                If Char.IsLetter(c) Then
                    uniqueLetters.Add(c)
                End If
            Next

            pivotDeductions = 5 * uniqueLetters.Count

            If Not frameLeft = "No" Then frameLDeductions = 19
            If Not frameRight = "No" Then frameRDeductions = 19

            result = width - insideDeductions - pivotDeductions - closingDeductions - frameLDeductions - frameRDeductions - hingedDeductions
        End If

        If blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim frameLDeductions As Decimal = 0
            Dim frameRDeductions As Decimal = 0
            Dim insideDeductions As Decimal = 0

            If Not frameLeft = "No" Then frameLDeductions = 19 * panelQty
            If Not frameRight = "No" Then frameRDeductions = 19 * panelQty
            If mounting = "Inside" Then insideDeductions = 2

            result = width - frameLDeductions - frameRDeductions - insideDeductions
        End If

        If blindName = "Fixed" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            If frameType = "U Channel" Then
                Dim clearanceLDeduction As Decimal = 1
                Dim clearanceRDeduction As Decimal = 1

                Dim frameLDeduction As Decimal = 0
                Dim frameRDeduction As Decimal = 0


                If frameLeft = "L Strip" Then clearanceLDeduction = 2
                If frameRight = "L Strip" Then clearanceRDeduction = 2

                If frameLeft = "L Strip" Then frameLDeduction = 7.25
                If frameRight = "L Strip" Then frameRDeduction = 7.25

                result = width - clearanceLDeduction - clearanceRDeduction - frameLDeduction - frameRDeduction
            End If

            If frameType = "19x19 Light Block" Then
                Dim clearanceDeduction As Decimal = 5

                result = width - clearanceDeduction
            End If
        End If
        Return result
    End Function

    Public Function HeightDeductPanorama(data As Object()) As Decimal
        Dim result As Decimal = 0

        Dim blindName As String = Convert.ToString(data(0))
        Dim drop As Integer = Convert.ToInt32(data(1))
        Dim mounting As String = Convert.ToString(data(2))
        Dim frameType As String = Convert.ToString(data(3))
        Dim frameTop As String = Convert.ToString(data(4))
        Dim frameBottom As String = Convert.ToString(data(5))
        Dim bottomTrackType As String = Convert.ToString(data(6))
        Dim horizontalTPost As String = Convert.ToString(data(7))

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            Dim crDeductionTop As Decimal = 0
            Dim crDeductionBottom As Decimal = 0

            Dim cfDeductionTop As Decimal = 0
            Dim cfDeductionBottom As Decimal = 0

            Dim frameDeduction As Decimal = 0
            Dim frameTDeduction As Decimal = 0
            Dim frameBDeduction As Decimal = 0
            Dim postDeduction As Decimal = 0

            If mounting = "Inside" Then
                If frameType = "Beaded L 48mm" Or frameType = "Insert L 50mm" Or frameType = "Insert L 63mm" Or frameType = "Flat L 48mm" Then
                    If frameTop = "L Striker Plate" Then crDeductionTop = 2
                    If frameBottom = "L Striker Plate" Then crDeductionBottom = 2
                End If

                If frameType.Contains("Z Frame") Then
                    If frameTop = "L Striker Plate" Then crDeductionTop = 3.2
                    If frameBottom = "L Striker Plate" Then crDeductionBottom = 3.2
                End If
            End If

            If Not frameTop = "No" Then cfDeductionTop = 3
            If Not frameBottom = "No" Then cfDeductionBottom = 3

            If frameType = "Beaded L 48mm" Then frameDeduction = 25.4
            If frameType = "Insert L 50mm" Then frameDeduction = 22.2
            If frameType = "Insert L 63mm" Then frameDeduction = 22.2
            If frameType = "Small Bullnose Z Frame" Then frameDeduction = 19.6
            If frameType = "Large Bullnose Z Frame" Then frameDeduction = 22.7
            If frameType = "Colonial Z Frame" Then frameDeduction = 18.4

            If Not frameTop = "No" Then frameTDeduction = frameDeduction
            If Not frameBottom = "No" Then frameBDeduction = frameDeduction

            If frameTop = "L Striker Plate" Or frameTop.Contains("Sill Plate") Then
                frameTDeduction = frameDeduction + 9.5
            End If

            If frameBottom = "L Striker Plate" Or frameBottom.Contains("Sill Plate") Then
                frameBDeduction = frameDeduction + 9.5
            End If

            If horizontalTPost = "No Post" Then postDeduction = 3 / 2
            If horizontalTPost = "Yes" Then postDeduction = 25.4 / 2

            result = drop - crDeductionTop - crDeductionBottom - cfDeductionTop - cfDeductionBottom - frameTDeduction - frameBDeduction - postDeduction
        End If

        If blindName = "Track Bi-fold" Or blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            Dim crTopDeduction As Decimal = 0
            Dim crBottomDeduction As Decimal = 0
            Dim trDeduction As Decimal = 51
            Dim mtDeduction As Decimal = 0
            Dim utDeduction As Decimal = 0
            Dim utfDeduction As Decimal = 0
            Dim frameDeduction As Decimal = 0
            Dim frameTDeduction As Decimal = 0
            Dim frameBDeduction As Decimal = 0

            If mounting = "Inside" Then crTopDeduction = 1
            If bottomTrackType = "M Track" Then mtDeduction = 24
            If bottomTrackType = "U Track" Then utDeduction = 33

            If frameTop = "Yes" Then frameTDeduction = 1

            If frameType = "100mm" Then frameDeduction = 19
            If frameType = "160mm" Then frameDeduction = 19
            If frameType = "200mm" Then frameDeduction = 19

            If frameTop = "Yes" Then frameTDeduction = frameDeduction

            result = drop - crTopDeduction - crBottomDeduction - trDeduction - mtDeduction - utDeduction - utfDeduction - frameTDeduction - frameBDeduction
        End If

        If blindName = "Fixed" Then
            If frameType = "U Channel" Then
                Dim topDeduction As Decimal = 0
                Dim bottomDeduction As Decimal = 0

                If frameTop = "Yes" Then topDeduction = 17.5
                If frameBottom = "Yes" Then bottomDeduction = 17.5

                result = drop - topDeduction - bottomDeduction
            End If

            If frameType = "19x19 Light Block" Then
                result = drop - 6
            End If
        End If

        Return result
    End Function


    '#deduction added function
    Public Function CountMultiLayout(input As String, substrings As String()) As Integer
        Dim count As Integer = 0
        For Each substring In substrings
            count += input.Split(substring).Length - 1
        Next
        Return count
    End Function


End Class





    








