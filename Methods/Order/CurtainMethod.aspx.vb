Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_Order_CurtainMethod
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
        Public Property curtainheading As String
        Public Property fabrictype As String
        Public Property fabriccolour As String
        Public Property tracktype As String
        Public Property trackcolour As String
        Public Property trackdraw As String
        Public Property stackposition As String
        Public Property returnleft As String
        Public Property returnright As String
        Public Property bottom As String
        Public Property tie As String
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

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
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
            Dim MyQuery As String = String.Format("SELECT * FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND Active=1 ORDER BY TubeType ASC", designid, UCase(blindid).ToString())
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
    Public Shared Function BindFabricType(ByVal designid As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT Type FROM Fabrics WHERE DesignId='{0}' AND Active='1' GROUP BY Type ORDER BY Type ASC", designid)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Type").ToString()},
                        {"text", row("Type").ToString()}
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
    Public Shared Function BindFabricColour(ByVal designid As String, ByVal fabrictype As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT Id, Colour FROM Fabrics WHERE DesignId='{0}' AND Type='{1}' AND Active='1'  ORDER BY Name ASC", designid, fabrictype)
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("Colour").ToString()}
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
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "fitting is required !", .field = "mounting"}}
            End If

            Dim width As Integer
            Dim drop As Integer
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
            
            If InArray(BlindName, "Complete Set", "Curtain Only") Then    
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
            End If

            If String.IsNullOrEmpty(data.curtainheading) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "curtain heading is required !",.field = "curtainheading"}}
            End If

            If Not BlindName = "Track Only" Then
                If String.IsNullOrEmpty(data.fabrictype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type is required !",.field = "fabrictype"}}
                End If
                If String.IsNullOrEmpty(data.fabriccolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour is required !",.field = "fabriccolour"}}
                End If
            End If
        
            IF Not BlindName = "Curtain Only" Then
                If String.IsNullOrEmpty(data.tracktype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "track type is required !",.field = "tracktype"}}
                End If
                If String.IsNullOrEmpty(data.trackcolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "track colour is required !",.field = "trackcolour"}}
                End If
                If String.IsNullOrEmpty(data.trackdraw) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "track draw is required !",.field = "trackdraw"}}
                End If
            End IF




            If String.IsNullOrEmpty(data.stackposition) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "stack position is required !",.field = "stackposition"}}
            End If

            Dim returnleft As Integer
            Dim returnright As Integer
            If Not String.IsNullOrEmpty(data.tracktype) Then
                If String.IsNullOrEmpty(data.returnleft) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "return length left is required !",.field = "returnleft"}}
                End If
                If Not Integer.TryParse(data.returnleft, returnleft) OrElse returnleft <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "return left must be a positive integer !",.field = "returnleft"}}
                End If

                If String.IsNullOrEmpty(data.returnright) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "return length right is required !",.field = "returnright"}}
                End If
                If Not Integer.TryParse(data.returnright, returnright) OrElse returnright <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "return right must be a positive integer !",.field = "returnright"}}
                End If
            End If

            If Not BlindName = "Track Only" Then
                If String.IsNullOrEmpty(data.bottom) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "bottom hem is required !",.field = "bottom"}}
                End If
                If String.IsNullOrEmpty(data.tie) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "tie back req is required !",.field = "tie"}}
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

            Dim FabricGroup As String = publicCfg.GetFabricGroup(data.fabriccolour)
            Dim PriceGroupName As String = String.Format("{0} - {1}", BlindName, FabricGroup)
            If BlindName = "Track Only" Then
                PriceGroupName = String.Format("Curtain - {0}", BlindName)
            End If

            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            If PriceGroupId = "" Then
                Throw New Exception("Something went wrong !")
            End If

            If BlindName = "Curtain Only" Then
                data.tracktype = ""
                data.trackcolour = ""
                data.trackdraw = ""
                data.returnleft = ""
                data.returnright = ""
            End IF

            If BlindName = "Track Only" Then
                data.drop = ""
                drop = 0
                data.fabrictype = ""
                data.fabriccolour = ""
                data.bottom = ""
                data.tie = ""
            End IF

            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, FabricId, PriceGroupId, Qty, Location, Mounting, MaterialChain, Width, [Drop], TrackType, TrackColour, Cleat, StackPosition, PelmetReturnSize, PelmetReturnSize2, BottomHoldDown, ChildSafe, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @FabricId, @PriceGroupId, @Qty, @Location, @Mounting, @MaterialChain, @Width, @Drop, @TrackType, @TrackColour, @Cleat, @StackPosition, @PelmetReturnSize, @PelmetReturnSize2, @BottomHoldDown, @ChildSafe, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.tubetype), DBNull.Value, UCase(data.tubetype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@MaterialChain", data.curtainheading)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                        myCmd.Parameters.AddWithValue("@Cleat", data.trackdraw)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                        myCmd.Parameters.AddWithValue("@PelmetReturnSize", returnleft)
                        myCmd.Parameters.AddWithValue("@PelmetReturnSize2", returnright)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.bottom)
                        myCmd.Parameters.AddWithValue("@ChildSafe", data.tie)
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
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, FabricId=@FabricId,PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, MaterialChain=@MaterialChain, Width=@Width, [Drop]=@Drop, TrackType=@TrackType, TrackColour=@TrackColour, Cleat=@Cleat, StackPosition=@StackPosition, PelmetReturnSize=@PelmetReturnSize, PelmetReturnSize2=@PelmetReturnSize2, BottomHoldDown=@BottomHoldDown, ChildSafe=@ChildSafe, Notes=@Notes, Matrix=0.00, Charge=0.00, TotalMatrix=0.00, TotalCharge=0.00, MarkUp=@MarkUp WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.tubetype), DBNull.Value, UCase(data.tubetype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@MaterialChain", data.curtainheading)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                        myCmd.Parameters.AddWithValue("@Cleat", data.trackdraw)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                        myCmd.Parameters.AddWithValue("@PelmetReturnSize", returnleft)
                        myCmd.Parameters.AddWithValue("@PelmetReturnSize2", returnright)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.bottom)
                        myCmd.Parameters.AddWithValue("@ChildSafe", data.tie)
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
