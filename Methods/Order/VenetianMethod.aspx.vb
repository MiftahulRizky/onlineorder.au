Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Partial Class Methods_Order_VenetianMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        '#Submit OrderHeader
        Public Property blindtype As String
        Public Property controltype As String
        Public Property colourtype As String
        Public Property qty As String
        Public Property room As String
        Public Property sizetype As String
        Public Property dropfloor As String
        Public Property mounting As String
        Public Property width As String
        Public Property drop As String
        Public Property controlposition As String
        Public Property controllength As String
        Public Property controllift As String
        Public Property controltilt As String
        Public Property bracket As String
        Public Property bottom As String
        Public Property holdbracket As String
        Public Property twoheadrail As String
        Public Property pelmettype As String
        Public Property pelmetsize As String
        Public Property pelmetwidth As String
        Public Property returnleft As String
        Public Property returnright As String
        Public Property toplhswidth As String
        Public Property toplhsheight As String
        Public Property toprhswidth As String
        Public Property toprhsheight As String
        Public Property botlhswidth As String
        Public Property botlhsheight As String
        Public Property botrhswidth As String
        Public Property botrhsheight As String
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
        Public Property controltype As String
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
                    query = String.Format("SELECT ControlType FROM HardwareKits WHERE DesignId='{0}' AND BlindId='{1}' AND Active=1 GROUP BY ControlType ORDER BY ControlType ASC", data.designid, UCase(data.blindtype).ToString())
                    Return GetFormattedData(query, "ControlType", "ControlType")

                Case "colourtype"
                    query = String.Format("SELECT Id, ColourType FROM HardwareKits WHERE DesignId='{0}' AND BlindId='{1}' AND ControlType = '{2}' AND Active=1 ORDER BY ColourType ASC", data.designid, UCase(data.blindtype).ToString(), data.controltype)
                    Return GetFormattedData(query, "Id", "ColourType")

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
            Dim ColourType As String = publicCfg.GetItemData(String.Format("SELECT ColourType FROM HardwareKits WHERE Id = '{0}'", data.colourtype))

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

            If InArray(BlindName, "50mm Aluminium", "50mm Timberstyle", "63mm Timberstyle") Then
                If String.IsNullOrEmpty(data.sizetype) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "size type is required !", .field = "sizetype"}}
                End If

                If data.sizetype = "Opening Size" AND String.IsNullOrEmpty(data.dropfloor) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "drop to the floor is required !", .field = "dropfloor"}}
                End If
            End IF

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
            ' If width < 150 Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 150 !",.field = "width"}}
            ' End If
            ' If width > 6000 Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 6000 !",.field = "width"}}
            ' End If

            Dim drop As Integer
            If String.IsNullOrEmpty(data.drop) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop is required !",.field = "drop"}}
            End If
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be a positive integer !",.field = "drop"}}
            End If
            ' If drop < 150 Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be greater than or equal to 150 !",.field = "drop"}}
            ' End If
            ' If drop > 3200 Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be less than or equal to 3200 !",.field = "drop"}}
            ' End If

            Dim controllength As Integer
            If InArray(BlindName, "25mm Aluminium", "50mm Aluminium", "50mm Timberstyle", "63mm Timberstyle") Then
                If String.IsNullOrEmpty(data.controlposition) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
                End If

                If Not String.IsNullOrEmpty(data.controllength) Then
                    If Not Integer.TryParse(data.controllength, controllength) OrElse controllength <= 0 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control length must be a positive integer !",.field = "controllength"}}
                    End If
                End If
            End If

            If InArray(BlindName, "50mm Mockwood", "63mm Mockwood", "50mm Wooden", "63mm Wooden") Then
                If String.IsNullOrEmpty(data.controllift) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control lift is required !",.field = "controllift"}}
                End If

                If String.IsNullOrEmpty(data.controltilt) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control tilt is required !",.field = "controltilt"}}
                End If
            End If

            If InArray(BlindName, "25mm Aluminium", "50mm Aluminium", "50mm Timberstyle", "63mm Timberstyle") Then
                If String.IsNullOrEmpty(data.bracket) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bracket is required !",.field = "bracket"}}
                End If
            End If

            If InArray(BlindName, "25mm Aluminium", "50mm Aluminium", "50mm Timberstyle", "63mm Timberstyle") Then
                If String.IsNullOrEmpty(data.bottom) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom is required !",.field = "bottom"}}
                End If
            End If

            If InArray(BlindName, "25mm Aluminium", "50mm Aluminium", "50mm Mockwood", "63mm Mockwood", "50mm Wooden", "63mm Wooden") Then
                ' If String.IsNullOrEmpty(data.holdbracket) Then
                '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "hold down bracket is required !",.field = "holdbracket"}}
                ' End If
            End If

            Dim pelmetwidth As Integer
            Dim returnleft As Integer
            Dim returnright As Integer
            If InArray(BlindName, "50mm Mockwood", "63mm Mockwood", "50mm Timberstyle", "63mm Timberstyle", "50mm Wooden", "63mm Wooden") Then
                If String.IsNullOrEmpty(data.pelmettype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "pelmet type is required !",.field = "pelmettype"}}
                End If

                If InArray(BlindName, "50mm Timberstyle", "63mm Timberstyle") Then
                    If String.IsNullOrEmpty(data.pelmetsize) Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "pelmet size is required !",.field = "pelmetsize"}}
                    End If
                End If

                If Not String.IsNullOrEmpty(data.pelmetwidth) Then
                    If Not Integer.TryParse(data.pelmetwidth, pelmetwidth) OrElse pelmetwidth <= 0 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "pelmet width must be a positive integer !",.field = "pelmetwidth"}}
                    End If
                End If

                If Not String.IsNullOrEmpty(data.returnleft) Then
                    If Not Integer.TryParse(data.returnleft, returnleft) OrElse returnleft <= 0 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "return left must be a positive integer !",.field = "returnleft"}}
                    End If
                End If

                If Not String.IsNullOrEmpty(data.returnright) Then
                    If Not Integer.TryParse(data.returnright, returnright) OrElse returnright <= 0 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "return right must be a positive integer !",.field = "returnright"}}
                    End If
                End If
            End If

            Dim toplhswidth As Integer
            If Not String.IsNullOrEmpty(data.toplhswidth) Then
                If Not Integer.TryParse(data.toplhswidth, toplhswidth) OrElse toplhswidth <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "top lhs width must be a positive integer !",.field = "toplhswidth"}}
                End If
            End If

            Dim toplhsheight As Integer
            If Not String.IsNullOrEmpty(data.toplhsheight) Then
                If Not Integer.TryParse(data.toplhsheight, toplhsheight) OrElse toplhsheight <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "top lhs height must be a positive integer !",.field = "toplhsheight"}}
                End If
            End If

            Dim toprhswidth As Integer
            If Not String.IsNullOrEmpty(data.toprhswidth) Then
                If Not Integer.TryParse(data.toprhswidth, toprhswidth) OrElse toprhswidth <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "top rhs width must be a positive integer !",.field = "toprhswidth"}}
                End If
            End If

            Dim toprhsheight As Integer
            If Not String.IsNullOrEmpty(data.toprhsheight) Then
                If Not Integer.TryParse(data.toprhsheight, toprhsheight) OrElse toprhsheight <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "top rhs height must be a positive integer !",.field = "toprhsheight"}}
                End If
            End If

            Dim botlhswidth As Integer
            If Not String.IsNullOrEmpty(data.botlhswidth) Then
                If Not Integer.TryParse(data.botlhswidth, botlhswidth) OrElse botlhswidth <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom lhs width must be a positive integer !",.field = "botlhswidth"}}
                End If
            End If

            Dim botlhsheight As Integer
            If Not String.IsNullOrEmpty(data.botlhsheight) Then
                If Not Integer.TryParse(data.botlhsheight, botlhsheight) OrElse botlhsheight <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom lhs height must be a positive integer !",.field = "botlhsheight"}}
                End If
            End If

            Dim botrhswidth As Integer
            If Not String.IsNullOrEmpty(data.botrhswidth) Then
                If Not Integer.TryParse(data.botrhswidth, botrhswidth) OrElse botrhswidth <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom rhs width must be a positive integer !",.field = "botrhswidth"}}
                End If
            End If

            Dim botrhsheight As Integer
            If Not String.IsNullOrEmpty(data.botrhsheight) Then
                If Not Integer.TryParse(data.botrhsheight, botrhsheight) OrElse botrhsheight <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom rhs height must be a positive integer !",.field = "botrhsheight"}}
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

            Dim SoeId As String = publicCfg.GetSoeKitId(data.colourtype)
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            Dim PriceGroupName As String = BlindName
            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            If PriceGroupId = "" Then
                Throw New Exception("price group not found!")
            End If

            If InArray(BlindName, "25mm Aluminium", "50mm Aluminium") Then
                data.controllift = ""
                data.controltilt = ""
                ' data.twoheadrail = ""
                data.pelmettype = ""
                data.pelmetsize = ""
                data.pelmetwidth = ""
                pelmetwidth = 0
                data.pelmetsize = ""
                data.returnleft = ""
                returnleft = 0
                data.returnright = ""
                returnright = 0
            End If

            If InArray(BlindName, "50mm Mockwood", "63mm Mockwood") Then
                data.controlposition = ""
                data.controllength = ""
                controllength = 0
                data.bracket = ""
                data.bottom = ""
                data.pelmetsize = ""
            End If

            If InArray(BlindName, "50mm Timberstyle", "63mm Timberstyle") Then
                data.controllift = ""
                data.controltilt = ""
                data.holdbracket = ""
            End If

            If InArray(BlindName, "50mm Wooden", "63mm Wooden") Then
                data.controlposition = ""
                data.controllength = ""
                controllength = 0
                data.bracket = ""
                data.bottom = ""
                data.pelmetsize = ""
            End If

            If Not InArray(BlindName, "50mm Aluminum", "50mm Timberstyle", "63mm Timberstyle") Then
                data.sizetype = ""
                data.dropfloor = ""
            End If

            '# yudi's request
            ' If InStr(BlindName, "Aluminium") > 0 Or InStr(BlindName, "Timber") > 0 Then
            '     If data.controllength = "" Or data.controllength = "0" Then
            '         controllength = CInt(drop * (2 / 3))
            '     End If

            '     If  InStr(BlindName, "Aluminium") > 0 Then
            '         If BlindName = "25mm Aluminium" Then
            '             data.bottom = "Yes"
            '         End If
            '     End If
            ' End If

            ' If InStr(BlindName, "Mockwood") > 0 Or InStr(BlindName, "Wooden") > 0 Then
            '    controllength = CInt(drop * (2 / 3))
            ' End If

            Dim FindControlPosition = data.controlposition
            If InStr(BlindName, "Mockwood") > 0 Or InStr(BlindName, "Wooden") > 0 Then
                FindControlPosition = String.Format("{0}|{1}", data.controllift, data.controltilt)
            End If

            Dim CutOut_LeftTop As Integer = 0
            Dim CutOut_RightTop As Integer = 0
            Dim CutOut_LeftBottom As Integer = 0
            Dim CutOut_RightBottom As Integer = 0

            If toplhswidth > 0 Or toplhsheight > 0 Then
                CutOut_LeftTop = 1
            End If
            If toprhswidth > 0 Or toprhsheight > 0 Then
                CutOut_RightTop = 1
            End If

            If botlhswidth > 0 Or botlhsheight > 0 Then
                CutOut_LeftBottom = 1
            End If
            If botrhswidth > 0 Or botrhsheight > 0 Then
                CutOut_RightBottom = 1
            End If

            If data.pelmetwidth = "" Or data.pelmetwidth = "0" Then
                If data.mounting = "Face Fit" Then
                    pelmetwidth = width + 20
                    If InStr(BlindName, "Timber") > 0 Then
                        pelmetwidth = width + 20
                    End If
                End If
                If data.mounting = "Reveal Fit" Then
                    pelmetwidth = width + 10
                    If InStr(BlindName, "Timber") > 0 Then
                        pelmetwidth = width + 8
                    End If
                End If
            End If

            If data.pelmettype = "With Return" Then
                ' returnleft = "67" : returnright = "67"
                If data.mounting = "Face Fit" Then
                    returnleft = 100 : returnright = 100
                    If InStr(BlindName, "Wooden") > 0 Then
                        returnleft = 70 : returnright = 70
                    End If
                    If InStr(BlindName, "Mockwood") > 0 Then
                        returnleft = 77 : returnright = 77
                    End If
                    If InStr(BlindName, "Timber") > 0 Then
                        returnleft = 67 : returnright = 67
                    End If
                End If
                If data.mounting = "Reveal Fit" Then
                    returnleft = 0 : returnright = 0
                End If
            End If

            If data.pelmettype = "Single Left Return" Then
                If data.mounting = "Face Fit" Then
                    returnleft = 77 : returnright = 0
                End If
                If data.mounting = "Reveal Fit" Then
                    returnleft = 0 : returnright = 0
                End If
            End If

            If data.pelmettype = "Single Right Return" Then
                If data.mounting = "Face Fit" Then
                    returnleft = 0 : returnright = 77
                End If    
                If data.mounting = "Reveal Fit" Then
                    returnleft = 0 : returnright = 0
                End If
            End If

            If InArray(data.pelmettype, "Bay Left", "Bay Right", "Main Bay", "Common (1 Pelmet Cover 2 or 3 blinds)") Then
                data.returnleft = ""
                returnleft = 0
                data.returnright = ""
                returnright = 0
            End If

            If data.pelmettype = "Single Left Return" Then
                data.returnright = ""
                returnright = 0
            End If

            If data.pelmettype = "Single Right Return" Then
                data.returnleft = ""
                returnleft = 0
            End If
            
            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()
            
                Dim Field As String = "Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, PriceGroupId, Qty, Location, LouvreSize, LouvrePosition, Mounting, Width, [Drop], ControlPosition, ControlLength, BracketOption, BottomHoldDown, BracketColour, DoorCutOut, PelmetType, PelmetWidth, PelmetSize, PelmetReturnSize, PelmetReturnSize2, CutOut_LeftTop, CutOut_RightTop, CutOut_LeftBottom, CutOut_RightBottom, LHSWidth_Top, LHSHeight_Top, RHSWidth_Top, RHSHeight_Top, LHSWidth_Bottom, LHSHeight_Bottom, RHSWidth_Bottom, RHSHeight_Bottom,  Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active"

                Dim Values As String = "@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @PriceGroupId, @Qty, @Location, @LouvreSize, @LouvrePosition, @Mounting, @Width, @Drop, @ControlPosition, @ControlLength, @BracketOption, @BottomHoldDown, @BracketColour, @DoorCutOut, @PelmetType, @PelmetWidth, @PelmetSize, @PelmetReturnSize, @PelmetReturnSize2, @CutOut_LeftTop, @CutOut_RightTop, @CutOut_LeftBottom, @CutOut_RightBottom, @LHSWidth_Top, @LHSHeight_Top, @RHSWidth_Top, @RHSHeight_Top, @LHSWidth_Bottom, @LHSHeight_Bottom, @RHSWidth_Bottom, @RHSHeight_Bottom, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1"

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(String.Format("INSERT INTO OrderDetails({0}) VALUES ({1})", Field, Values), thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.sizetype)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.dropfloor)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", FindControlPosition)
                        myCmd.Parameters.AddWithValue("@ControlLength", controllength)
                        myCmd.Parameters.AddWithValue("@BracketOption", data.bracket)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.bottom)
                        myCmd.Parameters.AddWithValue("@BracketColour", data.holdbracket)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.twoheadrail)
                        myCmd.Parameters.AddWithValue("@PelmetType", data.pelmettype)
                        myCmd.Parameters.AddWithValue("@PelmetWidth", pelmetwidth)
                        myCmd.Parameters.AddWithValue("@PelmetSize", data.pelmetsize)
                        myCmd.Parameters.AddWithValue("@PelmetReturnSize", returnleft)
                        myCmd.Parameters.AddWithValue("@PelmetReturnSize2", returnright)
                        myCmd.Parameters.AddWithValue("@CutOut_LeftTop", CutOut_LeftTop)
                        myCmd.Parameters.AddWithValue("@CutOut_RightTop", CutOut_RightTop)
                        myCmd.Parameters.AddWithValue("@CutOut_LeftBottom", CutOut_LeftBottom)
                        myCmd.Parameters.AddWithValue("@CutOut_RightBottom", CutOut_RightBottom)
                        myCmd.Parameters.AddWithValue("@LHSWidth_Top", toplhswidth)
                        myCmd.Parameters.AddWithValue("@LHSHeight_Top", toplhsheight)
                        myCmd.Parameters.AddWithValue("@RHSWidth_Top", toprhswidth)
                        myCmd.Parameters.AddWithValue("@RHSHeight_Top", toprhsheight)
                        myCmd.Parameters.AddWithValue("@LHSWidth_Bottom", botlhswidth)
                        myCmd.Parameters.AddWithValue("@LHSHeight_Bottom", botlhsheight)
                        myCmd.Parameters.AddWithValue("@RHSWidth_Bottom", botrhswidth)
                        myCmd.Parameters.AddWithValue("@RHSHeight_Bottom", botrhsheight)
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

                msg = "Item added successfully !"
            End If

            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then
                Dim ItemId As String = data.itemid
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, LouvreSize=@LouvreSize, LouvrePosition=@LouvrePosition, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, ControlPosition=@ControlPosition, ControlLength=@ControlLength, BracketOption=@BracketOption, BottomHoldDown=@BottomHoldDown, BracketColour=@BracketColour, DoorCutOut=@DoorCutOut, PelmetType=@PelmetType, PelmetWidth=@PelmetWidth, PelmetSize=@PelmetSize, PelmetReturnSize=@PelmetReturnSize, PelmetReturnSize2=@PelmetReturnSize2, CutOut_LeftTop=@CutOut_LeftTop, CutOut_RightTop=@CutOut_RightTop, CutOut_LeftBottom=@CutOut_LeftBottom, CutOut_RightBottom=@CutOut_RightBottom, LHSWidth_Top=@LHSWidth_Top, LHSHeight_Top=@LHSHeight_Top, RHSWidth_Top=@RHSWidth_Top, RHSHeight_Top=@RHSHeight_Top, LHSWidth_Bottom=@LHSWidth_Bottom, LHSHeight_Bottom=@LHSHeight_Bottom, RHSWidth_Bottom=@RHSWidth_Bottom, RHSHeight_Bottom=@RHSHeight_Bottom, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        ' myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.sizetype)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.dropfloor)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", FindControlPosition)
                        myCmd.Parameters.AddWithValue("@ControlLength", controllength)
                        myCmd.Parameters.AddWithValue("@BracketOption", data.bracket)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.bottom)
                        myCmd.Parameters.AddWithValue("@BracketColour", data.holdbracket)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.twoheadrail)
                        myCmd.Parameters.AddWithValue("@PelmetType", data.pelmettype)
                        myCmd.Parameters.AddWithValue("@PelmetWidth", pelmetwidth)
                        myCmd.Parameters.AddWithValue("@PelmetSize", data.pelmetsize)
                        myCmd.Parameters.AddWithValue("@PelmetReturnSize", returnleft)
                        myCmd.Parameters.AddWithValue("@PelmetReturnSize2", returnright)
                        myCmd.Parameters.AddWithValue("@CutOut_LeftTop", CutOut_LeftTop)
                        myCmd.Parameters.AddWithValue("@CutOut_RightTop", CutOut_RightTop)
                        myCmd.Parameters.AddWithValue("@CutOut_LeftBottom", CutOut_LeftBottom)
                        myCmd.Parameters.AddWithValue("@CutOut_RightBottom", CutOut_RightBottom)
                        myCmd.Parameters.AddWithValue("@LHSWidth_Top", toplhswidth)
                        myCmd.Parameters.AddWithValue("@LHSHeight_Top", toplhsheight)
                        myCmd.Parameters.AddWithValue("@RHSWidth_Top", toprhswidth)
                        myCmd.Parameters.AddWithValue("@RHSHeight_Top", toprhsheight)
                        myCmd.Parameters.AddWithValue("@LHSWidth_Bottom", botlhswidth)
                        myCmd.Parameters.AddWithValue("@LHSHeight_Bottom", botlhsheight)
                        myCmd.Parameters.AddWithValue("@RHSWidth_Bottom", botrhswidth)
                        myCmd.Parameters.AddWithValue("@RHSHeight_Bottom", botrhsheight)
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

                msg = "Item updated successfully !"
            End If

            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Dim msg As String = "Please contact our IT team at support@onlineorder.au"
            If data.rolename = "Administrator" Then msg = ex.Message
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = msg, .field = ""}}
        End Try
    End Function

End Class
