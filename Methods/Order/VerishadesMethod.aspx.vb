
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
        Public Property notes As String
        Public Property markup As String

        

        '#aditional param
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
    End Class

    Public Class ParamListData
        Public Property field As String
        Public Property designid As String
        Public Property blindtype As String
        Public Property tubetype As String
        Public Property fabrictype As String
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

            Dim customsize As Integer
            If InArray(BlindName, "Single", "Track Only") Then
                If String.IsNullOrEmpty(data.stack) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "stack configuration is required !",.field = "stack"}}
                End If
                
                If String.IsNullOrEmpty(data.tracktype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "track type is required !",.field = "tracktype"}}
                End If
                If Not String.IsNullOrEmpty(data.tracktype) AND String.IsNullOrEmpty(data.trackcolour)Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "track colour is required !",.field = "trackcolour"}}
                End If

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
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
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

            If BlindName = "Single" Then
                data.blindsize = ""
            End If

            If BlindName = "Slat Only" Then
                data.width = ""
                width = 0
                data.stack = ""
                data.tracktype = ""
                data.trackcolour = ""
                data.wandsize = ""
                data.wandcolour = ""
                data.customsize = ""
            End If

            If BlindName = "Track Only" Then
                data.drop = ""
                drop = 0
                data.fabrictype = ""
                data.fabriccolour = ""
                data.blindsize = ""
            End If

            If data.wandsize = "Custom" Then
                data.wandsize = data.customsize
            End IF

            ' throw New Exception( data.wandsize)

            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()
            
                Dim Field As String = "Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, FabricId, PriceGroupId, Qty, Location, Mounting, Width, [Drop], BlindSize,  StackPosition, TrackType, TrackColour, WandLength, WandColour, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active"

                Dim Values As String = "@Id, @HeaderId, 'Blind 1', @KitId, @SoeKitId, @ExactId, @FabricId, @PriceGroupId,  @Qty, @Location, @Mounting, @Width, @Drop, @BlindSize, @StackPosition, @TrackType, @TrackColour, @WandLength, @WandColour, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1"

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
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@BlindSize", data.blindsize)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stack)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                        myCmd.Parameters.AddWithValue("@WandLength", data.wandsize)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
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
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@width, [Drop]=@Drop, BlindSize=@BlindSize, StackPosition=@StackPosition, TrackType=@TrackType, TrackColour=@TrackColour, WandLength=@WandLength, WandColour=@WandColour, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
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
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@BlindSize", data.blindsize)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stack)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                        myCmd.Parameters.AddWithValue("@WandLength", data.wandsize)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
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
