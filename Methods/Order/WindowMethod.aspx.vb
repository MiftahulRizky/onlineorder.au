Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_Order_WindowMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

     

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


    ' <WebMethod()>
    ' <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    ' Public Shared Function BindListData(ByVal field As String, ByVal designid As String, ByVal blindtype As String) As Object
    '     Try
    '         Dim Query As String
    '         Dim List As New List(Of Dictionary(Of String, String))()

    '         If field = "blindtype" Then
    '             Query = String.Format("SELECT * FROM Blinds WHERE DesignId='{0}' AND Active=1 ORDER BY Name ASC", designid)
    '             Dim datas As DataSet = publicCfg.GetListData(Query)
    '             If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
    '                 For Each row As DataRow In datas.Tables(0).Rows
    '                     Dim result As New Dictionary(Of String, String) From {
    '                         {"value", row("Id").ToString()},
    '                         {"text", row("Name").ToString()}
    '                     }
    '                     List.Add(result)
    '                 Next
    '             End If
    '         End If

    '         If field = "tubetype" Then
    '             Query = String.Format("SELECT * FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND Active=1 ORDER BY Name ASC", designid, UCase(blindtype).ToString())
    '             Dim datas As DataSet = publicCfg.GetListData(Query)
    '             If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
    '                 For Each row As DataRow In datas.Tables(0).Rows
    '                     Dim result As New Dictionary(Of String, String) From {
    '                         {"value", row("Id").ToString()},
    '                         {"text", row("TubeType").ToString()}
    '                     }
    '                     List.Add(result)
    '                 Next
    '             End If
    '         End If

    '         Return list
    '     Catch ex As Exception
    '         Return New With {.error = ex.Message}
    '     End Try
    ' End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindListData(ByVal field As String, ByVal designid As String, ByVal blindtype As String) As Object
        Try
            Dim query As String = ""
            Dim resultList As New List(Of Dictionary(Of String, String))()

            Select Case field.ToLower()
                Case "blindtype"
                    query = String.Format("SELECT Id, Name FROM Blinds WHERE DesignId='{0}' AND Active=1 ORDER BY Name ASC", designid)
                    Return GetFormattedData(query, "Id", "Name")

                Case "tubetype"
                    query = String.Format("SELECT Id, TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND Active=1 ORDER BY Name ASC", designid, UCase(blindtype).ToString())
                    Return GetFormattedData(query, "Id", "TubeType")

                Case Else
                    Return New With {.error = "Invalid field"}
            End Select

        Catch ex As Exception
            Return New With {.error = ex.Message}
        End Try
    End Function

    Private Shared Function GetFormattedData(query As String, valueField As String, textField As String) As Object
        Dim list As New List(Of Dictionary(Of String, String))()

        Dim datas As DataSet = publicCfg.GetListData(query)

        If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
            For Each row As DataRow In datas.Tables(0).Rows
                list.Add(New Dictionary(Of String, String) From {
                    {"value", row(valueField).ToString()},
                    {"text", row(textField).ToString()}
                })
            Next
        End If

        Return list
    End Function

    
End Class
