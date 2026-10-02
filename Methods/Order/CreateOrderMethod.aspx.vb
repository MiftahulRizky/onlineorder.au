Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Imports System.IO
Imports System.Text
Partial Class Methods_Order_CreateOrderMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamListData
        Public Property field As String

        Public Property customerid As String
        Public Property username As String
        Public Property rolename As String
    End Class

    Public Class ParamSave
        '#Submit OrderHeader
        Public Property ordertype As String
        Public Property headerid As String
        Public Property customer As String
        Public Property customerold As String
        Public Property createdby As String
        Public Property createddate As String
        Public Property orderid As String
        Public Property createtype As String
        Public Property files As String
        Public Property ordernumber As String
        Public Property ordername As String
        Public Property delivery As String
        Public Property deliverytype As String
        Public Property sourcetype As String
        Public Property notes As String
        Public Property jobid As String
        Public Property jobdate As String
        Public Property shipmentid As String
        Public Property shippingaddress As String

        '#aditional param
        Public Property loginid As String
        Public Property action As String
        Public Property rolename As String
        Public Property csvContent As String
        Public Property fileName As String
    End Class

    Public Class ParamSaveShipping
        '#Submit OrderHeader
        Public Property unitnumber As String
        Public Property customer As String
        Public Property id As String
        Public Property streetaddress As String
        Public Property suburb As String
        Public Property states As String
        Public Property postcode As String
        Public Property addressport As String

        '#aditional param
        Public Property loginid As String
        Public Property rolename As String
    End Class

    Public Class ParamBindOrderAggregate
        Public Property headerid As String
        Public Property ordertype As String

        Public Property loginid As String
        Public Property rolename As String
    End Class

    Private Class SpacerInfo
        Public MinWidth As Integer
        Public MaxWidth As Integer
        Public Spacer1Type As String
        Public CarriersQty As Integer
    End Class

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindListData(ByVal data As ParamListData) As Object
        Try
            Dim query As String = ""
            Dim resultList As New List(Of Dictionary(Of String, String))() 

            Select Case data.field.ToLower()
                Case "ordertype"
                    Dim Name As String = ""
                    If data.customerid = "LS-A224" Then '#JPM Direct
                        Name = "AND Name IN ('Panorama')"
                    End If
                    If data.customerid = "DEFAULT" AndAlso data.username = "galih" Then
                        Name = "AND Name IN ('Panorama', 'Evolve')"
                    End If

                    Dim Environment As String = ""
                    If data.rolename = "Customer" Then
                        Environment = "AND Description = 'Environment : Production'"
                    End If
                    If InArray(data.rolename, "PPIC & DE", "Manager", "Customer Service") Then
                        Environment = "AND Description IN ('Environment : Production', 'Environment : Testing')"
                    End If

                    query = String.Format("SELECT Name FROM ProductType WHERE Active=1 {0} {1} ORDER BY Name ASC", Name, Environment)
                    resultList = GetFormattedData(query, "Name", "Name", data.field)

                Case "customer"

                    query = "SELECT Id, Name, Company, Delivery, Active FROM Customers WHERE Active='1' AND Id <> 'DEFAULT' ORDER BY Name ASC"
                    resultList = GetFormattedData(query, "Id", "Name", data.field)

                Case "createdby"

                    query = "SELECT UPPER(Id) AS IdText, UPPER(UserName) + ' | ' + UPPER(FullName) AS NameText FROM CustomerLogins ORDER BY UserName ASC"
                    resultList = GetFormattedData(query, "IdText", "NameText", data.field)

                Case "shipmentid"

                    query = "SELECT Id, ShipmentNumber FROM OrderShipments ORDER BY Id ASC"
                    resultList = GetFormattedData(query, "Id", "ShipmentNumber", data.field)

                Case Else
                    Return New With {.error = true, .message = "Invalid field"}
            End Select

            Return New With {
                .error = false,
                .list = resultList
            }
        Catch ex As Exception
            Return New With {.error = true, .message = String.Format("BindListData: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function GetFormattedData(query As String, valueField As String, textField As String, field As String) As Object
        Dim list As New List(Of Dictionary(Of String, String))()

        Dim datas As DataSet = publicCfg.GetListData(query)

        If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
            For Each row As DataRow In datas.Tables(0).Rows
                list.Add(New Dictionary(Of String, String) From {
                    {"value", row(valueField).ToString()},
                    {"text", row(textField).ToString()}
                })

                If field = "customer" Then
                    list(list.Count - 1).Add("company", row("Company").ToString())
                    list(list.Count - 1).Add("delivery", row("Delivery").ToString())
                End If
                
            Next
        End If

        Return list
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Save(ByVal data As ParamSave) As Object
        Try
            If String.IsNullOrEmpty(data.ordertype) Then
                Return New With { .warning = true, .message = "order type is required !", .field = "ordertype"}
            End If
            If String.IsNullOrEmpty(data.customer) Then
                Return New With { .warning = true, .message = "customer is required !", .field = "customer"}
            End If

            If Not String.IsNullOrEmpty(data.ordertype) AndAlso InArray(data.ordertype, "Panorama", "Evolve") Then
                If String.IsNullOrEmpty(data.createdby) Then
                    Return New With { .warning = true,  .message = "created by is required !", .field = "createdby"}
                End If
                If String.IsNullOrEmpty(data.createddate) Then
                    Return New With { .warning = true,  .message = "created date is required !", .field = "createddate"}
                End If
            End If

            If data.createtype = "Create New" Then
                Dim ValidatedCreate As Object = Validate(data.ordernumber, data.ordername, data)
                If ValidatedCreate.error Then 
                    Return New With { .warning = ValidatedCreate.error, .message = ValidatedCreate.message, .field = ValidatedCreate.field}
                End If
            End If

            Dim csvOrderProduct As String = ""
            Dim csvOrderNumber As String = ""
            Dim csvOrderName As String = ""
            Dim csvNotes As String = ""
            Dim csvFilePath As String = ""
            Dim listDetail As New List(Of Object)()
            If data.createtype = "Upload CSV" AndAlso data.action = "add" Then
                If String.IsNullOrWhiteSpace(data.csvContent) Then
                    Return New With { .warning = True, .message = "CSV file is required and cannot be empty!", .field = "files" }
                End If

                If Not String.IsNullOrWhiteSpace(data.csvContent) Then
                    
                    Dim rawLines As String() = data.csvContent.Split(New String() {vbCrLf, vbLf, vbCr}, StringSplitOptions.None)
                    Dim lines As New List(Of String)()
                    For Each l As String In rawLines
                        If Not String.IsNullOrWhiteSpace(l) Then
                            lines.Add(l)
                        End If
                    Next
    
                    If lines.Count < 4 Then
                        Return New With {.warning = True, .message = "CSV Format is not correct or incomplete!", .field = "files"}
                    End If
    
                    Dim delimiter As String = DetectDelimiter(lines(0))
                    Dim headerValues As String() = ParseCsvLine(lines(1), delimiter)
    
                    csvOrderProduct = GetColValue(headerValues, 0)
                    csvOrderNumber = GetColValue(headerValues, 1)
                    csvOrderName = GetColValue(headerValues, 2)
    
                    Dim ValidatedOrder As Object = Validate(csvOrderNumber, csvOrderName, data)
                    If ValidatedOrder.error Then 
                        Return New With { .warning = ValidatedOrder.error, .message = String.Format("CSV : {0}", ValidatedOrder.message), .field = ValidatedOrder.field}
                    End If
    
    
                    If Not String.IsNullOrEmpty(csvOrderNumber) Then data.ordernumber = csvOrderNumber
                    If Not String.IsNullOrEmpty(csvOrderName) Then data.ordername = csvOrderName
    
                    ' Throw New Exception(String.Format("Product: {0}, Order number: {1}, Order name: {2}", csvOrderProduct, csvOrderNumber, csvOrderName))

                    For i As Integer = 3 To lines.Count - 1
                        Dim detailCols As String() = ParseCsvLine(lines(i), delimiter)
                        If detailCols.Length > 0 AndAlso Not String.IsNullOrWhiteSpace(detailCols(0)) Then
                            Dim item as Object
                            Select Case csvOrderProduct
                                Case "Roller Blinds"
                                    Dim Res As Object = FindRollerDetails(detailCols)
                                    If Res.error Then Throw New Exception(Res.message)
                                    item = Res.item

                                Case "Venetian Blinds"
                                    Dim Res As Object = FindVenetianBlinds(detailCols)
                                    If Res.error Then Throw New Exception(Res.message)
                                    item = Res.item

                                Case "Veri Shades"
                                    Dim Res As Object = FindVeriShades(detailCols)
                                    If Res.error Then Throw New Exception(Res.message)
                                    item = Res.item

                                Case "Vertical Blinds"
                                    Dim Res As Object = FindVerticalBlinds(detailCols)
                                    If Res.error Then Throw New Exception(Res.message)
                                    item = Res.item

                                Case Else
                                    Throw New Exception("Data not found !")
                                    Continue For
                            End Select
    
                            listDetail.Add(item)
                        End If
                    Next

                    Try
                        Dim uploadDir As String = HttpContext.Current.Server.MapPath("~/File/Order/Upload/")

                        If Not Directory.Exists(uploadDir) Then
                            Directory.CreateDirectory(uploadDir)
                        End If

                        Dim ext As String = Path.GetExtension(data.fileName)
                        If String.IsNullOrEmpty(ext) Then ext = ".csv"

                        Dim safeFileName As String = String.Format("Upload_{0}_{1}{2}", data.ordernumber, DateTime.Now.ToString("yyyyMMdd_HHmmss"), ext)
                        
                        Dim fullPath As String = Path.Combine(uploadDir, safeFileName)

                        File.WriteAllText(fullPath, data.csvContent, Encoding.UTF8)
                        csvFilePath = "/File/Order/Upload/" & safeFileName

                    Catch ex As Exception
                        Throw New Exception("CSV upload error: " & ex.Message)
                    End Try

                End If

            End If

            ' Return New With {.debug = true, .message="Debug is running check your console", .list = listDetail}

            Dim HeaderData As String = publicCfg.GetItemData(String.Format("SELECT Id FROM OrderHeaders WHERE OrderNo='{0}' AND OrderCust='{1}' AND Active = 1", csvOrderNumber.Trim(), csvOrderName.Trim()))
            Dim FinalDelivery As String = data.delivery
            Dim CustomerDelivery As String = publicCfg.GetItemData(String.Format("SELECT Delivery FROM Customers WHERE Id = '{0}'", data.customer))
            If data.ordertype = "Blinds"Then

                If String.IsNullOrEmpty(data.delivery) AND String.IsNullOrEmpty(CustomerDelivery) AND String.IsNullOrEmpty(HeaderData) Then
                    Return New With { .warning = true, .message = "delivery is required !", .field = "delivery"}
                End If

                If data.action = "add" Then
                    If Not CustomerDelivery = "" Then
                        FinalDelivery = CustomerDelivery
                    End If
                End If

                If data.action = "edit" Then
                    If String.IsNullOrEmpty(data.delivery) Then
                        FinalDelivery = CustomerDelivery
                    Else
                        FinalDelivery = data.delivery
                    End If
                End If
            End If


            If data.ordertype = "Door and Window" Then
                FinalDelivery = ""
            End If
            If data.delivery = "Pick Up" Then
                data.deliverytype = ""
            End If
            
            Dim msg As String = "200"
            Dim url As String = "/"
            If data.action = "add" Then
                If InArray(data.ordertype, "Blinds", "Door and Window")  Then

                    data.headerid = publicCfg.CreateOrderHeaderId()
                    Using thisConn As New SqlConnection(myConn)
                        thisConn.Open()

                        Using trans As SqlTransaction = thisConn.BeginTransaction()
                            Try
                                If String.IsNullOrEmpty(HeaderData) Then
                                    Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, UserId, StoreId, OrderNo, OrderCust, Delivery, DeliveryType, OrderType, OrderSourceType, CreateType, Note, QuoteGST, QuoteDisc, QuoteInstall, QuoteMeasure, Status, CreatedDate, Active) VALUES (@Id, @UserId, @StoreId, LTRIM(RTRIM(@OrderNo)), LTRIM(RTRIM(@OrderCust)), @Delivery, @DeliveryType, @OrderType, @OrderSourceType, @CreateType, @Note, 'Yes', 0, 0, 0,  'Draft', GETDATE(), 1)", thisConn, trans)
                                        myCmd.Parameters.AddWithValue("@Id", data.headerid)
                                        myCmd.Parameters.AddWithValue("@UserId", UCase(data.loginid).ToString())
                                        myCmd.Parameters.AddWithValue("@StoreId", UCase(data.customer).ToString())
                                        myCmd.Parameters.AddWithValue("@OrderNo", data.ordernumber)
                                        myCmd.Parameters.AddWithValue("@OrderCust", data.ordername)
                                        myCmd.Parameters.AddWithValue("@Delivery", FinalDelivery)
                                        myCmd.Parameters.AddWithValue("@DeliveryType", data.deliverytype)
                                        myCmd.Parameters.AddWithValue("@OrderType", data.ordertype)
                                        myCmd.Parameters.AddWithValue("@OrderSourceType", "New")
                                        myCmd.Parameters.AddWithValue("@CreateType", data.createtype)
                                        myCmd.Parameters.AddWithValue("@Note", data.notes)

                                        myCmd.ExecuteNonQuery()
                                    End Using
                                End If
                                If Not String.IsNullOrEmpty(HeaderData) Then
                                    data.headerid = HeaderData
                                End If

                                Dim ResDetail As Object = Nothing
                                Select Case csvOrderProduct.Trim()
                                    Case "Roller Blinds"
                                        ResDetail = InsertRollerBlinds(data.headerid, listDetail, thisConn, trans)

                                    Case "Venetian Blinds"
                                        ResDetail = InsertVenetianBlinds(data.headerid, listDetail, thisConn, trans)

                                    Case "Veri Shades"
                                        ResDetail = InsertVeriShades(data.headerid, listDetail, thisConn, trans)
                                    
                                    Case "Vertical Blinds"
                                        ResDetail = InsertVerticalBlinds(data.headerid, data.customer, listDetail, thisConn, trans)

                                    Case Else
                                        Throw New Exception("Product not found")
                                End Select

                                If ResDetail IsNot Nothing AndAlso ResDetail.error Then
                                    Throw New Exception(ResDetail.message)
                                End If

                                trans.Commit()

                            Catch ex As Exception
                                If trans IsNot Nothing Then
                                    trans.Rollback()
                                End If
                                Throw
                            End Try
                        End Using


                        thisConn.Close()
                    End Using
                    url = String.Format("/order/orderdetails?param={0}&ordertype={1}", data.headerid, data.ordertype.ToLower())

                    Dim dataLog As Object() = {data.headerid, "", data.ordertype, data.loginid, "Create Order"}
                    orderCfg.Log_Orders(dataLog)

                End If

                If InArray(data.ordertype, "Panorama" ,"Evolve")  Then
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
                            myCmd.Parameters.AddWithValue("@OrderNote", data.Notes.Trim())
                            myCmd.Parameters.AddWithValue("@OrderType", data.ordertype)
                            myCmd.Parameters.AddWithValue("@CreatedBy", createdBy)
                            myCmd.Parameters.AddWithValue("@CreatedDate", data.createddate)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = String.Format("/order/shutters/detail?param={0}&ordertype={1}", headerId, data.ordertype.ToLower())

                    Dim dataLog As Object() = {headerId, "", data.ordertype, data.loginid, "Create Order"}
                    orderCfg.Log_Orders(dataLog)

                End If
                
                msg = "A new record has been created."
            End If

            If data.action = "edit" Then
                IF InArray(data.ordertype, "Blinds", "Door and Window") Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand("UPDATE OrderHeaders SET UserId=@UserId, StoreId=@StoreId, OrderNo=LTRIM(RTRIM(@OrderNo)), OrderCust=LTRIM(RTRIM(@OrderCust)), Delivery=@Delivery, DeliveryType=@DeliveryType, OrderSourceType=@OrderSourceType, Note=@Note, Active=1 WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", data.headerid)
                            myCmd.Parameters.AddWithValue("@UserId", UCase(data.createdby).ToString())
                            myCmd.Parameters.AddWithValue("@StoreId", UCase(data.customer).ToString())
                            myCmd.Parameters.AddWithValue("@OrderNo", data.ordernumber)
                            myCmd.Parameters.AddWithValue("@OrderCust", data.ordername)
                            myCmd.Parameters.AddWithValue("@Delivery", FinalDelivery)
                            myCmd.Parameters.AddWithValue("@DeliveryType", data.deliverytype)
                            myCmd.Parameters.AddWithValue("@OrderSourceType", data.sourcetype)
                            myCmd.Parameters.AddWithValue("@Note", data.notes)
                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = String.Format("/order/orderdetails?param={0}&ordertype={1}", data.headerid, data.ordertype.ToLower())
                End If

                If InArray(data.ordertype, "Panorama", "Evolve") Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET OrderId=@OrderId, JobId=@JobId, JobDate=@JobDate, ShipmentId=@ShipmentId, CustomerId=@CustomerId, CreatedBy=@CreatedBy, OrderNumber=@OrderNumber, OrderName=@OrderName, OrderNote=@OrderNote WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", data.headerid)
                            myCmd.Parameters.AddWithValue("@OrderId", data.orderid)
                            myCmd.Parameters.AddWithValue("@JobId", data.jobid)
                            myCmd.Parameters.AddWithValue("@JobDate", data.jobdate)
                            myCmd.Parameters.AddWithValue("@ShipmentId", If(String.IsNullOrEmpty(data.shipmentid), DBNull.Value, data.shipmentid))
                            myCmd.Parameters.AddWithValue("@CustomerId", UCase(data.customer).ToString())
                            myCmd.Parameters.AddWithValue("@CreatedBy", UCase(data.createdby).ToString())
                            myCmd.Parameters.AddWithValue("@CreatedDate", data.createddate)
                            myCmd.Parameters.AddWithValue("@OrderNumber", data.ordernumber.Trim())
                            myCmd.Parameters.AddWithValue("@OrderName", data.ordername.Trim())
                            myCmd.Parameters.AddWithValue("@OrderNote", data.notes.Trim())

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    url = String.Format("/order/shutters/detail?param={0}&ordertype={1}", data.headerid, data.ordertype.ToLower())

                End If

                Dim dataLog As Object() = {data.headerid, "", data.ordertype, data.loginid, "Edit Order"}
                orderCfg.Log_Orders(dataLog)
                msg = "Data has been updated successfully."
            End If

            If msg = "200" Then Throw New Exception(msg)

            Return New With {.success = true, .message = msg, .url = url}
        Catch ex As Exception
            Return New With { .error = true, .message = String.Format("Save: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SaveShipping(ByVal data As ParamSaveShipping) As Object
        Try
            Dim msg As String = "200"
            If String.IsNullOrEmpty(data.streetaddress) Then
                Return New With { .warning = true, .message = "street address is required !", .field = "streetaddress"}
            End If
            If String.IsNullOrEmpty(data.suburb) Then
                Return New With { .warning = true, .message = "suburb is required !", .field = "suburb"}
            End If
            If String.IsNullOrEmpty(data.states) Then
                Return New With { .warning = true, .message = "states is required !", .field = "states"}
            End If
            If String.IsNullOrEmpty(data.postcode) Then
                Return New With { .warning = true, .message = "postcode is required !", .field = "postcode"}
            End If
            If String.IsNullOrEmpty(data.addressport) Then
                Return New With { .warning = true, .message = "address port is required !", .field = "addressport"}
            End If
            
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

            Return New With {.success = true, .message = msg}
        Catch ex As Exception
            Return New With { .error = true, .message = String.Format("SaveShipping: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderAggregate(ByVal data As ParamBindOrderAggregate) As Object
        Try
            Dim HeaderData As Object = FindHeaderData(data)
            If HeaderData.error Then
                Throw New Exception(HeaderData.message)
            End If

            Dim Customers As Object  = BindListData(New ParamListData With {
                .field = "customer"
            })
            If Customers.error Then
                Throw New Exception(Customers.message)
            End If

            Dim Users As Object  = BindListData(New ParamListData With {
                .field = "createdby"
            })
            If Users.error Then
                Throw New Exception(Users.message)
            End If

            Dim Shipment As Object  = BindListData(New ParamListData With {
                .field = "shipmentid"
            })
            If Shipment.error Then
                Throw New Exception(Shipment.message)
            End If

            Dim CustomerId As String = HeaderData.List("CustomerId").ToString()
            Dim Shipping As Object  = FindShipping(CustomerId)
            If Shipping.error Then
                Throw New Exception(Shipping.message)
            End If

            Return New With {
                .HeaderData = HeaderData.list,
                .Customers = Customers.List,
                .Users = Users.List,
                .Shipment = Shipment.List,
                .Shipping = Shipping.list
            }
            
        Catch ex As Exception
            Return New With {.error = True, .message = String.Format("BindOrderAggregate: {0}", ex.Message)}
        End Try
    End Function

    '#--------------------------------------------|| Private Functions ||--------------------------------------------#

    Private Shared Function DetectDelimiter(ByVal headerLine As String) As String
        If String.IsNullOrEmpty(headerLine) Then Return ";"

        Dim countSemicolon As Integer = headerLine.Split(";"c).Length - 1
        Dim countComma As Integer = headerLine.Split(","c).Length - 1
        Dim countTab As Integer = headerLine.Split(vbTab).Length - 1

        If countSemicolon >= countComma AndAlso countSemicolon >= countTab AndAlso countSemicolon > 0 Then
            Return ";"
        ElseIf countComma > countSemicolon AndAlso countComma >= countTab AndAlso countComma > 0 Then
            Return ","
        ElseIf countTab > 0 Then
            Return vbTab
        End If

        Return ";"
    End Function

    Private Shared Function ParseCsvLine(ByVal line As String, ByVal delimiter As String) As String()
        If String.IsNullOrEmpty(line) Then Return New String() {}
        Return line.Split(New String() {delimiter}, StringSplitOptions.None)
    End Function

    Private Shared Function GetColValue(ByVal cols As String(), ByVal index As Integer) As String
        If index < cols.Length Then
            Return cols(index).Trim()
        End If
        Return ""
    End Function

    Private Shared Function Validate(ByVal number As String, ByVal ordername As String, ByVal data As ParamSave) As Object
        Try
            If String.IsNullOrEmpty(number) Then
                Return New With { .error = true, .message = "order number is required !", .field = "ordernumber"}
            End If

            If Not String.IsNullOrEmpty(number)Then
                If InStr(number, "\") > 0 Or  InStr(number, "/") > 0 Or  InStr(number, "|") > 0 Or  InStr(number, ",") > 0 Or  InStr(number, "&") > 0 Or  InStr(number, "#") > 0 Or  InStr(number, "'") > 0 Or  InStr(number, ".") > 0 Or  InStr(number, "`") > 0 Then
                    Return New With { .error = true, .message = "order number do not use special character !", .field = "ordernumber"}
                End If

                If number.Length > 20 Then
                    Return New With { .error = true, .message = "order number max length is 20 !", .field = "ordernumber"}
                End If

                If data.action = "add" Then
                    '#cek by product type on customer same
                    Dim OrderNumber As String = number.Trim()
                    Dim FindOrderNumberByCust As String = publicCfg.GetItemData(String.Format("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '{0}' AND CustomerId = '{1}' AND OrderType='{2}' AND Active = '1'", OrderNumber, data.customer, data.ordertype))
                    Dim CustomerId As String = publicCfg.GetItemData(String.Format("SELECT CustomerId FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '{0}' AND OrderType='{2}' AND Active = '1'", OrderNumber, data.customer, data.ordertype))
                    If String.Equals(OrderNumber.Trim(), FindOrderNumberByCust, StringComparison.OrdinalIgnoreCase) Then
                        If data.customer <> CustomerId AND data.createtype = "Upload CSV" Then
                            Return New With { .error = true, .message = "The order number in the CSV is already in use or available. The customer does not match.", .field = "ordernumber"}
                        End If

                        If data.createtype = "Create New" Then
                            Return New With { .error = true, .message = "order number already exist !", .field = "ordernumber"}
                        End If
                    End If

                     '# check by all customer
                    Dim FindOrderNumberAllCust As String = publicCfg.GetItemData(String.Format("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '{0}' AND CustomerId <> '{1}' AND Active = 1", OrderNumber, data.customer))
                    If String.Equals(OrderNumber.Trim(), FindOrderNumberAllCust, StringComparison.OrdinalIgnoreCase) Then
                        
                        If data.customer <> CustomerId AND data.createtype = "Upload CSV" Then
                            Return New With { .error = true, .message = "The order number in the CSV is already in use or available. The customer does not match.", .field = "ordernumber"}
                        End If

                        If data.createtype = "Create New" Then
                            Return New With { .error = true, .message = "order number already exist !", .field = "ordernumber"}
                        End If

                    End If
                End If

                If data.action = "edit" Then
                    Dim OrderNumber As String = ordernumber.Trim()
                    Dim FindOrderNumberAllCust As String = publicCfg.GetItemData(String.Format("SELECT LTRIM(RTRIM(OrderNumber)) FROM view_order_headers WHERE LTRIM(RTRIM(OrderNumber)) = '{0}' AND CustomerId <> '{1}' AND Active = 1", OrderNumber, data.customer))
                    Dim OtherCustomerId As String = publicCfg.GetItemData(String.Format("SELECT CustomerId FROM view_order_headers WHERE OrderNumber = '{0}'", FindOrderNumberAllCust))

                    Dim OrderNumberSame As boolean = String.Equals(OrderNumber.Trim(), FindOrderNumberAllCust, StringComparison.OrdinalIgnoreCase)
                    Dim CustomerOldSame As boolean = String.Equals(data.customerold, OtherCustomerId, StringComparison.OrdinalIgnoreCase)
                    If OrderNumberSame And CustomerOldSame = False Then
                        Return New With { .error = true, .message = "order number already exist !", .field = "ordernumber"}
                    End If
                End If

            End If

            If String.IsNullOrEmpty(ordername) Then
                Return New With { .warning = true, .message = "order name is required !", .field = "ordername"}
            End If
            If Not String.IsNullOrEmpty(ordername) Then
                If InStr(ordername, "\") > 0 Or  InStr(ordername, "/") > 0 Or  InStr(ordername, "|") > 0 Or  InStr(ordername, ",") > 0 Or  InStr(ordername, "&") > 0 Or  InStr(ordername, "#") > 0 Or  InStr(ordername, "'") > 0 Or  InStr(ordername, ".") > 0 Or  InStr(ordername, "`") > 0 Then
                    Return New With { .warning = true, .message = "order number do not use special character !", .field = "ordername"}
                End If
            End If

            Return New With {.error = False, .message = String.Empty, .field = String.Empty}
        Catch ex As Exception
            Return New With { .error = True, .message = String.Format("ValidateOrder: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindRollerDetails(ByVal detailCols As String()) As Object
        Try
            Dim item = New With {
                .BlindNo = GetColValue(detailCols, 0),
                .ConnectedId = GetColValue(detailCols, 1),
                .BlindType = GetColValue(detailCols, 2),
                .BracketType = GetColValue(detailCols, 3),
                .TubeType = GetColValue(detailCols, 4),
                .ControlType = GetColValue(detailCols, 5),
                .ColourType = GetColValue(detailCols, 6),
                .Qty = GetColValue(detailCols, 7),
                .Room = GetColValue(detailCols, 8),
                .Mounting = GetColValue(detailCols, 9),
                .Width = GetColValue(detailCols, 10),
                .Drop = GetColValue(detailCols, 11),
                .FabricType = GetColValue(detailCols, 12),
                .FabricColour = GetColValue(detailCols, 13),
                .MotorStyle = GetColValue(detailCols, 14),
                .MotorRemote = GetColValue(detailCols, 15),
                .ExternalBattery = GetColValue(detailCols, 16),
                .MotorCharger = GetColValue(detailCols, 17),
                .CableExitPoint = GetColValue(detailCols, 18),
                .FlushConnect = GetColValue(detailCols, 19),
                .RollDirection = GetColValue(detailCols, 20),
                .ControlPosition = GetColValue(detailCols, 21),
                .ChainColour = GetColValue(detailCols, 22),
                .ChainLength = GetColValue(detailCols, 23),
                .Trim = GetColValue(detailCols, 24),
                .BottomType = GetColValue(detailCols, 25),
                .BottomColour = GetColValue(detailCols, 26),
                .TubeSize = GetColValue(detailCols, 27),
                .ChildSafe = GetColValue(detailCols, 28),
                .Accessory = GetColValue(detailCols, 29),
                .Extras = GetColValue(detailCols, 30),
                .BracketCover = GetColValue(detailCols, 31),
                .BracketCoverColour = GetColValue(detailCols, 32),
                .BracketExtension = GetColValue(detailCols, 33)
            }

            Return New With {.error = False, .item = item}
        Catch ex As Exception
            Return New With { .error = True, .message = String.Format("FindRollerDetails: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindVenetianBlinds(ByVal detailCols As String()) As Object
        Try
            Dim item = New With {
                .BlindType = GetColValue(detailCols, 0),
                .ControlType = GetColValue(detailCols, 1),
                .ColourType = GetColValue(detailCols, 2),
                .Qty = GetColValue(detailCols, 3),
                .Room = GetColValue(detailCols, 4),
                .Mounting = GetColValue(detailCols, 5),
                .Width = GetColValue(detailCols, 6),
                .Drop = GetColValue(detailCols, 7),
                .ControlPosition = GetColValue(detailCols, 8),
                .ControlLength = GetColValue(detailCols, 9),
                .ControlLift = GetColValue(detailCols, 10),
                .ControlTilt = GetColValue(detailCols, 11),
                .Bracket = GetColValue(detailCols, 12),
                .BottomHoldDown = GetColValue(detailCols, 13),
                .TwoOneHeadrail = GetColValue(detailCols, 14),
                .PelmetType = GetColValue(detailCols, 15),
                .PelmetSize = GetColValue(detailCols, 16),
                .PelmetWidth = GetColValue(detailCols, 17),
                .ReturnLengthLeft = GetColValue(detailCols, 18),
                .ReturnLengthRight = GetColValue(detailCols, 19),
                .LhsWidthTop = GetColValue(detailCols, 20),
                .LhsHeightTop = GetColValue(detailCols, 21),
                .RhsWidthTop = GetColValue(detailCols, 22),
                .RhsHeightTop = GetColValue(detailCols, 23),
                .LhsWidthBot = GetColValue(detailCols, 24),
                .LhsHeightBot = GetColValue(detailCols, 25),
                .RhsWidthBot = GetColValue(detailCols, 26),
                .RhsHeightBot = GetColValue(detailCols, 27)
            }

            Return New With {.error = False, .item = item}
        Catch ex As Exception
            Return New With { .error = True, .message = String.Format("FindRollerDetails: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindVeriShades(ByVal detailCols As String()) As Object
        Try
            Dim item = New With {
                .BlindType = GetColValue(detailCols, 0),
                .Qty = GetColValue(detailCols, 1),
                .Room = GetColValue(detailCols, 2),
                .Mounting = GetColValue(detailCols, 3),
                .Width = GetColValue(detailCols, 4),
                .Drop = GetColValue(detailCols, 5),
                .FabricType = GetColValue(detailCols, 6),
                .FabricColour = GetColValue(detailCols, 7),
                .BlindSize = GetColValue(detailCols, 8),
                .StackConfigure = GetColValue(detailCols, 9),
                .TrackType = GetColValue(detailCols, 10),
                .TrackColour = GetColValue(detailCols, 11),
                .WandSize = GetColValue(detailCols, 12),
                .WandColour = GetColValue(detailCols, 13),
                .Bracket = GetColValue(detailCols, 14),
                .TapeColour = GetColValue(detailCols, 15)
            }

            Return New With {.error = False, .item = item}
        Catch ex As Exception
            Return New With { .error = True, .message = String.Format("FindVeriShades: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindVerticalBlinds(ByVal detailCols As String()) As Object
        Try
            Dim item = New With {
                .BlindType = GetColValue(detailCols, 0),
                .TubeType = GetColValue(detailCols, 1),
                .ControlType = GetColValue(detailCols, 2),
                .Qty = GetColValue(detailCols, 3),
                .Room = GetColValue(detailCols, 4),
                .Mounting = GetColValue(detailCols, 5),
                .Width = GetColValue(detailCols, 6),
                .Drop = GetColValue(detailCols, 7),
                .TrackType = GetColValue(detailCols, 8),
                .SlatQty = GetColValue(detailCols, 9),
                .FabricType = GetColValue(detailCols, 10),
                .FabricSlat = GetColValue(detailCols, 11),
                .FabricColour = GetColValue(detailCols, 12),
                .SlatSize = GetColValue(detailCols, 13),
                .TrackColour = GetColValue(detailCols, 14),
                .StackPosition = GetColValue(detailCols, 15),
                .ControlPosition = GetColValue(detailCols, 16),
                .WandLength = GetColValue(detailCols, 17),
                .WandColour = GetColValue(detailCols, 18),
                .ChainColour = GetColValue(detailCols, 19),
                .ChainLength = GetColValue(detailCols, 20),
                .BracketType = GetColValue(detailCols, 21),
                .BracketColour = GetColValue(detailCols, 22),
                .HangerType = GetColValue(detailCols, 23),
                .Bottom = GetColValue(detailCols, 24),
                .InsertInTrack = GetColValue(detailCols, 25),
                .Sloper = GetColValue(detailCols, 26)
            }

            Return New With {.error = False, .item = item}
        Catch ex As Exception
            Return New With { .error = True, .message = String.Format("FindVerticalBlinds: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function InsertRollerBlinds(headerId As String, listDetail As List(Of Object), conn As SqlConnection, trans As SqlTransaction) As Object
        Try
            If listDetail IsNot Nothing Then
                listDetail.RemoveAll(Function(x) x Is Nothing OrElse IsDBNull(x) OrElse x.ToString() = "null")
            End If
            If listDetail Is Nothing OrElse listDetail.Count = 0 Then
                Throw New Exception("Details is empty !")
            End If

            Dim GetProp = Function(obj As Object, propName As String) As String
                Try
                    Dim val = CallByName(obj, propName, CallType.Get)
                    Return If(val IsNot Nothing, val.ToString(), "")
                Catch ex As Exception
                    Return "" 
                End Try
            End Function

            Dim GetDbVal = Function(strVal As String) As Object
                Return If(String.IsNullOrEmpty(strVal), DBNull.Value, CObj(strVal))
            End Function


            For Each item In listDetail
                Dim ItemId As String = CreateOrderItemId(conn, trans)
                Dim UniqueId As String = Guid.NewGuid().ToString("N") 
                Dim DesignId As String = GetItemData("SELECT Id FROM Designs WHERE Name = 'Roller Blinds' AND Active = 1", conn, trans)

                Dim BlindType As String = GetProp(item, "BlindType")
                Dim BracketType As String = GetProp(item, "BracketType")
                Dim TubeType As String = GetProp(item, "TubeType")
                If String.IsNullOrEmpty(TubeType) Then TubeType = "N/A"
                Dim ControlType As String = GetProp(item, "ControlType")
                If String.IsNullOrEmpty(ControlType) Then ControlType = "N/A"
                Dim ColourType As String = GetProp(item, "ColourType")
                If String.IsNullOrEmpty(ColourType) Then ColourType = "N/A"


                Dim qBlind As String = String.Format("SELECT Id FROM Blinds WHERE DesignId = '{0}' AND Name = '{1}' AND Active = 1", DesignId, BlindType)
                Dim BlindId As String = GetItemData(qBlind, conn, trans)

                Dim qKit As String = String.Format("SELECT Id FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND BracketType = '{2}' AND TubeType = '{3}' AND ControlType = '{4}' AND ColourType = '{5}' AND Active = 1", DesignId, BlindId, BracketType, TubeType, ControlType, ColourType)
                Dim KitId As String = GetItemData(qKit, conn, trans)

                Dim qSoe As String = String.Format("SELECT SoeId FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND BracketType = '{2}' AND TubeType = '{3}' AND ControlType = '{4}' AND ColourType = '{5}' AND Active = 1", DesignId, BlindId, BracketType, TubeType, ControlType, ColourType)
                Dim SoeKitId As String = GetItemData(qSoe, conn, trans)

                If String.IsNullOrEmpty(KitId) Then
                    Throw New Exception("Kit Id not found")
                End If
                

                Dim ExactName As String = String.Format("Roller Blinds - {0}", BlindType)
                Dim ExactId As String = GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName), conn, trans)
                Dim FabricType As String = GetProp(item, "FabricType")
                Dim FabricColour As String = GetProp(item, "FabricColour")
                Dim FabricId As String = GetItemData(String.Format("SELECT Id FROM Fabrics WHERE DesignId='{0}' AND LTRIM(RTRIM(Type)) = '{1}' AND LTRIM(RTRIM(Colour)) ='{2}' AND Active =1", DesignId, FabricType, FabricColour), conn, trans)
                If String.IsNullOrEmpty(FabricId) Then
                    Throw New Exception("Fabric Id not found")
                End If

                

                Dim ChainColour As String = GetProp(item, "ChainColour")
                Dim ChainLength As String = GetProp(item, "ChainLength")
                Dim ChainName As String = String.Format("{0} Chain + Joiner ({1})", ChainLength, ChainColour)
                Dim ChainId As String = GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName), conn, trans)
                
                If String.IsNullOrEmpty(ChainId) Then
                    ChainName = String.Format("Custom Chain + Joiner ({0})", ChainColour)
                    ChainId = GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName), conn, trans)
                End If
                ' If String.IsNullOrEmpty(ChainId) Then
                '     Throw New Exception("Chain Id not found")
                ' End if


                Dim BottomType As String = GetProp(item, "BottomType")
                Dim BottomColour As String = GetProp(item, "BottomColour")
                Dim Trim As String = GetProp(item, "Trim")
                Dim BottomRailId As String = GetItemData(String.Format("SELECT Id FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '{0}' AND Type='{1}' AND Company = 'SP' AND Colour ='{2}' AND Trim ='{3}' AND Active ='1'", BracketType, BottomType, BottomColour, Trim), conn, trans)

                

                Dim FabricGroup As String = GetItemData(String.Format("SELECT [Group] FROM Fabrics WHERE Id = '{0}'",FabricId), conn, trans)
                Dim PriceGroupName As String = String.Format("Roller Blind - {0}", FabricGroup)
                If BlindType = "Skin Only" Then
                    PriceGroupName = String.Format("Roller Skin Only - {0}", FabricGroup)
                End If
                Dim PriceGroupId As String = GetItemData(String.Format("SELECT Id FROM PricesGroup WHERE DesignId = '{0}' AND Name = '{1}'", DesignId, PriceGroupName), conn, trans)
                If String.IsNullOrEmpty(PriceGroupId) Then
                    Throw New Exception("price group id not found !")
                End If

                

                Dim CassetteExtraId As String = ""
                Dim Mounting As String = GetProp(item, "Mounting")
                If BlindType = "Cassette" Then
                    Dim CassetteExtraName As String = String.Format("{0} - {1}", BracketType, Mounting)
                    If BracketType = "Headbox Only" Then
                        CassetteExtraName =BracketType
                    End If
                    CassetteExtraId = GetItemData(String.Format("SELECT Id FROM PricesGroup WHERE DesignId = '{0}' AND Name = '{1}'", DesignId, CassetteExtraName), conn, trans)

                    If String.IsNullOrEmpty(CassetteExtraId) Then
                        Throw New Exception("Cassette Extra Id not found")
                    End If
                End If

                ' Throw New Exception("Debug Time Out")
                

                Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, UniqueId, BlindNo, KitId, SoeKitId, ExactId, FabricId, ChainId, BottomRailId, PriceGroupId, CassetteExtraId, Qty, Location, LouvreSize, LouvrePosition, Mounting, Width, [Drop], RollDirection, ControlPosition, ChainLength, Accessory, TubeSize, Trim, BracketCover, BracketColour, BracketExtension, ChildSafe, MotorStyle, MotorRemote, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @UniqueId, @BlindNo, @KitId, @SoeKitId, @ExactId, @FabricId, @ChainId, @BottomRailId, @PriceGroupId, @CassetteExtraId, @Qty, @Location, @LouvreSize, @LouvrePosition, @Mounting, @Width, @Drop, @RollDirection, @ControlPosition, @ChainLength, @Accessory, @TubeSize, @Trim, @BracketCover, @BracketColour, @BracketExtension, @ChildSafe, @MotorStyle, @MotorRemote, @MotorBattery, @MotorCharger, @Connector, @AdditionalMotor, @CableExitPoint, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", conn, trans)
                    myCmd.Parameters.AddWithValue("@Id", ItemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", UCase(headerId).ToString())
                    myCmd.Parameters.AddWithValue("@UniqueId", GetDbVal(GetProp(item, "ConnectedId")))
                    myCmd.Parameters.AddWithValue("@BlindNo", GetDbVal(GetProp(item, "BlindNo")))
                    myCmd.Parameters.AddWithValue("@KitId", GetDbVal(KitId))
                    myCmd.Parameters.AddWithValue("@SoeKitId", GetDbVal(SoeKitId))
                    myCmd.Parameters.AddWithValue("@ExactId", GetDbVal(ExactId))
                    myCmd.Parameters.AddWithValue("@FabricId", GetDbVal(FabricId))
                    myCmd.Parameters.AddWithValue("@ChainId", GetDbVal(ChainId))
                    myCmd.Parameters.AddWithValue("@BottomRailId", GetDbVal(BottomRailId))
                    myCmd.Parameters.AddWithValue("@PriceGroupId", GetDbVal(PriceGroupId))
                    myCmd.Parameters.AddWithValue("@CassetteExtraId", GetDbVal(CassetteExtraId))
                    myCmd.Parameters.AddWithValue("@Qty", GetDbVal(GetProp(item, "Qty")))
                    myCmd.Parameters.AddWithValue("@Location", GetDbVal(GetProp(item, "Room")))
                    myCmd.Parameters.AddWithValue("@LouvreSize", "")
                    myCmd.Parameters.AddWithValue("@LouvrePosition", "")
                    myCmd.Parameters.AddWithValue("@Mounting", GetDbVal(Mounting))
                    myCmd.Parameters.AddWithValue("@Width", GetDbVal(GetProp(item, "Width")))
                    myCmd.Parameters.AddWithValue("@Drop", GetDbVal(GetProp(item, "Drop")))
                    myCmd.Parameters.AddWithValue("@RollDirection", GetDbVal(GetProp(item, "RollDirection")))
                    myCmd.Parameters.AddWithValue("@ControlPosition", GetDbVal(GetProp(item, "ControlPosition")))
                    myCmd.Parameters.AddWithValue("@ChainLength", GetDbVal(ChainLength))
                    myCmd.Parameters.AddWithValue("@Accessory", GetDbVal(GetProp(item, "Accessory")))
                    myCmd.Parameters.AddWithValue("@TubeSize", GetDbVal(GetProp(item, "TubeSize")))
                    myCmd.Parameters.AddWithValue("@Trim", GetDbVal(GetProp(item, "Trim")))
                    myCmd.Parameters.AddWithValue("@BracketCover", GetDbVal(GetProp(item, "BracketCover")))
                    myCmd.Parameters.AddWithValue("@BracketColour", GetDbVal(GetProp(item, "BracketCoverColour")))
                    myCmd.Parameters.AddWithValue("@BracketExtension", GetDbVal(GetProp(item, "BracketExtension")))
                    myCmd.Parameters.AddWithValue("@ChildSafe", GetDbVal(GetProp(item, "ChildSafe")))
                    myCmd.Parameters.AddWithValue("@MotorStyle", GetDbVal(GetProp(item, "MotorStyle")))
                    myCmd.Parameters.AddWithValue("@MotorRemote", GetDbVal(GetProp(item, "MotorRemote")))
                    myCmd.Parameters.AddWithValue("@MotorBattery", GetDbVal(GetProp(item, "ExternalBattery")))
                    myCmd.Parameters.AddWithValue("@MotorCharger", GetDbVal(GetProp(item, "MotorCharger")))
                    myCmd.Parameters.AddWithValue("@Connector", GetDbVal(GetProp(item, "FlushConnect")))
                    myCmd.Parameters.AddWithValue("@AdditionalMotor", GetDbVal(GetProp(item, "Extras")))
                    myCmd.Parameters.AddWithValue("@CableExitPoint", GetDbVal(GetProp(item, "CableExitPoint")))
                    myCmd.Parameters.AddWithValue("@Notes", "")
                    myCmd.Parameters.AddWithValue("@MarkUp", 0)
                    myCmd.ExecuteNonQuery()
                End Using

                
            Next
            
            ' Throw New Exception("Debug Time Out")


           
            Return New With {.error = False, .message = "success"}
        Catch ex As Exception
            Return New With { .error = True, .message = String.Format("InsertRollerBlinds: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function InsertVenetianBlinds(headerId As String, listDetail As List(Of Object), conn As SqlConnection, trans As SqlTransaction) As Object
        Try
            If listDetail IsNot Nothing Then
                listDetail.RemoveAll(Function(x) x Is Nothing OrElse IsDBNull(x) OrElse x.ToString() = "null")
            End If
            If listDetail Is Nothing OrElse listDetail.Count = 0 Then
                Throw New Exception("Details is empty !")
            End If

            Dim GetProp = Function(obj As Object, propName As String) As String
                Try
                    Dim val = CallByName(obj, propName, CallType.Get)
                    Return If(val IsNot Nothing, val.ToString(), "")
                Catch ex As Exception
                    Return "" 
                End Try
            End Function

            Dim GetDbVal = Function(strVal As String) As Object
                Return If(String.IsNullOrEmpty(strVal), DBNull.Value, CObj(strVal))
            End Function

            Dim GetInt = Function(strVal As String) As Object
                Return If(String.IsNullOrEmpty(strVal), 0, CObj(strVal))
            End Function


            For Each item In listDetail
                Dim ItemId As String = CreateOrderItemId(conn, trans)
                Dim DesignId As String = GetItemData("SELECT Id FROM Designs WHERE Name = 'Venetian Blinds' AND Active = 1", conn, trans)

                Dim BlindType As String = GetProp(item, "BlindType")
                Dim ControlType As String = GetProp(item, "ControlType")
                If String.IsNullOrEmpty(ControlType) Then ControlType = "N/A"
                Dim ColourType As String = GetProp(item, "ColourType")
                Dim Mounting As String = GetProp(item, "Mounting")
                Dim Width As Integer = GetInt(GetProp(item, "Width"))
                Dim Drop As Integer = GetInt(GetProp(item, "Drop"))
                Dim ControlPosition As String = GetProp(item, "ControlPosition")
                Dim ControlLift As String = GetProp(item, "ControlLift")
                Dim ControlTilt As String = GetProp(item, "ControlTilt")
                Dim PelmetType As String = GetProp(item, "PelmetType")
                Dim PelmetWidth As String = GetProp(item, "PelmetWidth")
                Dim ReturnLengthLeft As String = GetProp(item, "ReturnLengthLeft")
                Dim ReturnLengthRight As String = GetProp(item, "ReturnLengthRight")
                Dim CutOut_LeftTop As Integer = 0
                Dim CutOut_RightTop As Integer = 0
                Dim CutOut_LeftBottom As Integer = 0
                Dim CutOut_RightBottom As Integer = 0
                Dim LhsWidthTop As Integer = GetInt(GetProp(item, "LhsWidthTop"))
                Dim LhsHeightTop As Integer = GetInt(GetProp(item, "LhsHeightTop"))
                Dim RhsWidthTop As Integer = GetInt(GetProp(item, "RhsWidthTop"))
                Dim RhsHeightTop As Integer = GetInt(GetProp(item, "RhsHeightTop"))
                Dim LhsWidthBot As Integer = GetInt(GetProp(item, "LhsWidthBot"))
                Dim LhsHeightBot As Integer = GetInt(GetProp(item, "LhsHeightBot"))
                Dim RhsWidthBot As Integer = GetInt(GetProp(item, "RhsWidthBot"))
                Dim RhsHeightBot As Integer = GetInt(GetProp(item, "RhsHeightBot"))

                
                Dim qBlind As String = String.Format("SELECT Id FROM Blinds WHERE DesignId = '{0}' AND Name = '{1}' AND Active = 1", DesignId, BlindType)
                Dim BlindId As String = GetItemData(qBlind, conn, trans)   
        
                If String.IsNullOrEmpty(BlindId) Then
                    Throw New Exception("Blind Id not found")
                End If

                Dim qKit As String = String.Format("SELECT Id FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND ControlType = '{2}' AND ColourType = '{3}' AND Active = 1", DesignId, BlindId, ControlType, ColourType)
                Dim KitId As String = GetItemData(qKit, conn, trans)

                Dim qSoe As String = String.Format("SELECT SoeId FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND ControlType = '{2}' AND ColourType = '{3}' AND Active = 1", DesignId, BlindId, ControlType, ColourType)
                Dim SoeKitId As String = GetItemData(qSoe, conn, trans)

                If String.IsNullOrEmpty(KitId) Then
                    Throw New Exception("Kit Id not found")
                End If
                
                

                Dim ExactName As String = String.Format("Venetian Blinds - {0}", BlindType)
                Dim ExactId As String = GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName), conn, trans)

                Dim PriceGroupName As String = BlindType
                Dim PriceGroupId As String = GetItemData(String.Format("SELECT Id FROM PricesGroup WHERE DesignId = '{0}' AND Name = '{1}'", DesignId, PriceGroupName), conn, trans)
                If PriceGroupId = "" Then
                    Throw New Exception("price group not found!")
                End If

                
                If InStr(BlindType, "Mockwood") > 0 Or InStr(BlindType, "Wooden") > 0 Then
                    ControlPosition = String.Format("{0}|{1}", ControlLift, ControlTilt)
                End If

                If LhsWidthTop > 0 Or LhsHeightTop > 0 Then
                    CutOut_LeftTop = 1
                End If
                If RhsWidthTop > 0 Or RhsHeightTop > 0 Then
                    CutOut_RightTop = 1
                End If

                If LhsWidthBot > 0 Or LhsHeightBot > 0 Then
                    CutOut_LeftBottom = 1
                End If
                If RhsWidthBot > 0 Or RhsHeightBot > 0 Then
                    CutOut_RightBottom = 1
                End If


                If String.IsNullOrEmpty(PelmetWidth) OR PelmetWidth = "0" Then 
                    If Mounting = "Face Fit" Then
                        PelmetWidth = Width + 20
                        If InStr(BlindType, "Timber") > 0 Then
                            PelmetWidth = Width + 20
                        End If
                    End If
                    If Mounting = "Reveal Fit" Then
                         PelmetWidth = Width + 10
                        If InStr(BlindType, "Timber") > 0 Then
                            PelmetWidth = Width + 8
                        End If
                    End If
                End If

                If PelmetType = "With Return" Then
                    ' ReturnLengthLeft = "67" : ReturnLengthRight = "67"
                    If Mounting = "Face Fit" Then
                        ReturnLengthLeft = 100 : ReturnLengthRight = 100
                        If InStr(BlindType, "Wooden") > 0 Then
                            ReturnLengthLeft = 70 : ReturnLengthRight = 70
                        End If
                        If InStr(BlindType, "Mockwood") > 0 Then
                            ReturnLengthLeft = 77 : ReturnLengthRight = 77
                        End If
                        If InStr(BlindType, "Timber") > 0 Then
                            ReturnLengthLeft = 67 : ReturnLengthRight = 67
                        End If
                    End If
                    If Mounting = "Reveal Fit" Then
                        ReturnLengthLeft = 0 : ReturnLengthRight = 0
                    End If
                End If

                If PelmetType = "Single Left Return" Then
                    If Mounting = "Face Fit" Then
                        ReturnLengthLeft = 77 : ReturnLengthRight = 0
                    End If
                    If Mounting = "Reveal Fit" Then
                        ReturnLengthLeft = 0 : ReturnLengthRight = 0
                    End If
                End If

                If PelmetType = "Single Right Return" Then
                    If Mounting = "Face Fit" Then
                        ReturnLengthLeft = 0 : ReturnLengthRight = 77
                    End If    
                    If Mounting = "Reveal Fit" Then
                        ReturnLengthLeft = 0 : ReturnLengthRight = 0
                    End If
                End If

                
            
                Dim Field As String = "Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, PriceGroupId, Qty, Location, LouvreSize, LouvrePosition, Mounting, Width, [Drop], ControlPosition, ControlLength, BracketOption, BottomHoldDown, BracketColour, DoorCutOut, PelmetType, PelmetWidth, PelmetSize, PelmetReturnSize, PelmetReturnSize2, CutOut_LeftTop, CutOut_RightTop, CutOut_LeftBottom, CutOut_RightBottom, LHSWidth_Top, LHSHeight_Top, RHSWidth_Top, RHSHeight_Top, LHSWidth_Bottom, LHSHeight_Bottom, RHSWidth_Bottom, RHSHeight_Bottom,  Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active"

                Dim Values As String = "@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @PriceGroupId, @Qty, @Location, @LouvreSize, @LouvrePosition, @Mounting, @Width, @Drop, @ControlPosition, @ControlLength, @BracketOption, @BottomHoldDown, @BracketColour, @DoorCutOut, @PelmetType, @PelmetWidth, @PelmetSize, @PelmetReturnSize, @PelmetReturnSize2, @CutOut_LeftTop, @CutOut_RightTop, @CutOut_LeftBottom, @CutOut_RightBottom, @LHSWidth_Top, @LHSHeight_Top, @RHSWidth_Top, @RHSHeight_Top, @LHSWidth_Bottom, @LHSHeight_Bottom, @RHSWidth_Bottom, @RHSHeight_Bottom, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1"
                Using myCmd As New SqlCommand(String.Format("INSERT INTO OrderDetails({0}) VALUES ({1})", Field, Values), conn, trans)
                    myCmd.Parameters.AddWithValue("@Id", ItemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                    myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                    myCmd.Parameters.AddWithValue("@KitId", GetDbVal(KitId) )
                    myCmd.Parameters.AddWithValue("@SoeKitId", GetDbVal(SoeKitId))
                    myCmd.Parameters.AddWithValue("@ExactId", GetDbVal(ExactId))
                    myCmd.Parameters.AddWithValue("@PriceGroupId", GetDbVal(PriceGroupId))
                    myCmd.Parameters.AddWithValue("@Qty", GetDbVal(GetProp(item, "Qty")))
                    myCmd.Parameters.AddWithValue("@Location", GetDbVal(GetProp(item, "Room")))
                    myCmd.Parameters.AddWithValue("@LouvreSize", "")
                    myCmd.Parameters.AddWithValue("@LouvrePosition", "")
                    myCmd.Parameters.AddWithValue("@Mounting", Mounting)
                    myCmd.Parameters.AddWithValue("@Width", GetDbVal(GetProp(item, "Width")))
                    myCmd.Parameters.AddWithValue("@Drop", GetDbVal(GetProp(item, "Drop")))
                    myCmd.Parameters.AddWithValue("@ControlPosition", ControlPosition)
                    myCmd.Parameters.AddWithValue("@ControlLength", GetDbVal(GetProp(item, "ControlLength")))
                    myCmd.Parameters.AddWithValue("@BracketOption", GetDbVal(GetProp(item, "Bracket")))
                    myCmd.Parameters.AddWithValue("@BottomHoldDown", GetDbVal(GetProp(item, "BottomHoldDown")))
                    myCmd.Parameters.AddWithValue("@BracketColour", "")
                    myCmd.Parameters.AddWithValue("@DoorCutOut", GetDbVal(GetProp(item, "TwoOneHeadrail")))
                    myCmd.Parameters.AddWithValue("@PelmetType", PelmetType)
                    myCmd.Parameters.AddWithValue("@PelmetWidth", PelmetWidth)
                    myCmd.Parameters.AddWithValue("@PelmetSize", GetDbVal(GetProp(item, "PelmetSize")))
                    myCmd.Parameters.AddWithValue("@PelmetReturnSize", ReturnLengthLeft)
                    myCmd.Parameters.AddWithValue("@PelmetReturnSize2", ReturnLengthRight)
                    myCmd.Parameters.AddWithValue("@CutOut_LeftTop", CutOut_LeftTop)
                    myCmd.Parameters.AddWithValue("@CutOut_RightTop", CutOut_RightTop)
                    myCmd.Parameters.AddWithValue("@CutOut_LeftBottom", CutOut_LeftBottom)
                    myCmd.Parameters.AddWithValue("@CutOut_RightBottom", CutOut_RightBottom)
                    myCmd.Parameters.AddWithValue("@LHSWidth_Top", LhsWidthTop)
                    myCmd.Parameters.AddWithValue("@LHSHeight_Top", LhsHeightTop)
                    myCmd.Parameters.AddWithValue("@RHSWidth_Top", RhsWidthTop)
                    myCmd.Parameters.AddWithValue("@RHSHeight_Top", RhsHeightTop)
                    myCmd.Parameters.AddWithValue("@LHSWidth_Bottom", LhsWidthBot)
                    myCmd.Parameters.AddWithValue("@LHSHeight_Bottom", LhsHeightBot)
                    myCmd.Parameters.AddWithValue("@RHSWidth_Bottom", RhsWidthBot)
                    myCmd.Parameters.AddWithValue("@RHSHeight_Bottom", RhsHeightBot)
                    myCmd.Parameters.AddWithValue("@Notes", "")
                    myCmd.Parameters.AddWithValue("@MarkUp", 0)
                    myCmd.ExecuteNonQuery()
                End Using

                
            Next
            

           
            Return New With {.error = False, .message = "success"}
        Catch ex As Exception
            Return New With { .error = True, .message = String.Format("InsertVenetianBlinds: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function InsertVeriShades(headerId As String, listDetail As List(Of Object), conn As SqlConnection, trans As SqlTransaction) As Object
        Try
            If listDetail IsNot Nothing Then
                listDetail.RemoveAll(Function(x) x Is Nothing OrElse IsDBNull(x) OrElse x.ToString() = "null")
            End If
            If listDetail Is Nothing OrElse listDetail.Count = 0 Then
                Throw New Exception("Details is empty !")
            End If

            Dim GetProp = Function(obj As Object, propName As String) As String
                Try
                    Dim val = CallByName(obj, propName, CallType.Get)
                    Return If(val IsNot Nothing, val.ToString(), "")
                Catch ex As Exception
                    Return "" 
                End Try
            End Function

            Dim GetDbVal = Function(strVal As String) As Object
                Return If(String.IsNullOrEmpty(strVal), DBNull.Value, CObj(strVal))
            End Function

            Dim GetInt = Function(strVal As String) As Object
                Return If(String.IsNullOrEmpty(strVal), 0, CObj(strVal))
            End Function

            Dim CreateJSON = Function(value As string, chacked As Boolean) As string
                Dim json = New With { .value = value, .checked = chacked }
                Return Newtonsoft.Json.JsonConvert.SerializeObject({json})
            End Function


            For Each item In listDetail
                Dim ItemId As String = CreateOrderItemId(conn, trans)
                
                Dim BlindType As String = GetProp(item, "BlindType")
                Dim Mounting As String = GetProp(item, "Mounting")
                Dim Width As Integer = GetInt(GetProp(item, "Width"))
                Dim Drop As Integer = GetInt(GetProp(item, "Drop"))
                Dim FabricType As String = GetProp(item, "FabricType")
                Dim FabricColour As String = GetProp(item, "FabricColour")
                Dim BlindSize As String = GetProp(item, "BlindSize")
                Dim TrackType As String = GetProp(item, "TrackType")
                Dim StackConfigure As String = GetProp(item, "StackConfigure")
                Dim Carrier As Integer = 0
                Dim Spacer As Integer = 0
                Dim Slat As Integer = 0
                Dim SlatQty As Integer = 0
                Dim EndSlat As Integer = 0
                Dim TotalSlat As Integer = 0
                Dim FabricQty As Integer = 0

                Dim DesignId As String = GetItemData("SELECT Id FROM Designs WHERE Name = 'Veri Shades' AND Active = 1", conn, trans)
                
                Dim qBlind As String = String.Format("SELECT Id FROM Blinds WHERE DesignId = '{0}' AND Name = '{1}' AND Active = 1", DesignId, BlindType)
                Dim BlindId As String = GetItemData(qBlind, conn, trans)   
        
                If String.IsNullOrEmpty(BlindId) Then
                    Throw New Exception("Blind Id not found")
                End If  

                

                Dim qKit As String = String.Format("SELECT Id FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND Active = 1", DesignId, BlindId)
                Dim KitId As String = GetItemData(qKit, conn, trans)

                Dim qSoe As String = String.Format("SELECT SoeId FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND Active = 1", DesignId, BlindId)
                Dim SoeKitId As String = GetItemData(qSoe, conn, trans)

                If String.IsNullOrEmpty(KitId) Then
                    Throw New Exception("Kit Id not found")
                End If

                Dim ExactName As String = String.Format("Venetian Blinds - {0}", BlindType)
                Dim ExactId As String = GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName), conn, trans)

                Dim qFabric As String = String.Format("SELECT Id FROM Fabrics WHERE DesignId='{0}' AND LTRIM(RTRIM(Type)) = '{1}' AND LTRIM(RTRIM(Colour)) ='{2}' AND Active =1", DesignId, FabricType, FabricColour)
                Dim FabricId As String = GetItemData(qFabric, conn, trans)
                ' Throw New Exception(qFabric)
                If Not BlindType = "Track Only" And String.IsNullOrEmpty(FabricId) Then
                    Throw New Exception("Fabric Id not found")
                End If

                Dim PriceGroupName As String = BlindType
                Dim FabricGroup As String = GetItemData(String.Format("SELECT [Group] FROM Fabrics WHERE Id = '{0}'",FabricId), conn, trans)
                If BlindType = "Single" Then
                    PriceGroupName = String.Format("Veri Shades - {0}", FabricGroup)
                End If
                If BlindType = "Slat Only" Then
                    PriceGroupName = String.Format("{0} - {1}", BlindType, FabricGroup)
                End If

                Dim PriceGroupId As String = GetItemData(String.Format("SELECT Id FROM PricesGroup WHERE DesignId = '{0}' AND Name = '{1}'", DesignId, PriceGroupName), conn, trans)
                If String.IsNullOrEmpty(PriceGroupId) Then
                    Throw New Exception("price group not found!")
                End If

                Dim selected = SpacerVerishades _
                .Where(Function(x) Width <= x.MaxWidth) _
                .OrderBy(Function(x) x.MaxWidth) _
                .FirstOrDefault()
                Carrier = selected.CarriersQty
                Spacer = selected.Spacer1Type
                Dim CarrierVal As String = CreateJSON(Carrier, False)
                Dim SpacerVal As String = CreateJSON(Spacer, False)
                
                Slat = FindSlatSize(ListParam(
                    BlindType,
                    Mounting,
                    TrackType,
                    Drop
                ))
                Dim SlatVal As String = CreateJSON(Slat, False)
                

                SlatQty = FindSlats(ListParam(
                    "Slat Qty",
                    BlindType,
                    StackConfigure,
                    Carrier,
                    "False"
                ))
                EndSlat = FindSlats(ListParam(
                    "End Slats",
                    BlindType,
                    StackConfigure,
                    Carrier,
                    "False"
                ))
                Dim SlatQtyVal As String = CreateJSON(SlatQty, False)
                Dim EndSlatVal As String = CreateJSON(EndSlat, False)

                TotalSlat = SlatQty + EndSlat


                FabricQty = FindFabricQty(ListParam(
                    BlindType,
                    Slat,
                    TotalSlat,
                    Drop
                ))
                Dim FabricQtyVal As String = CreateJSON(FabricQty, False)

                If BlindSize = "Yes" Then
                    BlindSize = "1"
                Else
                    BlindSize = "0"
                End If
                

                Dim Field As String = "Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, FabricId, PriceGroupId, Qty, Location, LouvreSize, LouvrePosition, Mounting, Width, [Drop], BlindSize,  StackPosition, TrackType, TrackColour, WandLength, WandColour, BracketOption, BracketColour, FrameType, FrameLeft, Layout, LayoutSpecial, SlatSize, SlatQty, TubeSize, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active"

                Dim Values As String = "@Id, @HeaderId, 'Blind 1', @KitId, @SoeKitId, @ExactId, @FabricId, @PriceGroupId,  @Qty, @Location,  @LouvreSize, @LouvrePosition, @Mounting, @Width, @Drop, @BlindSize, @StackPosition, @TrackType, @TrackColour, @WandLength, @WandColour, @BracketOption, @BracketColour, @FrameType, @FrameLeft, @Layout, @LayoutSpecial, @SlatSize, @SlatQty, @TubeSize, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1"

                Using myCmd As New SqlCommand(String.Format("INSERT INTO OrderDetails({0}) VALUES ({1})", Field, Values), conn, trans)
                    myCmd.Parameters.AddWithValue("@Id", ItemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                    myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                    myCmd.Parameters.AddWithValue("@KitId", KitId)
                    myCmd.Parameters.AddWithValue("@SoeKitId", SoeKitId)
                    myCmd.Parameters.AddWithValue("@ExactId", ExactId)
                    myCmd.Parameters.AddWithValue("@FabricId", FabricId)
                    myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
                    myCmd.Parameters.AddWithValue("@Qty", GetDbVal(GetProp(item, "Qty")))
                    myCmd.Parameters.AddWithValue("@Location", GetDbVal(GetProp(item, "Room")))
                    myCmd.Parameters.AddWithValue("@LouvreSize", "")
                    myCmd.Parameters.AddWithValue("@LouvrePosition", "")
                    myCmd.Parameters.AddWithValue("@Mounting", GetDbVal(GetProp(item, "Mounting")))
                    myCmd.Parameters.AddWithValue("@Width", GetDbVal(GetProp(item, "Width")))
                    myCmd.Parameters.AddWithValue("@Drop", GetDbVal(GetProp(item, "Drop")))
                    myCmd.Parameters.AddWithValue("@BlindSize", BlindSize)
                    myCmd.Parameters.AddWithValue("@StackPosition", GetDbVal(GetProp(item, "StackConfigure")))
                    myCmd.Parameters.AddWithValue("@TrackType", GetDbVal(GetProp(item, "TrackType")))
                    myCmd.Parameters.AddWithValue("@TrackColour", GetDbVal(GetProp(item, "TrackColour")))
                    myCmd.Parameters.AddWithValue("@WandLength", GetDbVal(GetProp(item, "WandSize")))
                    myCmd.Parameters.AddWithValue("@WandColour", GetDbVal(GetProp(item, "WandColour")))
                    myCmd.Parameters.AddWithValue("@BracketOption", GetDbVal(GetProp(item, "Bracket")))
                    myCmd.Parameters.AddWithValue("@BracketColour", GetDbVal(GetProp(item, "TapeColour")))
                    myCmd.Parameters.AddWithValue("@FrameType", CarrierVal)
                    myCmd.Parameters.AddWithValue("@FrameLeft", SpacerVal)
                    myCmd.Parameters.AddWithValue("@Layout", SlatVal)
                    myCmd.Parameters.AddWithValue("@LayoutSpecial", SlatQtyVal)
                    myCmd.Parameters.AddWithValue("@SlatSize", EndSlatVal)
                    myCmd.Parameters.AddWithValue("@SlatQty", TotalSlat)
                    myCmd.Parameters.AddWithValue("@TubeSize", FabricQtyVal)
                    myCmd.Parameters.AddWithValue("@Notes", "")
                    myCmd.Parameters.AddWithValue("@MarkUp", 0)
                    myCmd.ExecuteNonQuery()
                End Using
                
            Next
            

           
            Return New With {.error = False, .message = "success"}
        Catch ex As Exception
            Return New With { .error = True, .message = String.Format("InsertVeriShades: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function InsertVerticalBlinds(headerId As String, customerId As String, listDetail As List(Of Object), conn As SqlConnection, trans As SqlTransaction) As Object
        Try
            If listDetail IsNot Nothing Then
                listDetail.RemoveAll(Function(x) x Is Nothing OrElse IsDBNull(x) OrElse x.ToString() = "null")
            End If
            If listDetail Is Nothing OrElse listDetail.Count = 0 Then
                Throw New Exception("Details is empty !")
            End If

            Dim GetProp = Function(obj As Object, propName As String) As String
                Try
                    Dim val = CallByName(obj, propName, CallType.Get)
                    Return If(val IsNot Nothing, val.ToString(), "")
                Catch ex As Exception
                    Return "" 
                End Try
            End Function

            Dim GetDbVal = Function(strVal As String) As Object
                Return If(String.IsNullOrEmpty(strVal), DBNull.Value, CObj(strVal))
            End Function

            Dim GetInt = Function(strVal As String) As Object
                Return If(String.IsNullOrEmpty(strVal), 0, CObj(strVal))
            End Function

            Dim CreateJSON = Function(value As string, chacked As Boolean) As string
                Dim json = New With { .value = value, .checked = chacked }
                Return Newtonsoft.Json.JsonConvert.SerializeObject({json})
            End Function

            Dim GetFabricCutDrop = Function(ListParam As List(Of Object)) As Integer
                Dim res As Integer = 0
                Dim d As Integer = CInt(ListParam(0)) '#Drop
                Dim q As Integer = CInt(ListParam(1)) '#Qty
                Dim b As String = CStr(ListParam(2)) '#BlindName
                Dim c As Integer = CInt(ListParam(3)) '#Carrier

                Dim tempValue As Double = (((d + 92) * c * q) / 1000)
                res = CInt(Math.Floor(tempValue)) + If(tempValue Mod 1 > 0, 1, 0)

                If b.Contains("Slat") Then
                    res = 0
                End If

                Return res
            End Function


            For Each item In listDetail
                Dim ItemId As String = CreateOrderItemId(conn, trans)
                
                Dim BlindType As String = GetProp(item, "BlindType")
                Dim TubeType As String = GetProp(item, "TubeType")
                Dim ControlType As String = GetProp(item, "ControlType")
                If String.IsNullOrEmpty(ControlType) Then ControlType = "N/A"
                Dim Qty As String = GetProp(item, "Qty")
                Dim Mounting As String = GetProp(item, "Mounting")
                Dim Width As Integer = GetInt(GetProp(item, "Width"))
                Dim Drop As Integer = GetInt(GetProp(item, "Drop"))
                Dim TrackType As String = GetProp(item, "TrackType")
                Dim SlatQty As Integer = GetInt(GetProp(item, "SlatQty"))    
                Dim FabricType As String = GetProp(item, "FabricType")
                Dim FabricSlat As String = GetProp(item, "FabricSlat")
                Dim FabricColour As String = GetProp(item, "FabricColour")
                Dim SlatSize As String = GetProp(item, "SlatSize")
                Dim StackPosition As String = GetProp(item, "StackPosition")
                Dim ControlPosition As String = GetProp(item, "ControlPosition")
                Dim ChainColour As String = GetProp(item, "ChainColour")
                Dim ChainLength As String = GetProp(item, "ChainLength")
                Dim Bottom As String = GetProp(item, "Bottom")
                Dim InsertInTrack As String = GetProp(item, "InsertInTrack")
                Dim Sloper As String = GetProp(item, "Sloper")

                Dim ChainId As String = ""
                Dim Carrier As Integer = 0
                Dim Spacer As Integer = 0
                Dim FabricDrop As Integer = 0
                Dim CutLength As Integer = 0

                If TubeType = "Phoenixline" Then
                    If Not InArray(customerId, "LS-A110326061139", "LS-A040226074223", "LS-A100326044643", "LS-A250226114410", "LS-A060226125426") Then
                        ' Throw New Exception("Not have permission for Phoenixline")
                        Continue For
                    End If
                End If

                Dim DesignId As String = GetItemData("SELECT Id FROM Designs WHERE Name = 'Vertical Blinds' AND Active = 1", conn, trans)
                
                Dim qBlind As String = String.Format("SELECT Id FROM Blinds WHERE DesignId = '{0}' AND Name = '{1}' AND Active = 1", DesignId, BlindType)
                Dim BlindId As String = GetItemData(qBlind, conn, trans)   
        
                If String.IsNullOrEmpty(BlindId) Then
                    Throw New Exception("Blind Id not found")
                End If  

                Dim qKit As String = String.Format("SELECT Id FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND TubeType = '{2}' AND ControlType = '{3}' AND Active = 1", DesignId, BlindId, TubeType, ControlType)
                Dim KitId As String = GetItemData(qKit, conn, trans)
                
                Dim qSoe As String = String.Format("SELECT SoeId FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND TubeType = '{2}' AND ControlType = '{3}' AND Active = 1", DesignId, BlindId, TubeType, ControlType)
                Dim SoeKitId As String = GetItemData(qSoe, conn, trans)

                If String.IsNullOrEmpty(KitId) Then
                    Throw New Exception("Kit Id not found")
                End If

                Dim ExactName As String = String.Format("Venetian Blinds - {0}", BlindType)
                Dim ExactId As String = GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName), conn, trans)

                Dim qFabric As String = String.Format("SELECT Id FROM Fabrics WHERE DesignId='{0}' AND LTRIM(RTRIM(Type)) = '{1}' AND LTRIM(RTRIM(Width)) ='{2}' AND LTRIM(RTRIM(Colour)) ='{3}' AND Active =1", DesignId, FabricType, FabricSlat, FabricColour)
                Dim FabricId As String = GetItemData(qFabric, conn, trans)
                ' Throw New Exception(qFabric)
                If Not BlindType = "Track Only" And String.IsNullOrEmpty(FabricId) Then
                    Throw New Exception("Fabric Id not found")
                End If

                Dim FabricGroup As String = GetItemData(String.Format("SELECT [Group] FROM Fabrics WHERE Id = '{0}'", FabricId), conn, trans)
                Dim PriceGroupName As String = String.Format("{0} - {1}", BlindType, FabricGroup)
                If BlindType = "Track Only" Then
                    PriceGroupName = String.Format("{0} - {1}", BlindType, TubeType)
                End If
                If BlindType = "Slat Only" And Bottom = "Top Hanger Only" Then
                    PriceGroupName = String.Format("{0} With Hanger - {1}", BlindType, FabricGroup)
                End If
                If BlindType = "Slat Only" And InStr(FabricType, "Builder") Then
                    PriceGroupName = String.Format("{0} - Group 1", BlindType, FabricGroup)
                    If Bottom = "Top Hanger Only" Then
                        PriceGroupName = String.Format("{0} With Hanger - Group 1", BlindType, FabricGroup)
                    End If
                End If

                Dim PriceGroupId As String = GetItemData(String.Format("SELECT Id FROM PricesGroup WHERE DesignId = '{0}' AND Name = '{1}'", DesignId, PriceGroupName), conn, trans)
                If String.IsNullOrEmpty(PriceGroupId) Then
                    Throw New Exception("price group not found!")
                End If

                If ControlType = "Chain" Then
                    ChainColour = String.format("({0})", ChainColour)

                    If String.IsNullOrEmpty(ChainLength) Or ChainLength = "0" Then
                        If drop >= 3000 Then
                            ChainLength = "2200"
                        ElseIf drop >= 2700 Then
                            ChainLength = "2000"
                        ElseIf drop >= 2400 Then
                            ChainLength = "1800"
                        ElseIf drop >= 2000 Then
                            ChainLength = "1500"
                        ElseIf drop >= 1600 Then
                            ChainLength = "1250"
                        ElseIf drop >= 1300 Then
                            ChainLength = "1000"
                        ElseIf drop >= 1100 Then
                            ChainLength = "800"
                        ElseIf drop >= 800 Then
                            ChainLength = "600"
                        Else
                            ChainLength = "500"
                        End If
                    End If


                    Dim ChainName As String = String.Format("{0} Chain + Joiner {1}", ChainLength, ChainColour)
                    Dim FormulaChain As String = GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName), conn, trans)

                    IF Not String.IsNullOrEmpty(FormulaChain) Then
                        ChainId = FormulaChain
                    End If
                    If String.IsNullOrEmpty(FormulaChain) Then
                        ChainName = String.Format("Custom Chain + Joiner {0}", ChainColour)
                        ChainId = GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName), conn, trans)
                    End If

                    If String.IsNullOrEmpty(ChainId) Then
                        Throw New Exception("Chain Id not found")
                    End If

                End If

                If BlindType = "Complete" Then
                    Select Case TubeType
                        Case "Louvolite"
                            CutLength = Drop - 45
                        Case "Fairline"
                            CutLength = Drop - 40
                        Case "Phoenixline"
                            CutLength = Drop - 35
                    End Select
                End If

                If BlindType = "Slat Only" Then
                    Select Case TrackType
                        Case "Louvolite", "Other"
                            CutLength = drop - 45
                        Case "Fairline", "40mm Sunlight"
                            CutLength = drop - 40
                        Case "Phoenixline"
                            CutLength = drop - 35
                    End Select
                End If

                If BlindType = "Track Only" Then
                   SlatQty = GetCarrierSpacer(ListParam(
                        SlatSize,
                        TubeType,
                        "Vertical Blinds",
                        BlindType,
                        Width,
                        "CarrierQty",
                        ControlPosition,
                        StackPosition
                   ))
                End If

                If InArray(BlindType , "Complete", "Slat Only") Then
                    SlatSize = FabricSlat
                End If

                If InsertInTrack = "Yes" Then
                    InsertInTrack = "1"
                Else
                    InsertInTrack = "0"
                End If

                If Sloper = "Yes" Then
                    Sloper = "1"
                Else
                    Sloper = "0"
                End If

                Carrier = GetCarrierSpacer(ListParam(
                    SlatSize,
                    TubeType,
                    "Vertical Blinds",
                    BlindType,
                    Width,
                    "CarrierQty",
                    ControlPosition,
                    StackPosition
                ))
                Dim CarrierVal As String = CreateJSON(Carrier, False)

                Spacer = GetCarrierSpacer(ListParam(
                    SlatSize,
                    TubeType,
                    "Vertical Blinds",
                    BlindType,
                    Width,
                    "Spacer1Type",
                    ControlPosition,
                    StackPosition
                ))
                Dim SpacerVal As String = CreateJSON(Spacer, False)

                FabricDrop = GetFabricCutDrop(ListParam(
                    Drop,
                    Qty,
                    BlindType,
                    Carrier
                ))

                ' Throw New Exception("Ready to insert")

                Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, FabricId, ChainId, PriceGroupId, Qty, Location, LouvreSize, LouvrePosition, Mounting, TrackType, SlatSize, SlatQty, Width, [Drop], StackPosition, ControlPosition, TrackColour, TrackLength, ChainLength, WandColour, WandLength, BracketOption, BracketColour, HangerType, BottomHoldDown, InsertInTrack, Sloper, FrameType, FrameLeft, CustomHeaderLength,  Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @FabricId, @ChainId, @PriceGroupId, @Qty, @Location, @LouvreSize, @LouvrePosition, @Mounting, @TrackType, @SlatSize, @SlatQty, @Width, @Drop, @StackPosition, @ControlPosition, @TrackColour, @TrackLength, @ChainLength, @WandColour, @WandLength, @BracketOption, @BracketColour, @HangerType, @BottomHoldDown, @InsertInTrack, @Sloper, @FrameType, @FrameLeft, @CustomHeaderLength, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", conn, trans)
                    myCmd.Parameters.AddWithValue("@Id", ItemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                    myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                    myCmd.Parameters.AddWithValue("@KitId", KitId)
                    myCmd.Parameters.AddWithValue("@SoeKitId", SoeKitId)
                    myCmd.Parameters.AddWithValue("@ExactId", ExactId)
                    myCmd.Parameters.AddWithValue("@FabricId", FabricId)
                    myCmd.Parameters.AddWithValue("@ChainId", ChainId)
                    myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
                    myCmd.Parameters.AddWithValue("@Qty", Qty)
                    myCmd.Parameters.AddWithValue("@Location", GetDbVal(GetProp(item, "Room")))
                    myCmd.Parameters.AddWithValue("@LouvreSize", DBNull.Value)
                    myCmd.Parameters.AddWithValue("@LouvrePosition", DBNull.Value)
                    myCmd.Parameters.AddWithValue("@Mounting", GetDbVal(GetProp(item, "Mounting")))
                    myCmd.Parameters.AddWithValue("@TrackType", TrackType)
                    myCmd.Parameters.AddWithValue("@SlatSize", SlatSize)
                    myCmd.Parameters.AddWithValue("@SlatQty", SlatQty)
                    myCmd.Parameters.AddWithValue("@Width", Width)
                    myCmd.Parameters.AddWithValue("@Drop", Drop)
                    myCmd.Parameters.AddWithValue("@StackPosition", StackPosition)
                    myCmd.Parameters.AddWithValue("@ControlPosition", ControlPosition)
                    myCmd.Parameters.AddWithValue("@TrackColour", GetDbVal(GetProp(item, "TrackColour")))
                    myCmd.Parameters.AddWithValue("@TrackLength", CutLength)
                    myCmd.Parameters.AddWithValue("@ChainLength", ChainLength)
                    myCmd.Parameters.AddWithValue("@WandColour", GetDbVal(GetProp(item, "WandColour")))
                    myCmd.Parameters.AddWithValue("@WandLength", GetDbVal(GetProp(item, "WandLength")))
                    myCmd.Parameters.AddWithValue("@BracketOption", GetDbVal(GetProp(item, "BracketType")))
                    myCmd.Parameters.AddWithValue("@BracketColour", GetDbVal(GetProp(item, "BracketColour")))
                    myCmd.Parameters.AddWithValue("@HangerType", GetDbVal(GetProp(item, "HangerType")))
                    myCmd.Parameters.AddWithValue("@BottomHoldDown", GetDbVal(GetProp(item, "Bottom")))
                    myCmd.Parameters.AddWithValue("@InsertInTrack", InsertInTrack)
                    myCmd.Parameters.AddWithValue("@Sloper", Sloper)
                    myCmd.Parameters.AddWithValue("@FrameType", CarrierVal)
                    myCmd.Parameters.AddWithValue("@FrameLeft", SpacerVal)
                    myCmd.Parameters.AddWithValue("@CustomHeaderLength", FabricDrop)
                    myCmd.Parameters.AddWithValue("@Notes", DBNull.Value)
                    myCmd.Parameters.AddWithValue("@MarkUp", 0)
                    myCmd.ExecuteNonQuery()
                End Using
                
            Next
            

           
            Return New With {.error = False, .message = "success"}
        Catch ex As Exception
            Return New With { .error = True, .message = String.Format("InsertVerticalBlinds: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function GetItemData(query As String, conn As SqlConnection, trans As SqlTransaction) As String
        Using cmd As New SqlCommand(query, conn, trans)
            Dim res = cmd.ExecuteScalar()
            Return If(res IsNot Nothing, res.ToString(), "")
        End Using
    End Function

    Private Shared Function CreateOrderItemId(conn As SqlConnection, trans As SqlTransaction) As String
        Dim result As String = "1"
        Dim idDetail As Object = Nothing

        ' Jalankan SELECT di dalam koneksi dan transaksi yang sedang aktif
        Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderDetails WITH (UPDLOCK) ORDER BY Id DESC", conn, trans)
            idDetail = myCmd.ExecuteScalar()
        End Using

        If idDetail IsNot Nothing AndAlso Not IsDBNull(idDetail) AndAlso IsNumeric(idDetail) Then
            result = (CInt(idDetail) + 1).ToString()
        End If

        Return result
    End Function

    ' New SpacerInfo With {.MinWidth = 0, .MaxWidth = 141, .Spacer1Type = "107|109|110", .CarriersQty = 1},
    Private Shared ReadOnly SpacerVerishades As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MinWidth = 0, .MaxWidth = 141, .Spacer1Type = "107", .CarriersQty = 1},
        New SpacerInfo With {.MinWidth = 142, .MaxWidth = 248, .Spacer1Type = "107", .CarriersQty = 2},
        New SpacerInfo With {.MinWidth = 249, .MaxWidth = 250, .Spacer1Type = "109", .CarriersQty = 2},
        New SpacerInfo With {.MinWidth = 251, .MaxWidth = 251, .Spacer1Type = "110", .CarriersQty = 2},
        New SpacerInfo With {.MinWidth = 252, .MaxWidth = 355, .Spacer1Type = "107", .CarriersQty = 3},
        New SpacerInfo With {.MinWidth = 356, .MaxWidth = 359, .Spacer1Type = "109", .CarriersQty = 3},
        New SpacerInfo With {.MinWidth = 360, .MaxWidth = 361, .Spacer1Type = "110", .CarriersQty = 3},
        New SpacerInfo With {.MinWidth = 362, .MaxWidth = 462, .Spacer1Type = "107", .CarriersQty = 4},
        New SpacerInfo With {.MinWidth = 463, .MaxWidth = 468, .Spacer1Type = "109", .CarriersQty = 4},
        New SpacerInfo With {.MinWidth = 469, .MaxWidth = 471, .Spacer1Type = "110", .CarriersQty = 4},
        New SpacerInfo With {.MinWidth = 472, .MaxWidth = 569, .Spacer1Type = "107", .CarriersQty = 5},
        New SpacerInfo With {.MinWidth = 570, .MaxWidth = 577, .Spacer1Type = "109", .CarriersQty = 5},
        New SpacerInfo With {.MinWidth = 578, .MaxWidth = 581, .Spacer1Type = "110", .CarriersQty = 5},
        New SpacerInfo With {.MinWidth = 582, .MaxWidth = 676, .Spacer1Type = "107", .CarriersQty = 6},
        New SpacerInfo With {.MinWidth = 677, .MaxWidth = 686, .Spacer1Type = "109", .CarriersQty = 6},
        New SpacerInfo With {.MinWidth = 687, .MaxWidth = 691, .Spacer1Type = "110", .CarriersQty = 6},
        New SpacerInfo With {.MinWidth = 692, .MaxWidth = 783, .Spacer1Type = "107", .CarriersQty = 7},
        New SpacerInfo With {.MinWidth = 784, .MaxWidth = 795, .Spacer1Type = "109", .CarriersQty = 7},
        New SpacerInfo With {.MinWidth = 796, .MaxWidth = 801, .Spacer1Type = "110", .CarriersQty = 7},
        New SpacerInfo With {.MinWidth = 802, .MaxWidth = 890, .Spacer1Type = "107", .CarriersQty = 8},
        New SpacerInfo With {.MinWidth = 891, .MaxWidth = 904, .Spacer1Type = "109", .CarriersQty = 8},
        New SpacerInfo With {.MinWidth = 905, .MaxWidth = 911, .Spacer1Type = "110", .CarriersQty = 8},
        New SpacerInfo With {.MinWidth = 912, .MaxWidth = 997, .Spacer1Type = "107", .CarriersQty = 9},
        New SpacerInfo With {.MinWidth = 998, .MaxWidth = 1013, .Spacer1Type = "109", .CarriersQty = 9},
        New SpacerInfo With {.MinWidth = 1014, .MaxWidth = 1021, .Spacer1Type = "110", .CarriersQty = 9},
        New SpacerInfo With {.MinWidth = 1022, .MaxWidth = 1104, .Spacer1Type = "107", .CarriersQty = 10},
        New SpacerInfo With {.MinWidth = 1105, .MaxWidth = 1122, .Spacer1Type = "109", .CarriersQty = 10},
        New SpacerInfo With {.MinWidth = 1123, .MaxWidth = 1131, .Spacer1Type = "110", .CarriersQty = 10},
        New SpacerInfo With {.MinWidth = 1132, .MaxWidth = 1211, .Spacer1Type = "107", .CarriersQty = 11},
        New SpacerInfo With {.MinWidth = 1212, .MaxWidth = 1231, .Spacer1Type = "109", .CarriersQty = 11},
        New SpacerInfo With {.MinWidth = 1232, .MaxWidth = 1241, .Spacer1Type = "110", .CarriersQty = 11},
        New SpacerInfo With {.MinWidth = 1242, .MaxWidth = 1318, .Spacer1Type = "107", .CarriersQty = 12},
        New SpacerInfo With {.MinWidth = 1319, .MaxWidth = 1340, .Spacer1Type = "109", .CarriersQty = 12},
        New SpacerInfo With {.MinWidth = 1341, .MaxWidth = 1351, .Spacer1Type = "110", .CarriersQty = 12},
        New SpacerInfo With {.MinWidth = 1352, .MaxWidth = 1425, .Spacer1Type = "107", .CarriersQty = 13},
        New SpacerInfo With {.MinWidth = 1426, .MaxWidth = 1449, .Spacer1Type = "109", .CarriersQty = 13},
        New SpacerInfo With {.MinWidth = 1450, .MaxWidth = 1461, .Spacer1Type = "110", .CarriersQty = 13},
        New SpacerInfo With {.MinWidth = 1462, .MaxWidth = 1532, .Spacer1Type = "107", .CarriersQty = 14},
        New SpacerInfo With {.MinWidth = 1533, .MaxWidth = 1558, .Spacer1Type = "109", .CarriersQty = 14},
        New SpacerInfo With {.MinWidth = 1559, .MaxWidth = 1571, .Spacer1Type = "110", .CarriersQty = 14},
        New SpacerInfo With {.MinWidth = 1572, .MaxWidth = 1639, .Spacer1Type = "107", .CarriersQty = 15},
        New SpacerInfo With {.MinWidth = 1640, .MaxWidth = 1667, .Spacer1Type = "109", .CarriersQty = 15},
        New SpacerInfo With {.MinWidth = 1668, .MaxWidth = 1681, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MinWidth = 1682, .MaxWidth = 1746, .Spacer1Type = "107", .CarriersQty = 16},
        New SpacerInfo With {.MinWidth = 1747, .MaxWidth = 1776, .Spacer1Type = "109", .CarriersQty = 16},
        New SpacerInfo With {.MinWidth = 1777, .MaxWidth = 1791, .Spacer1Type = "110", .CarriersQty = 16},
        New SpacerInfo With {.MinWidth = 1792, .MaxWidth = 1853, .Spacer1Type = "107", .CarriersQty = 17},
        New SpacerInfo With {.MinWidth = 1854, .MaxWidth = 1885, .Spacer1Type = "109", .CarriersQty = 17},
        New SpacerInfo With {.MinWidth = 1886, .MaxWidth = 1901, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MinWidth = 1902, .MaxWidth = 1960, .Spacer1Type = "107", .CarriersQty = 18},
        New SpacerInfo With {.MinWidth = 1961, .MaxWidth = 1994, .Spacer1Type = "109", .CarriersQty = 18},
        New SpacerInfo With {.MinWidth = 1995, .MaxWidth = 2011, .Spacer1Type = "110", .CarriersQty = 18},
        New SpacerInfo With {.MinWidth = 2012, .MaxWidth = 2067, .Spacer1Type = "107", .CarriersQty = 19},
        New SpacerInfo With {.MinWidth = 2068, .MaxWidth = 2103, .Spacer1Type = "109", .CarriersQty = 19},
        New SpacerInfo With {.MinWidth = 2104, .MaxWidth = 2121, .Spacer1Type = "110", .CarriersQty = 19},
        New SpacerInfo With {.MinWidth = 2122, .MaxWidth = 2174, .Spacer1Type = "107", .CarriersQty = 20},
        New SpacerInfo With {.MinWidth = 2175, .MaxWidth = 2212, .Spacer1Type = "109", .CarriersQty = 20},
        New SpacerInfo With {.MinWidth = 2213, .MaxWidth = 2231, .Spacer1Type = "110", .CarriersQty = 20},
        New SpacerInfo With {.MinWidth = 2232, .MaxWidth = 2281, .Spacer1Type = "107", .CarriersQty = 21},
        New SpacerInfo With {.MinWidth = 2282, .MaxWidth = 2321, .Spacer1Type = "109", .CarriersQty = 21},
        New SpacerInfo With {.MinWidth = 2322, .MaxWidth = 2341, .Spacer1Type = "110", .CarriersQty = 21},
        New SpacerInfo With {.MinWidth = 2342, .MaxWidth = 2388, .Spacer1Type = "107", .CarriersQty = 22},
        New SpacerInfo With {.MinWidth = 2389, .MaxWidth = 2430, .Spacer1Type = "109", .CarriersQty = 22},
        New SpacerInfo With {.MinWidth = 2431, .MaxWidth = 2451, .Spacer1Type = "110", .CarriersQty = 22},
        New SpacerInfo With {.MinWidth = 2452, .MaxWidth = 2495, .Spacer1Type = "107", .CarriersQty = 23},
        New SpacerInfo With {.MinWidth = 2496, .MaxWidth = 2539, .Spacer1Type = "109", .CarriersQty = 23},
        New SpacerInfo With {.MinWidth = 2540, .MaxWidth = 2561, .Spacer1Type = "110", .CarriersQty = 23},
        New SpacerInfo With {.MinWidth = 2562, .MaxWidth = 2602, .Spacer1Type = "107", .CarriersQty = 24},
        New SpacerInfo With {.MinWidth = 2603, .MaxWidth = 2648, .Spacer1Type = "109", .CarriersQty = 24},
        New SpacerInfo With {.MinWidth = 2649, .MaxWidth = 2671, .Spacer1Type = "110", .CarriersQty = 24},
        New SpacerInfo With {.MinWidth = 2672, .MaxWidth = 2709, .Spacer1Type = "107", .CarriersQty = 25},
        New SpacerInfo With {.MinWidth = 2710, .MaxWidth = 2757, .Spacer1Type = "109", .CarriersQty = 25},
        New SpacerInfo With {.MinWidth = 2758, .MaxWidth = 2781, .Spacer1Type = "110", .CarriersQty = 25},
        New SpacerInfo With {.MinWidth = 2782, .MaxWidth = 2816, .Spacer1Type = "107", .CarriersQty = 26},
        New SpacerInfo With {.MinWidth = 2817, .MaxWidth = 2866, .Spacer1Type = "109", .CarriersQty = 26},
        New SpacerInfo With {.MinWidth = 2867, .MaxWidth = 2891, .Spacer1Type = "110", .CarriersQty = 26},
        New SpacerInfo With {.MinWidth = 2892, .MaxWidth = 2923, .Spacer1Type = "107", .CarriersQty = 27},
        New SpacerInfo With {.MinWidth = 2924, .MaxWidth = 2975, .Spacer1Type = "109", .CarriersQty = 27},
        New SpacerInfo With {.MinWidth = 2976, .MaxWidth = 3001, .Spacer1Type = "110", .CarriersQty = 27},
        New SpacerInfo With {.MinWidth = 3002, .MaxWidth = 3030, .Spacer1Type = "107", .CarriersQty = 28},
        New SpacerInfo With {.MinWidth = 3031, .MaxWidth = 3084, .Spacer1Type = "109", .CarriersQty = 28},
        New SpacerInfo With {.MinWidth = 3085, .MaxWidth = 3111, .Spacer1Type = "110", .CarriersQty = 28},
        New SpacerInfo With {.MinWidth = 3112, .MaxWidth = 3137, .Spacer1Type = "107", .CarriersQty = 29},
        New SpacerInfo With {.MinWidth = 3138, .MaxWidth = 3193, .Spacer1Type = "109", .CarriersQty = 29},
        New SpacerInfo With {.MinWidth = 3194, .MaxWidth = 3221, .Spacer1Type = "110", .CarriersQty = 29},
        New SpacerInfo With {.MinWidth = 3222, .MaxWidth = 3244, .Spacer1Type = "107", .CarriersQty = 30},
        New SpacerInfo With {.MinWidth = 3245, .MaxWidth = 3302, .Spacer1Type = "109", .CarriersQty = 30},
        New SpacerInfo With {.MinWidth = 3303, .MaxWidth = 3331, .Spacer1Type = "110", .CarriersQty = 30},
        New SpacerInfo With {.MinWidth = 3332, .MaxWidth = 3351, .Spacer1Type = "107", .CarriersQty = 31},
        New SpacerInfo With {.MinWidth = 3352, .MaxWidth = 3411, .Spacer1Type = "109", .CarriersQty = 31},
        New SpacerInfo With {.MinWidth = 3412, .MaxWidth = 3441, .Spacer1Type = "110", .CarriersQty = 31},
        New SpacerInfo With {.MinWidth = 3442, .MaxWidth = 3458, .Spacer1Type = "107", .CarriersQty = 32},
        New SpacerInfo With {.MinWidth = 3459, .MaxWidth = 3520, .Spacer1Type = "109", .CarriersQty = 32},
        New SpacerInfo With {.MinWidth = 3521, .MaxWidth = 3551, .Spacer1Type = "110", .CarriersQty = 32},
        New SpacerInfo With {.MinWidth = 3552, .MaxWidth = 3565, .Spacer1Type = "107", .CarriersQty = 33},
        New SpacerInfo With {.MinWidth = 3566, .MaxWidth = 3629, .Spacer1Type = "109", .CarriersQty = 33},
        New SpacerInfo With {.MinWidth = 3630, .MaxWidth = 3661, .Spacer1Type = "110", .CarriersQty = 33},
        New SpacerInfo With {.MinWidth = 3662, .MaxWidth = 3672, .Spacer1Type = "107", .CarriersQty = 34},
        New SpacerInfo With {.MinWidth = 3673, .MaxWidth = 3738, .Spacer1Type = "109", .CarriersQty = 34},
        New SpacerInfo With {.MinWidth = 3739, .MaxWidth = 3771, .Spacer1Type = "110", .CarriersQty = 34},
        New SpacerInfo With {.MinWidth = 3772, .MaxWidth = 3779, .Spacer1Type = "107", .CarriersQty = 35},
        New SpacerInfo With {.MinWidth = 3780, .MaxWidth = 3847, .Spacer1Type = "109", .CarriersQty = 35},
        New SpacerInfo With {.MinWidth = 3848, .MaxWidth = 3881, .Spacer1Type = "110", .CarriersQty = 35},
        New SpacerInfo With {.MinWidth = 3882, .MaxWidth = 3886, .Spacer1Type = "107", .CarriersQty = 36},
        New SpacerInfo With {.MinWidth = 3887, .MaxWidth = 3956, .Spacer1Type = "109", .CarriersQty = 36},
        New SpacerInfo With {.MinWidth = 3957, .MaxWidth = 3991, .Spacer1Type = "110", .CarriersQty = 36},
        New SpacerInfo With {.MinWidth = 3992, .MaxWidth = 3993, .Spacer1Type = "107", .CarriersQty = 37},
        New SpacerInfo With {.MinWidth = 3994, .MaxWidth = 4065, .Spacer1Type = "109", .CarriersQty = 37},
        New SpacerInfo With {.MinWidth = 4066, .MaxWidth = 4101, .Spacer1Type = "110", .CarriersQty = 37},
        New SpacerInfo With {.MinWidth = 4102, .MaxWidth = 4102, .Spacer1Type = "107", .CarriersQty = 38},
        New SpacerInfo With {.MinWidth = 4103, .MaxWidth = 4174, .Spacer1Type = "109", .CarriersQty = 38},
        New SpacerInfo With {.MinWidth = 4175, .MaxWidth = 4211, .Spacer1Type = "110", .CarriersQty = 38},
        New SpacerInfo With {.MinWidth = 4212, .MaxWidth = 4212, .Spacer1Type = "107", .CarriersQty = 39},
        New SpacerInfo With {.MinWidth = 4213, .MaxWidth = 4283, .Spacer1Type = "109", .CarriersQty = 39},
        New SpacerInfo With {.MinWidth = 4284, .MaxWidth = 4321, .Spacer1Type = "110", .CarriersQty = 39},
        New SpacerInfo With {.MinWidth = 4322, .MaxWidth = 4322, .Spacer1Type = "107", .CarriersQty = 40},
        New SpacerInfo With {.MinWidth = 4323, .MaxWidth = 4392, .Spacer1Type = "109", .CarriersQty = 40},
        New SpacerInfo With {.MinWidth = 4393, .MaxWidth = 4431, .Spacer1Type = "110", .CarriersQty = 40},
        New SpacerInfo With {.MinWidth = 4432, .MaxWidth = 4432, .Spacer1Type = "107", .CarriersQty = 41},
        New SpacerInfo With {.MinWidth = 4433, .MaxWidth = 4501, .Spacer1Type = "109", .CarriersQty = 41},
        New SpacerInfo With {.MinWidth = 4502, .MaxWidth = 4541, .Spacer1Type = "110", .CarriersQty = 41},
        New SpacerInfo With {.MinWidth = 4542, .MaxWidth = 4542, .Spacer1Type = "107", .CarriersQty = 42},
        New SpacerInfo With {.MinWidth = 4543, .MaxWidth = 4610, .Spacer1Type = "109", .CarriersQty = 42},
        New SpacerInfo With {.MinWidth = 4611, .MaxWidth = 4651, .Spacer1Type = "110", .CarriersQty = 42},
        New SpacerInfo With {.MinWidth = 4652, .MaxWidth = 4652, .Spacer1Type = "107", .CarriersQty = 43},
        New SpacerInfo With {.MinWidth = 4653, .MaxWidth = 4719, .Spacer1Type = "109", .CarriersQty = 43},
        New SpacerInfo With {.MinWidth = 4720, .MaxWidth = 4761, .Spacer1Type = "110", .CarriersQty = 43},
        New SpacerInfo With {.MinWidth = 4762, .MaxWidth = 4762, .Spacer1Type = "107", .CarriersQty = 44},
        New SpacerInfo With {.MinWidth = 4763, .MaxWidth = 4828, .Spacer1Type = "109", .CarriersQty = 44},
        New SpacerInfo With {.MinWidth = 4829, .MaxWidth = 4871, .Spacer1Type = "110", .CarriersQty = 44},
        New SpacerInfo With {.MinWidth = 4872, .MaxWidth = 4872, .Spacer1Type = "107", .CarriersQty = 45},
        New SpacerInfo With {.MinWidth = 4873, .MaxWidth = 4937, .Spacer1Type = "109", .CarriersQty = 45},
        New SpacerInfo With {.MinWidth = 4938, .MaxWidth = 4981, .Spacer1Type = "110", .CarriersQty = 45},
        New SpacerInfo With {.MinWidth = 4982, .MaxWidth = 4982, .Spacer1Type = "107", .CarriersQty = 46},
        New SpacerInfo With {.MinWidth = 4983, .MaxWidth = 5046, .Spacer1Type = "109", .CarriersQty = 46},
        New SpacerInfo With {.MinWidth = 5047, .MaxWidth = 5091, .Spacer1Type = "110", .CarriersQty = 46},
        New SpacerInfo With {.MinWidth = 5092, .MaxWidth = 5092, .Spacer1Type = "107", .CarriersQty = 47},
        New SpacerInfo With {.MinWidth = 5093, .MaxWidth = 5155, .Spacer1Type = "109", .CarriersQty = 47},
        New SpacerInfo With {.MinWidth = 5156, .MaxWidth = 5201, .Spacer1Type = "110", .CarriersQty = 47},
        New SpacerInfo With {.MinWidth = 5202, .MaxWidth = 5202, .Spacer1Type = "107", .CarriersQty = 48},
        New SpacerInfo With {.MinWidth = 5203, .MaxWidth = 5264, .Spacer1Type = "109", .CarriersQty = 48},
        New SpacerInfo With {.MinWidth = 5265, .MaxWidth = 5311, .Spacer1Type = "110", .CarriersQty = 48},
        New SpacerInfo With {.MinWidth = 5312, .MaxWidth = 5312, .Spacer1Type = "107", .CarriersQty = 49},
        New SpacerInfo With {.MinWidth = 5313, .MaxWidth = 5373, .Spacer1Type = "109", .CarriersQty = 49},
        New SpacerInfo With {.MinWidth = 5374, .MaxWidth = 5421, .Spacer1Type = "110", .CarriersQty = 49},
        New SpacerInfo With {.MinWidth = 5422, .MaxWidth = 5422, .Spacer1Type = "107", .CarriersQty = 50},
        New SpacerInfo With {.MinWidth = 5423, .MaxWidth = 5482, .Spacer1Type = "109", .CarriersQty = 50},
        New SpacerInfo With {.MinWidth = 5483, .MaxWidth = 5531, .Spacer1Type = "110", .CarriersQty = 50},
        New SpacerInfo With {.MinWidth = 5532, .MaxWidth = 5532, .Spacer1Type = "107", .CarriersQty = 51},
        New SpacerInfo With {.MinWidth = 5533, .MaxWidth = 5591, .Spacer1Type = "109", .CarriersQty = 51},
        New SpacerInfo With {.MinWidth = 5592, .MaxWidth = 5641, .Spacer1Type = "110", .CarriersQty = 51},
        New SpacerInfo With {.MinWidth = 5642, .MaxWidth = 5642, .Spacer1Type = "107", .CarriersQty = 52},
        New SpacerInfo With {.MinWidth = 5643, .MaxWidth = 5700, .Spacer1Type = "109", .CarriersQty = 52},
        New SpacerInfo With {.MinWidth = 5701, .MaxWidth = 5751, .Spacer1Type = "110", .CarriersQty = 52},
        New SpacerInfo With {.MinWidth = 5752, .MaxWidth = 5752, .Spacer1Type = "107", .CarriersQty = 53},
        New SpacerInfo With {.MinWidth = 5753, .MaxWidth = 5809, .Spacer1Type = "109", .CarriersQty = 53},
        New SpacerInfo With {.MinWidth = 5810, .MaxWidth = 5861, .Spacer1Type = "110", .CarriersQty = 53}
    }

    Private Shared Function ListParam(ParamArray Params() As Object) As List(Of Object)
        Return New List(Of Object)(Params)
    End Function

    Private Shared Function FindSlatSize(ListParam As List(Of Object)) As String
        Try
            Dim Result As String = "0"
            Dim BlindName As String = CStr(ListParam(0))
            Dim Mounting As String = CStr(ListParam(1))
            Dim TrackType As String = CStr(ListParam(2))
            Dim Drop As Integer = CInt(ListParam(3))

            If InArray (BlindName, "Single", "Track Only", "Slat Only") Then
                If Mounting = "Face Fit" Then
                    Select Case TrackType
                        Case "Cube", "Standard"
                            Result = Drop - 55
                        Case "Decorative (Flat)", "Decorative (Round)"
                            Result = Drop - 70
                    End Select
                Else If Mounting = "Reveal Fit" Then
                    Select Case TrackType
                        Case "Cube", "Standard"
                            Result = Drop - 50
                        Case "Decorative (Flat)", "Decorative (Round)"
                            Result = Drop - 65
                    End Select
                End If
            End If

            Return Result
        Catch ex As Exception
            Return "500"
        End Try
    End Function

    Private Shared Function FindSlats(ListParam As List(Of Object)) As String
        Try
            Dim Result As String = "0"
            Dim SlatQty As String = "0"
            Dim EndSlats As String = "0"

            Dim Param As String = CStr(ListParam(0))
            Dim BlindName As String = CStr(ListParam(1))
            Dim Stacking As String = CStr(ListParam(2))
            Dim Carrier As Integer = CInt(ListParam(3))
            Dim CarrierOverride As Boolean = CBool(ListParam(4))

            If BlindName ="Single" Then
                Select Case Stacking
                    Case "Centre Split"
                        SlatQty = Carrier - 2
                        EndSlats = "4"
                    Case "Centre Stack"
                        SlatQty = Carrier - 1
                        EndSlats = "2"
                    Case "Left"
                        SlatQty = Carrier - 1
                        EndSlats = "2"
                    Case "Right"
                        SlatQty = Carrier - 1
                        EndSlats = "2"
                End Select

                If CarrierOverride = True Then
                    Select Case Stacking
                        Case "Centre Split"
                            SlatQty = Carrier - 2
                            EndSlats = "4"
                        Case "Centre Stack"
                            SlatQty = Carrier - 2
                            EndSlats = "4"
                        Case "Left"
                            SlatQty = Carrier - 1
                            EndSlats = "2"
                        Case "Right"
                            SlatQty = Carrier - 1
                            EndSlats = "2"
                    End Select
                End If
            End If

            If BlindName = "Track Only" Then
                Select Case Stacking
                    Case "Centre Split"
                        SlatQty = Carrier - 2
                        EndSlats = "4"
                    Case "Centre Stack"
                        SlatQty = Carrier - 2
                        EndSlats = "4"
                    Case "Left"
                        SlatQty = Carrier - 1
                        EndSlats = "2"
                    Case "Right"
                        SlatQty = Carrier - 1
                        EndSlats = "2"
                End Select

                If CarrierOverride = True Then
                    Select Case Stacking
                        Case "Centre Split"
                            SlatQty = Carrier - 2
                            EndSlats = "4"
                        Case "Centre Stack"
                            SlatQty = Carrier - 2
                            EndSlats = "4"
                        Case "Left"
                            SlatQty = Carrier - 1
                            EndSlats = "2"
                        Case "Right"
                            SlatQty = Carrier - 1
                            EndSlats = "2"
                    End Select
                End If
            End If

            If BlindName = "Slat Only" Then
                Select Case Stacking
                    Case "Centre Split"
                        SlatQty = Carrier - 2
                        EndSlats = "4"
                    Case "Centre Stack"
                        SlatQty = Carrier - 1
                        EndSlats = "2"
                    Case "Left"
                        SlatQty = Carrier - 1
                        EndSlats = "2"
                    Case "Right"
                        SlatQty = Carrier - 1
                        EndSlats = "2"
                End Select

                If CarrierOverride = True Then
                    Select Case Stacking
                        Case "Centre Split"
                            SlatQty = Carrier - 2
                            EndSlats = "4"
                        Case "Centre Stack"
                            SlatQty = Carrier - 2
                            EndSlats = "4"
                        Case "Left"
                            SlatQty = Carrier - 1
                            EndSlats = "2"
                        Case "Right"
                            SlatQty = Carrier - 1
                            EndSlats = "2"
                    End Select
                End If
            End If

            If Param = "End Slats" Then
                Result = EndSlats
            ElseIf Param = "Slat Qty" Then
                Result = SlatQty
            End If

            Return Result
        Catch ex As Exception
            Return ex.message '"500"
        End Try
    End Function

    Private Shared Function FindFabricQty(ListParam As List(Of Object)) As String
        Try
            Dim Result As String = "0"
            Dim BlindName As String = CStr(ListParam(0))
            Dim SlatSize As Integer = CInt(ListParam(1))
            Dim TotalSlats As Integer = CInt(ListParam(2))
            Dim Drop As Integer = CInt(ListParam(3))
            ' Dim SlatQtyOverride As Boolean = CBool(ListParam(4))
            ' Dim EndSlatsOverride As Boolean = CBool(ListParam(4))

            If BlindName = "Single" Then
                Result = Math.Round(((TotalSlats * Drop) + 100) / 1000, 2)
            End If

            If BlindName = "Slat Only" Then
                '#jika ada Drop & tidak ada slat size
                If Drop > 0 Then
                    If SlatSize < 1 Then
                        Result = Math.Round(((TotalSlats * Drop) + 100) / 1000, 2)
                    End If
                End If

                '#jika tidak ada Drop & ada slat size
                If Drop < 1 Then
                    If SlatSize > 0 Then
                        Result = Math.Round(((TotalSlats * SlatSize) + 100) / 1000, 2)
                    End If
                End If

                '#jika ada Drop & ada slat size
                If Drop > 0 Then
                    If SlatSize > 0 Then
                        Result = Math.Round(((TotalSlats * Drop) + 100) / 1000, 2)
                    End If
                End If
            End If

            Return Result
        Catch ex As Exception
            Return ex.message '"500"
        End Try
    End Function

    Private Shared Function GetCarrierSpacer(ListParam As List(Of Object)) As String
        Dim result As String = "0"
        Dim SlatSize As String = CStr(ListParam(0))
        Dim TubeType As String = CStr(ListParam(1))
        Dim DesignName As String = CStr(ListParam(2))
        Dim BlindName As String = CStr(ListParam(3))
        Dim Width As Integer = CStr(ListParam(4))
        Dim param As String = CStr(ListParam(5))
        Dim ControlPosition As String = CStr(ListParam(6))
        Dim StackPosition As String = CStr(ListParam(7))

        If DesignName = "Vertical Blinds" Then
            If BlindName = "Complete" OR BlindName = "Track Only" Then
                Select Case SlatSize
                    Case "127", "127mm"
                        If TubeType.Contains("Tiltrack") Then
                            Dim selected = Spacer127Tiltrack _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()

                            ' Dim selected = Spacer127Tiltrack _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        ElseIf TubeType.Contains("Louvolite") Then
                            Dim Spacers = Spacer127Louvolite
                            If InArray(StackPosition, "Left", "Right", "Center", "Fix") Then
                                Spacers = Spacer127LouvoliteA
                            End If
                            If InStr(StackPosition, "Split") > 0 AND InArray(ControlPosition, "Right", "Left", "Twin Wand") Then
                                Spacers = Spacer127LouvoliteB
                            End If

                            Dim selected = Spacers _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()

                            ' Dim selected = Spacer127Louvolite _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        ElseIf TubeType.Contains("Javaline") Then
                            Dim selected = Spacer127Javaline _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()
                            ' Dim selected = Spacer127Javaline _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        Else
                            Dim selected = Spacer127Metal _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()

                            ' Dim selected = Spacer127Metal _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        End If
                    Case "100", "100mm"
                        If TubeType.Contains("Tiltrack") Then
                            Dim selected = Spacer100Tiltrack _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()

                            ' Dim selected = Spacer100Tiltrack _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        Else
                            ' Dim selected = Spacer100Metal _
                            ' .Where(Function(x) Width <= x.MaxWidth) _
                            ' .OrderBy(Function(x) x.MaxWidth) _
                            ' .FirstOrDefault()

                            Dim selected = Spacer100Metal _
                            .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        End If
                    Case "89", "89mm"
                        If TubeType.Contains("Tiltrack") Then
                            Dim selected = Spacer89Tiltrack _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()

                            ' Dim selected = Spacer89Tiltrack _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type    
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        ElseIf TubeType.Contains("Louvolite") Then
                            Dim Spacers = Spacer89Louvolite
                            If InArray(StackPosition, "Left", "Right", "Center", "Fix") Then
                                Spacers = Spacer89LouvoliteA
                            End If
                            If InStr(StackPosition, "Split") > 0 AND InArray(ControlPosition, "Right", "Left", "Twin Wand") Then
                                Spacers = Spacer89LouvoliteB
                            End If

                            Dim selected = Spacers _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()
                            ' Dim selected = Spacers _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type    
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        ElseIf TubeType.Contains("Javaline") Then
                            Dim selected = Spacer89Javaline _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()

                            ' Dim selected = Spacer89Javaline _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        Else
                            Dim selected = Spacer89Metal _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()

                            ' Dim selected = Spacer89Metal _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type    
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        End If
                    Case "63", "63mm"
                        If TubeType.Contains("Tiltrack") Then
                            Dim selected = Spacer63Tiltrack _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()

                            ' Dim selected = Spacer63Tiltrack _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type    
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        Else
                            Dim selected = Spacer63Metal _
                            .Where(Function(x) Width <= x.MaxWidth) _
                            .OrderBy(Function(x) x.MaxWidth) _
                            .FirstOrDefault()

                            ' Dim selected = Spacer63Metal _
                            ' .FirstOrDefault(Function(x) Width <= x.MaxWidth)

                            If selected IsNot Nothing Then
                                If param = "Spacer1Type" Then
                                    result = selected.Spacer1Type    
                                ElseIf param = "CarrierQty" Then
                                    result = selected.CarriersQty
                                End If
                            End If
                        End If
                End Select
            End If
        End If
        
        Return result
    End Function

    Private Shared ReadOnly Spacer127Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 249, .Spacer1Type = "110", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 250, .Spacer1Type = "111", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 251, .Spacer1Type = "112", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 252, .Spacer1Type = "113", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 253, .Spacer1Type = "114", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 254, .Spacer1Type = "115", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 255, .Spacer1Type = "116", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 358, .Spacer1Type = "110", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 358, .Spacer1Type = "110", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 360, .Spacer1Type = "111", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 362, .Spacer1Type = "112", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 364, .Spacer1Type = "113", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 366, .Spacer1Type = "114", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 368, .Spacer1Type = "115", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 370, .Spacer1Type = "116", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 466, .Spacer1Type = "110", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 469, .Spacer1Type = "111", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 472, .Spacer1Type = "112", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 475, .Spacer1Type = "113", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 478, .Spacer1Type = "114", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 481, .Spacer1Type = "115", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 484, .Spacer1Type = "116", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 574, .Spacer1Type = "110", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 579, .Spacer1Type = "111", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 583, .Spacer1Type = "112", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 587, .Spacer1Type = "113", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 591, .Spacer1Type = "114", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 595, .Spacer1Type = "115", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 599, .Spacer1Type = "116", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 683, .Spacer1Type = "110", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 688, .Spacer1Type = "111", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 693, .Spacer1Type = "112", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 698, .Spacer1Type = "113", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 703, .Spacer1Type = "114", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 708, .Spacer1Type = "115", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 713, .Spacer1Type = "116", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 791, .Spacer1Type = "110", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 797, .Spacer1Type = "111", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 803, .Spacer1Type = "112", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 809, .Spacer1Type = "113", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 815, .Spacer1Type = "114", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 821, .Spacer1Type = "115", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 827, .Spacer1Type = "116", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 899, .Spacer1Type = "110", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 907, .Spacer1Type = "111", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 914, .Spacer1Type = "112", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 921, .Spacer1Type = "113", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 928, .Spacer1Type = "114", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 935, .Spacer1Type = "115", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 942, .Spacer1Type = "116", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 1007, .Spacer1Type = "110", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1016, .Spacer1Type = "111", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1024, .Spacer1Type = "112", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1032, .Spacer1Type = "113", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1040, .Spacer1Type = "114", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1048, .Spacer1Type = "115", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1056, .Spacer1Type = "116", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1116, .Spacer1Type = "110", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1126, .Spacer1Type = "111", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1135, .Spacer1Type = "112", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1144, .Spacer1Type = "113", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1153, .Spacer1Type = "114", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1162, .Spacer1Type = "115", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1171, .Spacer1Type = "116", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1224, .Spacer1Type = "110", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1235, .Spacer1Type = "111", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1245, .Spacer1Type = "112", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1255, .Spacer1Type = "113", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1265, .Spacer1Type = "114", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1275, .Spacer1Type = "115", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1285, .Spacer1Type = "116", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1332, .Spacer1Type = "110", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1344, .Spacer1Type = "111", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1355, .Spacer1Type = "112", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1366, .Spacer1Type = "113", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1377, .Spacer1Type = "114", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1388, .Spacer1Type = "115", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1399, .Spacer1Type = "116", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1441, .Spacer1Type = "110", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1454, .Spacer1Type = "111", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1466, .Spacer1Type = "112", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1478, .Spacer1Type = "113", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1490, .Spacer1Type = "114", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1502, .Spacer1Type = "115", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1514, .Spacer1Type = "116", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1549, .Spacer1Type = "110", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1563, .Spacer1Type = "111", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1576, .Spacer1Type = "112", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1589, .Spacer1Type = "113", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1602, .Spacer1Type = "114", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1615, .Spacer1Type = "115", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1628, .Spacer1Type = "116", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1641, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1657, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1673, .Spacer1Type = "111", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1687, .Spacer1Type = "112", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1701, .Spacer1Type = "113", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1715, .Spacer1Type = "114", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1729, .Spacer1Type = "115", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1743, .Spacer1Type = "116", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1766, .Spacer1Type = "110", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1782, .Spacer1Type = "111", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1797, .Spacer1Type = "112", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1812, .Spacer1Type = "113", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1827, .Spacer1Type = "114", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1842, .Spacer1Type = "115", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1857, .Spacer1Type = "116", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1872, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1874, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1891, .Spacer1Type = "111", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1907, .Spacer1Type = "112", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1923, .Spacer1Type = "113", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1939, .Spacer1Type = "114", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1955, .Spacer1Type = "115", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1971, .Spacer1Type = "116", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1982, .Spacer1Type = "110", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2001, .Spacer1Type = "111", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2018, .Spacer1Type = "112", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2035, .Spacer1Type = "113", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2052, .Spacer1Type = "114", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2069, .Spacer1Type = "115", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2086, .Spacer1Type = "116", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2090, .Spacer1Type = "110", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2110, .Spacer1Type = "111", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2128, .Spacer1Type = "112", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2146, .Spacer1Type = "113", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2164, .Spacer1Type = "114", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2182, .Spacer1Type = "115", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2199, .Spacer1Type = "110", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "116", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2218, .Spacer1Type = "111", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2220, .Spacer1Type = "111", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2239, .Spacer1Type = "112", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2258, .Spacer1Type = "113", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2277, .Spacer1Type = "114", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2296, .Spacer1Type = "115", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2307, .Spacer1Type = "110", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2315, .Spacer1Type = "116", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2329, .Spacer1Type = "111", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2349, .Spacer1Type = "112", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2369, .Spacer1Type = "113", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2389, .Spacer1Type = "114", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2409, .Spacer1Type = "115", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2415, .Spacer1Type = "110", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2429, .Spacer1Type = "116", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2438, .Spacer1Type = "111", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2449, .Spacer1Type = "112", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2459, .Spacer1Type = "112", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2480, .Spacer1Type = "113", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2501, .Spacer1Type = "114", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2522, .Spacer1Type = "115", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2524, .Spacer1Type = "110", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2543, .Spacer1Type = "116", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2548, .Spacer1Type = "111", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2564, .Spacer1Type = "112", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2570, .Spacer1Type = "112", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2592, .Spacer1Type = "113", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2614, .Spacer1Type = "114", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2632, .Spacer1Type = "110", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2636, .Spacer1Type = "115", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2657, .Spacer1Type = "111", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2658, .Spacer1Type = "116", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2680, .Spacer1Type = "112", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2703, .Spacer1Type = "113", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2726, .Spacer1Type = "114", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2740, .Spacer1Type = "110", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2749, .Spacer1Type = "115", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2767, .Spacer1Type = "111", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2772, .Spacer1Type = "116", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2791, .Spacer1Type = "112", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2815, .Spacer1Type = "113", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2839, .Spacer1Type = "114", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2849, .Spacer1Type = "110", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2863, .Spacer1Type = "115", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2876, .Spacer1Type = "111", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2887, .Spacer1Type = "116", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2901, .Spacer1Type = "112", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2926, .Spacer1Type = "113", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2951, .Spacer1Type = "114", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2957, .Spacer1Type = "110", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2976, .Spacer1Type = "115", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2985, .Spacer1Type = "111", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3001, .Spacer1Type = "116", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3011, .Spacer1Type = "112", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3037, .Spacer1Type = "113", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3063, .Spacer1Type = "114", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3065, .Spacer1Type = "110", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3089, .Spacer1Type = "115", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3095, .Spacer1Type = "111", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3115, .Spacer1Type = "116", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3122, .Spacer1Type = "112", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3149, .Spacer1Type = "113", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3173, .Spacer1Type = "110", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3176, .Spacer1Type = "114", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3203, .Spacer1Type = "115", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3204, .Spacer1Type = "111", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3230, .Spacer1Type = "116", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3232, .Spacer1Type = "112", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3260, .Spacer1Type = "113", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3282, .Spacer1Type = "110", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3288, .Spacer1Type = "114", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3314, .Spacer1Type = "111", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3316, .Spacer1Type = "115", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3343, .Spacer1Type = "112", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3344, .Spacer1Type = "116", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3372, .Spacer1Type = "113", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3390, .Spacer1Type = "110", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3401, .Spacer1Type = "114", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3423, .Spacer1Type = "111", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3430, .Spacer1Type = "115", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3453, .Spacer1Type = "112", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3459, .Spacer1Type = "116", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3483, .Spacer1Type = "113", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3498, .Spacer1Type = "110", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3513, .Spacer1Type = "114", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3532, .Spacer1Type = "111", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3543, .Spacer1Type = "115", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3563, .Spacer1Type = "112", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3573, .Spacer1Type = "116", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3594, .Spacer1Type = "113", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3607, .Spacer1Type = "110", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3625, .Spacer1Type = "114", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3642, .Spacer1Type = "111", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3656, .Spacer1Type = "115", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3674, .Spacer1Type = "112", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3687, .Spacer1Type = "116", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3706, .Spacer1Type = "113", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3715, .Spacer1Type = "110", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3738, .Spacer1Type = "114", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3751, .Spacer1Type = "111", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3770, .Spacer1Type = "115", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3784, .Spacer1Type = "112", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3802, .Spacer1Type = "116", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3817, .Spacer1Type = "113", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3823, .Spacer1Type = "110", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3850, .Spacer1Type = "114", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3861, .Spacer1Type = "111", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3883, .Spacer1Type = "115", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3895, .Spacer1Type = "112", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3916, .Spacer1Type = "116", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3929, .Spacer1Type = "113", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3932, .Spacer1Type = "110", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3963, .Spacer1Type = "114", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3970, .Spacer1Type = "111", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3997, .Spacer1Type = "115", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4005, .Spacer1Type = "112", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4031, .Spacer1Type = "116", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4040, .Spacer1Type = "110", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4040, .Spacer1Type = "113", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4075, .Spacer1Type = "114", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4079, .Spacer1Type = "111", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4110, .Spacer1Type = "115", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4115, .Spacer1Type = "112", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4145, .Spacer1Type = "116", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4148, .Spacer1Type = "110", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4151, .Spacer1Type = "113", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4187, .Spacer1Type = "114", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4189, .Spacer1Type = "111", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4223, .Spacer1Type = "115", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4226, .Spacer1Type = "112", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4256, .Spacer1Type = "110", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4259, .Spacer1Type = "116", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4263, .Spacer1Type = "113", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4298, .Spacer1Type = "111", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4300, .Spacer1Type = "114", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4336, .Spacer1Type = "112", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4337, .Spacer1Type = "115", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4365, .Spacer1Type = "110", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4374, .Spacer1Type = "113", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4374, .Spacer1Type = "116", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4408, .Spacer1Type = "111", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4412, .Spacer1Type = "114", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4447, .Spacer1Type = "112", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4450, .Spacer1Type = "115", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4473, .Spacer1Type = "110", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4486, .Spacer1Type = "113", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4488, .Spacer1Type = "116", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4517, .Spacer1Type = "111", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4525, .Spacer1Type = "114", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4557, .Spacer1Type = "112", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4564, .Spacer1Type = "115", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4581, .Spacer1Type = "110", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4597, .Spacer1Type = "113", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4603, .Spacer1Type = "116", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4626, .Spacer1Type = "111", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4637, .Spacer1Type = "114", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4667, .Spacer1Type = "112", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4677, .Spacer1Type = "115", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4690, .Spacer1Type = "110", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4708, .Spacer1Type = "113", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4717, .Spacer1Type = "116", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4736, .Spacer1Type = "111", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4749, .Spacer1Type = "114", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4778, .Spacer1Type = "112", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4790, .Spacer1Type = "115", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4798, .Spacer1Type = "110", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4820, .Spacer1Type = "113", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4831, .Spacer1Type = "116", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4845, .Spacer1Type = "111", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4862, .Spacer1Type = "114", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4888, .Spacer1Type = "112", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4904, .Spacer1Type = "115", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4906, .Spacer1Type = "110", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 4931, .Spacer1Type = "113", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4946, .Spacer1Type = "116", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4955, .Spacer1Type = "111", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 4974, .Spacer1Type = "114", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4999, .Spacer1Type = "112", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5015, .Spacer1Type = "110", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5017, .Spacer1Type = "115", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5043, .Spacer1Type = "113", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5060, .Spacer1Type = "116", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5064, .Spacer1Type = "111", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5087, .Spacer1Type = "114", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5109, .Spacer1Type = "112", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5123, .Spacer1Type = "110", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5131, .Spacer1Type = "115", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5154, .Spacer1Type = "113", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5173, .Spacer1Type = "111", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5175, .Spacer1Type = "116", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5199, .Spacer1Type = "114", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5219, .Spacer1Type = "112", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5231, .Spacer1Type = "110", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5244, .Spacer1Type = "115", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5265, .Spacer1Type = "113", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5283, .Spacer1Type = "111", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5289, .Spacer1Type = "116", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5311, .Spacer1Type = "114", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5330, .Spacer1Type = "112", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5339, .Spacer1Type = "110", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5357, .Spacer1Type = "115", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5377, .Spacer1Type = "113", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5392, .Spacer1Type = "111", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5403, .Spacer1Type = "116", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5424, .Spacer1Type = "114", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5440, .Spacer1Type = "112", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5448, .Spacer1Type = "110", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5471, .Spacer1Type = "115", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5488, .Spacer1Type = "113", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5502, .Spacer1Type = "111", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5518, .Spacer1Type = "116", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5536, .Spacer1Type = "114", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5551, .Spacer1Type = "112", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5556, .Spacer1Type = "110", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5584, .Spacer1Type = "115", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5600, .Spacer1Type = "113", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5611, .Spacer1Type = "111", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5632, .Spacer1Type = "116", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5649, .Spacer1Type = "114", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5661, .Spacer1Type = "112", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5664, .Spacer1Type = "110", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5698, .Spacer1Type = "115", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5711, .Spacer1Type = "113", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5720, .Spacer1Type = "111", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5747, .Spacer1Type = "116", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5761, .Spacer1Type = "114", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5771, .Spacer1Type = "112", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5773, .Spacer1Type = "110", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5811, .Spacer1Type = "115", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5822, .Spacer1Type = "113", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5830, .Spacer1Type = "111", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5861, .Spacer1Type = "116", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5873, .Spacer1Type = "114", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5881, .Spacer1Type = "110", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 5882, .Spacer1Type = "112", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5924, .Spacer1Type = "115", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5934, .Spacer1Type = "113", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5939, .Spacer1Type = "111", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 5975, .Spacer1Type = "116", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5986, .Spacer1Type = "114", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5989, .Spacer1Type = "110", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 5992, .Spacer1Type = "112", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6038, .Spacer1Type = "115", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 6045, .Spacer1Type = "113", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6049, .Spacer1Type = "111", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6090, .Spacer1Type = "116", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 6097, .Spacer1Type = "110", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6098, .Spacer1Type = "114", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6103, .Spacer1Type = "112", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6151, .Spacer1Type = "115", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6157, .Spacer1Type = "113", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6158, .Spacer1Type = "111", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6204, .Spacer1Type = "116", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6206, .Spacer1Type = "110", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6211, .Spacer1Type = "114", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6213, .Spacer1Type = "112", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6265, .Spacer1Type = "115", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6267, .Spacer1Type = "111", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6268, .Spacer1Type = "113", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6314, .Spacer1Type = "110", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6319, .Spacer1Type = "116", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6323, .Spacer1Type = "112", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6323, .Spacer1Type = "114", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6377, .Spacer1Type = "111", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6378, .Spacer1Type = "115", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6379, .Spacer1Type = "113", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6422, .Spacer1Type = "110", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 6433, .Spacer1Type = "116", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6434, .Spacer1Type = "112", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6435, .Spacer1Type = "114", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6486, .Spacer1Type = "111", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 6491, .Spacer1Type = "113", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6491, .Spacer1Type = "115", .CarriersQty = 57}
    }

    Private Shared ReadOnly Spacer100Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 196, .Spacer1Type = "84", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 197, .Spacer1Type = "85", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 198, .Spacer1Type = "86", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 199, .Spacer1Type = "87", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 200, .Spacer1Type = "88", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 279, .Spacer1Type = "84", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 281, .Spacer1Type = "85", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 283, .Spacer1Type = "86", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 285, .Spacer1Type = "87", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 287, .Spacer1Type = "88", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 361, .Spacer1Type = "84", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 364, .Spacer1Type = "85", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 367, .Spacer1Type = "86", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 370, .Spacer1Type = "87", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 373, .Spacer1Type = "88", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 444, .Spacer1Type = "84", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 448, .Spacer1Type = "85", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 452, .Spacer1Type = "86", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 456, .Spacer1Type = "87", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 460, .Spacer1Type = "88", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 526, .Spacer1Type = "84", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 531, .Spacer1Type = "85", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 536, .Spacer1Type = "86", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 541, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 546, .Spacer1Type = "88", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 608, .Spacer1Type = "84", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 614, .Spacer1Type = "85", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 620, .Spacer1Type = "86", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 626, .Spacer1Type = "87", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 632, .Spacer1Type = "88", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 691, .Spacer1Type = "84", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 698, .Spacer1Type = "85", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 705, .Spacer1Type = "86", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 712, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 719, .Spacer1Type = "88", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 773, .Spacer1Type = "84", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 781, .Spacer1Type = "85", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 789, .Spacer1Type = "86", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 797, .Spacer1Type = "87", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 805, .Spacer1Type = "88", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 856, .Spacer1Type = "84", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 865, .Spacer1Type = "85", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 874, .Spacer1Type = "86", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 883, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 892, .Spacer1Type = "88", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 938, .Spacer1Type = "84", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 948, .Spacer1Type = "85", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 958, .Spacer1Type = "86", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 968, .Spacer1Type = "87", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 978, .Spacer1Type = "88", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1020, .Spacer1Type = "84", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1031, .Spacer1Type = "85", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1042, .Spacer1Type = "86", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1053, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1064, .Spacer1Type = "88", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1103, .Spacer1Type = "84", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1115, .Spacer1Type = "85", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1127, .Spacer1Type = "86", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1139, .Spacer1Type = "87", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1151, .Spacer1Type = "88", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1185, .Spacer1Type = "84", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1198, .Spacer1Type = "85", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1211, .Spacer1Type = "86", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1224, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1237, .Spacer1Type = "88", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1268, .Spacer1Type = "84", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1282, .Spacer1Type = "85", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1296, .Spacer1Type = "86", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1310, .Spacer1Type = "87", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1324, .Spacer1Type = "88", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1350, .Spacer1Type = "84", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1365, .Spacer1Type = "85", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1380, .Spacer1Type = "86", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1395, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1410, .Spacer1Type = "88", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1432, .Spacer1Type = "84", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1448, .Spacer1Type = "85", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1464, .Spacer1Type = "86", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1480, .Spacer1Type = "87", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1496, .Spacer1Type = "88", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1515, .Spacer1Type = "84", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1532, .Spacer1Type = "85", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1549, .Spacer1Type = "86", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1566, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1583, .Spacer1Type = "88", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1597, .Spacer1Type = "84", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1615, .Spacer1Type = "85", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1633, .Spacer1Type = "86", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1651, .Spacer1Type = "87", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1669, .Spacer1Type = "88", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1680, .Spacer1Type = "84", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1699, .Spacer1Type = "85", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1718, .Spacer1Type = "86", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1737, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1756, .Spacer1Type = "88", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1762, .Spacer1Type = "84", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1782, .Spacer1Type = "85", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1802, .Spacer1Type = "86", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1822, .Spacer1Type = "87", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1842, .Spacer1Type = "88", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1844, .Spacer1Type = "84", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "85", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1886, .Spacer1Type = "86", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1907, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1927, .Spacer1Type = "84", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1928, .Spacer1Type = "88", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1949, .Spacer1Type = "85", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1971, .Spacer1Type = "86", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1993, .Spacer1Type = "87", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2009, .Spacer1Type = "84", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2015, .Spacer1Type = "88", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2032, .Spacer1Type = "85", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2055, .Spacer1Type = "86", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2078, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2092, .Spacer1Type = "84", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2101, .Spacer1Type = "88", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2116, .Spacer1Type = "85", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2140, .Spacer1Type = "86", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2164, .Spacer1Type = "87", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2174, .Spacer1Type = "84", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2188, .Spacer1Type = "88", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2199, .Spacer1Type = "85", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2224, .Spacer1Type = "86", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2249, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2256, .Spacer1Type = "84", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2274, .Spacer1Type = "88", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2282, .Spacer1Type = "85", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2308, .Spacer1Type = "86", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2334, .Spacer1Type = "87", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2339, .Spacer1Type = "84", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2360, .Spacer1Type = "88", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2366, .Spacer1Type = "85", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2393, .Spacer1Type = "86", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2420, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2421, .Spacer1Type = "84", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2447, .Spacer1Type = "88", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2449, .Spacer1Type = "85", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2477, .Spacer1Type = "86", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2504, .Spacer1Type = "84", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2505, .Spacer1Type = "87", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "85", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "88", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2562, .Spacer1Type = "86", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2586, .Spacer1Type = "84", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2591, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2616, .Spacer1Type = "85", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2620, .Spacer1Type = "88", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2646, .Spacer1Type = "86", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2668, .Spacer1Type = "84", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2676, .Spacer1Type = "87", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2699, .Spacer1Type = "85", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2706, .Spacer1Type = "88", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2730, .Spacer1Type = "86", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2751, .Spacer1Type = "84", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2761, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2783, .Spacer1Type = "85", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2792, .Spacer1Type = "88", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2815, .Spacer1Type = "86", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2833, .Spacer1Type = "84", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2847, .Spacer1Type = "87", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2866, .Spacer1Type = "85", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2879, .Spacer1Type = "88", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2899, .Spacer1Type = "86", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2916, .Spacer1Type = "84", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2932, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2950, .Spacer1Type = "85", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2965, .Spacer1Type = "88", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2984, .Spacer1Type = "86", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2998, .Spacer1Type = "84", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3018, .Spacer1Type = "87", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3033, .Spacer1Type = "85", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3052, .Spacer1Type = "88", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3068, .Spacer1Type = "86", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3080, .Spacer1Type = "84", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3103, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3116, .Spacer1Type = "85", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3138, .Spacer1Type = "88", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3152, .Spacer1Type = "86", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3163, .Spacer1Type = "84", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3188, .Spacer1Type = "87", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3200, .Spacer1Type = "85", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3224, .Spacer1Type = "88", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3237, .Spacer1Type = "86", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3245, .Spacer1Type = "84", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3274, .Spacer1Type = "87", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3283, .Spacer1Type = "85", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3311, .Spacer1Type = "88", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3321, .Spacer1Type = "86", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3328, .Spacer1Type = "84", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3359, .Spacer1Type = "87", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3367, .Spacer1Type = "85", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3397, .Spacer1Type = "88", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3406, .Spacer1Type = "86", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3410, .Spacer1Type = "84", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3445, .Spacer1Type = "87", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3450, .Spacer1Type = "85", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3484, .Spacer1Type = "88", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3490, .Spacer1Type = "86", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3492, .Spacer1Type = "84", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3530, .Spacer1Type = "87", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3533, .Spacer1Type = "85", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3570, .Spacer1Type = "88", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3574, .Spacer1Type = "86", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3575, .Spacer1Type = "84", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3615, .Spacer1Type = "87", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3617, .Spacer1Type = "85", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3656, .Spacer1Type = "88", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3657, .Spacer1Type = "84", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3659, .Spacer1Type = "86", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3700, .Spacer1Type = "85", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3701, .Spacer1Type = "87", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3740, .Spacer1Type = "84", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3743, .Spacer1Type = "86", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3743, .Spacer1Type = "88", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3784, .Spacer1Type = "85", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3786, .Spacer1Type = "87", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3822, .Spacer1Type = "84", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3828, .Spacer1Type = "86", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3829, .Spacer1Type = "88", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3867, .Spacer1Type = "85", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3872, .Spacer1Type = "87", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3904, .Spacer1Type = "84", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3912, .Spacer1Type = "86", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3916, .Spacer1Type = "88", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3950, .Spacer1Type = "85", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3957, .Spacer1Type = "87", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3987, .Spacer1Type = "84", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3996, .Spacer1Type = "86", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4002, .Spacer1Type = "88", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 4034, .Spacer1Type = "85", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4042, .Spacer1Type = "87", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4069, .Spacer1Type = "84", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4081, .Spacer1Type = "86", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4088, .Spacer1Type = "88", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4117, .Spacer1Type = "85", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4128, .Spacer1Type = "87", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4152, .Spacer1Type = "84", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4165, .Spacer1Type = "86", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4175, .Spacer1Type = "88", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4201, .Spacer1Type = "85", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4213, .Spacer1Type = "87", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4234, .Spacer1Type = "84", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4250, .Spacer1Type = "86", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4261, .Spacer1Type = "88", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4284, .Spacer1Type = "85", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4299, .Spacer1Type = "87", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4316, .Spacer1Type = "84", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4334, .Spacer1Type = "86", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4348, .Spacer1Type = "88", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4367, .Spacer1Type = "85", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4384, .Spacer1Type = "87", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4399, .Spacer1Type = "84", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4418, .Spacer1Type = "86", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4434, .Spacer1Type = "88", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4451, .Spacer1Type = "85", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4469, .Spacer1Type = "87", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4481, .Spacer1Type = "84", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4503, .Spacer1Type = "86", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4520, .Spacer1Type = "88", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4534, .Spacer1Type = "85", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4555, .Spacer1Type = "87", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4564, .Spacer1Type = "84", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4587, .Spacer1Type = "86", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4607, .Spacer1Type = "88", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4618, .Spacer1Type = "85", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4640, .Spacer1Type = "87", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4646, .Spacer1Type = "84", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4672, .Spacer1Type = "86", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4693, .Spacer1Type = "88", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4701, .Spacer1Type = "85", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4726, .Spacer1Type = "87", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4728, .Spacer1Type = "84", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4756, .Spacer1Type = "86", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4780, .Spacer1Type = "88", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4784, .Spacer1Type = "85", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4811, .Spacer1Type = "84", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4811, .Spacer1Type = "87", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4840, .Spacer1Type = "86", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4866, .Spacer1Type = "88", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4868, .Spacer1Type = "85", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4893, .Spacer1Type = "84", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4896, .Spacer1Type = "87", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4925, .Spacer1Type = "86", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4951, .Spacer1Type = "85", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4952, .Spacer1Type = "88", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4976, .Spacer1Type = "84", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4982, .Spacer1Type = "87", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 5009, .Spacer1Type = "86", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5035, .Spacer1Type = "85", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5039, .Spacer1Type = "88", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 5058, .Spacer1Type = "84", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5067, .Spacer1Type = "87", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5094, .Spacer1Type = "86", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5118, .Spacer1Type = "85", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5125, .Spacer1Type = "88", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5140, .Spacer1Type = "84", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5153, .Spacer1Type = "87", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5178, .Spacer1Type = "86", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5201, .Spacer1Type = "85", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5212, .Spacer1Type = "88", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5223, .Spacer1Type = "84", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5238, .Spacer1Type = "87", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5262, .Spacer1Type = "86", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5285, .Spacer1Type = "85", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5298, .Spacer1Type = "88", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5305, .Spacer1Type = "84", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5323, .Spacer1Type = "87", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5347, .Spacer1Type = "86", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5368, .Spacer1Type = "85", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5384, .Spacer1Type = "88", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5388, .Spacer1Type = "84", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5409, .Spacer1Type = "87", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5431, .Spacer1Type = "86", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5452, .Spacer1Type = "85", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5470, .Spacer1Type = "84", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5471, .Spacer1Type = "88", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5494, .Spacer1Type = "87", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5516, .Spacer1Type = "86", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5535, .Spacer1Type = "85", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5552, .Spacer1Type = "84", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5557, .Spacer1Type = "88", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5580, .Spacer1Type = "87", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5600, .Spacer1Type = "86", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5618, .Spacer1Type = "85", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5635, .Spacer1Type = "84", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5644, .Spacer1Type = "88", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5665, .Spacer1Type = "87", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5684, .Spacer1Type = "86", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5702, .Spacer1Type = "85", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5717, .Spacer1Type = "84", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5730, .Spacer1Type = "88", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5750, .Spacer1Type = "87", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5769, .Spacer1Type = "86", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5785, .Spacer1Type = "85", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5800, .Spacer1Type = "84", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5816, .Spacer1Type = "88", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5836, .Spacer1Type = "87", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5853, .Spacer1Type = "86", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5869, .Spacer1Type = "85", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5882, .Spacer1Type = "84", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5903, .Spacer1Type = "88", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5921, .Spacer1Type = "87", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5938, .Spacer1Type = "86", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5952, .Spacer1Type = "85", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5964, .Spacer1Type = "84", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5989, .Spacer1Type = "88", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 6007, .Spacer1Type = "87", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 6022, .Spacer1Type = "86", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 6035, .Spacer1Type = "85", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6047, .Spacer1Type = "84", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6076, .Spacer1Type = "88", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 6092, .Spacer1Type = "87", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 6106, .Spacer1Type = "86", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6119, .Spacer1Type = "85", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6129, .Spacer1Type = "84", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6162, .Spacer1Type = "88", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 6177, .Spacer1Type = "87", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6191, .Spacer1Type = "86", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6202, .Spacer1Type = "85", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6212, .Spacer1Type = "84", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6248, .Spacer1Type = "88", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6263, .Spacer1Type = "87", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6275, .Spacer1Type = "86", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6286, .Spacer1Type = "85", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6294, .Spacer1Type = "84", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 6335, .Spacer1Type = "88", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6348, .Spacer1Type = "87", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6360, .Spacer1Type = "86", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6369, .Spacer1Type = "85", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 6376, .Spacer1Type = "84", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 6421, .Spacer1Type = "88", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6434, .Spacer1Type = "87", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6444, .Spacer1Type = "86", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 6452, .Spacer1Type = "85", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 6459, .Spacer1Type = "84", .CarriersQty = 78}
    }

    Private Shared ReadOnly Spacer89Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 103, .Spacer1Type = "76", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 175, .Spacer1Type = "74", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 176, .Spacer1Type = "75", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 177, .Spacer1Type = "76", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 178, .Spacer1Type = "77", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 180, .Spacer1Type = "78", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 248, .Spacer1Type = "74", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 250, .Spacer1Type = "75", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 252, .Spacer1Type = "76", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 254, .Spacer1Type = "77", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 256, .Spacer1Type = "78", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 320, .Spacer1Type = "74", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 323, .Spacer1Type = "75", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 326, .Spacer1Type = "76", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 329, .Spacer1Type = "77", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 333, .Spacer1Type = "78", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 393, .Spacer1Type = "74", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 397, .Spacer1Type = "75", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 401, .Spacer1Type = "76", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 405, .Spacer1Type = "77", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 409, .Spacer1Type = "78", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 465, .Spacer1Type = "74", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 470, .Spacer1Type = "75", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 475, .Spacer1Type = "76", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 480, .Spacer1Type = "77", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 486, .Spacer1Type = "78", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 537, .Spacer1Type = "74", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 543, .Spacer1Type = "75", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 549, .Spacer1Type = "76", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 555, .Spacer1Type = "77", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 562, .Spacer1Type = "78", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 610, .Spacer1Type = "74", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 617, .Spacer1Type = "75", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 624, .Spacer1Type = "76", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 631, .Spacer1Type = "77", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 639, .Spacer1Type = "78", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 682, .Spacer1Type = "74", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 690, .Spacer1Type = "75", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 698, .Spacer1Type = "76", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 706, .Spacer1Type = "77", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 715, .Spacer1Type = "78", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 755, .Spacer1Type = "74", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 764, .Spacer1Type = "75", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 773, .Spacer1Type = "76", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 782, .Spacer1Type = "77", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 792, .Spacer1Type = "78", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 827, .Spacer1Type = "74", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 837, .Spacer1Type = "75", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 847, .Spacer1Type = "76", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 857, .Spacer1Type = "77", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 868, .Spacer1Type = "78", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 899, .Spacer1Type = "74", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 910, .Spacer1Type = "75", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 921, .Spacer1Type = "76", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 932, .Spacer1Type = "77", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 945, .Spacer1Type = "78", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 972, .Spacer1Type = "74", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 984, .Spacer1Type = "75", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 996, .Spacer1Type = "76", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1008, .Spacer1Type = "77", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1021, .Spacer1Type = "78", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1044, .Spacer1Type = "74", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1057, .Spacer1Type = "75", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1070, .Spacer1Type = "76", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1083, .Spacer1Type = "77", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1098, .Spacer1Type = "78", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1117, .Spacer1Type = "74", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1131, .Spacer1Type = "75", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "76", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1159, .Spacer1Type = "77", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1174, .Spacer1Type = "78", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1189, .Spacer1Type = "74", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1204, .Spacer1Type = "75", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1219, .Spacer1Type = "76", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1234, .Spacer1Type = "77", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1251, .Spacer1Type = "78", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1261, .Spacer1Type = "74", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1277, .Spacer1Type = "75", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1293, .Spacer1Type = "76", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1309, .Spacer1Type = "77", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1327, .Spacer1Type = "78", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1334, .Spacer1Type = "74", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1351, .Spacer1Type = "75", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1368, .Spacer1Type = "76", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1385, .Spacer1Type = "77", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1404, .Spacer1Type = "78", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1406, .Spacer1Type = "74", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1424, .Spacer1Type = "75", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1442, .Spacer1Type = "76", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1460, .Spacer1Type = "77", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1479, .Spacer1Type = "74", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1480, .Spacer1Type = "78", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1498, .Spacer1Type = "75", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1517, .Spacer1Type = "76", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1536, .Spacer1Type = "77", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1551, .Spacer1Type = "74", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1557, .Spacer1Type = "78", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1571, .Spacer1Type = "75", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1591, .Spacer1Type = "76", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1611, .Spacer1Type = "77", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1623, .Spacer1Type = "74", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1633, .Spacer1Type = "78", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1644, .Spacer1Type = "75", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1665, .Spacer1Type = "76", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1686, .Spacer1Type = "77", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1696, .Spacer1Type = "74", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1710, .Spacer1Type = "78", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1718, .Spacer1Type = "75", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1740, .Spacer1Type = "76", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1762, .Spacer1Type = "77", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1768, .Spacer1Type = "74", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1786, .Spacer1Type = "78", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1791, .Spacer1Type = "75", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1814, .Spacer1Type = "76", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1837, .Spacer1Type = "77", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1841, .Spacer1Type = "74", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1863, .Spacer1Type = "78", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "75", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1889, .Spacer1Type = "76", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1913, .Spacer1Type = "74", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1913, .Spacer1Type = "77", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1938, .Spacer1Type = "75", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1939, .Spacer1Type = "78", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1963, .Spacer1Type = "76", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1985, .Spacer1Type = "74", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1988, .Spacer1Type = "77", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2011, .Spacer1Type = "75", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2016, .Spacer1Type = "78", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2037, .Spacer1Type = "76", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2058, .Spacer1Type = "74", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2063, .Spacer1Type = "77", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2085, .Spacer1Type = "75", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2092, .Spacer1Type = "78", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2112, .Spacer1Type = "76", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2130, .Spacer1Type = "74", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2139, .Spacer1Type = "77", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2158, .Spacer1Type = "75", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2169, .Spacer1Type = "78", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2186, .Spacer1Type = "76", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2203, .Spacer1Type = "74", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2214, .Spacer1Type = "77", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2232, .Spacer1Type = "75", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2245, .Spacer1Type = "78", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2261, .Spacer1Type = "76", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2275, .Spacer1Type = "74", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2290, .Spacer1Type = "77", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2305, .Spacer1Type = "75", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2322, .Spacer1Type = "78", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2335, .Spacer1Type = "76", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2347, .Spacer1Type = "74", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2365, .Spacer1Type = "77", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2378, .Spacer1Type = "75", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2398, .Spacer1Type = "78", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2409, .Spacer1Type = "76", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2420, .Spacer1Type = "74", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2440, .Spacer1Type = "77", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2452, .Spacer1Type = "75", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2475, .Spacer1Type = "78", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2484, .Spacer1Type = "76", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2492, .Spacer1Type = "74", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2516, .Spacer1Type = "77", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2525, .Spacer1Type = "75", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2551, .Spacer1Type = "78", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2558, .Spacer1Type = "76", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2565, .Spacer1Type = "74", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2591, .Spacer1Type = "77", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2599, .Spacer1Type = "75", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2628, .Spacer1Type = "78", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2633, .Spacer1Type = "76", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2637, .Spacer1Type = "74", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2667, .Spacer1Type = "77", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2672, .Spacer1Type = "75", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2704, .Spacer1Type = "78", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2707, .Spacer1Type = "76", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2709, .Spacer1Type = "74", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2742, .Spacer1Type = "77", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2745, .Spacer1Type = "75", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2781, .Spacer1Type = "76", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2781, .Spacer1Type = "78", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2782, .Spacer1Type = "74", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2817, .Spacer1Type = "77", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2819, .Spacer1Type = "75", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2854, .Spacer1Type = "74", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2856, .Spacer1Type = "76", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2857, .Spacer1Type = "78", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2892, .Spacer1Type = "75", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2893, .Spacer1Type = "77", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2927, .Spacer1Type = "74", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2930, .Spacer1Type = "76", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2934, .Spacer1Type = "78", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2966, .Spacer1Type = "75", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2968, .Spacer1Type = "77", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2999, .Spacer1Type = "74", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3005, .Spacer1Type = "76", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3010, .Spacer1Type = "78", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3039, .Spacer1Type = "75", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3044, .Spacer1Type = "77", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3071, .Spacer1Type = "74", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3079, .Spacer1Type = "76", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3087, .Spacer1Type = "78", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3112, .Spacer1Type = "75", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3119, .Spacer1Type = "77", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3144, .Spacer1Type = "74", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3153, .Spacer1Type = "76", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3163, .Spacer1Type = "78", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3186, .Spacer1Type = "75", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3194, .Spacer1Type = "77", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3216, .Spacer1Type = "74", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3228, .Spacer1Type = "76", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3240, .Spacer1Type = "78", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3259, .Spacer1Type = "75", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3270, .Spacer1Type = "77", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3289, .Spacer1Type = "74", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3302, .Spacer1Type = "76", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3316, .Spacer1Type = "78", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3333, .Spacer1Type = "75", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3345, .Spacer1Type = "77", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3361, .Spacer1Type = "74", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3377, .Spacer1Type = "76", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3393, .Spacer1Type = "78", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3406, .Spacer1Type = "75", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3421, .Spacer1Type = "77", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3433, .Spacer1Type = "74", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3451, .Spacer1Type = "76", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3469, .Spacer1Type = "78", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3479, .Spacer1Type = "75", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3496, .Spacer1Type = "77", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3506, .Spacer1Type = "74", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3525, .Spacer1Type = "76", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3546, .Spacer1Type = "78", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3553, .Spacer1Type = "75", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3571, .Spacer1Type = "77", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3578, .Spacer1Type = "74", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3600, .Spacer1Type = "76", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3622, .Spacer1Type = "78", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3626, .Spacer1Type = "75", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3647, .Spacer1Type = "77", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3651, .Spacer1Type = "74", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3674, .Spacer1Type = "76", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3699, .Spacer1Type = "78", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3700, .Spacer1Type = "75", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3722, .Spacer1Type = "77", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3723, .Spacer1Type = "74", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3749, .Spacer1Type = "76", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3773, .Spacer1Type = "75", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3775, .Spacer1Type = "78", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3795, .Spacer1Type = "74", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3798, .Spacer1Type = "77", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3823, .Spacer1Type = "76", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3846, .Spacer1Type = "75", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3852, .Spacer1Type = "78", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3868, .Spacer1Type = "74", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3873, .Spacer1Type = "77", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3897, .Spacer1Type = "76", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3920, .Spacer1Type = "75", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3928, .Spacer1Type = "78", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3940, .Spacer1Type = "74", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3948, .Spacer1Type = "77", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3972, .Spacer1Type = "76", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3993, .Spacer1Type = "75", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4005, .Spacer1Type = "78", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4013, .Spacer1Type = "74", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4024, .Spacer1Type = "77", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4046, .Spacer1Type = "76", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4067, .Spacer1Type = "75", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4081, .Spacer1Type = "78", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4085, .Spacer1Type = "74", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4099, .Spacer1Type = "77", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4121, .Spacer1Type = "76", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4140, .Spacer1Type = "75", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4157, .Spacer1Type = "74", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4158, .Spacer1Type = "78", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4175, .Spacer1Type = "77", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4195, .Spacer1Type = "76", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4213, .Spacer1Type = "75", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4230, .Spacer1Type = "74", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4234, .Spacer1Type = "78", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4250, .Spacer1Type = "77", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4269, .Spacer1Type = "76", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4287, .Spacer1Type = "75", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4302, .Spacer1Type = "74", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4311, .Spacer1Type = "78", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4325, .Spacer1Type = "77", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4344, .Spacer1Type = "76", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4360, .Spacer1Type = "75", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "74", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4387, .Spacer1Type = "78", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4401, .Spacer1Type = "77", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4418, .Spacer1Type = "76", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4434, .Spacer1Type = "75", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4447, .Spacer1Type = "74", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4464, .Spacer1Type = "78", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4476, .Spacer1Type = "77", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4493, .Spacer1Type = "76", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4507, .Spacer1Type = "75", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4519, .Spacer1Type = "74", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4540, .Spacer1Type = "78", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4552, .Spacer1Type = "77", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4567, .Spacer1Type = "76", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4580, .Spacer1Type = "75", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4592, .Spacer1Type = "74", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4617, .Spacer1Type = "78", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4627, .Spacer1Type = "77", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4641, .Spacer1Type = "76", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4654, .Spacer1Type = "75", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4664, .Spacer1Type = "74", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4693, .Spacer1Type = "78", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4702, .Spacer1Type = "77", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4716, .Spacer1Type = "76", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4727, .Spacer1Type = "75", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4737, .Spacer1Type = "74", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4770, .Spacer1Type = "78", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4778, .Spacer1Type = "77", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4790, .Spacer1Type = "76", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4801, .Spacer1Type = "75", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4809, .Spacer1Type = "74", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4846, .Spacer1Type = "78", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4853, .Spacer1Type = "77", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4865, .Spacer1Type = "76", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4874, .Spacer1Type = "75", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4881, .Spacer1Type = "74", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4923, .Spacer1Type = "78", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4929, .Spacer1Type = "77", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4939, .Spacer1Type = "76", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4947, .Spacer1Type = "75", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4954, .Spacer1Type = "74", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 4999, .Spacer1Type = "78", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5004, .Spacer1Type = "77", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5013, .Spacer1Type = "76", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5021, .Spacer1Type = "75", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5026, .Spacer1Type = "74", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5076, .Spacer1Type = "78", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5079, .Spacer1Type = "77", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5088, .Spacer1Type = "76", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5094, .Spacer1Type = "75", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5099, .Spacer1Type = "74", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5152, .Spacer1Type = "78", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5155, .Spacer1Type = "77", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5162, .Spacer1Type = "76", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5168, .Spacer1Type = "75", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5171, .Spacer1Type = "74", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5229, .Spacer1Type = "78", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5230, .Spacer1Type = "77", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5237, .Spacer1Type = "76", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5241, .Spacer1Type = "75", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5243, .Spacer1Type = "74", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5305, .Spacer1Type = "78", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5306, .Spacer1Type = "77", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5311, .Spacer1Type = "76", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5314, .Spacer1Type = "75", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5316, .Spacer1Type = "74", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5381, .Spacer1Type = "77", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5382, .Spacer1Type = "78", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5385, .Spacer1Type = "76", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5388, .Spacer1Type = "74", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5388, .Spacer1Type = "75", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5456, .Spacer1Type = "77", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5458, .Spacer1Type = "78", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5460, .Spacer1Type = "76", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5461, .Spacer1Type = "74", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5461, .Spacer1Type = "75", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5532, .Spacer1Type = "77", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5533, .Spacer1Type = "74", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5534, .Spacer1Type = "76", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5535, .Spacer1Type = "75", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5535, .Spacer1Type = "78", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5605, .Spacer1Type = "74", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5607, .Spacer1Type = "77", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5608, .Spacer1Type = "75", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5609, .Spacer1Type = "76", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5611, .Spacer1Type = "78", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5678, .Spacer1Type = "74", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5681, .Spacer1Type = "75", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5683, .Spacer1Type = "76", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5683, .Spacer1Type = "77", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5688, .Spacer1Type = "78", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5750, .Spacer1Type = "74", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5755, .Spacer1Type = "75", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5757, .Spacer1Type = "76", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5764, .Spacer1Type = "78", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5823, .Spacer1Type = "74", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5828, .Spacer1Type = "75", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5832, .Spacer1Type = "76", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5895, .Spacer1Type = "74", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 5902, .Spacer1Type = "75", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5906, .Spacer1Type = "76", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5975, .Spacer1Type = "75", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 5981, .Spacer1Type = "76", .CarriersQty = 81}
    }

    Private Shared ReadOnly Spacer63Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 77, .Spacer1Type = "51", .CarriersQty = 1},
        New SpacerInfo With {.MaxWidth = 126, .Spacer1Type = "51", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 176, .Spacer1Type = "51", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 225, .Spacer1Type = "51", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 275, .Spacer1Type = "51", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 324, .Spacer1Type = "51", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 373, .Spacer1Type = "51", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 423, .Spacer1Type = "51", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 472, .Spacer1Type = "51", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 522, .Spacer1Type = "51", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 571, .Spacer1Type = "51", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 620, .Spacer1Type = "51", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 670, .Spacer1Type = "51", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 719, .Spacer1Type = "51", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 769, .Spacer1Type = "51", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 818, .Spacer1Type = "51", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 867, .Spacer1Type = "51", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 917, .Spacer1Type = "51", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 966, .Spacer1Type = "51", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1016, .Spacer1Type = "51", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1065, .Spacer1Type = "51", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1114, .Spacer1Type = "51", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1164, .Spacer1Type = "51", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1213, .Spacer1Type = "51", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1263, .Spacer1Type = "51", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1312, .Spacer1Type = "51", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1361, .Spacer1Type = "51", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1411, .Spacer1Type = "51", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 1460, .Spacer1Type = "51", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 1510, .Spacer1Type = "51", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 1559, .Spacer1Type = "51", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 1608, .Spacer1Type = "51", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 1658, .Spacer1Type = "51", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 1707, .Spacer1Type = "51", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 1757, .Spacer1Type = "51", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 1806, .Spacer1Type = "51", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 1855, .Spacer1Type = "51", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 1905, .Spacer1Type = "51", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 1954, .Spacer1Type = "51", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2004, .Spacer1Type = "51", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2053, .Spacer1Type = "51", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 2102, .Spacer1Type = "51", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 2152, .Spacer1Type = "51", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 2201, .Spacer1Type = "51", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 2251, .Spacer1Type = "51", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 2300, .Spacer1Type = "51", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 2349, .Spacer1Type = "51", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 2399, .Spacer1Type = "51", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 2448, .Spacer1Type = "51", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 2498, .Spacer1Type = "51", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 2547, .Spacer1Type = "51", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 2596, .Spacer1Type = "51", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 2646, .Spacer1Type = "51", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 2695, .Spacer1Type = "51", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 2745, .Spacer1Type = "51", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 2794, .Spacer1Type = "51", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 2843, .Spacer1Type = "51", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 2893, .Spacer1Type = "51", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 2942, .Spacer1Type = "51", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 2992, .Spacer1Type = "51", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 3041, .Spacer1Type = "51", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 3090, .Spacer1Type = "51", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 3140, .Spacer1Type = "51", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 3189, .Spacer1Type = "51", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 3239, .Spacer1Type = "51", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 3288, .Spacer1Type = "51", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 3337, .Spacer1Type = "51", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 3387, .Spacer1Type = "51", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 3436, .Spacer1Type = "51", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 3486, .Spacer1Type = "51", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 3535, .Spacer1Type = "51", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 3584, .Spacer1Type = "51", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 3634, .Spacer1Type = "51", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 3683, .Spacer1Type = "51", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 3733, .Spacer1Type = "51", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 3782, .Spacer1Type = "51", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 3831, .Spacer1Type = "51", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 3881, .Spacer1Type = "51", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 3930, .Spacer1Type = "51", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 3980, .Spacer1Type = "51", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 4029, .Spacer1Type = "51", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 4078, .Spacer1Type = "51", .CarriersQty = 82},
        New SpacerInfo With {.MaxWidth = 4128, .Spacer1Type = "51", .CarriersQty = 83},
        New SpacerInfo With {.MaxWidth = 4177, .Spacer1Type = "51", .CarriersQty = 84},
        New SpacerInfo With {.MaxWidth = 4227, .Spacer1Type = "51", .CarriersQty = 85},
        New SpacerInfo With {.MaxWidth = 4276, .Spacer1Type = "51", .CarriersQty = 86},
        New SpacerInfo With {.MaxWidth = 4325, .Spacer1Type = "51", .CarriersQty = 87},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "51", .CarriersQty = 88}
    }

    Private Shared ReadOnly Spacer127Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 555, .Spacer1Type = "106", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 575, .Spacer1Type = "110", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 585, .Spacer1Type = "112", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 595, .Spacer1Type = "114", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 610, .Spacer1Type = "117", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 661, .Spacer1Type = "106", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 685, .Spacer1Type = "110", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 697, .Spacer1Type = "112", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 709, .Spacer1Type = "114", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 727, .Spacer1Type = "117", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 767, .Spacer1Type = "106", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 795, .Spacer1Type = "110", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 809, .Spacer1Type = "112", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 823, .Spacer1Type = "114", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 844, .Spacer1Type = "117", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 873, .Spacer1Type = "106", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 905, .Spacer1Type = "110", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 921, .Spacer1Type = "112", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 937, .Spacer1Type = "114", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 961, .Spacer1Type = "117", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 979, .Spacer1Type = "106", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1015, .Spacer1Type = "110", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1033, .Spacer1Type = "112", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1051, .Spacer1Type = "114", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1078, .Spacer1Type = "117", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1085, .Spacer1Type = "106", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1125, .Spacer1Type = "110", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "112", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1165, .Spacer1Type = "114", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1191, .Spacer1Type = "106", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1195, .Spacer1Type = "117", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1235, .Spacer1Type = "110", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1257, .Spacer1Type = "112", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1279, .Spacer1Type = "114", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1297, .Spacer1Type = "106", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1312, .Spacer1Type = "117", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1345, .Spacer1Type = "110", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1369, .Spacer1Type = "112", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1393, .Spacer1Type = "114", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1403, .Spacer1Type = "106", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1429, .Spacer1Type = "117", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1455, .Spacer1Type = "110", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1481, .Spacer1Type = "112", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1507, .Spacer1Type = "114", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1509, .Spacer1Type = "106", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1546, .Spacer1Type = "117", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1565, .Spacer1Type = "110", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1593, .Spacer1Type = "112", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1615, .Spacer1Type = "106", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1621, .Spacer1Type = "114", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1663, .Spacer1Type = "117", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1675, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1705, .Spacer1Type = "112", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1721, .Spacer1Type = "106", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1735, .Spacer1Type = "114", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1780, .Spacer1Type = "117", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1785, .Spacer1Type = "110", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1817, .Spacer1Type = "112", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1827, .Spacer1Type = "106", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1849, .Spacer1Type = "114", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1895, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1897, .Spacer1Type = "117", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1929, .Spacer1Type = "112", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1933, .Spacer1Type = "106", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1963, .Spacer1Type = "114", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2005, .Spacer1Type = "110", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2014, .Spacer1Type = "117", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2039, .Spacer1Type = "106", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2041, .Spacer1Type = "112", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2077, .Spacer1Type = "114", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2115, .Spacer1Type = "110", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2131, .Spacer1Type = "117", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2145, .Spacer1Type = "106", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2153, .Spacer1Type = "112", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2191, .Spacer1Type = "114", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2225, .Spacer1Type = "110", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2248, .Spacer1Type = "117", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2251, .Spacer1Type = "106", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2265, .Spacer1Type = "112", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2305, .Spacer1Type = "114", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2335, .Spacer1Type = "110", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2357, .Spacer1Type = "106", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2365, .Spacer1Type = "117", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2377, .Spacer1Type = "112", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2419, .Spacer1Type = "114", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2445, .Spacer1Type = "110", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2463, .Spacer1Type = "106", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2482, .Spacer1Type = "117", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2489, .Spacer1Type = "112", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "114", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2555, .Spacer1Type = "110", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2569, .Spacer1Type = "106", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2599, .Spacer1Type = "117", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2601, .Spacer1Type = "112", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2647, .Spacer1Type = "114", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2665, .Spacer1Type = "110", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2675, .Spacer1Type = "106", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2713, .Spacer1Type = "112", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2716, .Spacer1Type = "117", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2761, .Spacer1Type = "114", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2775, .Spacer1Type = "110", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2781, .Spacer1Type = "106", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2825, .Spacer1Type = "112", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2833, .Spacer1Type = "117", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2875, .Spacer1Type = "114", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2885, .Spacer1Type = "110", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2887, .Spacer1Type = "106", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2937, .Spacer1Type = "112", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2950, .Spacer1Type = "117", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2989, .Spacer1Type = "114", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2993, .Spacer1Type = "106", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2995, .Spacer1Type = "110", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3049, .Spacer1Type = "112", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3067, .Spacer1Type = "117", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3099, .Spacer1Type = "106", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3103, .Spacer1Type = "114", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3105, .Spacer1Type = "110", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3161, .Spacer1Type = "112", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3184, .Spacer1Type = "117", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3205, .Spacer1Type = "106", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3215, .Spacer1Type = "110", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3217, .Spacer1Type = "114", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3273, .Spacer1Type = "112", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3301, .Spacer1Type = "117", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3311, .Spacer1Type = "106", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3325, .Spacer1Type = "110", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3331, .Spacer1Type = "114", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3385, .Spacer1Type = "112", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3417, .Spacer1Type = "106", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3418, .Spacer1Type = "117", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3435, .Spacer1Type = "110", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3445, .Spacer1Type = "114", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3497, .Spacer1Type = "112", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3523, .Spacer1Type = "106", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3535, .Spacer1Type = "117", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3545, .Spacer1Type = "110", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3559, .Spacer1Type = "114", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3609, .Spacer1Type = "112", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3629, .Spacer1Type = "106", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3652, .Spacer1Type = "117", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3655, .Spacer1Type = "110", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3673, .Spacer1Type = "114", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3721, .Spacer1Type = "112", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3735, .Spacer1Type = "106", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3765, .Spacer1Type = "110", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3769, .Spacer1Type = "117", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3787, .Spacer1Type = "114", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3833, .Spacer1Type = "112", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3841, .Spacer1Type = "106", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3875, .Spacer1Type = "110", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3886, .Spacer1Type = "117", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3901, .Spacer1Type = "114", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3945, .Spacer1Type = "112", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3947, .Spacer1Type = "106", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3985, .Spacer1Type = "110", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4003, .Spacer1Type = "117", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 4015, .Spacer1Type = "114", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4053, .Spacer1Type = "106", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4057, .Spacer1Type = "112", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4095, .Spacer1Type = "110", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4120, .Spacer1Type = "117", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4129, .Spacer1Type = "114", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4159, .Spacer1Type = "106", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4169, .Spacer1Type = "112", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4205, .Spacer1Type = "110", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4237, .Spacer1Type = "117", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4243, .Spacer1Type = "114", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4265, .Spacer1Type = "106", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4281, .Spacer1Type = "112", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4315, .Spacer1Type = "110", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4354, .Spacer1Type = "117", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4357, .Spacer1Type = "114", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4371, .Spacer1Type = "106", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4393, .Spacer1Type = "112", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4425, .Spacer1Type = "110", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4471, .Spacer1Type = "114", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4471, .Spacer1Type = "117", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4477, .Spacer1Type = "106", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4505, .Spacer1Type = "112", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4535, .Spacer1Type = "110", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4583, .Spacer1Type = "106", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4585, .Spacer1Type = "114", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4588, .Spacer1Type = "117", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4617, .Spacer1Type = "112", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4645, .Spacer1Type = "110", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4689, .Spacer1Type = "106", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4699, .Spacer1Type = "114", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4705, .Spacer1Type = "117", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4729, .Spacer1Type = "112", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4755, .Spacer1Type = "110", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4795, .Spacer1Type = "106", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 4813, .Spacer1Type = "114", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4822, .Spacer1Type = "117", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4841, .Spacer1Type = "112", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4865, .Spacer1Type = "110", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4901, .Spacer1Type = "106", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 4927, .Spacer1Type = "114", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4939, .Spacer1Type = "117", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4953, .Spacer1Type = "112", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4975, .Spacer1Type = "110", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5007, .Spacer1Type = "106", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5041, .Spacer1Type = "114", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5056, .Spacer1Type = "117", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 5065, .Spacer1Type = "112", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5085, .Spacer1Type = "110", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5113, .Spacer1Type = "106", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5155, .Spacer1Type = "114", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5173, .Spacer1Type = "117", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5177, .Spacer1Type = "112", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5195, .Spacer1Type = "110", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5219, .Spacer1Type = "106", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5269, .Spacer1Type = "114", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5289, .Spacer1Type = "112", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5290, .Spacer1Type = "117", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5305, .Spacer1Type = "110", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5325, .Spacer1Type = "106", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5383, .Spacer1Type = "114", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5401, .Spacer1Type = "112", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5407, .Spacer1Type = "117", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5415, .Spacer1Type = "110", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5431, .Spacer1Type = "106", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5497, .Spacer1Type = "114", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5513, .Spacer1Type = "112", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5524, .Spacer1Type = "117", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5525, .Spacer1Type = "110", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5611, .Spacer1Type = "114", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5625, .Spacer1Type = "112", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5635, .Spacer1Type = "110", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5641, .Spacer1Type = "117", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5725, .Spacer1Type = "114", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5737, .Spacer1Type = "112", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5758, .Spacer1Type = "117", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5839, .Spacer1Type = "114", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5875, .Spacer1Type = "117", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5992, .Spacer1Type = "117", .CarriersQty = 51}
    }

    Private Shared ReadOnly Spacer100Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 425, .Spacer1Type = "80", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 435, .Spacer1Type = "82", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 460, .Spacer1Type = "87", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 505, .Spacer1Type = "80", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 517, .Spacer1Type = "82", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 547, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 585, .Spacer1Type = "80", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 599, .Spacer1Type = "82", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 634, .Spacer1Type = "87", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 665, .Spacer1Type = "80", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 681, .Spacer1Type = "82", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 721, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 745, .Spacer1Type = "80", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 763, .Spacer1Type = "82", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 808, .Spacer1Type = "87", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 825, .Spacer1Type = "80", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 845, .Spacer1Type = "82", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 895, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 905, .Spacer1Type = "80", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 927, .Spacer1Type = "82", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 982, .Spacer1Type = "87", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 985, .Spacer1Type = "80", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1009, .Spacer1Type = "82", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1065, .Spacer1Type = "80", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1069, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1091, .Spacer1Type = "82", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "80", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1156, .Spacer1Type = "87", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1173, .Spacer1Type = "82", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1225, .Spacer1Type = "80", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1243, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1255, .Spacer1Type = "82", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1305, .Spacer1Type = "80", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1330, .Spacer1Type = "87", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1337, .Spacer1Type = "82", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1385, .Spacer1Type = "80", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1417, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1419, .Spacer1Type = "82", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1465, .Spacer1Type = "80", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1501, .Spacer1Type = "82", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1504, .Spacer1Type = "87", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1545, .Spacer1Type = "80", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1583, .Spacer1Type = "82", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1591, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1625, .Spacer1Type = "80", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1665, .Spacer1Type = "82", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1678, .Spacer1Type = "87", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1705, .Spacer1Type = "80", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1747, .Spacer1Type = "82", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1765, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1785, .Spacer1Type = "80", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1829, .Spacer1Type = "82", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1852, .Spacer1Type = "87", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "80", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1911, .Spacer1Type = "82", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1939, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1945, .Spacer1Type = "80", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1993, .Spacer1Type = "82", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2025, .Spacer1Type = "80", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2026, .Spacer1Type = "87", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2075, .Spacer1Type = "82", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2105, .Spacer1Type = "80", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2113, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2157, .Spacer1Type = "82", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2185, .Spacer1Type = "80", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "87", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2239, .Spacer1Type = "82", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2265, .Spacer1Type = "80", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2287, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2321, .Spacer1Type = "82", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2345, .Spacer1Type = "80", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2374, .Spacer1Type = "87", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2403, .Spacer1Type = "82", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2425, .Spacer1Type = "80", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2461, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2485, .Spacer1Type = "82", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2505, .Spacer1Type = "80", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2548, .Spacer1Type = "87", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2567, .Spacer1Type = "82", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2585, .Spacer1Type = "80", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2635, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2649, .Spacer1Type = "82", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2665, .Spacer1Type = "80", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2722, .Spacer1Type = "87", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2731, .Spacer1Type = "82", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2745, .Spacer1Type = "80", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2809, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2813, .Spacer1Type = "82", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2825, .Spacer1Type = "80", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2895, .Spacer1Type = "82", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2896, .Spacer1Type = "87", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2905, .Spacer1Type = "80", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2977, .Spacer1Type = "82", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2983, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2985, .Spacer1Type = "80", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3059, .Spacer1Type = "82", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3065, .Spacer1Type = "80", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3070, .Spacer1Type = "87", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3141, .Spacer1Type = "82", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3145, .Spacer1Type = "80", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3157, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3223, .Spacer1Type = "82", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3225, .Spacer1Type = "80", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3244, .Spacer1Type = "87", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3305, .Spacer1Type = "80", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3305, .Spacer1Type = "82", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3331, .Spacer1Type = "87", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3385, .Spacer1Type = "80", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3387, .Spacer1Type = "82", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3418, .Spacer1Type = "87", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3465, .Spacer1Type = "80", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3469, .Spacer1Type = "82", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3505, .Spacer1Type = "87", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3545, .Spacer1Type = "80", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3551, .Spacer1Type = "82", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3592, .Spacer1Type = "87", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3625, .Spacer1Type = "80", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3633, .Spacer1Type = "82", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3679, .Spacer1Type = "87", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3705, .Spacer1Type = "80", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3715, .Spacer1Type = "82", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3766, .Spacer1Type = "87", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3785, .Spacer1Type = "80", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3797, .Spacer1Type = "82", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3853, .Spacer1Type = "87", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3865, .Spacer1Type = "80", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3879, .Spacer1Type = "82", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3940, .Spacer1Type = "87", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3945, .Spacer1Type = "80", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3961, .Spacer1Type = "82", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4025, .Spacer1Type = "80", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4025, .Spacer1Type = "80", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4027, .Spacer1Type = "87", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 4043, .Spacer1Type = "82", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4105, .Spacer1Type = "80", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4114, .Spacer1Type = "87", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4125, .Spacer1Type = "82", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4125, .Spacer1Type = "82", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4201, .Spacer1Type = "87", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4207, .Spacer1Type = "82", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4265, .Spacer1Type = "80", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4288, .Spacer1Type = "87", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4345, .Spacer1Type = "80", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4371, .Spacer1Type = "82", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "87", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "87", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4425, .Spacer1Type = "80", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4453, .Spacer1Type = "82", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4462, .Spacer1Type = "87", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4505, .Spacer1Type = "80", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4535, .Spacer1Type = "82", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4585, .Spacer1Type = "80", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4617, .Spacer1Type = "82", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4636, .Spacer1Type = "87", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4665, .Spacer1Type = "80", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4699, .Spacer1Type = "82", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4723, .Spacer1Type = "87", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4745, .Spacer1Type = "80", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4781, .Spacer1Type = "82", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4810, .Spacer1Type = "87", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4825, .Spacer1Type = "80", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4863, .Spacer1Type = "82", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4897, .Spacer1Type = "87", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4905, .Spacer1Type = "80", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4945, .Spacer1Type = "82", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4984, .Spacer1Type = "87", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4985, .Spacer1Type = "80", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5027, .Spacer1Type = "82", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5065, .Spacer1Type = "80", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5071, .Spacer1Type = "87", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 5109, .Spacer1Type = "82", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5145, .Spacer1Type = "80", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5158, .Spacer1Type = "87", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5191, .Spacer1Type = "82", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5245, .Spacer1Type = "87", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5273, .Spacer1Type = "82", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5332, .Spacer1Type = "87", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5419, .Spacer1Type = "87", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5506, .Spacer1Type = "87", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5593, .Spacer1Type = "87", .CarriersQty = 64}
    }

    Private Shared ReadOnly Spacer89Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 390, .Spacer1Type = "73", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 400, .Spacer1Type = "75", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 463, .Spacer1Type = "73", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 475, .Spacer1Type = "75", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 536, .Spacer1Type = "73", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 550, .Spacer1Type = "75", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 609, .Spacer1Type = "73", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 625, .Spacer1Type = "75", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 682, .Spacer1Type = "73", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 700, .Spacer1Type = "75", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 755, .Spacer1Type = "73", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 775, .Spacer1Type = "75", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 828, .Spacer1Type = "73", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 850, .Spacer1Type = "75", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 901, .Spacer1Type = "73", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 925, .Spacer1Type = "75", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 974, .Spacer1Type = "73", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1000, .Spacer1Type = "75", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1047, .Spacer1Type = "73", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1075, .Spacer1Type = "75", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1120, .Spacer1Type = "73", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1150, .Spacer1Type = "75", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1193, .Spacer1Type = "73", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1225, .Spacer1Type = "75", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1266, .Spacer1Type = "73", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1300, .Spacer1Type = "75", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1339, .Spacer1Type = "73", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1375, .Spacer1Type = "75", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1412, .Spacer1Type = "73", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1450, .Spacer1Type = "75", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1485, .Spacer1Type = "73", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1525, .Spacer1Type = "75", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1558, .Spacer1Type = "73", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1600, .Spacer1Type = "75", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1631, .Spacer1Type = "73", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1675, .Spacer1Type = "75", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1704, .Spacer1Type = "73", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1750, .Spacer1Type = "75", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1777, .Spacer1Type = "73", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1825, .Spacer1Type = "75", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1850, .Spacer1Type = "73", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1900, .Spacer1Type = "75", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1923, .Spacer1Type = "73", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1975, .Spacer1Type = "75", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1996, .Spacer1Type = "73", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2050, .Spacer1Type = "75", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2069, .Spacer1Type = "73", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2125, .Spacer1Type = "75", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2142, .Spacer1Type = "73", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "75", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2215, .Spacer1Type = "73", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2275, .Spacer1Type = "75", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2288, .Spacer1Type = "73", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2350, .Spacer1Type = "75", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2361, .Spacer1Type = "73", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2425, .Spacer1Type = "75", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2434, .Spacer1Type = "73", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2500, .Spacer1Type = "75", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2507, .Spacer1Type = "73", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2575, .Spacer1Type = "75", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2580, .Spacer1Type = "73", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2650, .Spacer1Type = "75", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2653, .Spacer1Type = "73", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2725, .Spacer1Type = "75", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2726, .Spacer1Type = "73", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2799, .Spacer1Type = "73", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2800, .Spacer1Type = "75", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2872, .Spacer1Type = "73", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2875, .Spacer1Type = "75", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2945, .Spacer1Type = "73", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2950, .Spacer1Type = "75", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3018, .Spacer1Type = "73", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3025, .Spacer1Type = "75", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3091, .Spacer1Type = "73", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3100, .Spacer1Type = "75", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3164, .Spacer1Type = "73", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3175, .Spacer1Type = "75", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3237, .Spacer1Type = "73", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3250, .Spacer1Type = "75", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3310, .Spacer1Type = "73", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3325, .Spacer1Type = "75", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3383, .Spacer1Type = "73", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3400, .Spacer1Type = "75", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3456, .Spacer1Type = "73", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3475, .Spacer1Type = "75", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3529, .Spacer1Type = "73", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3550, .Spacer1Type = "75", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3602, .Spacer1Type = "73", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3625, .Spacer1Type = "75", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3675, .Spacer1Type = "73", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3675, .Spacer1Type = "73", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3700, .Spacer1Type = "75", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3748, .Spacer1Type = "73", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3775, .Spacer1Type = "75", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3775, .Spacer1Type = "75", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3850, .Spacer1Type = "75", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3894, .Spacer1Type = "73", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3967, .Spacer1Type = "73", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4000, .Spacer1Type = "75", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4040, .Spacer1Type = "73", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4075, .Spacer1Type = "75", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4113, .Spacer1Type = "73", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4150, .Spacer1Type = "75", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4186, .Spacer1Type = "73", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4225, .Spacer1Type = "75", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4259, .Spacer1Type = "73", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4300, .Spacer1Type = "75", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4332, .Spacer1Type = "73", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "75", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4405, .Spacer1Type = "73", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4450, .Spacer1Type = "75", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4478, .Spacer1Type = "73", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4525, .Spacer1Type = "75", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4551, .Spacer1Type = "73", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4600, .Spacer1Type = "75", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4624, .Spacer1Type = "73", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4675, .Spacer1Type = "75", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4697, .Spacer1Type = "73", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4750, .Spacer1Type = "75", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4825, .Spacer1Type = "75", .CarriersQty = 64}
    }

    Private Shared ReadOnly Spacer63Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 260, .Spacer1Type = "47", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 307, .Spacer1Type = "47", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 354, .Spacer1Type = "47", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 401, .Spacer1Type = "47", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 448, .Spacer1Type = "47", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 495, .Spacer1Type = "47", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 542, .Spacer1Type = "47", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 589, .Spacer1Type = "47", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 636, .Spacer1Type = "47", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 683, .Spacer1Type = "47", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 730, .Spacer1Type = "47", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 777, .Spacer1Type = "47", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 824, .Spacer1Type = "47", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 871, .Spacer1Type = "47", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 918, .Spacer1Type = "47", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 965, .Spacer1Type = "47", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1012, .Spacer1Type = "47", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1059, .Spacer1Type = "47", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1106, .Spacer1Type = "47", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1153, .Spacer1Type = "47", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1200, .Spacer1Type = "47", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1247, .Spacer1Type = "47", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1294, .Spacer1Type = "47", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1341, .Spacer1Type = "47", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 1388, .Spacer1Type = "47", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 1435, .Spacer1Type = "47", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 1482, .Spacer1Type = "47", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 1529, .Spacer1Type = "47", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 1576, .Spacer1Type = "47", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 1623, .Spacer1Type = "47", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 1670, .Spacer1Type = "47", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 1717, .Spacer1Type = "47", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 1764, .Spacer1Type = "47", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 1811, .Spacer1Type = "47", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 1858, .Spacer1Type = "47", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 1905, .Spacer1Type = "47", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 1952, .Spacer1Type = "47", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 1999, .Spacer1Type = "47", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 2046, .Spacer1Type = "47", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 2093, .Spacer1Type = "47", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 2140, .Spacer1Type = "47", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 2187, .Spacer1Type = "47", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 2234, .Spacer1Type = "47", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 2281, .Spacer1Type = "47", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 2328, .Spacer1Type = "47", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 2375, .Spacer1Type = "47", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 2422, .Spacer1Type = "47", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 2469, .Spacer1Type = "47", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 2516, .Spacer1Type = "47", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 2563, .Spacer1Type = "47", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 2610, .Spacer1Type = "47", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 2657, .Spacer1Type = "47", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 2704, .Spacer1Type = "47", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 2751, .Spacer1Type = "47", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 2798, .Spacer1Type = "47", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 2845, .Spacer1Type = "47", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 2892, .Spacer1Type = "47", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 2939, .Spacer1Type = "47", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 2986, .Spacer1Type = "47", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 3033, .Spacer1Type = "47", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 3080, .Spacer1Type = "47", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 3127, .Spacer1Type = "47", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 3174, .Spacer1Type = "47", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 3221, .Spacer1Type = "47", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 3268, .Spacer1Type = "47", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 3315, .Spacer1Type = "47", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 3362, .Spacer1Type = "47", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 3409, .Spacer1Type = "47", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 3456, .Spacer1Type = "47", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 3503, .Spacer1Type = "47", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 3550, .Spacer1Type = "47", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 3597, .Spacer1Type = "47", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 3644, .Spacer1Type = "47", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 3691, .Spacer1Type = "47", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 3738, .Spacer1Type = "47", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 3785, .Spacer1Type = "47", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 3832, .Spacer1Type = "47", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 3879, .Spacer1Type = "47", .CarriersQty = 82},
        New SpacerInfo With {.MaxWidth = 3926, .Spacer1Type = "47", .CarriersQty = 83},
        New SpacerInfo With {.MaxWidth = 3973, .Spacer1Type = "47", .CarriersQty = 84},
        New SpacerInfo With {.MaxWidth = 4020, .Spacer1Type = "47", .CarriersQty = 85},
        New SpacerInfo With {.MaxWidth = 4067, .Spacer1Type = "47", .CarriersQty = 86},
        New SpacerInfo With {.MaxWidth = 4114, .Spacer1Type = "47", .CarriersQty = 87}
    }

    Private Shared ReadOnly Spacer127Louvolite As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 253,  .Spacer1Type = "123", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 255,  .Spacer1Type = "125", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 257,  .Spacer1Type = "127", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 264,  .Spacer1Type = "129", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 363,  .Spacer1Type = "123", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 367,  .Spacer1Type = "125", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 371,  .Spacer1Type = "127", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 380,  .Spacer1Type = "129", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 473,  .Spacer1Type = "123", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 480,  .Spacer1Type = "125", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 486,  .Spacer1Type = "127", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 497,  .Spacer1Type = "129", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 583,  .Spacer1Type = "123", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 592,  .Spacer1Type = "125", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 600,  .Spacer1Type = "127", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 613,  .Spacer1Type = "129", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 694,  .Spacer1Type = "123", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 705,  .Spacer1Type = "125", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 714,  .Spacer1Type = "127", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 729,  .Spacer1Type = "129", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 804,  .Spacer1Type = "123", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 817,  .Spacer1Type = "125", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 829,  .Spacer1Type = "127", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 845,  .Spacer1Type = "129", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 914,  .Spacer1Type = "123", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 930,  .Spacer1Type = "125", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 943,  .Spacer1Type = "127", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 962,  .Spacer1Type = "129", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 1024, .Spacer1Type = "123", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1042, .Spacer1Type = "125", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1057, .Spacer1Type = "127", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1078, .Spacer1Type = "129", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1134, .Spacer1Type = "123", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1155, .Spacer1Type = "125", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1171, .Spacer1Type = "127", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1194, .Spacer1Type = "129", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1244, .Spacer1Type = "123", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1267, .Spacer1Type = "125", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1286, .Spacer1Type = "127", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1310, .Spacer1Type = "129", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1355, .Spacer1Type = "123", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1380, .Spacer1Type = "125", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1400, .Spacer1Type = "127", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1427, .Spacer1Type = "129", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1465, .Spacer1Type = "123", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1492, .Spacer1Type = "125", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1514, .Spacer1Type = "127", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1543, .Spacer1Type = "129", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1575, .Spacer1Type = "123", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1605, .Spacer1Type = "125", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1629, .Spacer1Type = "127", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1659, .Spacer1Type = "129", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1685, .Spacer1Type = "123", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1717, .Spacer1Type = "125", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1743, .Spacer1Type = "127", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1775, .Spacer1Type = "129", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1795, .Spacer1Type = "123", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1830, .Spacer1Type = "125", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1857, .Spacer1Type = "127", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1892, .Spacer1Type = "129", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1905, .Spacer1Type = "123", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1942, .Spacer1Type = "125", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1972, .Spacer1Type = "127", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2008, .Spacer1Type = "129", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2015, .Spacer1Type = "123", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2055, .Spacer1Type = "125", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2086, .Spacer1Type = "127", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2124, .Spacer1Type = "129", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2126, .Spacer1Type = "123", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2167, .Spacer1Type = "125", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "127", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2240, .Spacer1Type = "129", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2236, .Spacer1Type = "123", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2280, .Spacer1Type = "125", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2314, .Spacer1Type = "127", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2357, .Spacer1Type = "129", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2346, .Spacer1Type = "123", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2392, .Spacer1Type = "125", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2429, .Spacer1Type = "127", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2473, .Spacer1Type = "129", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2456, .Spacer1Type = "123", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2505, .Spacer1Type = "125", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2543, .Spacer1Type = "127", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2589, .Spacer1Type = "129", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2566, .Spacer1Type = "123", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2617, .Spacer1Type = "125", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2657, .Spacer1Type = "127", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2705, .Spacer1Type = "129", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2676, .Spacer1Type = "123", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2730, .Spacer1Type = "125", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2772, .Spacer1Type = "127", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2822, .Spacer1Type = "129", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2786, .Spacer1Type = "123", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2842, .Spacer1Type = "125", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2886, .Spacer1Type = "127", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2938, .Spacer1Type = "129", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2897, .Spacer1Type = "123", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2955, .Spacer1Type = "125", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3000, .Spacer1Type = "127", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3054, .Spacer1Type = "129", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3007, .Spacer1Type = "123", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3067, .Spacer1Type = "125", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3115, .Spacer1Type = "127", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3170, .Spacer1Type = "129", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3117, .Spacer1Type = "123", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3180, .Spacer1Type = "125", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3229, .Spacer1Type = "127", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3287, .Spacer1Type = "129", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3227, .Spacer1Type = "123", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3292, .Spacer1Type = "125", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3343, .Spacer1Type = "127", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3403, .Spacer1Type = "129", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3337, .Spacer1Type = "123", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3405, .Spacer1Type = "125", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3457, .Spacer1Type = "127", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3519, .Spacer1Type = "129", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3447, .Spacer1Type = "123", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3517, .Spacer1Type = "125", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3572, .Spacer1Type = "127", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3635, .Spacer1Type = "129", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3558, .Spacer1Type = "123", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3630, .Spacer1Type = "125", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3686, .Spacer1Type = "127", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3752, .Spacer1Type = "129", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3668, .Spacer1Type = "123", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3742, .Spacer1Type = "125", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3800, .Spacer1Type = "127", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3868, .Spacer1Type = "129", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3778, .Spacer1Type = "123", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3855, .Spacer1Type = "125", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3915, .Spacer1Type = "127", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3984, .Spacer1Type = "129", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3888, .Spacer1Type = "123", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3967, .Spacer1Type = "125", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4029, .Spacer1Type = "127", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4100, .Spacer1Type = "129", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3998, .Spacer1Type = "123", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4080, .Spacer1Type = "125", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4143, .Spacer1Type = "127", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4217, .Spacer1Type = "129", .CarriersQty = 36}
    }

    Private Shared ReadOnly Spacer127LouvoliteA As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 253, .Spacer1Type = "123", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 255, .Spacer1Type = "125", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 257, .Spacer1Type = "127", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 264, .Spacer1Type = "129", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 363, .Spacer1Type = "123", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 367, .Spacer1Type = "125", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 371, .Spacer1Type = "127", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 380, .Spacer1Type = "129", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 473, .Spacer1Type = "123", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 480, .Spacer1Type = "125", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 486, .Spacer1Type = "127", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 497, .Spacer1Type = "129", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 583, .Spacer1Type = "123", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 592, .Spacer1Type = "125", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 600, .Spacer1Type = "127", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 613, .Spacer1Type = "129", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 694, .Spacer1Type = "123", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 705, .Spacer1Type = "125", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 714, .Spacer1Type = "127", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 729, .Spacer1Type = "129", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 804, .Spacer1Type = "123", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 817, .Spacer1Type = "125", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 829, .Spacer1Type = "127", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 845, .Spacer1Type = "129", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 914, .Spacer1Type = "123", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 930, .Spacer1Type = "125", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 943, .Spacer1Type = "127", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 962, .Spacer1Type = "129", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 1024, .Spacer1Type = "123", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1042, .Spacer1Type = "125", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1057, .Spacer1Type = "127", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1078, .Spacer1Type = "129", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1134, .Spacer1Type = "123", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1155, .Spacer1Type = "125", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1171, .Spacer1Type = "127", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1194, .Spacer1Type = "129", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1244, .Spacer1Type = "123", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1267, .Spacer1Type = "125", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1286, .Spacer1Type = "127", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1310, .Spacer1Type = "129", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1355, .Spacer1Type = "123", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1380, .Spacer1Type = "125", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1400, .Spacer1Type = "127", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1427, .Spacer1Type = "129", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1465, .Spacer1Type = "123", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1492, .Spacer1Type = "125", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1514, .Spacer1Type = "127", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1543, .Spacer1Type = "129", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1575, .Spacer1Type = "123", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1605, .Spacer1Type = "125", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1629, .Spacer1Type = "127", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1659, .Spacer1Type = "129", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1685, .Spacer1Type = "123", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1717, .Spacer1Type = "125", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1743, .Spacer1Type = "127", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1775, .Spacer1Type = "129", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1795, .Spacer1Type = "123", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1830, .Spacer1Type = "125", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1857, .Spacer1Type = "127", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1892, .Spacer1Type = "129", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1905, .Spacer1Type = "123", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1942, .Spacer1Type = "125", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1972, .Spacer1Type = "127", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2008, .Spacer1Type = "129", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2015, .Spacer1Type = "123", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2055, .Spacer1Type = "125", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2086, .Spacer1Type = "127", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2124, .Spacer1Type = "129", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2126, .Spacer1Type = "123", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2167, .Spacer1Type = "125", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "127", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2240, .Spacer1Type = "129", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2236, .Spacer1Type = "123", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2280, .Spacer1Type = "125", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2314, .Spacer1Type = "127", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2357, .Spacer1Type = "129", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2346, .Spacer1Type = "123", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2392, .Spacer1Type = "125", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2429, .Spacer1Type = "127", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2473, .Spacer1Type = "129", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2456, .Spacer1Type = "123", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2505, .Spacer1Type = "125", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2543, .Spacer1Type = "127", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2589, .Spacer1Type = "129", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2566, .Spacer1Type = "123", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2617, .Spacer1Type = "125", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2657, .Spacer1Type = "127", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2705, .Spacer1Type = "129", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2676, .Spacer1Type = "123", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2730, .Spacer1Type = "125", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2772, .Spacer1Type = "127", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2822, .Spacer1Type = "129", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2786, .Spacer1Type = "123", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2842, .Spacer1Type = "125", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2886, .Spacer1Type = "127", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2938, .Spacer1Type = "129", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2897, .Spacer1Type = "123", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2955, .Spacer1Type = "125", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3000, .Spacer1Type = "127", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3054, .Spacer1Type = "129", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3007, .Spacer1Type = "123", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3067, .Spacer1Type = "125", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3115, .Spacer1Type = "127", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3170, .Spacer1Type = "129", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3117, .Spacer1Type = "123", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3180, .Spacer1Type = "125", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3229, .Spacer1Type = "127", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3287, .Spacer1Type = "129", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3227, .Spacer1Type = "123", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3292, .Spacer1Type = "125", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3343, .Spacer1Type = "127", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3403, .Spacer1Type = "129", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3337, .Spacer1Type = "123", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3405, .Spacer1Type = "125", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3457, .Spacer1Type = "127", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3519, .Spacer1Type = "129", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3447, .Spacer1Type = "123", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3517, .Spacer1Type = "125", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3572, .Spacer1Type = "127", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3635, .Spacer1Type = "129", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3558, .Spacer1Type = "123", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3630, .Spacer1Type = "125", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3686, .Spacer1Type = "127", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3752, .Spacer1Type = "129", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3668, .Spacer1Type = "123", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3742, .Spacer1Type = "125", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3800, .Spacer1Type = "127", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3868, .Spacer1Type = "129", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3778, .Spacer1Type = "123", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3855, .Spacer1Type = "125", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3915, .Spacer1Type = "127", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3984, .Spacer1Type = "129", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3888, .Spacer1Type = "123", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3967, .Spacer1Type = "125", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4029, .Spacer1Type = "127", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4100, .Spacer1Type = "129", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3998, .Spacer1Type = "123", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4080, .Spacer1Type = "125", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4143, .Spacer1Type = "127", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4217, .Spacer1Type = "129", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4108, .Spacer1Type = "123", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4192, .Spacer1Type = "125", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4258, .Spacer1Type = "127", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4333, .Spacer1Type = "129", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4218, .Spacer1Type = "123", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4305, .Spacer1Type = "125", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4372, .Spacer1Type = "127", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4449, .Spacer1Type = "129", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4329, .Spacer1Type = "123", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4417, .Spacer1Type = "125", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4486, .Spacer1Type = "127", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4565, .Spacer1Type = "129", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4439, .Spacer1Type = "123", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4530, .Spacer1Type = "125", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4600, .Spacer1Type = "127", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4682, .Spacer1Type = "129", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4549, .Spacer1Type = "123", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4642, .Spacer1Type = "125", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4715, .Spacer1Type = "127", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4798, .Spacer1Type = "129", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4659, .Spacer1Type = "123", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4755, .Spacer1Type = "125", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4829, .Spacer1Type = "127", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4914, .Spacer1Type = "129", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4769, .Spacer1Type = "123", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4867, .Spacer1Type = "125", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4943, .Spacer1Type = "127", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 5030, .Spacer1Type = "129", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4879, .Spacer1Type = "123", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4980, .Spacer1Type = "125", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5058, .Spacer1Type = "127", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5147, .Spacer1Type = "129", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4989, .Spacer1Type = "123", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5092, .Spacer1Type = "125", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5172, .Spacer1Type = "127", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5263, .Spacer1Type = "129", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5100, .Spacer1Type = "123", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5205, .Spacer1Type = "125", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5286, .Spacer1Type = "127", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5379, .Spacer1Type = "129", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5210, .Spacer1Type = "123", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5317, .Spacer1Type = "125", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5401, .Spacer1Type = "127", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5495, .Spacer1Type = "129", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5320, .Spacer1Type = "123", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5430, .Spacer1Type = "125", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5515, .Spacer1Type = "127", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5612, .Spacer1Type = "129", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5430, .Spacer1Type = "123", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5542, .Spacer1Type = "125", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5629, .Spacer1Type = "127", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5728, .Spacer1Type = "129", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5540, .Spacer1Type = "123", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5655, .Spacer1Type = "125", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5743, .Spacer1Type = "127", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5650, .Spacer1Type = "123", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5767, .Spacer1Type = "125", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5761, .Spacer1Type = "123", .CarriersQty = 52}
    }

    Private Shared ReadOnly Spacer127LouvoliteB As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 234, .Spacer1Type = "123", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 236, .Spacer1Type = "125", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 238, .Spacer1Type = "127", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 240, .Spacer1Type = "129", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 454, .Spacer1Type = "123", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 458, .Spacer1Type = "125", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 467, .Spacer1Type = "127", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 471, .Spacer1Type = "129", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 674, .Spacer1Type = "123", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 683, .Spacer1Type = "125", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 696, .Spacer1Type = "127", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 703, .Spacer1Type = "129", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 894, .Spacer1Type = "123", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 907, .Spacer1Type = "125", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 924, .Spacer1Type = "127", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 935, .Spacer1Type = "129", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 1114, .Spacer1Type = "123", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1132, .Spacer1Type = "125", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1153, .Spacer1Type = "127", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1168, .Spacer1Type = "129", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1334, .Spacer1Type = "123", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1357, .Spacer1Type = "125", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1382, .Spacer1Type = "127", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1400, .Spacer1Type = "129", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1554, .Spacer1Type = "123", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1581, .Spacer1Type = "125", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1610, .Spacer1Type = "127", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1633, .Spacer1Type = "129", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1774, .Spacer1Type = "123", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1806, .Spacer1Type = "125", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1839, .Spacer1Type = "127", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "129", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1994, .Spacer1Type = "123", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2031, .Spacer1Type = "125", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2067, .Spacer1Type = "127", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2097, .Spacer1Type = "129", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2214, .Spacer1Type = "123", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2256, .Spacer1Type = "125", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2296, .Spacer1Type = "127", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2330, .Spacer1Type = "129", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2434, .Spacer1Type = "123", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2480, .Spacer1Type = "125", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2525, .Spacer1Type = "127", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2562, .Spacer1Type = "129", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2654, .Spacer1Type = "123", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2705, .Spacer1Type = "125", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2753, .Spacer1Type = "127", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2795, .Spacer1Type = "129", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2874, .Spacer1Type = "123", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2930, .Spacer1Type = "125", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2982, .Spacer1Type = "127", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3027, .Spacer1Type = "129", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3094, .Spacer1Type = "123", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3154, .Spacer1Type = "125", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3210, .Spacer1Type = "127", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3259, .Spacer1Type = "129", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3314, .Spacer1Type = "123", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3379, .Spacer1Type = "125", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3439, .Spacer1Type = "127", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3492, .Spacer1Type = "129", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3534, .Spacer1Type = "123", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3604, .Spacer1Type = "125", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3668, .Spacer1Type = "127", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3724, .Spacer1Type = "129", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3754, .Spacer1Type = "123", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3828, .Spacer1Type = "125", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3896, .Spacer1Type = "127", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3957, .Spacer1Type = "129", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3974, .Spacer1Type = "123", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4053, .Spacer1Type = "125", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4125, .Spacer1Type = "127", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4189, .Spacer1Type = "129", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4194, .Spacer1Type = "123", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4278, .Spacer1Type = "125", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4353, .Spacer1Type = "127", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4421, .Spacer1Type = "129", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4414, .Spacer1Type = "123", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4502, .Spacer1Type = "125", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4582, .Spacer1Type = "127", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4654, .Spacer1Type = "129", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4634, .Spacer1Type = "123", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4727, .Spacer1Type = "125", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4811, .Spacer1Type = "127", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4886, .Spacer1Type = "129", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4854, .Spacer1Type = "123", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4952, .Spacer1Type = "125", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5039, .Spacer1Type = "127", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5119, .Spacer1Type = "129", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5074, .Spacer1Type = "123", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5176, .Spacer1Type = "125", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5268, .Spacer1Type = "127", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5351, .Spacer1Type = "129", .CarriersQty = 46}
    }

    Private Shared ReadOnly Spacer89Louvolite As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 178, .Spacer1Type = "83", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 180, .Spacer1Type = "85", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 182, .Spacer1Type = "87", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 184, .Spacer1Type = "89", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 248, .Spacer1Type = "83", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 252, .Spacer1Type = "85", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 256, .Spacer1Type = "87", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 260, .Spacer1Type = "89", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 317, .Spacer1Type = "83", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 323, .Spacer1Type = "85", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 330, .Spacer1Type = "87", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 336, .Spacer1Type = "89", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 387, .Spacer1Type = "83", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 395, .Spacer1Type = "85", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 404, .Spacer1Type = "87", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 412, .Spacer1Type = "89", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 456, .Spacer1Type = "83", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 467, .Spacer1Type = "85", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 478, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 488, .Spacer1Type = "89", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 526, .Spacer1Type = "83", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 539, .Spacer1Type = "85", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 553, .Spacer1Type = "87", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 564, .Spacer1Type = "89", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 595, .Spacer1Type = "83", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 610, .Spacer1Type = "85", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 627, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 640, .Spacer1Type = "89", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 665, .Spacer1Type = "83", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 682, .Spacer1Type = "85", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 701, .Spacer1Type = "87", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 716, .Spacer1Type = "89", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 734, .Spacer1Type = "83", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 754, .Spacer1Type = "85", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 775, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 792, .Spacer1Type = "89", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 804, .Spacer1Type = "83", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 825, .Spacer1Type = "85", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 849, .Spacer1Type = "87", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 868, .Spacer1Type = "89", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 873, .Spacer1Type = "83", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 897, .Spacer1Type = "85", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 923, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 944, .Spacer1Type = "89", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 943, .Spacer1Type = "83", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 969, .Spacer1Type = "85", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 997, .Spacer1Type = "87", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1020, .Spacer1Type = "89", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1012, .Spacer1Type = "83", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1040, .Spacer1Type = "85", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1071, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1096, .Spacer1Type = "89", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1082, .Spacer1Type = "83", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1112, .Spacer1Type = "85", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "87", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1172, .Spacer1Type = "89", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1151, .Spacer1Type = "83", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1184, .Spacer1Type = "85", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1219, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1248, .Spacer1Type = "89", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1221, .Spacer1Type = "83", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1256, .Spacer1Type = "85", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1294, .Spacer1Type = "87", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1324, .Spacer1Type = "89", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1290, .Spacer1Type = "83", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1327, .Spacer1Type = "85", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1368, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1400, .Spacer1Type = "89", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1360, .Spacer1Type = "83", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1399, .Spacer1Type = "85", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1442, .Spacer1Type = "87", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1476, .Spacer1Type = "89", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1429, .Spacer1Type = "83", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1471, .Spacer1Type = "85", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1516, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1552, .Spacer1Type = "89", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1499, .Spacer1Type = "83", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1542, .Spacer1Type = "85", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1590, .Spacer1Type = "87", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1628, .Spacer1Type = "89", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1568, .Spacer1Type = "83", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1614, .Spacer1Type = "85", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1664, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1704, .Spacer1Type = "89", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1638, .Spacer1Type = "83", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1686, .Spacer1Type = "85", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1738, .Spacer1Type = "87", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1780, .Spacer1Type = "89", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1707, .Spacer1Type = "83", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1757, .Spacer1Type = "85", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1812, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1856, .Spacer1Type = "89", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1777, .Spacer1Type = "83", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1829, .Spacer1Type = "85", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1886, .Spacer1Type = "87", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1932, .Spacer1Type = "89", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1846, .Spacer1Type = "83", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1901, .Spacer1Type = "85", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1960, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2008, .Spacer1Type = "89", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1916, .Spacer1Type = "83", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1973, .Spacer1Type = "85", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2035, .Spacer1Type = "87", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2084, .Spacer1Type = "89", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1985, .Spacer1Type = "83", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2044, .Spacer1Type = "85", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2109, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2160, .Spacer1Type = "89", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2055, .Spacer1Type = "83", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2116, .Spacer1Type = "85", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2183, .Spacer1Type = "87", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2236, .Spacer1Type = "89", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2124, .Spacer1Type = "83", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2188, .Spacer1Type = "85", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2257, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2312, .Spacer1Type = "89", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2194, .Spacer1Type = "83", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2259, .Spacer1Type = "85", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2331, .Spacer1Type = "87", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2388, .Spacer1Type = "89", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2263, .Spacer1Type = "83", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2331, .Spacer1Type = "85", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2405, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2464, .Spacer1Type = "89", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2333, .Spacer1Type = "83", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2403, .Spacer1Type = "85", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2479, .Spacer1Type = "87", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2540, .Spacer1Type = "89", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2402, .Spacer1Type = "83", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2474, .Spacer1Type = "85", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2553, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2616, .Spacer1Type = "89", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2472, .Spacer1Type = "83", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2546, .Spacer1Type = "85", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2627, .Spacer1Type = "87", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2692, .Spacer1Type = "89", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2541, .Spacer1Type = "83", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2618, .Spacer1Type = "85", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2701, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2768, .Spacer1Type = "89", .CarriersQty = 36}
    }

    Private Shared ReadOnly Spacer89LouvoliteA As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 178, .Spacer1Type = "83", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 180, .Spacer1Type = "85", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 182, .Spacer1Type = "87", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 184, .Spacer1Type = "89", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 248, .Spacer1Type = "83", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 252, .Spacer1Type = "85", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 256, .Spacer1Type = "87", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 260, .Spacer1Type = "89", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 317, .Spacer1Type = "83", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 323, .Spacer1Type = "85", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 330, .Spacer1Type = "87", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 336, .Spacer1Type = "89", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 387, .Spacer1Type = "83", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 395, .Spacer1Type = "85", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 404, .Spacer1Type = "87", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 412, .Spacer1Type = "89", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 456, .Spacer1Type = "83", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 467, .Spacer1Type = "85", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 478, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 488, .Spacer1Type = "89", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 526, .Spacer1Type = "83", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 539, .Spacer1Type = "85", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 553, .Spacer1Type = "87", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 564, .Spacer1Type = "89", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 595, .Spacer1Type = "83", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 610, .Spacer1Type = "85", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 627, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 640, .Spacer1Type = "89", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 665, .Spacer1Type = "83", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 682, .Spacer1Type = "85", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 701, .Spacer1Type = "87", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 716, .Spacer1Type = "89", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 734, .Spacer1Type = "83", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 754, .Spacer1Type = "85", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 775, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 792, .Spacer1Type = "89", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 804, .Spacer1Type = "83", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 825, .Spacer1Type = "85", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 849, .Spacer1Type = "87", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 868, .Spacer1Type = "89", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 873, .Spacer1Type = "83", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 897, .Spacer1Type = "85", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 923, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 944, .Spacer1Type = "89", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 943, .Spacer1Type = "83", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 969, .Spacer1Type = "85", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 997, .Spacer1Type = "87", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1020, .Spacer1Type = "89", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1012, .Spacer1Type = "83", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1040, .Spacer1Type = "85", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1071, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1096, .Spacer1Type = "89", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1082, .Spacer1Type = "83", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1112, .Spacer1Type = "85", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "87", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1172, .Spacer1Type = "89", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1151, .Spacer1Type = "83", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1184, .Spacer1Type = "85", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1219, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1248, .Spacer1Type = "89", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1221, .Spacer1Type = "83", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1256, .Spacer1Type = "85", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1294, .Spacer1Type = "87", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1324, .Spacer1Type = "89", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1290, .Spacer1Type = "83", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1327, .Spacer1Type = "85", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1368, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1400, .Spacer1Type = "89", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1360, .Spacer1Type = "83", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1399, .Spacer1Type = "85", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1442, .Spacer1Type = "87", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1476, .Spacer1Type = "89", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1429, .Spacer1Type = "83", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1471, .Spacer1Type = "85", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1516, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1552, .Spacer1Type = "89", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1499, .Spacer1Type = "83", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1542, .Spacer1Type = "85", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1590, .Spacer1Type = "87", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1628, .Spacer1Type = "89", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1568, .Spacer1Type = "83", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1614, .Spacer1Type = "85", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1664, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1704, .Spacer1Type = "89", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1638, .Spacer1Type = "83", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1686, .Spacer1Type = "85", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1738, .Spacer1Type = "87", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1780, .Spacer1Type = "89", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1707, .Spacer1Type = "83", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1757, .Spacer1Type = "85", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1812, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1856, .Spacer1Type = "89", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1777, .Spacer1Type = "83", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1829, .Spacer1Type = "85", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1886, .Spacer1Type = "87", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1932, .Spacer1Type = "89", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1846, .Spacer1Type = "83", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1901, .Spacer1Type = "85", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1960, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2008, .Spacer1Type = "89", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1916, .Spacer1Type = "83", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1973, .Spacer1Type = "85", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2035, .Spacer1Type = "87", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2084, .Spacer1Type = "89", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1985, .Spacer1Type = "83", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2044, .Spacer1Type = "85", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2109, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2160, .Spacer1Type = "89", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2055, .Spacer1Type = "83", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2116, .Spacer1Type = "85", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2183, .Spacer1Type = "87", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2236, .Spacer1Type = "89", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2124, .Spacer1Type = "83", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2188, .Spacer1Type = "85", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2257, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2312, .Spacer1Type = "89", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2194, .Spacer1Type = "83", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2259, .Spacer1Type = "85", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2331, .Spacer1Type = "87", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2388, .Spacer1Type = "89", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2263, .Spacer1Type = "83", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2331, .Spacer1Type = "85", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2405, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2464, .Spacer1Type = "89", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2333, .Spacer1Type = "83", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2403, .Spacer1Type = "85", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2479, .Spacer1Type = "87", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2540, .Spacer1Type = "89", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2402, .Spacer1Type = "83", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2474, .Spacer1Type = "85", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2553, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2616, .Spacer1Type = "89", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2472, .Spacer1Type = "83", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2546, .Spacer1Type = "85", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2627, .Spacer1Type = "87", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2692, .Spacer1Type = "89", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2541, .Spacer1Type = "83", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2618, .Spacer1Type = "85", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2701, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2768, .Spacer1Type = "89", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2611, .Spacer1Type = "83", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2690, .Spacer1Type = "85", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2776, .Spacer1Type = "87", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2844, .Spacer1Type = "89", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2680, .Spacer1Type = "83", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2761, .Spacer1Type = "85", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2850, .Spacer1Type = "87", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2920, .Spacer1Type = "89", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2750, .Spacer1Type = "83", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2833, .Spacer1Type = "85", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2924, .Spacer1Type = "87", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2996, .Spacer1Type = "89", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2819, .Spacer1Type = "83", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2905, .Spacer1Type = "85", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2998, .Spacer1Type = "87", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3072, .Spacer1Type = "89", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2889, .Spacer1Type = "83", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 2976, .Spacer1Type = "85", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3072, .Spacer1Type = "87", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3148, .Spacer1Type = "89", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 2958, .Spacer1Type = "83", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3048, .Spacer1Type = "85", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3146, .Spacer1Type = "87", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3224, .Spacer1Type = "89", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3028, .Spacer1Type = "83", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3120, .Spacer1Type = "85", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3220, .Spacer1Type = "87", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3300, .Spacer1Type = "89", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3097, .Spacer1Type = "83", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3191, .Spacer1Type = "85", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3294, .Spacer1Type = "87", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3376, .Spacer1Type = "89", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3167, .Spacer1Type = "83", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3263, .Spacer1Type = "85", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3368, .Spacer1Type = "87", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3452, .Spacer1Type = "89", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3236, .Spacer1Type = "83", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3335, .Spacer1Type = "85", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3442, .Spacer1Type = "87", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3528, .Spacer1Type = "89", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3306, .Spacer1Type = "83", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3407, .Spacer1Type = "85", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3517, .Spacer1Type = "87", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3604, .Spacer1Type = "89", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3375, .Spacer1Type = "83", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3478, .Spacer1Type = "85", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3591, .Spacer1Type = "87", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3680, .Spacer1Type = "89", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3445, .Spacer1Type = "83", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3550, .Spacer1Type = "85", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3665, .Spacer1Type = "87", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3756, .Spacer1Type = "89", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3514, .Spacer1Type = "83", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3622, .Spacer1Type = "85", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3739, .Spacer1Type = "87", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3832, .Spacer1Type = "89", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3584, .Spacer1Type = "83", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3693, .Spacer1Type = "85", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3813, .Spacer1Type = "87", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3908, .Spacer1Type = "89", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3654, .Spacer1Type = "83", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3765, .Spacer1Type = "85", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3887, .Spacer1Type = "87", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3984, .Spacer1Type = "89", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3723, .Spacer1Type = "83", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3837, .Spacer1Type = "85", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3961, .Spacer1Type = "87", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4060, .Spacer1Type = "89", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3793, .Spacer1Type = "83", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3908, .Spacer1Type = "85", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4035, .Spacer1Type = "87", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4136, .Spacer1Type = "89", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3862, .Spacer1Type = "83", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 3980, .Spacer1Type = "85", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4109, .Spacer1Type = "87", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4212, .Spacer1Type = "89", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 3932, .Spacer1Type = "83", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4052, .Spacer1Type = "85", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4183, .Spacer1Type = "87", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4288, .Spacer1Type = "89", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4001, .Spacer1Type = "83", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4124, .Spacer1Type = "85", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4258, .Spacer1Type = "87", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4364, .Spacer1Type = "89", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4071, .Spacer1Type = "83", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4195, .Spacer1Type = "85", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4332, .Spacer1Type = "87", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4440, .Spacer1Type = "89", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4140, .Spacer1Type = "83", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4267, .Spacer1Type = "85", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4406, .Spacer1Type = "87", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4516, .Spacer1Type = "89", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4210, .Spacer1Type = "83", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4339, .Spacer1Type = "85", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4480, .Spacer1Type = "87", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4592, .Spacer1Type = "89", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4279, .Spacer1Type = "83", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4410, .Spacer1Type = "85", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4554, .Spacer1Type = "87", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4668, .Spacer1Type = "89", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4349, .Spacer1Type = "83", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4482, .Spacer1Type = "85", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4628, .Spacer1Type = "87", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4744, .Spacer1Type = "89", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4418, .Spacer1Type = "83", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4554, .Spacer1Type = "85", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4702, .Spacer1Type = "87", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4820, .Spacer1Type = "89", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4488, .Spacer1Type = "83", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4625, .Spacer1Type = "85", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4776, .Spacer1Type = "87", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4896, .Spacer1Type = "89", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4557, .Spacer1Type = "83", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4697, .Spacer1Type = "85", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4850, .Spacer1Type = "87", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4972, .Spacer1Type = "89", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4627, .Spacer1Type = "83", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4769, .Spacer1Type = "85", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4924, .Spacer1Type = "87", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5048, .Spacer1Type = "89", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4696, .Spacer1Type = "83", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4840, .Spacer1Type = "85", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4999, .Spacer1Type = "87", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5124, .Spacer1Type = "89", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4766, .Spacer1Type = "83", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 4912, .Spacer1Type = "85", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5073, .Spacer1Type = "87", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5200, .Spacer1Type = "89", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 4835, .Spacer1Type = "83", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 4984, .Spacer1Type = "85", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5147, .Spacer1Type = "87", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5276, .Spacer1Type = "89", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 4905, .Spacer1Type = "83", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5056, .Spacer1Type = "85", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5221, .Spacer1Type = "87", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5352, .Spacer1Type = "89", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 4974, .Spacer1Type = "83", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5127, .Spacer1Type = "85", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5295, .Spacer1Type = "87", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5428, .Spacer1Type = "89", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5044, .Spacer1Type = "83", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5199, .Spacer1Type = "85", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5369, .Spacer1Type = "87", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5504, .Spacer1Type = "89", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5113, .Spacer1Type = "83", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5271, .Spacer1Type = "85", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5443, .Spacer1Type = "87", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5580, .Spacer1Type = "89", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5183, .Spacer1Type = "83", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5342, .Spacer1Type = "85", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5517, .Spacer1Type = "87", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5656, .Spacer1Type = "89", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5252, .Spacer1Type = "83", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5414, .Spacer1Type = "85", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5591, .Spacer1Type = "87", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5732, .Spacer1Type = "89", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5322, .Spacer1Type = "83", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5486, .Spacer1Type = "85", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5665, .Spacer1Type = "87", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5391, .Spacer1Type = "83", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5557, .Spacer1Type = "85", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5740, .Spacer1Type = "85", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5461, .Spacer1Type = "83", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5629, .Spacer1Type = "85", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5530, .Spacer1Type = "83", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5701, .Spacer1Type = "85", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5600, .Spacer1Type = "83", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5773, .Spacer1Type = "85", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5669, .Spacer1Type = "83", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 5739, .Spacer1Type = "83", .CarriersQty = 82}
    }

    Private Shared ReadOnly Spacer89LouvoliteB As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 158, .Spacer1Type = "83", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 160, .Spacer1Type = "85", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 162, .Spacer1Type = "87", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 164, .Spacer1Type = "89", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 297, .Spacer1Type = "83", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 304, .Spacer1Type = "85", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 310, .Spacer1Type = "87", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 316, .Spacer1Type = "89", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 437, .Spacer1Type = "83", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 447, .Spacer1Type = "85", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 458, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 469, .Spacer1Type = "89", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 576, .Spacer1Type = "83", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 591, .Spacer1Type = "85", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 607, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 621, .Spacer1Type = "89", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 715, .Spacer1Type = "83", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 735, .Spacer1Type = "85", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 755, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 774, .Spacer1Type = "89", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 855, .Spacer1Type = "83", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 878, .Spacer1Type = "85", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 903, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 927, .Spacer1Type = "89", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 994, .Spacer1Type = "83", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1022, .Spacer1Type = "85", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1051, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1079, .Spacer1Type = "89", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1133, .Spacer1Type = "83", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1166, .Spacer1Type = "85", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1199, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1232, .Spacer1Type = "89", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1272, .Spacer1Type = "83", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1309, .Spacer1Type = "85", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1348, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1384, .Spacer1Type = "89", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1412, .Spacer1Type = "83", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1453, .Spacer1Type = "85", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1496, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1537, .Spacer1Type = "89", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1551, .Spacer1Type = "83", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1597, .Spacer1Type = "85", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1644, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1690, .Spacer1Type = "89", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1690, .Spacer1Type = "83", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1740, .Spacer1Type = "85", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1792, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1842, .Spacer1Type = "89", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1830, .Spacer1Type = "83", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1884, .Spacer1Type = "85", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1940, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1995, .Spacer1Type = "89", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1969, .Spacer1Type = "83", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2028, .Spacer1Type = "85", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2089, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2147, .Spacer1Type = "89", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2108, .Spacer1Type = "83", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2172, .Spacer1Type = "85", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2237, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2300, .Spacer1Type = "89", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2248, .Spacer1Type = "83", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2315, .Spacer1Type = "85", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2385, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2453, .Spacer1Type = "89", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2387, .Spacer1Type = "83", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2459, .Spacer1Type = "85", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2605, .Spacer1Type = "89", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2526, .Spacer1Type = "83", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2603, .Spacer1Type = "85", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2681, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2758, .Spacer1Type = "89", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2665, .Spacer1Type = "83", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2746, .Spacer1Type = "85", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2830, .Spacer1Type = "87", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2910, .Spacer1Type = "89", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2805, .Spacer1Type = "83", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2890, .Spacer1Type = "85", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2978, .Spacer1Type = "87", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3063, .Spacer1Type = "89", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2944, .Spacer1Type = "83", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3034, .Spacer1Type = "85", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3126, .Spacer1Type = "87", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3216, .Spacer1Type = "89", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3083, .Spacer1Type = "83", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3177, .Spacer1Type = "85", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3274, .Spacer1Type = "87", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3368, .Spacer1Type = "89", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3223, .Spacer1Type = "83", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3321, .Spacer1Type = "85", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3422, .Spacer1Type = "87", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3521, .Spacer1Type = "89", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3362, .Spacer1Type = "83", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3465, .Spacer1Type = "85", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3571, .Spacer1Type = "87", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3673, .Spacer1Type = "89", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3501, .Spacer1Type = "83", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3608, .Spacer1Type = "85", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3719, .Spacer1Type = "87", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3826, .Spacer1Type = "89", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3641, .Spacer1Type = "83", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3752, .Spacer1Type = "85", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3867, .Spacer1Type = "87", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3979, .Spacer1Type = "89", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3780, .Spacer1Type = "83", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3896, .Spacer1Type = "85", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4015, .Spacer1Type = "87", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4131, .Spacer1Type = "89", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3919, .Spacer1Type = "83", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4039, .Spacer1Type = "85", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4163, .Spacer1Type = "87", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4284, .Spacer1Type = "89", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4058, .Spacer1Type = "83", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4183, .Spacer1Type = "85", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4312, .Spacer1Type = "87", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4436, .Spacer1Type = "89", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4198, .Spacer1Type = "83", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4327, .Spacer1Type = "85", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4460, .Spacer1Type = "87", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4589, .Spacer1Type = "89", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4337, .Spacer1Type = "83", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4470, .Spacer1Type = "85", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4608, .Spacer1Type = "87", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4742, .Spacer1Type = "89", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4476, .Spacer1Type = "83", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4614, .Spacer1Type = "85", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4756, .Spacer1Type = "87", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4894, .Spacer1Type = "89", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4616, .Spacer1Type = "83", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4758, .Spacer1Type = "85", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4904, .Spacer1Type = "87", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5047, .Spacer1Type = "89", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4755, .Spacer1Type = "83", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 4901, .Spacer1Type = "85", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5053, .Spacer1Type = "87", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5199, .Spacer1Type = "89", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 4894, .Spacer1Type = "83", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5045, .Spacer1Type = "85", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5201, .Spacer1Type = "87", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5352, .Spacer1Type = "89", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5034, .Spacer1Type = "83", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5189, .Spacer1Type = "85", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5349, .Spacer1Type = "87", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5505, .Spacer1Type = "89", .CarriersQty = 72}
    }

    Private Shared ReadOnly Spacer127Javaline As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 591,  .Spacer1Type = "115", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 705,  .Spacer1Type = "115", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 818,  .Spacer1Type = "115", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 931,  .Spacer1Type = "115", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 1044, .Spacer1Type = "115", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1157, .Spacer1Type = "115", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1271, .Spacer1Type = "115", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1384, .Spacer1Type = "115", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1497, .Spacer1Type = "115", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1610, .Spacer1Type = "115", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1723, .Spacer1Type = "115", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1837, .Spacer1Type = "115", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1950, .Spacer1Type = "115", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2063, .Spacer1Type = "115", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2176, .Spacer1Type = "115", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2289, .Spacer1Type = "115", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2403, .Spacer1Type = "115", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2516, .Spacer1Type = "115", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2629, .Spacer1Type = "115", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2742, .Spacer1Type = "115", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2855, .Spacer1Type = "115", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2969, .Spacer1Type = "115", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3082, .Spacer1Type = "115", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3195, .Spacer1Type = "115", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3308, .Spacer1Type = "115", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3421, .Spacer1Type = "115", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3535, .Spacer1Type = "115", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3648, .Spacer1Type = "115", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3761, .Spacer1Type = "115", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3874, .Spacer1Type = "115", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3987, .Spacer1Type = "115", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4109, .Spacer1Type = "115", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4223, .Spacer1Type = "115", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4337, .Spacer1Type = "115", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4451, .Spacer1Type = "115", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4565, .Spacer1Type = "115", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4679, .Spacer1Type = "115", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4793, .Spacer1Type = "115", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4907, .Spacer1Type = "115", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 5021, .Spacer1Type = "115", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5135, .Spacer1Type = "115", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5249, .Spacer1Type = "115", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5363, .Spacer1Type = "115", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5477, .Spacer1Type = "115", .CarriersQty = 48}
    }

    Private Shared ReadOnly Spacer89Javaline As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 472,  .Spacer1Type = "79", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 546,  .Spacer1Type = "79", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 620,  .Spacer1Type = "79", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 695,  .Spacer1Type = "79", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 769,  .Spacer1Type = "79", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 843,  .Spacer1Type = "79", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 917,  .Spacer1Type = "79", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 991,  .Spacer1Type = "79", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1066, .Spacer1Type = "79", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1140, .Spacer1Type = "79", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1214, .Spacer1Type = "79", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1288, .Spacer1Type = "79", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1362, .Spacer1Type = "79", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1437, .Spacer1Type = "79", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1511, .Spacer1Type = "79", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1585, .Spacer1Type = "79", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1659, .Spacer1Type = "79", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1733, .Spacer1Type = "79", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1808, .Spacer1Type = "79", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1882, .Spacer1Type = "79", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1956, .Spacer1Type = "79", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2030, .Spacer1Type = "79", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2104, .Spacer1Type = "79", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2179, .Spacer1Type = "79", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2253, .Spacer1Type = "79", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2327, .Spacer1Type = "79", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2401, .Spacer1Type = "79", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2475, .Spacer1Type = "79", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2550, .Spacer1Type = "79", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2624, .Spacer1Type = "79", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2698, .Spacer1Type = "79", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2772, .Spacer1Type = "79", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2846, .Spacer1Type = "79", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2921, .Spacer1Type = "79", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2995, .Spacer1Type = "79", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3069, .Spacer1Type = "79", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3143, .Spacer1Type = "79", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3217, .Spacer1Type = "79", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3292, .Spacer1Type = "79", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3366, .Spacer1Type = "79", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3440, .Spacer1Type = "79", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3514, .Spacer1Type = "79", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3588, .Spacer1Type = "79", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3663, .Spacer1Type = "79", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3737, .Spacer1Type = "79", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3811, .Spacer1Type = "79", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3885, .Spacer1Type = "79", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3959, .Spacer1Type = "79", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4031, .Spacer1Type = "79", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4105, .Spacer1Type = "79", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4179, .Spacer1Type = "79", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4253, .Spacer1Type = "79", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4327, .Spacer1Type = "79", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4401, .Spacer1Type = "79", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4475, .Spacer1Type = "79", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4549, .Spacer1Type = "79", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4623, .Spacer1Type = "79", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4697, .Spacer1Type = "79", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4771, .Spacer1Type = "79", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4845, .Spacer1Type = "79", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4919, .Spacer1Type = "79", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4993, .Spacer1Type = "79", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5067, .Spacer1Type = "79", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5141, .Spacer1Type = "79", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5215, .Spacer1Type = "79", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5289, .Spacer1Type = "79", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5363, .Spacer1Type = "79", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5437, .Spacer1Type = "79", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5511, .Spacer1Type = "79", .CarriersQty = 74}
    }
    
    Private Shared Function FindHeaderData(ByVal data As ParamBindOrderAggregate) As Object
        Try
            Dim HeaderData As New Dictionary(Of String, Object)()
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand("SELECT * FROM view_order_headers WHERE OrderType=@OrderType AND Id = @Id", conn)
                    cmd.Parameters.AddWithValue("@Id", data.headerid)
                    cmd.Parameters.AddWithValue("@OrderType", data.ordertype)

                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            For i As Integer = 0 To reader.FieldCount - 1
                                Dim columnName As String = reader.GetName(i)

                                If reader.IsDBNull(i) Then
                                    HeaderData(columnName) = Nothing
                                Else
                                    HeaderData(columnName) = reader.GetValue(i)
                                End If
                            Next
                        End If
                    End Using
                End Using
            End Using

            Return New With { .error = False, .list = HeaderData}
        Catch ex As Exception
            Return New With {.error = True, .message = String.Format("FindHeaderData: {0}", ex.Message)}
        End Try
    End Function

    Private Shared Function FindShipping(ByVal customerid As String) As Object
        Try
            Dim ShippingData As New Dictionary(Of String, Object)()
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand("SELECT * FROM CustomerAddress WHERE CustomerId = @CustomerId AND [Primary] = 1", conn)
                    cmd.Parameters.AddWithValue("@CustomerId", customerid)

                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            For i As Integer = 0 To reader.FieldCount - 1
                                Dim columnName As String = reader.GetName(i)

                                If reader.IsDBNull(i) Then
                                    ShippingData(columnName) = Nothing
                                Else
                                    ShippingData(columnName) = reader.GetValue(i)
                                End If
                            Next
                        End If
                    End Using
                End Using
            End Using

            Return New With { .error = False, .list = ShippingData}
        Catch ex As Exception
            Return New With {.error = True, .message = String.Format("FindShipping: {0}", ex.Message)}
        End Try
    End Function

End Class
