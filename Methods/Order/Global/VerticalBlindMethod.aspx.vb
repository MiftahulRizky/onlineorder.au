Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_Order_VerticalBlindMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamBindFormAggregate
        Public Property headerid As String
        Public Property ordertype As String
        Public Property designid As String
        Public Property itemid As String
        Public Property itemaction As String
    End Class

    Public Class ParamListData
        Public Property field As String
        Public Property designid As String
        Public Property blindtype As String
        Public Property brackettype As String
        Public Property tubetype As String
        Public Property controltype As String
        Public Property fabrictype As String
        Public Property fabriclength As String
        Public Property trim As String
        Public Property railtype As String
    End Class

    Public Class ParamSubmit
        '#Submit OrderHeader
        Public Property blindtype As String
        Public Property tubetype As String
        Public Property controltype As String
        Public Property colourtype As String
        Public Property qty As String
        Public Property room As String
        Public Property mounting As String
        Public Property fabricinsert As String
        Public Property fabrictype As String
        Public Property fabriccolour As String
        Public Property bladeqty As String
        Public Property width As String
        Public Property drop As String
        Public Property stackposition As String
        Public Property controlposition As String
        Public Property chaincolour As String
        Public Property chainlength As String
        Public Property customchainlength As String
        Public Property wandcolour As String
        Public Property wandlength As String
        Public Property customwandlength As String
        Public Property bottomjoining As String
        Public Property extbracket As String
        Public Property sloping As String
        Public Property notes As String
        Public Property markup As String
        

        '#aditional param
        Public Property headerid As String
        Public Property ordertype As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
    End Class

    Public Class ParamBindItemOrder
        Public Property headerid As String
        Public Property ordertype As String
        Public Property designid As String
        Public Property itemid As String
        Public Property itemaction As String
    End Class


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindFormAggregate(ByVal data As ParamBindFormAggregate) As Object
        Try
            Dim DesignName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Designs WHERE Id = '{0}'", data.designid))

            Dim HeaderData As Object
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand("SELECT OrderId, OrderNumber, OrderName FROM view_order_headers WHERE OrderType IN ('Blinds', 'Door and Window', 'Furnishings') AND Id = @Id", conn)
                    cmd.Parameters.AddWithValue("@Id", data.headerid)
                    ' cmd.Parameters.AddWithValue("@OrderType", data.ordertype)

                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            HeaderData = New With {
                                .OrderId = reader("OrderId").ToString(),
                                .OrderNumber = reader("OrderNumber").ToString(),
                                .OrderName = reader("OrderName").ToString()
                            }
                        End If
                    End Using
                End Using
            End Using

            Dim Blinds As Object  = BindListData(New ParamListData With {
                .field = "blindtype",
                .designid = data.designid
            })
            If Blinds.error Then
                Throw New Exception(Blinds.message)
            End If

            Return New With {
                .DesignName = DesignName,
                .HeaderData = HeaderData,
                .Blinds = Blinds.list
            }
        Catch ex As Exception
            Return New With {.error = True, .message = String.Format("BindFormAggregate: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindListData(ByVal data As ParamListData) As Object
        Try
            Dim query As String = ""
            Dim resultList As New List(Of Dictionary(Of String, String))() 

            Select Case data.field.ToLower()
                Case "blindtype"
                    query = String.Format("SELECT Id, Name FROM Blinds WHERE DesignId='{0}' AND Active=1 ORDER BY Name ASC", data.designid)
                    resultList = GetFormattedData(query, "Id", "Name")

                Case "tubetype"
                    query = String.Format("SELECT TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND Active=1 GROUP BY TubeType ORDER BY TubeType ASC", data.designid, UCase(data.blindtype).ToString())
                    resultList = GetFormattedData(query, "TubeType", "TubeType")

                Case "controltype"
                    query = String.Format("SELECT ControlType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND TubeType='{2}' AND Active=1 GROUP BY ControlType ORDER BY ControlType ASC", data.designid, UCase(data.blindtype).ToString(), data.tubetype)
                    resultList = GetFormattedData(query, "ControlType", "ControlType")

                Case "colourtype"
                    query = String.Format("SELECT Id, ColourType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND TubeType = '{2}' AND ControlType='{3}' AND Active=1 ORDER BY Name ASC", data.designid, UCase(data.blindtype).ToString(), data.tubetype, data.controltype)
                    resultList = GetFormattedData(query, "Id", "ColourType")

                Case "fabrictype"
                    data.designid ="919E843A-A3F4-4BE2-98E8-C0CC4088F513" '#Global roller
                    query = String.Format("SELECT Type FROM Fabrics WHERE DesignId='{0}' AND Active='1' GROUP BY Type ORDER BY Type ASC", data.designid)
                    resultList = GetFormattedData(query, "Type", "Type")

                Case "fabriccolour"
                    data.designid ="919E843A-A3F4-4BE2-98E8-C0CC4088F513" '#Global roller
                    query = String.Format("SELECT Id, Colour FROM Fabrics WHERE DesignId='{0}' AND Active='1' AND Type='{1}' ORDER BY Name ASC", data.designid, data.fabrictype)
                    resultList = GetFormattedData(query, "Id", "Colour")

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

    Private Shared Function GetFormattedData(query As String, valueField As String, textField As String) As Object
        Dim list As New List(Of Dictionary(Of String, String))()

        Dim datas As DataSet = publicCfg.GetListData(query)

        If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
            For Each row As DataRow In datas.Tables(0).Rows
                list.Add(New Dictionary(Of String, String) From {
                    {"value", row(valueField).ToString()},
                    {"text", row(textField).ToString()}
                })
            Next
        End If

        Return list
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Submit(ByVal data As ParamSubmit) As Object
        Try
            Dim BlindName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Blinds WHERE Id = '{0}'", data.blindtype))
            Dim isComplete As Boolean = BlindName = "Complete Set"
            Dim isSlatOnly As Boolean = BlindName = "Slat Only"
            Dim isTrackOnly As Boolean = BlindName = "Track Only"
            Dim is127mm As Boolean = data.tubetype = "127mm"
            Dim is89mm As Boolean = data.tubetype = "89mm"
            Dim isChain As Boolean = data.controltype = "Chain"
            Dim isWand As Boolean = data.controltype = "Wand"
            Dim isInsert As Boolean = data.fabricinsert = "Yes"

            Dim qty As Integer
            If String.IsNullOrEmpty(data.qty) Then
                Return New With { .warning = true, .message = "qty is required !", .field = "qty"}
            End If
            If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then
                Return New With { .warning = true, .message = "please check your qty !", .field = "qty"}
            End If

            If Not String.IsNullOrEmpty(data.room) Then
                If InStr(data.room, "&") > 0 Then
                    Return New With { .warning = true, .message = "character [&] is not allowed !", .field = "room"}
                End If
            End If

            If String.IsNullOrEmpty(data.mounting) Then
                Return New With { .warning = true, .message = "mounting is required !", .field = "mounting"}
            End If

            If isComplete Or isSlatOnly Or (isTrackOnly And isInsert) Then
                If String.IsNullOrEmpty(data.fabrictype) Then
                    Return New With {.warning = true, .message = "fabric type is required !",.field = "fabrictype"}
                End If
                If String.IsNullOrEmpty(data.fabriccolour) Then
                    Return New With {.warning = true, .message = "fabric colour is required !",.field = "fabriccolour"}
                End If
            End If
            
            If isSlatOnly Or isTrackOnly Then
                If String.IsNullOrEmpty(data.bladeqty) Then
                    Return New With {.warning = true, .message = "blade quantity is required !",.field = "bladeqty"}
                End If
            End If
            
            Dim width As Integer
            Dim minwidth As Integer = 300
            Dim maxwidth As Integer = 0
            If isComplete Or isTrackOnly Then
                If String.IsNullOrEmpty(data.width) Then
                    Return New With {.warning = true, .message = "width is required !",.field = "width"}
                End If
                If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then
                    Return New With {.warning = true, .message = "width must be a positive integer !",.field = "width"}
                End If

                If is127mm Then maxwidth = 5477
                If is89mm Then maxwidth = 5511

                If width < minwidth Then
                    Return New With {.warning = true, .message = String.Format("width must be greater than or equal to {0} !", minwidth), .field = "width"}
                End If
                If width > maxwidth Then
                    Return New With {.warning = true, .message = String.Format("width must be less than or equal to {0} !", maxwidth), .field = "width"}
                End If
            End If

            Dim drop As Integer
            Dim mindrop As Integer = 0
            Dim maxdrop As Integer = 3050
            If isComplete Or isSlatOnly Then
                If String.IsNullOrEmpty(data.drop) Then
                    Return New With {.warning = true, .message = "drop is required !",.field = "drop"}
                End If
                If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                    Return New With {.warning = true, .message = "drop must be a positive integer !",.field = "drop"}
                End If
                If drop < mindrop Then
                    Return New With {.warning = true, .message = String.Format("drop must be greater than or equal to {0} !", mindrop),.field = "drop"}
                End If
                If drop > maxdrop Then
                    Return New With {.warning = true, .message = String.Format("drop must be less than or equal to {0} !", maxdrop),.field = "drop"}
                End If
            End If

            If isComplete Or isTrackOnly Then
                If String.IsNullOrEmpty(data.stackposition) Then
                    Return New With {.warning = true, .message = "stack position is required !",.field = "stackposition"}
                End If
                If isChain Then
                    If String.IsNullOrEmpty(data.controlposition) Then
                        Return New With {.warning = true, .message = "control position is required !",.field = "controlposition"}
                    End If
                End If
            End If
            
            
            Dim chainlength As Integer
            Dim customchainlength As Integer
            If isChain Then
                If String.IsNullOrEmpty(data.chaincolour) Then
                    Return New With {.warning = true, .message = "chain colour is required !",.field = "chaincolour"}
                End If
                If String.IsNullOrEmpty(data.chainlength) Then
                    Return New With {.warning = true, .message = "chain length is required !",.field = "chainlength"}
                End If
                If data.chainlength = "Custom" Then
                    If String.IsNullOrEmpty(data.customchainlength) Then
                        Return New With {.warning = true, .message = "custom chain length is required !",.field = "customchainlength"}
                    End If
                    If Not Integer.TryParse(data.customchainlength, customchainlength) OrElse customchainlength <= 0 Then
                        Return New With {.warning = true, .message = "custom chain length must be a positive integer !",.field = "customchainlength"}
                    End If
                End If
            End If  

            Dim wandlength As Integer
            Dim customwandlength As Integer
            If isWand Then
                If String.IsNullOrEmpty(data.wandcolour) Then
                    Return New With {.warning = true, .message = "wand colour is required !",.field = "wandcolour"}
                End If
                If String.IsNullOrEmpty(data.wandlength) Then
                    Return New With {.warning = true, .message = "wand length is required !",.field = "wandlength"}
                End If
                If data.wandlength = "Custom" Then
                    If String.IsNullOrEmpty(data.customwandlength) Then
                        Return New With {.warning = true, .message = "custom wand length is required !",.field = "customwandlength"}
                    End If
                    If Not Integer.TryParse(data.customwandlength, customwandlength) OrElse customwandlength <= 0 Then
                        Return New With {.warning = true, .message = "custom wand length must be a positive integer !",.field = "customchainlength"}
                    End If
                End If
            End If  

            If isComplete Or isSlatOnly Then
                If String.IsNullOrEmpty(data.bottomjoining) Then
                    Return New With {.warning = true, .message = "bottom joining is required !",.field = "bottomjoining"}
                End If
            End If

            If Not String.IsNullOrEmpty(data.notes) Then
                If InStr(data.notes, "&") > 0 Then
                    Return New With {.warning = true, .message = "notes must not contain [&] character !",.field = "notes"}
                End If

                If data.notes.Trim().Length > 1000 Then
                    Return New With {.warning = true, .message = "notes must be less than 1000 characters !",.field = "notes"}
                End If
            End If

            Dim markup As Integer
            If Not String.IsNullOrEmpty(data.markup) Then
                If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then
                    Return New With {.warning = true, .message = "please check your markup !",.field = "markup"}
                End If
            End If

            If String.IsNullOrEmpty(data.markup) Then
                data.markup = "0"
            End If

            If isTrackOnly AND isInsert = False Then
                data.fabrictype = "" : data.fabriccolour = ""
            End If

            Dim SoeId As String = publicCfg.GetSoeKitId(data.colourtype)
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            Dim FabricGroup As String = publicCfg.GetFabricGroup(data.fabriccolour)
            Dim PriceGroupName As String = "Global Vertical Blinds"
            
            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            If PriceGroupId = "" Then
                Throw New Exception("price group not found!")
            End If

            Dim ChainId As String = ""
            Dim MaterialChain As String = ""
            If isChain Then
                Dim ChainColour As String = String.format("({0})", data.chaincolour)
                If data.chainlength = "Standard" Then
                    chainlength = 2/3 * drop
                End If
                If data.chainlength = "Custom" Then
                    chainlength = customchainlength
                End If

                Dim ChainName As String = String.Format("{0} Chain + Joiner {1}", chainlength, ChainColour)
                Dim FormulaChain As String = publicCfg.GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName))

                If Not String.IsNullOrEmpty(FormulaChain) Then
                    ChainId = FormulaChain
                End If
                If String.IsNullOrEmpty(ChainId) Then
                    ChainName = String.Format("Custom Chain + Joiner {0}", ChainColour)
                    ChainId = publicCfg.GetItemData("SELECT Id FROM Chains WHERE Name = '" + ChainName + "'")
                End If

                If String.IsNullOrEmpty(ChainId) Then
                    Throw New Exception("chain id not found!")
                End If

                Dim CreateJSON = New With { .name = data.chainlength, .value = chainlength}
                MaterialChain = Newtonsoft.Json.JsonConvert.SerializeObject(CreateJSON)

                data.wandlength = "" : data.wandcolour = "" : wandlength = 0 : customwandlength = 0
            End If

            Dim MaterialWand As String = ""
            If isWand Then
                If data.wandlength = "Standard" Then
                    wandlength = Math.Round((2/3) * drop)
                    If wandlength > 1000 Then wandlength = 1000
                End If
                If data.wandlength = "Custom" Then
                    wandlength = customwandlength
                End If

                If data.stackposition = "Centre" Then data.controlposition = "Right and Left"
                If data.stackposition = "Split" Then data.controlposition = "Middle"
                If data.stackposition = "Left" Then data.controlposition = "Right"
                If data.stackposition = "Right" Then data.controlposition = "Left"

                Dim CreateJSON = New With { .name = data.wandlength, .value = wandlength}
                MaterialWand = Newtonsoft.Json.JsonConvert.SerializeObject(CreateJSON)

                data.chaincolour = "" : data.chainlength = "" : chainlength = 0 : customchainlength = 0 : ChainId = ""
            End If

            If isTrackOnly Then
                If is127mm Then
                    width = data.bladeqty * 115
                    If data.bladeqty < 6 Then width = 472
                End If
                If is89mm Then
                    width = data.bladeqty * 79
                    If data.bladeqty < 5 Then width = 591
                End If
            End If

            If isSlatOnly Then
                width = 0
            End If

            


            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, FabricId, ChainId, PriceGroupId, Qty, Location, Mounting, InsertInTrack, SlatQty, Width, [Drop], StackPosition, ControlPosition, MaterialChain, WandColour, MaterialCord, BottomHoldDown, BracketExtension, Cleat, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @FabricId, @ChainId, @PriceGroupId, @Qty, @Location, @Mounting, @InsertInTrack, @SlatQty, @Width, @Drop, @StackPosition, @ControlPosition, @MaterialChain, @WandColour, @MaterialCord, @BottomHoldDown, @BracketExtension, @Cleat, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@InsertInTrack", if(data.fabricinsert = "Yes", 1, 0))
                        myCmd.Parameters.AddWithValue("@SlatQty", data.bladeqty)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@MaterialChain", MaterialChain)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                        myCmd.Parameters.AddWithValue("@MaterialCord", MaterialWand)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.bottomjoining)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.extbracket)
                        myCmd.Parameters.AddWithValue("@Cleat", data.sloping)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                publicCfg.ResetPriceDetail(ItemId)
                publicCfg.HitungHarga(data.headerid, ItemId)
                publicCfg.HitungSurcharge(data.headerid, ItemId)

                Dim OrderType As String = publicCfg.GetItemData(String.Format("SELECT OrderType FROM OrderHeaders WHERE Id='{0}'", data.headerid))
                Dim dataLog As Object() = {data.headerid, ItemId, OrderType, data.loginid, "Add Item Order"}
                orderCfg.Log_Orders(dataLog)

            End If

            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then
                Dim ItemId As String = data.itemid

                 Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, FabricId=@FabricId, ChainId=@ChainId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, InsertInTrack=@InsertInTrack, SlatQty=@SlatQty, Width=@Width, [Drop]=@Drop, StackPosition=@StackPosition, ControlPosition=@ControlPosition, MaterialChain=@MaterialChain, WandColour=@WandColour, MaterialCord=@MaterialCord,BottomHoldDown=@BottomHoldDown, BracketExtension=@BracketExtension, Cleat=@Cleat, Notes=@Notes, Matrix=0.00, Charge=0.00, TotalMatrix=0.00, TotalCharge=0.00, MarkUp=@MarkUp WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@InsertInTrack", if(data.fabricinsert = "Yes", 1, 0))
                        myCmd.Parameters.AddWithValue("@SlatQty", data.bladeqty)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@MaterialChain", MaterialChain)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                        myCmd.Parameters.AddWithValue("@MaterialCord", MaterialWand)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.bottomjoining)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.extbracket)
                        myCmd.Parameters.AddWithValue("@Cleat", data.sloping)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                publicCfg.ResetPriceDetail(ItemId)
                publicCfg.HitungHarga(data.headerid, ItemId)
                publicCfg.HitungSurcharge(data.headerid, ItemId)

                Dim OrderType As String = publicCfg.GetItemData(String.Format("SELECT OrderType FROM OrderHeaders WHERE Id='{0}'", data.headerid))
                Dim dataLog As Object() = {data.headerid, ItemId, OrderType, data.loginid, "Update Item Order"}
                orderCfg.Log_Orders(dataLog)

            End If

            Dim url As String = publicCfg.MyCancel(data.headerid, data.ordertype)
            Return New With {.success = true, .message = "Item saved successfully !", .url = url}
        Catch ex As Exception
            Return New With { .error = true, .message = String.Format("Submit: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function Cancel(ByVal headerid As String, ByVal ordertype As String, ByVal rolename As String ) As Object
        Try
            Dim msg As String = "200"
            Dim url As String = publicCfg.MyCancel(headerid, ordertype)
            Return New With {.success = true, .message = msg, .url = url}
        Catch ex As Exception
            Return New With { .error = true, .message = String.Format("Cancel: {0}", ex.Message)}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindItemOrder(ByVal data As ParamBindItemOrder) As Object
        Try

            If data Is Nothing Then
                Throw New Exception("Data parameter input bernilai null.")
            End If

            Dim DetailData As New Dictionary(Of String, Object)()
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand("SELECT * FROM view_details WHERE Id = @Id", conn)
                    cmd.Parameters.AddWithValue("@Id", data.itemid)

                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            For i As Integer = 0 To reader.FieldCount - 1
                                Dim columnName As String = reader.GetName(i)

                                If reader.IsDBNull(i) Then
                                    DetailData(columnName) = Nothing
                                Else
                                    DetailData(columnName) = reader.GetValue(i)
                                End If
                            Next
                            
                        End If
                    End Using
                End Using
            End Using

            Dim SafeString = Function(val As Object) As String
                If val Is Nothing OrElse IsDBNull(val) Then Return String.Empty
                Return val.ToString()
            End Function


            Dim Tubes As Object  = BindListData(New ParamListData With {
                .field = "tubetype",
                .designid = data.designid,
                .blindtype = DetailData("BlindId").ToString()
            })
            If Tubes.error Then
                Throw New Exception(Tubes.message)
            End If

            Dim Controls As Object  = BindListData(New ParamListData With {
                .field = "controltype",
                .designid = data.designid,
                .blindtype = DetailData("BlindId").ToString(),
                .tubetype = DetailData("TubeType").ToString()
            })
            If Controls.error Then
                Throw New Exception(Controls.message)
            End If

            Dim Colours As Object  = BindListData(New ParamListData With {
                .field = "colourtype",
                .designid = data.designid,
                .blindtype = DetailData("BlindId").ToString(),
                .tubetype = DetailData("TubeType").ToString(),
                .controltype = DetailData("ControlType").ToString()
            })
            If Colours.error Then
                Throw New Exception(Colours.message)
            End If

            Dim Fabrics As Object  = BindListData(New ParamListData With {
                .field = "fabrictype",
                .designid = data.designid
            })
            If Fabrics.error Then
                Throw New Exception(Fabrics.message)
            End If

            Dim FabricColours As Object  = BindListData(New ParamListData With {
                .field = "fabriccolour",
                .designid = data.designid,
                .fabrictype = SafeString(DetailData("FabricType"))
            })
            If FabricColours.error Then
                Throw New Exception(FabricColours.message)
            End If

            Return New With {
                .DetailData = DetailData,
                .Tubes = Tubes.list,
                .Controls = Controls.list,
                .Colours = Colours.list,
                .Fabrics = Fabrics.list,
                .FabricColours = FabricColours.list
            }
        Catch ex As Exception
            Return New With {.error = True, .message = String.Format("BindItemOrder: {0}", ex.Message)}
        End Try
    End Function
End Class
