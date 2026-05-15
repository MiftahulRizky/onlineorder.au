Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_ReportProductMethod
    Inherits System.Web.UI.Page
    
    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamListData
        Public Property field As String
        Public Property findby As String
    End Class

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
    Public Shared Function BindListData(ByVal data As ParamListData) As Object
        Try
            Dim query As String = ""
            Dim resultList As New List(Of Dictionary(Of String, String))()

            Select Case data.field.ToLower()
                Case "fined"
                    If data.findby = "product" Then
                        query = String.Format("SELECT Id, Name FROM Designs WHERE Active=1 ORDER BY Name ASC")
                    End If

                    If data.findby = "customer" Then
                        query = String.Format("SELECT Id, Name FROM Customers WHERE Active=1 ORDER BY Name ASC")
                    End If
                    Return GetFormattedData(query, "Id", "Name")

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

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function
End Class
