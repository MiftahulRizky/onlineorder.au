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
Partial Class Methods_Order_DefaultMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()

    '#--- Initialize Class ---#
    Public Class OrdersParams
        Public Property storeid As String
        Public Property storecompany As String
        Public Property userid As String
        Public Property rolename As String
        Public Property levelname As String
        Public Property status As String
        Public Property active As String
        Public Property storetype As String
    
       
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
        Public Property data As List(Of OrdersMatrixReturnRow)
    End Class

    Public Class OrdersMatrixReturnRow
        Public Property No As String 
        Public Property Id As String 
        Public Property UserId As String 
        Public Property JoNumber As String 
        Public Property StoreName As String 
        Public Property OrderNo As String 
        Public Property OrderCust As String 
        Public Property Delivery As String 
        Public Property Status As String 
        Public Property CreatedDate As String 
        Public Property SubmittedDate As String 
        Public Property CanceledDate As String 
        Public Property CompletedDate As String
        Public Property Active As String
    End Class

    Public Class ParamUpdateStatusOrder
        Public Property id  As String
        Public Property status As String
        Public Property statusOld As String
        Public Property submitteddate As String
        Public Property completeddate As String
        Public Property canceleddate As String
        Public Property description As String

        Public Property username As String
    End Class


    '#--- Kelas Output WebMethod ---#
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


    <WebMethod(EnableSession:=True)>
    Public Shared Sub SetHeaderAction(ByVal action As String)
        HttpContext.Current.Session("headerAction") = action
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Sub SetSessionOpenOrderDetail(ByVal headerid As String)
        HttpContext.Current.Session("headerId") = headerid 
    End Sub



    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrders(params As OrdersParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of OrdersMatrixReturnRow)()

        
        Try
            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()

                
                Dim byRole As String = ""
                
                If params.rolename = "PPIC & DE" Then
                    byRole = " AND StoreCompany = '" + params.storecompany + "'"
                End If

                If params.rolename = "Customer" Then
                    byRole = " AND StoreId = '" + params.storeid + "'"
                    If params.levelname = "Member" Then
                        byRole = " AND UserId = '" + params.userid + "'"
                    End If
                End If
                

                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT( Id ) FROM view_headers WHERE Active = @Active" + byRole + " AND (@Status = 'all' OR Status = @Status) AND (@StoreType = 'ALL' OR StoreType = @StoreType)"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@Status", params.status)
                    countCmd.Parameters.AddWithValue("@Active", params.active)
                    countCmd.Parameters.AddWithValue("@StoreType", params.storetype)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT Id, UserId, JoNumber, StoreName, OrderNo, OrderCust, Delivery,Status, CreatedDate, SubmittedDate, CanceledDate, CompletedDate, Active")
                sqlBuilder.AppendLine("FROM view_headers")
                sqlBuilder.AppendLine("WHERE Active = @Active" + byRole + " AND (@Status = 'all' OR Status = @Status) AND (@StoreType = 'ALL' OR StoreType = @StoreType)")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@Status", params.status)
                cmd.Parameters.AddWithValue("@Active", params.active)
                cmd.Parameters.AddWithValue("@StoreType", params.storetype)



                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    whereClause.AppendLine(" AND ( JoNumber LIKE @SearchValue OR StoreName LIKE @SearchValue OR OrderNo LIKE @SearchValue OR OrderCust LIKE @SearchValue OR Delivery LIKE @SearchValue OR Status LIKE @SearchValue )")
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
                    '# Notes: File di bawah ini untuk menambahkan order by ke query
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "JoNumber"}, _
                        {2, "StoreName"}, _
                        {3, "OrderNo"}, _
                        {4, "OrderCust"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(" ORDER BY CreatedDate DESC")
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(" ORDER BY CreatedDate DESC")
                End If
                sqlBuilder.Append(orderByClause.ToString())
                
                ' ... kode selanjutnya ...

                ' --- Tambahkan Pagination (OFFSET/FETCH NEXT untuk SQL Server 2012+) ---
                sqlBuilder.AppendLine(" OFFSET " & params.start.ToString() & " ROWS FETCH NEXT " & params.length.ToString() & " ROWS ONLY")

                cmd.CommandText = sqlBuilder.ToString()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim noCounter As Integer = params.start + 1 ' Mulai hitung dari offset

                    While reader.Read()
                        Dim row As New OrdersMatrixReturnRow With {
                            .No = noCounter.ToString(),
                            .Id = reader("Id").ToString(),
                            .UserId = reader("UserId").ToString(),
                            .JoNumber = reader("JoNumber").ToString(),
                            .StoreName = reader("StoreName").ToString(),
                            .OrderNo = reader("OrderNo").ToString(),
                            .OrderCust = reader("OrderCust").ToString(),
                            .Delivery = reader("Delivery").ToString(),
                            .Status = reader("Status").ToString(),
                            .CreatedDate = reader("CreatedDate").ToString(),
                            .SubmittedDate = reader("SubmittedDate").ToString(),
                            .CanceledDate = reader("CanceledDate").ToString(),
                            .CompletedDate = reader("CompletedDate").ToString(),
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
            response.data = New List(Of OrdersMatrixReturnRow)()
            ' Untuk debugging, bisa kirim error ke client, tapi jangan di production
            ' response.error = ex.Message
            Return response
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderId(ByVal headerid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id = '" + headerid + "'")

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
    Public Shared Function UpdateStatusOrder(data As ParamUpdateStatusOrder) As Object
        Try
        Dim msg As String

        '#-------------------------|| SET VALIDATE RULES ||-----------------------#
            '#-------------------------|| id ||-----------------------#
            If String.IsNullOrEmpty(data.id) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "this order is missing !",
                        .field = "#modalChangeStatus #id"
                    }
                }
            End If

            '#-------------------------|| status ||-----------------------#
            If String.IsNullOrEmpty(data.status) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "status is required !",
                        .field = "#modalChangeStatus #status"
                    }
                }
            End If
            If data.status = data.statusOld Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "you don't choose different changes on status, don't do it with the same status!",
                        .field = "#modalChangeStatus #status"
                    }
                }
            End If


            If data.status = "New Order" Then
                If data.submittedDate = "" Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "submitted date is required !",
                            .field = "#modalChangeStatus #submitteddate"
                        }
                    }
                End If
            End If

            If data.status = "Completed" Then
                If data.completeddate = "" Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "shipped date is required !",
                            .field = "#modalChangeStatus #completeddate"
                        }
                    }
                End If
            End If

            If data.status = "Canceled" Then
                If data.canceleddate = "" Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "canceled date is required !",
                            .field = "#modalChangeStatus #canceleddate"
                        }
                    }
                End If
            End If
            
            If data.description = "" AndAlso (data.status <> "Draft" AndAlso data.status <> "On Hold" AndAlso data.status <> "In Production") Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "description is required !",
                        .field = "#modalChangeStatus #description"
                    }
                }
            End If
        '#------------------------------------------------|| Prepare Submit ||-------------------------------------------------#
            '#-----------------------------------|| Set default values before submission ||----------------------------------------#
            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

            Dim findDesc As String = data.description
            Select Case data.status
                Case "New Order"
                    findDesc = "Status changed to new order by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "In Production"
                    findDesc = "Your order is currently in the production process"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "On Hold"
                    findDesc = "Your order on hold by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "Completed"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "Canceled"
                    findDesc = "Your order has been canceled by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
            End Select

            If Not String.IsNullOrEmpty(data.id) Then
                Dim query As String = "UPDATE OrderHeaders SET Status='Draft', StatusDescription=NULL, SubmittedDate=NULL, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                Select Case data.status
                    Case "New Order"
                        query = "UPDATE OrderHeaders SET Status='New Order', StatusDescription=@StatusDescription, SubmittedDate=@SubmittedDate, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                    Case "In Production"
                        query = "UPDATE OrderHeaders SET Status='In Production', StatusDescription=@StatusDescription, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                    Case "On Hold"
                        query = "UPDATE OrderHeaders SET Status='On Hold', StatusDescription=@StatusDescription, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                    Case "Completed"
                        query = "UPDATE OrderHeaders SET Status='Completed', StatusDescription=@StatusDescription, CanceledDate=NULL, CompletedDate=@CompletedDate WHERE Id=@Id"
                    Case "Canceled"
                        query = "UPDATE OrderHeaders SET Status='Canceled', StatusDescription=@StatusDescription, CanceledDate=@CanceledDate WHERE Id=@Id"
                End Select

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(query, thisConn)
                        myCmd.Parameters.AddWithValue("@Id", data.id)
                        myCmd.Parameters.AddWithValue("@Status", data.status)
                        myCmd.Parameters.AddWithValue("@StatusDescription", findDesc)
                        myCmd.Parameters.AddWithValue("@SubmittedDate", data.submitteddate)
                        myCmd.Parameters.AddWithValue("@CompletedDate", data.completeddate)
                        myCmd.Parameters.AddWithValue("@CanceledDate", data.canceleddate)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                msg = "Status has been updated successfully."
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
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SwitchOrder(ByVal id As String, ByVal action As String) As Object
        Try
         Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        '#DELETE & RESTORE
        Dim msg As String = "Data has been restored successfully."
        Dim Key As String = "Active=1"
        If Not String.IsNullOrEmpty(id) Then
            If action = "delete" Then
                Key = "Active=0"
                msg = "Data has been deleted successfully."
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderHeaders SET " & Key & " WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", id)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

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
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function DownloadCSVOrder(ByVal HeaderId As String) As String
        Dim publicCfg As New PublicConfig()
        Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id = '" + HeaderId + "'")
        Dim Blank As String = ""
        Dim ordercust As String = headerData.Tables(0).Rows(0).Item("OrderCust").ToString
        Dim csv As New StringBuilder()
        csv.AppendLine("HEADER,STORE ID,STORE ORDER NO,STORE CUSTOMER,BARCODE,INVOICE,WORK ORDER")
        csv.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",
            Blank,
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

        Return csv.ToString()
    End Function
End Class
