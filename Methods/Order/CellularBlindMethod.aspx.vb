Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Partial Class Methods_Order_CellularBlindMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        Public Property blindtype As String
        Public Property brackettype As String
        Public Property controltype As String
        Public Property qty As String
        Public Property room As String
        Public Property sizetype As String
        Public Property dropfloor As String
        Public Property mounting As String
        Public Property fabrictype As String
        Public Property fabriccolour As String
        Public Property fabrictype2 As String
        Public Property fabriccolour2 As String
        Public Property width As String
        Public Property drop As String
        Public Property cordtype As String
        Public Property controlposition As String
        Public Property chainlength As String
        Public Property controlsystem As List(Of String)
        Public Property motortype As String
        Public Property motorextra As String
        Public Property holddown As String
        Public Property cutout As String
        Public Property additional As String
        Public Property notes As String

        Public Property markup As String
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
        Public Property brackettype As String
        Public Property tubetype As String
        Public Property controltype As String
        Public Property colourtype As String
        Public Property fabrictype As String
        Public Property fabrictype2 As String
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

                Case "brackettype"
                    query = String.Format("SELECT BracketType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND Active=1 GROUP BY BracketType ORDER BY BracketType ASC", data.designid, UCase(data.blindtype).ToString())
                    Return GetFormattedData(query, "BracketType", "BracketType")

                Case "controltype"
                    query = String.Format("SELECT Id, ControlType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND BracketType='{2}' AND Active=1 ORDER BY ControlType ASC", data.designid, UCase(data.blindtype).ToString(), data.brackettype)
                    Return GetFormattedData(query, "Id", "ControlType")

                Case "fabrictype", "fabrictype2"
                    Dim BlindName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Blinds WHERE Id='{0}'", UCase(data.blindtype).ToString()))
                    Dim ControlName As String = publicCfg.GetItemData(String.Format("SELECT ControlType FROM HardwareKits WHERE Id='{0}'", data.controltype))
                    
                    Dim AditionalQueries As String = "Description NOT LIKE '%Potrait%' AND"
                    If BlindName = "Potrait" Then
                        AditionalQueries = String.Format("Description LIKE '%{0}%' AND", data.brackettype)
                    End If
                    query = String.Format("SELECT Type FROM Fabrics WHERE DesignId='{0}' AND {1} Active='1' GROUP BY Type ORDER BY Type ASC", data.designid, AditionalQueries)
                    Return GetFormattedData(query, "Type", "Type")

                Case "fabriccolour", "fabriccolour2"
                    query = String.Format("SELECT Id, Colour FROM Fabrics WHERE DesignId='{0}' AND Active='1' AND Type='{1}' ORDER BY Name ASC", data.designid, data.fabrictype)
                    Return GetFormattedData(query, "Id", "Colour")

                Case "controlsystem"
                    Dim ControlName As String = publicCfg.GetItemData(String.Format("SELECT ControlType FROM HardwareKits WHERE Id='{0}'", data.controltype))
                    query = String.Format("SELECT Name FROM ControlType WHERE Description LIKE '%{0}%' ORDER BY Name ASC", ControlName)
                    Return GetFormattedData(query, "Name", "Name")

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

            If InArray(BlindName, "Cellora", "Galaxy") Then
                ' If String.IsNullOrEmpty(data.sizetype) Then
                '     Return New ErrorResponse With { .error = New ErrorDetail With { .message = "size type is required !", .field = "sizetype"}}
                ' End If

                '  If data.sizetype = "Opening Size" AND data.mounting = "Face Fit"
                '     If String.IsNullOrEmpty(data.dropfloor) Then
                '         Return New ErrorResponse With { .error = New ErrorDetail With { .message = "drop to the floor is required !", .field = "dropfloor"}}
                '     End If
                ' End If
            End IF

            If String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "mounting is required !",.field = "mounting"}}
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

            If String.IsNullOrEmpty(data.fabrictype) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type is required !",.field = "fabrictype"}}
            End If

            If String.IsNullOrEmpty(data.fabriccolour) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour is required !",.field = "fabriccolour"}}
            End If

            If BlindName =  "Potrait" AND (data.controlsystem Is Nothing OrElse data.controlsystem.Count = 0) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control type is required !",.field = "controlsystem"}}
            End If

            If BlindName = "Galaxy" AND InStr(ControlName, "Corded") > 0 Then
                If String.IsNullOrEmpty(data.cordtype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "cord type is required !",.field = "cordtype"}}
                End If
            End IF

            If data.controlsystem IsNot Nothing AndAlso Not data.controlsystem.Contains("Cordless") Then
                If String.IsNullOrEmpty(data.controlposition) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control side is required !",.field = "controlposition"}}
                End If
            End If

            If data.controlsystem IsNot Nothing AndAlso data.controlsystem.Contains("Motorised") Then
                If String.IsNullOrEmpty(data.motortype) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "motor type is required !",.field = "motortype"}}
                End IF

                If String.IsNullOrEmpty(data.motorextra) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "motor extra is required !",.field = "motorextra"}}
                End IF
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
                markup = 0
            End If


            Dim SoeId As String = publicCfg.GetItemData("SELECT SoeId FROM HardwareKits WHERE Id = '" + data.controltype + "'")

            '#price group 1
            Dim PriceGroupId As String = GetPriceGroupId(data.fabriccolour, BlindName, data.brackettype, ControlName, data.designid)
            If PriceGroupId = "300" Then
                Throw New Exception("Price group not found !")
            End If

            Dim PriceGroupId2 As String = ""
            If BlindName = "Galaxy" And (ControlName = "DN Corded" Or ControlName = "DN Cordless") Then
                PriceGroupId2 = GetPriceGroupId(data.fabriccolour2, BlindName, data.brackettype, ControlName, data.designid)
                If PriceGroupId2 = "300" Then
                    Throw New Exception("Price group 2 not found !")
                End If
            Else
                data.fabriccolour2 = ""
            End If

            Dim squareMetre As Decimal = Math.Round(width * drop / 1000000, 4)
            Dim linearMetre As Decimal = Math.Round(width / 1000, 4)
            If Not BlindName = "Potrait" Then
                squareMetre = 0
                linearMetre = 0
            End If

            Dim ResponseUpdateStatusOrder As String = UpdateStatusOrder(data.headerid, data.fabriccolour)
            If Not ResponseUpdateStatusOrder = "200" Then
                Throw New Exception(ResponseUpdateStatusOrder)
            End If
            
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = DesignName & " - " & BlindName
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            ' Throw New Exception(ExactName & " - " & ExactId)

            If BlindName = "Potrait" Then
                If data.controlsystem IsNot Nothing AndAlso Not data.controlsystem.Contains("Motorised") Then
                    data.motortype = ""
                    data.motorextra = ""
                End If
            Else
                data.controlsystem = New List(Of String)
                data.motortype = ""
                data.motorextra = ""
            End If

            Dim controlSystemValue As String = ""
            If data.controlsystem IsNot Nothing AndAlso data.controlsystem.Any() Then
                controlSystemValue = String.Join(",", data.controlsystem)
            End If

            If Not InArray(BlindName, "Cellora", "Galaxy") Then
                data.sizetype = ""
                data.dropfloor = ""
            End If
            data.sizetype = ""
            data.dropfloor = ""



            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim itemId As String = publicCfg.CreateOrderItemId()


                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, KitId, SoeKitId, ExactId, FabricId, FabricIdB, PriceGroupId, PriceGroupIdB, BlindNo, Qty, Location, LouvreSize, LouvrePosition, Mounting, Width, [Drop], MaterialCord, HangerType, ControlPosition, ChainLength, MotorStyle, AdditionalMotor, BottomHoldDown, DoorCutOut, Accessory, SquareMetre, LinearMetre, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active) VALUES (@Id, @HeaderId, @KitId, @SoeKitId, @ExactId, @FabricId, @FabricIdB, @PriceGroupId, @PriceGroupIdB, 'Blind 1', @Qty, @Location, @LouvreSize, @LouvrePosition, @Mounting, @Width, @Drop, @MaterialCord, @HangerType, @ControlPosition, @ChainLength, @MotorStyle, @AdditionalMotor, @BottomHoldDown, @DoorCutOut, @Accessory, @SquareMetre, @LinearMetre, @Notes, 0, 0, 0, 0, 0, 0, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
                        myCmd.Parameters.AddWithValue("@ExactId", ExactId)
                        myCmd.Parameters.AddWithValue("@FabricId", UCase(data.fabriccolour).ToString())
                        myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabriccolour2), DBNull.Value, UCase(data.fabriccolour2).ToString()))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(PriceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@PriceGroupIdB", If(String.IsNullOrEmpty(PriceGroupId2), DBNull.Value, UCase(PriceGroupId2).ToString()))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.sizetype)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.dropfloor)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MaterialCord", data.cordtype)
                        myCmd.Parameters.AddWithValue("@HangerType", controlSystemValue)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", data.chainlength)
                        myCmd.Parameters.AddWithValue("@MotorStyle", data.motortype)
                        myCmd.Parameters.AddWithValue("@AdditionalMotor", data.motorextra)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.holddown)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.cutout)
                        myCmd.Parameters.AddWithValue("@Accessory", data.additional)
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
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET KitId = @KitId, SoeKitId = @SoeKitId, ExactId = @ExactId, FabricId = @FabricId, FabricIdB = @FabricIdB, PriceGroupId = @PriceGroupId, PriceGroupIdB = @PriceGroupIdB, BlindNo = 'Blind 1', Qty = @Qty, Location = @Location, LouvreSize = @LouvreSize, LouvrePosition = @LouvrePosition, Mounting = @Mounting, Width = @Width, [Drop] = @Drop, MaterialCord = @MaterialCord, HangerType = @HangerType, ControlPosition = @ControlPosition, ChainLength = @ChainLength,  MotorStyle = @MotorStyle, AdditionalMotor = @AdditionalMotor, BottomHoldDown = @BottomHoldDown, DoorCutOut = @DoorCutOut, Accessory = @Accessory, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes = @Notes, MarkUp = @MarkUp WHERE Id = @Id")
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        ' myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
                        myCmd.Parameters.AddWithValue("@ExactId", ExactId)
                        myCmd.Parameters.AddWithValue("@FabricId", UCase(data.fabriccolour).ToString())
                        myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabriccolour2), DBNull.Value, UCase(data.fabriccolour2).ToString()))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(PriceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@PriceGroupIdB", If(String.IsNullOrEmpty(PriceGroupId2), DBNull.Value, UCase(PriceGroupId2).ToString()))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.sizetype)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.dropfloor)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MaterialCord", data.cordtype)
                        myCmd.Parameters.AddWithValue("@HangerType", controlSystemValue)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", data.chainlength)
                        myCmd.Parameters.AddWithValue("@MotorStyle", data.motortype)
                        myCmd.Parameters.AddWithValue("@AdditionalMotor", data.motorextra)
                        myCmd.Parameters.AddWithValue("@BottomHoldDown", data.holddown)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.cutout)
                        myCmd.Parameters.AddWithValue("@Accessory", data.additional)
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

                Dim Group As String = publicCfg.GetItemData(String.Format("SELECT [Group] FROM Fabrics WHERE Id = '{0}'", data.fabriccolour))
                Dim Cost As Decimal = publicCfg.GetItemData(String.Format("SELECT Cost FROM OrderDetailsPrice where HeaderId={0} AND ItemId={1} AND Type='Matrix'", data.headerid, itemId))
                Dim Poa As Decimal = publicCfg.GetItemData(String.Format("SELECT Poa FROM OrderDetailsPrice where HeaderId={0} AND ItemId={1} AND Type='Matrix'", data.headerid, itemId))

                publicCfg.ResetPriceDetail(itemId)
                publicCfg.HitungHarga(data.headerid, itemId)
                publicCfg.HitungSurcharge(data.headerid, itemId)

                IF Group = "POA" Then
                    Dim Res As String = UpdateOverridePricing(itemId, Cost, Poa)
                    If Not Res = "200" Then
                        Throw New Exception(Res)
                    End If

                    Dim Matrix As Decimal = publicCfg.GetItemData(String.Format("SELECT SUM(Cost) As Matrix FROM OrderDetailsPrice WHERE HeaderId={0} AND ItemId={1} AND Type='Matrix'", data.headerid, itemId))
                    publicCfg.UpdateMatrix(UCase(itemId).ToString(), qty, Matrix)
                End If

                Dim dataLog As Object() = {data.headerid, itemId, "Blinds", data.loginid, "Update Item Order"}
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


    Private Shared Function UpdateOverridePricing(id As String, cost As Decimal, poa As Decimal) As String
        Try
            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetailsPrice SET Cost=@Cost, Poa=@Poa WHERE Type='Matrix' AND ItemId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", UCase(id).ToString())
                    myCmd.Parameters.AddWithValue("@Cost", poa)
                    myCmd.Parameters.AddWithValue("@Poa", poa)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function


    Private Shared Function GetPriceGroupId(ByVal fabricid As String, ByVal blindname As String, ByVal brackettype As String, ByVal controlname As String, ByVal designid As String) As String
        Try
            Dim fabricData As DataSet = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + fabricid + "'")
            If fabricData.Tables(0).Rows.Count = 0 Then
                Return "300"
            End If
            
            Dim fabricGroupName As String = fabricData.Tables(0).Rows(0).Item("Group").ToString()

            Dim priceGroupName As String = String.Format("{0} {1} - {2}", blindname, controlname, fabricGroupName)
            If blindname = "Galaxy" Then
                priceGroupName = String.Format("Galaxy {0} - {1}", brackettype, fabricGroupName)
            End If

            If blindname = "Potrait" Then
                Dim bracket As String = "Standard"
                If controlname = "Patio Door Vertical" Then
                    bracket = "Patio"
                End If
                priceGroupName = String.Format("Potrait {0} - {1}", bracket, fabricGroupName)
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

    Private Shared Function UpdateStatusOrder(ByVal headerid As String, ByVal fabricid As String) As String
        Try
            Dim Group As String = publicCfg.GetItemData(String.Format("SELECT [Group] FROM Fabrics WHERE Id = '{0}'", fabricid))
            If String.IsNullOrEmpty(Group) Then
                Return "Group Not Found !"
            End If

            If Not Group = "POA" Then
                Return "200"
            End If

            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderHeaders SET Status = 'Pending Price Approval' WHERE Id = @Id")
                    myCmd.Parameters.AddWithValue("@Id",  UCase(headerid).ToString())
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Return "UpdateStatusOrder Error: " + ex.Message
        End Try
    End Function
End Class
