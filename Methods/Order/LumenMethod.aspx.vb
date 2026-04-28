Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Partial Class Methods_Order_LumenMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        '#Submit OrderHeader
        Public Property blindtype As String
        Public Property controltype As String
        Public Property qty As String
        Public Property room As String
        Public Property mounting As String
        Public Property width As String
        Public Property drop As String
        Public Property fabrictype As String
        Public Property fabriccolour As String
        Public Property railcolour As String
        Public Property controlposition As String
        Public Property chaincolour As String
        Public Property chainlength As String
        Public Property motoroption As String
        Public Property remoteoption As String
        Public Property chargeroption As String
        Public Property headboxtype As String
        Public Property headboxcolour As String
        Public Property side As String
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
    Public Shared Function BindControlType(ByVal designid As String, ByVal blindid As String) As Object
        Try
            Dim MyQuery As String = String.Format("SELECT *, UPPER(ControlType) AS ControlText FROM HardwareKits WHERE DesignId='{0}' AND BlindId = '{1}' AND Active=1 ORDER BY Name ASC", designid, UCase(blindid).ToString())
            Dim datas As DataSet = publicCfg.GetListData(MyQuery)
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

    Private Shared Function InArray(value As String, ParamArray list() As String) As Boolean
        Return list.Contains(value)
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
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "mounting is required !", .field = "mounting"}}
            End If

            Dim width As Integer
            If String.IsNullOrEmpty(data.width) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width is required !",.field = "width"}}
            End If
            If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be a positive integer !",.field = "width"}}
            End If
            If width < 300 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 300 !",.field = "width"}}
            End If
            If width > 2600 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 2600 !",.field = "width"}}
            End If

            Dim drop As Integer
            If String.IsNullOrEmpty(data.drop) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop is required !",.field = "drop"}}
            End If
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be a positive integer !",.field = "drop"}}
            End If
            If drop < 600 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be greater than or equal to 600 !",.field = "drop"}}
            End If
            ' If drop > 3200 Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be less than or equal to 3200 !",.field = "drop"}}
            ' End If

            If String.IsNullOrEmpty(data.fabrictype) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type is required !",.field = "fabrictype"}}
            End If
        
            If String.IsNullOrEmpty(data.fabriccolour) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour is required !",.field = "fabriccolour"}}
            End If

            If data.fabrictype = "Sonatine Fresh" Then
                If drop > 2700 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be less than or equal to 2700 !",.field = "drop"}}
                End If
            Else
                If drop > 2600 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be less than or equal to 2600 !",.field = "drop"}}
                End If
            End IF

            If String.IsNullOrEmpty(data.controlposition) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "control position is required !", .field = "controlposition"}}
            End If

            Dim ControlType As String = publicCfg.GetItemData(String.Format("SELECT ControlType FROM HardwareKits WHERE Id = '{0}'", data.controltype))
            If InArray(ControlType, "Chain", "Cord") Then
                If String.IsNullOrEmpty(data.chaincolour) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = String.Format("{0} colour is required !", controltype), .field = "chaincolour"}}
                End If
                If ControlType = "Cord" Then
                    If String.IsNullOrEmpty(data.chainlength) Then
                        Return New ErrorResponse With { .error = New ErrorDetail With { .message = "cord length is required !", .field = "chainlength"}}
                    End If
                End If
            End If

            If ControlType = "Motorised" Then
                If String.IsNullOrEmpty(data.motoroption) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "motor option is required !", .field = "motoroption"}}
                End If
                If String.IsNullOrEmpty(data.remoteoption) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "remote option is required !", .field = "remoteoption"}}
                End If
                If String.IsNullOrEmpty(data.chargeroption) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "charger option is required !", .field = "chargeroption"}}
                End If
                If String.IsNullOrEmpty(data.headboxtype) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "headbox type is required !", .field = "headboxtype"}}
                End If
                If String.IsNullOrEmpty(data.headboxcolour) Then
                    Return New ErrorResponse With { .error = New ErrorDetail With { .message = "headbox colour is required !", .field = "headboxcolour"}}
                End If
            End IF

            If String.IsNullOrEmpty(data.railcolour) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "rail colour is required !", .field = "railcolour"}}
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

            Dim SoeId As String = publicCfg.GetSoeKitId(data.controltype)
            Dim DesignName As String = publicCfg.GetDesignName(data.designid)
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            Dim FabricGroup As String = publicCfg.GetFabricGroup(data.fabriccolour)
            Dim PriceGroupName As String = String.Format("{0} - {1}", BlindName, FabricGroup)

            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, PriceGroupName)
            If PriceGroupId = "" Then
                Throw New Exception("Something went wrong !")
            End If

            Dim ChainId As String = ""
            Dim CLength As String = data.chainlength
            If ControlType = "Chain" Then
                Dim ChainColour As String = String.format("({0})", data.chaincolour)

                If String.IsNullOrEmpty(data.chainlength) Or data.chainlength = "0" Then
                    If drop >= 3000 Then
                        CLength = "2200"
                    ElseIf drop >= 2700 Then
                        CLength = "2000"
                    ElseIf drop >= 2400 Then
                        CLength = "1800"
                    ElseIf drop >= 2000 Then
                        CLength = "1500"
                    ElseIf drop >= 1600 Then
                        CLength = "1250"
                    ElseIf drop >= 1300 Then
                        CLength = "1000"
                    ElseIf drop >= 1100 Then
                        CLength = "800"
                    ElseIf drop >= 800 Then
                        CLength = "600"
                    Else
                        CLength = "500"
                    End If
                End If


                Dim ChainName As String = String.Format("{0} Chain + Joiner {1}", CLength, ChainColour)
                Dim FormulaChain As String = publicCfg.GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName))

                IF Not FormulaChain = "" Then
                    ChainId = FormulaChain
                End If
                If FormulaChain = "" Then
                    ChainName = String.Format("Custom Chain + Joiner {0}", ChainColour)
                    ChainId = publicCfg.GetItemData("SELECT Id FROM Chains WHERE Name = '" + ChainName + "'")
                End If

                data.motoroption = ""
                data.remoteoption = ""
                data.chargeroption = ""

            End If

            If ControlType = "Motorised" Then
                data.chainlength = ""
                data.chaincolour = ""
                ChainId = ""
            End If

            Dim squareMetre As Decimal = Math.Round(width * drop / 1000000, 4)
            Dim linearMetre As Decimal = Math.Round(width / 1000, 4)


            ' Throw New Exception(CLength)

            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim ItemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, BlindNo, KitId, SoeKitId, ExactId, FabricId, ChainId, PriceGroupId, Qty, Location, Mounting, Width, [Drop], SwipelColour, ControlPosition, ChainLength, CordColour, CordLength, MotorStyle, MotorRemote, MotorCharger, TrackType, TrackColour, SideBySide, SquareMetre, LinearMetre, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @ExactId, @FabricId, @ChainId, @PriceGroupId, @Qty, @Location, @Mounting, @Width, @Drop, @SwipelColour, @ControlPosition, @ChainLength, @CordColour, @CordLength, @MotorStyle, @MotorRemote, @MotorCharger, @TrackType, @TrackColour, @SideBySide, @SquareMetre, @LinearMetre, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", UCase(data.fabriccolour).ToString())
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@SwipelColour", data.railcolour)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", CLength)
                        myCmd.Parameters.AddWithValue("@CordColour", data.chaincolour)
                        myCmd.Parameters.AddWithValue("@CordLength", data.chainlength)
                        myCmd.Parameters.AddWithValue("@MotorStyle", data.motoroption)
                        myCmd.Parameters.AddWithValue("@MotorRemote", data.remoteoption)
                        myCmd.Parameters.AddWithValue("@MotorCharger", data.chargeroption)
                        myCmd.Parameters.AddWithValue("@TrackType", data.headboxtype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.headboxcolour)
                        myCmd.Parameters.AddWithValue("@SideBySide", data.side)
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
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, FabricId=@FabricId, ChainId=@ChainId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, SwipelColour=@SwipelColour, ControlPosition=@ControlPosition, ChainLength=@ChainLength, CordColour=@CordColour, CordLength=@CordLength, MotorStyle=@MotorStyle, MotorRemote=@MotorRemote, MotorCharger=@MotorCharger, TrackType=@TrackType, TrackColour=@TrackColour, SideBySide=@SideBySide, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, Matrix=0.00, Charge=0.00, TotalMatrix=0.00, TotalCharge=0.00, MarkUp=@MarkUp WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@BlindNo", "Blind 1")
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
                        myCmd.Parameters.AddWithValue("@ExactId", If(String.IsNullOrEmpty(ExactId), DBNull.Value, ExactId))
                        myCmd.Parameters.AddWithValue("@FabricId", UCase(data.fabriccolour).ToString())
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, ChainId))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", PriceGroupId)
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@SwipelColour", data.railcolour)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ChainLength", CLength)
                        myCmd.Parameters.AddWithValue("@CordColour", data.chaincolour)
                        myCmd.Parameters.AddWithValue("@CordLength", data.chainlength)
                        myCmd.Parameters.AddWithValue("@MotorStyle", data.motoroption)
                        myCmd.Parameters.AddWithValue("@MotorRemote", data.remoteoption)
                        myCmd.Parameters.AddWithValue("@MotorCharger", data.chargeroption)
                        myCmd.Parameters.AddWithValue("@TrackType", data.headboxtype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.headboxcolour)
                        myCmd.Parameters.AddWithValue("@SideBySide", data.side)
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
