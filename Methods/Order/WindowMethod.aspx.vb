Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Partial Class Methods_Order_WindowMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        Public Property blindtype As String
        Public Property tubetype As String
        Public Property qty As String
        Public Property room As String
        Public Property mounting As String
        Public Property width As String
        Public Property drop As String
        Public Property meshtype As String
        Public Property slidingtype As String
        Public Property stacking As String
        Public Property trackless As String
        Public Property frametype As String
        Public Property framecolour As String
        Public Property customframecolour As String
        Public Property brace As String
        Public Property bracelength As String
        Public Property dualhinges As String
        Public Property install As String
        Public Property fitting As String
        Public Property remove As String
        Public Property handle As String
        Public Property pullcord As String
        Public Property cutout As String
        Public Property extras As String
        Public Property notes As String
        Public Property markup As String
        

        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
        Public Property blindno As String
        Public Property uniqueid As String
    End Class

    Public Class ExtraItem
        Public Property name As String
        Public Property unit As String
        Public Property value As String
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
    Public Shared Function BindListData(ByVal field As String, ByVal designid As String, ByVal blindtype As String) As Object
        Try
            Dim query As String = ""
            Dim resultList As New List(Of Dictionary(Of String, String))()

            Select Case field.ToLower()
                Case "blindtype"
                    query = String.Format("SELECT Id, Name FROM Blinds WHERE DesignId='{0}' AND Active=1 ORDER BY Name ASC", designid)
                    Return GetFormattedData(query, "Id", "Name")

                Case "tubetype"
                    query = String.Format("SELECT Id, TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND Active=1 ORDER BY Name ASC", designid, UCase(blindtype).ToString())
                    Return GetFormattedData(query, "Id", "TubeType")

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
            Dim TubeName As String = publicCfg.GetItemData(String.Format("SELECT TubeType FROM HardwareKits WHERE Id = '{0}'", data.tubetype))
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

            ' If InArray(BlindName, "Basic Window") Then
            ' End If

             If InArray(TubeName, "Retractable Flyscreen Pleated") Then
                If String.IsNullOrEmpty(data.slidingtype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "sliding type is required !",.field = "slidingtype"}}
                End If
                If String.IsNullOrEmpty(data.stacking) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "stacking is required !",.field = "stacking"}}
                End If
                If String.IsNullOrEmpty(data.trackless) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "trackless is required !",.field = "trackless"}}
                End If
            End If

            If InArray(TubeName, "Flyscreens") Then
                If String.IsNullOrEmpty(data.meshtype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "mesh type is required !",.field = "meshtype"}}
                End If
            End If
            
            If InArray(TubeName, "Flyscreens", "Retractable Flyscreen Roll-Up Down", "Heavy Duty Diamond", "Ultra Barrier", "Ultra Guard") Then
                If String.IsNullOrEmpty(data.frametype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "frame type is required !",.field = "frametype"}}
                End If
            End If

            If String.IsNullOrEmpty(data.framecolour) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "frame colour is required !",.field = "framecolour"}}
            End If

            If Not String.IsNullOrEmpty(data.framecolour) Then
                If data.framecolour = "Powder Coating" And String.IsNullOrEmpty(data.customframecolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "custom frame colour is required !",.field = "customframecolour"}}
                End If
            End If
            
            Dim bracelength As Integer
            If InArray(TubeName, "Flyscreens", "Heavy Duty Diamond", "Ultra Barrier", "Ultra Guard") Then
              
                ' If String.IsNullOrEmpty(data.brace) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "brace is required !",.field = "brace"}}
                ' End If

                If Not InArray(data.brace, "Horizontal Centre Brace", "Vertical Centre Brace") AND Not String.IsNullOrEmpty(data.brace) Then
                    If String.IsNullOrEmpty(data.bracelength) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "brace length is required !",.field = "bracelength"}}
                    End If

                    If Not Integer.TryParse(data.bracelength, bracelength) OrElse bracelength <= 0 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "brace length must be a positive integer !",.field = "bracelength"}}
                    End If
                End If
            End If

            If InArray(TubeName, "Ultra Guard") Then
                ' If String.IsNullOrEmpty(data.remove) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "remove product is required !",.field = "remove"}}
                ' End If
            End If

            If InArray(TubeName, "Heavy Duty Diamond", "Ultra Barrier") Then
                ' If String.IsNullOrEmpty(data.dualhinges) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "two hinges is required !",.field = "dualhinges"}}
                ' End If
                
                ' If String.IsNullOrEmpty(data.install) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "installation is required !",.field = "install"}}
                ' End If
            End If

            Dim CutOutList As List(Of ExtraItem)
            If InArray(TubeName, "Heavy Duty Diamond", "Ultra Barrier", "Ultra Guard") Then
                ' If String.IsNullOrEmpty(data.cutout) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "cut out is required !",.field = "cutout"}}
                ' End If

                If Not String.IsNullOrWhiteSpace(data.cutout) OrElse data.cutout = "[]" Then
                    Try
                        CutOutList = JsonConvert.DeserializeObject(Of List(Of ExtraItem))(data.cutout)
                    Catch ex As Exception
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = "Invalid cutout format", .field = "cutout"}}
                    End Try

                    For Each item In CutOutList
                        If String.IsNullOrEmpty(item.value) Then
                            Return New ErrorResponse With { .error = New ErrorDetail With { .message = item.name & " (" & item.unit & ") is required", .field = "cutout"}}
                        End If

                        If item.unit = "mm" Then
                            Dim num As Decimal
                            If Not Decimal.TryParse(item.value, num) Then
                                Return New ErrorResponse With { .error = New ErrorDetail With { .message = item.name & " must be numeric", .field = "cutout"}}
                            End If
                        End If
                    Next
                End IF

            End If

            If InArray(TubeName, "Flyscreens") Then
                ' If String.IsNullOrEmpty(data.install) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "install is required !",.field = "install"}}
                ' End If

                ' If String.IsNullOrEmpty(data.fitting) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fitting is required !",.field = "fitting"}}
                ' End If
            End If

            If InArray(TubeName, "Retractable Flyscreen Roll-Up Down") Then
                If String.IsNullOrEmpty(data.handle) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "handle is required !",.field = "handle"}}
                End If

                If String.IsNullOrEmpty(data.pullcord) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "pullcord is required !",.field = "pullcord"}}
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

            ' If String.IsNullOrWhiteSpace(data.extras) OrElse data.extras = "[]" Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "extras is required !",.field = "extras"}}
            ' End If

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

                    If item.unit = "mm" Then
                        Dim num As Decimal
                        If Not Decimal.TryParse(item.value, num) Then
                            Return New ErrorResponse With { .error = New ErrorDetail With { .message = item.name & " must be numeric", .field = "extras"}}
                        End If
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
            End If

            Dim SoeId As String = publicCfg.GetSoeKitId(data.tubetype)
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            Dim PriceGroupName As String = String.Format("{0} - {1}", BlindName, TubeName)

            Dim Sliding As String = "Single Sliding"
            If data.slidingtype = "Double Sliding Pleated" Then Sliding = "Double Sliding"
            If TubeName = "Retractable Flyscreen Pleated" Then
                PriceGroupName = String.Format("{0} - {1} #{2}", BlindName, TubeName, Sliding)
            End If
            
            If TubeName = "Flyscreens" Then
                PriceGroupName = String.Format("{0} - {1} #{2}", BlindName, TubeName, data.frametype)
            End If

            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            ' Throw new Exception(PriceGroupName)
            If PriceGroupId = "" Then
                Throw New Exception("Something went wrong !")
            End If

            Dim squareMetre As Decimal = Math.Round(width * drop / 1000000, 4)
            Dim linearMetre As Decimal = Math.Round(width / 1000, 4)

            If TubeName = "Flyscreens" Then
                data.slidingtype = ""
                data.stacking = ""
                data.trackless = ""
                data.dualhinges = ""
                data.remove = ""
                data.handle = ""
                data.pullcord = ""
                data.cutout = ""
            End If

            If TubeName = "Retractable Flyscreen Pleated" Then
                data.meshtype = ""
                data.frametype = ""
                data.brace = ""
                data.bracelength = ""
                data.dualhinges = ""
                data.install = ""
                data.fitting = ""
                data.remove = ""
                data.handle = ""
                data.pullcord = ""
                data.cutout = ""
            End If

            If TubeName = "Retractable Flyscreen Roll-Up Down" Then
                data.meshtype = ""
                data.slidingtype = ""
                data.stacking = ""
                data.trackless = ""
                data.brace = ""
                data.bracelength = ""
                data.dualhinges = ""
                data.install = ""
                data.fitting = ""
                data.remove = ""
                data.cutout = ""
            End if

            If InArray(TubeName, "Heavy Duty Diamond", "Ultra Barrier") Then
                data.slidingtype = ""
                data.stacking = ""
                data.trackless = ""
                data.fitting = ""
                data.remove = ""
                data.handle = ""
                data.pullcord = ""
            End If

            If TubeName = "Ultra Barrier" Then
                data.meshtype = ""
            End if

            If TubeName = "Ultra Guard" Then
                data.meshtype = ""
                data.slidingtype = ""
                data.stacking = ""
                data.trackless = ""
                data.dualhinges = ""
                data.install = ""
                data.fitting = ""
                data.handle = ""
                data.pullcord = ""
            End if

            If InArray(data.brace, "Horizontal Centre Brace", "Vertical Centre Brace") Then
                data.bracelength = ""
            End If

            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()

                Dim Field As String = "Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, PriceGroupId, Qty, Location, Mounting, Width, [Drop], MeshType, BottomTrackType, StackPosition, TilterPosition, FrameType, FrameColour, FrameLeft, Brace, TrackLength, BracketOption, BracketCover, Fitting, BracketExtension, PortHole, PlungerPin, FlatType, AdditionalMotor, SquareMetre, LinearMetre, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active"

                Dim Values As String = "@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @PriceGroupId, @Qty, @Location, @Mounting, @Width, @Drop, @MeshType, @BottomTrackType, @StackPosition, @TilterPosition, @FrameType, @FrameColour, @FrameLeft,  @Brace, @TrackLength, @BracketOption, @BracketCover, @Fitting, @BracketExtension, @PortHole, @PlungerPin, @FlatType, @AdditionalMotor, @SquareMetre, @LinearMetre, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1"

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(String.Format("INSERT INTO OrderDetails({0}) VALUES ({1})", Field, Values), thisConn)
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
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@BottomTrackType", data.slidingtype)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stacking)
                        myCmd.Parameters.AddWithValue("@TilterPosition", data.trackless)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.customframecolour)
                        myCmd.Parameters.AddWithValue("@Brace", data.brace)
                        myCmd.Parameters.AddWithValue("@TrackLength", data.bracelength)
                        myCmd.Parameters.AddWithValue("@BracketOption", data.dualhinges)
                        myCmd.Parameters.AddWithValue("@BracketCover", data.install)
                        myCmd.Parameters.AddWithValue("@Fitting", data.fitting)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.remove)
                        myCmd.Parameters.AddWithValue("@PortHole", data.handle)
                        myCmd.Parameters.AddWithValue("@PlungerPin", data.pullcord)
                        myCmd.Parameters.AddWithValue("@FlatType", data.cutout)
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
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@width, [Drop]=@Drop, MeshType=@MeshType, BottomTrackType=@BottomTrackType, StackPosition=@StackPosition, TilterPosition=@TilterPosition, FrameType=@FrameType, FrameColour=@FrameColour, FrameLeft=@FrameLeft, Brace=@Brace, TrackLength=@TrackLength, BracketOption=@BracketOption, BracketCover=@BracketCover, Fitting=@Fitting, BracketExtension=@BracketExtension, PortHole=@PortHole, PlungerPin=@PlungerPin, FlatType=@FlatType, AdditionalMotor=@AdditionalMotor, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
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
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@BottomTrackType", data.slidingtype)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stacking)
                        myCmd.Parameters.AddWithValue("@TilterPosition", data.trackless)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.customframecolour)
                        myCmd.Parameters.AddWithValue("@Brace", data.brace)
                        myCmd.Parameters.AddWithValue("@TrackLength", data.bracelength)
                        myCmd.Parameters.AddWithValue("@BracketOption", data.dualhinges)
                        myCmd.Parameters.AddWithValue("@BracketCover", data.install)
                        myCmd.Parameters.AddWithValue("@Fitting", data.fitting)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.remove)
                        myCmd.Parameters.AddWithValue("@PortHole", data.handle)
                        myCmd.Parameters.AddWithValue("@PlungerPin", data.pullcord)
                        myCmd.Parameters.AddWithValue("@FlatType", data.cutout)
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
