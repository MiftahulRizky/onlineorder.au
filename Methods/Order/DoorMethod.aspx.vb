
Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Partial Class Methods_Order_DoorMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        '#Submit OrderHeader
        Public Property blindtype As String
        Public Property tubetype As String
        Public Property controltype As String
        Public Property qty As String
        Public Property room As String
        Public Property mounting As String
        Public Property width As String
        Public Property widthmid As String
        Public Property widthbot As String
        Public Property drop As String
        Public Property sliding As String
        Public Property stacking As String
        Public Property trackless As String
        Public Property frametype As String
        Public Property framecolour As String
        Public Property coatingtype As String
        Public Property coatingcolour As String
        Public Property meshtype As String
        Public Property handleside As String
        Public Property handleheight As String
        Public Property handleheightmm As String
        Public Property inswing As String
        Public Property lockcolour As String
        Public Property keyed As String
        Public Property midrail As String
        Public Property bugseal As String
        Public Property closer As String
        Public Property install As String
        Public Property fixing As String
        Public Property fitted As String
        Public Property remove As String
        Public Property petdoortype As String
        Public Property petdoorposition As String
        Public Property petdoorpositionw As String
        Public Property half As String
        Public Property interlock As String
        Public Property extras As String
        Public Property notes As String
        Public Property markup As String
        

        '#aditional param
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
    End Class

    Public Class ExtraItem
        Public Property name As String
        Public Property unit As String
        Public Property value As String
    End Class

    Public Class ParamListData
        Public Property field As String
        Public Property designid As String
        Public Property blindtype As String
        Public Property tubetype As String
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
                    query = String.Format("SELECT TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND Active=1 GROUP BY TubeType ORDER BY TubeType ASC", data.designid, UCase(data.blindtype).ToString())
                    Return GetFormattedData(query, "TubeType", "TubeType")

                Case "controltype"
                    query = String.Format("SELECT Id, ControlType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND TubeType = '{2}' AND Active=1 ORDER BY ControlType ASC", data.designid, UCase(data.blindtype).ToString(), data.tubetype)
                    Return GetFormattedData(query, "Id", "ControlType")

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
            Dim ControlName As String = publicCfg.GetItemData(String.Format("SELECT ControlType FROM HardwareKits WHERE Id = '{0}'", data.controltype))

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

            Dim width As Integer
            Dim widthmid As Integer
            Dim widthbot As Integer
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

            If ControlName = "Hinged Door" Then
                If String.IsNullOrEmpty(data.widthmid) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width middle is required !",.field = "widthmid"}}
                End If
                If Not Integer.TryParse(data.widthmid, widthmid) OrElse widthmid <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width middle must be a positive integer !",.field = "widthmid"}}
                End If
                If widthmid < 150 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width middle must be less than or equal to 150 !",.field = "widthmid"}}
                End If
                If widthmid > 6000 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width middle must be less than or equal to 6000 !",.field = "widthmid"}}
                End If

                If String.IsNullOrEmpty(data.widthbot) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width bottom is required !",.field = "widthbot"}}
                End If
                If Not Integer.TryParse(data.widthbot, widthbot) OrElse widthbot <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width bottom must be a positive integer !",.field = "widthbot"}}
                End If
                If widthbot < 150 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width bottom must be less than or equal to 150 !",.field = "widthbot"}}
                End If
                If widthbot > 6000 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width bottom must be less than or equal to 6000 !",.field = "widthbot"}}
                End If
            End If

            Dim drop As Integer
            If String.IsNullOrEmpty(data.drop) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "height is required !",.field = "drop"}}
            End If
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "height must be a positive integer !",.field = "drop"}}
            End If
            If drop < 150 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "height must be greater than or equal to 150 !",.field = "drop"}}
            End If
            If drop > 3200 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "height must be less than or equal to 3200 !",.field = "drop"}}
            End If

            If InArray(BlindName, "Basic Door") AND InArray(data.tubetype, "Retractable Pleated") Then
                If String.IsNullOrEmpty(data.sliding) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "sliding is required !",.field = "sliding"}}
                End If

                If String.IsNullOrEmpty(data.stacking) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "stacking is required !",.field = "stacking"}}
                End If

                If String.IsNullOrEmpty(data.trackless) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "trackless is required !",.field = "trackless"}}
                End If
            End If

            If InArray(BlindName, "Basic Door", "Safety Door") AND NOT InArray(data.tubetype, "Retractable Pleated") Then
                If String.IsNullOrEmpty(data.frametype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "grille type is required !",.field = "frametype"}}
                End If
            End If

            If InArray(BlindName, "Basic Door", "Safety Door", "Security Door") Then
                Dim lblFrameColour As String = "grille colour"
                If InArray(BlindName, "Security Door") Then lblFrameColour = "frame colour"
                If Not String.IsNullOrEmpty(data.frametype) OR InArray(BlindName, "Security Door") OR InArray(data.tubetype, "Retractable Pleated")  Then
                    If String.IsNullOrEmpty(data.framecolour) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = String.Format("{0} is required !", lblFrameColour),.field = "framecolour"}}
                    End If
                End If
            End If

            If Not String.IsNullOrEmpty(data.framecolour) Then
                If data.framecolour = "Powder Coating" And String.IsNullOrEmpty(data.coatingtype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "coating type is required !",.field = "coatingtype"}}
                End If
                If Not String.IsNullOrEmpty(data.coatingtype) AND String.IsNullOrEmpty(data.coatingcolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "coating colour is required !",.field = "coatingcolour"}}
                End If
            End If

            If InArray(BlindName, "Basic Door", "Safety Door") AND NOT InArray(data.tubetype, "Retractable Pleated") Then
                If Not data.frametype = "Ultra Barrier Screen Door" Then
                    If String.IsNullOrEmpty(data.meshtype) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "mesh is required !",.field = "meshtype"}}
                    End If
                End IF
            End IF

            Dim handleheightmm As Integer
            If InArray(BlindName, "Basic Door", "Safety Door", "Security Door") AND NOT InArray(data.tubetype, "Retractable Pleated") Then
                If String.IsNullOrEmpty(data.handleside) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "handle side is required !",.field = "handleside"}}
                End If
                If Not InArray(data.handleside, "Sidelight", "") Then
                    If String.IsNullOrEmpty(data.handleheight) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "handle height is required !",.field = "handleheight"}}
                    End If
                End If

                Dim handleheight As String = InArray(data.handleheight, "Lock Height", "Specify")
                If ControlName = "Hinged Door" Then
                    handleheight = InArray(data.handleheight, "Lock Height", "Centre of Handle", "Bot of Tongue", "Centre of Tongue", "To Centre of Handle", "To Bottom of Tongue", "To Centre of Tongue", "Specify", "Keyed Lock")
                End If
                If Not String.IsNullOrEmpty(data.handleheight) AND handleheight Then
                    If String.IsNullOrEmpty(data.handleheightmm) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "handle height in mm is required !",.field = "handleheightmm"}}
                    End If

                    If Not Integer.TryParse(data.handleheightmm, handleheightmm) OrElse handleheightmm <= 0 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "height must be a positive integer !",.field = "handleheightmm"}}
                    End If
                    If handleheightmm < 150 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "height must be greater than or equal to 150 !",.field = "handleheightmm"}}
                    End If
                    If handleheightmm > 3200 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "height must be less than or equal to 3200 !",.field = "handleheightmm"}}
                    End If
                End If

                
                If InArray(ControlName, "Hinged Door") AND NOT InArray(BlindName, "Security Door") Then
                    ' If String.IsNullOrEmpty(data.inswing) Then
                    '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "inswing is required !",.field = "inswing"}}
                    ' End If

                    If String.IsNullOrEmpty(data.lockcolour) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "lock colour is required !",.field = "lockcolour"}}
                    End If
                End If
                
                ' If String.IsNullOrEmpty(data.keyed) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "keyed is required !",.field = "keyed"}}
                ' End If
                
                If Not InArray(data.frametype,"Heavy Duty Diamond") OR InArray(BlindName,"Security Door") Then
                    If String.IsNullOrEmpty(data.midrail) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "midrail is required !",.field = "midrail"}}
                    End If
                End IF

                If String.IsNullOrEmpty(data.bugseal) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bugseal is required !",.field = "bugseal"}}
                End If

                If InArray(ControlName, "Hinged Door") Then
                    If String.IsNullOrEmpty(data.closer) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "closer is required !",.field = "closer"}}
                    End If
                End IF
            End If

            If InArray(BlindName, "Basic Door", "Safety Door") AND NOT InArray(data.tubetype, "Retractable Pleated") Then
                
                ' If String.IsNullOrEmpty(data.install) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "install is required !",.field = "install"}}
                ' End If

                If InArray(ControlName, "Hinged Door") Then
                    ' If String.IsNullOrEmpty(data.fixing) Then
                    '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fixing is required !",.field = "fixing"}}
                    ' End If

                    ' If String.IsNullOrEmpty(data.fitted) Then
                    '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fitted is required !",.field = "fitted"}}
                    ' End If

                    ' If String.IsNullOrEmpty(data.remove) Then
                    '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "remove product is required !",.field = "remove"}}
                    ' End If
                End IF

                ' If String.IsNullOrEmpty(data.petdoortype) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "petdoor type is required !",.field = "petdoortype"}}
                ' End If

                If Not String.IsNullOrEmpty(data.petdoortype) Then
                    If String.IsNullOrEmpty(data.petdoorposition) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "petdoor position is required !",.field = "petdoorposition"}}
                    End If
                End If

                If Not String.IsNullOrEmpty(data.petdoorposition) AND data.petdoorposition = "Specify" Then
                    If String.IsNullOrEmpty(data.petdoorpositionw) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "petdoor position in write is required !",.field = "petdoorpositionw"}}
                    End If
                End If

                ' If String.IsNullOrEmpty(data.half) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "half panel is required !",.field = "half"}}
                ' End If
            End If
            
            If InArray(BlindName, "Basic Door", "Safety Door", "Security Door") AND NOT InArray(data.tubetype, "Retractable Pleated") Then
                Dim lblInterlock As String = "interlock"
                If InArray(ControlName, "Hinged Door") Then lblInterlock = "Adaptors and options"
                If InArray(BlindName, "Security Door") Then lblInterlock = "Adaptors"
                ' If String.IsNullOrEmpty(data.interlock) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = String.Format("{0} is required !", lblInterlock),.field = "interlock"}}
                ' End If               
            End If

            Dim ExtrasList As List(Of ExtraItem)
            If Not String.IsNullOrWhiteSpace(data.extras) OrElse data.extras = "[]" Then
                Try
                    ExtrasList = JsonConvert.DeserializeObject(Of List(Of ExtraItem))(data.extras)
                Catch ex As Exception
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "Invalid extras format", .field = "extras"}}
                End Try

                For Each item In ExtrasList
                    If String.IsNullOrEmpty(item.value) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = item.name & " (" & item.unit & ") is required", .field = "extras"}}
                    End If

                    ' If item.unit = "mm" Then
                    ' End If
                    Dim num As Decimal
                    If Not Decimal.TryParse(item.value, num) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = item.name & " must be numeric", .field = "extras"}}
                    End If
                    If num < 1 Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = item.name & " must be greater than 0", .field = "extras"}}
                    End If
                Next
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

            Dim SoeId As String = publicCfg.GetSoeKitId(data.controltype)
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            Dim PriceGroupName As String = DesignName
            If InArray(BlindName, "Security Door", "Basic Door", "Safety Door") Then
                Dim Sliding As String = "Single Sliding"
                If data.sliding = "Double Sliding Pleated" Then Sliding = "Double Sliding"

                PriceGroupName = String.Format("{0} - {1}", BlindName, data.tubetype)
                If data.tubetype = "Retractable Pleated" Then
                    PriceGroupName = String.Format("{0} - {1} #{2}", BlindName, data.tubetype, Sliding)
                End If
            End If

            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            If PriceGroupId = "" Then
                Throw New Exception("Something went wrong !")
            End If

            Dim squareMetre As Decimal = Math.Round(width * drop / 1000000, 4)
            Dim linearMetre As Decimal = Math.Round(width / 1000, 4)

            If InArray(BlindName, "Basic Door") AND InArray(data.tubetype, "Retractable Pleated") Then
                data.frametype = ""
                data.meshtype = ""
                data.handleside = ""
                data.handleheight = ""
                data.inswing = ""
                data.lockcolour = ""
                data.keyed = ""
                data.midrail = ""
                data.bugseal = ""
                data.closer = ""
                data.install = ""
                data.fixing = ""
                data.fitted = ""
                data.remove = ""
                data.petdoortype = ""
                data.petdoorposition = ""
                data.half = ""
                data.interlock = ""
            End If

            If InArray(BlindName, "Basic Door", "Safety Door") Then
                If Not InArray(data.tubetype, "Retractable Pleated") Then
                    data.sliding = ""
                    data.stacking = ""
                    data.trackless = ""
                End If
                If data.frametype = "Ultra Barrier Screen Door" Then
                    data.meshtype = ""
                End If
                If data.frametype = "Heavy Duty Diamond" Then
                    data.midrail = ""
                End If

                If InArray(ControlName, "Sliding Door") Then
                    data.inswing = ""
                    data.lockcolour = ""
                    data.closer = ""
                    data.fixing = ""
                    data.fitted = ""
                    data.remove = ""
                End IF
            End IF

            If InArray(BlindName, "Security Door") Then
                data.sliding = ""
                data.stacking = ""
                data.trackless = ""
                data.frametype = ""
                data.meshtype = ""
                data.inswing = ""
                data.lockcolour = ""
                data.install = ""
                data.fixing = ""
                data.fitted = ""
                data.remove = ""
                data.petdoortype = ""
                data.petdoorposition = ""
                data.half = ""
                If InArray(ControlName, "Sliding Door") Then
                    data.closer = ""
                End IF
            End If

            If InArray(ControlName, "Hinged Door") Then
                If data.handleheight = "Tulip A Latch" Then
                    data.handleheightmm = ""
                    handleheightmm = 0
                End If
            End If

            If InArray(ControlName, "Sliding Door") Then
                data.widthmid = ""
                data.widthbot = ""
                widthmid = 0
                widthbot = 0

                If InArray(data.handleheight, "Lock Height", "Specify") Then
                    data.handleheightmm = ""
                    handleheightmm = 0
                End If
            End IF

            If Not data.framecolour = "Powder Coating" Then
                data.coatingtype = ""
                data.coatingcolour = ""
            End If

            ' Throw New Exception(DesignName)

            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()
            
                Dim Field As String = "Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, PriceGroupId, Qty, Location, Mounting, Width, WidthMiddle, WidthBottom, [Drop], BottomTrackType, StackPosition, TilterPosition, FrameType, FrameColour, FrameLeft, FrameRight, MeshType, Brace, SlatSize, SlatQty, PortHole, PlungerPin, Batten, MidrailCritical, FlatType, ChildSafe, BracketCover, Fitting, Cleat, BracketExtension, TrackType, TrackColour, WandPosition, AcornPlasticColour, Accessory, AdditionalMotor, SquareMetre, LinearMetre, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active"

                Dim Values As String = "@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @PriceGroupId, @Qty, @Location, @Mounting, @Width, @WidthMiddle, @WidthBottom, @Drop, @BottomTrackType, @StackPosition, @TilterPosition, @FrameType, @FrameColour, @FrameLeft, @FrameRight, @MeshType, @Brace, @SlatSize, @SlatQty, @PortHole, @PlungerPin, @Batten, @MidrailCritical, @FlatType, @ChildSafe, @BracketCover, @Fitting, @Cleat, @BracketExtension, @TrackType, @TrackColour, @WandPosition, @AcornPlasticColour, @Accessory, @AdditionalMotor, @SquareMetre, @LinearMetre, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1"

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(String.Format("INSERT INTO OrderDetails({0}) VALUES ({1})", Field, Values), thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.controltype), DBNull.Value, UCase(data.controltype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@WidthMiddle", widthmid)
                        myCmd.Parameters.AddWithValue("@WidthBottom", widthbot)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@BottomTrackType", data.sliding)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stacking)
                        myCmd.Parameters.AddWithValue("@TilterPosition", data.trackless)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.coatingtype)
                        myCmd.Parameters.AddWithValue("@FrameRight", data.coatingcolour)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@Brace", data.handleside)
                        myCmd.Parameters.AddWithValue("@SlatSize", data.handleheight)
                        myCmd.Parameters.AddWithValue("@SlatQty", data.handleheightmm)
                        myCmd.Parameters.AddWithValue("@PortHole", data.inswing)
                        myCmd.Parameters.AddWithValue("@PlungerPin", data.lockcolour)
                        myCmd.Parameters.AddWithValue("@Batten", data.keyed)
                        myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrail)
                        myCmd.Parameters.AddWithValue("@FlatType", data.bugseal)
                        myCmd.Parameters.AddWithValue("@ChildSafe", data.closer)
                        myCmd.Parameters.AddWithValue("@BracketCover", data.install)
                        myCmd.Parameters.AddWithValue("@Fitting", data.fixing)
                        myCmd.Parameters.AddWithValue("@Cleat", data.fitted)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.remove)
                        myCmd.Parameters.AddWithValue("@TrackType", data.petdoortype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.petdoorposition)
                        myCmd.Parameters.AddWithValue("@WandPosition", data.petdoorpositionw)
                        myCmd.Parameters.AddWithValue("@AcornPlasticColour", data.half)
                        myCmd.Parameters.AddWithValue("@Accessory", data.interlock)
                        myCmd.Parameters.AddWithValue("@AdditionalMotor", data.extras)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
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

                Dim dataLog As Object() = {data.headerid, ItemId, "Door and Window", data.loginid, "Add Item Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Item added successfully !"
                
            End If

            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then

                Dim ItemId As String = data.itemid
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@width, WidthMiddle=@WidthMiddle, WidthBottom=@WidthBottom, [Drop]=@Drop, BottomTrackType=@BottomTrackType, StackPosition=@StackPosition, TilterPosition=@TilterPosition, FrameType=@FrameType, FrameColour=@FrameColour, FrameLeft=@FrameLeft, FrameRight=@FrameRight, MeshType=@MeshType, Brace=@Brace, SlatSize=@SlatSize, SlatQty=@SlatQty, PortHole=@PortHole, PlungerPin=@PlungerPin, Batten=@Batten, MidrailCritical=@MidrailCritical, FlatType=@FlatType, ChildSafe=@ChildSafe, BracketCover=@BracketCover, Fitting=@Fitting, Cleat=@Cleat, BracketExtension=@BracketExtension, TrackType=@TrackType, TrackColour=@TrackColour, WandPosition=@WandPosition, AcornPlasticColour=@AcornPlasticColour, Accessory=@Accessory, AdditionalMotor=@AdditionalMotor, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.controltype), DBNull.Value, UCase(data.controltype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@WidthMiddle", widthmid)
                        myCmd.Parameters.AddWithValue("@WidthBottom", widthbot)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@BottomTrackType", data.sliding)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stacking)
                        myCmd.Parameters.AddWithValue("@TilterPosition", data.trackless)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.coatingtype)
                        myCmd.Parameters.AddWithValue("@FrameRight", data.coatingcolour)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@Brace", data.handleside)
                        myCmd.Parameters.AddWithValue("@SlatSize", data.handleheight)
                        myCmd.Parameters.AddWithValue("@SlatQty", data.handleheightmm)
                        myCmd.Parameters.AddWithValue("@PortHole", data.inswing)
                        myCmd.Parameters.AddWithValue("@PlungerPin", data.lockcolour)
                        myCmd.Parameters.AddWithValue("@Batten", data.keyed)
                        myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrail)
                        myCmd.Parameters.AddWithValue("@FlatType", data.bugseal)
                        myCmd.Parameters.AddWithValue("@ChildSafe", data.closer)
                        myCmd.Parameters.AddWithValue("@BracketCover", data.install)
                        myCmd.Parameters.AddWithValue("@Fitting", data.fixing)
                        myCmd.Parameters.AddWithValue("@Cleat", data.fitted)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.remove)
                        myCmd.Parameters.AddWithValue("@TrackType", data.petdoortype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.petdoorposition)
                        myCmd.Parameters.AddWithValue("@WandPosition", data.petdoorpositionw)
                        myCmd.Parameters.AddWithValue("@AcornPlasticColour", data.half)
                        myCmd.Parameters.AddWithValue("@Accessory", data.interlock)
                        myCmd.Parameters.AddWithValue("@AdditionalMotor", data.extras)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
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

                Dim dataLog As Object() = {data.headerid, ItemId, "Door and Window", data.loginid, "Update Item Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Item updated successfully !"

            End If


            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message, .field = ""}}
        End Try
    End Function

End Class
