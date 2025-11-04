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
Partial Class Methods_Setting_Customer_Detail
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()

    Public Class ServerSideParams
        Public Property customerid As String

        
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

        '#ContactServerSide
        Public Property Id As String 
        Public Property Name As String
        Public Property Role As String 
        Public Property Salutation As String 
        Public Property Email As String
        Public Property Phone As String
        Public Property Mobile As String
        Public Property Tags As String
        Public Property Note As String
        Public Property Primary As String

        '#AddressServerSide
        ' Public Property Id As String => On #ContactServerSide
        Public Property Description As String
        Public Property Address As String
        Public Property Port As String
        ' Public Property Tags As String => On #ContactServerSide
        Public Property Instruction As String
        ' Public Property Primary As String => On #ContactServerSide


        '#LoginServerSide
        Public Property CustomerId As String
        Public Property Application As String
        ' Public Property Role As String => On #ContactServerSide
        Public Property Username As String
        Public Property LastLogin As String
        Public Property Active As String

        '#DiscountsServerSide
        ' Public Property Id As String => On #ContactServerSide
        Public Property Title As String
        Public Property Discount As String
        Public Property StartDate As String
        Public Property EndDate As String
        Public Property FinalDiscount As String

        '#ProductAccessServerSide
        ' Public Property Id As String => On #ContactServerSide
        Public Property Product As String

        '#QuotesServerSide
        ' Public Property Id As String => On #ContactServerSide
        Public Property Logo As String
        Public Property Terms As String

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
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function bindCustomerDetail(ByVal id As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT Customers.*, CustomerGroups.Name AS CustomerGroup, CustomerPriceGroups.Name AS CustomerPriceGroup FROM Customers LEFT JOIN CustomerGroups ON CustomerGroups.Id = Customers.[Group] LEFT JOIN CustomerPriceGroups ON CustomerPriceGroups.Id = Customers.Pricing WHERE Customers.Id = '" + id + "'")

            Dim data As DataSet = DirectCast(datas, DataSet)
            Dim resultList As New List(Of Dictionary(Of String, String))()

            If data IsNot Nothing AndAlso data.Tables.Count > 0 Then
                For Each row As DataRow In data.Tables(0).Rows
                    Dim dict As New Dictionary(Of String, String)()
                    For Each col As DataColumn In data.Tables(0).Columns
                        dict(col.ColumnName) = row(col).ToString()
                    Next
                    resultList.Add(dict)
                Next
            End If

            Return resultList
        Catch ex As Exception
            ' Tangani error agar bisa dikenali di JavaScript
            Return New With {.error = True, .message = ex.Message}
        End Try
    End Function


    <WebMethod()>
    Public Shared Function ContactServerSide(params As ServerSideParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of ServerSideReturnRow)()
        
        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()


                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT(*) FROM CustomerContacts WHERE CustomerId=@CustomerId"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@CustomerId", params.customerid)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT *")
                sqlBuilder.AppendLine("FROM CustomerContacts")
                sqlBuilder.AppendLine("WHERE CustomerId=@CustomerId")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@CustomerId", params.customerid)



                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    whereClause.AppendLine(" AND ( FullName LIKE @SearchValue OR Email LIKE @SearchValue )")
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
                Dim keyOrderBy As String = " ORDER BY Id ASC"
                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    ' Perbaiki bagian ini:
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Id"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()
                   

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(keyOrderBy)
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(keyOrderBy)
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
                            .Name = reader("FullName").ToString(),
                            .Salutation = reader("Salutation").ToString(),
                            .Role = reader("Role").ToString(),
                            .Email = reader("Email").ToString(),
                            .Phone = reader("Phone").ToString(),
                            .Mobile = reader("Mobile").ToString(),
                            .Tags = reader("Tags").ToString(),
                            .Note = reader("Note").ToString(),
                            .Primary = reader("Primary").ToString()
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
    
    <WebMethod()>
    Public Shared Function AdressesServerSide(params As ServerSideParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of ServerSideReturnRow)()
        
        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()


                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT(*) FROM CustomerAddress WHERE CustomerId=@CustomerId"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@CustomerId", params.customerid)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT *")
                sqlBuilder.AppendLine("FROM CustomerAddress")
                sqlBuilder.AppendLine("WHERE CustomerId=@CustomerId")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@CustomerId", params.customerid)



                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    ' whereClause.AppendLine(" AND ( Description LIKE @SearchValue OR Address LIKE @SearchValue )")
                    whereClause.AppendLine(" AND 1=1")
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
                Dim keyOrderBy As String = " ORDER BY Id ASC"
                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    ' Perbaiki bagian ini:
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Id"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()
                   

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(keyOrderBy)
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(keyOrderBy)
                End If
                sqlBuilder.Append(orderByClause.ToString())
                
                ' ... kode selanjutnya ...

                ' --- Tambahkan Pagination (OFFSET/FETCH NEXT untuk SQL Server 2012+) ---
                sqlBuilder.AppendLine(" OFFSET " & params.start.ToString() & " ROWS FETCH NEXT " & params.length.ToString() & " ROWS ONLY")

                cmd.CommandText = sqlBuilder.ToString()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim noCounter As Integer = params.start + 1 ' Mulai hitung dari offset

                    While reader.Read()
                        Dim unitNumber As String = reader("UnitNumber").ToString()
                        Dim street As String = reader("Street").ToString()
                        Dim suburb As String = reader("Suburb").ToString()
                        Dim states As String = reader("States").ToString()
                        Dim postCode As String = reader("PostCode").ToString()

                        Dim adressFull As String = street & ", " & suburb & ", " & states & " " & postCode
                        If Not unitNumber = "" Then
                            adressFull = unitNumber & ", " & street & ", " & suburb & ", " & states & " " & postCode
                        End If

                        Dim row As New ServerSideReturnRow With {
                            .No = noCounter.ToString(),
                            .Id = reader("Id").ToString(),
                            .Description = reader("Description").ToString(),
                            .Address = adressFull,
                            .Port = reader("Port").ToString(),
                            .Tags = reader("Tags").ToString(),
                            .Instruction = reader("Instruction").ToString(),
                            .Primary = reader("Primary").ToString()
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

    <WebMethod()>
    Public Shared Function LoginsServerSide(params As ServerSideParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of ServerSideReturnRow)()
        
        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()

                

                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT(*) FROM CustomerLogins LEFT JOIN Applications ON CustomerLogins.ApplicationId = Applications.Id LEFT JOIN CustomerLoginRoles ON CustomerLogins.RoleId = CustomerLoginRoles.Id LEFT JOIN CustomerLoginLevels ON CustomerLogins.LevelId = CustomerLoginLevels.Id WHERE CustomerLogins.CustomerId=@CustomerId"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@CustomerId", params.customerid)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT CustomerLogins.*, Applications.Name AS AppName, CustomerLoginRoles.Name AS RoleName, CustomerLoginLevels.Name AS LevelName ")
                sqlBuilder.AppendLine("FROM CustomerLogins LEFT JOIN Applications ON CustomerLogins.ApplicationId = Applications.Id LEFT JOIN CustomerLoginRoles ON CustomerLogins.RoleId = CustomerLoginRoles.Id LEFT JOIN CustomerLoginLevels ON CustomerLogins.LevelId = CustomerLoginLevels.Id")
                sqlBuilder.AppendLine("WHERE CustomerId=@CustomerId")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@CustomerId", params.customerid)



                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    whereClause.AppendLine(" AND ( CustomerLogins.FullName LIKE @SearchValue OR CustomerLogins.UserName LIKE @SearchValue )")
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
                Dim keyOrderBy As String = " ORDER BY CASE WHEN Applications.Name = 'Application Administrator' THEN 0 WHEN Applications.Name = 'Application Lifestyle' THEN 1 ELSE 2 END, CASE WHEN CustomerLoginRoles.Name = 'Administrator' THEN 0 WHEN CustomerLoginRoles.Name = 'Customer Service' THEN 1 WHEN CustomerLoginRoles.Name = 'Representative' THEN 2 WHEN CustomerLoginRoles.Name = 'Data Entry' THEN 3 WHEN CustomerLoginRoles.Name = 'Account' THEN 4 WHEN CustomerLoginRoles.Name = 'Customer' THEN 5 ELSE 6 END, CASE WHEN CustomerLoginLevels.Name = 'Leader' THEN 0 WHEN CustomerLoginLevels.Name = 'Member' THEN 1 WHEN CustomerLoginLevels.Name = 'Support' THEN 2 ELSE 3 END, CustomerLogins.UserName ASC"
                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    ' Perbaiki bagian ini:
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Id"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()
                   

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(keyOrderBy)
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(keyOrderBy)
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
                            .Application = reader("AppName").ToString(),
                            .Role = reader("RoleName").ToString(),
                            .Username = reader("UserName").ToString(),
                            .Name = reader("FullName").ToString(),
                            .LastLogin = reader("LastLogin").ToString(),
                            .Active = reader("Active").ToString()
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

    <WebMethod()>
    Public Shared Function DiscountsServerSide(params As ServerSideParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of ServerSideReturnRow)()
        
        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()
                
                Dim customerGroup As String = publicCfg.GetItemData("SELECT [Group] FROM Customers WHERE Id='" + params.customerid + "'")

                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT(*) FROM CustomerDiscounts WHERE CustomerData=@CustomerId OR CustomerData=@CustomerGroup"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@CustomerId", params.customerid)
                    countCmd.Parameters.AddWithValue("@CustomerGroup", customerGroup)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT *")
                sqlBuilder.AppendLine("FROM CustomerDiscounts")
                sqlBuilder.AppendLine("WHERE CustomerData=@CustomerId OR CustomerData=@CustomerGroup")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@CustomerId", params.customerid)
                cmd.Parameters.AddWithValue("@CustomerGroup", customerGroup)




                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    ' whereClause.AppendLine(" AND ( Id LIKE @SearchValue )")
                    whereClause.AppendLine(" AND 1=1")
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
                Dim keyOrderBy As String = " ORDER BY CASE WHEN DiscountType = 'Product' THEN 1 WHEN DiscountType = 'Fabric' THEN 2 ELSE 3 END, CASE WHEN DiscountType = 'Fabric' THEN CAST(FabricId AS INT) ELSE NULL END ASC, CASE WHEN DiscountType != 'Fabric' THEN Id ELSE NULL END ASC"
                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    ' Perbaiki bagian ini:
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Id"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()
                   

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(keyOrderBy)
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(keyOrderBy)
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
                            .Title = Title(reader),
                            .Discount = Discount(reader),
                            .StartDate = reader("StartDate").ToString(),
                            .EndDate = reader("EndDate").ToString(),
                            .FinalDiscount = ""
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

    Private Shared Function Title(reader As SqlDataReader ) As String
        Dim result As String = ""
        Dim type As String = reader("DiscountType").ToString()
        If type = "Product" Then
            Dim designId As String = reader("DesignId").ToString()
            Dim designName As String = publicCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
            result = "Product : " & designName
        End If

        If type = "Fabric" Then
            Dim fabricId As String = reader("FabricId").ToString()
            Dim fabricColourId As String = reader("FabricColourId").ToString()
            Dim fabricCustom As String = Convert.ToInt32(reader("FabricCustom"))
            Dim fabricName As String = publicCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")

            result = "Fabric : " & fabricName

            If fabricCustom = 1 Then
                If Not String.IsNullOrEmpty(fabricColourId) Then
                    Dim ids As String() = fabricColourId.Split(","c).Where(Function(x) Not String.IsNullOrWhiteSpace(x)).ToArray()

                    Dim names As New List(Of String)

                    For Each thisId As String In ids
                        Dim name As String = publicCfg.GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" & thisId.Trim() & "'")
                        names.Add(name)
                    Next
                    Dim colourName As String = String.Join(",", names)
                    result = "Fabric : " & fabricName & " (" & colourName & ")"
                End If
            End If
        End If

        Return result
    End Function

    Private Shared Function Discount(reader As SqlDataReader) As String
        Dim result As String = String.Empty
        Dim Data As Decimal = Convert.ToDecimal(reader("Discount"))
        Try
            If Data > 0 Then
                Dim enUS As CultureInfo = CultureInfo.GetCultureInfo("en-US")
                result = Data.ToString("G29", enUS) & "%"
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    <WebMethod()>
    Public Shared Function ProductAccessServerSide(params As ServerSideParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of ServerSideReturnRow)()
        
        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()
                

                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT(*) FROM CustomerProductAccess WHERE Id=@CustomerId"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@CustomerId", params.customerid)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT *")
                sqlBuilder.AppendLine("FROM CustomerProductAccess")
                sqlBuilder.AppendLine("WHERE Id=@CustomerId")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@CustomerId", params.customerid)




                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    ' whereClause.AppendLine(" AND ( Id LIKE @SearchValue )")
                    whereClause.AppendLine(" AND 1=1")
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
                Dim keyOrderBy As String = " ORDER BY Id ASC"
                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    ' Perbaiki bagian ini:
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Id"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()
                   

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(keyOrderBy)
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(keyOrderBy)
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
                            .Product = Product(reader)
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

    Private Shared Function Product(reader As SqlDataReader) As String
        Dim result As String = String.Empty
        Try
            Dim hasil As String = String.Empty
            Dim CustomerId As String = reader("Id").ToString()

            Dim myData As DataSet = publicCfg.GetListData("SELECT Designs.Name AS DesignName FROM CustomerProductAccess CROSS APPLY STRING_SPLIT(CustomerProductAccess.DesignId, ',') AS designArray LEFT JOIN Designs ON designArray.VALUE = Designs.Id WHERE CustomerProductAccess.Id = '" + CustomerId + "' ORDER BY Designs.Name ASC ")
            If Not myData.Tables(0).Rows.Count = 0 Then
                For i As Integer = 0 To myData.Tables(0).Rows.Count - 1
                    Dim designName As String = myData.Tables(0).Rows(i).Item("DesignName").ToString()
                    hasil += designName & ", "
                Next
            End If

            result = hasil.Remove(hasil.Length - 2).ToString()
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    <WebMethod()>
    Public Shared Function QuotesServerSide(params As ServerSideParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of ServerSideReturnRow)()
        
        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()
                

                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT(*) FROM CustomerQuotes WHERE Id=@CustomerId"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@CustomerId", params.customerid)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT *")
                sqlBuilder.AppendLine("FROM CustomerQuotes")
                sqlBuilder.AppendLine("WHERE Id=@CustomerId")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@CustomerId", params.customerid)




                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    ' whereClause.AppendLine(" AND ( Id LIKE @SearchValue )")
                    whereClause.AppendLine(" AND 1=1")
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
                Dim keyOrderBy As String = " ORDER BY Id ASC"
                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    ' Perbaiki bagian ini:
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Id"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()
                   

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(keyOrderBy)
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(keyOrderBy)
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
                            .Logo = reader("Logo").ToString(),
                            .Terms = reader("Terms").ToString()
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
