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
Partial Class Methods_Setting_Cutomer_CustomerMethod
    Inherits System.Web.UI.Page
    Shared publicCfg As New PublicConfig()

    Public Class ServerSideParams
        ' Public Property designid As String
        ' Public Property blindid As String
        
        '# Parameter DataTables untuk server-side processing
        Public Property draw As Integer
        Public Property start As Integer
        Public Property length As Integer
        Public Property order As List(Of OrderParam)
        Public Property columns As List(Of ColumnParam)
        Public Property search As SearchParam
    End Class

    Public Class OrderParam
        Public Property column As Integer
        Public Property dir As String ' "asc" or "desc"
    End Class

    Public Class ColumnParam
        Public Property data As String
        Public Property name As String
        Public Property searchable As Boolean
        Public Property orderable As Boolean
        Public Property search As SearchParam
    End Class

    Public Class SearchParam
        Public Property value As String
        Public Property regex As Boolean
    End Class

    Public Class DataTableResponse
        Public Property draw As Integer
        Public Property recordsTotal As Integer
        Public Property recordsFiltered As Integer
        Public Property data As List(Of ServerSideReturnRow)
    End Class

    Public Class ServerSideReturnRow
        Public Property No As String 
        Public Property Id As String 
        Public Property ExactId As String
        Public Property Name As String
        Public Property CustomerGroup As String 
        Public Property CustomerCashSale As String 
        Public Property CustomerOnStop As String
        Public Property CustomerMinSurcharge As String
        Public Property DataActive As String
    End Class

    ' Public Class FormData
    '     Public Property id As String
    '     Public Property soeid As String
    '     Public Property name As String
    '     Public Property designtype As String
    '     Public Property blindtype As String
    '     Public Property bracket As String
    '     Public Property tube As String
    '     Public Property control As String
    '     Public Property colour As String
    '     Public Property des As String
    '     Public Property active As String
    ' End Class

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
    Public Shared Function CustomerServerSide(params As ServerSideParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of ServerSideReturnRow)()

        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()

                

                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT Customers.*, CASE WHEN Customers.CashSale = 1 THEN 'Yes' ELSE 'No' END AS CustomerCashSale, CASE WHEN Customers.OnStop = 1 THEN 'Yes' ELSE 'No' END AS CustomerOnStop, CASE WHEN Customers.MinimumOrderSurcharge = 1 THEN 'Yes' ELSE 'No' END AS CustomerMinSurcharge, CustomerGroups.Name AS CustomerGroup, CASE WHEN Customers.Active = 1 THEN 'Yes' WHEN Customers.Active = 0 THEN 'No' ELSE 'Error' END AS DataActive FROM Customers LEFT JOIN CustomerGroups ON CustomerGroups.Id = Customers.[Group]"
                Using countCmd As New SqlCommand(countSql, conn)
                    ' countCmd.Parameters.AddWithValue("@DesignId", params.designid)
                    ' countCmd.Parameters.AddWithValue("@BlindId", params.blindid)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT Customers.*, CASE WHEN Customers.CashSale = 1 THEN 'Yes' ELSE 'No' END AS CustomerCashSale, CASE WHEN Customers.OnStop = 1 THEN 'Yes' ELSE 'No' END AS CustomerOnStop, CASE WHEN Customers.MinimumOrderSurcharge = 1 THEN 'Yes' ELSE 'No' END AS CustomerMinSurcharge, CustomerGroups.Name AS CustomerGroup, CASE WHEN Customers.Active = 1 THEN 'Yes' WHEN Customers.Active = 0 THEN 'No' ELSE 'Error' END AS DataActive")
                sqlBuilder.AppendLine("FROM Customers LEFT JOIN CustomerGroups ON CustomerGroups.Id = Customers.[Group]")
                ' sqlBuilder.AppendLine("WHERE HardwareKits.DesignId = @DesignId AND HardwareKits.BlindId = @BlindId")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                ' cmd.Parameters.AddWithValue("@DesignId", params.designid)
                ' cmd.Parameters.AddWithValue("@BlindId", params.blindid)



                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    whereClause.AppendLine(" AND ( Customers.Id LIKE @SearchValue OR Customers.MicronetId LIKE @SearchValue OR Customers.ExactId LIKE @SearchValue OR Customers.Name LIKE @SearchValue OR Customers.Type LIKE @SearchValue OR Customers.Name LIKE @SearchValue) ")
                    cmd.Parameters.AddWithValue("@SearchValue", searchValue)
                End If

                sqlBuilder.Append(whereClause.ToString())
                
                ' --- Query untuk menghitung Filtered Records ---
                Dim filteredCountSql As String = "SELECT COUNT(T.Id) FROM (" & sqlBuilder.ToString() & ") AS T"
                Using filteredCountCmd As New SqlCommand(filteredCountSql, conn)
                    For Each p As SqlParameter In cmd.Parameters
                        filteredCountCmd.Parameters.Add(New SqlParameter(p.ParameterName, p.Value))
                    Next
                    filteredRecords = CInt(filteredCountCmd.ExecuteScalar())
                End Using
                

                ' ... kode sebelumnya ...
                Dim orderByClause As New System.Text.StringBuilder()
                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    ' Perbaiki bagian ini:
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Id"}, _
                        {2, "Name"}, _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(" ORDER BY Customers.Name ASC")
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(" ORDER BY Customers.Name ASC")
                End If
                sqlBuilder.Append(orderByClause.ToString())
                
                ' ... kode selanjutnya ...

                ' --- Tambahkan Pagination (OFFSET/FETCH NEXT untuk SQL Server 2012+) ---
                sqlBuilder.AppendLine(" OFFSET " & params.start.ToString() & " ROWS FETCH NEXT " & params.length.ToString() & " ROWS ONLY")

                cmd.CommandText = sqlBuilder.ToString()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim noCounter As Integer = params.start + 1 ' Mulai hitung dari offset

                    While reader.Read()
                        Dim row As New ServerSideReturnRow With {
                            .No = noCounter.ToString(),
                            .Id = reader("Id").ToString(),
                            .ExactId = reader("ExactId").ToString(),
                            .Name = reader("Name").ToString(),
                            .CustomerGroup = reader("CustomerGroup").ToString(),
                            .CustomerCashSale = reader("CustomerCashSale").ToString(),
                            .CustomerOnStop = reader("CustomerOnStop").ToString(),
                            .DataActive = reader("DataActive").ToString()
                        }
                        resultList.Add(row)
                        noCounter += 1
                    End While
                End Using

            End Using

            ' --- Siapkan Respons ---
            response.draw = params.draw
            response.recordsTotal = totalRecords
            response.recordsFiltered = filteredRecords
            response.data = resultList

            Return response

        Catch ex As Exception
            response.draw = If(params Is Nothing, 0, params.draw)
            response.recordsTotal = 0
            response.recordsFiltered = 0
            response.data = New List(Of ServerSideReturnRow)()
            ' Untuk debugging, bisa kirim error ke client, tapi jangan di production
            ' response.error = ex.Message
            Return response
        End Try
    End Function
End Class
