Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic

Partial Class Methods_Order_CelloraMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetDesignType(ByVal designId As String) As Object
        Dim designName As String = publicCfg.GetDesignName(designId)
        Dim result As New Dictionary(Of String, String) From {
            {"designName", designName}
        }
        Return result
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetHeaderData(ByVal headerId As String) As Object
        Dim orderno As String = publicCfg.GetOrderNo(headerId)
        Dim ordercust As String = publicCfg.GetOrderCust(headerId)
        Dim result As New Dictionary(Of String, String) From {
            {"orderNo", orderno},
            {"orderCust", ordercust}
        }
        Return result
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindBlindType(ByVal designId As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Blinds WHERE DesignId='" + designId + "' AND Active=1 ORDER BY Name ASC")
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
    Public Shared Function BindColourType(ByVal designId As String, ByVal blindId As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, ColourType FROM HardwareKits WHERE DesignId = '" + designId + "' AND BlindId='" + UCase(blindId).ToString() + "' AND Active=1 ORDER BY ColourType ASC")
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
    Public Shared Function BindFabricType(ByVal designId As String) As Object
        Try
            
            Dim datas As DataSet = publicCfg.GetListData("SELECT Type FROM Fabrics WHERE DesignId='" + designId + "' AND Active='1' GROUP BY Type ORDER BY Type ASC")
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
    Public Shared Function BindFabricColour(ByVal designId As String, ByVal fabricType As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, Colour FROM Fabrics WHERE DesignId='" + designId + "' AND Active='1' AND Type='" + fabricType + "' ORDER BY Name ASC")
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
    Public Shared Function BindItemOrder(ByVal itemId As String) As Object
        Try
            ' Gunakan parameterized query (idealnya pakai SqlParameter, ini simulasi fungsi GetListData Anda)
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id = '" + itemId + "'")

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


    Public Class FormData
        Public Property blindtype As String
        Public Property colourtype As String
        Public Property qty As String
        Public Property room As String
        Public Property mounting As String
        Public Property fabrictype As String
        Public Property fabriccolour As String
        Public Property width As String
        Public Property drop As String
        Public Property controlposition As String
        Public Property chainlength As String
        Public Property notes As String
        Public Property markup As String
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
    End Class

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SubmitForm(ByVal data As FormData) As Object
        Try
            If String.IsNullOrEmpty(data.blindtype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "type is required !", .field = "blindtype"}}
            End If

            If String.IsNullOrEmpty(data.colourtype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "colour is required !", .field = "colourtype"}}
            End If

            Dim blindName As String = publicCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + data.blindtype + "'")

            Dim qty As Integer
            If String.IsNullOrEmpty(data.qty) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "qty is required !", .field = "qty"}}
            End If
            If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "qty must be a positive integer !",.field = "qty"}}
            End If
            If qty > 5 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "qty must be less than or equal to 5 !",.field = "qty"}}
            End If

            If String.IsNullOrEmpty(data.room) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "room to install is required !",.field = "room"}}
            End If

            If String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "mounting is required !",.field = "mounting"}}
            End If

            ' If String.IsNullOrEmpty(data.fabrictype) Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type is required !",.field = "fabrictype"}}
            ' End If

            ' If String.IsNullOrEmpty(data.fabriccolour) Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour is required !",.field = "fabriccolour"}}
            ' End If

            Dim width As Integer
            If String.IsNullOrEmpty(data.width) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width is required !",.field = "width"}}
            End If
            If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be a positive integer !",.field = "width"}}
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
            If drop > 3000 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be less than or equal to 3000 !",.field = "drop"}}
            End If

            If String.IsNullOrEmpty(data.controlposition) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control side is required !",.field = "controlposition"}}
            End If

            Dim chainlength As Integer
            If String.IsNullOrEmpty(data.chainlength) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length is required !",.field = "chainlength"}}
            End If
            If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
            End If
            If chainlength > 3000 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chainl ength must be less than or equal to 3000 !",.field = "chainlength"}}
            End If

            If Not String.IsNullOrEmpty(data.notes) Then
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

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

            Dim soeKitId As String = publicCfg.GetItemData("SELECT SoeId FROM HardwareKits WHERE Id = '" + data.colourtype + "'")
            Dim fabricData As DataSet = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + data.fabriccolour + "'")
            If fabricData.Tables(0).Rows.Count = 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "203.1 : Something went wrong !",.field = ""}}
            End If
            
            Dim fabricId As String = fabricData.Tables(0).Rows(0).Item("Id").ToString()
            Dim fabricGroupName As String = fabricData.Tables(0).Rows(0).Item("Group").ToString()

            Dim peiceGroupName As String = "Cellora " & blindName & " - " & fabricGroupName
            Dim priceGroupId As String = publicCfg.GetPriceGroupId(data.designId,peiceGroupName)
            If String.IsNullOrEmpty(priceGroupId) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "203.2 : Something went wrong !",.field = ""}}
            End If

            Dim msg As String = String.Empty
            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim itemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, KitId, SoeKitId, FabricId, PriceGroupId, BlindNo, Qty, Location, Mounting, Width, [Drop], ControlPosition, ChainLength, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active) VALUES (@Id, @HeaderId, @KitId, @SoeKitId, @FabricId, @PriceGroupId, 'Blind 1', @Qty, @Location, @Mounting, @Width, @Drop, @ControlPosition, @ChainLength, @Notes, 0, 0, 0, 0, 0, 0, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.colourtype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", soeKitId)
                        myCmd.Parameters.AddWithValue("@FabricId", fabricId)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(priceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", data.chainlength)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                publicCfg.ResetPriceDetail(itemId)
                publicCfg.HitungHarga(data.headerid, itemId)
                publicCfg.HitungSurcharge(data.headerid, itemId)


                msg = "Item added successfully !"
            End If

            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then

                Dim itemId As String = data.itemid

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET  KitId = @KitId, SoeKitId = @SoeKitId, FabricId = @FabricId, PriceGroupId = @PriceGroupId, BlindNo = 'Blind 1', Qty = @Qty, Location = @Location, Mounting = @Mounting, Width = @Width, [Drop] = @Drop, ControlPosition = @ControlPosition, ChainLength = @ChainLength, Notes = @Notes, MarkUp = @MarkUp WHERE Id = @Id")
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        ' myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.colourtype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", soeKitId)
                        myCmd.Parameters.AddWithValue("@FabricId", fabricId)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(priceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", data.chainlength)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                publicCfg.ResetPriceDetail(itemId)
                publicCfg.HitungHarga(data.headerid, itemId)
                publicCfg.HitungSurcharge(data.headerid, itemId)

                msg = "Item updated successfully !"
            End If


            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function
End Class
