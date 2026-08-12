
Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Partial Class Methods_VerishadesMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        '#Submit OrderHeader
        Public Property blindtype As String
        Public Property tubetype As String
        Public Property qty As String
        Public Property room As String
        Public Property sizetype As String
        Public Property dropfloor As String
        Public Property mounting As String
        Public Property width As String
        Public Property drop As String
        Public Property fabrictype As String
        Public Property fabriccolour As String
        Public Property blindsize As String
        Public Property stack As String
        Public Property tracktype As String
        Public Property trackcolour As String
        Public Property wandsize As String
        Public Property wandcolour As String
        Public Property customsize As String
        Public Property bracket As String
        Public Property tape As String
        Public Property carrier As String
        Public Property carrieroverride As String
        Public Property spacer As String
        Public Property spaceroverride As String
        Public Property slat As String
        Public Property slatoverride As String
        Public Property slatqty As String
        Public Property slatqtyoverride As String
        Public Property endslats As String
        Public Property endslatsoverride As String
        Public Property totalslats As String
        Public Property fabricqty As String
        Public Property fabricqtyoverride As String

        Public Property notes As String
        Public Property markup As String

        

        '#aditional param
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
        Public Property rolename As String
    End Class

    Public Class ParamListData
        Public Property field As String
        Public Property designid As String
        Public Property blindtype As String
        Public Property tubetype As String
        Public Property fabrictype As String
    End Class

    Private Class SpacerInfo
        Public MinWidth As Integer
        Public MaxWidth As Integer
        Public Spacer1Type As String
        Public CarriersQty As Integer
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
    Public Shared Function BindListData(ByVal data As ParamListData) As Object
        Try
            Dim query As String = ""
            Dim resultList As New List(Of Dictionary(Of String, String))()

            Select Case data.field.ToLower()
                Case "blindtype"
                    query = String.Format("SELECT Id, Name FROM Blinds WHERE DesignId='{0}' AND Active=1 ORDER BY Name ASC", data.designid)
                    Return GetFormattedData(query, "Id", "Name")

                Case "tubetype"
                    query = String.Format("SELECT Id, TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND Active=1 ORDER BY TubeType ASC", data.designid, UCase(data.blindtype).ToString())
                    Return GetFormattedData(query, "Id", "TubeType")

                Case "controltype"
                    query = String.Format("SELECT Id, ControlType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND TubeType = '{2}' AND Active=1 ORDER BY ControlType ASC", data.designid, UCase(data.blindtype).ToString(), data.tubetype)
                    Return GetFormattedData(query, "Id", "ControlType")

                Case "fabrictype"
                    query = String.Format("SELECT Type FROM Fabrics WHERE DesignId='{0}' AND Active='1' GROUP BY Type ORDER BY Type ASC", data.designid)
                    Return GetFormattedData(query, "Type", "Type")

                Case "fabriccolour"
                    query = String.Format("SELECT Id, UPPER(Colour) AS Colour FROM Fabrics WHERE DesignId='{0}' AND Active='1' AND Type='{1}' ORDER BY Name ASC", data.designid, data.fabrictype)
                    Return GetFormattedData(query, "Id", "Colour")

                Case Else
                    Return New With {.error = "Invalid field"}
            End Select

        Catch ex As Exception
            Return New With {.error = ex.Message}
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

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindItemOrder(ByVal itemid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_details WHERE Id = '{0}'", itemid))

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
            Dim msg As String = "200"
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

            If InArray(BlindName, "Single") Then
                If String.IsNullOrEmpty(data.sizetype) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "size type is required !", .field = "sizetype"}}
                End If

                If data.sizetype = "Opening Size" AND data.mounting = "Face Fit"
                    If String.IsNullOrEmpty(data.dropfloor) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = "drop to the floor is required !", .field = "dropfloor"}}
                    End If
                End If
            End IF

            If String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "mounting type is required !", .field = "mounting"}}
            End If

            Dim width As Integer
            Dim drop As Integer
            If InArray(BlindName, "Single", "Track Only") Then
                If String.IsNullOrEmpty(data.width) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width is required !",.field = "width"}}
                End If
                If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be a positive integer !",.field = "width"}}
                End If
                If width < 150 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "minimum width is 150!",.field = "width"}}
                End If
                If width > 6000 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "maximum width is 6000!",.field = "width"}}
                End If
            End If

            If InArray(BlindName, "Single", "Slat Only") Then
                If String.IsNullOrEmpty(data.drop) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop is required !",.field = "drop"}}
                End If
                If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be a positive integer !",.field = "drop"}}
                End If
                If drop < 150 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "minimum drop is 150!",.field = "drop"}}
                End If
                If drop > 3200 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "maximum drop is 3200!",.field = "drop"}}
                End If
            End If

            If InArray(BlindName, "Single", "Slat Only") Then
                If String.IsNullOrEmpty(data.fabrictype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type is required !",.field = "fabrictype"}}
                End If
                If Not String.IsNullOrEmpty(data.fabrictype) AND String.IsNullOrEmpty(data.fabriccolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour is required !",.field = "fabriccolour"}}
                End If
            End If

           
            If String.IsNullOrEmpty(data.stack) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "stack configuration is required !",.field = "stack"}}
            End If
            
            If String.IsNullOrEmpty(data.tracktype) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "track type is required !",.field = "tracktype"}}
            End If
            If Not String.IsNullOrEmpty(data.tracktype) AND String.IsNullOrEmpty(data.trackcolour)Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "track colour is required !",.field = "trackcolour"}}
            End If

            Dim customsize As Integer
            If InArray(BlindName, "Single", "Track Only") Then
                If String.IsNullOrEmpty(data.wandsize) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "wand size is required !",.field = "wandsize"}}
                End If
                If Not String.IsNullOrEmpty(data.wandsize) AND String.IsNullOrEmpty(data.wandcolour)Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "wand colour is required !",.field = "wandcolour"}}
                End If
                If data.wandsize = "Custom" AND String.IsNullOrEmpty(data.customsize)Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "custom size is required !",.field = "customsize"}}
                End If
                If Not String.IsNullOrEmpty(data.customsize) Then
                    If Not Integer.TryParse(data.customsize, customsize) OrElse customsize <= 0 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "custom size must be a positive integer !",.field = "customsize"}}
                    End If
                End If

                If String.IsNullOrEmpty(data.bracket) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bracket is required !",.field = "bracket"}}
            End If
            End If

            If String.IsNullOrEmpty(data.tape) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "tape colour is required !",.field = "tape"}}
            End If

            Dim carrier As Integer
            If data.carrieroverride = "True" Then
                If String.IsNullOrEmpty(data.carrier) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "carrier qty is required !",.field = "carrier"}}
                End If

                If Not Integer.TryParse(data.carrier, carrier) OrElse carrier <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "carrier qty must be a positive integer !",.field = "carrier"}}
                End If
            End If

            Dim spacer As Integer
            If data.spaceroverride = "True" Then
                If String.IsNullOrEmpty(data.spacer) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "spacer is required !",.field = "spacer"}}
                End If

                If Not Integer.TryParse(data.spacer, spacer) OrElse spacer <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "spacer must be a positive integer !",.field = "spacer"}}
                End If
            End If

            Dim slat As Integer
            If data.slatoverride = "True" Then
                If String.IsNullOrEmpty(data.slat) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "slat size is required !",.field = "slat"}}
                End If

                If Not Integer.TryParse(data.slat, slat) OrElse slat <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "slat size must be a positive integer !",.field = "slat"}}
                End If
            End If

            Dim slatqty As Integer
            If data.slatqtyoverride = "True" Then
                If String.IsNullOrEmpty(data.slatqty) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "slat qty is required !",.field = "slatqty"}}
                End If

                If Not Integer.TryParse(data.slatqty, slatqty) OrElse slatqty <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "slat qty must be a positive integer !",.field = "slatqty"}}
                End If
            End If

            Dim endslats As Integer
            If data.endslatsoverride = "True" Then
                If String.IsNullOrEmpty(data.endslats) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "end slats is required !",.field = "endslats"}}
                End If

                If Not Integer.TryParse(data.endslats, endslats) OrElse endslats <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "end slats must be a positive integer !",.field = "endslats"}}
                End If
            End If

            Dim fabricqty As Integer
            If data.fabricqtyoverride = "True" Then
                If String.IsNullOrEmpty(data.fabricqty) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric qty is required !",.field = "fabricqty"}}
                End If

                If Not Integer.TryParse(data.fabricqty, fabricqty) OrElse fabricqty <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric qty must be a positive integer !",.field = "fabricqty"}}
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
                markup = 0
            End If


            Dim SoeId As String = publicCfg.GetSoeKitId(data.tubetype)
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = String.Format("{0}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            Dim PriceGroupName As String = BlindName
            Dim FabricGroup As String = publicCfg.GetFabricGroup(data.fabriccolour)
            If BlindName = "Single" Then
                PriceGroupName = String.Format("{0} - {1}", DesignName, FabricGroup)
            End If
            If BlindName = "Slat Only" Then
                PriceGroupName = String.Format("{0} - {1}", BlindName, FabricGroup)
            End If

            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            If PriceGroupId = "" Then
                Throw New Exception("Something went wrong !")
            End If

            Dim WidthDeduc As Integer = 0
            Dim DropDeduc As Integer = 0
            If data.sizetype = "Opening Size" Then
                Dim DeducWidth As String = publicCfg.GetItemData(String.Format("SELECT Width FROM Deduc WHERE DesignId = '{0}' AND BlindId = '{1}' AND Mounting ='{2}'", data.designid, data.blindtype, data.mounting))

                Dim DeducDrop As String = publicCfg.GetItemData(String.Format("SELECT [Drop] FROM Deduc WHERE DesignId = '{0}' AND BlindId = '{1}' AND Mounting ='{2}'", data.designid, data.blindtype, data.mounting))

                Dim DropFloor As String = "0"
                If data.dropfloor = "Yes" Then
                    DropFloor = publicCfg.GetItemData(String.Format("SELECT DropFloor FROM Deduc WHERE DesignId = '{0}' AND BlindId = '{1}' AND Mounting ='{2}'", data.designid, data.blindtype, data.mounting))
                End If

                WidthDeduc = width + CInt(DeducWidth)
                DropDeduc = drop + CInt(DeducDrop) + CInt(DropFloor)
            End If


            '#Carrier Qty
            If data.carrieroverride = "True" Then
                Dim CreateJSON = New With { .value = carrier, .checked = True}
                data.carrier = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            Else If data.carrieroverride = "False" Then
                For Each item In SpacerVerishades
                    If WidthDeduc >= item.MinWidth AndAlso WidthDeduc <= item.MaxWidth Then
                        carrier = item.CarriersQty
                        Exit For
                    End If
                Next

                Dim CreateJSON = New With { .value = carrier, .checked = False}
                data.carrier = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            End If

            '#Spacer Size
            If data.spaceroverride = "True" Then
                Dim CreateJSON = New With { .value = spacer, .checked = True}
                data.spacer = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            Else If data.spaceroverride = "False" Then
                For Each item In SpacerVerishades
                    If WidthDeduc >= item.MinWidth AndAlso WidthDeduc <= item.MaxWidth Then
                        spacer = item.Spacer1Type
                        Exit For
                    End If
                Next

                Dim CreateJSON = New With { .value = spacer, .checked = False}
                data.spacer = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            End If

            '#Slat Size
            If data.slatoverride = "True" Then
                Dim CreateJSON = New With { .value = slat, .checked = True}
                data.slat = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            Else If data.slatoverride = "False" Then
                Dim ListParam As New List(Of Object) From {
                    BlindName,
                    data.mounting,
                    data.tracktype,
                    DropDeduc
                }
                slat = FindSlatSize(ListParam)

                Dim CreateJSON = New With { .value = slat, .checked = False}
                data.slat = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            End If

            '#Slat Qty
            If data.slatqtyoverride = "True" Then
                Dim CreateJSON = New With { .value = slatqty, .checked = True}
                data.slatqty = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            Else If data.slatqtyoverride = "False" Then
                Dim ListParam As New List(Of Object) From {
                    "Slat Qty",
                    BlindName,
                    data.stack,
                    carrier,
                    data.carrieroverride
                }
                slatqty = FindSlats(ListParam)

                Dim CreateJSON = New With { .value = slatqty, .checked = False}
                data.slatqty = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            End If

            '#End Slats
            If data.endslatsoverride = "True" Then
                Dim CreateJSON = New With { .value = endslats, .checked = True}
                data.endslats = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            Else If data.endslatsoverride = "False" Then
                Dim ListParam As New List(Of Object) From {
                    "End Slats",
                    BlindName,
                    data.stack,
                    carrier,
                    data.carrieroverride
                }
                endslats = FindSlats(ListParam)

                Dim CreateJSON = New With { .value = endslats, .checked = False}
                data.endslats = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            End If

            '#Total Slats
            data.totalslats = slatqty + endslats 

            '#Fabric Qty
            If data.fabricqtyoverride = "True" Then
                Dim CreateJSON = New With { .value = fabricqty, .checked = True}
                data.fabricqty = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            Else If data.fabricqtyoverride = "False" Then
                Dim ListParam As New List(Of Object) From {
                    BlindName,
                    slat,
                    data.totalslats,
                    DropDeduc
                }
                fabricqty = FindFabricQty(ListParam)

                Dim CreateJSON = New With { .value = fabricqty, .checked = False}
                data.fabricqty = Newtonsoft.Json.JsonConvert.SerializeObject({CreateJSON})
            End If

            If BlindName = "Single" Then
                data.blindsize = ""
            End If

            If BlindName = "Slat Only" Then
                data.sizetype = ""
                data.dropfloor = ""
                data.width = ""
                width = 0
                ' data.stack = ""
                ' data.tracktype = ""
                ' data.trackcolour = ""
                data.wandsize = ""
                data.wandcolour = ""
                data.customsize = ""
                data.bracket = ""
            End If

            If BlindName = "Track Only" Then
                data.sizetype = ""
                data.dropfloor = ""
                data.drop = ""
                drop = 0
                data.fabrictype = ""
                data.fabriccolour = ""
                data.blindsize = ""
                data.slat =""
                data.slatqty =""
                data.endslats =""
                data.totalslats = 0
                data.fabricqty = ""
            End If

            If data.wandsize = "Custom" Then
                data.wandsize = data.customsize
            End IF

            If data.sizetype = "Make Size" OR (data.sizetype = "Opening Size" AND data.mounting = "Reveal Fit") Then
                data.dropfloor = ""
            End If

            ' throw New Exception(data.rolename)

            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()
            
                Dim Field As String = "Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, FabricId, PriceGroupId, Qty, Location, LouvreSize, LouvrePosition, Mounting, Width, [Drop], BlindSize,  StackPosition, TrackType, TrackColour, WandLength, WandColour, BracketOption, BracketColour, FrameType, FrameLeft, Layout, LayoutSpecial, SlatSize, SlatQty, TubeSize, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active"

                Dim Values As String = "@Id, @HeaderId, 'Blind 1', @KitId, @SoeKitId, @ExactId, @FabricId, @PriceGroupId,  @Qty, @Location,  @LouvreSize, @LouvrePosition, @Mounting, @Width, @Drop, @BlindSize, @StackPosition, @TrackType, @TrackColour, @WandLength, @WandColour, @BracketOption, @BracketColour, @FrameType, @FrameLeft, @Layout, @LayoutSpecial, @SlatSize, @SlatQty, @TubeSize, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1"

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(String.Format("INSERT INTO OrderDetails({0}) VALUES ({1})", Field, Values), thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.tubetype), DBNull.Value, UCase(data.tubetype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.sizetype)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.dropfloor)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@BlindSize", data.blindsize)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stack)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                        myCmd.Parameters.AddWithValue("@WandLength", data.wandsize)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                        myCmd.Parameters.AddWithValue("@BracketOption", data.bracket)
                        myCmd.Parameters.AddWithValue("@BracketColour", data.tape)
                        myCmd.Parameters.AddWithValue("@FrameType", data.carrier)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.spacer)
                        myCmd.Parameters.AddWithValue("@Layout", data.slat)
                        myCmd.Parameters.AddWithValue("@LayoutSpecial", data.slatqty)
                        myCmd.Parameters.AddWithValue("@SlatSize", data.endslats)
                        myCmd.Parameters.AddWithValue("@SlatQty", data.totalslats)
                        myCmd.Parameters.AddWithValue("@TubeSize", data.fabricqty)
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

                Dim dataLog As Object() = {data.headerid, ItemId, "Blinds", data.loginid, "Add Item Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Item added successfully !"
                
            End If

             If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then

                Dim ItemId As String = data.itemid
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, LouvreSize=@LouvreSize, LouvrePosition=@LouvrePosition, Mounting=@Mounting, Width=@width, [Drop]=@Drop, BlindSize=@BlindSize, StackPosition=@StackPosition, TrackType=@TrackType, TrackColour=@TrackColour, WandLength=@WandLength, WandColour=@WandColour, BracketOption=@BracketOption, BracketColour=@BracketColour, FrameType=@FrameType, FrameLeft=@FrameLeft, Layout=@Layout, LayoutSpecial=@LayoutSpecial, SlatSize=@SlatSize, SlatQty=@SlatQty, TubeSize=@TubeSize, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.tubetype), DBNull.Value, UCase(data.tubetype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.sizetype)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.dropfloor)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@BlindSize", data.blindsize)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stack)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                        myCmd.Parameters.AddWithValue("@WandLength", data.wandsize)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                        myCmd.Parameters.AddWithValue("@BracketOption", data.bracket)
                        myCmd.Parameters.AddWithValue("@BracketColour", data.tape)
                        myCmd.Parameters.AddWithValue("@FrameType", data.carrier)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.spacer)
                        myCmd.Parameters.AddWithValue("@Layout", data.slat)
                        myCmd.Parameters.AddWithValue("@LayoutSpecial", data.slatqty)
                        myCmd.Parameters.AddWithValue("@SlatSize", data.endslats)
                        myCmd.Parameters.AddWithValue("@SlatQty", data.totalslats)
                        myCmd.Parameters.AddWithValue("@TubeSize", data.fabricqty)
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

                Dim dataLog As Object() = {data.headerid, ItemId, "Blinds", data.loginid, "Update Item Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Item updated successfully !"

            End If

            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Dim msg As String = ex.Message
            If Not data.rolename = "Administrator" Then msg = "Please contact our IT team at support@onlineorder.au"
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = msg, .field = ""}}
        End Try
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

End Class
