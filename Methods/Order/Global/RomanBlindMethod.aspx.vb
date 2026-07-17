Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Partial Class Methods_Order_RomanBlindMethod
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
        Public Property fabrictype As String
        Public Property fabriccolour As String
        Public Property width As String
        Public Property drop As String
        Public Property controlposition As String
        Public Property materialchain As String
        Public Property chaincolour As String
        Public Property chainlength As String
        Public Property cordcolour As String
        Public Property cordlength As String
        Public Property plasticcolour As String
        Public Property cleat As String
        Public Property battencolour As String
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
        Public Property controltype As String
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

                Case "controltype"
                    query = String.Format("SELECT Id, ControlType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId='{1}' AND Active=1 ORDER BY ControlType ASC", data.designid, UCase(data.blindtype).ToString())
                    Return GetFormattedData(query, "Id", "ControlType")

                Case "fabrictype"
                    Dim BlindName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Blinds WHERE Id='{0}'", UCase(data.blindtype).ToString()))
                    Dim ControlName As String = publicCfg.GetItemData(String.Format("SELECT ControlType FROM HardwareKits WHERE Id='{0}'", UCase(data.controltype).ToString()))
                    Dim des As String = ""
                    Select Case BlindName
                        Case "Classic"
                            If ControlName = "Cord" Then
                                des = "Cord Classic"
                            End If
                            If ControlName = "Chain" Then
                                des = "Chain Classic"
                            End If
                        Case "Plantation"
                            If ControlName = "Chain" Then
                                des = "Chain Plantation"
                            End If
                            If ControlName = "Cord" Then
                                des = "Cord Plantation"
                            End If
                        Case "Sewless"
                            If ControlName = "Chain" Then
                                des = "Chain Sewless"
                            End If
                            If ControlName = "Cord" Then
                                des = "Cord Sewless"
                            End If
                    End Select
                    query = String.Format("SELECT Type FROM Fabrics WHERE DesignId='{0}' AND Description LIKE '%{1}%'AND Active='1' GROUP BY Type ORDER BY Type ASC", data.designid, des)
                    Return GetFormattedData(query, "Type", "Type")

                Case "fabriccolour"
                    query = String.Format("SELECT Id, Colour FROM Fabrics WHERE DesignId='{0}' AND Active='1' AND Type='{1}' ORDER BY Name ASC", data.designid, data.fabrictype)
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
            Dim ControlName As String = publicCfg.GetItemData(String.Format("SELECT ControlType FROM HardwareKits WHERE Id = '{0}'", data.controltype))

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
            If String.IsNullOrEmpty(data.width) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width is required !",.field = "width"}}
            End If
            If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be a positive integer !",.field = "width"}}
            End If
            ' If width < 150 Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 150 !",.field = "width"}}
            ' End If
            If width > 3000 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "width must be less than or equal to 3000 !",.field = "width"}}
            End If

            
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
            If drop > 3200 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "drop must be less than or equal to 3200 !",.field = "drop"}}
            End If

            If String.IsNullOrEmpty(data.fabrictype) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric type is required !",.field = "fabrictype"}}
            End If

            If Not String.IsNullOrEmpty(data.fabrictype) Then
                If String.IsNullOrEmpty(data.fabriccolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "fabric colour is required !",.field = "fabriccolour"}}
                End If
            End If

            If String.IsNullOrEmpty(data.controlposition) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "control position is required !",.field = "controlposition"}}
            End If

            Dim chainlength As Integer
            If ControlName = "Chain" Then
                If String.IsNullOrEmpty(data.materialchain) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "material chain is required !",.field = "materialchain"}}
                End If

                If String.IsNullOrEmpty(data.chaincolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "chaincolour"}}
                End If

                If Not String.IsNullOrEmpty(data.chainlength) Then
                    If Not Integer.TryParse(data.chainlength, chainlength) OrElse chainlength <= 0 Then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain length must be a positive integer !",.field = "chainlength"}}
                    End If
                End If
            End If

            Dim cordlength As Integer
            If ControlName = "Cord" Then
                If String.IsNullOrEmpty(data.cordcolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "cord colour is required !",.field = "cordcolour"}}
                End If

                If String.IsNullOrEmpty(data.cordlength) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "cord length is required !",.field = "cordlength"}}
                End If

                If Not Integer.TryParse(data.cordlength, cordlength) OrElse cordlength <= 0 Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "cord length must be a positive integer !",.field = "cordlength"}}
                End If
            End If

            If Not BlindName = "Classic" Then
                If String.IsNullOrEmpty(data.battencolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "chain colour is required !",.field = "battencolour"}}
                End If
            End If

            If ControlName = "Cord" Then
                If String.IsNullOrEmpty(data.plasticcolour) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "plastic colour is required !",.field = "plasticcolour"}}
                End If

                If String.IsNullOrEmpty(data.cleat) Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "cleat is required !",.field = "cleat"}}
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


            Dim ChainId As String
            Dim chaincolour As String = String.Format("({0})", data.chaincolour)
            If ControlName = "Chain" Then
                If String.IsNullOrEmpty(data.chainlength) Then
                    chainlength = "500"
                    If data.drop > 700 Then : chainlength = 600 : End If
                    If data.drop > 800 Then : chainlength = 800 : End If
                    If data.drop > 1100 Then : chainlength = 1000 : End If
                    If data.drop > 1300 Then : chainlength = 1200 : End If
                    If data.drop > 1600 Then : chainlength = 1500 : End If
                    If data.drop > 2000 Then : chainlength = 1800 : End If
                    If data.drop > 2400 Then : chainlength = 2000 : End If
                    If data.drop > 2700 Then : chainlength = 2200 : End If
                End If

                Dim ChainName As String = String.Format("{0} Chain + Joiner {1}", chainlength, chaincolour)
                ChainId = publicCfg.GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName))

                If String.IsNullOrEmpty(ChainId) Then
                    ChainName = String.Format("Custom Chain + Joiner {0}", chaincolour)
                    ChainId = publicCfg.GetItemData(String.Format("SELECT Id FROM Chains WHERE Name = '{0}'", ChainName))
                End If

                data.cordcolour = "" : data.cordlength = "" : data.plasticcolour ="" : data.cleat = ""
            End If

            If ControlName = "Cord" Then
                '# kosongkan opsi chain
                ChainId = "" : data.materialchain = "" : data.chaincolour = "" : data.chainlength = "" : chainlength = 0
                If BlindName = "Classic" Then : data.battencolour = "" : End If
            End If

            Dim SoeId As String = publicCfg.GetItemData("SELECT SoeId FROM HardwareKits WHERE Id = '" + data.controlType + "'")
            Dim FabricGroup As String = publicCfg.GetItemData(String.Format("SELECT [Group] FROM Fabrics WHERE Id = '{0}'", data.fabriccolour))

            Dim PriceGroupName As String =  String.Format("Roman Blind - {0} {1} {2}", ControlName, BlindName, FabricGroup)
            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designId, PriceGroupName)
            If String.IsNullOrEmpty(PriceGroupId) Then
                Throw New Exception("Price group not found !")
            End If

            Dim DesignName As String = publicCfg.GetDesignName(data.designId)
            Dim ExactName As String = String.Format("{0} - {1}", DesignName, BlindName)
            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", ExactName))

            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim itemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, KitId, SoeKitId, ExactId, FabricId, ChainId, PriceGroupId, BlindNo, Qty, Location, Mounting, Width, [Drop], ControlPosition, MaterialChain, ChainLength, CordColour, CordLength, BattenColour,  AcornPlasticColour, Cleat, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active) VALUES (@Id, @HeaderId, @KitId, @SoeKitId, @ExactId, @FabricId, @ChainId, @PriceGroupId, 'Blind 1', @Qty, @Location, @Mounting, @Width, @Drop, @ControlPosition, @MaterialChain, @ChainLength, @CordColour, @CordLength, @BattenColour, @AcornPlasticColour, @Cleat, @Notes, 0, 0, 0, 0, 0, 0, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.controltype), DBNull.Value, UCase(data.controltype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
                        myCmd.Parameters.AddWithValue("@ExactId", ExactId)
                        myCmd.Parameters.AddWithValue("@FabricId", IF(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, UCase(ChainId).ToString()))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, UCase(PriceGroupId).ToString()))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@MaterialChain", data.materialchain)
                        myCmd.Parameters.AddWithValue("@ChainLength", chainlength)
                        myCmd.Parameters.AddWithValue("@CordColour", data.cordcolour)
                        myCmd.Parameters.AddWithValue("@CordLength", cordlength)
                        myCmd.Parameters.AddWithValue("@BattenColour", data.battencolour)
                        myCmd.Parameters.AddWithValue("@AcornPlasticColour", data.plasticcolour)
                        myCmd.Parameters.AddWithValue("@Cleat", data.cleat)
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
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET KitId=@KitId, SoeKitId=@SoeKitId, ExactId=@ExactId, FabricId=@FabricId, ChainId=@ChainId, PriceGroupId=@PriceGroupId, BlindNo='Blind 1', Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, ControlPosition=@ControlPosition, MaterialChain=@MaterialChain, ChainLength=@ChainLength, CordColour=@CordColour, CordLength=@CordLength, BattenColour=@BattenColour, AcornPlasticColour=@AcornPlasticColour, Cleat=@Cleat, Notes=@Notes, Matrix=0, Charge=0, Discount=0, TotalMatrix=0, TotalCharge=0, TotalDiscount=0, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        ' myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(data.controltype), DBNull.Value, UCase(data.controltype).ToString()))
                        myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
                        myCmd.Parameters.AddWithValue("@ExactId", ExactId)
                        myCmd.Parameters.AddWithValue("@FabricId", IF(String.IsNullOrEmpty(data.fabriccolour), DBNull.Value, UCase(data.fabriccolour).ToString()))
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(ChainId), DBNull.Value, UCase(ChainId).ToString()))
                        myCmd.Parameters.AddWithValue("@PriceGroupId", If(String.IsNullOrEmpty(PriceGroupId), DBNull.Value, UCase(PriceGroupId).ToString()))
                        myCmd.Parameters.AddWithValue("@Qty", qty)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@MaterialChain", data.materialchain)
                        myCmd.Parameters.AddWithValue("@ChainLength", chainlength)
                        myCmd.Parameters.AddWithValue("@CordColour", data.cordcolour)
                        myCmd.Parameters.AddWithValue("@CordLength", cordlength)
                        myCmd.Parameters.AddWithValue("@BattenColour", data.battencolour)
                        myCmd.Parameters.AddWithValue("@AcornPlasticColour", data.plasticcolour)
                        myCmd.Parameters.AddWithValue("@Cleat", data.cleat)
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
            Dim msg As String = ex.Message
            If Not data.rolename = "Administrator" Then msg = "Please contact our IT team at support@onlineorder.au"
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message, .field = ""}}
        End Try
    End Function

End Class
