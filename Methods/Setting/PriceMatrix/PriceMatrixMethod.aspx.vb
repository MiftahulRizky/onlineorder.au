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

Partial Class Methods_SettingPage_PriceMatrix_PriceMatrixMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()

    '#----------------------------|| Intialization ||----------------------------#
    '#--------------|| define class row ||--------------#
    ' --- Kelas Input WebMethod (MatrixParams) ---
    ' Jika Anda mengirim ID sebagai parameter, pastikan tipenya benar
    Public Class MatrixParams
        Public Property pricegroupid As String
        Public Property type As String
        Public Property width As String
        Public Property drop As string
        ' Jika ada properti Id di sini dan itu adalah GUID dari klien
        ' Public Property Id As String ' Atau Public Property Id As Guid

        ' Parameter DataTables untuk server-side processing
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

    ' --- Kelas Output WebMethod (untuk Respons DataTables) ---
    Public Class DataTableResponse
        Public Property draw As Integer
        Public Property recordsTotal As Integer
        Public Property recordsFiltered As Integer
        Public Property data As List(Of MatrixReturnRow)
    End Class

    Public Class MatrixReturnRow
        Public Property No As String 
        Public Property Id As String 
        Public Property PriceGroupName As String
        Public Property Type As String
        Public Property Width As String 
        Public Property Drop As String 
        Public Property Cost As String
    End Class

    Public Class CSVParams
        Public Property filePath  As String
        Public Property pricegroupid As String
        Public Property type As String
    End Class


    Public Class ParamSaveData
        Public Property id As String
        Public Property designid As String
        Public Property pricegroupid As String
        Public Property type As String
        Public Property width As String
        Public Property drop As String
        Public Property cost As String
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



    '#----------------------------------------|| Binding Function ||----------------------------------------#
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindDesignType() As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Designs WHERE Name NOT LIKE '%Headbox%' ORDER BY Name ASC")
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
    Public Shared Function BindDesignByPriceGroupId(ByVal pricegroupid As String) As Object
        Try
            If Not String.IsNullOrEmpty(pricegroupid) Then
                Dim designid As String = publicCfg.GetItemData("SELECT DesignId FROM PricesGroup WHERE Id='" & pricegroupid & "'")

                ' Kembalikan dalam bentuk object agar tetap bisa dibaca dari JavaScript
                Return New With {.value = designid}
            End If

            Return New With {.value = Nothing}
        Catch ex As Exception
            Return New With {.error = ex.Message}
        End Try
    End Function



    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindPriceGroup(ByVal designid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM PricesGroup WHERE DesignId='" + designid + "' AND Name NOT LIKE '%Headbox%' ORDER BY Name ASC")
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
    Public Shared Function BindPriceMatrix(params As MatrixParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of MatrixReturnRow)()

        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()

                

                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT(Prices.Id) FROM Prices INNER JOIN PricesGroup ON PricesGroup.Id = Prices.PriceGroupId WHERE Prices.PriceGroupId = @PriceGroupId AND Prices.Type = @Type AND Prices.Width >= @Width AND Prices.[Drop] >= @Drop"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@PriceGroupId", params.pricegroupid)
                    countCmd.Parameters.AddWithValue("@Type", params.type)
                    countCmd.Parameters.AddWithValue("@Width", params.width)
                    countCmd.Parameters.AddWithValue("@Drop", params.drop)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT Prices.Id, Prices.Type, Prices.Width, Prices.[Drop], Prices.Cost, PricesGroup.Name AS PriceGroupName")
                sqlBuilder.AppendLine("FROM Prices INNER JOIN PricesGroup ON PricesGroup.Id = Prices.PriceGroupId")
                sqlBuilder.AppendLine("WHERE Prices.PriceGroupId = @PriceGroupId AND Prices.Type = @Type AND Prices.Width >= @Width AND Prices.[Drop] >= @Drop")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@PriceGroupId", params.pricegroupid)
                cmd.Parameters.AddWithValue("@Type", params.type)
                cmd.Parameters.AddWithValue("@Width", params.width)
                cmd.Parameters.AddWithValue("@Drop", params.drop)



                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    whereClause.AppendLine(" AND (PricesGroup.Name LIKE @SearchValue OR Prices.Type LIKE @SearchValue OR CONVERT(NVARCHAR(50), Prices.Width) LIKE @SearchValue OR CONVERT(NVARCHAR(50), Prices.[Drop]) LIKE @SearchValue OR CONVERT(NVARCHAR(50), Prices.Cost) LIKE @SearchValue)")
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
                        {1, "PriceGroupName"}, _
                        {2, "Type"}, _
                        {3, "Width"}, _
                        {4, "Drop"}, _
                        {5, "Cost"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(" ORDER BY PriceGroupName, Prices.Type, Prices.Width, Prices.[Drop], Prices.Cost ASC")
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(" ORDER BY PriceGroupName, Prices.Type, Prices.Width, Prices.[Drop], Prices.Cost ASC")
                End If
                sqlBuilder.Append(orderByClause.ToString())
                
                ' ... kode selanjutnya ...

                ' --- Tambahkan Pagination (OFFSET/FETCH NEXT untuk SQL Server 2012+) ---
                sqlBuilder.AppendLine(" OFFSET " & params.start.ToString() & " ROWS FETCH NEXT " & params.length.ToString() & " ROWS ONLY")

                cmd.CommandText = sqlBuilder.ToString()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim noCounter As Integer = params.start + 1 ' Mulai hitung dari offset

                    While reader.Read()
                        Dim row As New MatrixReturnRow With {
                            .No = noCounter.ToString(),
                            .Id = reader("Id").ToString(),
                            .PriceGroupName = reader("PriceGroupName").ToString(),
                            .Type = reader("Type").ToString(),
                            .Width = reader("Width").ToString(),
                            .Drop = reader("Drop").ToString(),
                            .Cost = reader("Cost").ToString()
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
            response.data = New List(Of MatrixReturnRow)()
            ' Untuk debugging, bisa kirim error ke client, tapi jangan di production
            ' response.error = ex.Message
            Return response
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindPriceMatrixId(ByVal id As String) As Object
        Try
            ' Gunakan parameterized query (idealnya pakai SqlParameter, ini simulasi fungsi GetListData Anda)
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Prices WHERE Id = '" + id + "'")

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
    Public Shared Function SubmitCSV(data As CSVParams) As Object
        Try
            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
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
                            Dim rawCost = row(i).Replace(".", ",")

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


    <WebMethod()>
    Public Shared Function SaveData(data As ParamSaveData) As Object
        Try
            '#-------------------------|| SET VALIDATE RULES ||-----------------------#
            '#-------------------------|| designid ||-----------------------#
             If String.IsNullOrEmpty(data.designid) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "design type is required !",
                        .field = "#modalSaveData #designid"
                    }
                }
            End If
            '#-------------------------|| pricegroupid ||-----------------------#
             If String.IsNullOrEmpty(data.pricegroupid) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "group name is required !",
                        .field = "#modalSaveData #pricegroupid"
                    }
                }
            End If
            '#-------------------------|| type ||-----------------------#
             If String.IsNullOrEmpty(data.type) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "type is required !",
                        .field = "#modalSaveData #type"
                    }
                }
            End If
            '#-------------------------|| width ||-----------------------#
             If String.IsNullOrEmpty(data.width) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "width is required !",
                        .field = "#modalSaveData #width"
                    }
                }
            End If
            '#-------------------------|| drop ||-----------------------#
             If String.IsNullOrEmpty(data.drop) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "drop is required !",
                        .field = "#modalSaveData #drop"
                    }
                }
            End If
            '#-------------------------|| cost ||-----------------------#
             If String.IsNullOrEmpty(data.cost) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "cost is required !",
                        .field = "#modalSaveData #cost"
                    }
                }
            End If
            '#------------------------------------------------|| Prepare Submit ||-------------------------------------------------#
            '#-----------------------------------|| Set default values before submission ||----------------------------------------#
            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

            Dim msg As String

            '#INSERT
            If data.id = "" Then

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO Prices VALUES (NEWID(), @PriceGroupId, @Type, @Drop, @Width, @Cost)", thisConn)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(data.pricegroupid).ToString())
                        myCmd.Parameters.AddWithValue("@Type", data.type)
                        myCmd.Parameters.AddWithValue("@Width", data.width)
                        myCmd.Parameters.AddWithValue("@Drop", data.drop)
                        myCmd.Parameters.AddWithValue("@Cost", data.cost)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                msg = "Data has been saved successfully."
            End If 

            '#UPDATE
            If Not data.id = "" Then

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE Prices SET PriceGroupId=@PriceGroupId, Type=@Type, [Drop]=@Drop, Width=@Width, Cost=@Cost WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", data.id)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(data.pricegroupid).ToString())
                        myCmd.Parameters.AddWithValue("@Type", data.type)
                        myCmd.Parameters.AddWithValue("@Width", data.width)
                        myCmd.Parameters.AddWithValue("@Drop", data.drop)
                        myCmd.Parameters.AddWithValue("@Cost", data.cost)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                msg = "Data has been updated successfully."
            End If 


            Return New SuccessResponse With { .success = msg }
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
    Public Shared Function DeletePriceMatrix(ByVal id As String) As Object
        Try
         Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        '#DELETE
        If Not String.IsNullOrEmpty(id) Then

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("DELETE FROM Prices WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", id)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

        End If 


        Return New SuccessResponse With { .success = "Data has been deleted successfully." }
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
    Public Shared Function DeletePriceMatrixByGroup(ByVal groupid As String, ByVal type As String) As Object
        Try
         Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        '#DELETE
        If Not String.IsNullOrEmpty(groupid) Then

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("DELETE FROM Prices WHERE PriceGroupId=@PriceGroupId AND Type=@Type", thisConn)
                    myCmd.Parameters.AddWithValue("@PriceGroupId", groupid)
                    myCmd.Parameters.AddWithValue("@Type", type)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

        End If 


        Return New SuccessResponse With { .success = "Data has been deleted successfully." }
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
