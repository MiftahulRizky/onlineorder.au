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

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderHeaderAggregate(ByVal data As ParamBindOrderHeaderAggregate) As Object
        Try
            Dim ProductType As Object = BindProductType(data.customerid, data.username, data.rolename)
            If ProductType.error Then
                Throw New Exception(ProductType.message)
            End If

            Dim Orders As New List(Of Object)()
            Dim FindWhere As Object = FindWhereOrder(data)
            If FindWhere.error Then
                Throw New Exception(FindWhere.message)
            End If

            Dim FindOrdered As Object = FindOrderedOrder(data)
            If FindOrdered.error Then
                Throw New Exception(FindOrdered.message)
            End If

            Dim searchValue As String = String.Format("%{0}%", data.search.Trim())


            Using conn As New SqlConnection(myConn)
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT Id, OrderId, JoNumberId, CustomerId, OrderNumber, OrderName, Delivery, OrderType, Status, StatusAdditional, CreatedDate, SubmittedDate,CanceledDate, CompletedDate, Active, CustomerName")
                sqlBuilder.AppendLine("FROM view_order_headers")
                sqlBuilder.AppendLine(String.Format("WHERE Active = @Active {0} {1} {2} AND (@CustomerAccount = 'ALL' OR CustomerAccount = @CustomerAccount)", FindWhere.whereRole, FindWhere.whereOrderType, FindWhere.statusMerged))
                sqlBuilder.AppendLine(" AND (OrderId LIKE @SearchValue OR CustomerName LIKE @SearchValue OR OrderNumber LIKE @SearchValue OR OrderName LIKE @SearchValue )")
                 sqlBuilder.AppendLine(FindOrdered.customOrderBy)
                 
                 
                 
                 Using cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                    cmd.Parameters.AddWithValue("@Status", data.status)
                    cmd.Parameters.AddWithValue("@OrderType", data.ordertype)
                    cmd.Parameters.AddWithValue("@Active", data.active)
                    cmd.Parameters.AddWithValue("@CustomerAccount", data.storetype)
                    cmd.Parameters.AddWithValue("@SearchValue", searchValue)

                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim Created As String = If(IsDBNull(reader("CreatedDate")), "", Convert.ToDateTime(reader("CreatedDate")).ToString("dd MMM yyyy"))
                            Dim Submitted As String = If(IsDBNull(reader("SubmittedDate")), "", Convert.ToDateTime(reader("SubmittedDate")).ToString("dd MMM yyyy"))
                            Dim Canceled As String = If(IsDBNull(reader("CanceledDate")), "", Convert.ToDateTime(reader("CanceledDate")).ToString("dd MMM yyyy"))
                            Dim Completed As String = If(IsDBNull(reader("CompletedDate")), "", Convert.ToDateTime(reader("CompletedDate")).ToString("dd MMM yyyy"))

                            Dim Delivery As Object = FindDelivery(reader("Delivery").ToString())
                            If Delivery.error Then
                                Throw New Exception(Delivery.message)
                            End If

                            Dim Status As Object = FindStatus(reader("Status").ToString(), reader("StatusAdditional").ToString(), reader("OrderType").ToString())
                            If Status.error Then
                                Throw New Exception(Status.message)
                            End If

                            Orders.Add( New With {
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
                                .CreatedDate = Created,
                                .SubmittedDate = Submitted,
                                .CanceledDate = Canceled,
                                .CompletedDate = Completed,
                                .Active = reader("Active").ToString()
                            })
                        End While
                    End Using
                End Using
            End Using

            Return New With {
                .ProductType = ProductType.data,
                .FindWhere = FindWhere,
                .Orders = Orders
            }
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("BindOrderHeaderAggregate: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function BindProductType(ByVal customerid As String, ByVal username As String, ByVal rolename As String) As Object
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
            
            Return New With {.error = false, .data = list}
        Catch ex As Exception
            Return New With {.error = True, .message = String.Format("BindProductType: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindWhereOrder(ByVal data As ParamBindOrderHeaderAggregate) As Object
        Try
            Dim statusMerged As String = "AND (@Status = 'ALL' OR Status = @Status)"
            If data.status = "Draft" Then
                statusMerged = "AND (@Status = 'ALL' OR Status IN ('Draft', 'Unsubmitted'))"
            End If

            Dim whereRole As String = String.Format(" AND CustomerId = '{0}'", data.customerid)
            IF data.customercompany = "SP" Then
                whereRole = ""
                If data.rolename = "PPIC & DE" Then
                    ' whereRole = " AND CustomerCompany = '" + data.customercompany + "'"
                    whereRole = ""
                End If

                If data.rolename = "Customer" Then
                    whereRole = String.Format(" AND CustomerId = '{0}'", data.customerid)
                    If data.levelname = "Member" Then
                        whereRole = String.Format(" AND CreatedBy = '{0}'", data.loginid)
                    End If
                End If
            ElseIf data.customercompany = "LOOP" Then
                Select Case data.rolename
                    Case "Administrator", "Customer Service", "Data Entry", "PPIC & DE", "Account"
                        whereRole = ""
                    Case "Representative"
                        whereRole = String.Format(" AND CustomerId = '{0}' AND CreatedBy = '{1}'", data.customerid, data.loginid)
                    Case "Customer"
                        whereRole = String.Format(" AND CustomerId = '{0}'", data.customerid)
                        If data.customeraccount = "Master" Then
                            whereRole = String.Format(" AND CustomerId = '{0}' AND CustomerMasterId = '{1}'", data.customerid)
                        End If
                End Select
            ElseIf data.customercompany = "ALL" Then
                whereRole = ""
            End If

            Dim whereOrderType As String = " AND (@OrderType = 'ALL' OR OrderType = @OrderType) "
            If data.loginid = "admin" or data.username = "galih" Then
                whereOrderType = " AND OrderType <> 'Blinds' "
            End If
            
            Return New With {.error = false, .statusMerged = statusMerged, .whereRole = whereRole, .whereOrderType = whereOrderType}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindWhereOrder: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindOrderedOrder(ByVal data As ParamBindOrderHeaderAggregate) As Object
        Try
            Dim customOrderBy As String = " ORDER BY CreatedDate DESC"
            IF data.customercompany = "SP" Then
                customOrderBy = " ORDER BY CreatedDate DESC"
            ElseIf data.customercompany = "LOOP" Then
                customOrderBy = " ORDER BY CreatedDate DESC"
                If data.rolename = "Administrator" Then
                    customOrderBy = " ORDER BY CreatedDate, CASE WHEN Status = 'New Order' THEN 1 WHEN Status = 'In Production' THEN 2 WHEN Status = 'Completed' THEN 3 WHEN Status = 'Unsubmitted' THEN 4 WHEN Status = 'Draft' THEN 4 WHEN Status = 'Canceled' THEN 5 END DESC"
                End If

                If data.rolename = "Customer Service" Or data.rolename = "Data Entry" Or data.rolename = "PPIC & DE" Then
                    customOrderBy = " ORDER BY CreatedDate, CASE WHEN Status = 'New Order' THEN 1 WHEN Status = 'In Production' THEN 2 WHEN Status = 'Completed' THEN 3 WHEN Status = 'Unsubmitted' THEN 4 WHEN Status = 'Draft' THEN 4 WHEN Status = 'Canceled' THEN 5 END DESC"
                End If

                If data.rolename = "Account" Then
                    customOrderBy = " ORDER BY OrderHeaders.CreatedDate, CASE WHEN Status = 'New Order' THEN 1 WHEN Status = 'In Production' THEN 2 WHEN Status = 'Completed' THEN 3 END DESC"
                End If

                If data.rolename = "Representative" Then
                    customOrderBy = " ORDER BY CreatedDate DESC"
                End If

                If data.rolename = "Customer" Then
                    customOrderBy = " ORDER BY CreatedDate DESC"
                End If
            ElseIf data.customercompany = "ALL" Then
                customOrderBy = " ORDER BY CreatedDate DESC"
            End If
            
            Return New With {.error = false, .customOrderBy = customOrderBy}
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("FindOrderedOrder: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindDelivery(ByVal delivery As String) As Object
        Try
            Dim result As String = String.Empty
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

    Private Shared Function FindStatus(ByVal status As String, ByVal statusadd As String, ByVal ordertype As String) As Object
        Try
            Dim result As String = String.Empty
           
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

End Class
