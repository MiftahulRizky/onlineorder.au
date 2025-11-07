Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography

Public Class SettingConfig
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
        Dim result As Integer = 0
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
            result = 0
        End Try
        Return result
    End Function

    Public Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Double = 0.00
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
            result = 0.00
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

    Public Function GenerateNewPassword(length As Integer) As String
        Dim chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim result As New StringBuilder()
        Dim crypto As New RNGCryptoServiceProvider()
        Dim data(length - 1) As Byte

        crypto.GetBytes(data)

        For Each b As Byte In data
            result.Append(chars(b Mod chars.Length))
        Next

        Return result.ToString()
    End Function

    Public Function InsertSession() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DECLARE @NewId UNIQUEIDENTIFIER = NEWID(); INSERT INTO Sessions (Id, LoginId) VALUES (@NewId, NULL); SELECT @NewId;", thisConn)
                    thisConn.Open()
                    Dim newId As Object = myCmd.ExecuteScalar()
                    thisConn.Close()

                    If newId IsNot Nothing Then
                        Dim generatedGuid As Guid = CType(newId, Guid)
                        result = generatedGuid.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return result
    End Function

    Public Sub UpdateSession(Id As String, LoginId As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Sessions SET LoginId = @LoginId WHERE Id = @Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", UCase(Id).ToString())
                    myCmd.Parameters.AddWithValue("@LoginId", UCase(LoginId).ToString())

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub DeleteSession(Id As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Sessions WHERE Id = @Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", UCase(Id).ToString())
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Function GetProductAccess() As String
        Dim result As String = String.Empty
        Try
            Dim hasil As String = String.Empty

            Dim cekDesign As DataSet = GetListData("SELECT * FROM Designs WHERE Type <> 'Additional' ORDER BY Name ASC")
            If Not cekDesign.Tables(0).Rows.Count = 0 Then
                For i As Integer = 0 To cekDesign.Tables(0).Rows.Count - 1
                    Dim id As String = cekDesign.Tables(0).Rows(i).Item("Id").ToString()
                    hasil += UCase(id).ToString() & ","
                Next
            End If

            result = hasil.Remove(hasil.Length - 1).ToString()
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

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

    Public Function GetRandomCode() As String
        Dim result As String = String.Empty
        Try
            Dim numbers As String = "1234567890"
            Dim characters As String = numbers

            Dim length As Integer = 6
            Dim otp As String = String.Empty
            For i As Integer = 0 To length - 1
                Dim character As String = String.Empty
                Do
                    Dim index As Integer = New Random().Next(0, characters.Length)
                    character = characters.ToCharArray()(index).ToString()
                Loop While otp.IndexOf(character) <> -1
                otp += character
            Next
            result = otp
        Catch ex As Exception
        End Try

        Return result
    End Function

    Public Function CreateFabricId() As String
        Dim result As String = String.Empty
        Try
            Dim id As String = String.Empty
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM Fabrics ORDER BY Id DESC", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            id = rdResult.Item("Id").ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
            If id = "" Then : result = 1
            Else : result = CInt(id) + 1
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function CreateFabricColourId() As String
        Dim result As String = String.Empty
        Try
            Dim id As String = String.Empty
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM FabricColours ORDER BY Id DESC", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            id = rdResult.Item("Id").ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
            If id = "" Then : result = 1
            Else : result = CInt(id) + 1
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function CustomerNewsletter(CustomerId As String) As Boolean
        Dim result As Boolean = False
        Try
            result = GetItemData_Boolean("SELECT Newsletter FROM Customers WHERE Id = '" + CustomerId + "'")
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Sub Log_System(data As Object())
        Try
            If data.Length = 4 Then
                Dim type As String = Convert.ToString(data(0))
                Dim dataId As String = Convert.ToString(data(1))
                Dim loginId As String = Convert.ToString(data(2))
                Dim description As String = Convert.ToString(data(3))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Log_Systems VALUES (NEWID(), @Type, @DataId, @ActionBy, GETDATE(), @Description)")
                        myCmd.Parameters.AddWithValue("@Type", type)
                        myCmd.Parameters.AddWithValue("@DataId", UCase(dataId).ToString())
                        myCmd.Parameters.AddWithValue("@ActionBy", UCase(loginId).ToString())
                        myCmd.Parameters.AddWithValue("@Description", description)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Log_Access(data As Object())
        Try
            If data.Length = 4 Then
                Dim type As String = Convert.ToString(data(0))
                Dim dataId As String = Convert.ToString(data(1))
                Dim loginId As String = Convert.ToString(data(2))
                Dim description As String = Convert.ToString(data(3))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Log_Access VALUES (NEWID(), @Type, @DataId, @ActionBy, GETDATE(), @Description)")
                        myCmd.Parameters.AddWithValue("@Type", type)
                        myCmd.Parameters.AddWithValue("@DataId", UCase(dataId).ToString())
                        myCmd.Parameters.AddWithValue("@ActionBy", UCase(loginId).ToString())
                        myCmd.Parameters.AddWithValue("@Description", description)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Log_Customer(data As Object())
        Try
            If data.Length = 4 Then
                Dim type As String = Convert.ToString(data(0))
                Dim customerId As String = Convert.ToString(data(1))
                Dim loginId As String = Convert.ToString(data(2))
                Dim description As String = Convert.ToString(data(3))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Log_Customers VALUES (NEWID(), @Type, @CustomerId, @ActionBy, GETDATE(), @Description)")
                        myCmd.Parameters.AddWithValue("@Type", type)
                        myCmd.Parameters.AddWithValue("@CustomerId", UCase(customerId).ToString())
                        myCmd.Parameters.AddWithValue("@ActionBy", UCase(loginId).ToString())
                        myCmd.Parameters.AddWithValue("@Description", description)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Log_Product(data As Object())
        Try
            If data.Length = 4 Then
                Dim type As String = Convert.ToString(data(0))
                Dim dataId As String = Convert.ToString(data(1))
                Dim loginId As String = Convert.ToString(data(2))
                Dim description As String = Convert.ToString(data(3))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Log_Products VALUES (NEWID(), @Type, @DataId, @ActionBy, GETDATE(), @Description)")
                        myCmd.Parameters.AddWithValue("@Type", type)
                        myCmd.Parameters.AddWithValue("@DataId", UCase(dataId).ToString())
                        myCmd.Parameters.AddWithValue("@ActionBy", UCase(loginId).ToString())
                        myCmd.Parameters.AddWithValue("@Description", description)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Log_Price(data As Object())
        Try
            If data.Length = 4 Then
                Dim type As String = Convert.ToString(data(0))
                Dim dataId As String = Convert.ToString(data(1))
                Dim loginId As String = Convert.ToString(data(2))
                Dim description As String = Convert.ToString(data(3))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Log_Prices VALUES (NEWID(), @Type, @DataId, @ActionBy, GETDATE(), @Description)")
                        myCmd.Parameters.AddWithValue("@Type", type)
                        myCmd.Parameters.AddWithValue("@DataId", UCase(dataId).ToString())
                        myCmd.Parameters.AddWithValue("@ActionBy", UCase(loginId).ToString())
                        myCmd.Parameters.AddWithValue("@Description", description)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Function CreateId(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Dim id As Integer = 0
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult As SqlDataReader = myCmd.ExecuteReader()
                        If rdResult.Read() Then
                            Integer.TryParse(rdResult(0).ToString(), id)
                        End If
                    End Using
                End Using
            End Using

            result = (id + 1).ToString()
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function
End Class
