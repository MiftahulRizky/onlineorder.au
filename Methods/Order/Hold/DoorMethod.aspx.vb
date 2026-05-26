
Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_Order_DoorMethod
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
        Public Property mounting As String
        Public Property widthtop As String
        Public Property widthmiddle As String
        Public Property widthbottom As String
        Public Property widthmin As String
        Public Property drop As String
        Public Property frametype As String
        Public Property framecolour As String
        Public Property fitted As String
        Public Property meshtype As String
        Public Property fixing As String
        Public Property top As String
        Public Property hingetype As String
        Public Property locktype As String
        Public Property lockhandling As String
        Public Property sideframe As String
        Public Property headframe As String
        Public Property extframe As String
        Public Property extwidth As String
        Public Property extdrop As String
        Public Property slambar As String
        Public Property levelhandler As String
        Public Property lock As String
        Public Property layoutcode As String
        Public Property handleposition As String
        Public Property handlemeasure As String
        Public Property handleheight As String
        Public Property midrailposition As String
        Public Property midrailrequest As String
        Public Property petdoortype As String
        Public Property petdoorposition As String
        Public Property triplelock As String
        Public Property latchbass As String
        Public Property bugseal As String
        Public Property doorcloser As String
        Public Property boldpatio As String
        Public Property crossbrace As String
        Public Property notes As String
        Public Property markup As String
        

        '#aditional param
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
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
    Public Shared Function BindTubeType(ByVal designid As String, ByVal blindid As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT * FROM HardwareKits WHERE DesignId='{0}' AND BlindId = '{1}' AND Active=1 ORDER BY Name ASC", designid, UCase(blindid).ToString())
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("TubeType").ToString()}
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

            If String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "opening is required !", .field = "mounting"}}
            End If

            Dim LblWidth As String = "width"
            If InStr(BlindName, "Door") > 0 Then
                LblWidth = "width top"
            End If
            Dim widthtop As Integer
            Dim widthmiddle As Integer
            Dim widthbottom As Integer
            Dim widthmin As Integer
            If String.IsNullOrEmpty(data.widthtop) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = String.Format("{0} is required !", LblWidth), .field = "widthtop"}}
            End If
            If Not Integer.TryParse(data.widthtop, widthtop) OrElse widthtop <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = String.Format("{0} be a positive integer !", LblWidth), .field = "widthtop"}}
            End If

            If InStr(BlindName, "Door") > 0 Then
                If String.IsNullOrEmpty(data.widthmiddle) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "width middle  is required !", .field = "widthmiddle"}}
                End If
                If Not Integer.TryParse(data.widthmiddle, widthmiddle) OrElse widthmiddle <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width middle must be a positive integer !", .field = "widthmiddle"}}
                End If

                If String.IsNullOrEmpty(data.widthbottom) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "width bottom is required !", .field = "widthbottom"}}
                End If
                If Not Integer.TryParse(data.widthbottom, widthbottom) OrElse widthbottom <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width bottom must be a positive integer !", .field = "widthbottom"}}
                End If 

                If String.IsNullOrEmpty(data.widthmin) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "width minimum is required !", .field = "widthmin"}}
                End If
                If Not Integer.TryParse(data.widthmin, widthmin) OrElse widthmin <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width minimum must be a positive integer !", .field = "widthmin"}}
                End If  
            End If

            Dim drop As Integer
            If String.IsNullOrEmpty(data.drop) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "drop is required !", .field = "drop"}}
            End If
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be a positive integer !", .field = "drop"}}
            End If

            Dim LblFrameType As String = "frame type"
            If InStr(BlindName, "Steel") > 0 Then LblFrameType = "type"
            If InStr(BlindName, "Grile") > 0 Or InStr(BlindName, "Steel") > 0 Then
                If String.IsNullOrEmpty(data.frametype) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = String.Format("{0} is required !", LblFrameType), .field = "frametype"}}
                End If
            End If

            If String.IsNullOrEmpty(data.framecolour) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "frame colour is required !", .field = "framecolour"}}
            End If  
            
            If InStr(BlindName, "Steel") > 0 Then
                If String.IsNullOrEmpty(data.fitted) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fitted is required !", .field = "fitted"}}
                End If  
            End If

            If String.IsNullOrEmpty(data.meshtype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "mesh type is required !", .field = "meshtype"}}
            End If

            If InStr(BlindName, "Steel") > 0 Then
                If String.IsNullOrEmpty(data.fixing) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fixing is required !", .field = "fixing"}}
                End If

                If String.IsNullOrEmpty(data.top) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "top is required !", .field = "top"}}
                End If

                If String.IsNullOrEmpty(data.hingetype) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "hinge type is required !", .field = "hingetype"}}
                End If

                If String.IsNullOrEmpty(data.locktype) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "lock type is required !", .field = "locktype"}}
                End If

                If String.IsNullOrEmpty(data.lockhandling) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "lock handling is required !", .field = "lockhandling"}}
                End If

                If String.IsNullOrEmpty(data.sideframe) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "side frame is required !", .field = "sideframe"}}
                End If

                If String.IsNullOrEmpty(data.headframe) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "head frame is required !", .field = "headframe"}}
                End If

                If String.IsNullOrEmpty(data.extframe) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "extended frame is required !", .field = "extframe"}}
                End If

                If String.IsNullOrEmpty(data.extwidth) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "extended width is required !", .field = "extwidth"}}
                End If

                If String.IsNullOrEmpty(data.extdrop) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "extended drop is required !", .field = "extdrop"}}
                End If

                If String.IsNullOrEmpty(data.slambar) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "slam bar is required !", .field = "slambar"}}
                End If

                If String.IsNullOrEmpty(data.levelhandler) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "level handler is required !", .field = "levelhandler"}}
                End If

                If String.IsNullOrEmpty(data.lock) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "lock is required !", .field = "lock"}}
                End If
            End If

            If InStr(BlindName, "Door") > 0 AND InStr(BlindName, "Steel") = 0 Then
                If String.IsNullOrEmpty(data.layoutcode) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "layout code is required !", .field = "layoutcode"}}
                End If  

                If String.IsNullOrEmpty(data.handleposition) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "handle position is required !", .field = "handleposition"}}
                End If 

                If String.IsNullOrEmpty(data.handlemeasure) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "handle measure is required !", .field = "handlemeasure"}}
                End If 

                If String.IsNullOrEmpty(data.handleheight) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "handle height is required !", .field = "handleheight"}}
                End If
            End If

            If (InStr(BlindName, "Door") > 0 Or InStr(BlindName, "Grile") > 0) AND InStr(BlindName, "Steel") = 0 Then
                If String.IsNullOrEmpty(data.midrailposition) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "midrail position is required !", .field = "midrailposition"}}
                End If  

                If String.IsNullOrEmpty(data.midrailrequest) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "midrail request is required !", .field = "midrailrequest"}}
                End If 
            End If

            If InStr(BlindName, "Door") > 0 AND InStr(BlindName, "Steel") = 0 Then
                ' If String.IsNullOrEmpty(data.petdoortype) Then
                '     Return New ErrorResponse With { .error = New ErrorDetail With { .message = "pet door type is required !", .field = "petdoortype"}}
                ' End If

                If Not String.IsNullOrEmpty(data.petdoortype) And String.IsNullOrEmpty(data.petdoorposition) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "pet door position is required !", .field = "petdoorposition"}}
                End If

                If String.IsNullOrEmpty(data.triplelock) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "triple lock is required !", .field = "triplelock"}}
                End If

                If String.IsNullOrEmpty(data.latchbass) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "latch bass is required !", .field = "latchbass"}}
                End If

                If String.IsNullOrEmpty(data.bugseal) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "bugseal is required !", .field = "bugseal"}}
                End If

                If String.IsNullOrEmpty(data.doorcloser) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "door closer is required !", .field = "doorcloser"}}
                End If

                If String.IsNullOrEmpty(data.boldpatio) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "bold patio is required !", .field = "boldpatio"}}
                End If
            End If

            If BlindName = "Flyscreen" Then
                If String.IsNullOrEmpty(data.crossbrace) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "cross brace is required !", .field = "crossbrace"}}
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

            Dim SoeId As String = publicCfg.GetSoeKitId(data.tubetype)
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            Dim PriceGroupName As String = "Door"
            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            If PriceGroupId = "" Then
                Throw New Exception("Something went wrong !")
            End If

            If BlindName = "Flyscreen" Then
                data.widthmiddle = ""
                widthmiddle = 0
                data.widthbottom = ""
                widthbottom = 0
                data.widthmin = ""
                widthmin = 0

                data.frametype = ""
                data.fitted = ""
                data.fixing = ""
                data.top = ""
                data.hingetype = ""
                data.locktype = ""
                data.lockhandling = ""
                data.sideframe = ""
                data.headframe = ""
                data.extframe = ""
                data.extwidth = ""
                data.extdrop = ""
                data.slambar = ""
                data.levelhandler = ""
                data.lock = ""
                data.layoutcode = ""
                data.handleposition = ""
                data.handlemeasure = ""
                data.handleheight = ""
                data.midrailposition = ""
                data.midrailrequest = ""
                data.petdoortype = ""
                data.petdoorposition = ""
                data.triplelock = ""
                data.latchbass = ""
                data.bugseal = ""
                data.doorcloser = ""
                data.boldpatio = ""
            End If

            If InStr(BlindName, "Door") > 0 AND InStr(BlindName, "Steel") = 0 Then
                data.frametype = ""
                data.fitted = ""
                data.fixing = ""
                data.top = ""
                data.hingetype = ""
                data.locktype = ""
                data.lockhandling = ""
                data.sideframe = ""
                data.headframe = ""
                data.extframe = ""
                data.extwidth = ""
                data.extdrop = ""
                data.slambar = ""
                data.levelhandler = ""
                data.lock = ""
                data.crossbrace = ""
            End If

            If InStr(BlindName, "Grile") > 0 AND InStr(BlindName, "Steel") = 0 Then
                data.widthmiddle = ""
                widthmiddle = 0
                data.widthbottom = ""
                widthbottom = 0
                data.widthmin = ""
                widthmin = 0

                data.fitted = ""
                data.fixing = ""
                data.top = ""
                data.hingetype = ""
                data.locktype = ""
                data.lockhandling = ""
                data.sideframe = ""
                data.headframe = ""
                data.extframe = ""
                data.extwidth = ""
                data.extdrop = ""
                data.slambar = ""
                data.levelhandler = ""
                data.lock = ""
                data.layoutcode = ""
                data.handleposition = ""
                data.handlemeasure = ""
                data.handleheight = ""
                data.petdoortype = ""
                data.petdoorposition = ""
                data.triplelock = ""
                data.latchbass = ""
                data.bugseal = ""
                data.doorcloser = ""
                data.boldpatio = ""
                data.crossbrace = ""
            End If

            If InStr(BlindName, "Steel") > 0 Then
                data.layoutcode = ""
                data.handleposition = ""
                data.handlemeasure = ""
                data.handleheight = ""
                data.midrailposition = ""
                data.midrailrequest = ""
                data.petdoortype = ""
                data.petdoorposition = ""
                data.triplelock = ""
                data.latchbass = ""
                data.bugseal = ""
                data.doorcloser = ""
                data.boldpatio = ""
                data.crossbrace = ""
            End If


            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, PriceGroupId, Qty, Location, Mounting, Width, WidthMiddle, WidthBottom, WidthB, [Drop], FrameType, Fitting, FrameColour, MeshType, SemiInsideMount, LayoutSpecial, HangerType, TiltrodType, TiltrodSplit, FrameLeft, FrameRight, FrameTop, PanelQty, TrackQty, DoorCutOut, SpecialShape, TemplateProvided, Layout, TrackType, TrackColour, TrackLength, MidrailCritical, MidrailHeight1, Buildout, BuildoutPosition, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, ChildSafe, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @PriceGroupId, @Qty, @Location, @Mounting, @Width, @WidthMiddle, @WidthBottom, @WidthB, @Drop, @FrameType, @Fitting, @FrameColour, @MeshType, @SemiInsideMount, @LayoutSpecial, @HangerType, @TiltrodType, @TiltrodSplit, @FrameLeft, @FrameRight, @FrameTop, @PanelQty, @TrackQty, @DoorCutOut, @SpecialShape, @TemplateProvided, @Layout, @TrackType, @TrackColour, @TrackLength, @MidrailCritical, @MidrailHeight1, @Buildout, @BuildoutPosition, @JoinedPanels, @ReverseHinged, @PelmetFlat, @ExtraFascia, @HingesLoose, @ChildSafe, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.tubetype), DBNull.Value, UCase(data.tubetype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", widthtop)
                        myCmd.Parameters.AddWithValue("@WidthMiddle", widthmiddle)
                        myCmd.Parameters.AddWithValue("@WidthBottom", widthbottom)
                        myCmd.Parameters.AddWithValue("@WidthB", widthmin)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@Fitting", data.fitted)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@SemiInsideMount", data.fixing)
                        myCmd.Parameters.AddWithValue("@LayoutSpecial", data.top)
                        myCmd.Parameters.AddWithValue("@HangerType", data.hingetype)
                        myCmd.Parameters.AddWithValue("@TiltrodType", data.locktype)
                        myCmd.Parameters.AddWithValue("@TiltrodSplit", data.lockhandling)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.sideframe)
                        myCmd.Parameters.AddWithValue("@FrameRight", data.headframe)
                        myCmd.Parameters.AddWithValue("@FrameTop", data.extframe)
                        myCmd.Parameters.AddWithValue("@PanelQty", data.extwidth)
                        myCmd.Parameters.AddWithValue("@TrackQty", data.extdrop)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.slambar)
                        myCmd.Parameters.AddWithValue("@SpecialShape", data.levelhandler)
                        myCmd.Parameters.AddWithValue("@TemplateProvided", data.lock)
                        myCmd.Parameters.AddWithValue("@Layout", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@TrackType", data.handleposition)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.handlemeasure)
                        myCmd.Parameters.AddWithValue("@TrackLength", data.handleheight)
                        myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailposition)
                        myCmd.Parameters.AddWithValue("@MidrailHeight1", data.midrailrequest)
                        myCmd.Parameters.AddWithValue("@Buildout", data.petdoortype)
                        myCmd.Parameters.AddWithValue("@BuildoutPosition", data.petdoorposition)
                        myCmd.Parameters.AddWithValue("@JoinedPanels", data.triplelock)
                        myCmd.Parameters.AddWithValue("@ReverseHinged", data.latchbass)
                        myCmd.Parameters.AddWithValue("@PelmetFlat", data.bugseal)
                        myCmd.Parameters.AddWithValue("@ExtraFascia", data.doorcloser)
                        myCmd.Parameters.AddWithValue("@HingesLoose", data.boldpatio)
                        myCmd.Parameters.AddWithValue("@ChildSafe", data.crossbrace)
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
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@Width, WidthMiddle=@WidthMiddle, WidthBottom=@WidthBottom, WidthB=@WidthB, [Drop]=@Drop, FrameType=@FrameType, FrameColour=@FrameColour, MeshType=@MeshType, SemiInsideMount=@SemiInsideMount, LayoutSpecial=@LayoutSpecial, HangerType=@HangerType, TiltrodType=@TiltrodType, TiltrodSplit=@TiltrodSplit, FrameLeft=@FrameLeft, FrameRight=@FrameRight, FrameTop=@FrameTop, PanelQty=@PanelQty, TrackQty=@TrackQty, DoorCutOut=@DoorCutOut, SpecialShape=@SpecialShape, TemplateProvided=@TemplateProvided, Layout=@Layout, TrackType=@TrackType, TrackColour=@TrackColour, TrackLength=@TrackLength, MidrailCritical=@MidrailCritical, MidrailHeight1=@MidrailHeight1, Buildout=@Buildout, BuildoutPosition=@BuildoutPosition, JoinedPanels=@JoinedPanels, ReverseHinged=@ReverseHinged, PelmetFlat=@PelmetFlat, ExtraFascia=@ExtraFascia, HingesLoose=@HingesLoose, ChildSafe=@ChildSafe, Notes=@Notes, Matrix=0.00, Charge=0.00, TotalMatrix=0.00, TotalCharge=0.00, MarkUp=@MarkUp WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.tubetype), DBNull.Value, UCase(data.tubetype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", widthtop)
                        myCmd.Parameters.AddWithValue("@WidthMiddle", widthmiddle)
                        myCmd.Parameters.AddWithValue("@WidthBottom", widthbottom)
                        myCmd.Parameters.AddWithValue("@WidthB", widthmin)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@Fitting", data.fitted)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@SemiInsideMount", data.fixing)
                        myCmd.Parameters.AddWithValue("@LayoutSpecial", data.top)
                        myCmd.Parameters.AddWithValue("@HangerType", data.hingetype)
                        myCmd.Parameters.AddWithValue("@TiltrodType", data.locktype)
                        myCmd.Parameters.AddWithValue("@TiltrodSplit", data.lockhandling)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.sideframe)
                        myCmd.Parameters.AddWithValue("@FrameRight", data.headframe)
                        myCmd.Parameters.AddWithValue("@FrameTop", data.extframe)
                        myCmd.Parameters.AddWithValue("@PanelQty", data.extwidth)
                        myCmd.Parameters.AddWithValue("@TrackQty", data.extdrop)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.slambar)
                        myCmd.Parameters.AddWithValue("@SpecialShape", data.levelhandler)
                        myCmd.Parameters.AddWithValue("@TemplateProvided", data.lock)
                        myCmd.Parameters.AddWithValue("@Layout", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@TrackType", data.handleposition)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.handlemeasure)
                        myCmd.Parameters.AddWithValue("@TrackLength", data.handleheight)
                        myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailposition)
                        myCmd.Parameters.AddWithValue("@MidrailHeight1", data.midrailrequest)
                        myCmd.Parameters.AddWithValue("@Buildout", data.petdoortype)
                        myCmd.Parameters.AddWithValue("@BuildoutPosition", data.petdoorposition)
                        myCmd.Parameters.AddWithValue("@JoinedPanels", data.triplelock)
                        myCmd.Parameters.AddWithValue("@ReverseHinged", data.latchbass)
                        myCmd.Parameters.AddWithValue("@PelmetFlat", data.bugseal)
                        myCmd.Parameters.AddWithValue("@ExtraFascia", data.doorcloser)
                        myCmd.Parameters.AddWithValue("@HingesLoose", data.boldpatio)
                        myCmd.Parameters.AddWithValue("@ChildSafe", data.crossbrace)
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
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message, .field = ""}}
        End Try
    End Function 


End Class
