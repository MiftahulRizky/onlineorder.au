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
Partial Class Methods_Setting_Fabric_FabricMethod
    Inherits System.Web.UI.Page
    Shared publicCfg As New PublicConfig()

    Public Class FabricParams
        Public Property designid As String
        Public Property active As String
        
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
        Public Property data As List(Of FabricReturnRow)
    End Class

    Public Class FabricReturnRow
        Public Property No As String 
        Public Property Id As String 
        Public Property DesignName As String
        Public Property FabricName As String
        Public Property Width As String 
        Public Property Group As String 
        Public Property Active As String
    End Class

     Public Class FormData
        Public Property id As String
        Public Property name As String
        Public Property type As String
        Public Property colour As String
        Public Property width As String
        Public Property group As String
        Public Property designtype As String
        Public Property des As String
        Public Property activate As String
        Public Property action As String
    End Class

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
    Public Shared Function Find(ByVal id As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + UCase(id).ToString() + "'")

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
    Public Shared Function Bind(params As FabricParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of FabricReturnRow)()

        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()

                

                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT(Fabrics.Id) FROM Fabrics LEFT JOIN Designs ON Designs.Id = Fabrics.DesignId WHERE Fabrics.DesignId = @DesignId AND Fabrics.Active = @Active"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@DesignId", params.designid)
                    countCmd.Parameters.AddWithValue("@Active", params.active)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT Fabrics.*, Designs.Name AS DesignName")
                sqlBuilder.AppendLine("FROM Fabrics LEFT JOIN Designs ON Designs.Id = Fabrics.DesignId")
                sqlBuilder.AppendLine("WHERE Fabrics.DesignId = @DesignId AND Fabrics.Active = @Active")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@DesignId", params.designid)
                cmd.Parameters.AddWithValue("@Active", params.active)



                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    whereClause.AppendLine(" AND (Fabrics.Id LIKE @SearchValue OR Fabrics.Name LIKE @SearchValue OR Fabrics.Width LIKE @SearchValue OR Fabrics.Group LIKE @SearchValue)")
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
                        {1, "DesignName"}, _
                        {2, "FabricName"}, _
                        {3, "Width"}, _
                        {4, "Group"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(" ORDER BY Fabrics.Name ASC")
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(" ORDER BY Fabrics.Name ASC")
                End If
                sqlBuilder.Append(orderByClause.ToString())
                
                ' ... kode selanjutnya ...

                ' --- Tambahkan Pagination (OFFSET/FETCH NEXT untuk SQL Server 2012+) ---
                sqlBuilder.AppendLine(" OFFSET " & params.start.ToString() & " ROWS FETCH NEXT " & params.length.ToString() & " ROWS ONLY")

                cmd.CommandText = sqlBuilder.ToString()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim noCounter As Integer = params.start + 1 ' Mulai hitung dari offset

                    While reader.Read()
                        Dim row As New FabricReturnRow With {
                            .No = noCounter.ToString(),
                            .Id = reader("Id").ToString(),
                            .DesignName = reader("DesignName").ToString(),
                            .FabricName = reader("Name").ToString(),
                            .Width = reader("Width").ToString(),
                            .Group = reader("Group").ToString(),
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
            response.data = New List(Of FabricReturnRow)()
            ' Untuk debugging, bisa kirim error ke client, tapi jangan di production
            ' response.error = ex.Message
            Return response
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Submit(ByVal data As FormData) As Object
        Try
            If String.IsNullOrEmpty(data.id) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fabric id is required !", .field = "id"}}
            End If

            Dim fabricData As DataSet = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" & data.id & "'")
            If fabricData.Tables(0).Rows.Count > 0 Then
                Dim fabricName As String = fabricData.Tables(0).Rows(0).Item("Name").ToString()
                If data.action = "create" Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fabric id " & data.id & " already exist !", .field = "id"}}
                ' Else if data.action = "update" AND fabricName <> data.name Then
                '     Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fabric id " & data.id & " already exist !", .field = "id"}}
                End If
            End If
            


            If String.IsNullOrEmpty(data.name) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fabric name is required !", .field = "name"}}
            End If
            If String.IsNullOrEmpty(data.type) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fabric type is required !", .field = "type"}}
            End If
            If String.IsNullOrEmpty(data.colour) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fabric colour is required !", .field = "colour"}}
            End If
            If String.IsNullOrEmpty(data.width) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fabric width is required !", .field = "width"}}
            End If
            If String.IsNullOrEmpty(data.group) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fabric group is required !", .field = "group"}}
            End If
            If String.IsNullOrEmpty(data.designtype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "design type is required !", .field = "designtype"}}
            End If

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString    

            Dim msg As String = "200"
            ' ' '#insert
            If data.action = "create" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO Fabrics VALUES (@Id, @DesignId, @Name, @Type, @Colour, @Width, @Group, @Description, @Active)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", data.id)
                        myCmd.Parameters.AddWithValue("@DesignId", UCase(data.designtype).ToString())
                        myCmd.Parameters.AddWithValue("@Name", data.name)
                        myCmd.Parameters.AddWithValue("@Type", data.type)
                        myCmd.Parameters.AddWithValue("@Colour", data.colour)
                        myCmd.Parameters.AddWithValue("@Width", data.width)
                        myCmd.Parameters.AddWithValue("@Group", data.group)
                        myCmd.Parameters.AddWithValue("@Description", data.des)
                        myCmd.Parameters.AddWithValue("@Active", data.activate)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
                msg = "Data has been saved successfully."
            End If

            If data.action = "update" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE Fabrics SET DesignId=@DesignId, Name=@Name, Type=@Type, Colour=@Colour, Width=@Width, [Group]=@Group, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", data.id)
                        myCmd.Parameters.AddWithValue("@DesignId", UCase(data.designtype).ToString())
                        myCmd.Parameters.AddWithValue("@Name", data.name)
                        myCmd.Parameters.AddWithValue("@Type", data.type)
                        myCmd.Parameters.AddWithValue("@Colour", data.colour)
                        myCmd.Parameters.AddWithValue("@Width", data.width)
                        myCmd.Parameters.AddWithValue("@Group", data.group)
                        myCmd.Parameters.AddWithValue("@Description", data.des)
                        myCmd.Parameters.AddWithValue("@Active", data.activate)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
                msg = "Data has been updated successfully."
            End If

            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Switch(ByVal id As String, ByVal active As Boolean) As Object
        Try
            If String.IsNullOrEmpty(id) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "key is invalid !", .field = ""}}
            End If

            Dim thisData = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + UCase(id).ToString() + "'")
            If thisData.Tables.Count = 0 Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "key is missing !", .field = ""}}
            End If

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString    
            Dim activeVal As String = 1
            If active = True Then
                activeVal = 0
            End If
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE Fabrics SET Active=@Active WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", UCase(id).ToString())
                    myCmd.Parameters.AddWithValue("@Active", activeVal)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
            

            Return New SuccessResponse With {.success = "Switch Successfully."}
        Catch ex As Exception
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Delete(ByVal id As String) As Object
        Try
            If String.IsNullOrEmpty(id) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "key is invalid !", .field = ""}}
            End If

            Dim thisData = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + UCase(id).ToString() + "'")
            If thisData.Tables.Count = 0 Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "key is missing !", .field = ""}}
            End If

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString    

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("DELETE FROM Fabrics WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", UCase(id).ToString())
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
            

            Return New SuccessResponse With {.success = "Delete Successfully."}
        Catch ex As Exception
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function




    '#------------------------------------|| Other Methods ||------------------------------------#
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindDesignType() As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Designs ORDER BY Name ASC")
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

End Class
