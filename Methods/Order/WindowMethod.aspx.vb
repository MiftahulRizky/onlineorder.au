Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_Order_WindowMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

     Public Class ParamSubmit
        '#Submit OrderHeader
        Public Property blindtype As String
        Public Property colourtype As String
        Public Property qty As String
        Public Property room As String
        Public Property mounting As String
        Public Property width As String
        Public Property drop As String
        Public Property meshtype As String
        Public Property framecolour As String
        Public Property brace As String
        Public Property angletype As String
        Public Property anglelength As String
        Public Property angleqty As String
        Public Property porthole As String
        Public Property plungerpin As String
        Public Property swivelcolour As String
        Public Property swivelqty As String
        Public Property swivelqtyb As String
        Public Property springqty As String
        Public Property topplasticqty As String
        Public Property notes As String
        Public Property markup As String
        

        '#aditional param
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
        Public Property blindno As String
        Public Property uniqueid As String
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
    Public Shared Function BindBlindType(ByVal designid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Blinds WHERE DesignId='" + designid + "' AND Active=1 ORDER BY Name ASC")
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
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindColourType(ByVal designid As String, ByVal blindid As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT *, UPPER(ColourType) AS ColourText FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND Active=1 ORDER BY Name ASC", designid, UCase(blindid).ToString())
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("ColourType").ToString()}
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
    Public Shared Function Submit(ByVal data As ParamSubmit) As Object
        Try
            Dim BlindName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Blinds WHERE Id = '{0}'", data.blindtype))
            Dim qty As Integer
            If String.IsNullOrEmpty(data.qty) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "qty type is required !", .field = "qty"}}
            End If
            If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "please check your qty !", .field = "qty"}}
            End If

            If Not String.IsNullOrEmpty(data.room) Then
                If InStr(data.room, "&") > 0 Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "character [&] is not allowed !", .field = "room"}}
                End If
            End If

            If String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "mounting is required !", .field = "mounting"}}
            End If

            Dim width As Integer
            If String.IsNullOrEmpty(data.width) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width is required !",.field = "width"}}
            End If
            If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be a positive integer !",.field = "width"}}
            End If
            If width < 150 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 150 !",.field = "width"}}
            End If
            If width > 6000 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 6000 !",.field = "width"}}
            End If

            Dim drop As Integer
            If String.IsNullOrEmpty(data.drop) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop is required !",.field = "drop"}}
            End If
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be a positive integer !",.field = "drop"}}
            End If
            If drop < 150 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be greater than or equal to 150 !",.field = "drop"}}
            End If
            If drop > 3200 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be less than or equal to 3200 !",.field = "drop"}}
            End If

            If String.IsNullOrEmpty(data.meshtype) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "mesh type is required !",.field = "meshtype"}}
            End If

            If String.IsNullOrEmpty(data.framecolour) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "frame colour is required !",.field = "framecolour"}}
            End If

            Dim anglelength As Integer
            Dim angletype As Integer
            If Not String.IsNullOrEmpty(data.angletype) Then
                If String.IsNullOrEmpty(data.anglelength) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "angle length is required !",.field = "anglelength"}}
                End If
                If Not Integer.TryParse(data.anglelength, anglelength) OrElse anglelength <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "angle length must be a positive integer !",.field = "anglelength"}}
                End If
                If anglelength < 1 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "please check your angle length !",.field = "anglelength"}}
                End If
                If anglelength > 5000 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "angle length must be less than or equal to 5000 !",.field = "anglelength"}}
                End If

                If String.IsNullOrEmpty(data.angletype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "angle type is required !",.field = "angletype"}}
                End If
                If Not Integer.TryParse(data.angletype, angletype) OrElse angletype <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "angle type must be a positive integer !",.field = "angletype"}}
                End If
                If angletype < 1 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "please check your angle type !",.field = "angletype"}}
                End If
                If angletype > 10 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "angle type must be less than or equal to 10 !",.field = "angletype"}}
                End If
            End If

            Dim swivelqty As Integer
            Dim swivelqtyb As Integer
            If Not String.isNullOrEmpty(data.swivelcolour) Then
                If String.IsNullOrEmpty(data.swivelqty) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "swivel clip qty for 1.6mm is required !",.field = "swivelqty"}}
                End If
                If Not Integer.TryParse(data.swivelqty, swivelqty) OrElse swivelqty <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "swivel clip qty for 1.6mm must be a positive integer !",.field = "swivelqty"}}
                End If
                If swivelqty < 1 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "please check your swivel clip qty for 1.6mm !",.field = "swivelqty"}}
                End If
                If swivelqty > 10 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "swivel clip qty for 1.6mm must be less than or equal to 10 !",.field = "swivelqty"}}
                End If

                If String.IsNullOrEmpty(data.swivelqtyb) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "swivel clip qty for 11mm is required !",.field = "swivelqtyb"}}
                End If
                If Not Integer.TryParse(data.swivelqtyb, swivelqtyb) OrElse swivelqtyb <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "swivel clip qty for 11mm must be a positive integer !",.field = "swivelqtyb"}}
                End If
                If swivelqtyb < 1 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "please check your swivel clip qty for 11mm !",.field = "swivelqtyb"}}
                End If
                If swivelqtyb > 10 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "swivel clip qty for 11mm must be less than or equal to 10 !",.field = "swivelqtyb"}}
                End If
            End If



            

            If Not String.IsNullOrEmpty(data.notes) Then
                If InStr(data.notes, "&") > 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "notes must not contain [&] character !",.field = "notes"}}
                End If

                If data.notes.Trim().Length > 1000 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "notes must be less than 1000 characters !",.field = "notes"}}
                End If
            End If

            Dim markup As Integer
            If Not String.IsNullOrEmpty(data.markup) Then
                If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "please check your markup !",.field = "markup"}}
                End If
            End If



            If String.IsNullOrEmpty(data.markup) Then
                data.markup = "0"
            End If

            
            
            ' Dim SoeId As String = publicCfg.GetSoeKitId(data.colourtype)
            ' Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            ' Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            ' Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            ' Dim FabricGroup As String = publicCfg.GetFabricGroup(data.fabriccolour)
            ' Dim PriceGroupName As String = String.Format("Roller Blind - {0}", FabricGroup)
            ' If BlindName = "Skin Only" Then
            '     PriceGroupName = String.Format("Roller Skin Only - {0}", FabricGroup)
            ' End If

            ' Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            ' Dim CassetteExtraId As String = ""
            ' If BlindName = "Cassette" Then
            '     Dim CassetteExtraName As String = String.Format("{0} - {1}", data.brackettype, data.mounting)
            '     If data.brackettype = "Headbox Only" Then
            '         CassetteExtraName = data.brackettype
            '     End If
            '     ' PriceGroupId = publicCfg.GetPriceGroupId(data.designid, CassetteExtraName)
            '     CassetteExtraId = publicCfg.GetPriceGroupId(data.designid, CassetteExtraName)
            ' End If

            ' If PriceGroupId = "" Then
            '     Throw New Exception("Something went wrong !")
            ' End If

            
            ' Return New ErrorResponse With {.error = New ErrorDetail With {.message = data.uniqueid, .field = ""}}
            
            
            Dim msg As String = "200"
            ' If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
            '     Dim ItemId As String = publicCfg.CreateOrderItemId()
            '     data.uniqueid = ""

            '     If data.brackettype = "Double" Or InStr(data.brackettype, "Linked") > 0 Or InStr(data.brackettype, "Link") > 0 Then
            '         data.uniqueid = GenerateUniqueId()
            '     End If

            '     ' Return New ErrorResponse With {.error = New ErrorDetail With {.message = data.uniqueid, .field = ""}}


            '     Using thisConn As New SqlConnection(myConn)
            '         Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, UniqueId, BlindNo, KitId, SoeKitId, ExactId, FabricId, ChainId, BottomRailId, PriceGroupId, CassetteExtraId, Qty, Location, Mounting, Width, [Drop], RollDirection, ControlPosition, ChainLength, Accessory, TubeSize, Trim, BracketCover, BracketExtension, ChildSafe, MotorStyle, MotorRemote, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @UniqueId, @BlindNo, @KitId, @SoeKitId, @ExactId, @FabricId, @ChainId, @BottomRailId, @PriceGroupId, @CassetteExtraId, @Qty, @Location, @Mounting, @Width, @Drop, @RollDirection, @ControlPosition, @ChainLength, @Accessory, @TubeSize, @Trim, @BracketCover, @BracketExtension, @ChildSafe, @MotorStyle, @MotorRemote, @MotorBattery, @MotorCharger, @Connector, @AdditionalMotor, @CableExitPoint, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", thisConn)
            '             myCmd.Parameters.AddWithValue("@Id", ItemId)
            '             myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
            '             myCmd.Parameters.AddWithValue("@UniqueId", If( String.IsNullOrEmpty(data.uniqueid), DBNull.Value, data.uniqueid))
            '             myCmd.Parameters.AddWithValue("@BlindNo", data.blindno)
            '             myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
            '             myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
            '             myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
            '             myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
            '             myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
            '             myCmd.Parameters.AddWithValue("@BottomRailId", If(String.IsNullOrEmpty(BottomRailId), DBNull.Value, BottomRailId))
            '             myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
            '             myCmd.Parameters.AddWithValue("@CassetteExtraId", If(String.IsNullOrEmpty(CassetteExtraId), DBNull.Value, CassetteExtraId))
            '             myCmd.Parameters.AddWithValue("@Qty", qty)
            '             myCmd.Parameters.AddWithValue("@Location", data.room)
            '             myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
            '             myCmd.Parameters.AddWithValue("@Width", width)
            '             myCmd.Parameters.AddWithValue("@Drop", drop)
            '             myCmd.Parameters.AddWithValue("@RollDirection", data.roll)
            '             myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
            '             myCmd.Parameters.AddWithValue("@ChainLength", If(String.IsNullOrEmpty(CLength), DBNull.Value, CLength))
            '             myCmd.Parameters.AddWithValue("@Accessory", data.accessory)
            '             myCmd.Parameters.AddWithValue("@TubeSize", data.tubesize)
            '             myCmd.Parameters.AddWithValue("@Trim", data.trim)
            '             myCmd.Parameters.AddWithValue("@BracketCover", data.bracketcovers)
            '             myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketext)
            '             myCmd.Parameters.AddWithValue("@ChildSafe", data.childsafe)
            '             myCmd.Parameters.AddWithValue("@MotorStyle", data.motorstyle)
            '             myCmd.Parameters.AddWithValue("@MotorRemote", data.motorremote)
            '             myCmd.Parameters.AddWithValue("@MotorBattery", data.externalbattery)
            '             myCmd.Parameters.AddWithValue("@MotorCharger", data.charger)
            '             myCmd.Parameters.AddWithValue("@Connector", data.connector)
            '             myCmd.Parameters.AddWithValue("@AdditionalMotor", data.extras)
            '             myCmd.Parameters.AddWithValue("@CableExitPoint", data.cableexitpoint)
            '             myCmd.Parameters.AddWithValue("@Notes", data.notes)
            '             myCmd.Parameters.AddWithValue("@MarkUp", markup)
            '             myCmd.Connection = thisConn
            '             thisConn.Open()
            '             myCmd.ExecuteNonQuery()
            '             thisConn.Close()
            '         End Using
            '     End Using

            '     publicCfg.ResetPriceDetail(ItemId)
            '     publicCfg.HitungHarga(data.headerid, ItemId)
            '     publicCfg.HitungSurcharge(data.headerid, ItemId)

            '     Dim dataLog As Object() = {data.headerid, ItemId, "Blinds", data.loginid, "Add Item Order"}
            '     orderCfg.Log_Orders(dataLog)

            '     msg = "Item added successfully !"

            '     If data.brackettype = "Double" Or InStr(data.brackettype, "Linked") > 0 Or InStr(data.brackettype, "Link") > 0 Then
            '         Dim BlindNoSelected As String = "first blind"
            '         If data.blindno = "Blind 2" Then
            '             BlindNoSelected = "second blind"
            '         End If

            '         msg += String.Format("<br/><br/> This is the <b>{0}</b>.", BlindNoSelected)
            '         msg += String.Format("<br/> from <b>{0}</b> - <b>{1}</b>", BlindName, data.brackettype)
            '         msg += String.Format("<br /><br />Please click the <b>Next Item</b> button that is written in green color of the <b>ITEM ID {0}</b>.", ItemId)
            '     End If

            '     If InStr(data.brackettype, "Linked") > 0 AND data.controltype = "Somfy WF" Then
            '         msg += "<br/><br/><b>Warning :</b>Check SP the availability for linking blind for WF motorised !"
            '     End If
            '     If InStr(data.brackettype, "Linked") > 0 AND data.controltype = "Alpha WF" AndAlso data.motorstyle = "Alpha 2NM Std" Then
            '         msg += "<br/><br/><b>Warning :</b> Check SP the availability for linking blind for WF motorised !"
            '     End If

            '     ' Return New ErrorResponse With {.error = New ErrorDetail With {.message = msg, .field = ""}}

                
            ' End If

            ' If data.itemaction = "NextItem" Then
            '     Dim ItemId As String = publicCfg.CreateOrderItemId()

            '     Using thisConn As New SqlConnection(myConn)
            '         Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, UniqueId, BlindNo, KitId, SoeKitId, ExactId, FabricId, ChainId, BottomRailId, PriceGroupId, CassetteExtraId, Qty, Location, Mounting, Width, [Drop], RollDirection, ControlPosition, ChainLength, Accessory, TubeSize, Trim, BracketCover, BracketExtension, ChildSafe, MotorStyle, MotorRemote, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @UniqueId, @BlindNo, @KitId, @SoeKitId, @ExactId, @FabricId, @ChainId, @BottomRailId, @PriceGroupId, @CassetteExtraId, @Qty, @Location, @Mounting, @Width, @Drop, @RollDirection, @ControlPosition, @ChainLength, @Accessory, @TubeSize, @Trim, @BracketCover, @BracketExtension, @ChildSafe, @MotorStyle, @MotorRemote, @MotorBattery, @MotorCharger, @Connector, @AdditionalMotor, @CableExitPoint, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", thisConn)
            '             myCmd.Parameters.AddWithValue("@Id", itemId)
            '             myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
            '             myCmd.Parameters.AddWithValue("@UniqueId", data.uniqueid)
            '             myCmd.Parameters.AddWithValue("@BlindNo", data.blindno)
            '             myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
            '             myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
            '             myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
            '             myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
            '             myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
            '             myCmd.Parameters.AddWithValue("@BottomRailId", If(String.IsNullOrEmpty(BottomRailId), DBNull.Value, BottomRailId))
            '             myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
            '             myCmd.Parameters.AddWithValue("@CassetteExtraId", If(String.IsNullOrEmpty(CassetteExtraId), DBNull.Value, CassetteExtraId))
            '             myCmd.Parameters.AddWithValue("@Qty", qty)
            '             myCmd.Parameters.AddWithValue("@Location", data.room)
            '             myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
            '             myCmd.Parameters.AddWithValue("@Width", width)
            '             myCmd.Parameters.AddWithValue("@Drop", drop)
            '             myCmd.Parameters.AddWithValue("@RollDirection", data.roll)
            '             myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
            '             myCmd.Parameters.AddWithValue("@ChainLength", If(String.IsNullOrEmpty(CLength), DBNull.Value, CLength))
            '             myCmd.Parameters.AddWithValue("@Accessory", data.accessory)
            '             myCmd.Parameters.AddWithValue("@TubeSize", data.tubesize)
            '             myCmd.Parameters.AddWithValue("@Trim", data.trim)
            '             myCmd.Parameters.AddWithValue("@BracketCover", data.bracketcovers)
            '             myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketext)
            '             myCmd.Parameters.AddWithValue("@ChildSafe", data.childsafe)
            '             myCmd.Parameters.AddWithValue("@MotorStyle", data.motorstyle)
            '             myCmd.Parameters.AddWithValue("@MotorRemote", data.motorremote)
            '             myCmd.Parameters.AddWithValue("@MotorBattery", data.externalbattery)
            '             myCmd.Parameters.AddWithValue("@MotorCharger", data.charger)
            '             myCmd.Parameters.AddWithValue("@Connector", data.connector)
            '             myCmd.Parameters.AddWithValue("@AdditionalMotor", data.extras)
            '             myCmd.Parameters.AddWithValue("@CableExitPoint", data.cableexitpoint)
            '             myCmd.Parameters.AddWithValue("@Notes", data.notes)
            '             myCmd.Parameters.AddWithValue("@MarkUp", markup)
            '             myCmd.Connection = thisConn
            '             thisConn.Open()
            '             myCmd.ExecuteNonQuery()
            '             thisConn.Close()
            '         End Using
            '     End Using

            '     If data.brackettype = "Double" Then
            '         '#SdsNext
            '         Dim ListNext As New List(Of Object) From {
            '             data.uniqueid,
            '             data.tubesize,
            '             data.mounting,
            '             data.room,
            '             data.childsafe,
            '             data.accessory,
            '             data.bracketcovers,
            '             data.bracketext,
            '             data.motorstyle,
            '             markup
            '         }
            '         Dim ResNext As String = SdsNext(ListNext)
            '         IF Not ResNext = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
            '         End If

            '         '#SdsSize
            '         Dim ListSize As New List(Of Object) From {
            '             data.uniqueid,
            '             width,
            '             drop
            '         }
            '         Dim ResSize As String = SdsSize(ListSize)
            '         IF Not ResSize = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResSize, .field = ""}}
            '         End If
            '     End If

            '     If InArray(data.brackettype, "Linked 2 Blinds (Dep)", "Linked 3 Blinds (Dep)") Then
            '         '#SdsDrop
            '         Dim ListDrop As New List(Of Object) From {
            '             data.uniqueid,
            '             drop
            '         }
            '         Dim ResDrop As String = SdsDrop(ListDrop)
            '         IF Not ResDrop = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDrop, .field = ""}}
            '         End If

            '         '#SdsRollDep
            '         Dim ListRollDep As New List(Of Object) From {
            '             data.uniqueid,
            '             data.roll
            '         }
            '         Dim ResRollDep As String = SdsRollDep(ListRollDep)
            '         IF Not ResRollDep = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResRollDep, .field = ""}}
            '         End If

            '         '#SdsFabric
            '         Dim ListFabric As New List(Of Object) From {
            '             data.uniqueid,
            '             data.fabriccolour,
            '             PriceGroupId
            '         }
            '         Dim ResFabric As String = SdsFabric(ListFabric)
            '         IF Not ResFabric = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResFabric, .field = ""}}
            '         End If

            '         '#SdsNext
            '         Dim ListNext As New List(Of Object) From {
            '             data.uniqueid,
            '             data.tubesize,
            '             data.mounting,
            '             data.room,
            '             data.childsafe,
            '             data.accessory,
            '             data.bracketcovers,
            '             data.bracketext,
            '             data.motorstyle,
            '             markup
            '         }
            '         Dim ResNext As String = SdsNext(ListNext)
            '         IF Not ResNext = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
            '         End If

            '     End If

            '     If InArray(data.brackettype, "Linked 2 Blinds (Ind)", "Linked 3 Blinds (Ind)") Then
            '         '#SdsNext
            '         Dim ListNext As New List(Of Object) From {
            '             data.uniqueid,
            '             data.tubesize,
            '             data.mounting,
            '             data.room,
            '             data.childsafe,
            '             data.accessory,
            '             data.bracketcovers,
            '             data.bracketext,
            '             data.motorstyle,
            '             markup
            '         }
            '         Dim ResNext As String = SdsNext(ListNext)
            '         IF Not ResNext = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
            '         End If

            '         '#SdsFabric
            '         Dim ListFabric As New List(Of Object) From {
            '             data.uniqueid,
            '             data.fabriccolour,
            '             PriceGroupId
            '         }
            '         Dim ResFabric As String = SdsFabric(ListFabric)
            '         IF Not ResFabric = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResFabric, .field = ""}}
            '         End If

            '     End If

            '     If data.brackettype = "Double and Link System Dep" Then
            '         '#SdsDrop
            '         Dim ListDrop As New List(Of Object) From {
            '             data.uniqueid,
            '             drop
            '         }
            '         Dim ResDrop As String = SdsDrop(ListDrop)
            '         IF Not ResDrop = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDrop, .field = ""}}
            '         End If

            '         If data.blindno = "Blind 2" Then
            '             '#SdsDB2First
            '             Dim ListDB2DepFirst As New List(Of Object) From {
            '                 data.uniqueid,
            '                 data.fabriccolour,
            '                 PriceGroupId,
            '                 data.roll
            '             }
            '             Dim ResDB2DepFirst As String = SdsDB2First(ListDB2DepFirst)
            '             IF Not ResDB2DepFirst = "200" Then
            '                 Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2DepFirst, .field = ""}}
            '             End If
            '         End If

            '         If data.blindno = "Blind 4" Then
            '             '#SdsDB2Second
            '             Dim ListDB2DepSecond As New List(Of Object) From {
            '                 data.uniqueid,
            '                 data.fabriccolour,
            '                 PriceGroupId,
            '                 data.roll
            '             }
            '             Dim ResDB2DepSecond As String = SdsDB2Second(ListDB2DepSecond)
            '             IF Not ResDB2DepSecond = "200" Then
            '                 Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2DepSecond, .field = ""}}
            '             End If
            '         End If
                    
            '         '#SdsNext
            '         Dim ListNext As New List(Of Object) From {
            '             data.uniqueid,
            '             data.tubesize,
            '             data.mounting,
            '             data.room,
            '             data.childsafe,
            '             data.accessory,
            '             data.bracketcovers,
            '             data.bracketext,
            '             data.motorstyle,
            '             markup
            '         }
            '         Dim ResNext As String = SdsNext(ListNext)
            '         IF Not ResNext = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
            '         End If

            '     End If

            '     If data.brackettype = "Double and Link System Ind" Then

            '         If data.blindno = "Blind 2" Then
            '             '#SdsDB2IndFirst
            '             Dim ListDB2IndFirst As New List(Of Object) From {
            '                 data.uniqueid,
            '                 data.fabriccolour,
            '                 PriceGroupId,
            '                 data.roll
            '             }
            '             Dim ResDB2IndFirst As String = SdsDB2First(ListDB2IndFirst)
            '             IF Not ResDB2IndFirst = "200" Then
            '                 Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2IndFirst, .field = ""}}
            '             End If
            '         End If

            '         If data.blindno = "Blind 4" Then
            '             '#SdsDB2DepSecond
            '             Dim ListDB2DepSecond As New List(Of Object) From {
            '                 data.uniqueid,
            '                 data.fabriccolour,
            '                 PriceGroupId,
            '                 data.roll
            '             }
            '             Dim ResDB2DepSecond As String = SdsDB2Second(ListDB2DepSecond)
            '             IF Not ResDB2DepSecond = "200" Then
            '                 Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2DepSecond, .field = ""}}
            '             End If
            '         End If
                    
            '         '#SdsNext
            '         Dim ListNext As New List(Of Object) From {
            '             data.uniqueid,
            '             data.tubesize,
            '             data.mounting,
            '             data.room,
            '             data.childsafe,
            '             data.accessory,
            '             data.bracketcovers,
            '             data.bracketext,
            '             data.motorstyle,
            '             markup
            '         }
            '         Dim ResNext As String = SdsNext(ListNext)
            '         IF Not ResNext = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
            '         End If

            '     End If

            '     publicCfg.ResetPriceDetail(ItemId)
            '     publicCfg.HitungHarga(data.headerid, ItemId)
            '     publicCfg.HitungSurcharge(data.headerid, ItemId)

            '     Dim dataLog As Object() = {data.headerid, ItemId, "Blinds", data.loginid, "Add Item Order"}
            '     orderCfg.Log_Orders(dataLog)

            '     If data.blindno = "Blind 2" AND InArray(data.brackettype, "Linked 3 Blinds (Dep)", "Linked 3 Blinds (Ind)", "Double and Link System Dep", "Double and Link System Ind") Then
            '         Dim BlindNoSelected As String = "first blind"
            '         If data.blindno = "Blind 2" Then
            '             BlindNoSelected = "second blind"
            '         End If

            '         msg += String.Format("<br/><br/> This is the <b>{0}</b>.", BlindNoSelected)
            '         msg += String.Format("<br/> from <b>{0}</b> - <b>{1}</b>", BlindName, data.brackettype)
            '         msg += String.Format("<br /><br />Please click the <b>Next Item</b> button that is written in green color of the <b>ITEM ID {0}</b>.", ItemId)
            '     End If

            '     msg = "Item added successfully !"
            ' End If

            ' If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then

                
            '     Dim ItemId As String = data.itemid
            '     Using thisConn As New SqlConnection(myConn)
            '         Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, UniqueId=@UniqueId, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, FabricId=@FabricId, ChainId=@ChainId, BottomRailId=@BottomRailId, PriceGroupId=@PriceGroupId, CassetteExtraId=@CassetteExtraId, Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, RollDirection=@RollDirection, ControlPosition=@ControlPosition, ChainLength=@ChainLength, Accessory=@Accessory, TubeSize=@TubeSize, Trim=@Trim, BracketCover=@BracketCover, BracketExtension=@BracketExtension, ChildSafe=@ChildSafe, MotorStyle=@MotorStyle, MotorRemote=@MotorRemote, MotorBattery=@MotorBattery, MotorCharger=@MotorCharger, Connector=@Connector, AdditionalMotor=@AdditionalMotor, CableExitPoint=@CableExitPoint, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
            '             myCmd.Parameters.AddWithValue("@Id", ItemId)
            '             myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
            '             myCmd.Parameters.AddWithValue("@UniqueId", If(String.IsNullOrEmpty(data.uniqueid), DBNull.Value, data.uniqueid))
            '             myCmd.Parameters.AddWithValue("@BlindNo", data.blindno)
            '             myCmd.Parameters.AddWithValue("@KitId", UCase(data.colourtype).ToString())
            '             myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
            '             myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
            '             myCmd.Parameters.AddWithValue("@FabricId", UCase(data.fabriccolour).ToString())
            '             myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
            '             myCmd.Parameters.AddWithValue("@BottomRailId", If(String.IsNullOrEmpty(BottomRailId), DBNull.Value, BottomRailId))
            '             myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(PriceGroupId).ToString())
            '             myCmd.Parameters.AddWithValue("@CassetteExtraId", If(String.IsNullOrEmpty(CassetteExtraId), DBNull.Value, CassetteExtraId))
            '             myCmd.Parameters.AddWithValue("@Qty", qty)
            '             myCmd.Parameters.AddWithValue("@Location", data.room)
            '             myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
            '             myCmd.Parameters.AddWithValue("@Width", width)
            '             myCmd.Parameters.AddWithValue("@Drop", drop)
            '             myCmd.Parameters.AddWithValue("@RollDirection", data.roll)
            '             myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
            '             myCmd.Parameters.AddWithValue("@ChainLength", If(String.IsNullOrEmpty(CLength), DBNull.Value, CLength))
            '             myCmd.Parameters.AddWithValue("@Accessory", data.accessory)
            '             myCmd.Parameters.AddWithValue("@TubeSize", data.tubesize)
            '             myCmd.Parameters.AddWithValue("@Trim", data.trim)
            '             myCmd.Parameters.AddWithValue("@BracketCover", data.bracketcovers)
            '             myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketext)
            '             myCmd.Parameters.AddWithValue("@ChildSafe", data.childsafe)
            '             myCmd.Parameters.AddWithValue("@MotorStyle", data.motorstyle)
            '             myCmd.Parameters.AddWithValue("@MotorRemote", data.motorremote)
            '             myCmd.Parameters.AddWithValue("@MotorBattery", data.externalbattery)
            '             myCmd.Parameters.AddWithValue("@MotorCharger", data.charger)
            '             myCmd.Parameters.AddWithValue("@Connector", data.connector)
            '             myCmd.Parameters.AddWithValue("@AdditionalMotor", data.extras)
            '             myCmd.Parameters.AddWithValue("@CableExitPoint", data.cableexitpoint)
            '             myCmd.Parameters.AddWithValue("@Notes", data.notes)
            '             myCmd.Parameters.AddWithValue("@MarkUp", markup)
            '             myCmd.Connection = thisConn
            '             thisConn.Open()
            '             myCmd.ExecuteNonQuery()
            '             thisConn.Close()
            '         End Using
            '     End Using

            '     If data.brackettype = "Double" Then
            '         '#SdsNext
            '         Dim ListNext As New List(Of Object) From {
            '             data.uniqueid,
            '             data.tubesize,
            '             data.mounting,
            '             data.room,
            '             data.childsafe,
            '             data.accessory,
            '             data.bracketcovers,
            '             data.bracketext,
            '             data.motorstyle,
            '             markup
            '         }
            '         Dim ResNext As String = SdsNext(ListNext)
            '         IF Not ResNext = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
            '         End If

            '         '#SdsSize
            '         Dim ListSize As New List(Of Object) From {
            '             data.uniqueid,
            '             width,
            '             drop
            '         }
            '         Dim ResSize As String = SdsSize(ListSize)
            '         IF Not ResSize = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResSize, .field = ""}}
            '         End If
            '     End If

            '     If InArray(data.brackettype, "Linked 2 Blinds (Dep)", "Linked 3 Blinds (Dep)") Then
            '         '#SdsDrop
            '         Dim ListDrop As New List(Of Object) From {
            '             data.uniqueid,
            '             drop
            '         }
            '         Dim ResDrop As String = SdsDrop(ListDrop)
            '         IF Not ResDrop = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDrop, .field = ""}}
            '         End If

            '         '#SdsRollDep
            '         Dim ListRollDep As New List(Of Object) From {
            '             data.uniqueid,
            '             data.roll
            '         }
            '         Dim ResRollDep As String = SdsRollDep(ListRollDep)
            '         IF Not ResRollDep = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResRollDep, .field = ""}}
            '         End If

            '         '#SdsFabric
            '         Dim ListFabric As New List(Of Object) From {
            '             data.uniqueid,
            '             data.fabriccolour,
            '             PriceGroupId
            '         }
            '         Dim ResFabric As String = SdsFabric(ListFabric)
            '         IF Not ResFabric = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResFabric, .field = ""}}
            '         End If

            '         '#SdsNext
            '         Dim ListNext As New List(Of Object) From {
            '             data.uniqueid,
            '             data.tubesize,
            '             data.mounting,
            '             data.room,
            '             data.childsafe,
            '             data.accessory,
            '             data.bracketcovers,
            '             data.bracketext,
            '             data.motorstyle,
            '             markup
            '         }
            '         Dim ResNext As String = SdsNext(ListNext)
            '         IF Not ResNext = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
            '         End If

            '     End If

            '     If InArray(data.brackettype, "Linked 2 Blinds (Ind)", "Linked 3 Blinds (Ind)") Then
            '         '#SdsNext
            '         Dim ListNext As New List(Of Object) From {
            '             data.uniqueid,
            '             data.tubesize,
            '             data.mounting,
            '             data.room,
            '             data.childsafe,
            '             data.accessory,
            '             data.bracketcovers,
            '             data.bracketext,
            '             data.motorstyle,
            '             markup
            '         }
            '         Dim ResNext As String = SdsNext(ListNext)
            '         IF Not ResNext = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResNext, .field = ""}}
            '         End If

            '         '#SdsTubeSize
            '         Dim ListTube As New List(Of Object) From {
            '             data.uniqueid,
            '             data.tubesize
            '         }
            '         Dim ResTube As String = SdsTubeSize(ListTube)
            '         IF Not ResTube = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResTube, .field = ""}}
            '         End If

            '         '#SdsFabric
            '         Dim ListFabric As New List(Of Object) From {
            '             data.uniqueid,
            '             data.fabriccolour,
            '             PriceGroupId
            '         }
            '         Dim ResFabric As String = SdsFabric(ListFabric)
            '         IF Not ResFabric = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResFabric, .field = ""}}
            '         End If

            '     End If

            '     If data.brackettype = "Double and Link System Dep" Then
            '         '#SdsDrop
            '         Dim ListDrop As New List(Of Object) From {
            '             data.uniqueid,
            '             drop
            '         }
            '         Dim ResDrop As String = SdsDrop(ListDrop)
            '         IF Not ResDrop = "200" Then
            '             Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDrop, .field = ""}}
            '         End If

            '         If InArray(data.blindno, "Blind 1", "Blind 2") Then
            '             '#SdsDB2IndFirst
            '             Dim ListDB2IndFirst As New List(Of Object) From {
            '                 data.uniqueid,
            '                 data.fabriccolour,
            '                 PriceGroupId,
            '                 data.roll
            '             }
            '             Dim ResDB2IndFirst As String = SdsDB2First(ListDB2IndFirst)
            '             IF Not ResDB2IndFirst = "200" Then
            '                 Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2IndFirst, .field = ""}}
            '             End If
            '         End If

            '         If InArray(data.blindno, "Blind 3", "Blind 4") Then
            '             '#SdsDB2DepSecond
            '             Dim ListDB2DepSecond As New List(Of Object) From {
            '                 data.uniqueid,
            '                 data.fabriccolour,
            '                 PriceGroupId,
            '                 data.roll
            '             }
            '             Dim ResDB2DepSecond As String = SdsDB2Second(ListDB2DepSecond)
            '             IF Not ResDB2DepSecond = "200" Then
            '                 Return New ErrorResponse With {.error = New ErrorDetail With {.message = ResDB2DepSecond, .field = ""}}
            '             End If
            '         End If

            '     End If

            '     publicCfg.ResetPriceDetail(ItemId)
            '     publicCfg.HitungHarga(data.headerid, ItemId)
            '     publicCfg.HitungSurcharge(data.headerid, ItemId)

            '     Dim dataLog As Object() = {data.headerid, ItemId, "Blinds", data.loginid, "Update Item Order"}
            '     orderCfg.Log_Orders(dataLog)

            '     msg = "Item updated successfully !"

            '     If InStr(data.brackettype, "Linked") > 0 AND data.controltype = "Somfy WF" Then
            '         msg += "<br/><br/><b>Warning :</b>Check SP the availability for linking blind for WF motorised !"
            '     End If
            '     If InStr(data.brackettype, "Linked") > 0 AND data.controltype = "Alpha WF" AndAlso data.motorstyle = "Alpha 2NM Std" Then
            '         msg += "<br/><br/><b>Warning :</b> Check SP the availability for linking blind for WF motorised !"
            '     End If

            ' End If

            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message, .field = ""}}
        End Try
    End Function
End Class
