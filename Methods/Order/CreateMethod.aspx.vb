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
Partial Class Methods_Order_CreateMethod
    Inherits System.Web.UI.Page
    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()

    Public Class ParamSubmit
        Public Property id  As String
        Public Property ordertype As String
        Public Property customer As String
        Public Property createdby As String
        Public Property createddate As String
        Public Property ordernumber As String
        Public Property ordername As String
        Public Property delivery As String
        Public Property note As String

        Public Property loginid As String
        Public Property actions As String
        Public Property headerid As String
    End Class

    '#--- Kelas Output WebMethod ---#
    Public Class ErrorDetail
        Public Property message As String
        Public Property field As String
    End Class

    Public Class ErrorResponse
        Public Property [error] As ErrorDetail
    End Class

    Public Class SuccessDetail
        Public Property message As String
        Public Property url As String
    End Class
    Public Class SuccessResponse
        Public Property [success] As SuccessDetail
    End Class

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Find(ByVal id As String, ByVal ordertype As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM view_order_headers WHERE Id = '" + UCase(id).ToString() + "' AND OrderType = '" + ordertype + "'")

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
    Public Shared Function Submit(ByVal data As ParamSubmit) As Object
        Try
            If String.IsNullOrEmpty(data.ordertype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order type is required !", .field = "ordertype"}}
            End If
            If String.IsNullOrEmpty(data.customer) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "customer is required !", .field = "customer"}}
            End If

            If Not String.IsNullOrEmpty(data.ordertype) AndAlso data.ordertype = "panorama" Then
                If String.IsNullOrEmpty(data.createdby) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "created by is required !", .field = "createdby"}}
                End If
                If String.IsNullOrEmpty(data.createddate) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "created date is required !", .field = "createddate"}}
                End If
            End If

            If String.IsNullOrEmpty(data.ordernumber) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number is required !", .field = "ordernumber"}}
            End If
            If Not String.IsNullOrEmpty(data.ordernumber) Then
                If InStr(data.ordernumber, "\") > 0 Or  InStr(data.ordernumber, "/") > 0 Or  InStr(data.ordernumber, "|") > 0 Or  InStr(data.ordernumber, ",") > 0 Or  InStr(data.ordernumber, "&") > 0 Or  InStr(data.ordernumber, "#") > 0 Or  InStr(data.ordernumber, "'") > 0 Or  InStr(data.ordernumber, ".") > 0 Or  InStr(data.ordernumber, "`") > 0 Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number do not use special character !", .field = "ordernumber"}}
                End If

                If data.ordernumber.Length > 20 Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number max length is 20 !", .field = "ordernumber"}}
                End If

                If data.actions = "add" Then
                    '# check by customer
                    Dim OrderNumber As String = data.ordernumber.Trim()
                    Dim OrderNumberByCust As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '" & OrderNumber & "' AND CustomerId = '" & data.customer & "' AND Active = 1")

                    If String.Equals(OrderNumber.Trim(), OrderNumberByCust, StringComparison.OrdinalIgnoreCase) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                    End If

                    '# check by all customer
                    Dim OrderNumberAllCust As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '" & OrderNumber & "' AND Active = 1")
                    If String.Equals(OrderNumber.Trim(), OrderNumberAllCust, StringComparison.OrdinalIgnoreCase) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                    End If
                Else If data.actions = "edit" Then
                    '# check by customer
                    Dim OrderNumber As String = data.ordernumber.Trim()
                    Dim OrderNumberByCust As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '" & OrderNumber & "' AND CustomerId = '" & data.customer & "' AND Id <> '" & data.headerid & "' AND Active = 1")

                    If String.Equals(OrderNumber.Trim(), OrderNumberByCust, StringComparison.OrdinalIgnoreCase) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                    End If

                    '# check by all customer
                    Dim OrderNumberAllCust As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '" & OrderNumber & "' AND Id <> '" & data.headerid & "' AND Active = 1")
                    If String.Equals(OrderNumber.Trim(), OrderNumberAllCust, StringComparison.OrdinalIgnoreCase) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                    End If
                End If


            End If


            If String.IsNullOrEmpty(data.ordername) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order name is required !", .field = "ordername"}}
            End If
            If Not String.IsNullOrEmpty(data.ordername) Then
                If InStr(data.ordername, "\") > 0 Or  InStr(data.ordername, "/") > 0 Or  InStr(data.ordername, "|") > 0 Or  InStr(data.ordername, ",") > 0 Or  InStr(data.ordername, "&") > 0 Or  InStr(data.ordername, "#") > 0 Or  InStr(data.ordername, "'") > 0 Or  InStr(data.ordername, ".") > 0 Or  InStr(data.ordername, "`") > 0 Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number do not use special character !", .field = "ordername"}}
                End If
            End If

            If Not String.IsNullOrEmpty(data.ordertype) AndAlso data.ordertype = "blinds" Then
                If String.IsNullOrEmpty(data.delivery) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "delivery is required !", .field = "delivery"}}
                End If
            End If

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString    

            Dim msg As String = "200"
            Dim url As String = "/"
            '#insert
            If String.IsNullOrEmpty(data.id) Then
                IF data.ordertype = "Blinds" Then
                    Dim id As String = publicCfg.CreateOrderHeaderId()
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, UserId, StoreId, OrderNo, OrderCust, Delivery, Note, QuoteGST, QuoteDisc, QuoteInstall, QuoteMeasure, Status, CreatedDate, Active) VALUES (@Id, @UserId, @StoreId, LTRIM(RTRIM(@OrderNo)), LTRIM(RTRIM(@OrderCust)), @Delivery, @Note, 'Yes', 0, 0, 0,  'Draft', GETDATE(), 1)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", id)
                            myCmd.Parameters.AddWithValue("@UserId", UCase(data.loginid).ToString())
                            myCmd.Parameters.AddWithValue("@StoreId", UCase(data.customer).ToString())
                            myCmd.Parameters.AddWithValue("@OrderNo", data.ordernumber)
                            myCmd.Parameters.AddWithValue("@OrderCust", data.ordername)
                            myCmd.Parameters.AddWithValue("@Delivery", data.delivery)
                            myCmd.Parameters.AddWithValue("@Note", data.Note)
                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = "/order/detail?ultron=" & id & "&infinity=" & data.ordertype
                Else If data.ordertype = "Panorama" Then
                    Dim headerId As String = orderCfg.CreateOrderHeaderId()
                    Dim orderId As String = "SPP-" & headerId

                    Dim createdBy As String = UCase(data.createdby).ToString()
                    If createdBy = "" Then
                        createdBy = UCase(data.loginid).ToString()
                    End If

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders_Shutters(Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, Deposit, Approved, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, 'Panorama', 'Unsubmitted', @CreatedBy, @CreatedDate, 0, 0, 1) INSERT INTO OrderQuotes VALUES (@Id, '', '', '', '', '', '', 0.00, 0.00, 0.00, 0.00)")
                            myCmd.Parameters.AddWithValue("@Id", headerId)
                            myCmd.Parameters.AddWithValue("@OrderId", orderId)
                            myCmd.Parameters.AddWithValue("@CustomerId", UCase(data.customer).ToString())
                            myCmd.Parameters.AddWithValue("@OrderNumber", data.ordernumber.Trim())
                            myCmd.Parameters.AddWithValue("@OrderName", data.ordernumber.Trim())
                            myCmd.Parameters.AddWithValue("@OrderNote",data.Note.Trim())
                            myCmd.Parameters.AddWithValue("@CreatedBy", createdBy)
                            myCmd.Parameters.AddWithValue("@CreatedDate", data.createddate)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = "/order/loop/detail?ultron=" & headerId & "&infinity=" & data.ordertype
                End If
                msg = "Data has been saved successfully. <br /> Click oke to continue."
            End If

            '#update
            If Not String.IsNullOrEmpty(data.id) Then
                IF data.ordertype = "Blinds" Then
                     Dim id As String = data.id
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand("UPDATE OrderHeaders SET UserId=@UserId, StoreId=@StoreId, OrderNo=LTRIM(RTRIM(@OrderNo)), OrderCust=LTRIM(RTRIM(@OrderCust)), Delivery=@Delivery, Note=@Note, Active=1 WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", id)
                            myCmd.Parameters.AddWithValue("@UserId", UCase(data.loginid).ToString())
                            myCmd.Parameters.AddWithValue("@StoreId", UCase(data.customer).ToString())
                            myCmd.Parameters.AddWithValue("@OrderNo", data.ordernumber)
                            myCmd.Parameters.AddWithValue("@OrderCust", data.ordername)
                            myCmd.Parameters.AddWithValue("@Delivery", data.delivery)
                            myCmd.Parameters.AddWithValue("@Note", data.Note)
                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = "/order/detail?ultron=" & id & "&infinity=" & data.ordertype
                Else If data.ordertype = "Panorama" Then
                    url = "/order/loop/detail?ultron=" & data.id & "&infinity=" & data.ordertype
                End If
                msg = "Data has been updated successfully."
            End If

            Return New SuccessResponse With {.success = New SuccessDetail With {.message = msg, .url = url}}
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
    Public Shared Function BindCustomer(ByVal ordertype As String, ByVal rolename As String) As Object
        Try
            Dim company As String = "LOOP"
            Dim query As String = "SELECT Id, Name, Company, Active FROM view_customers WHERE Company IN ('" + company + "', 'ALL') AND (Id <> 'DEFAULT' AND Id <> 'LIFESTYLE' ) AND Active='1' ORDER BY Name ASC"

            If ordertype = "Blinds" Then 
                company = "SP" 
                query = "SELECT Id, Name, Company, Active FROM view_customers WHERE Company IN ('" + company + "', 'ALL') AND (Id <> 'DEFAULT' AND Id <> 'LIFESTYLE' ) AND Active='1' ORDER BY Name ASC"
            Else If ordertype = "Panorama" Then
                company = "LOOP"
                query = "SELECT Id, Name, Company, Active FROM view_customers WHERE Company IN ('" + company + "', 'ALL') AND Active='1' ORDER BY Name ASC"
                If rolename = "Customer Service" Or rolename = "Data Entry" Then
                    query = "SELECT Id, Name, Company, Active FROM view_customers WHERE Company IN ('" + company + "', 'ALL') AND Id <> 'DEFAULT' AND Active='1' ORDER BY Name ASC"
                End If
            End If
            
            Dim datas As DataSet = publicCfg.GetListData(query)
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
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindUser() As Object
        Try
           Dim datas As DataSet = publicCfg.GetListData("SELECT UPPER(Id) AS IdText, UPPER(UserName) + ' | ' + UPPER(FullName) AS NameText FROM view_customer_logins ORDER BY UserName ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("IdText").ToString()},
                        {"text", row("NameText").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindShipment() As Object
        Try
           Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM OrderShipments ORDER BY Id ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("ShipmentNumber").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindShipping(ByVal customerid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM CustomerAddress WHERE CustomerId = '" + customerid + "' AND [Primary] = 1")

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

End Class
