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
        '#Submit OrderHeader
        Public Property id  As String
        Public Property orderid As String
        Public Property jobid As String
        Public Property jobdate As String
        Public Property shipmentid As String
        Public Property ordertype As String
        Public Property customer As String
        Public Property createdby As String
        Public Property createddate As String
        Public Property ordernumber As String
        Public Property ordername As String
        Public Property delivery As String
        Public Property note As String

        '#Submit SubmitShipping
        ' Public Property id As String => On #Submit OrderHeader
        ' Public Property customer As String => On #Submit OrderHeader
        Public Property unitnumber As String
        Public Property streetaddress As String
        Public Property suburb As String
        Public Property states As String
        Public Property postcode As String
        Public Property addressport As String

        '#aditional param
        Public Property loginid As String
        Public Property rolename As String
        Public Property actions As String
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
    Public Shared Function GetItemData(ByVal query As String) As Object
        Try
            Dim Item As String = publicCfg.GetItemData(query)
            Return Item
        Catch ex As Exception
            Return "ERROR: " & ex.Message ' biar kelihatan errornya
        End Try
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

            If Not String.IsNullOrEmpty(data.ordertype) AndAlso data.ordertype = "Panorama" Then
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
                    ' '# check by customer
                    ' Dim OrderNumber As String = data.ordernumber.Trim()
                    ' Dim OrderNumberByCust As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '" & OrderNumber & "' AND CustomerId = '" & data.customer & "' AND Active = 1")

                    ' If String.Equals(OrderNumber.Trim(), OrderNumberByCust, StringComparison.OrdinalIgnoreCase) Then
                    '     Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                    ' End If

                    ' '# check by all customer
                    ' Dim OrderNumberAllCust As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '" & OrderNumber & "' AND Active = 1")
                    ' If String.Equals(OrderNumber.Trim(), OrderNumberAllCust, StringComparison.OrdinalIgnoreCase) Then
                    '     Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                    ' End If

                    '#cek by product type on customer same
                    Dim OrderNumber As String = data.ordernumber.Trim()
                    Dim FindOrderNumberByCust As String = publicCfg.GetItemData(String.Format("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '{0}' AND CustomerId = '{1}' AND OrderType='{2}' AND Active = '1'", OrderNumber, data.customer, data.ordertype))
                    If String.Equals(OrderNumber.Trim(), FindOrderNumberByCust, StringComparison.OrdinalIgnoreCase) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                    End If

                     '# check by all customer
                    Dim FindOrderNumberAllCust As String = publicCfg.GetItemData(String.Format("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '{0}' AND CustomerId <> '{1}' AND Active = 1", OrderNumber, data.customer))
                    If String.Equals(OrderNumber.Trim(), FindOrderNumberAllCust, StringComparison.OrdinalIgnoreCase) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                    End If


                Else If data.actions = "edit" Then
                '     '# check by customer
                '     Dim OrderNumber As String = data.ordernumber.Trim()
                '     Dim OrderNumberByCust As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '" & OrderNumber & "' AND CustomerId = '" & data.customer & "' AND Id <> '" & data.id & "' AND Active = 1")

                '     If String.Equals(OrderNumber.Trim(), OrderNumberByCust, StringComparison.OrdinalIgnoreCase) Then
                '         Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                '     End If

                '     '# check by all customer
                '     Dim OrderNumberAllCust As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '" & OrderNumber & "' AND Id <> '" & data.id & "' AND Active = 1")
                '     If String.Equals(OrderNumber.Trim(), OrderNumberAllCust, StringComparison.OrdinalIgnoreCase) Then
                '         Return New ErrorResponse With { .error = New ErrorDetail With { .message = "order number already exist !", .field = "ordernumber"}}
                '     End If

                    Dim OrderNumber As String = data.ordernumber.Trim()
                    Dim FindOrderNumberAllCust As String = publicCfg.GetItemData(String.Format("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '{0}' AND CustomerId <> '{1}' AND Active = 1", OrderNumber, data.customer))
                    If String.Equals(OrderNumber.Trim(), FindOrderNumberAllCust, StringComparison.OrdinalIgnoreCase) Then
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

            Dim FinalDelivery As String = data.delivery
            Dim CustomerDelivery As String = publicCfg.GetItemData(String.Format("SELECT Delivery FROM Customers WHERE Id = '{0}'", data.customer))
            If InArray(data.ordertype, "Blinds", "Door", "Window", "Door and Window") Then

                If String.IsNullOrEmpty(data.delivery) AND String.IsNullOrEmpty(CustomerDelivery) AND Not InArray(data.ordertype, "Door", "Window", "Door and Window") Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "delivery is required !", .field = "delivery"}}
                End If

                '#Create
                If String.IsNullOrEmpty(data.id) Then
                    If Not CustomerDelivery = "" Then
                        FinalDelivery = CustomerDelivery
                    End If
                End If

                '#Update
                If Not String.IsNullOrEmpty(data.id) Then
                    If String.IsNullOrEmpty(data.delivery) Then
                        FinalDelivery = CustomerDelivery
                    Else
                        FinalDelivery = data.delivery
                    End If

                    If data.ordertype = "Door" Then
                        FinalDelivery = ""
                    End If
                End If
            End If

            ' Throw New Exception("200")

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString    

            Dim msg As String = "200"
            Dim url As String = "/"
            '#insert
            If String.IsNullOrEmpty(data.id) Then
                IF InArray(data.ordertype, "Blinds", "Door", "Window", "Door and Window")  Then
                    Dim id As String = publicCfg.CreateOrderHeaderId()
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, UserId, StoreId, OrderNo, OrderCust, Delivery, OrderType, Note, QuoteGST, QuoteDisc, QuoteInstall, QuoteMeasure, Status, CreatedDate, Active) VALUES (@Id, @UserId, @StoreId, LTRIM(RTRIM(@OrderNo)), LTRIM(RTRIM(@OrderCust)), @Delivery, @OrderType, @Note, 'Yes', 0, 0, 0,  'Draft', GETDATE(), 1)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", id)
                            myCmd.Parameters.AddWithValue("@UserId", UCase(data.loginid).ToString())
                            myCmd.Parameters.AddWithValue("@StoreId", UCase(data.customer).ToString())
                            myCmd.Parameters.AddWithValue("@OrderNo", data.ordernumber)
                            myCmd.Parameters.AddWithValue("@OrderCust", data.ordername)
                            myCmd.Parameters.AddWithValue("@Delivery", FinalDelivery)
                            myCmd.Parameters.AddWithValue("@OrderType", data.ordertype)
                            myCmd.Parameters.AddWithValue("@Note", data.Note)
                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = "/order/detail?param=" & id & "&ordertype=" & data.ordertype.ToLower()

                    Dim dataLog As Object() = {id, "", data.ordertype, HttpContext.Current.Session("LoginId").ToString(), "Create Order"}
                    orderCfg.Log_Orders(dataLog)
                Else If InArray(data.ordertype, "Panorama" ,"Evolve") Then
                    Dim headerId As String = orderCfg.CreateOrderHeaderId()
                    Dim orderId As String = "SPP-" & headerId

                    Dim createdBy As String = UCase(data.createdby).ToString()
                    If createdBy = "" Then
                        createdBy = UCase(data.loginid).ToString()
                    End If

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders_Shutters(Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, Deposit, Approved, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, @OrderType, 'Unsubmitted', @CreatedBy, @CreatedDate, 0, 0, 1) INSERT INTO OrderQuotes VALUES (@Id, '', '', '', '', '', '', 0.00, 0.00, 0.00, 0.00)")
                            myCmd.Parameters.AddWithValue("@Id", headerId)
                            myCmd.Parameters.AddWithValue("@OrderId", orderId)
                            myCmd.Parameters.AddWithValue("@CustomerId", UCase(data.customer).ToString())
                            myCmd.Parameters.AddWithValue("@OrderNumber", data.ordernumber.Trim())
                            myCmd.Parameters.AddWithValue("@OrderName", data.ordername.Trim())
                            myCmd.Parameters.AddWithValue("@OrderNote", data.Note.Trim())
                            myCmd.Parameters.AddWithValue("@OrderType", data.ordertype)
                            myCmd.Parameters.AddWithValue("@CreatedBy", createdBy)
                            myCmd.Parameters.AddWithValue("@CreatedDate", data.createddate)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = "/order/shutters/detail?param=" & headerId & "&ordertype=" & data.ordertype.ToLower()

                    Dim dataLog As Object() = {headerId, "", data.ordertype, HttpContext.Current.Session("LoginId").ToString(), "Create Order"}
                    orderCfg.Log_Orders(dataLog)
                End If
                msg = "A new record has been created. <br />Click Next to continue."
            End If

            '#update
            If Not String.IsNullOrEmpty(data.id) Then
                IF InArray(data.ordertype, "Blinds","Door","Window", "Door and Window") Then
                    Dim id As String = data.id
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand("UPDATE OrderHeaders SET UserId=@UserId, StoreId=@StoreId, OrderNo=LTRIM(RTRIM(@OrderNo)), OrderCust=LTRIM(RTRIM(@OrderCust)), Delivery=@Delivery, Note=@Note, Active=1 WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", id)
                            myCmd.Parameters.AddWithValue("@UserId", UCase(data.createdby).ToString())
                            myCmd.Parameters.AddWithValue("@StoreId", UCase(data.customer).ToString())
                            myCmd.Parameters.AddWithValue("@OrderNo", data.ordernumber)
                            myCmd.Parameters.AddWithValue("@OrderCust", data.ordername)
                            myCmd.Parameters.AddWithValue("@Delivery", FinalDelivery)
                            myCmd.Parameters.AddWithValue("@Note", data.Note)
                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = "/order/detail?param=" & id & "&ordertype=" & data.ordertype.ToLower()
                Else If InArray(data.ordertype, "Panorama", "Evolve") Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET OrderId=@OrderId, JobId=@JobId, JobDate=@JobDate, ShipmentId=@ShipmentId, CustomerId=@CustomerId, CreatedBy=@CreatedBy, OrderNumber=@OrderNumber, OrderName=@OrderName, OrderNote=@OrderNote WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", data.id)
                            myCmd.Parameters.AddWithValue("@OrderId", data.orderid)
                            myCmd.Parameters.AddWithValue("@JobId", data.jobid)
                            myCmd.Parameters.AddWithValue("@JobDate", data.jobdate)
                            myCmd.Parameters.AddWithValue("@ShipmentId", If(String.IsNullOrEmpty(data.shipmentid), DBNull.Value, data.shipmentid))
                            myCmd.Parameters.AddWithValue("@CustomerId", UCase(data.customer).ToString())
                            myCmd.Parameters.AddWithValue("@CreatedBy", UCase(data.createdby).ToString())
                            myCmd.Parameters.AddWithValue("@CreatedDate", data.createddate)
                            myCmd.Parameters.AddWithValue("@OrderNumber", data.ordernumber.Trim())
                            myCmd.Parameters.AddWithValue("@OrderName", data.ordername.Trim())
                            myCmd.Parameters.AddWithValue("@OrderNote", data.note.Trim())

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = "/order/shutters/detail?param=" & data.id & "&ordertype=" & data.ordertype.ToLower()

                End If
                Dim dataLog As Object() = {data.id, "", data.ordertype, HttpContext.Current.Session("LoginId").ToString(), "Edit Order"}
                orderCfg.Log_Orders(dataLog)
                msg = "Data has been updated successfully."
            End If

            Return New SuccessResponse With {.success = New SuccessDetail With {.message = msg, .url = url}}
        Catch ex As Exception
            Dim msg As String = "Please contact our IT team at support@onlineorder.au"
            If data.rolename = "Administrator" Then msg = ex.Message
            Return New ErrorResponse With {.error = New ErrorDetail With {.message = msg,.field = ""}}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SubmitShipping(ByVal data As ParamSubmit) As Object
        Try
            If String.IsNullOrEmpty(data.streetaddress) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "street address is required !", .field = "streetaddress"}}
            End If
            If String.IsNullOrEmpty(data.suburb) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "suburb is required !", .field = "suburb"}}
            End If
            If String.IsNullOrEmpty(data.states) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "states is required !", .field = "states"}}
            End If
            If String.IsNullOrEmpty(data.postcode) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "postcode is required !", .field = "postcode"}}
            End If
            If String.IsNullOrEmpty(data.addressport) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "address port is required !", .field = "addressport"}}
            End If
            

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString    

            Dim msg As String = "200"
            Dim url As String = "/"
            '#insert
            If String.IsNullOrEmpty(data.id) Then
                Dim Id As string = orderCfg.GetCustomerAddressId()
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET [Primary]=0 WHERE CustomerId = @CustomerId; INSERT INTO CustomerAddress VALUES (NEWID(), @CustomerId, 'Delivery', @UnitNumber, @Street, @Suburb, @States, @PostCode, @Port, 'Delivery', NULL, 1)")
                        myCmd.Parameters.AddWithValue("@Id", Id)
                        myCmd.Parameters.AddWithValue("@CustomerId", UCase(data.customer).ToString())
                        myCmd.Parameters.AddWithValue("@UnitNumber", data.unitnumber.Trim())
                        myCmd.Parameters.AddWithValue("@Street", data.streetaddress.Trim())
                        myCmd.Parameters.AddWithValue("@Suburb", data.suburb.Trim())
                        myCmd.Parameters.AddWithValue("@States", data.states)
                        myCmd.Parameters.AddWithValue("@PostCode", data.postcode.Trim())
                        myCmd.Parameters.AddWithValue("@Port", data.addressport)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
                msg = "Shipping Address has been saved successfully. <br /> Click oke to continue."
            End If

            '#update
            If Not String.IsNullOrEmpty(data.id) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET UnitNumber=@UnitNumber, Street=@Street, Suburb=@Suburb, States=@States, PostCode=@PostCode, Port=@Port WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", data.id)
                        myCmd.Parameters.AddWithValue("@CustomerId", UCase(data.customer).ToString())
                        myCmd.Parameters.AddWithValue("@UnitNumber", data.unitnumber.Trim())
                        myCmd.Parameters.AddWithValue("@Street", data.streetaddress.Trim())
                        myCmd.Parameters.AddWithValue("@Suburb", data.suburb.Trim())
                        myCmd.Parameters.AddWithValue("@States", data.states)
                        myCmd.Parameters.AddWithValue("@PostCode", data.postcode.Trim())
                        myCmd.Parameters.AddWithValue("@Port", data.addressport)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
                msg = "Shipping Address has been updated successfully."
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
            Dim query As String = "SELECT Id, Name, Company, Delivery, Active FROM Customers WHERE Active='1' AND Id <> 'DEFAULT' ORDER BY Name ASC"

            Dim datas As DataSet = publicCfg.GetListData(query)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("Name").ToString()},
                        {"delivery", row("Delivery").ToString()}
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
           Dim datas As DataSet = publicCfg.GetListData("SELECT UPPER(Id) AS IdText, UPPER(UserName) + ' | ' + UPPER(FullName) AS NameText FROM CustomerLogins ORDER BY UserName ASC")
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


    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function

End Class
