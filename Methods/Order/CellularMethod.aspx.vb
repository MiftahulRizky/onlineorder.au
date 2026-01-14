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
    Shared orderCfg As New OrderConfig()

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetDesignType(ByVal designid As String) As Object
        Dim designName As String = publicCfg.GetDesignName(designid)
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
    Public Shared Function BindBracketType(ByVal designid As String, ByVal blindid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT BracketType FROM HardwareKits WHERE DesignId = '" + designid + "' AND BlindId='" + UCase(blindid).ToString() + "' AND Active=1 GROUP BY BracketType ORDER BY BracketType ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("BracketType").ToString()},
                        {"text", row("BracketType").ToString()}
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
    Public Shared Function BindControlType(ByVal designid As String, ByVal blindid As String, ByVal brackettype As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, ControlType FROM HardwareKits WHERE DesignId = '" + designid + "' AND BlindId='" + UCase(blindid).ToString() + "' AND BracketType='" + brackettype + "' AND Active=1 ORDER BY ControlType ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("ControlType").ToString()}
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
            
            Dim datas As DataSet = publicCfg.GetListData("SELECT Type FROM Fabrics WHERE DesignId='" + designid + "' AND Active='1' GROUP BY Type ORDER BY Type ASC")
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
    Public Shared Function BindFabricColour(ByVal designid As String, ByVal fabricType As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, Colour FROM Fabrics WHERE DesignId='" + designid + "' AND Active='1' AND Type='" + fabricType + "' ORDER BY Name ASC")
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
        Public Property brackettype As String
        Public Property controltype As String
        Public Property qty As String
        Public Property room As String
        Public Property mounting As String
        Public Property fabrictype As String
        Public Property fabriccolour As String
        Public Property fabrictype2 As String
        Public Property fabriccolour2 As String
        Public Property cordtype As String
        Public Property width As String
        Public Property drop As String
        Public Property controlposition As String
        Public Property chainlength As String
        Public Property holddown As String
        Public Property cutout As String
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

            If String.IsNullOrEmpty(data.controltype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "colour is required !", .field = "controltype"}}
            End If

            Dim blindName As String = publicCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + data.blindtype + "'")
            Dim controlName As String = publicCfg.GetItemData("SELECT ControlType FROM HardwareKits WHERE Id = '" + data.controltype + "'")

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

            If blindName = "Galaxy" And (controlName ="DN Corded" Or controlName ="DN Cordless") Then
                If String.IsNullOrEmpty(data.fabrictype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type day is required !",.field = "fabrictype"}}
                End If

                If String.IsNullOrEmpty(data.fabriccolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour day is required !",.field = "fabriccolour"}}
                End If
                If String.IsNullOrEmpty(data.fabrictype2) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type night is required !",.field = "fabrictype2"}}
                End If

                If String.IsNullOrEmpty(data.fabriccolour2) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour night is required !",.field = "fabriccolour2"}}
                End If
            Else
                If String.IsNullOrEmpty(data.fabrictype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type is required !",.field = "fabrictype"}}
                End If

                If String.IsNullOrEmpty(data.fabriccolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour is required !",.field = "fabriccolour"}}
                End If
            End If

            If String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "mounting is required !",.field = "mounting"}}
            End If

            If blindName = "Galaxy" Then
                If String.IsNullOrEmpty(data.cordtype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "cord type is required !",.field = "cordtype"}}
                End If
            End IF

            If String.IsNullOrEmpty(data.controlposition) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control side is required !",.field = "controlposition"}}
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

            Dim soeKitId As String = publicCfg.GetItemData("SELECT SoeId FROM HardwareKits WHERE Id = '" + data.controltype + "'")

            '#price group 1
            Dim priceGroupId As String = GetPriceGroupId(data.fabriccolour, blindName, data.brackettype, controlName, data.designid)
            If priceGroupId = "300" Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Price group not found !",.field = ""}}
            End If

            Dim priceGroupId2 As String = ""
            If blindName = "Galaxy" And (controlName = "DN Corded" Or controlName = "DN Cordless") Then
                priceGroupId2 = GetPriceGroupId(data.fabriccolour2, blindName, data.brackettype, controlName, data.designid)
                If priceGroupId2 = "300" Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "203.2 : Something went wrong !",.field = ""}}
                End If
            End If
            
             Dim designName As String = publicCfg.GetDesignName(data.designid)
            Dim exactName As String = designName & " - " & blindName
            Dim exactId As String = orderCfg.GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")


            Dim msg As String = String.Empty
            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim itemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, KitId, SoeKitId, FabricId, FabricIdB, PriceGroupId, PriceGroupIdB, BlindNo, Qty, Location, Mounting, Width, [Drop], MaterialCord, ControlPosition, ChainLength, BottomHoldDown, DoorCutOut, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active) VALUES (@Id, @HeaderId, @KitId, @SoeKitId, @FabricId, @FabricIdB, @PriceGroupId, @PriceGroupIdB, 'Blind 1', @Qty, @Location, @Mounting, @Width, @Drop, @MaterialCord, @ControlPosition, @ChainLength, @BottomHoldDown, @DoorCutOut, @Notes, 0, 0, 0, 0, 0, 0, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", soeKitId)
                        myCmd.Parameters.AddWithValue("@ExactId", exactId)
                        myCmd.Parameters.AddWithValue("@FabricId", UCase(data.fabriccolour).ToString())
                        myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabriccolour2), DBNull.Value, UCase(data.fabriccolour2).ToString()))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(priceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@PriceGroupIdB", If(String.IsNullOrEmpty(priceGroupId2), DBNull.Value, UCase(priceGroupId2).ToString()))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MaterialCord", data.cordtype)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", data.chainlength)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.holddown)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.cutout)
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

                Dim dataLog As Object() = {data.headerid, itemId, "Blinds", data.loginid, "Add Item Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Item added successfully !"
            End If

            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then

                Dim itemId As String = data.itemid

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET KitId = @KitId, SoeKitId = @SoeKitId, FabricId = @FabricId, FabricIdB = @FabricIdB, PriceGroupId = @PriceGroupId, PriceGroupIdB = @PriceGroupIdB, BlindNo = 'Blind 1', Qty = @Qty, Location = @Location, Mounting = @Mounting, Width = @Width, [Drop] = @Drop, MaterialCord = @MaterialCord, ControlPosition = @ControlPosition, ChainLength = @ChainLength, BottomHoldDown = @BottomHoldDown, DoorCutOut = @DoorCutOut, Notes = @Notes, MarkUp = @MarkUp WHERE Id = @Id")
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        ' myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", soeKitId)
                        myCmd.Parameters.AddWithValue("@ExactId", exactId)
                        myCmd.Parameters.AddWithValue("@FabricId", UCase(data.fabriccolour).ToString())
                        myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabriccolour2), DBNull.Value, UCase(data.fabriccolour2).ToString()))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(priceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@PriceGroupIdB", If(String.IsNullOrEmpty(priceGroupId2), DBNull.Value, UCase(priceGroupId2).ToString()))
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MaterialCord", data.cordtype)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", data.chainlength)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.holddown)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.cutout)
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

                Dim dataLog As Object() = {data.headerid, itemId, "Blinds", data.loginid, "Update Item Order"}
                orderCfg.Log_Orders(dataLog)

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


    Private Shared Function GetPriceGroupId(ByVal fabricid As String, ByVal blindname As String, ByVal brackettype As String, ByVal controlname As String, ByVal designid As String) As String
        Try
            Dim fabricData As DataSet = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + fabricid + "'")
            If fabricData.Tables(0).Rows.Count = 0 Then
                Return "300"
            End If
            
            Dim fabricGroupName As String = fabricData.Tables(0).Rows(0).Item("Group").ToString()

            Dim priceGroupName As String =  blindName & " " & controlName & " - " & fabricGroupName
            If blindname = "Galaxy" Then
                priceGroupName = blindName & " " & brackettype & " - " & fabricGroupName
            End If

            Dim priceGroupId As String = publicCfg.GetPriceGroupId(designid ,priceGroupName)
            If String.IsNullOrEmpty(priceGroupId) Then
                Return "300"
            End If

            Return priceGroupId
        Catch ex As Exception
            Return "300"
        End Try
    End Function
End Class
