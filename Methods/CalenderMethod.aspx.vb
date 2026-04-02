Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_CalenderMethod
    Inherits System.Web.UI.Page
    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        Public Property key As String
        Public Property notes As String
    End Class

    '#--- Kelas Output WebMethod ---#
    Public Class ErrorDetail
        Public Property message As String
        Public Property field As String
    End Class

    Public Class ErrorResponse
        Public Property [error] As ErrorDetail
    End Class

    Public Class SuccessResponse
        Public Property success As String
    End Class

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetItemData(ByVal query As String) As Object
        Try
            Dim Item As String = publicCfg.GetItemData(query)
            Return Item
        Catch ex As Exception
            Return "ERROR: " & ex.Message ' biar kelihatan errornya
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Delete(ByVal key As String) As Object
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("DELETE FROM Callenders WHERE [Key] = @Key", thisConn)
                    myCmd.Parameters.AddWithValue("@Key", key)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
            Return "200"
        Catch ex As Exception
            Return "ERROR: " & ex.Message ' biar kelihatan errornya
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Save(ByVal data As ParamSubmit) As Object
        Try
            Dim msg = "Notes has been saved"

            Dim Key As String = publicCfg.GetItemData(String.Format("SELECT [Key] FROM Callenders WHERE [Key] = '{0}'", data.key))
            Dim MyQuery As String = String.Format("INSERT INTO Callenders (Id, [Key], Description) VALUES (NEWID(), @Key, @Description)")
            If Not Key = "" Then
                MyQuery = String.Format("UPDATE Callenders SET Description = '{0}' WHERE [Key] = '{1}'", data.notes, Key)
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand(MyQuery, thisConn)
                    myCmd.Parameters.AddWithValue("@Description", data.notes)
                    myCmd.Parameters.AddWithValue("@Key", data.key)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message, .field = ""}}
        End Try
    End Function 
End Class
