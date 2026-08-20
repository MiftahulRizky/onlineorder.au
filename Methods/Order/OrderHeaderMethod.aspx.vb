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
Partial Class Methods_Order_OrderHeaderMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared mailCfg As New MailConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamBindOrderHeaderAggregate
        Public Property status As String
        Public Property ordertype As String
        Public Property active As String
        Public Property storetype As String
        Public Property search As String

        Public Property loginid As String
        Public Property customerid As String
        Public Property customeraccount As String
        Public Property customercompany As String
        Public Property rolename As String
        Public Property levelname As String
        Public Property username As String
    End Class

    Public Class OrdersParams
        Public Property loginid As String
        Public Property customerid As String
        Public Property customeraccount As String
        Public Property customercompany As String
        Public Property rolename As String
        Public Property username As String
        Public Property levelname As String
        Public Property status As String
        Public Property ordertype As String
        Public Property active As String
        Public Property customeraccountfilter As String
    
       
        Public Property draw As Integer
        Public Property start As Integer
        Public Property length As Integer
        Public Property order As List(Of OrderParam)
        Public Property columns As List(Of ColumnParam)
        Public Property search As SearchParam
    End Class

    Public Class OrderParam
        Public Property column As Integer
        Public Property dir As String
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
        Public Property data As List(Of OrdersMatrixReturnRow)
        Public Property [error] As String
    End Class

    Public Class OrdersMatrixReturnRow
        Public Property No As String 
        Public Property Id As String 
        Public Property OrderId As String 
        Public Property CustomerId As String 
        Public Property JoNumber As String 
        Public Property CustomerName As String 
        Public Property OrderNumber As String 
        Public Property OrderName As String 
        Public Property OrderType As String 
        Public Property Delivery As String 
        Public Property Status As String 
        Public Property StatusVal As String 
        Public Property StatusAdditional As String 
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
        Public Property loginid As String
    End Class

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindProductType(ByVal customerid As String, ByVal username As String, ByVal rolename As String) As Object
        Try
            Dim Name As String = ""
            If customerid = "LS-A224" Then '#JPM Direct
                Name = "AND Name IN ('Panorama')"
            End If
            If customerid = "DEFAULT" AndAlso username = "galih" Then
                Name = "AND Name IN ('Panorama', 'Evolve')"
            End If

            Dim Environment As String = ""
            If rolename = "Customer" Then
                Environment = "AND Description = 'Environment : Production'"
            End If
            If InArray(rolename, "PPIC & DE", "Manager", "Customer Service") Then
                Environment = "AND Description IN ('Environment : Production', 'Environment : Testing')"
            End If
            Dim datas As DataSet = publicCfg.GetListData(String.Format("SELECT Name FROM ProductType WHERE Active=1 {0} {1} ORDER BY Name ASC", Name, Environment))
            Dim list As New List(Of String)()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    list.Add(row("Name").ToString())
                Next
                list.Add("ALL")
            End If
            
            Return New With {.error = false, .list = list}
        Catch ex As Exception
            Return New With {.error = True, .message = String.Format("BindProductType: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrders(params As OrdersParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of OrdersMatrixReturnRow)()

        
        Try
            Using conn As New SqlConnection(myConn)
                conn.Open()

                '#---------------------------------------------------------|| Addtitional Where Query ||---------------------------------------------------------#
                Dim FindWhere As Object = FindWhereOrder(params)
                If FindWhere.error Then
                    Throw New Exception(FindWhere.message)
                End If
                
                '#---------------------------------------------------------|| Count Records ||---------------------------------------------------------#
                Dim countSql As String = String.Format("SELECT COUNT( Id ) FROM view_order_headers WHERE Active = @Active {0} {1} {2} AND (@CustomerAccount = 'ALL' OR CustomerAccount = @CustomerAccount)", FindWhere.whereRole, FindWhere.whereOrderType, FindWhere.statusMerged)
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@Status", params.status)
                    countCmd.Parameters.AddWithValue("@OrderType", params.ordertype)
                    countCmd.Parameters.AddWithValue("@Active", params.active)
                    countCmd.Parameters.AddWithValue("@CustomerAccount", params.customeraccountfilter)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                
                '#---------------------------------------------------------|| Mian Query ||---------------------------------------------------------#
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT Id, OrderId, JoNumberId, CustomerId, OrderNumber, OrderName, Delivery, OrderType, Status, StatusAdditional, CreatedDate, SubmittedDate,CanceledDate, CompletedDate, Active, CustomerName")
                sqlBuilder.AppendLine("FROM view_order_headers")
                sqlBuilder.AppendLine(String.Format("WHERE Active = @Active {0} {1} {2} AND (@CustomerAccount = 'ALL' OR CustomerAccount = @CustomerAccount)", FindWhere.whereRole, FindWhere.whereOrderType, FindWhere.statusMerged))

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@Status", params.status)
                cmd.Parameters.AddWithValue("@OrderType", params.ordertype)
                cmd.Parameters.AddWithValue("@Active", params.active)
                cmd.Parameters.AddWithValue("@CustomerAccount", params.customeraccountfilter)

                '#-------------------------------------------------|| Global Search ||-------------------------------------------------#
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = String.Format("%{0}%", params.search.value.Trim())
                    whereClause.AppendLine(" AND (OrderId LIKE @SearchValue OR CustomerName LIKE @SearchValue OR OrderNumber LIKE @SearchValue OR OrderName LIKE @SearchValue )")
                    cmd.Parameters.AddWithValue("@SearchValue", searchValue)
                End If

                sqlBuilder.Append(whereClause.ToString())
                Dim filteredCountSql As String = String.Format("SELECT COUNT(T.Id) FROM ({0}) AS T", sqlBuilder.ToString())
                Using filteredCountCmd As New SqlCommand(filteredCountSql, conn)
                    For Each p As SqlParameter In cmd.Parameters
                        filteredCountCmd.Parameters.Add(New SqlParameter(p.ParameterName, p.Value))
                    Next
                    filteredRecords = CInt(filteredCountCmd.ExecuteScalar())
                End Using
                

                '#-------------------------------------------------|| Order By ||-------------------------------------------------#
                Dim orderByClause As New System.Text.StringBuilder()
                Dim FindOrdered As Object = FindOrderedOrder(params)
                If FindOrdered.error Then
                    Throw New Exception(FindOrdered.message)
                End If

                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Id"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        orderByClause.AppendLine(FindOrdered.customOrderBy)
                    End If
                Else
                    orderByClause.AppendLine(FindOrdered.customOrderBy)
                End If
                sqlBuilder.Append(orderByClause.ToString())
                sqlBuilder.AppendLine(" OFFSET " & params.start.ToString() & " ROWS FETCH NEXT " & params.length.ToString() & " ROWS ONLY")
                cmd.CommandText = sqlBuilder.ToString()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim noCounter As Integer = params.start + 1

                    While reader.Read()

                        Dim Delivery As Object = FindDelivery(reader)
                        If Delivery.error Then
                            Throw New Exception(Delivery.message)
                        End If

                        Dim Status As Object = FindStatus(reader)
                        If Status.error Then
                            Throw New Exception(Status.message)
                        End If

                        Dim Dates As Object = FindDates(reader)
                        If Dates.error Then
                            Throw New Exception(Dates.message)
                        End If

                        Dim row As New OrdersMatrixReturnRow With {
                            .No = noCounter.ToString(),
                            .Id = reader("Id").ToString(),
                            .OrderId = reader("OrderId").ToString(),
                            .CustomerId = reader("CustomerId").ToString(),
                            .JoNumber = reader("JoNumberId").ToString(),
                            .CustomerName = reader("CustomerName").ToString(),
                            .OrderNumber = reader("OrderNumber").ToString(),
                            .OrderName = reader("OrderName").ToString(),
                            .OrderType = reader("OrderType").ToString(),
                            .Delivery = Delivery.value,
                            .Status = Status.value,
                            .StatusVal = reader("Status").ToString(),
                            .StatusAdditional = reader("StatusAdditional").ToString(),
                            .CreatedDate = Dates.CreatedDate,
                            .SubmittedDate = Dates.SubmittedDate,
                            .CanceledDate = Dates.CanceledDate,
                            .CompletedDate = Dates.CompletedDate,
                            .Active = reader("Active").ToString()
                        }
                        resultList.Add(row)
                        noCounter += 1
                    End While
                End Using

            End Using

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
            response.error = String.Format("BindOrders: {0} | {1}", ex.Message, ex.StackTrace)
            Return response
        End Try
    End Function

    Private Shared Function FindWhereOrder(ByVal params As OrdersParams) As Object
        Try
            Dim statusMerged As String = "AND (@Status = 'ALL' OR Status = @Status)"
            If params.status = "Draft" Then
                statusMerged = "AND (@Status = 'ALL' OR Status IN ('Draft', 'Unsubmitted'))"
            End If

            Dim whereRole As String = String.Format(" AND CustomerId = '{0}'", params.customerid)
            IF params.customercompany = "SP" Then
                whereRole = ""
                If params.rolename = "PPIC & DE" Then
                    ' whereRole = " AND CustomerCompany = '" + params.customercompany + "'"
                    whereRole = ""
                End If

                If params.rolename = "Customer" Then
                    whereRole = String.Format(" AND CustomerId = '{0}'", params.customerid)
                    If params.levelname = "Member" Then
                        whereRole = String.Format(" AND CreatedBy = '{0}'", params.loginid)
                    End If
                End If
            ElseIf params.customercompany = "LOOP" Then
                Select Case params.rolename
                    Case "Administrator", "Customer Service", "Data Entry", "PPIC & DE", "Account"
                        whereRole = ""
                    Case "Representative"
                        whereRole = String.Format(" AND CustomerId = '{0}' AND CreatedBy = '{1}'", params.customerid, params.loginid)
                    Case "Customer"
                        whereRole = String.Format(" AND CustomerId = '{0}'", params.customerid)
                        If params.customeraccount = "Master" Then
                            whereRole = String.Format(" AND CustomerId = '{0}' AND CustomerMasterId = '{1}'", params.customerid)
                        End If
                End Select
            ElseIf params.customercompany = "ALL" Then
                whereRole = ""
            End If

            Dim whereOrderType As String = " AND (@OrderType = 'ALL' OR OrderType = @OrderType) "
            If params.loginid = "admin" or params.username = "galih" Then
                whereOrderType = " AND OrderType <> 'Blinds' "
            End If
            
            Return New With {.error = false, .statusMerged = statusMerged, .whereRole = whereRole, .whereOrderType = whereOrderType}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindWhereOrder: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindOrderedOrder(ByVal params As OrdersParams) As Object
        Try
            Dim customOrderBy As String = " ORDER BY CreatedDate DESC"
            IF params.customercompany = "SP" Then
                customOrderBy = " ORDER BY CreatedDate DESC"
            ElseIf params.customercompany = "LOOP" Then
                customOrderBy = " ORDER BY CreatedDate DESC"
                If params.rolename = "Administrator" Then
                    customOrderBy = " ORDER BY CreatedDate, CASE WHEN Status = 'New Order' THEN 1 WHEN Status = 'In Production' THEN 2 WHEN Status = 'Completed' THEN 3 WHEN Status = 'Unsubmitted' THEN 4 WHEN Status = 'Draft' THEN 4 WHEN Status = 'Canceled' THEN 5 END DESC"
                End If

                If params.rolename = "Customer Service" Or params.rolename = "Data Entry" Or params.rolename = "PPIC & DE" Then
                    customOrderBy = " ORDER BY CreatedDate, CASE WHEN Status = 'New Order' THEN 1 WHEN Status = 'In Production' THEN 2 WHEN Status = 'Completed' THEN 3 WHEN Status = 'Unsubmitted' THEN 4 WHEN Status = 'Draft' THEN 4 WHEN Status = 'Canceled' THEN 5 END DESC"
                End If

                If params.rolename = "Account" Then
                    customOrderBy = " ORDER BY OrderHeaders.CreatedDate, CASE WHEN Status = 'New Order' THEN 1 WHEN Status = 'In Production' THEN 2 WHEN Status = 'Completed' THEN 3 END DESC"
                End If

                If params.rolename = "Representative" Then
                    customOrderBy = " ORDER BY CreatedDate DESC"
                End If

                If params.rolename = "Customer" Then
                    customOrderBy = " ORDER BY CreatedDate DESC"
                End If
            ElseIf params.customercompany = "ALL" Then
                customOrderBy = " ORDER BY CreatedDate DESC"
            End If
            
            Return New With {.error = false, .customOrderBy = customOrderBy}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindOrderedOrder: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindDelivery(reader As SqlDataReader) As Object
        Try
            Dim result As String = String.Empty
            Dim delivery As String = reader("Delivery").ToString()
            Select Case delivery
                Case "Pick Up"
                    result = String.Format("<div class='badge bg-pink-lt d-flex align-item-center'><i class='ti ti-truck-delivery fs-3'></i> {0}</div>", delivery)
                Case "Delivery"
                    result = String.Format("<div class='badge bg-cyan-lt d-flex align-item-center'><i class='ti ti-package fs-3'></i> {0}</div>", delivery)
                Case "INT-PU", "INT-FIS"
                    result = String.Format("<div class='badge bg-orange-lt d-flex align-item-center'><i class='ti ti-building-skyscraper fs-3'></i> {0}</div>", delivery)
            End Select
            
            Return New With {.error = false, .value = result}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindDelivery: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindStatus(reader As SqlDataReader) As Object
        Try
            Dim result As String = String.Empty
            Dim status As String = reader("Status").ToString()
            Dim statusadd As String = reader("StatusAdditional").ToString()
            Dim ordertype As String = reader("OrderType").ToString()
           
            Dim icon As String = String.Empty
            Dim addStat As String = String.Empty
            If InArray(ordertype, "Panorama", "Evolve") Then
                addStat = String.Format("<br> <span class='text-secondary'>{0}</span>", statusadd)
            End If

            Select Case status
                Case "Draft", "Unsubmitted", "Pending Price Approval"
                    icon = "<i class='ti ti-clock opacity-50 fs-3 me-1'></i>"
                Case "New Order"
                    icon = "<i class='ti ti-clipboard-check opacity-50 fs-3 me-1'></i>"
                Case "In Production"
                    icon = "<i class='ti ti-hourglass-high opacity-50 fs-3 me-1'></i>"
                Case "On Hold"
                    icon = "<i class='ti ti-clock-pause opacity-50 fs-3 me-1'></i>"
                Case "Canceled"
                    icon = "<i class='ti ti-square-rounded-x opacity-50 fs-3 me-1'></i>"
                Case "Completed"
                    icon = "<i class='ti ti-square-rounded-check opacity-50 fs-3 me-1'></i>"
            End Select

            result = String.Format("<span class='d-flex align-items-center'>{0} {1}</span> {2}", icon, status,  addStat)
            
            Return New With {.error = false, .value = result}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindStatus: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindDates(reader As SqlDataReader) As Object
        Try
            Dim Created As String = If(IsDBNull(reader("CreatedDate")), "", Convert.ToDateTime(reader("CreatedDate")).ToString("dd MMM yyyy"))
            Dim Submitted As String = If(IsDBNull(reader("SubmittedDate")), "", Convert.ToDateTime(reader("SubmittedDate")).ToString("dd MMM yyyy"))
            Dim Canceled As String = If(IsDBNull(reader("CanceledDate")), "", Convert.ToDateTime(reader("CanceledDate")).ToString("dd MMM yyyy"))
            Dim Completed As String = If(IsDBNull(reader("CompletedDate")), "", Convert.ToDateTime(reader("CompletedDate")).ToString("dd MMM yyyy"))
            
            Return New With {
                .error = false, 
                .CreatedDate = Created, 
                .SubmittedDate = Submitted, 
                .CanceledDate = Canceled, 
                .CompletedDate = Completed
            }
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindStatus: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared function SendProductionOrder() As Object
        Try
            mailCfg.MailProduction()

            Return New With { .success = true, .message = "Production Order has been sent successfully."}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("SendProductionOrder: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderByID(ByVal headerid As String, ByVal ordertype As String) As Object
        Try
            Dim HeaderData As Object
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand("SELECT * FROM OrderHeaders WHERE Id = @Id AND OrderType = @OrderType", conn)
                    cmd.Parameters.AddWithValue("@Id", headerid)
                    cmd.Parameters.AddWithValue("@OrderType", ordertype)

                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            HeaderData = New With {
                                .Id = reader("Id").ToString(),
                                .Status = reader("Status").ToString(),
                                .SubmittedDate = reader("SubmittedDate").ToString(),
                                .CompletedDate = reader("CompletedDate").ToString(),
                                .CanceledDate = reader("CanceledDate").ToString(),
                                .StatusDescription = reader("StatusDescription").ToString()
                            }
                        End If
                    End Using
                End Using
            End Using

            Return HeaderData
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

            If String.IsNullOrEmpty(data.id) Then
                Throw New Exception("this order is missing !")
            End If

            If String.IsNullOrEmpty(data.status) Then
                Return New With { .warning = true, .message = "status is required !", .field = "#modalChangeStatus #status"}
            End If
            If data.status = data.statusOld Then
                Return New With { .warning = true, .message = "you don't choose different changes on status, don't do it with the same status!", .field = "#modalChangeStatus #status"}
            End If


            If data.status = "New Order" Then
                If data.submittedDate = "" Then
                    Return New With { .warning = true, .message = "submitted date is required !", .field = "#modalChangeStatus #submitteddate"}
                End If
            End If

            If data.status = "Completed" Then
                If data.completeddate = "" Then
                    Return New With { .warning = true, .message = "shipped date is required !", .field = "#modalChangeStatus #completeddate"}
                End If
            End If

            If data.status = "Canceled" Then
                If data.canceleddate = "" Then
                    Return New With { .warning = true, .message = "canceled date is required !", .field = "#modalChangeStatus #canceleddate"}
                End If
            End If
            
            If data.description = "" Then
                Return New With { .warning = true, .message = "description is required !", .field = "#modalChangeStatus #description"}
            End If          

            Dim findDesc As String = data.description
            Select Case data.status
                Case "Draft"
                    findDesc = "Status changed to draft by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
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
                Dim query As String = "UPDATE OrderHeaders SET Status='Draft', StatusDescription=@StatusDescription, SubmittedDate=NULL, JobDate=NULL, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                Select Case data.status
                    Case "New Order"
                        query = "UPDATE OrderHeaders SET Status='New Order', StatusDescription=@StatusDescription, SubmittedDate=@SubmittedDate WHERE Id=@Id"
                    Case "In Production"
                        query = "UPDATE OrderHeaders SET Status='In Production', StatusDescription=@StatusDescription, JobDate=GETDATE() WHERE Id=@Id"
                    Case "On Hold"
                        query = "UPDATE OrderHeaders SET Status='On Hold', StatusDescription=@StatusDescription WHERE Id=@Id"
                    Case "Completed"
                        query = "UPDATE OrderHeaders SET Status='Completed', StatusDescription=@StatusDescription, CompletedDate=@CompletedDate WHERE Id=@Id"
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

                Dim dataLog As Object() = {data.id, "", "Blinds", data.loginid, "Update Status Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Status has been updated successfully."
             End If

            Return New With {.success = true, .message = msg}
        Catch ex As Exception
            Return New With {.error = true, .message = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SwitchOrder(ByVal id As String, ByVal action As String, ByVal type As String) As Object
        Try
            '#DELETE & RESTORE
            Dim msg As String = "Data has been restored successfully."
            Dim Key As String = "Active=1"
            If Not String.IsNullOrEmpty(id) Then
                If action = "delete" Then
                    Key = "Active=0"
                    msg = "Data has been deleted successfully."
                End If

                Dim tableTo As String = "OrderHeaders"
                If type = "Panorama" Or type = "Evolve" Then
                    tableTo = "OrderHeaders_Shutters"
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE " & tableTo & " SET " & Key & " WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", id)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

            End If 
            Return New With { .success = true, .message = msg}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("SwitchOrder: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindLogs(ByVal headerid As String, ByVal ordertype As String) As Object
        Try
            Dim Logs As DataSet = publicCfg.GetListData(String.Format("SELECT CustomerLogins.FullName, Log_Orders.ItemId, Log_Orders.ActionDate, Log_Orders.Description FROM Log_Orders INNER JOIN CustomerLogins ON Log_Orders.ActionBy=CustomerLogins.Id WHERE Log_Orders.HeaderId='{0}' AND Log_Orders.Type='{1}'  ORDER BY ActionDate DESC", headerid, ordertype))

            Dim LogsData As List(Of Dictionary(Of String, Object)) = New List(Of Dictionary(Of String, Object))()
            For Each row As DataRow In Logs.Tables(0).Rows
                Dim dict As New Dictionary(Of String, Object)()
                For Each col As DataColumn In Logs.Tables(0).Columns
                    dict.Add(col.ColumnName, row(col))
                Next
                LogsData.Add(dict)
            Next

            Return LogsData
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("BindLogs: {0}", ex.Message)}
        End Try
    End Function

End Class
