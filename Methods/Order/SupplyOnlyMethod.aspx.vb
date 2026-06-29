
Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Partial Class Methods_Order_SupplyOnlyMethod
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

        Public Property size As String
        Public Property widthinput As String
        Public Property widthselect As String
        Public Property drop As String
        Public Property length As String
        Public Property colour As String
        Public Property cutout As String
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

            ' If Not String.IsNullOrEmpty(data.room) Then
            '     If InStr(data.room, "&") > 0 Then
            '         Return New ErrorResponse With { .error = New ErrorDetail With { .message = "character [&] is not allowed !", .field = "room"}}
            '     End If
            ' End If

            If InArray(BlindName, "Mesh Only") AND InArray(TubeName, "Ultra Barrier Mesh") Then
                If String.IsNullOrEmpty(data.size) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "size is required !",.field = "size"}}
                End If
            End If

            Dim widthinput As Integer
            Dim drop As Integer
            If InArray(BlindName, "Frame Only") OR (InArray(BlindName, "Mesh Only") AND InArray(TubeName, "Ultra Barrier Mesh")) Then
                If String.IsNullOrEmpty(data.widthinput) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width is required !",.field = "widthinput"}}
                End If
                If Not Integer.TryParse(data.widthinput, widthinput) OrElse widthinput <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be a positive integer !",.field = "widthinput"}}
                End If

                If String.IsNullOrEmpty(data.drop) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop is required !",.field = "drop"}}
                End If
                If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be a positive integer !",.field = "drop"}}
                End If
            End If

            Dim length As Decimal
            If InArray(BlindName, "Mesh Only") AND NOT InArray(TubeName, "Ultra Barrier Mesh") Then
                If String.IsNullOrEmpty(data.widthselect) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width is required !",.field = "widthselect"}}
                End If

                If String.IsNullOrEmpty(data.length) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "length is required !",.field = "length"}}
                End If
                If Not Decimal.TryParse(data.length.Replace(".", ","), length) OrElse length <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "length must be a positive number !",.field = "length"}}
                End If
            End If

            Dim cutout As Integer
            If InArray(BlindName, "Frame Only") Then
                If String.IsNullOrEmpty(data.cutout) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "cutout is required !",.field = "cutout"}}
                End If
                If Not Integer.TryParse(data.cutout, cutout) OrElse cutout <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "cutout must be a positive integer !",.field = "cutout"}}
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

            Dim PriceGroupName As String = DesignName
            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            If PriceGroupId = "" Then
                Throw New Exception("Something went wrong !")
            End If

            Dim FindWidth As Integer = 0
            Dim FindDrop As Integer = 0

            If InArray(BlindName, "Frame Only") Then
                data.size =""
                data.widthselect = ""
                data.length = ""
                length = 0

                FindWidth = widthinput
                FindDrop = drop
            End If

            If InArray(BlindName, "Mesh Only") AND NOT InArray(TubeName, "Ultra Barrier Mesh") Then
                data.widthinput = ""
                widthinput = 0
                data.drop = ""
                drop = 0
                data.cutout = ""
                cutout = 0

                FindWidth = data.widthselect
                FindDrop = 0
            End If

            If InArray(BlindName, "Mesh Only") AND InArray(TubeName, "Ultra Barrier Mesh") Then
                data.widthselect = ""
                data.length = ""
                length = 0
                data.cutout = ""
                cutout = 0

                FindWidth = widthinput
                FindDrop = drop
            End If
           
            Dim squareMetre As Decimal = 0.00'Math.Round(width * drop / 1000000, 4)
            Dim linearMetre As Decimal = 0.00'Math.Round(width / 1000, 4)
            If InArray(BlindName, "Mesh Only") AND NOT InArray(TubeName, "Ultra Barrier Mesh") Then
                squareMetre = 0.00
                linearMetre = length
            End If

            If InArray(BlindName, "Frame Only") OR (InArray(BlindName, "Mesh Only") AND InArray(TubeName, "Ultra Barrier Mesh")) Then
                squareMetre = Math.Round(FindWidth * FindDrop / 1000000, 4)
                linearMetre = Math.Round(FindWidth / 1000, 4)
            End If


            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()
            
                Dim Field As String = "Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, PriceGroupId, Qty, Location, PelmetSize, PanelSize, Width, [Drop], SwipelColour, CutOut_LeftTop, SquareMetre, LinearMetre, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active"

                Dim Values As String = "@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @PriceGroupId, @Qty, @Location, @PelmetSize, @PanelSize, @Width, @Drop, @SwipelColour, @CutOut_LeftTop, @SquareMetre, @LinearMetre, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1"

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
                        myCmd.Parameters.AddWithValue("@PelmetSize", data.size)
                        myCmd.Parameters.AddWithValue("@PanelSize", length.ToString())
                        myCmd.Parameters.AddWithValue("@Width", FindWidth)
                        myCmd.Parameters.AddWithValue("@Drop", FindDrop)
                        myCmd.Parameters.AddWithValue("@SwipelColour", data.colour)
                        myCmd.Parameters.AddWithValue("@CutOut_LeftTop", cutout)
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
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, PelmetSize=@PelmetSize, PanelSize=@PanelSize, Width=@width, [Drop]=@Drop, SwipelColour=@SwipelColour, CutOut_LeftTop=@CutOut_LeftTop, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                         myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.tubetype), DBNull.Value, UCase(data.tubetype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", If(String.IsNullOrEmpty(SoeId), DBNull.Value, SoeId))
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, PriceGroupId))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@PelmetSize", data.size)
                        myCmd.Parameters.AddWithValue("@PanelSize", length.ToString())
                        myCmd.Parameters.AddWithValue("@Width", FindWidth)
                        myCmd.Parameters.AddWithValue("@Drop", FindDrop)
                        myCmd.Parameters.AddWithValue("@SwipelColour", data.colour)
                        myCmd.Parameters.AddWithValue("@CutOut_LeftTop", cutout)
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
