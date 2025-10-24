Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports OfficeOpenXml

Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic


Partial Class Methods_SettingPage_CassetteExtra_CassetteExtraMethod
    Inherits System.Web.UI.Page
    Shared publicCfg As New PublicConfig()


    '#--------------|| define class response ||--------------#
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
    Public Shared Function BindPriceGroup() As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM PricesGroup WHERE Name LIKE '%Headbox%' ORDER BY Name ASC")
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


    Public Class CassetteRow
        Public Property No As String
        Public Property PriceGroupName As String
        Public Property Width As String
        Public Property Drop As String
        Public Property Cost As String
        Public Property Id As String
    End Class

    <WebMethod()>
    Public Shared Function BindCassetteExtra(ByVal pricegroupid As String, ByVal width As String, ByVal drop As String) As Object
        Dim resultList As New List(Of CassetteRow)()
        Dim no As Integer = 1

        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()
                Dim sql As String = "SELECT CassetteExtra.*, PricesGroup.Name AS PriceGroupName FROM CassetteExtra INNER JOIN PricesGroup ON PricesGroup.Id = CassetteExtra.PriceGroupId"

                IF pricegroupid <> ""  Or width <> "" Or drop <> "" Then
                    sql = "SELECT CassetteExtra.*, PricesGroup.Name AS PriceGroupName FROM CassetteExtra INNER JOIN PricesGroup ON PricesGroup.Id = CassetteExtra.PriceGroupId WHERE CassetteExtra.PriceGroupId = @PriceGroupId AND CassetteExtra.Width >= @Width AND CassetteExtra.[Drop] >= @Drop ORDER BY PriceGroupName, CassetteExtra.Width, CassetteExtra.[Drop], CassetteExtra.Cost ASC"
                End If
                
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@PriceGroupId", pricegroupid)
                    cmd.Parameters.AddWithValue("@Width", width)
                    cmd.Parameters.AddWithValue("@Drop", drop)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim row As New CassetteRow With {
                                .No = no.ToString(),
                                .PriceGroupName = reader("PriceGroupName").ToString(),
                                .Width = reader("Width").ToString(),
                                .Drop = reader("Drop").ToString(),
                                .Cost = reader("Cost").ToString(),
                                .Id = reader("Id").ToString()
                            }

                            resultList.Add(row)
                            no += 1
                        End While
                    End Using
                End Using
            End Using

            Return New With {
                .data = resultList
            }

        Catch ex As Exception
            Return New With {
                .data = New List(Of Object),
                .error = ex.Message
            }
        End Try
    End Function


    '#-------------------------------|| Submit Function ||---------------------------------#
    '#-------------------------------|| Import CSV ||---------------------------------#
    '#DEFINE CLASS FORMDATA
    Public Class FormData
        Public Property filePath  As String
        Public Property pricegroupid As String
    End Class

    <WebMethod()>
    Public Shared Function ImportCSV(data As FormData) As Object
        Try
            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Dim filePath = HttpContext.Current.Server.MapPath("~/Temp/" & data.filePath)

            If Not IO.File.Exists(filePath) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "CSV file not found!",
                        .field = "#modalImport #fileupload"
                    }
                }
            End If

            Dim pgId = Guid.Parse(data.pricegroupid)

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

                                Dim cmd As New SqlCommand("INSERT INTO CassetteExtra (Id, PriceGroupId, [Drop], Width, Cost) VALUES (@Id, @PriceGroupId, @Drop, @Width, @Cost)", conn)
                                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid())
                                cmd.Parameters.AddWithValue("@PriceGroupId", pgId)
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
                .success = "Import data successfully."
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
