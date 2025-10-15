' App_Code/DownloadCSV.vb
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports Microsoft.VisualBasic
Public Class DownloadCSV
    Shared publicCfg As New PublicConfig()
    ' Public Shared Function GetListData(thisString As String) As DataSet
    '     Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    '     Dim thisCmd As New SqlCommand(thisString)
    '     Using thisConn As New SqlConnection(myConn)
    '         Using thisAdapter As New SqlDataAdapter()
    '             thisCmd.Connection = thisConn
    '             thisAdapter.SelectCommand = thisCmd
    '             Using thisDataSet As New DataSet()
    '                 thisAdapter.Fill(thisDataSet)
    '                 Return thisDataSet
    '             End Using
    '         End Using
    '     End Using
    ' End Function



    Public Shared Sub ExportOrderToCSV(response As HttpResponse, HeaderId As String)
        ' Simulasi data
        Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id = '" + HeaderId + "'")
        Dim Blank As String = ""
        Dim ordercust As String = headerData.Tables(0).Rows(0).Item("OrderCust").ToString
        Dim csv As New StringBuilder()
        csv.AppendLine("HEADER,STORE ID,STORE ORDER NO,STORE CUTOMER,BARCODE,INVOICE,WORK ORDER")
        csv.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",
            blank,
            headerData.Tables(0).Rows(0).Item("StoreId").ToString(),
            headerData.Tables(0).Rows(0).Item("OrderNo").ToString(),
            ordercust,
            ordercust,
            ordercust,
            ordercust
        ))

        Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId = '" + HeaderId + "' AND Active='1' ORDER BY Id, BlindNo, DesignName ASC")

        For Each row As DataRow In detailData.Tables(0).Rows
            csv.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",
                row.Item("DesignName").ToString(),
                row.Item("Id").ToString(),
                row.Item("Mounting").ToString(),
                row.Item("Width").ToString(),
                row.Item("Drop").ToString(),
                row.Item("Location").ToString(),
                row.Item("Qty").ToString()
            ))
        Next

        ' Output ke response
        response.Clear()
        response.ContentType = "text/csv"
        response.AddHeader("Content-Disposition", "attachment; filename=-SPOD-ID-" & HeaderId & "-" & ordercust & ".csv")
        response.ContentEncoding = Encoding.UTF8
        response.Write(csv.ToString())
        response.End()
    End Sub

End Class
