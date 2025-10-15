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
Partial Class Methods_SettingPage_PriceMatrix_ImportMatrixMethods
    Inherits System.Web.UI.Page

    

    Shared publicCfg As New PublicConfig()

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindDesignType() As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Designs WHERE Active=1 ORDER BY Name ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("Name").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindPriceGroup(ByVal designid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM PricesGroup WHERE DesignId='" + designid + "' AND Name NOT LIKE '%Headbox%' AND Active=1 ORDER BY Name ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("Name").ToString()},
                        {"active", row("Active").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function


    '#-------------------------------|| Submit File ||---------------------------------#
    '#DEFINE CLASS FORMDATA
    Public Class FormData
        Public Property filePath  As String
        Public Property pricegroupid As String
        Public Property type As String
    End Class

    '#DEFINE CLASS RESPONSE
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

    '#SUBMIT FUNCTION
    '#this is old code if you can import excel extension .xlsx/.xlsm and do not remove this code 
    ' <WebMethod()>
    ' Public Shared Function SubmitForm(data As FormData) As Object
    '     Try
    '         Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    '         Dim filePath = HttpContext.Current.Server.MapPath("~/Temp/" & data.filePath)

    '         If Not IO.File.Exists(filePath) Then
    '             Return New ErrorResponse With {
    '                 .error = New ErrorDetail With {
    '                     .message = "Excel file not found !",
    '                     .field = "fileupload"
    '                 }
    '             }
    '         End If

    '         Dim pgId = Guid.Parse(data.pricegroupid)
    '         Dim itemType = data.type

    '         ExcelPackage.LicenseContext = LicenseContext.NonCommercial
    '         Using package As New ExcelPackage(New IO.FileInfo(filePath))
    '             Dim ws = package.Workbook.Worksheets(0)

    '             ' Ambil kolom width (baris pertama), dan baris drop (kolom pertama)
    '             Dim colStart = 2
    '             Dim rowStart = 2
    '             Dim colEnd = ws.Dimension.End.Column
    '             Dim rowEnd = ws.Dimension.End.Row

    '             Dim widths As New List(Of Integer)
    '             For c = colStart To colEnd
    '                 widths.Add(Convert.ToInt32(ws.Cells(1, c).Value))
    '             Next

    '             Using conn As New SqlConnection(myConn)
    '                 conn.Open()

    '                 For r = rowStart To rowEnd
    '                     Dim dropVal = Convert.ToInt32(ws.Cells(r, 1).Value)

    '                     For i = 0 To widths.Count - 1
    '                         Dim widthVal = widths(i)
    '                         Dim costValRaw = ws.Cells(r, i + colStart).Value

    '                         If costValRaw IsNot Nothing AndAlso IsNumeric(costValRaw) Then
    '                             Dim costVal = Convert.ToDecimal(costValRaw)

    '                             Dim cmd As New SqlCommand("INSERT INTO Prices (Id, PriceGroupId, Type, [Drop], Width, Cost) VALUES (@Id, @PriceGroupId, @Type, @Drop, @Width, @Cost)", conn)
    '                             cmd.Parameters.AddWithValue("@Id", Guid.NewGuid())
    '                             cmd.Parameters.AddWithValue("@PriceGroupId", pgId)
    '                             cmd.Parameters.AddWithValue("@Type", itemType)
    '                             cmd.Parameters.AddWithValue("@Drop", dropVal.ToString().ToUpper())
    '                             cmd.Parameters.AddWithValue("@Width", widthVal)
    '                             cmd.Parameters.AddWithValue("@Cost", costVal)

    '                             cmd.ExecuteNonQuery()
    '                         End If
    '                     Next
    '                 Next
    '             End Using
    '         End Using

    '         Return New SuccessResponse With {
    '             .success = "Data has been saved successfully."
    '         }
    '     Catch ex As Exception
    '         Return New ErrorResponse With {
    '             .error = New ErrorDetail With {
    '                 .message = ex.Message,
    '                 .field = ""
    '             }
    '         }
    '     End Try
    ' End Function


    <WebMethod()>
    Public Shared Function SubmitForm(data As FormData) As Object
        Try
            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Dim dumy As String = "2.08"
            Dim decimalDumy As Decimal = Convert.ToDecimal(dumy)
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = decimalDumy.ToString(),
                    .field = "fileupload"
                }
            }
            Dim filePath = HttpContext.Current.Server.MapPath("~/Temp/" & data.filePath)

            If Not IO.File.Exists(filePath) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "CSV file not found!",
                        .field = "fileupload"
                    }
                }
            End If

            Dim pgId = Guid.Parse(data.pricegroupid)
            Dim itemType = data.type

            ' Parse CSV file
            Using parser As New TextFieldParser(filePath)
                parser.TextFieldType = FieldType.Delimited
                parser.SetDelimiters(",")

                ' Baris pertama: header (Width)
                Dim headers = parser.ReadFields()
                Dim widths As New List(Of Integer)
                For i = 1 To headers.Length - 1
                    widths.Add(Convert.ToInt32(headers(i)))
                Next

                Using conn As New SqlConnection(myConn)
                    conn.Open()

                    ' Baris selanjutnya: Drop + Cost
                    While Not parser.EndOfData
                        Dim row = parser.ReadFields()
                        If row.Length = 0 Then Continue While

                        Dim dropVal As Integer
                        If Not Integer.TryParse(row(0), dropVal) Then Continue While

                        For i = 1 To widths.Count
                            Dim width = widths(i - 1)
                            Dim rawCost = row(i)

                            If Not String.IsNullOrWhiteSpace(rawCost) AndAlso IsNumeric(rawCost) Then
                                Dim cost = Convert.ToDecimal(rawCost)

                                Dim cmd As New SqlCommand("INSERT INTO Prices (Id, PriceGroupId, Type, [Drop], Width, Cost) VALUES (@Id, @PriceGroupId, @Type, @Drop, @Width, @Cost)", conn)
                                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid())
                                cmd.Parameters.AddWithValue("@PriceGroupId", pgId)
                                cmd.Parameters.AddWithValue("@Type", itemType)
                                cmd.Parameters.AddWithValue("@Drop", dropVal)
                                cmd.Parameters.AddWithValue("@Width", width)
                                cmd.Parameters.AddWithValue("@Cost", cost)
                                cmd.ExecuteNonQuery()
                            End If
                        Next
                    End While
                End Using
            End Using

            Return New SuccessResponse With {
                .success = "Data has been saved successfully."
            }

        Catch ex As Exception
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function
End Class
