Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports System.Web.Services
Imports OfficeOpenXml
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Globalization
Imports System.Linq ' Pastikan ini ada di bagian atas file Anda untuk LINQ
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports Microsoft.VisualBasic
Imports Newtonsoft.Json
Imports System.Net
Imports System.Net.Mail
Partial Class Methods_ReportProductMethod
    Inherits System.Web.UI.Page
    
    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString


    Public Class ParamListData
        Public Property field As String
        Public Property findby As String

        Public Property rolename As String
    End Class

    Public Class ParamReport
        '#Submit OrderHeader
        Public Property findby As String
        Public Property fined As String
        Public Property fromdate As String
        Public Property status As String
        Public Property todate As String
        Public Property rolename As String
    End Class

    Public Class RowReport
        Public Property DesignId As String
        Public Property DesignName As String
        Public Property Qty As Integer
    End Class

    Public Class ErrorResponse
        Public Property [error] As ErrorDetail
    End Class

    Public Class ErrorDetail
        Public Property message As String
        Public Property field As String
    End Class

    Public Class SuccessResponse
        Public Property [success] As SuccessDetail
        Public Property reportData As List(Of Dictionary(Of String, Object))
    End Class

    Public Class SuccessDetail
        Public Property message As String
        Public Property dir As String
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
                    Dim Desc As String = "'Environment : Production'"
                    If data.rolename = "Administrator" Then
                        Desc = "'Environment : Production', 'Environment : Testing', 'Environment : Development'"
                    End If

                    If data.findby = "product" Then
                        query = String.Format("SELECT Id, Name FROM Designs WHERE Active=1 AND Company <> 'LOOP' AND Description IN ({0}) ORDER BY Name ASC", Desc)
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

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function MyReport(ByVal data As ParamReport) As Object
        Try
            Dim msg As String = "200"
            Dim dir As String = "/reportproduct/"
            If String.IsNullOrEmpty(data.findby) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "findby is required !", .field = "findby"}}
            End If

            If String.IsNullOrEmpty(data.fined) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fined is required !", .field = "fined"}}
            End If

            If String.IsNullOrEmpty(data.fromdate) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fromdate is required !", .field = "fromdate"}}
            End If

            If String.IsNullOrEmpty(data.todate) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "todate is required !", .field = "todate"}}
            End If

            Dim parsedFromDate As DateTime
            Dim parsedToDate As DateTime
            If DateTime.TryParse(data.fromdate, parsedFromDate) AndAlso DateTime.TryParse(data.todate, parsedToDate) Then
                If parsedFromDate > parsedToDate Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "from date cannot be later than todate !", .field = "fromdate"}}
                End If
            Else
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Invalid date format !", .field = "date"}}
            End If

            Dim list As New List(Of Dictionary(Of String, Object))()

            If data.findby = "product" Then
                Dim finedValue As String = "NULL"
                If Not String.IsNullOrEmpty(data.fined) AndAlso data.fined <> "all" Then
                    finedValue = String.Format("'{0}'", data.fined)
                End If

                Dim fromDateSQL As String = parsedFromDate.ToString("yyyy-MM-dd")
                Dim toDateSQL As String = parsedToDate.ToString("yyyy-MM-dd")

                Dim DataReport As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM dbo.FindReportByProducts('{0}','{1}','{2}',{3}) ORDER BY YearNum ASC, MonthNum ASC, DesignName ASC", fromDateSQL, toDateSQL, data.status,finedValue))

                For Each row As DataRow In DataReport.Tables(0).Rows
                    Dim dict As New Dictionary(Of String, Object)
                    For Each col As DataColumn In DataReport.Tables(0).Columns
                        dict(col.ColumnName) = row(col)
                    Next
                    list.Add(dict)
                Next
            End If

            If data.findby = "customer" Then
                Dim dt As New DataTable()
                Using conn As New SqlConnection(myConn)
                    Using cmd As New SqlCommand("FindReportByCustomers", conn)
                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@fromdate", parsedFromDate)
                        cmd.Parameters.AddWithValue("@todate", parsedToDate)
                        cmd.Parameters.AddWithValue("@status", data.status)
                        cmd.Parameters.AddWithValue("@fined", data.fined)

                        Using da As New SqlDataAdapter(cmd)
                            da.Fill(dt)
                        End Using
                    End Using
                End Using

                For Each row As DataRow In dt.Rows
                    Dim dict As New Dictionary(Of String, Object)
                    For Each col As DataColumn In dt.Columns
                        dict(col.ColumnName) = row(col)
                    Next
                    list.Add(dict)
                Next
            End If


            Return New SuccessResponse With {
                .success = New SuccessDetail With {
                    .message = "Data loaded successfully",
                    .dir = "U"
                },
                .reportData = list
            }

        Catch ex As Exception
            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ex.Message, .field = ""}}
        End Try
    End Function
End Class
