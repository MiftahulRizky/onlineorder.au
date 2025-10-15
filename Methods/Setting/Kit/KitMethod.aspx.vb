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
Partial Class Methods_Setting_Kit_KitMethod
    Inherits System.Web.UI.Page
    Shared publicCfg As New PublicConfig()

    Public Class KitParams
        Public Property designid As String
        Public Property blindid As String
        
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
        Public Property data As List(Of KitReturnRow)
    End Class

    Public Class KitReturnRow
        Public Property No As String 
        Public Property Id As String 
        Public Property SoeId As String
        Public Property Name As String
        Public Property DesignName As String 
        Public Property BlindName As String 
        Public Property Active As String
    End Class

    Public Class FormData
        Public Property id As String
        Public Property soeid As String
        Public Property name As String
        Public Property designtype As String
        Public Property blindtype As String
        Public Property bracket As String
        Public Property tube As String
        Public Property control As String
        Public Property colour As String
        Public Property des As String
        Public Property active As String
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
    Public Shared Function KitServerSide(params As KitParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of KitReturnRow)()

        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()

                

                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT(HardwareKits.Id) FROM HardwareKits INNER JOIN Designs ON Designs.Id = HardwareKits.DesignId INNER JOIN Blinds ON Blinds.Id = HardwareKits.BlindId WHERE HardwareKits.DesignId = @DesignId AND HardwareKits.BlindId = @BlindId"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@DesignId", params.designid)
                    countCmd.Parameters.AddWithValue("@BlindId", params.blindid)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT HardwareKits.*, Designs.Name AS DesignName, Blinds.Name AS BlindName")
                sqlBuilder.AppendLine("FROM HardwareKits INNER JOIN Designs ON Designs.Id = HardwareKits.DesignId INNER JOIN Blinds ON Blinds.Id = HardwareKits.BlindId")
                sqlBuilder.AppendLine("WHERE HardwareKits.DesignId = @DesignId AND HardwareKits.BlindId = @BlindId")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@DesignId", params.designid)
                cmd.Parameters.AddWithValue("@BlindId", params.blindid)



                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    whereClause.AppendLine(" AND (HardwareKits.SoeId LIKE @SearchValue OR HardwareKits.Name LIKE @SearchValue ")
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
                        {1, "SoeId"}, _
                        {2, "Name"}, _
                        {3, "DesignName"}, _
                        {4, "BlindName"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(" ORDER BY HardwareKits.Name ASC")
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(" ORDER BY HardwareKits.Name ASC")
                End If
                sqlBuilder.Append(orderByClause.ToString())
                
                ' ... kode selanjutnya ...

                ' --- Tambahkan Pagination (OFFSET/FETCH NEXT untuk SQL Server 2012+) ---
                sqlBuilder.AppendLine(" OFFSET " & params.start.ToString() & " ROWS FETCH NEXT " & params.length.ToString() & " ROWS ONLY")

                cmd.CommandText = sqlBuilder.ToString()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim noCounter As Integer = params.start + 1 ' Mulai hitung dari offset

                    While reader.Read()
                        Dim row As New KitReturnRow With {
                            .No = noCounter.ToString(),
                            .Id = reader("Id").ToString(),
                            .SoeId = reader("SoeId").ToString(),
                            .Name = reader("Name").ToString(),
                            .DesignName = reader("DesignName").ToString(),
                            .BlindName = reader("BlindName").ToString(),
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
            response.data = New List(Of KitReturnRow)()
            ' Untuk debugging, bisa kirim error ke client, tapi jangan di production
            ' response.error = ex.Message
            Return response
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Find(ByVal kitid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM HardwareKits WHERE Id = '" + UCase(kitid).ToString() + "'")

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
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Submit(ByVal data As FormData) As Object
        Try
            If String.IsNullOrEmpty(data.soeid) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "soe id is required !", .field = "soeid"}}
            End If
            If String.IsNullOrEmpty(data.name) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "kit name is required !", .field = "name"}}
            End If
            If String.IsNullOrEmpty(data.designtype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "design type is required !", .field = "designtype"}}
            End If
            If String.IsNullOrEmpty(data.blindtype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "blind type is required !", .field = "blindtype"}}
            End If
            If String.IsNullOrEmpty(data.bracket) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "bracket type is required !", .field = "bracket"}}
            End If
            If String.IsNullOrEmpty(data.tube) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "tube type is required !", .field = "tube"}}
            End If
            If String.IsNullOrEmpty(data.control) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "control type is required !", .field = "control"}}
            End If
            If String.IsNullOrEmpty(data.colour) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "colour type is required !", .field = "colour"}}
            End If

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString    

            Dim msg As String = "200"
            ' '#insert
            If String.IsNullOrEmpty(data.id) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO HardwareKits VALUES (NEWID(), @SoeId, @DesignId, @BlindId, @Name, @BracketType, @TubeType, @ControlType, @ColourType, @Description, @Active)", thisConn)
                        myCmd.Parameters.AddWithValue("@SoeId", data.soeid)
                        myCmd.Parameters.AddWithValue("@DesignId", UCase(data.designtype).ToString())
                        myCmd.Parameters.AddWithValue("@BlindId", UCase(data.blindtype).ToString())
                        myCmd.Parameters.AddWithValue("@Name", data.name)
                        myCmd.Parameters.AddWithValue("@BracketType", data.bracket)
                        myCmd.Parameters.AddWithValue("@TubeType", data.tube)
                        myCmd.Parameters.AddWithValue("@ControlType", data.control)
                        myCmd.Parameters.AddWithValue("@ColourType", data.colour)
                        myCmd.Parameters.AddWithValue("@Description", data.des)
                        myCmd.Parameters.AddWithValue("@Active", data.active)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
                msg = "Data has been saved successfully."
            End If

            If Not String.IsNullOrEmpty(data.id) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE HardwareKits SET SoeId=@SoeId, DesignId=@DesignId, BlindId=@BlindId, Name=@Name, BracketType=@BracketType, TubeType=@TubeType, ControlType=@ControlType, ColourType=@ColourType, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", UCase(data.id).ToString())
                        myCmd.Parameters.AddWithValue("@SoeId", data.soeid)
                        myCmd.Parameters.AddWithValue("@DesignId", UCase(data.designtype).ToString())
                        myCmd.Parameters.AddWithValue("@BlindId", UCase(data.blindtype).ToString())
                        myCmd.Parameters.AddWithValue("@Name", data.name)
                        myCmd.Parameters.AddWithValue("@BracketType", data.bracket)
                        myCmd.Parameters.AddWithValue("@TubeType", data.tube)
                        myCmd.Parameters.AddWithValue("@ControlType", data.control)
                        myCmd.Parameters.AddWithValue("@ColourType", data.colour)
                        myCmd.Parameters.AddWithValue("@Description", data.des)
                        myCmd.Parameters.AddWithValue("@Active", data.active)
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
    Public Shared Function Delete(ByVal id As String) As Object
        Try
            If String.IsNullOrEmpty(id) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "key is invalid !", .field = ""}}
            End If

            Dim thisData = publicCfg.GetListData("SELECT * FROM HardwareKits WHERE Id = '" + UCase(id).ToString() + "'")
            If thisData.Tables.Count = 0 Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "key is missing !", .field = ""}}
            End If

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString    

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("DELETE FROM HardwareKits WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", UCase(id).ToString())
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
            

            Return New SuccessResponse With {.success = "Delete Surcharge Successfully."}
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

            Dim thisData = publicCfg.GetListData("SELECT * FROM HardwareKits WHERE Id = '" + UCase(id).ToString() + "'")
            If thisData.Tables.Count = 0 Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "key is missing !", .field = ""}}
            End If

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString    
            Dim activeVal As String = 1
            If active = True Then
                activeVal = 0
            End If
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE HardwareKits SET Active=@Active WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", UCase(id).ToString())
                    myCmd.Parameters.AddWithValue("@Active", activeVal)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
            

            Return New SuccessResponse With {.success = "Switch Surcharge Successfully."}
        Catch ex As Exception
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function






    '#----------------------------------------|| Other Methods ||----------------------------------------#
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

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindBlindType(ByVal designid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Blinds WHERE DesignId='" + UCase(designid).ToString() + "' ORDER BY Name ASC")
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
    
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindBracket() As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, Name FROM BracketType WHERE Active=1 ORDER BY Name ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Name").ToString()},
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
    Public Shared Function BindTube() As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, Name FROM TubeType WHERE Active=1 ORDER BY Name ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Name").ToString()},
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
    Public Shared Function BindControl() As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, Name FROM ControlType WHERE Active=1 ORDER BY Name ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Name").ToString()},
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
    Public Shared Function BindColour() As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, Name FROM ColourType WHERE Active=1 ORDER BY Name ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Name").ToString()},
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


End Class
