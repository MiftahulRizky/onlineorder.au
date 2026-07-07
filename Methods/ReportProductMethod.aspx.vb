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
                    If data.findby = "product" Then
                        query = String.Format("SELECT Id, Name FROM Designs WHERE Active=1 AND Company <> 'LOOP' AND Description IN ('Environment : Production', 'Environment : Testing') ORDER BY Name ASC")
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
            End If

            If data.findby = "customer" Then
                Dim dt As New DataTable()
                Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("YourConn").ConnectionString)
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

                ' Convert ke List(Of Dictionary)
                ' Dim list As New List(Of Dictionary(Of String, Object))()

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

    
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function FindReport(ByVal data As ParamReport) As Object
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
                    Return New ErrorResponse With {  .error = New ErrorDetail With { .message = "from date cannot be later than todate !", .field = "fromdate"}}
                End If

            Else
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "Invalid date format !", .field = "date"}}
            End If

            If InArray(data.findby, "product", "customer") then
                msg = "Report is successfully prepared. <br> Click <b>OK</b> to open it."
                dir = "/reportproduct/preview"

                Dim ResPDF As String = CreatePDFReport(data.findby, data.fined, data.status, data.fromdate, data.todate)
                If Not ResPDF = "200" Then
                    Throw New Exception(ResPDF)
                End If
            End If


            Return New SuccessResponse With {.success = New SuccessDetail With {.message = msg, .dir = dir}}
        Catch ex As Exception
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message, .field = ""}}
        End Try
    End Function

    Private Shared tableStart As String = "<table style='width:100%;border:1px solid black;border-collapse:collapse;margin-bottom:15px;'>"
    Private Shared tableEnd As String = "</table>"

    Private Shared trStart As String = "<tr>"
    Private Shared trEnd As String = "</tr>"

    Private Shared thStart As String = "<th style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;word-wrap:break-word;'>"
    Private Shared thStartColSpan2 As String = "<th colspan='2' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Private Shared thStartColSpan3 As String = "<th colspan='3' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Private Shared thStartColSpan4 As String = "<th colspan='4' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Private Shared thStartColSpan8 As String = "<th colspan='8' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Private Shared thStartRowSpan2 As String = "<th rowspan='2' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;word-wrap:break-word;'>"
    Private Shared thStartRowSpan3 As String = "<th rowspan='3' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;word-wrap:break-word;'>"
    Private Shared thEnd As String = "</th>"

    Private Shared tdStart As String = "<td style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdStartRowSpan2 As String = "<td rowspan='2' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdStartColSpan2 As String = "<td colspan='2' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdStartColSpan3 As String = "<td colspan='3' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdStartColSpan4 As String = "<td colspan='4' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdEnd As String = "</td>"

    Private Shared bNotesStart As String = "<b style='margin-left:100px;color:red;'>Notes: "
    Private Shared bNotesEnd As String = "</b>"

    Private Shared spanStart As String = "<span style='font-size:12px;font-weight:bold;'>"
    Private Shared spanEnd As String = "</span>"

    Private Shared queryQtyBlind As String = "SELECT COUNT(*) FROM view_details WHERE Active = 1 {0} {1}"



    Private Shared Function CreatePDFReport(findby As String, fined As String, status As String, fromdate As String, todate As String) As String
        Try
            Dim result As String = ""
            Dim directory As String = HttpContext.Current.Server.MapPath("~/file/report")
            Dim filename As String = ""
            Dim Query As String = ""

            ' If findby = "product" Then
            '     Dim bydesign As String = ""
            '     If Not fined = "all" Then
            '         bydesign = String.Format("AND DesignId = '{0}'",fined)
            '     End If
                
            '     Dim Title As String = String.Format("{0} / {1}", Convert.ToDateTime(fromdate).ToString("dd MMM yyyy"), Convert.ToDateTime(todate).ToString("dd MMM yyyy"))
            '     If fromdate = todate Then
            '         Title = String.Format("On {0}", Convert.ToDateTime(fromdate).ToString("dd MMM yyyy"))
            '     End IF

            '     Dim ProductData As DataSet = publicCfg.GetListData(String.Format("SELECT DesignId, DesignName, SUM(Qty) As Qty FROM view_details WHERE Active = 1 AND Status = '{3}' AND SubmittedDate >= '{0}' AND SubmittedDate < DATEADD(DAY, 1, '{1}') {2} GROUP BY DesignId, DesignName ORDER BY DesignName ASC ", fromdate, todate, bydesign, status))
            '     Dim ProductCount As Integer = ProductData.Tables(0).Rows.Count

            '     result += spanStart & Title & spanEnd
            '     result += tableStart
            '     result += trStart
            '     result += thStart & "No" & thEnd
            '     result += thStart & "Products" & thEnd
            '     result += thStart & "Qty" & thEnd
            '     result += trEnd
            '     If Not ProductCount = 0 Then

            '         For i As Integer = 0 To ProductCount - 1
            '             Dim ProductName As String = ProductData.Tables(0).Rows(i).Item("DesignName").ToString()
            '             Dim Qty As String = ProductData.Tables(0).Rows(i).Item("Qty").ToString()
            '             result += trStart
            '             result += tdStart & (i + 1) & tdEnd
            '             result += tdStart & ProductName & tdEnd
            '             result += tdStart & If(Qty = "", "0", Qty) & tdEnd
            '             result += trEnd
            '         Next
            '         result += tableEnd
            '     Else
            '         result += trStart
            '         result += tdStartColSpan3 & "Data Not Found :(" & tdEnd
            '         result += trEnd
            '         result += tableEnd
            '     End If

            '     filename = "Report By Products.pdf"
            ' End If

            If findby = "product" Then
                Dim bydesign As String = ""
                If Not fined = "all" Then
                    bydesign = String.Format("AND DesignId = '{0}'", fined)
                End If
                
                Dim Title As String = String.Format("{2} - {0} / {1}", Convert.ToDateTime(fromdate).ToString("dd MMM yyyy"), Convert.ToDateTime(todate).ToString("dd MMM yyyy"), status)
                If fromdate = todate Then
                    Title = String.Format("{1} On {0}", Convert.ToDateTime(fromdate).ToString("dd MMM yyyy"), status)
                End If

                ' --- PERUBAHAN SQL: Tambahkan MonthNum, MonthName, dan YearNum ---
                ' Menggunakan Datename(month, ...) untuk mengambil nama bulan (Januari, February, dll)
                Dim sql As String = "SELECT " &
                                    "  MONTH(SubmittedDate) As MonthNum, " &
                                    "  YEAR(SubmittedDate) As YearNum, " &
                                    "  DATENAME(MONTH, SubmittedDate) As MonthName, " &
                                    "  DesignId, DesignName, SUM(Qty) As Qty " &
                                    "FROM view_details " &
                                    "WHERE Active = 1 AND Status = '{3}' AND (OrderNumber COLLATE SQL_Latin1_General_CP1_CI_AS NOT LIKE '%test%' AND OrderName COLLATE SQL_Latin1_General_CP1_CI_AS NOT LIKE '%test%') " &
                                    "  AND SubmittedDate >= '{0}' AND SubmittedDate < DATEADD(DAY, 1, '{1}') {2} " &
                                    "GROUP BY YEAR(SubmittedDate), MONTH(SubmittedDate), DATENAME(MONTH, SubmittedDate), DesignId, DesignName " &
                                    "ORDER BY YearNum ASC, MonthNum ASC, DesignName ASC"

                Dim ProductData As DataSet = publicCfg.GetListData(String.Format(sql, fromdate, todate, bydesign, status))
                Dim ProductCount As Integer = ProductData.Tables(0).Rows.Count

                result += spanStart & Title & spanEnd

                If Not ProductCount = 0 Then
                    Dim currentMonthYear As String = ""
                    Dim isTableOpen As Boolean = False
                    Dim nomorUrut As Integer = 1

                    For i As Integer = 0 To ProductCount - 1
                        Dim row As DataRow = ProductData.Tables(0).Rows(i)
                        
                        ' Ambil nama bulan dan tahun (Contoh: "January 2026")
                        Dim rowMonthYear As String = row("MonthName").ToString() & " " & row("YearNum").ToString()
                        Dim ProductName As String = row("DesignName").ToString()
                        Dim Qty As String = row("Qty").ToString()

                        ' --- LOGIK PECAH BULAN ---
                        ' Jika berganti bulan, tutup tabel lama (jika ada) dan buat judul bulan baru beserta Header Tabelnya
                        If rowMonthYear <> currentMonthYear Then
                            if isTableOpen Then
                                result += tableEnd ' Tutup tabel bulan sebelumnya
                                result += "<br/>"  ' Kasih jarak antar bulan
                            End If

                            currentMonthYear = rowMonthYear
                            nomorUrut = 1 ' Reset nomor urut jadi 1 lagi di bulan baru
                            
                            ' Tulis Nama Bulan
                            result += String.Format("<h4>{0}</h4>", currentMonthYear)
                            
                            ' Buka Tabel Baru dan Header-nya
                            result += tableStart
                            result += trStart
                            result += thStart & "No" & thEnd
                            result += thStart & "Products" & thEnd
                            result += thStart & "Qty" & thEnd
                            result += trEnd
                            isTableOpen = True
                        End If

                        ' Isi Baris Data
                        result += trStart
                        result += tdStart & nomorUrut & tdEnd
                        result += tdStart & ProductName & tdEnd
                        result += tdStart & If(Qty = "", "0", Qty) & tdEnd
                        result += trEnd
                        
                        nomorUrut += 1
                    Next

                    ' Jangan lupa tutup tabel terakhir setelah loop selesai
                    If isTableOpen Then
                        result += tableEnd
                    End If

                Else
                    ' Jika sama sekali tidak ada data dari rentang tgl tersebut
                    result += tableStart
                    result += trStart
                    result += tdStartColSpan3 & "Data Not Found :(" & tdEnd
                    result += trEnd
                    result += tableEnd
                End If

                filename = "Report By Products.pdf"
            End If

            If findby = "customer" Then
                ' Return "200"
                Dim Title As String = String.Format("{0} / {1}", Convert.ToDateTime(fromdate).ToString("dd MMM yyyy"), Convert.ToDateTime(todate).ToString("dd MMM yyyy"))
                If fromdate = todate Then
                    Title = String.Format("On ", Convert.ToDateTime(fromdate).ToString("dd MMM yyyy"))
                End IF

                Dim byCustomer As String = String.Format("AND StoreId = '{0}'", fined)
                If fined = "all" Then
                    byCustomer = ""
                End If
                Dim CustomerData As DataSet = publicCfg.GetListData(String.Format("SELECT StoreId As CustomerId, StoreName As CustomerName FROM view_headers WHERE Active = 1 AND Status = '{3}' AND SubmittedDate >= '{0}' AND SubmittedDate < DATEADD(DAY, 1, '{1}') AND (OrderNo COLLATE SQL_Latin1_General_CP1_CI_AS NOT LIKE '%test%' AND OrderCust COLLATE SQL_Latin1_General_CP1_CI_AS NOT LIKE '%test%') {2} GROUP BY StoreId, StoreName ORDER BY CustomerName ASC", fromdate, todate, byCustomer, status))

                Dim ProductData As DataSet = publicCfg.GetListData(String.Format("SELECT Id, Name FROM Designs WHERE Active=1 AND Company <> 'LOOP' AND Description IN ('Environment : Production') ORDER BY Name ASC"))
                Dim ProductDataCount As Integer = ProductData.Tables(0).Rows.Count


                Dim thProduct As String = String.Format("<th colspan='{0}' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>", ProductDataCount)
                result += spanStart & Title & spanEnd
                result += tableStart
                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "Customers" & thEnd
                result += thProduct & "Products" & thEnd
                result += trEnd

                result += trStart
                For h As Integer = 0 To ProductDataCount - 1
                    Dim Name As String = ProductData.Tables(0).Rows(h).Item("Name").ToString()
                    result += thStart & Name & thEnd
                Next
                result += trEnd
                If Not CustomerData.Tables(0).Rows.Count = 0 Then

                    For i As Integer = 0 To CustomerData.Tables(0).Rows.Count - 1
                        Dim CustomerName As String = CustomerData.Tables(0).Rows(i).Item("CustomerName").ToString

                        result += trStart
                        result += tdStart & i + 1 & tdEnd
                        result += tdStart & CustomerName & tdEnd
                        For k As Integer = 0 To ProductDataCount - 1
                            Dim DesignId As String = ProductData.Tables(0).Rows(k).Item("Id").ToString()
                            Dim Qty As String = publicCfg.GetItemData(String.Format("SELECT SUM(Qty) FROM view_details WHERE DesignId='{0}' AND Active = 1 AND Status = '{4}' AND SubmittedDate >= '{1}' AND SubmittedDate < DATEADD(DAY, 1, '{2}') AND CustomerName = '{3}' AND (OrderNumber COLLATE SQL_Latin1_General_CP1_CI_AS NOT LIKE '%test%' AND OrderName COLLATE SQL_Latin1_General_CP1_CI_AS NOT LIKE '%test%') ", DesignId, fromdate, todate, CustomerName, status))
                            result += tdStart & If(Qty = "", "0", Qty) & tdEnd
                        Next
                        result += trEnd
                    Next
                    result += tableEnd
                
                Else

                    Dim tdProduct As String = String.Format("<td colspan='{0}' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>", ProductDataCount+2)

                    result += trStart
                    result += tdProduct & "Data Not Found :(" & tdEnd
                    result += trEnd
                    result += tableEnd

                End If

                filename = "Report By Customer.pdf"
            End If

            HttpContext.Current.Session("ReportPreview") = filename
            Using stream As FileStream = New FileStream(directory + "/" + filename, FileMode.Create)
                Dim pdfDoc As Document = New Document(PageSize.A4.Rotate)
                Dim writer As PdfWriter = PdfWriter.GetInstance(pdfDoc, stream)
                pdfDoc.Open()
                Dim sr As StringReader = New StringReader(result)
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr)
                pdfDoc.NewPage()
                pdfDoc.Close()
                stream.Close()
            End Using
            Return "200"
        Catch ex As Exception
            Return "ERROR ThisCreatePDFOrder: " & ex.Message ' biar kelihatan errornya
        End Try
    End Function


End Class
