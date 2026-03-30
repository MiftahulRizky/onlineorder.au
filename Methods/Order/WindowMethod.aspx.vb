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
        Public Property framesize As String
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

            If String.IsNullOrEmpty(data.framesize) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "frame size is required !",.field = "framesize"}}
            End If


                
            Dim anglelength As Integer
            Dim angleqty As Integer
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

                If String.IsNullOrEmpty(data.angleqty) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "angle qty is required !",.field = "angleqty"}}
                End If
                If Not Integer.TryParse(data.angleqty, angleqty) OrElse angleqty <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "angle qty must be a positive integer !",.field = "angleqty"}}
                End If
                If angleqty < 1 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "please check your angle qty !",.field = "angleqty"}}
                End If
                If angleqty > 10 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "angle qty must be less than or equal to 10 !",.field = "angleqty"}}
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

            
            
            Dim SoeId As String = publicCfg.GetSoeKitId(data.colourtype)
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            Dim PriceGroupName As String = String.Format("Window - {0}", BlindName)
            If BlindName = "Standard" Then
                PriceGroupName = String.Format("Window - {0} Without Mesh", BlindName)
                If data.meshtype = "SS Mesh" Then
                    PriceGroupName = String.Format("Window - {0} With Mesh", BlindName)
                End If
            End If
            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
        
            If PriceGroupId = "" Then
                Throw New Exception("Something went wrong !")
            End If

            Dim squareMetre As Decimal = Math.Round(width * drop / 1000000, 4)
            Dim linearMetre As Decimal = Math.Round(width / 1000, 4)
            
            
            
            Dim msg As String = "200"
            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, PriceGroupId, Qty, Location, Mounting, Width, [Drop], MeshType, FrameColour, FrameType, Brace, AngleType, AngleLength, AngleQty, PortHole, PlungerPin, SwipelColour, SwipelQty, SwipelQtyB, SpringQty, TopPlasticQty, SquareMetre, LinearMetre, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @PriceGroupId, @Qty, @Location, @Mounting, @Width, @Drop, @MeshType, @FrameColour, @FrameType, @Brace, @AngleType, @AngleLength, @AngleQty, @PortHole, @PlungerPin, @SwipelColour, @SwipelQty, @SwipelQtyB, @SpringQty, @TopPlasticQty, @SquareMetre, @LinearMetre, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@FrameType", data.framesize)
                        myCmd.Parameters.AddWithValue("@Brace", data.brace)
                        myCmd.Parameters.AddWithValue("@AngleType", data.angletype)
                        myCmd.Parameters.AddWithValue("@AngleLength", data.anglelength)
                        myCmd.Parameters.AddWithValue("@AngleQty", data.angleqty)
                        myCmd.Parameters.AddWithValue("@PortHole", data.porthole)
                        myCmd.Parameters.AddWithValue("@PlungerPin", data.plungerpin)
                        myCmd.Parameters.AddWithValue("@SwipelColour", data.swivelcolour)
                        myCmd.Parameters.AddWithValue("@SwipelQty", data.swivelqty)
                        myCmd.Parameters.AddWithValue("@SwipelQtyB", data.swivelqtyb)
                        myCmd.Parameters.AddWithValue("@SpringQty", data.springqty)
                        myCmd.Parameters.AddWithValue("@TopPlasticQty", data.topplasticqty)
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

                Dim dataLog As Object() = {data.headerid, ItemId, "Blinds", data.loginid, "Add Item Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Item added successfully !"
                
            End If


            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then

                Dim ItemId As String = data.itemid
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@width, [Drop]=@Drop, MeshType=@MeshType, FrameColour=@FrameColour, FrameType=@FrameType, Brace=@Brace, AngleType=@AngleType, AngleLength=@AngleLength, AngleQty=@AngleQty, PortHole=@PortHole, PlungerPin=@PlungerPin, SwipelColour=@SwipelColour, SwipelQty=@SwipelQty, SwipelQtyB=@SwipelQtyB, SpringQty=@SpringQty, TopPlasticQty=@TopPlasticQty, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.colourtype), DBNull.Value, UCase(data.colourtype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@FrameType", data.framesize)
                        myCmd.Parameters.AddWithValue("@Brace", data.brace)
                        myCmd.Parameters.AddWithValue("@AngleType", data.angletype)
                        myCmd.Parameters.AddWithValue("@AngleLength", data.anglelength)
                        myCmd.Parameters.AddWithValue("@AngleQty", data.angleqty)
                        myCmd.Parameters.AddWithValue("@PortHole", data.porthole)
                        myCmd.Parameters.AddWithValue("@PlungerPin", data.plungerpin)
                        myCmd.Parameters.AddWithValue("@SwipelColour", data.swivelcolour)
                        myCmd.Parameters.AddWithValue("@SwipelQty", data.swivelqty)
                        myCmd.Parameters.AddWithValue("@SwipelQtyB", data.swivelqtyb)
                        myCmd.Parameters.AddWithValue("@SpringQty", data.springqty)
                        myCmd.Parameters.AddWithValue("@TopPlasticQty", data.topplasticqty)
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
