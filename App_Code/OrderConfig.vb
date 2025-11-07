Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Public Class OrderConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Public Function GetListData(thisString As String) As DataSet
        Dim thisCmd As New SqlCommand(thisString)
        Using thisConn As New SqlConnection(myConn)
            Using thisAdapter As New SqlDataAdapter()
                thisCmd.Connection = thisConn
                thisAdapter.SelectCommand = thisCmd
                Using thisDataSet As New DataSet()
                    thisAdapter.Fill(thisDataSet)
                    Return thisDataSet
                End Using
            End Using
        End Using
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetItemDataOnJson(thisString As String, ParamArray parameters() As SqlParameter) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand(thisString, thisConn)
                    If parameters IsNot Nothing AndAlso parameters.Length > 0 Then
                        myCmd.Parameters.AddRange(parameters)
                    End If

                    thisConn.Open()

                    Dim obj As Object = myCmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso Not Convert.IsDBNull(obj) Then
                        result = Convert.ToString(obj)
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function



    Public Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Double = 0.00
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 0.00
        End Try
        Return result
    End Function

    Public Function GetItemData_Integer(thisString As String) As Integer
        Dim result As Integer = 0
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Function GetItemData_Boolean(thisString As String) As Boolean
        Dim result As Boolean = False
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    'GET DATA

    Public Function GetCustomerAddressId() As String
        Dim result As String = String.Empty
        Try
            Dim id As Integer = 0
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM CustomerAddress ORDER BY Id DESC", thisConn)
                    Using rdResult As SqlDataReader = myCmd.ExecuteReader()
                        If rdResult.Read() Then
                            Integer.TryParse(rdResult(0).ToString(), id)
                        End If
                    End Using
                End Using
            End Using

            result = (id + 1).ToString()
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetProductName(id As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT Name FROM Products WHERE Id = '" + id + "'", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetProductPriceName(id As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT Name FROM PriceProductGroups WHERE Id = '" + id + "'", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetProductPriceGroupId(design As String, name As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT Id FROM PriceProductGroups WHERE DesignId = '" + UCase(design).ToString() + "' AND Name = '" + name + "'", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetFabricFactory(fabricId As String) As String
        Dim result As String = String.Empty
        Try
            If Not fabricId = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()
                    Using myCmd As New SqlCommand("SELECT Factory FROM Fabrics WHERE Id = '" + fabricId + "'", thisConn)
                        Using rdResult = myCmd.ExecuteReader
                            While rdResult.Read
                                result = rdResult.Item(0).ToString()
                            End While
                        End Using
                    End Using
                    thisConn.Close()
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetFabricGroup(fabricId As String) As String
        Dim result As String = String.Empty
        Try
            If Not fabricId = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()
                    Using myCmd As New SqlCommand("SELECT [Group] FROM Fabrics WHERE Id = '" + fabricId + "'", thisConn)
                        Using rdResult = myCmd.ExecuteReader
                            While rdResult.Read
                                result = rdResult.Item("Group").ToString()
                            End While
                        End Using
                    End Using
                    thisConn.Close()
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetFabricWeight(fabricId As String) As String
        Dim result As Integer = 0
        Try
            If Not fabricId = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()
                    Using myCmd As New SqlCommand("SELECT Weight FROM Fabrics WHERE Id = '" + fabricId + "'", thisConn)
                        Using rdResult = myCmd.ExecuteReader
                            While rdResult.Read
                                result = rdResult.Item("Weight").ToString()
                            End While
                        End Using
                    End Using
                    thisConn.Close()
                End Using
            End If
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Function GetFabricRailRoad(fabricId As String) As Boolean
        Dim result As Boolean = False
        Try
            If Not fabricId = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()
                    Using myCmd As New SqlCommand("SELECT NoRailRoad FROM Fabrics WHERE Id = '" + fabricId + "'", thisConn)
                        Using rdResult = myCmd.ExecuteReader
                            While rdResult.Read
                                result = rdResult.Item("NoRailRoad")
                            End While
                        End Using
                    End Using
                    thisConn.Close()
                End Using
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function GetFabricWidth(id As String) As Integer
        Dim result As Integer = 0
        Try
            If Not id = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()
                    Using myCmd As New SqlCommand("SELECT Width FROM FabricColours WHERE Id = '" + id + "'", thisConn)
                        Using rdResult = myCmd.ExecuteReader
                            While rdResult.Read
                                result = rdResult.Item("Width").ToString()
                            End While
                        End Using
                    End Using
                    thisConn.Close()
                End Using
            End If
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Function GetGridSpring(type As String, weight As String, width As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(String.Format("SELECT TOP 1 [SpringType] FROM SpringAssist WHERE [Type] = '{0}' AND [Weight] >= '{1}' AND [Width] >= '{2}' ORDER BY [Weight], [Width] ASC", type, weight, width), thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function CreateOrderHeaderId() As String
        Dim result As String = String.Empty
        Try
            Dim idDetail As String = String.Empty
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderHeaders ORDER BY Id DESC", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            idDetail = rdResult.Item("Id").ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
            If idDetail = "" Then : result = 1
            Else : result = CInt(idDetail) + 1
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function CreateOrderItemId() As String
        Dim result As String = String.Empty
        Try
            Dim idDetail As String = ""
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderDetails ORDER BY Id DESC", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            idDetail = rdResult.Item("Id").ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
            If idDetail = "" Then : result = 1
            Else : result = CInt(idDetail) + 1
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function CreateOrderItemNumber(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim idDetail As String = String.Empty
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT TOP 1 Number FROM OrderDetails WHERE HeaderId = '" + HeaderId + "' ORDER BY Number DESC", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            idDetail = rdResult.Item("Number").ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
            If idDetail = "" Then : result = 1
            Else : result = CInt(idDetail) + 1
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    ' Public Function CreateOrderItemId() As Integer
    '     Dim result As Integer = 0
    '     Try
    '         Using thisConn As New SqlConnection(myConn)
    '             thisConn.Open()
    '             Using myCmd As New SqlCommand("SELECT ISNULL(MAX(Id),0)+1 FROM OrderDetails", thisConn)
    '                 myCmd.CommandTimeout = 60 ' Timeout dalam detik
    '                 result = Convert.ToInt32(myCmd.ExecuteScalar())
    '             End Using
    '             thisConn.Close()
    '         End Using
    '     Catch ex As Exception
    '         Throw New Exception("Gagal generate Order Item Id: " & ex.Message)
    '     End Try
    '     Return result
    ' End Function

    ' Public Function CreateOrderItemNumber(HeaderId As String) As Integer
    '     Dim result As Integer = 0
    '     Try
    '         Using thisConn As New SqlConnection(myConn)
    '             thisConn.Open()
    '             Using myCmd As New SqlCommand("SELECT ISNULL(MAX(Number),0)+1 FROM OrderDetails WHERE HeaderId=@HeaderId", thisConn)
    '                 myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
    '                 myCmd.CommandTimeout = 60 ' Timeout dalam detik
    '                 result = Convert.ToInt32(myCmd.ExecuteScalar())
    '             End Using
    '             thisConn.Close()
    '         End Using
    '     Catch ex As Exception
    '         Throw New Exception("Gagal generate Order Item Number: " & ex.Message)
    '     End Try
    '     Return result
    ' End Function





    Public Function ManualItemIdMnet(Id As String, ProductId As String, Production As String)
        Dim result As String = String.Empty
        Try
            Dim designId As String = GetItemData("SELECT DesignId FROM Products WHERE Id = '" + ProductId + "'")
            Dim blindId As String = GetItemData("SELECT BlindId FROM Products WHERE Id = '" + ProductId + "'")
            Dim tubeType As String = GetItemData("SELECT TubeType FROM Products WHERE Id = '" + ProductId + "'")

            Dim designName As String = GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
            Dim blindName As String = GetItemData("SELECT Name FROM Blinds WHERE Id = '" + blindId + "'")

            Dim custGroup As String = GetItemData("SELECT Customers.[Group] FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId = Customers.Id WHERE OrderHeaders.Id = '" + Id + "'")

            If designName = "Zebra Blind" Then
                result = "17010.001"
            End If
            If designName = "Cellular Shades" Then
                result = "17030"
            End If
            If designName = "Veri Shades" Then
                result = "17040"
            End If
            If designName = "Aluminium Blind" Or designName = "Venetian Blind" Or designName = "LS Venetian" Then
                result = "58000.001"
            End If
            If designName = "Curtain" Then
                result = "62010"
            End If
            If designName = "Panel Glide" Or designName = "Pelmet" Or designName = "Roman Blind" Or designName = "Roller Blind" Or designName = "Skin Only" Then
                If Production = "JKT" Then
                    result = "FABRJKT"
                End If
                If Production = "Orion" Then
                    result = "FABRORI"
                End If
                If designName = "Roller Blind" And tubeType = "Como" Then
                    result = "FABRJKTCOMO"
                    If custGroup = "TBP" Then
                        result = "FABRJKTCOMOSP"
                    End If
                End If
            End If
            If designName = "Panorama PVC Parts" Or designName = "Panorama PVC Shutters" Then
                result = "SHPVC"
            End If

            If designName = "Vertical" Then
                If Production = "JKT" Then
                    result = "VERTJKT"
                End If
                If Production = "Orion" Then
                    result = "VERTORI"
                End If
            End If

            If designName = "Additional" Then
                If blindName = "Freight" Then
                    result = "FREIGHT"
                End If
                If blindName = "Packing Fee" Then
                    result = "FZPACK"
                End If
                If blindName = "Installation" Or blindName = "Check Measure" Or blindName = "Takedown" Or blindName = "Travel Charge" Then
                    result = "SISMISC"
                End If
                If blindName = "Minimum Order Surcharge" Then
                    result = "SURCHARGE"
                End If
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function


    ' LOG ACTIVITY
    Public Sub Log_Orders(data As Object())
        Try
            If data.Length = 4 Then
                Dim headerId As String = Convert.ToString(data(0))
                Dim itemId As String = Convert.ToString(data(1))
                Dim loginId As String = Convert.ToString(data(2))
                Dim desc As String = Convert.ToString(data(3))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Log_Orders VALUES (NEWID(), @HeaderId, @ItemId, @ActionBy, GETDATE(), @Description)")
                        myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                        myCmd.Parameters.AddWithValue("@ItemId", itemId)
                        myCmd.Parameters.AddWithValue("@ActionBy", UCase(loginId).ToString())
                        myCmd.Parameters.AddWithValue("@Description", desc)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub


    ' ORDER OTORISASI
    Public Sub ResetAuthorization(HeaderId As String, ItemId As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderAuthorizations WHERE HeaderId=@HeaderId AND ItemId=@ItemId")
                    myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                    myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub UpdateProductType(Id As String)
        Try
            Dim orderType As String = GetItemData("SELECT TOP 1 Designs.Type FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Id ASC")
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET OrderType=@OrderType WHERE Id=@HeaderId")
                    myCmd.Parameters.AddWithValue("@HeaderId", Id)
                    myCmd.Parameters.AddWithValue("@OrderType", orderType)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub InsertAuthorization(HeaderId As String, ItemId As String, Desc As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderAuthorizations VALUES (NEWID(), @HeaderId, @ItemId, @Description, NULL, NULL, 1)")
                    myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                    myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                    myCmd.Parameters.AddWithValue("@Description", Desc)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Function RollerOtorisasi(headerId As String, itemId As String) As Boolean
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM OrderDetails WHERE Id='" + itemId + "' AND Active=1")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim productId As String = thisData.Tables(0).Rows(0).Item("ProductId").ToString()
                Dim tubeType As String = GetItemData("SELECT TubeType FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")
                Dim controlType As String = GetItemData("SELECT ControlType FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")

                Dim blindId As String = GetItemData("SELECT BlindId FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")
                Dim blindName As String = GetItemData("SELECT Name FROM Blinds WHERE Id = '" + UCase(blindId).ToString() + "'")

                Dim width As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("Width") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("Width")))
                Dim widthB As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("WidthB") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("WidthB")))
                Dim widthC As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("WidthC") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("WidthC")))
                Dim widthD As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("WidthD") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("WidthD")))
                Dim widthE As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("WidthE") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("WidthE")))
                Dim widthF As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("WidthF") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("WidthF")))
                Dim drop As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("Drop") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("Drop")))

                Dim fabricId As String = thisData.Tables(0).Rows(0).Item("FabricId").ToString()
                Dim fabricIdB As String = thisData.Tables(0).Rows(0).Item("FabricIdB").ToString()
                Dim fabricIdC As String = thisData.Tables(0).Rows(0).Item("FabricIdC").ToString()
                Dim fabricIdD As String = thisData.Tables(0).Rows(0).Item("FabricIdD").ToString()
                Dim fabricIdE As String = thisData.Tables(0).Rows(0).Item("FabricIdE").ToString()
                Dim fabricIdF As String = thisData.Tables(0).Rows(0).Item("FabricIdF").ToString()

                Dim fabricColourId As String = thisData.Tables(0).Rows(0).Item("FabricColourId").ToString()
                Dim fabricColourIdB As String = thisData.Tables(0).Rows(0).Item("FabricColourIdB").ToString()
                Dim fabricColourIdC As String = thisData.Tables(0).Rows(0).Item("FabricColourIdC").ToString()
                Dim fabricColourIdD As String = thisData.Tables(0).Rows(0).Item("FabricColourIdD").ToString()
                Dim fabricColourIdE As String = thisData.Tables(0).Rows(0).Item("FabricColourIdE").ToString()
                Dim fabricColourIdF As String = thisData.Tables(0).Rows(0).Item("FabricColourIdF").ToString()

                Dim springAssist As String = thisData.Tables(0).Rows(0).Item("SpringAssist").ToString()

                Dim fabricWeight As String = GetFabricWeight(fabricId)
                Dim springType As String = GetGridSpring(tubeType, fabricWeight, width)

                Dim tubeDesc As String = " [ " & tubeType & " ]"
                Dim controlDesc As String = " [ " & controlType & " ]"

                Dim minWidth As String = String.Empty
                Dim maxWidth As String = String.Empty
                Dim minDrop As String = String.Empty
                Dim maxDrop As String = String.Empty

                Dim minWidthB As String = String.Empty
                Dim maxWidthB As String = String.Empty
                Dim minDropB As String = String.Empty
                Dim maxDropB As String = String.Empty

                Dim minWidthC As String = String.Empty
                Dim maxWidthC As String = String.Empty
                Dim minDropC As String = String.Empty
                Dim maxDropC As String = String.Empty

                Dim minWidthD As String = String.Empty
                Dim maxWidthD As String = String.Empty
                Dim minDropD As String = String.Empty
                Dim maxDropD As String = String.Empty

                Dim minWidthE As String = String.Empty
                Dim maxWidthE As String = String.Empty
                Dim minDropE As String = String.Empty
                Dim maxDropE As String = String.Empty

                Dim minWidthF As String = String.Empty
                Dim maxWidthF As String = String.Empty
                Dim minDropF As String = String.Empty
                Dim maxDropF As String = String.Empty

                'FIRST - MIN WIDTH
                If width < 360 Then minWidth = "Minimum width is 360 mm"
                If width < 550 And controlType = "Alpha 1Nm WF" Then minWidth = "Minimum width is 550 mm" & controlDesc
                If width < 650 And (InStr(controlType, "Alpha") > 0 Or controlType = "Altus" Or controlType = "Sonesse") Then minWidth = "Minimum width is 650 mm" & controlDesc
                If width < 800 And tubeType = "Acmeda" Then minWidth = "Minimum width is 800 mm" & tubeDesc
                If controlType = "Chain" And tubeType.Contains("Sunboss") And springAssist = "Yes" Then
                    If width < 720 And springType.Contains("LD") Then minWidth = "Minimum width is 720 mm" & tubeDesc & controlDesc
                    If width < 995 And springType.Contains("MD") Then minWidth = "Minimum width is 995 mm" & tubeDesc & controlDesc
                    If width < 1055 And springType.Contains("HD") Then minWidth = "Minimum width is 1055 mm" & tubeDesc & controlDesc
                End If

                If Not String.IsNullOrEmpty(minWidth) Then
                    If blindName = "Double Bracket" Or blindName.Contains("Link") Or blindName.Contains("DB Link") Then
                        minWidth = "1st" & " | " & minWidth
                    End If
                    InsertAuthorization(headerId, itemId, minWidth)
                End If

                'FIRST - MAX WIDTH
                If width > 3610 Then maxWidth = "Maximum width is 3610 mm" & tubeDesc
                If width > 3350 And tubeType = "Sunboss 63mm" Then maxWidth = "Maximum width is 3350 mm" & tubeDesc
                If width > 3220 And tubeType = "Acmeda" And fabricWeight <= 400 Then maxWidth = "Maximum width is 3220 mm" & tubeDesc
                If width > 3050 And tubeType = "Sunboss 50mm" Then maxWidth = "Maximum width is 3050 mm" & tubeDesc
                If width > 3000 And (controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxWidth = "Maximum width is 3000 mm" & tubeDesc & controlDesc
                If width > 3000 And (controlType = "Altus" Or controlType = "Sonesse") Then maxWidth = "Maximum width is 3000 mm" & controlDesc
                If width > 2920 And tubeType = "Acmeda" And fabricWeight > 400 Then maxWidth = "Maximum width is 2920 mm" & tubeDesc
                If width > 2700 And tubeType = "Sunboss 43mm" Then maxWidth = "Maximum width is 2700 mm" & tubeDesc
                If width > 2700 And controlType = "Alpha 2Nm WF" And tubeType = "Sunboss 50mm" Then maxWidth = "Maximum width is 2700 mm" & tubeDesc & controlDesc
                If width > 2000 And controlType = "Alpha 1Nm WF" Then maxWidth = "Maximum width is 2000 mm" & controlDesc

                If Not String.IsNullOrEmpty(maxWidth) Then
                    If blindName = "Double Bracket" Or blindName.Contains("Link") Or blindName.Contains("DB Link") Then
                        maxWidth = "1st" & " | " & maxWidth
                    End If
                    InsertAuthorization(headerId, itemId, maxWidth)
                End If

                'FIRST - MIN DROP
                If drop < 500 Then minDrop = "Minimum drop is 500 mm"
                If Not String.IsNullOrEmpty(minDrop) Then
                    If blindName = "Double Bracket" Or blindName.Contains("Link") Or blindName.Contains("DB Link") Then
                        minDrop = "1st Blind" & " | " & minDrop
                    End If
                    InsertAuthorization(headerId, itemId, minDrop)
                End If

                'FIRST - MAX DROP
                If drop > 5500 And tubeType = "Sunboss 63mm" Then maxDrop = "Maximum drop is 5500 mm" & tubeDesc
                If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxDrop = "Maximum drop is 5000 mm" & tubeDesc & controlDesc
                If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 43mm" Then maxDrop = "Maximum drop is 5000 mm" & tubeDesc & controlDesc
                If drop > 4000 And controlType = "Chain" And tubeType = "Sunboss 50mm" Then maxDrop = "Maximum drop is 5000 mm" & tubeDesc & controlDesc
                If drop > 3000 Then maxDrop = "Maximum drop is 3000 mm"
                If drop > 3000 And controlType = "Alpha 2Nm WF" And (tubeType = "Sunboss 43mm" Or tubeType = "Sunboss 50mm") Then maxDrop = "Maximum drop is 3000 mm" & tubeDesc & controlDesc
                If drop > 2000 And controlType = "Alpha 1Nm WF" Then maxDrop = "Maximum drop is 2000 mm" & controlDesc

                If Not String.IsNullOrEmpty(maxDrop) Then
                    If blindName = "Double Bracket" Or blindName.Contains("Link") Or blindName.Contains("DB Link") Then
                        maxDrop = "1st Blind" & " | " & maxDrop
                    End If
                    InsertAuthorization(headerId, itemId, maxDrop)
                End If

                Dim railRoad As Boolean = GetFabricRailRoad(fabricId)
                Dim fabricWidth As Integer = GetFabricWidth(fabricColourId)
                If railRoad = False And width > fabricWidth Then InsertAuthorization(headerId, itemId, "Fabric needs to be railroaded.")

                ' SECOND BLIND
                If blindName = "Double Bracket" Or blindName = "Link 2 Blinds Dep" Or blindName = "Link 2 Blinds Ind" Or blindName = "Link 2 Blinds Head to Tail" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "Link 3 Blinds Dep" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "Link 4 Blinds Ind with Dep" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                    Dim fabricWeightB As String = GetFabricWeight(fabricIdB)
                    Dim springTypeB As String = GetGridSpring(tubeType, fabricWeightB, widthB)

                    'SECOND - MIN WIDTH
                    If widthB < 360 Then minWidthB = "2nd blind | Minimum width is 360 mm"
                    If widthB < 550 And controlType = "Alpha 1Nm WF" Then minWidthB = "2nd blind | Minimum width is 550mm" & controlDesc
                    If widthB < 650 And (InStr(controlType, "Alpha") > 0 Or controlType = "Altus" Or controlType = "Sonesse") Then minWidthB = "2nd blind | Minimum width is 650 mm" & controlDesc
                    If widthB < 800 And tubeType = "Acmeda" Then minWidthB = "2nd blind | Minimum width is 800 mm" & tubeDesc
                    If controlType = "Chain" And tubeType.Contains("Sunboss") And springAssist = "Yes" Then
                        If widthB < 720 And springTypeB.Contains("LD") Then minWidthB = "2nd blind | Minimum width is 720 mm" & tubeDesc
                        If widthB < 995 And springTypeB.Contains("MD") Then minWidthB = "2nd blind | Minimum width is 995 mm" & tubeDesc
                        If widthB < 1055 And springTypeB.Contains("HD") Then minWidthB = "2nd blind | Minimum width is 1055 mm" & tubeDesc
                    End If

                    If Not String.IsNullOrEmpty(minWidthB) Then InsertAuthorization(headerId, itemId, minWidthB)

                    'SECOND - MAX WIDTH
                    If widthB > 3610 Then maxWidthB = "2nd blind | Maximum width is 3610 mm"
                    If widthB > 3350 And tubeType = "Sunboss 63mm" Then maxWidthB = "2nd blind | Maximum width is 3350 mm" & tubeDesc
                    If widthB > 3220 And tubeType = "Acmeda" And fabricWeight <= 400 Then maxWidthB = "2nd blind | Maximum width is 3220 mm" & tubeDesc
                    If widthB > 3050 And tubeType = "Sunboss 50mm" Then maxWidthB = "2nd blind | Maximum width is 3050 mm" & tubeDesc
                    If widthB > 3000 And (controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxWidthB = "2nd blind | Maximum width is 3000 mm" & tubeDesc & controlDesc
                    If widthB > 3000 And (controlType = "Altus" Or controlType = "Sonesse") Then maxWidthB = "2nd blind | Maximum width is 3000 mm" & controlDesc
                    If widthB > 2920 And tubeType = "Acmeda" And fabricWeight > 400 Then maxWidthB = "2nd blind | Maximum width is 2920 mm" & tubeDesc
                    If widthB > 2700 And tubeType = "Sunboss 43mm" Then maxWidthB = "2nd blind | Maximum width is 2700 mm" & tubeDesc
                    If widthB > 2700 And controlType = "Alpha 2Nm WF" And tubeType = "Sunboss 50mm" Then maxWidthB = "2nd blind | Maximum width is 2700 mm" & tubeDesc & controlDesc
                    If widthB > 2000 And controlType = "Alpha 1Nm WF" Then maxWidthB = "2nd blind | Maximum width is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxWidthB) Then InsertAuthorization(headerId, itemId, maxWidthB)

                    'SECOND - MIN DROP
                    If drop < 500 Then minDropB = "2nd blind | Minimum drop is 500 mm"
                    If Not String.IsNullOrEmpty(minDropB) Then InsertAuthorization(headerId, itemId, minDropB)

                    'SECOND - MAX DROP
                    If drop > 5500 And tubeType = "Sunboss 63mm" Then maxDropB = "2nd blind | Maximum drop is 5500 mm" & tubeDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxDropB = "2nd blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 43mm" Then maxDropB = "2nd blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 4000 And controlType = "Chain" And tubeType = "Sunboss 50mm" Then maxDropB = "2nd blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 3000 Then maxDropB = "2nd blind | Maximum drop is 3000 mm"
                    If drop > 3000 And controlType = "Alpha 2Nm WF" And (tubeType = "Sunboss 43mm" Or tubeType = "Sunboss 50mm") Then maxDropB = "2nd blind | Maximum drop is 3000 mm" & tubeDesc & controlDesc
                    If drop > 2000 And controlType = "Alpha 1Nm WF" Then maxDropB = "2nd blind | Maximum drop is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxDropB) Then InsertAuthorization(headerId, itemId, maxDropB)

                    Dim railRoadB As Boolean = GetFabricRailRoad(fabricIdB)
                    Dim fabricWidthB As Integer = GetFabricWidth(fabricColourIdB)
                    If railRoadB = False And widthB > fabricWidthB Then InsertAuthorization(headerId, itemId, "2nd blind | Fabric needs to be railroaded.")
                End If

                'THIRD BLIND
                If blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "Link 3 Blinds Dep" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "Link 4 Blinds Ind with Dep" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                    Dim fabricWeightC As String = GetFabricWeight(fabricIdC)
                    Dim springTypeC As String = GetGridSpring(tubeType, fabricWeightC, widthC)

                    'THIRD - MIN WIDTH
                    If widthC < 360 Then minWidthC = "3th blind | Minimum width is 360 mm"
                    If widthC < 550 And controlType = "Alpha 1Nm WF" Then minWidthC = "3th blind | Minimum width is 550mm" & controlDesc
                    If widthC < 650 And (InStr(controlType, "Alpha") > 0 Or controlType = "Altus" Or controlType = "Sonesse") Then minWidthC = "3th blind | Minimum width is 650 mm" & controlDesc
                    If widthC < 800 And tubeType = "Acmeda" Then minWidthC = "3th blind | Minimum width is 800 mm" & tubeDesc
                    If controlType = "Chain" And tubeType.Contains("Sunboss") And springAssist = "Yes" Then
                        If widthC < 720 And springTypeC.Contains("LD") Then minWidthC = "3th blind | Minimum width is 720 mm" & tubeDesc
                        If widthC < 995 And springTypeC.Contains("MD") Then minWidthC = "3th blind | Minimum width is 995 mm" & tubeDesc
                        If widthC < 1055 And springTypeC.Contains("HD") Then minWidthC = "3th blind | Minimum width is 1055 mm" & tubeDesc
                    End If

                    If Not String.IsNullOrEmpty(minWidthC) Then InsertAuthorization(headerId, itemId, minWidthC)

                    'THIRD - MAX WIDTH
                    If widthC > 3610 Then maxWidthC = "3th blind | Maximum width is 3610 mm"
                    If widthC > 3350 And tubeType = "Sunboss 63mm" Then maxWidthC = "3th blind | Maximum width is 3350 mm" & tubeDesc
                    If widthC > 3220 And tubeType = "Acmeda" And fabricWeightC <= 400 Then maxWidthC = "3th blind | Maximum width is 3220 mm" & tubeDesc
                    If widthC > 3050 And tubeType = "Sunboss 50mm" Then maxWidthC = "3th blind | Maximum width is 3050 mm" & tubeDesc
                    If widthC > 3000 And (controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxWidthC = "3th blind | Maximum width is 3000 mm" & tubeDesc & controlDesc
                    If widthC > 3000 And (controlType = "Altus" Or controlType = "Sonesse") Then maxWidthC = "3th blind | Maximum width is 3000 mm" & controlDesc
                    If widthC > 2920 And tubeType = "Acmeda" And fabricWeightC > 400 Then maxWidthC = "3th blind | Maximum width is 2920 mm" & tubeDesc
                    If widthC > 2700 And tubeType = "Sunboss 43mm" Then maxWidthC = "3th blind | Maximum width is 2700 mm" & tubeDesc
                    If widthC > 2700 And controlType = "Alpha 2Nm WF" And tubeType = "Sunboss 50mm" Then maxWidthC = "3th blind | Maximum width is 2700 mm" & tubeDesc & controlDesc
                    If widthC > 2000 And controlType = "Alpha 1Nm WF" Then maxWidthC = "3th blind | Maximum width is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxWidthC) Then InsertAuthorization(headerId, itemId, maxWidthC)

                    'THIRD - MIN DROP
                    If drop < 500 Then minDropC = "3th blind | Minimum drop is 500 mm"
                    If Not String.IsNullOrEmpty(minDropC) Then InsertAuthorization(headerId, itemId, minDropC)

                    'THIRD - MAX DROP
                    If drop > 5500 And tubeType = "Sunboss 63mm" Then maxDropC = "3th blind | Maximum drop is 5500 mm" & tubeDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxDropC = "3th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 43mm" Then maxDropC = "3th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 4000 And controlType = "Chain" And tubeType = "Sunboss 50mm" Then maxDropC = "3th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 3000 Then maxDropC = "3th blind | Maximum drop is 3000 mm"
                    If drop > 3000 And controlType = "Alpha 2Nm WF" And (tubeType = "Sunboss 43mm" Or tubeType = "Sunboss 50mm") Then maxDropC = "3th blind | Maximum drop is 3000 mm" & tubeDesc & controlDesc
                    If drop > 2000 And controlType = "Alpha 1Nm WF" Then maxDropC = "3th blind | Maximum drop is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxDropC) Then InsertAuthorization(headerId, itemId, maxDropC)

                    Dim railRoadC As Boolean = GetFabricRailRoad(fabricIdC)
                    Dim fabricWidthB As Integer = GetFabricWidth(fabricColourIdC)
                    If railRoadC = False And widthC > fabricWidthB Then InsertAuthorization(headerId, itemId, "3th blind | Fabric needs to be railroaded.")
                End If

                'FOURTH BLIND
                If blindName = "Link 4 Blinds Ind with Dep" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                    Dim fabricWeightD As String = GetFabricWeight(fabricIdD)
                    Dim springTypeD As String = GetGridSpring(tubeType, fabricWeightD, widthD)

                    'FOURTH - MIN WIDTH
                    If widthD < 360 Then minWidthD = "4th blind | Minimum width is 360 mm"
                    If widthD < 550 And controlType = "Alpha 1Nm WF" Then minWidthD = "4th blind | Minimum width is 550mm" & controlDesc
                    If widthD < 650 And (InStr(controlType, "Alpha") > 0 Or controlType = "Altus" Or controlType = "Sonesse") Then minWidthD = "4th blind | Minimum width is 650 mm" & controlDesc
                    If widthD < 800 And tubeType = "Acmeda" Then minWidthD = "4th blind | Minimum width is 800 mm" & tubeDesc
                    If controlType = "Chain" And tubeType.Contains("Sunboss") And springAssist = "Yes" Then
                        If widthD < 720 And springTypeD.Contains("LD") Then minWidthD = "4th blind | Minimum width is 720 mm" & tubeDesc
                        If widthD < 995 And springTypeD.Contains("MD") Then minWidthD = "4th blind | Minimum width is 995 mm" & tubeDesc
                        If widthD < 1055 And springTypeD.Contains("HD") Then minWidthD = "4th blind | Minimum width is 1055 mm" & tubeDesc
                    End If

                    If Not String.IsNullOrEmpty(minWidthD) Then InsertAuthorization(headerId, itemId, minWidthD)

                    'FOURTH - MAX WIDTH
                    If widthD > 3610 Then maxWidthD = "4th blind | Maximum width is 3610 mm"
                    If widthD > 3350 And tubeType = "Sunboss 63mm" Then maxWidthD = "4th blind | Maximum width is 3350 mm" & tubeDesc
                    If widthD > 3220 And tubeType = "Acmeda" And fabricWeightD <= 400 Then maxWidthD = "4th blind | Maximum width is 3220 mm" & tubeDesc
                    If widthD > 3050 And tubeType = "Sunboss 50mm" Then maxWidthD = "4th blind | Maximum width is 3050 mm" & tubeDesc
                    If widthD > 3000 And (controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxWidthD = "4th blind | Maximum width is 3000 mm" & tubeDesc & controlDesc
                    If widthD > 3000 And (controlType = "Altus" Or controlType = "Sonesse") Then maxWidthD = "4th blind | Maximum width is 3000 mm" & controlDesc
                    If widthD > 2920 And tubeType = "Acmeda" And fabricWeightD > 400 Then maxWidthD = "4th blind | Maximum width is 2920 mm" & tubeDesc
                    If widthD > 2700 And tubeType = "Sunboss 43mm" Then maxWidthD = "4th blind | Maximum width is 2700 mm" & tubeDesc
                    If widthD > 2700 And controlType = "Alpha 2Nm WF" And tubeType = "Sunboss 50mm" Then maxWidthD = "4th blind | Maximum width is 2700 mm" & tubeDesc & controlDesc
                    If widthD > 2000 And controlType = "Alpha 1Nm WF" Then maxWidthD = "4th blind | Maximum width is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxWidthD) Then InsertAuthorization(headerId, itemId, maxWidthD)

                    'FOURTH - MIN DROP
                    If drop < 500 Then minDropD = "4th blind | Minimum drop is 500 mm"
                    If Not String.IsNullOrEmpty(minDropD) Then InsertAuthorization(headerId, itemId, minDropD)

                    'FOURTH - MAX DROP
                    If drop > 5500 And tubeType = "Sunboss 63mm" Then maxDropD = "4th blind | Maximum drop is 5500 mm" & tubeDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxDropD = "4th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 43mm" Then maxDropD = "4th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 4000 And controlType = "Chain" And tubeType = "Sunboss 50mm" Then maxDropD = "4th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 3000 Then maxDropD = "4th blind | Maximum drop is 3000 mm"
                    If drop > 3000 And controlType = "Alpha 2Nm WF" And (tubeType = "Sunboss 43mm" Or tubeType = "Sunboss 50mm") Then maxDropD = "4th blind | Maximum drop is 3000 mm" & tubeDesc & controlDesc
                    If drop > 2000 And controlType = "Alpha 1Nm WF" Then maxDropD = "4th blind | Maximum drop is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxDropD) Then InsertAuthorization(headerId, itemId, maxDropD)

                    Dim railRoadD As Boolean = GetFabricRailRoad(fabricIdD)
                    Dim fabricWidthD As Integer = GetFabricWidth(fabricColourIdD)
                    If railRoadD = False And widthD > fabricWidthD Then InsertAuthorization(headerId, itemId, "4th blind | Fabric needs to be railroaded.")
                End If

                'FIFTH BLIND
                If blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                    Dim fabricWeightE As String = GetFabricWeight(fabricIdE)
                    Dim springTypeE As String = GetGridSpring(tubeType, fabricWeightE, widthE)

                    'FIFTH - MIN WIDTH
                    If widthE < 360 Then minWidthE = "5th blind | Minimum width is 360 mm"
                    If widthE < 550 And controlType = "Alpha 1Nm WF" Then minWidthE = "5th blind | Minimum width is 550mm" & controlDesc
                    If widthE < 650 And (InStr(controlType, "Alpha") > 0 Or controlType = "Altus" Or controlType = "Sonesse") Then minWidthE = "5th blind | Minimum width is 650 mm" & controlDesc
                    If widthE < 800 And tubeType = "Acmeda" Then minWidthE = "5th blind | Minimum width is 800 mm" & tubeDesc
                    If controlType = "Chain" And tubeType.Contains("Sunboss") And springAssist = "Yes" Then
                        If widthE < 720 And springTypeE.Contains("LD") Then minWidthE = "5th blind | Minimum width is 720 mm" & tubeDesc
                        If widthE < 995 And springTypeE.Contains("MD") Then minWidthE = "5th blind | Minimum width is 995 mm" & tubeDesc
                        If widthE < 1055 And springTypeE.Contains("HD") Then minWidthE = "5th blind | Minimum width is 1055 mm" & tubeDesc
                    End If

                    If Not String.IsNullOrEmpty(minWidthE) Then InsertAuthorization(headerId, itemId, minWidthE)

                    'FIFTH - MAX WIDTH
                    If widthE > 3610 Then maxWidthE = "5th blind | Maximum width is 3610 mm"
                    If widthE > 3350 And tubeType = "Sunboss 63mm" Then maxWidthE = "5th blind | Maximum width is 3350 mm" & tubeDesc
                    If widthE > 3220 And tubeType = "Acmeda" And fabricWeightE <= 400 Then maxWidthE = "5th blind | Maximum width is 3220 mm" & tubeDesc
                    If widthE > 3050 And tubeType = "Sunboss 50mm" Then maxWidthE = "5th blind | Maximum width is 3050 mm" & tubeDesc
                    If widthE > 3000 And (controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxWidthE = "5th blind | Maximum width is 3000 mm" & tubeDesc & controlDesc
                    If widthE > 3000 And (controlType = "Altus" Or controlType = "Sonesse") Then maxWidthE = "5th blind | Maximum width is 3000 mm" & controlDesc
                    If widthE > 2920 And tubeType = "Acmeda" And fabricWeightE > 400 Then maxWidthE = "5th blind | Maximum width is 2920 mm" & tubeDesc
                    If widthE > 2700 And tubeType = "Sunboss 43mm" Then maxWidthE = "5th blind | Maximum width is 2700 mm" & tubeDesc
                    If widthE > 2700 And controlType = "Alpha 2Nm WF" And tubeType = "Sunboss 50mm" Then maxWidthE = "5th blind | Maximum width is 2700 mm" & tubeDesc & controlDesc
                    If widthE > 2000 And controlType = "Alpha 1Nm WF" Then maxWidthE = "5th blind | Maximum width is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxWidthE) Then InsertAuthorization(headerId, itemId, maxWidthE)

                    'FIFTH - MIN DROP
                    If drop < 500 Then minDropE = "5th blind | Minimum drop is 500 mm"
                    If Not String.IsNullOrEmpty(minDropE) Then InsertAuthorization(headerId, itemId, minDropE)

                    'FIFTH - MAX DROP
                    If drop > 5500 And tubeType = "Sunboss 63mm" Then maxDropE = "5th blind | Maximum drop is 5500 mm" & tubeDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxDropE = "5th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 43mm" Then maxDropE = "5th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 4000 And controlType = "Chain" And tubeType = "Sunboss 50mm" Then maxDropE = "5th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 3000 Then maxDropE = "5th blind | Maximum drop is 3000 mm"
                    If drop > 3000 And controlType = "Alpha 2Nm WF" And (tubeType = "Sunboss 43mm" Or tubeType = "Sunboss 50mm") Then maxDropE = "5th blind | Maximum drop is 3000 mm" & tubeDesc & controlDesc
                    If drop > 2000 And controlType = "Alpha 1Nm WF" Then maxDropE = "5th blind | Maximum drop is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxDropE) Then InsertAuthorization(headerId, itemId, maxDropE)

                    Dim railRoadE As Boolean = GetFabricRailRoad(fabricIdE)
                    Dim fabricWidthE As Integer = GetFabricWidth(fabricColourIdE)
                    If railRoadE = False And widthE > fabricWidthE Then InsertAuthorization(headerId, itemId, "5th blind | Fabric needs to be railroaded.")
                End If

                'SIXTH BLIND
                If blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                    Dim fabricWeightF As String = GetFabricWeight(fabricIdF)
                    Dim springTypeF As String = GetGridSpring(tubeType, fabricWeightF, widthF)

                    'SIXTH - MIN WIDTH
                    If widthF < 360 Then minWidthF = "6th blind | Minimum width is 360 mm"
                    If widthF < 550 And controlType = "Alpha 1Nm WF" Then minWidthF = "6th blind | Minimum width is 550mm" & controlDesc
                    If widthF < 650 And (InStr(controlType, "Alpha") > 0 Or controlType = "Altus" Or controlType = "Sonesse") Then minWidthF = "6th blind | Minimum width is 650 mm" & controlDesc
                    If widthF < 800 And tubeType = "Acmeda" Then minWidthF = "6th blind | Minimum width is 800 mm" & tubeDesc
                    If controlType = "Chain" And tubeType.Contains("Sunboss") And springAssist = "Yes" Then
                        If widthF < 720 And springTypeF.Contains("LD") Then minWidthF = "6th blind | Minimum width is 720 mm" & tubeDesc
                        If widthF < 995 And springTypeF.Contains("MD") Then minWidthF = "6th blind | Minimum width is 995 mm" & tubeDesc
                        If widthF < 1055 And springTypeF.Contains("HD") Then minWidthF = "6th blind | Minimum width is 1055 mm" & tubeDesc
                    End If

                    If Not String.IsNullOrEmpty(minWidthF) Then InsertAuthorization(headerId, itemId, minWidthF)

                    'SIXTH - MAX WIDTH

                    If widthF > 3610 Then maxWidthF = "6th blind | Maximum width is 3610 mm"
                    If widthF > 3350 And tubeType = "Sunboss 63mm" Then maxWidthF = "6th blind | Maximum width is 3350 mm" & tubeDesc
                    If widthF > 3220 And tubeType = "Acmeda" And fabricWeightF <= 400 Then maxWidthF = "6th blind | Maximum width is 3220 mm" & tubeDesc
                    If widthF > 3050 And tubeType = "Sunboss 50mm" Then maxWidthF = "6th blind | Maximum width is 3050 mm" & tubeDesc
                    If widthF > 3000 And (controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxWidthF = "6th blind | Maximum width is 3000 mm" & tubeDesc & controlDesc
                    If widthF > 3000 And (controlType = "Altus" Or controlType = "Sonesse") Then maxWidthF = "6th blind | Maximum width is 3000 mm" & controlDesc
                    If widthF > 2920 And tubeType = "Acmeda" And fabricWeightF > 400 Then maxWidthF = "6th blind | Maximum width is 2920 mm" & tubeDesc
                    If widthF > 2700 And tubeType = "Sunboss 43mm" Then maxWidthF = "6th blind | Maximum width is 2700 mm" & tubeDesc
                    If widthF > 2700 And controlType = "Alpha 2Nm WF" And tubeType = "Sunboss 50mm" Then maxWidthF = "6th blind | Maximum width is 2700 mm" & tubeDesc & controlDesc
                    If widthF > 2000 And controlType = "Alpha 1Nm WF" Then maxWidthF = "6th blind | Maximum width is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxWidthF) Then InsertAuthorization(headerId, itemId, maxWidthF)

                    'SIXTH - MIN DROP
                    If drop < 500 Then minDropF = "6th blind | Minimum drop is 500 mm"
                    If Not String.IsNullOrEmpty(minDropF) Then InsertAuthorization(headerId, itemId, minDropF)

                    'SIXTH - MAX DROP
                    If drop > 5500 And tubeType = "Sunboss 63mm" Then maxDropF = "6th blind | Maximum drop is 5500 mm" & tubeDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 50mm" Then maxDropF = "6th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 5000 And (controlType.Contains("Alpha") Or controlType = "Altus" Or controlType = "Sonesse") And tubeType = "Sunboss 43mm" Then maxDropF = "6th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 4000 And controlType = "Chain" And tubeType = "Sunboss 50mm" Then maxDropF = "6th blind | Maximum drop is 5000 mm" & tubeDesc & controlDesc
                    If drop > 3000 Then maxDropF = "6th blind | Maximum drop is 3000 mm"
                    If drop > 3000 And controlType = "Alpha 2Nm WF" And (tubeType = "Sunboss 43mm" Or tubeType = "Sunboss 50mm") Then maxDropF = "6th blind | Maximum drop is 3000 mm" & tubeDesc & controlDesc
                    If drop > 2000 And controlType = "Alpha 1Nm WF" Then maxDropF = "6th blind | Maximum drop is 2000 mm" & controlDesc

                    If Not String.IsNullOrEmpty(maxDropF) Then InsertAuthorization(headerId, itemId, maxDropF)

                    Dim railRoadF As Boolean = GetFabricRailRoad(fabricIdF)
                    Dim fabricWidthF As Integer = GetFabricWidth(fabricColourIdF)
                    If railRoadF = False And widthF > fabricWidthF Then InsertAuthorization(headerId, itemId, "6th blind | Fabric needs to be railroaded.")
                End If

                If Not (String.IsNullOrEmpty(minWidth) AndAlso String.IsNullOrEmpty(maxWidth) AndAlso String.IsNullOrEmpty(minDrop) AndAlso String.IsNullOrEmpty(maxDrop) AndAlso String.IsNullOrEmpty(minWidthB) AndAlso String.IsNullOrEmpty(maxWidthB) AndAlso String.IsNullOrEmpty(minDropB) AndAlso String.IsNullOrEmpty(maxDropB) AndAlso String.IsNullOrEmpty(minWidthC) AndAlso String.IsNullOrEmpty(maxWidthC) AndAlso String.IsNullOrEmpty(minDropC) AndAlso String.IsNullOrEmpty(maxDropC) AndAlso String.IsNullOrEmpty(minWidthD) AndAlso String.IsNullOrEmpty(maxWidthD) AndAlso String.IsNullOrEmpty(minDropD) AndAlso String.IsNullOrEmpty(maxDropD) AndAlso String.IsNullOrEmpty(minWidthE) AndAlso String.IsNullOrEmpty(maxWidthE) AndAlso String.IsNullOrEmpty(minDropE) AndAlso String.IsNullOrEmpty(maxDropE) AndAlso String.IsNullOrEmpty(minWidthF) AndAlso String.IsNullOrEmpty(maxWidthF) AndAlso String.IsNullOrEmpty(minDropF) AndAlso String.IsNullOrEmpty(maxDropF)) Then
                    Return True
                End If
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
        Return False
    End Function

    Public Function RomanOtorisasi(HeaderId As String, ItemId As String) As Boolean
        Dim result As Boolean = False
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM OrderDetails WHERE Id='" + ItemId + "' AND Active=1")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim productId As String = thisData.Tables(0).Rows(0).Item("ProductId").ToString()
                Dim controlType As String = GetItemData("SELECT ControlType FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")
                Dim width As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("Width") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("Width")))
                Dim drop As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("Drop") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("Drop")))
                Dim fabricId As String = thisData.Tables(0).Rows(0).Item("FabricId").ToString()
                Dim fabricColourId As String = thisData.Tables(0).Rows(0).Item("FabricColourId").ToString()

                Dim sqm As Decimal = width * drop / 1000000
                Dim railRoad As Boolean = GetItemData("SELECT NoRailRoad FROM Fabrics WHERE Id = '" + fabricId + "'")
                Dim fabricWidth As Integer = GetItemData("SELECT Width FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                Dim otorisasi As String = String.Empty

                If sqm > 4.5 Then
                    otorisasi = "Yes"
                    InsertAuthorization(HeaderId, ItemId, "Maximum square metre is 4.5 [" & controlType & "]")
                End If
                If railRoad = False And width > fabricWidth Then
                    otorisasi = "Yes"
                    InsertAuthorization(HeaderId, ItemId, "Fabric needs to be railroaded.")
                End If

                If Not String.IsNullOrEmpty(otorisasi) Then
                    result = True
                End If
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function SkinOtorisasi(HeaderId As String, ItemId As String) As Boolean
        Dim result As Boolean = False
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM OrderDetails WHERE Id='" + ItemId + "' AND Active=1")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim width As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("Width") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("Width")))
                Dim drop As Integer = Convert.ToInt32(If(thisData.Tables(0).Rows(0).Item("Drop") Is DBNull.Value, 0, thisData.Tables(0).Rows(0).Item("Drop")))

                Dim otorisasi As String = String.Empty

                If width < 200 Or width > 3010 Then
                    otorisasi = "Yes"
                    InsertAuthorization(HeaderId, ItemId, "Width must be between 200 and 3010")
                End If
                If drop > 3000 Then
                    otorisasi = "Yes"
                    InsertAuthorization(HeaderId, ItemId, "Maximum drop is 3000")
                End If

                If Not String.IsNullOrEmpty(otorisasi) Then
                    result = True
                End If
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    ' PRICING
    Public Function BuyPrice(headerId As String, itemId As String) As Decimal
        Dim result As Decimal = 0.00
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM OrderDetails WHERE Id='" + itemId + "' AND Active=1")
            If Not thisData.Tables(0).Rows.Count = 0 Then

            End If
        Catch ex As Exception
            result = 10000.0
        End Try
        Return result
    End Function

    Public Function GetGridBuy(group As String, drop As Integer, width As Integer) As Decimal
        Dim result As Decimal = 0.00
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(String.Format("SELECT TOP 1 [Cost] FROM BuyMatrixs WHERE [PriceGroupId] = '{0}' AND [Drop] >= '{1}' AND Width >= '{2}' AND [Cost] >= 0 ORDER BY [Drop], Width, [Cost] ASC", UCase(group).ToString(), drop, width), thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        If rdResult.HasRows Then
                            While rdResult.Read
                                result = rdResult.Item(0)
                            End While
                        Else
                            result = 100000
                        End If
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Function BuySurcharge(headerId As String, itemId As String, number As String) As Decimal
        Dim result As Decimal = 0.00
        Try

        Catch ex As Exception
            result = 10000
        End Try
        Return result
    End Function

    Public Sub UpdateBuyPrice(ItemId As String, Cost As Decimal)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET BuyPrice=@BuyPrice WHERE Id=@ItemId")
                myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                myCmd.Parameters.AddWithValue("@BuyPrice", Cost)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    ' SELL PRICE

    Public Function CountCost(headerId As String, itemId As String) As Decimal
        Dim result As Decimal = 0.00
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM OrderDetails WHERE Id='" + itemId + "' AND Active=1")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim customerId As String = GetItemData("SELECT CustomerId FROM OrderHeaders WHERE Id = '" + headerId + "'")
                Dim customerGroup As String = GetItemData("SELECT Customers.[Group] FROM Customers INNER JOIN OrderHeaders ON Customers.Id = OrderHeaders.CustomerId WHERE OrderHeaders.Id='" + headerId + "'")
                Dim customerPriceGroup As String = GetItemData("SELECT Customers.Pricing FROM Customers INNER JOIN OrderHeaders ON Customers.Id = OrderHeaders.CustomerId WHERE OrderHeaders.Id='" + headerId + "'")

                Dim productId As String = thisData.Tables(0).Rows(0).Item("ProductId").ToString()
                Dim designId As String = GetItemData("SELECT DesignId FROM Products WHERE Id = '" + productId + "'")
                Dim blindId As String = GetItemData("SELECT BlindId FROM Products WHERE Id = '" + productId + "'")

                Dim designName As String = GetItemData("SELECT Name FROM Designs WHERE Id='" + designId + "'")
                Dim blindName As String = GetItemData("SELECT Name FROM Blinds WHERE Id = '" + blindId + "'")
                Dim productName As String = GetItemData("SELECT Name FROM Products WHERE Id = '" + productId + "'")
                Dim tubeType As String = GetItemData("SELECT TubeType FROM Products WHERE Id = '" + productId + "'")
                Dim controlType As String = GetItemData("SELECT ControlType FROM Products WHERE Id = '" + productId + "'")

                Dim priceGroupId As String = thisData.Tables(0).Rows(0).Item("ProductPriceGroupId").ToString()
                Dim priceGroupIdB As String = thisData.Tables(0).Rows(0).Item("ProductPriceGroupIdB").ToString()
                Dim priceGroupIdC As String = thisData.Tables(0).Rows(0).Item("ProductPriceGroupIdC").ToString()
                Dim priceGroupIdD As String = thisData.Tables(0).Rows(0).Item("ProductPriceGroupIdD").ToString()
                Dim priceGroupIdE As String = thisData.Tables(0).Rows(0).Item("ProductPriceGroupIdE").ToString()
                Dim priceGroupIdF As String = thisData.Tables(0).Rows(0).Item("ProductPriceGroupIdF").ToString()

                Dim qty As Integer = thisData.Tables(0).Rows(0).Item("Qty")
                Dim qtyBlade As String = thisData.Tables(0).Rows(0).Item("QtyBlade").ToString()
                Dim partLength As String = thisData.Tables(0).Rows(0).Item("PartLength").ToString()
                Dim partCategory As String = thisData.Tables(0).Rows(0).Item("PartCategory").ToString()
                Dim partComponent As String = thisData.Tables(0).Rows(0).Item("PartComponent").ToString()

                Dim width As String = thisData.Tables(0).Rows(0).Item("Width").ToString()
                Dim widthB As String = thisData.Tables(0).Rows(0).Item("WidthB").ToString()
                Dim widthC As String = thisData.Tables(0).Rows(0).Item("widthC").ToString()
                Dim widthD As String = thisData.Tables(0).Rows(0).Item("WidthD").ToString()
                Dim widthE As String = thisData.Tables(0).Rows(0).Item("WidthE").ToString()
                Dim widthF As String = thisData.Tables(0).Rows(0).Item("WidthF").ToString()

                Dim drop As String = thisData.Tables(0).Rows(0).Item("Drop").ToString()
                Dim dropB As String = thisData.Tables(0).Rows(0).Item("DropB").ToString()
                Dim dropC As String = thisData.Tables(0).Rows(0).Item("DropC").ToString()
                Dim dropD As String = thisData.Tables(0).Rows(0).Item("DropD").ToString()
                Dim dropE As String = thisData.Tables(0).Rows(0).Item("DropE").ToString()
                Dim dropF As String = thisData.Tables(0).Rows(0).Item("DropF").ToString()

                Dim widthExecute As String = width
                Dim widthExecuteB As String = widthB
                Dim widthExecuteC As String = widthC
                Dim widthExecuteD As String = widthD
                Dim widthExecuteE As String = widthE
                Dim widthExecuteF As String = widthF

                Dim dropExecute As String = drop
                Dim dropExecuteB As String = dropB
                Dim dropExecuteC As String = dropC
                Dim dropExecuteD As String = dropD
                Dim dropExecuteE As String = dropE
                Dim dropExecuteF As String = dropF

                Dim priceGroupName As String = GetProductPriceName(priceGroupId)
                Dim priceGroupNameB As String = GetProductPriceName(priceGroupIdB)
                Dim priceGroupNameC As String = GetProductPriceName(priceGroupIdC)
                Dim priceGroupNameD As String = GetProductPriceName(priceGroupIdD)
                Dim priceGroupNameE As String = GetProductPriceName(priceGroupIdE)
                Dim priceGroupNameF As String = GetProductPriceName(priceGroupIdF)

                Dim fabricId As String = thisData.Tables(0).Rows(0).Item("FabricId").ToString()
                Dim fabricIdB As String = thisData.Tables(0).Rows(0).Item("FabricIdB").ToString()
                Dim fabricIdC As String = thisData.Tables(0).Rows(0).Item("FabricIdC").ToString()
                Dim fabricIdD As String = thisData.Tables(0).Rows(0).Item("FabricIdD").ToString()
                Dim fabricIdE As String = thisData.Tables(0).Rows(0).Item("FabricIdE").ToString()
                Dim fabricIdF As String = thisData.Tables(0).Rows(0).Item("FabricIdF").ToString()

                Dim fabricColourId As String = thisData.Tables(0).Rows(0).Item("FabricColourId").ToString()
                Dim fabricColourIdB As String = thisData.Tables(0).Rows(0).Item("FabricColourIdB").ToString()
                Dim fabricColourIdC As String = thisData.Tables(0).Rows(0).Item("FabricColourIdC").ToString()
                Dim fabricColourIdD As String = thisData.Tables(0).Rows(0).Item("FabricColourIdD").ToString()
                Dim fabricColourIdE As String = thisData.Tables(0).Rows(0).Item("FabricColourIdE").ToString()
                Dim fabricColourIdF As String = thisData.Tables(0).Rows(0).Item("FabricColourIdF").ToString()

                Dim fabricWeight As String = GetFabricWeight(fabricId)
                'Dim springType As String = GetGridSpring(tubeType, fabricWeight, width)

                If designName = "Vertical" And blindName = "Blades Only" Then widthExecute = qtyBlade
                If designName = "Additional" Then widthExecute = 0 : dropExecute = 0
                If designName = "Curtain" Then dropExecute = 0
                If designName = "Evolve Parts" Or designName = "Evolve Parts" Or designName = "Evolve Shutters" Or designName = "Panorama PVC Shutters" Or designName = "Panorama PVC Parts" Then widthExecute = 0 : dropExecute = 0

                Dim descriptionPrice As String = String.Empty

                Dim getNow As String = DateTime.Parse(Now).ToString("yyyy-MM-dd")
                Dim priceDetail As Object() = {}

                'FIRST BLIND
                Dim thisCost As Decimal = 0.00
                If Not priceGroupId = "" Then
                    Dim gridMatrix As Decimal = GetGridCost(priceGroupId, dropExecute, widthExecute)
                    If designName = "Roller Blind" Then
                        Dim checkOtorisasi As Boolean = RollerOtorisasi(headerId, itemId)
                        If checkOtorisasi = True Then gridMatrix = 100000
                    End If
                    If designName = "Roman Blind" Then
                        Dim checkOtorisasi As Boolean = RomanOtorisasi(headerId, itemId)
                        If checkOtorisasi = True Then gridMatrix = 100000
                    End If
                    If designName = "Skin Only" Then
                        Dim checkOtorisasi As Boolean = SkinOtorisasi(headerId, itemId)
                        If checkOtorisasi = True Then gridMatrix = 100000
                    End If
                    Dim thisGridMatrix As Decimal = gridMatrix
                    Dim thisSurcharge As Decimal = 0.00
                    Dim thisProductDisc As Decimal = 0.00

                    Dim blindNumber As Integer = 1

                    descriptionPrice = priceGroupName

                    If designName = "Additional" Then
                        If blindName = "Parts" Then
                            Dim addPrice As Decimal = thisData.Tables(0).Rows(0).Item("AddPrice")
                            thisGridMatrix = addPrice
                        End If
                        If blindName = "Travel Charge" And productName = "Misc. Travel Charge" Then
                            Dim addPrice As Decimal = thisData.Tables(0).Rows(0).Item("AddPrice")
                            thisGridMatrix = addPrice
                        End If

                        If blindName = "Freight" Then
                            Dim thisQuery As String = "SELECT SUM(SquareMetre) FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderDetails.HeaderId = '" + headerId + "' AND OrderDetails.Active=1"
                            Dim totalSqm As Decimal = GetItemData_Decimal(thisQuery)

                            If productName = "Curtain" Then
                                Dim addPrice As Decimal = thisData.Tables(0).Rows(0).Item("AddPrice")
                                thisGridMatrix = addPrice
                            End If

                            If productName = "Brisbane - Sydney - Melbourne Areas (Outside)" Then
                                If totalSqm < 1 Then : totalSqm = 1 : End If

                                thisGridMatrix = gridMatrix * totalSqm
                            End If

                            If productName = "TAS - NT - WA" Then
                                Dim addPrice As Decimal = thisData.Tables(0).Rows(0).Item("AddPrice")
                                thisGridMatrix = addPrice * totalSqm
                                If thisGridMatrix < 290 Then : thisGridMatrix = 290.0 : End If
                            End If

                            If productName = "Special - SA" Then
                                thisGridMatrix = totalSqm * 45
                                If totalSqm > 10 Then thisGridMatrix = totalSqm * 35
                                If thisGridMatrix < 290 Then thisGridMatrix = 290
                            End If

                            If productName = "Other" Then
                                Dim addPrice As Decimal = thisData.Tables(0).Rows(0).Item("AddPrice")
                                thisGridMatrix = addPrice * totalSqm
                            End If
                        End If
                    End If

                    If designName = "Panel Glide" And blindName = "Panel Only" Then
                        Dim sqm As Decimal = Math.Round(width * drop / 1000000, 4)
                        thisGridMatrix = gridMatrix * sqm
                    End If

                    If designName = "Skin Only" Then
                        thisGridMatrix = gridMatrix
                        If width > 3010 Or drop > 3010 Then thisGridMatrix = 100000
                    End If

                    If designName = "Evolve Shutters" Then
                        Dim sqm As Decimal = Math.Round(width * drop / 1000000, 4)
                        thisGridMatrix = gridMatrix * sqm
                    End If

                    If designName = "Evolve Parts" Then
                        thisGridMatrix = 0.00
                    End If

                    If designName = "Panorama PVC Shutters" Then
                        Dim sqm As Decimal = Math.Round(width * drop / 1000000, 4)
                        thisGridMatrix = gridMatrix * sqm
                        If sqm < 0.5 Then
                            If customerPriceGroup = "" Or customerPriceGroup = "Standard" Then
                                thisGridMatrix = gridMatrix / 2
                            End If
                            If customerPriceGroup = "$164" Then
                                thisGridMatrix = gridMatrix / 2
                            End If
                            If customerPriceGroup = "$168" Then
                                thisGridMatrix = gridMatrix / 2
                            End If
                            If customerPriceGroup = "$185" Then
                                thisGridMatrix = gridMatrix / 2
                            End If
                            If customerPriceGroup = "$190" Then
                                thisGridMatrix = gridMatrix / 2
                            End If
                            If customerPriceGroup = "B" Then
                                thisGridMatrix = gridMatrix / 2
                            End If
                            descriptionPrice = "Minimum Opening Size Charge (" & priceGroupName & ")"
                        End If

                        If sqm < 0.7 Then
                            If customerPriceGroup = "SISStandard" Then
                                thisGridMatrix = gridMatrix / 2
                            End If
                            If customerPriceGroup = "SISCorporate" Then
                                thisGridMatrix = gridMatrix / 2
                            End If
                            descriptionPrice = "Minimum Opening Size Charge (" & priceGroupName & ")"
                        End If
                    End If

                    If designName = "Panorama PVC Parts" Then
                        Dim lm As Decimal = 0.00

                        If partLength > 0 Then
                            lm = Math.Round(partLength / 1000, 4)
                        End If
                        thisGridMatrix = Math.Round((gridMatrix * lm) * qty, 2)

                        If partComponent = "Standard Louvre Pin" Or partComponent = "Louvre Repair Pin" Or partComponent = "Hinge Spacer" Or partComponent = "Post Mounting Bracket" Or partComponent = "Magnet" Or partComponent = "Standard Striker Plate" Or partComponent = "L Shape Striker Plate" Or partComponent = "Hoffman Key" Or partComponent = "76mm Non-Mortise Hinge" Or partComponent = "76mm Rabbet Hinge" Then
                            thisGridMatrix = gridMatrix * qty
                        End If

                        If partCategory = "Track Hardware" Then
                            thisGridMatrix = gridMatrix * qty
                        End If

                        descriptionPrice = priceGroupName & " " & "(Qty: " & qty & "pcs)"
                    End If

                    If designName = "Roller Blind" Then
                        If blindName = "Double Bracket" Or blindName = "Link 2 Blinds Dep" Or blindName = "Link 3 Blinds Dep" Or blindName = "Link 2 Blinds Ind" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 4 Blinds Ind with Dep" Or blindName = "Link 2 Blinds Head to Tail" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                            descriptionPrice = "#1 " & priceGroupName
                        End If
                    End If

                    UpdateCost(itemId, thisGridMatrix)
                    priceDetail = {headerId, itemId, blindNumber, "Base", descriptionPrice, thisGridMatrix}
                    InsertPriceDetail(priceDetail)

                    thisSurcharge = CountCharge(headerId, itemId, "1")

                    If designName = "Additional" Then
                        thisCost = thisGridMatrix
                    End If

                    If designName = "Aluminium Blind" Then
                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = designName & " " & productDisc & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If
                        thisCost = thisGridMatrix + thisSurcharge - thisProductDisc
                    End If

                    If designName = "Cellular Shades" Or designName = "Curtain" Then
                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricCustom As Integer = 0
                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricId + "' AND (FabricProduct IS NULL OR FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricCustom = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FabricCustom"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                        End If

                        If finalDiscount = 0 Then
                            thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        If finalDiscount = 1 Then
                            thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                            InsertPriceDetail(priceDetail)
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100

                            descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        thisCost = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If

                    If designName = "LS Venetian" Then
                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If
                        thisCost = thisGridMatrix + thisSurcharge - thisProductDisc
                    End If

                    If designName = "Panel Glide" Then
                        thisCost = thisGridMatrix + thisSurcharge
                        If blindName = "Track and Panel" Then
                            Dim thisFabricDisc As Decimal = 0.00
                            Dim thisFabricDiscFinal As Decimal = 0.00

                            Dim fabricDiscount As Decimal = 0.00
                            Dim finalDiscount As Integer = 2

                            Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricId + "' AND (FabricProduct IS NULL OR FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                            If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                                finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                                fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                            End If

                            If finalDiscount = 0 Then
                                thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            If finalDiscount = 1 Then
                                thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                InsertPriceDetail(priceDetail)
                            End If

                            Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                            If productDisc > 0 Then
                                thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                                descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            thisCost = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                        End If
                    End If

                    If designName = "Panorama PVC Shutters" Then
                        thisCost = thisGridMatrix + thisSurcharge
                    End If

                    If designName = "Panorama PVC Parts" Then
                        thisCost = thisGridMatrix + thisSurcharge
                    End If

                    If designName = "Pelmet" Then
                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricId + "' AND (FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                        End If

                        If finalDiscount = 0 Then
                            thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        If finalDiscount = 1 Then
                            thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                            InsertPriceDetail(priceDetail)
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        thisCost = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If

                    If designName = "Roller Blind" Then
                        thisCost = thisGridMatrix + thisSurcharge

                        Dim withouDisc As String() = {"B58C41FF-2A15-4D63-8B42-10085D2BCA71", "F9051919-3022-44A7-A208-CE8F0E5018F6", "6F8C41B5-5F4F-48E4-BFB1-93FEC5974A63", "755C94BD-05FC-4D6E-8150-BF446240FC68"}

                        If Not withouDisc.Contains(UCase(priceGroupId).ToString()) Then
                            Dim thisFabricDisc As Decimal = 0.00
                            Dim thisFabricDiscFinal As Decimal = 0.00

                            Dim fabricDiscount As Decimal = 0.00
                            Dim finalDiscount As Integer = 2
                            Dim fabricCustom As Integer = 0
                            Dim fabricColourDiscount As String = String.Empty

                            Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricId + "' AND (FabricProduct IS NULL OR FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                            If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                                finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                                fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                                fabricCustom = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FabricCustom"))
                                fabricColourDiscount = dataFabricDisc.Tables(0).Rows(0).Item("FabricColourId").ToString()
                            End If

                            If finalDiscount = 0 Then
                                If fabricCustom = 0 Then
                                    thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                    descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    If blindName = "Double Bracket" Or blindName = "Link 2 Blinds Dep" Or blindName = "Link 3 Blinds Dep" Or blindName = "Link 2 Blinds Ind" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 4 Blinds Ind with Dep" Or blindName = "Link 2 Blinds Head to Tail" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                                        descriptionPrice = "#1 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    End If
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                    InsertPriceDetail(priceDetail)
                                End If

                                If fabricCustom = 1 Then
                                    Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                    Dim isFound As Boolean = False

                                    For Each id As String In fabricIdArray
                                        If id.Trim() = fabricColourId.ToString() Then
                                            isFound = True
                                            Exit For
                                        End If
                                    Next

                                    If isFound Then
                                        thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                        descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                        priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                        InsertPriceDetail(priceDetail)
                                    End If
                                End If
                            End If

                            If finalDiscount = 1 Then
                                If fabricCustom = 0 Then
                                    thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                    descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    If blindName = "Double Bracket" Or blindName = "Link 2 Blinds Dep" Or blindName = "Link 3 Blinds Dep" Or blindName = "Link 2 Blinds Ind" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 4 Blinds Ind with Dep" Or blindName = "Link 2 Blinds Head to Tail" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                                        descriptionPrice = "#1 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    End If
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                    InsertPriceDetail(priceDetail)
                                End If

                                If fabricCustom = 1 Then
                                    Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                    Dim isFound As Boolean = False

                                    For Each id As String In fabricIdArray
                                        If id.Trim() = fabricColourId.ToString() Then
                                            isFound = True
                                            Exit For
                                        End If
                                    Next

                                    If isFound Then
                                        thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                        descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                        If blindName = "Double Bracket" Or blindName = "Link 2 Blinds Dep" Or blindName = "Link 3 Blinds Dep" Or blindName = "Link 2 Blinds Ind" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 4 Blinds Ind with Dep" Or blindName = "Link 2 Blinds Head to Tail" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                                            descriptionPrice = "#1 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                        End If
                                        priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                        InsertPriceDetail(priceDetail)
                                    End If
                                End If
                            End If

                            Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                            If productDisc > 0 Then
                                thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                                descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                                If blindName = "Double Bracket" Or blindName = "Link 2 Blinds Dep" Or blindName = "Link 3 Blinds Dep" Or blindName = "Link 2 Blinds Ind" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 4 Blinds Ind with Dep" Or blindName = "Link 2 Blinds Head to Tail" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                                    descriptionPrice = "#1 " & designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                                End If
                                priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            thisCost = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                        End If
                    End If

                    If designName = "Roman Blind" Then
                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricId + "' AND (FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                        End If

                        If finalDiscount = 0 Then
                            thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        If finalDiscount = 1 Then
                            thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                            InsertPriceDetail(priceDetail)
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        thisCost = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If

                    If designName = "Skin Only" Then
                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If
                        thisCost = thisGridMatrix + thisSurcharge - thisProductDisc
                    End If

                    If designName = "Venetian Blind" Then
                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If
                        thisCost = thisGridMatrix + thisSurcharge - thisProductDisc
                    End If

                    If designName = "Veri Shades" Then
                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricId + "' AND (FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                        End If

                        If finalDiscount = 0 Then
                            thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        If finalDiscount = 1 Then
                            thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                            InsertPriceDetail(priceDetail)
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        thisCost = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If

                    If designName = "Vertical" Then
                        thisCost = thisGridMatrix + thisSurcharge
                        If blindName = "Track and Blades" Then
                            Dim thisFabricDisc As Decimal = 0.00
                            Dim thisFabricDiscFinal As Decimal = 0.00

                            Dim fabricDiscount As Decimal = 0.00
                            Dim finalDiscount As Integer = 2

                            Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricId + "' AND (FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                            If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                                finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                                fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                            End If

                            If finalDiscount = 0 Then
                                thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            If finalDiscount = 1 Then
                                thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                InsertPriceDetail(priceDetail)
                            End If

                            Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                            If productDisc > 0 Then
                                thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                                descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            thisCost = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                        End If
                    End If

                    If designName = "Zebra Blind" Then
                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricId + "' AND (FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                        End If

                        If finalDiscount = 0 Then
                            thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        If finalDiscount = 1 Then
                            thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                            descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                            InsertPriceDetail(priceDetail)
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        thisCost = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If
                End If

                'SECOND BLIND
                Dim thisCost2 As Decimal = 0.00
                If Not priceGroupIdB = "" Then
                    If designName = "Roller Blind" And (blindName = "Double Bracket" Or blindName = "Link 2 Blinds Dep" Or blindName = "Link 3 Blinds Dep" Or blindName = "Link 2 Blinds Ind" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 4 Blinds Ind with Dep" Or blindName = "Link 2 Blinds Head to Tail" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Ind with Dep") Then
                        Dim thisSurcharge As Decimal = 0.00
                        Dim thisProductDisc As Decimal = 0.00
                        Dim blindNumber As Integer = 2

                        Dim thisGridMatrix As Decimal = GetGridCost(priceGroupIdB, dropExecuteB, widthExecuteB)

                        Dim checkOtorisasi As Boolean = RollerOtorisasi(headerId, itemId)
                        If checkOtorisasi = True Then thisGridMatrix = 100000

                        descriptionPrice = "#2 " & priceGroupNameB
                        priceDetail = {headerId, itemId, blindNumber, "Base", descriptionPrice, thisGridMatrix}
                        InsertPriceDetail(priceDetail)

                        thisSurcharge = CountCharge(headerId, itemId, "2")

                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2
                        Dim fabricCustom As Integer = 0
                        Dim fabricColourDiscount As String = String.Empty

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricIdB + "' AND (FabricProduct IS NULL OR FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                            fabricCustom = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FabricCustom"))
                            fabricColourDiscount = dataFabricDisc.Tables(0).Rows(0).Item("FabricColourId").ToString()
                        End If

                        If finalDiscount = 0 Then
                            If fabricCustom = 0 Then
                                thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                descriptionPrice = "#2 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdB.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                    descriptionPrice = "#2 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        If finalDiscount = 1 Then
                            If fabricCustom = 0 Then
                                thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                descriptionPrice = "#2 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdB.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                    descriptionPrice = "#2 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix - thisFabricDisc + thisSurcharge) * productDisc / 100
                            descriptionPrice = "#2 " & designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        thisCost2 = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If
                End If

                'THIRD BLIND
                Dim thisCost3 As Decimal = 0.00
                If Not priceGroupIdC = "" Then
                    If designName = "Roller Blind" And (blindName = "Link 3 Blinds Dep" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 4 Blinds Ind with Dep" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Ind with Dep") Then
                        Dim thisSurcharge As Decimal = 0.00
                        Dim thisProductDisc As Decimal = 0.00
                        Dim blindNumber As Integer = 3

                        Dim thisGridMatrix As Decimal = GetGridCost(priceGroupIdC, dropExecuteC, widthExecuteC)
                        Dim checkOtorisasi As Boolean = RollerOtorisasi(headerId, itemId)
                        If checkOtorisasi = True Then thisGridMatrix = 100000

                        descriptionPrice = "#3 " & priceGroupNameC
                        priceDetail = {headerId, itemId, blindNumber, "Base", descriptionPrice, thisGridMatrix}
                        InsertPriceDetail(priceDetail)

                        thisSurcharge = CountCharge(headerId, itemId, "3")

                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2
                        Dim fabricCustom As Integer = 0
                        Dim fabricColourDiscount As String = String.Empty

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricIdC + "' AND (FabricProduct IS NULL OR FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                            fabricCustom = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FabricCustom"))
                            fabricColourDiscount = dataFabricDisc.Tables(0).Rows(0).Item("FabricColourId").ToString()
                        End If

                        If finalDiscount = 0 Then
                            If fabricCustom = 0 Then
                                thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                descriptionPrice = "#3 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdC.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                    descriptionPrice = "#3 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = "#3 " & designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        If finalDiscount = 1 Then
                            If fabricCustom = 0 Then
                                thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                descriptionPrice = "#3 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdC.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDiscFinal = (thisGridMatrix + thisSurcharge - thisProductDisc) * fabricDiscount / 100
                                    descriptionPrice = "#3 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        thisCost3 = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If
                End If

                'FOURTH BLIND
                Dim thisCost4 As Decimal = 0.00
                If Not priceGroupIdD = "" Then
                    If designName = "Roller Blind" And (blindName = "Link 4 Blinds Ind with Dep" Or blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Or blindName = "DB Link 3 Blinds Ind with Dep") Then
                        Dim thisSurcharge As Decimal = 0.00
                        Dim thisProductDisc As Decimal = 0.00
                        Dim blindNumber As Integer = 4

                        Dim thisGridMatrix As Decimal = GetGridCost(priceGroupIdD, dropExecuteD, widthExecuteD)
                        Dim checkOtorisasi As Boolean = RollerOtorisasi(headerId, itemId)
                        If checkOtorisasi = True Then thisGridMatrix = 100000

                        descriptionPrice = "#4 " & priceGroupNameC
                        priceDetail = {headerId, itemId, blindNumber, "Base", descriptionPrice, thisGridMatrix}
                        InsertPriceDetail(priceDetail)

                        thisSurcharge = CountCharge(headerId, itemId, "4")

                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2
                        Dim fabricCustom As Integer = 0
                        Dim fabricColourDiscount As String = String.Empty

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricIdD + "' AND (FabricProduct IS NULL OR FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                            fabricCustom = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FabricCustom"))
                            fabricColourDiscount = dataFabricDisc.Tables(0).Rows(0).Item("FabricColourId").ToString()
                        End If

                        If finalDiscount = 0 Then
                            If fabricCustom = 0 Then
                                thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                descriptionPrice = "#4 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdD.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                    descriptionPrice = "#4 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = "#4 " & designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        If finalDiscount = 1 Then
                            If fabricCustom = 0 Then
                                thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                descriptionPrice = "#4 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdD.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                    descriptionPrice = "#4 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        thisCost4 = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If
                End If

                'FIFTH BLIND
                Dim thisCost5 As Decimal = 0.00
                If Not priceGroupIdE = "" Then
                    If designName = "Roller Blind" And (blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 3 Blinds Ind with Dep") Then
                        Dim thisSurcharge As Decimal = 0.00
                        Dim thisProductDisc As Decimal = 0.00
                        Dim blindNumber As Integer = 5

                        Dim thisGridMatrix As Decimal = GetGridCost(priceGroupIdE, dropExecuteE, widthExecuteE)
                        Dim checkOtorisasi As Boolean = RollerOtorisasi(headerId, itemId)
                        If checkOtorisasi = True Then thisGridMatrix = 100000

                        descriptionPrice = "#5 " & priceGroupNameD
                        priceDetail = {headerId, itemId, blindNumber, "Base", descriptionPrice, thisGridMatrix}
                        InsertPriceDetail(priceDetail)

                        thisSurcharge = CountCharge(headerId, itemId, "5")

                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2
                        Dim fabricCustom As Integer = 0
                        Dim fabricColourDiscount As String = String.Empty

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricIdE + "' AND (FabricProduct IS NULL OR FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                            fabricCustom = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FabricCustom"))
                            fabricColourDiscount = dataFabricDisc.Tables(0).Rows(0).Item("FabricColourId").ToString()
                        End If

                        If finalDiscount = 0 Then
                            If fabricCustom = 0 Then
                                thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                descriptionPrice = "#5 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdE.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                    descriptionPrice = "#5 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = "#5 " & designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        If finalDiscount = 1 Then
                            If fabricCustom = 0 Then
                                thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                descriptionPrice = "#5 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdE.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                    descriptionPrice = "#5 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        thisCost5 = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If
                End If

                'SIXTH BLIND
                Dim thisCost6 As Decimal = 0.00
                If Not priceGroupIdF = "" Then
                    If designName = "Roller Blind" And (blindName = "Link 3 Blinds Head to Tail with Ind" Or blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 3 Blinds Ind with Dep") Then
                        Dim thisSurcharge As Decimal = 0.00
                        Dim thisProductDisc As Decimal = 0.00
                        Dim blindNumber As Integer = 6

                        Dim thisGridMatrix As Decimal = GetGridCost(priceGroupIdF, dropExecuteF, widthExecuteF)
                        Dim checkOtorisasi As Boolean = RollerOtorisasi(headerId, itemId)
                        If checkOtorisasi = True Then thisGridMatrix = 100000

                        descriptionPrice = "#6 " & priceGroupNameF
                        priceDetail = {headerId, itemId, blindNumber, "Base", descriptionPrice, thisGridMatrix}
                        InsertPriceDetail(priceDetail)

                        thisSurcharge = CountCharge(headerId, itemId, "6")

                        Dim thisFabricDisc As Decimal = 0.00
                        Dim thisFabricDiscFinal As Decimal = 0.00

                        Dim fabricDiscount As Decimal = 0.00
                        Dim finalDiscount As Integer = 2
                        Dim fabricCustom As Integer = 0
                        Dim fabricColourDiscount As String = String.Empty

                        Dim dataFabricDisc As DataSet = GetListData("SELECT * FROM CustomerDiscounts WHERE DiscountType = 'Fabric' AND FabricId = '" + fabricIdF + "' AND (FabricProduct IS NULL OR FabricProduct = '' OR FabricProduct LIKE '%" + designId + "%') AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "') AND CONVERT(DATE, StartDate) <= '" + getNow + "' AND CONVERT(DATE, EndDate) >= '" + getNow + "'")
                        If dataFabricDisc.Tables(0).Rows.Count > 0 Then
                            finalDiscount = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FinalDiscount"))
                            fabricDiscount = dataFabricDisc.Tables(0).Rows(0).Item("Discount")
                            fabricCustom = Convert.ToInt32(dataFabricDisc.Tables(0).Rows(0).Item("FabricCustom"))
                            fabricColourDiscount = dataFabricDisc.Tables(0).Rows(0).Item("FabricColourId").ToString()
                        End If

                        If finalDiscount = 0 Then
                            If fabricCustom = 0 Then
                                thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                descriptionPrice = "Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                descriptionPrice = "#6 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdF.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDisc = thisGridMatrix * fabricDiscount / 100
                                    descriptionPrice = "#6 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabric", descriptionPrice, thisFabricDisc}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        Dim productDisc As Decimal = GetItemData_Decimal("SELECT Discount FROM CustomerDiscounts WHERE DiscountType = 'Product' AND DesignId = '" + designId + "' AND Active = 1 AND (CustomerData = '" + customerId + "' OR CustomerData = '" + customerGroup + "')")
                        If productDisc > 0 Then
                            thisProductDisc = (thisGridMatrix + thisSurcharge) * productDisc / 100
                            descriptionPrice = "#6 " & designName & " " & productDisc.ToString("G29", enUS) & "% Discount"
                            priceDetail = {headerId, itemId, blindNumber, "DiscountProduct", descriptionPrice, thisProductDisc}
                            InsertPriceDetail(priceDetail)
                        End If

                        If finalDiscount = 1 Then
                            If fabricCustom = 0 Then
                                thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                descriptionPrice = "#6 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                InsertPriceDetail(priceDetail)
                            End If

                            If fabricCustom = 1 Then
                                Dim fabricIdArray As String() = fabricColourDiscount.Split(","c)
                                Dim isFound As Boolean = False

                                For Each id As String In fabricIdArray
                                    If id.Trim() = fabricColourIdF.ToString() Then
                                        isFound = True
                                        Exit For
                                    End If
                                Next

                                If isFound Then
                                    thisFabricDiscFinal = (thisGridMatrix + thisSurcharge) * fabricDiscount / 100
                                    descriptionPrice = "#6 Fabric Discount " & fabricDiscount.ToString("G29", enUS) & "%"
                                    priceDetail = {headerId, itemId, blindNumber, "DiscountFabricFinal", descriptionPrice, thisFabricDiscFinal}
                                    InsertPriceDetail(priceDetail)
                                End If
                            End If
                        End If

                        thisCost6 = thisGridMatrix - thisFabricDisc + thisSurcharge - thisProductDisc - thisFabricDiscFinal
                    End If
                End If

                result = thisCost + thisCost2 + thisCost3 + thisCost4 + thisCost5 + thisCost6
            End If
        Catch ex As Exception
            result = 100000
            Dim mailCfg As New MailConfig
            mailCfg.MailError("", "", "39BFC30E-915F-4C46-9612-2761D177F09D", ex.ToString())
        End Try
        Return result
    End Function

    Public Function CountCharge(headerId As String, itemId As String, number As String) As Decimal
        Dim result As Decimal = 0.00
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM OrderDetails WHERE Id='" + itemId + "' AND Active=1 ORDER BY Id ASC")
            If thisData.Tables(0).Rows.Count > 0 Then
                Dim productId As String = thisData.Tables(0).Rows(0).Item("ProductId").ToString()
                Dim designId As String = GetItemData("SELECT DesignId FROM Products WHERE Id = '" + productId + "'")
                Dim blindId As String = GetItemData("SELECT BlindId FROM Products WHERE Id = '" + productId + "'")

                Dim surchargeData As DataSet = GetListData("SELECT * FROM PriceSurcharges WHERE DesignId='" + UCase(designId).ToString() + "' AND BlindId='" + UCase(blindId).ToString() + "' AND BlindNumber = '" + number + "' AND Active=1 ORDER BY Id ASC")

                If surchargeData.Tables(0).Rows.Count > 0 Then
                    Dim subCharge As Decimal = 0.00
                    For i As Integer = 0 To surchargeData.Tables(0).Rows.Count - 1
                        Dim id As String = surchargeData.Tables(0).Rows(i).Item("Id").ToString()
                        Dim name As String = surchargeData.Tables(0).Rows(i).Item("Name").ToString()
                        Dim fieldName As String = surchargeData.Tables(0).Rows(i).Item("FieldName").ToString()
                        Dim formula As String = surchargeData.Tables(0).Rows(i).Item("Formula").ToString()
                        Dim charge As String = surchargeData.Tables(0).Rows(i).Item("Charge").ToString()
                        Dim description As String = surchargeData.Tables(0).Rows(i).Item("Description").ToString()
                        Dim blindNumber As String = surchargeData.Tables(0).Rows(i).Item("BlindNumber").ToString()
                        Dim thisCharge As Decimal = 0.00

                        Dim cekFormula As String = GetItemData("SELECT " + fieldName + " FROM viewDetails WHERE Id='" + itemId + "' AND " + formula)
                        If Not cekFormula = "" Then
                            Dim queryCharge As String = "SELECT " + charge + " FROM viewDetails WHERE Id='" + itemId + "'"
                            thisCharge = GetItemData(queryCharge)
                            Dim priceDetail As Object() = {headerId, itemId, blindNumber, "Surcharge", description, thisCharge}
                            InsertPriceDetail(priceDetail)
                        End If
                        subCharge = subCharge + thisCharge
                    Next
                    result = subCharge
                End If
            End If
        Catch ex As Exception
            result = 10000
        End Try
        Return result
    End Function

    Public Function GetGridCost(group As String, drop As Integer, width As Integer) As Decimal
        Dim result As Decimal = 0.00
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(String.Format("SELECT TOP 1 [Cost] FROM PriceMatrixs WHERE [PriceGroupId] = '{0}' AND [Drop] >= '{1}' AND Width >= '{2}' AND [Cost] >= 0 ORDER BY [Drop], Width, [Cost] ASC", UCase(group).ToString(), drop, width), thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        If rdResult.HasRows Then
                            While rdResult.Read
                                result = rdResult.Item(0)
                            End While
                        Else
                            result = 100000
                        End If
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 100000
        End Try
        Return result
    End Function

    Public Sub InsertPriceDetail(data As Object())
        Try
            If data.Length = 6 Then
                Dim headerId As String = Convert.ToString(data(0))
                Dim itemId As String = Convert.ToString(data(1))
                Dim number As Integer = Convert.ToInt32(data(2))
                Dim type As String = Convert.ToString(data(3))
                Dim desc As String = Convert.ToString(data(4))
                Dim cost As Decimal = Convert.ToDecimal(data(5))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetailPrices VALUES(NEWID(), @HeaderId, @ItemId, @Number, @Type, @Description, @Cost, 1)")
                        myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                        myCmd.Parameters.AddWithValue("@ItemId", itemId)
                        myCmd.Parameters.AddWithValue("@Number", number)
                        myCmd.Parameters.AddWithValue("@Type", type)
                        myCmd.Parameters.AddWithValue("@Description", desc)
                        myCmd.Parameters.AddWithValue("@Cost", cost)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub UpdateCost(ItemId As String, Cost As Decimal)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Cost=@Cost WHERE Id=@ItemId")
                myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                myCmd.Parameters.AddWithValue("@Cost", Cost)
                ' myCmd.Parameters.AddWithValue("@Cost", 0.00)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Sub UpdateCostOverride(ItemId As String, Cost As Decimal)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET CostOverride=@Cost WHERE Id=@ItemId")
                myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                myCmd.Parameters.AddWithValue("@Cost", Cost)
                ' myCmd.Parameters.AddWithValue("@Cost", 0.00)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Sub UpdateFinalCost(ItemId As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET FinalCost = CostOverride - Discount WHERE Id=@ItemId")
                myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Sub UpdateFinalCostIvoices(Id As String)
        Try
            Dim orderCost As Decimal = GetItemData_Decimal("SELECT SUM(FinalCost) FROM OrderDetails WHERE HeaderId='" + Id + "'")
            Dim gst As Decimal = orderCost * 10 / 100

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderInvoices VALUES (@Id, NULL, @OrderCost, @GST, 0.00)")
                    myCmd.Parameters.AddWithValue("@Id", Id)
                    myCmd.Parameters.AddWithValue("@OrderCost", orderCost)
                    myCmd.Parameters.AddWithValue("@GST", gst)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub ResetPriceDetail(HeaderId As String, itemId As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderDetailPrices WHERE HeaderId=@HeaderId AND ItemId=@ItemId")
                myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                myCmd.Parameters.AddWithValue("@ItemId", itemId)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Public Sub ResetPriceDetail_Edited(HeaderId As String, ItemId As String, Type As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderDetailPrices WHERE HeaderId=@HeaderId AND ItemId=@ItemId AND Type=@Type")
                myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                myCmd.Parameters.AddWithValue("@ItemId", ItemId)
                myCmd.Parameters.AddWithValue("@Type", Type)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    ' SHUTTERS

    Public Function CountMultiLayout(input As String, substrings As String()) As Integer
        Dim count As Integer = 0
        For Each substring In substrings
            count += input.Split(substring).Length - 1
        Next
        Return count
    End Function

    Public Function CheckStringLayoutD(layout As String) As Boolean
        For i As Integer = 0 To layout.Length - 1
            If layout(i) = "D"c Then
                Dim hasDashBefore As Boolean = (i > 0 AndAlso layout(i - 1) = "-"c)
                Dim hasDashAfter As Boolean = (i < layout.Length - 1 AndAlso layout(i + 1) = "-"c)
                If Not (hasDashBefore Or hasDashAfter) Then
                    Return False
                End If
            End If
        Next
        Return True
    End Function

    Public Function WidthDeductPanorama(data As Object()) As Decimal
        Dim result As Decimal = 0.00

        Dim blindName As String = Convert.ToString(data(0))
        Dim type As String = Convert.ToString(data(1))
        Dim width As Integer = Convert.ToInt32(data(2))

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim hingeDeduction As Decimal = 2.5
            Dim frameDeduction As Decimal = 0
            Dim frameLDeduction As Decimal = 0
            Dim frameRDeduction As Decimal = 0
            Dim tPostDeduction As Decimal = 0
            Dim bPostDeduction As Decimal = 0
            Dim cPostDeduction As Decimal = 0
            Dim csDeduction As Decimal = 0
            Dim crDeduction As Decimal = 0
            Dim ccDeduction As Decimal = 0

            Dim jumlahT As Integer = layoutCode.Split("T").Length - 1
            Dim jumlahB As Integer = layoutCode.Split("B").Length - 1
            Dim jumlahC As Integer = layoutCode.Split("C").Length - 1
            Dim jumlahD As Integer = layoutCode.Split("D").Length - 1

            If mounting = "Inside" Then
                crDeduction = -2
                If frameType.Contains("Z Frame") Then
                    crDeduction = -3.2
                End If
            End If

            If frameType = "Beaded L 48mm" Then frameDeduction = 25.4
            If frameType = "Insert L 50mm" Then frameDeduction = 22.2
            If frameType = "Insert L 63mm" Then frameDeduction = 22.2
            If frameType = "Small Bullnose Z Frame" Then frameDeduction = 19.6
            If frameType = "Large Bullnose Z Frame" Then frameDeduction = 22.7
            If frameType = "Colonial Z Frame" Then frameDeduction = 18.4

            If Not frameLeft = "No" Then
                frameLDeduction = frameDeduction
            End If
            If Not frameRight = "No" Then
                frameRDeduction = frameDeduction
            End If

            If layoutCode.Contains("D") Then csDeduction = 3.5 / 2
            If layoutCode.Contains("T") Then tPostDeduction = 25.4 / 2

            If layoutCode.Contains("B") Then
                If frameType = "Beaded L 48mm" Or frameType = "Insert L 50mm" Or frameType = "Insert L 63mm" Or frameType = "No Frame" Then
                    bPostDeduction = 30 / 2
                End If
                If mounting = "Inside" And frameType = "Small Bullnose Z Frame" Then
                    bPostDeduction = 38 / 2
                End If
                If mounting = "Inside" And frameType = "Large Bullnose Z Frame" Then
                    bPostDeduction = 40 / 2
                End If
                If mounting = "Inside" And frameType = "Colonial Z Frame" Then
                    bPostDeduction = 34 / 2
                End If
                If mounting = "Outside" And frameType = "Beaded L 48mm" Then
                    bPostDeduction = 34.7 / 2
                End If
                If mounting = "Outside" And frameType = "Insert L 50mm" Then
                    bPostDeduction = 34.7 / 2
                End If
                If mounting = "Outside" And frameType = "Insert L 63mm" Then
                    bPostDeduction = 40.4 / 2
                End If
            End If

            If layoutCode.Contains("C") Then
                If mounting = "Inside" Then
                    If frameType = "Beaded L 48mm" Or frameType = "Insert L 50mm" Or frameType = "Insert L 63mm" Or frameType = "No Frame" Then cPostDeduction = 30 / 2
                    If frameType = "Small Bullnose Z Frame" Then cPostDeduction = 42 / 2
                    If frameType = "Large Bullnose Z Frame" Then cPostDeduction = 44 / 2
                    If frameType = "Colonial Z Frame" Then cPostDeduction = 36 / 2
                End If
                If mounting = "Outside" Then
                    If frameType = "Beaded L 48mm" Then cPostDeduction = 62.5 / 2
                    If frameType = "Insert L 50mm" Then cPostDeduction = 65 / 2
                    If frameType = "Insert L 63mm" Then cPostDeduction = 78 / 2
                End If
            End If

            If type = "All" Then
                result = width - (hingeDeduction * panelQty) - ((frameLDeduction + frameRDeduction) * panelQty) - (tPostDeduction * jumlahT) - (bPostDeduction * jumlahB) - (cPostDeduction * jumlahC) - (csDeduction * jumlahD) - (crDeduction * panelQty) - (ccDeduction * panelQty)
            End If

            If type = "Gap" Then
                result = width - (hingeDeduction * panelQty) - (frameLDeduction + frameRDeduction) - tPostDeduction - bPostDeduction - cPostDeduction - csDeduction - crDeduction - ccDeduction
            End If
        End If

        If blindName = "Track Bi-fold" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim insideDeductions As Decimal = 0.00
            Dim pivotDeductions As Decimal = 0.00
            Dim closingDeductions As Decimal = 5
            Dim frameLDeductions As Decimal = 0.00
            Dim frameRDeductions As Decimal = 0.00
            Dim hingedDeductions As Decimal = 0.00

            Dim result1 As Integer = 0
            Dim parts As String() = layoutCode.Split("/"c)
            If parts.Length > 0 Then
                result1 = CountMultiLayout(parts(0), New String() {"L", "R", "F"}) - 1
            End If

            Dim result2 As Integer = 0
            If layoutCode.Contains("/") Then
                Dim partss As String() = layoutCode.Split("/"c)
                If partss.Length > 1 Then
                    result2 = CountMultiLayout(partss(1), New String() {"L", "R", "F"}) - 1
                End If
            End If

            hingedDeductions = (result1 + result2) * 5

            If mounting = "Inside" Then insideDeductions = 2

            Dim uniqueLetters As New HashSet(Of Char)
            For Each c As Char In layoutCode.ToLower()
                If Char.IsLetter(c) Then
                    uniqueLetters.Add(c)
                End If
            Next

            pivotDeductions = 5 * uniqueLetters.Count

            If Not frameLeft = "No" Then frameLDeductions = 19
            If Not frameRight = "No" Then frameRDeductions = 19

            result = width - insideDeductions - pivotDeductions - closingDeductions - frameLDeductions - frameRDeductions - hingedDeductions
        End If

        If blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim frameLDeductions As Decimal = 0
            Dim frameRDeductions As Decimal = 0
            Dim insideDeductions As Decimal = 0

            If Not frameLeft = "No" Then frameLDeductions = 19 * panelQty
            If Not frameRight = "No" Then frameRDeductions = 19 * panelQty
            If mounting = "Inside" Then insideDeductions = 2

            result = width - frameLDeductions - frameRDeductions - insideDeductions
        End If

        If blindName = "Fixed" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            If frameType = "U Channel" Then
                Dim clearanceLDeduction As Decimal = 1
                Dim clearanceRDeduction As Decimal = 1

                Dim frameLDeduction As Decimal = 0
                Dim frameRDeduction As Decimal = 0


                If frameLeft = "L Strip" Then clearanceLDeduction = 2
                If frameRight = "L Strip" Then clearanceRDeduction = 2

                If frameLeft = "L Strip" Then frameLDeduction = 7.25
                If frameRight = "L Strip" Then frameRDeduction = 7.25

                result = width - clearanceLDeduction - clearanceRDeduction - frameLDeduction - frameRDeduction
            End If

            If frameType = "19x19 Light Block" Then
                Dim clearanceDeduction As Decimal = 5

                result = width - clearanceDeduction
            End If
        End If
        Return result
    End Function

    Public Function HeightDeductPanorama(data As Object()) As Decimal
        Dim result As Decimal = 0

        Dim blindName As String = Convert.ToString(data(0))
        Dim drop As Integer = Convert.ToInt32(data(1))
        Dim mounting As String = Convert.ToString(data(2))
        Dim frameType As String = Convert.ToString(data(3))
        Dim frameTop As String = Convert.ToString(data(4))
        Dim frameBottom As String = Convert.ToString(data(5))
        Dim bottomTrackType As String = Convert.ToString(data(6))
        Dim horizontalTPost As String = Convert.ToString(data(7))

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            Dim crDeductionTop As Decimal = 0
            Dim crDeductionBottom As Decimal = 0

            Dim cfDeductionTop As Decimal = 0
            Dim cfDeductionBottom As Decimal = 0

            Dim frameDeduction As Decimal = 0
            Dim frameTDeduction As Decimal = 0
            Dim frameBDeduction As Decimal = 0
            Dim postDeduction As Decimal = 0

            If mounting = "Inside" Then
                If frameType = "Beaded L 48mm" Or frameType = "Insert L 50mm" Or frameType = "Insert L 63mm" Or frameType = "Flat L 48mm" Then
                    If frameTop = "L Striker Plate" Then crDeductionTop = 2
                    If frameBottom = "L Striker Plate" Then crDeductionBottom = 2
                End If

                If frameType.Contains("Z Frame") Then
                    If frameTop = "L Striker Plate" Then crDeductionTop = 3.2
                    If frameBottom = "L Striker Plate" Then crDeductionBottom = 3.2
                End If
            End If

            If Not frameTop = "No" Then cfDeductionTop = 3
            If Not frameBottom = "No" Then cfDeductionBottom = 3

            If frameType = "Beaded L 48mm" Then frameDeduction = 25.4
            If frameType = "Insert L 50mm" Then frameDeduction = 22.2
            If frameType = "Insert L 63mm" Then frameDeduction = 22.2
            If frameType = "Small Bullnose Z Frame" Then frameDeduction = 19.6
            If frameType = "Large Bullnose Z Frame" Then frameDeduction = 22.7
            If frameType = "Colonial Z Frame" Then frameDeduction = 18.4

            If Not frameTop = "No" Then frameTDeduction = frameDeduction
            If Not frameBottom = "No" Then frameBDeduction = frameDeduction

            If frameTop = "L Striker Plate" Or frameTop.Contains("Sill Plate") Then
                frameTDeduction = frameDeduction + 9.5
            End If

            If frameBottom = "L Striker Plate" Or frameBottom.Contains("Sill Plate") Then
                frameBDeduction = frameDeduction + 9.5
            End If

            If horizontalTPost = "No Post" Then postDeduction = 3 / 2
            If horizontalTPost = "Yes" Then postDeduction = 25.4 / 2

            result = drop - crDeductionTop - crDeductionBottom - cfDeductionTop - cfDeductionBottom - frameTDeduction - frameBDeduction - postDeduction
        End If

        If blindName = "Track Bi-fold" Or blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            Dim crTopDeduction As Decimal = 0
            Dim crBottomDeduction As Decimal = 0
            Dim trDeduction As Decimal = 51
            Dim mtDeduction As Decimal = 0
            Dim utDeduction As Decimal = 0
            Dim utfDeduction As Decimal = 0
            Dim frameDeduction As Decimal = 0
            Dim frameTDeduction As Decimal = 0
            Dim frameBDeduction As Decimal = 0

            If mounting = "Inside" Then crTopDeduction = 1
            If bottomTrackType = "M Track" Then mtDeduction = 24
            If bottomTrackType = "U Track" Then utDeduction = 33

            If frameTop = "Yes" Then frameTDeduction = 1

            If frameType = "100mm" Then frameDeduction = 19
            If frameType = "160mm" Then frameDeduction = 19
            If frameType = "200mm" Then frameDeduction = 19

            If frameTop = "Yes" Then frameTDeduction = frameDeduction

            result = drop - crTopDeduction - crBottomDeduction - trDeduction - mtDeduction - utDeduction - utfDeduction - frameTDeduction - frameBDeduction
        End If

        If blindName = "Fixed" Then
            If frameType = "U Channel" Then
                Dim topDeduction As Decimal = 0
                Dim bottomDeduction As Decimal = 0

                If frameTop = "Yes" Then topDeduction = 17.5
                If frameBottom = "Yes" Then bottomDeduction = 17.5

                result = drop - topDeduction - bottomDeduction
            End If

            If frameType = "19x19 Light Block" Then
                result = drop - 6
            End If
        End If

        Return result
    End Function

    Public Function GetPanelQty(data As String()) As Integer
        Dim result As Integer = 0

        Dim blindName As String = data(0)
        Dim panelQty As String = data(1)
        Dim layout As String = data(2)
        Dim horizontalHeight As String = data(3)

        Dim countL As Integer = 0
        Dim countR As Integer = 0
        Dim countF As Integer = 0
        Dim countM As Integer = 0
        Dim countB As Integer = 0

        countL = layout.Split("L").Length - 1
        countR = layout.Split("R").Length - 1
        countF = layout.Split("F").Length - 1
        countM = layout.Split("M").Length - 1
        If blindName = "Track Sliding" Then
            countB = layout.Split("B").Length - 1
        End If

        Dim hitung As Integer = countL + countR + countF + countM + countB
        If horizontalHeight > 0 Then
            hitung = (countL + countR + countF + countM + countB) * 2
        End If

        If blindName = "Panel Only" Then
            hitung = panelQty
        End If

        result = hitung

        Return result
    End Function

    Public Function MidrailHeigtEvolve(data As Object()) As Decimal
        Dim result As Decimal = 0

        Dim blindName As String = Convert.ToString(data(0))

        Return result
    End Function

    Public Function WidthDeductEvolve(data As Object()) As Decimal
        Dim result As Decimal = 0.00

        Dim blindName As String = Convert.ToString(data(0))
        Dim type As String = Convert.ToString(data(1))
        Dim width As Integer = Convert.ToInt32(data(2))

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim hingeDeduction As Decimal = 2.5
            Dim frameDeduction As Decimal = 0
            Dim frameLDeduction As Decimal = 0
            Dim frameRDeduction As Decimal = 0
            Dim tPostDeduction As Decimal = 0
            Dim bPostDeduction As Decimal = 0
            Dim cPostDeduction As Decimal = 0
            Dim csDeduction As Decimal = 0
            Dim crDeduction As Decimal = 0
            Dim ccDeduction As Decimal = 0

            Dim jumlahT As Integer = layoutCode.Split("T").Length - 1
            Dim jumlahB As Integer = layoutCode.Split("B").Length - 1
            Dim jumlahC As Integer = layoutCode.Split("C").Length - 1
            Dim jumlahD As Integer = layoutCode.Split("D").Length - 1

            If mounting = "Inside" Then
                crDeduction = -2
                If frameType.Contains("Z Frame") Then
                    crDeduction = -3.2
                End If
            End If

            If frameType = "Beaded L 49mm" Then frameDeduction = 22.5
            If frameType = "Insert L 49mm" Then frameDeduction = 22.5
            If frameType = "Insert L 63mm" Then frameDeduction = 22.5
            If frameType = "Small Bullnose Z Frame" Then frameDeduction = 22.5
            If frameType = "Large Bullnose Z Frame" Then frameDeduction = 22.5

            If Not frameLeft = "No" Then
                frameLDeduction = frameDeduction
            End If
            If Not frameRight = "No" Then
                frameRDeduction = frameDeduction
            End If

            If layoutCode.Contains("D") Then csDeduction = 3.52 / 2
            If layoutCode.Contains("T") Then tPostDeduction = 25.4 / 2

            If layoutCode.Contains("B") And mounting = "Inside" Then
                If frameType = "Beaded L 49mm" Or frameType = "Insert L 49mm" Or frameType = "No Frame" Then
                    bPostDeduction = 23.4 / 2
                End If
            End If

            If layoutCode.Contains("C") Then
                If mounting = "Inside" Then
                    cPostDeduction = 25
                End If
            End If

            If type = "All" Then
                result = width - (hingeDeduction * panelQty) - ((frameLDeduction + frameRDeduction) * panelQty) - (tPostDeduction * jumlahT) - (bPostDeduction * jumlahB) - (cPostDeduction * jumlahC) - (csDeduction * jumlahD) - (crDeduction * panelQty) - (ccDeduction * panelQty)
            End If

            If type = "Gap" Then
                result = width - (hingeDeduction * panelQty) - (frameLDeduction + frameRDeduction) - tPostDeduction - bPostDeduction - cPostDeduction - csDeduction - crDeduction - ccDeduction
            End If
        End If

        If blindName = "Track Bi-fold" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim insideDeductions As Decimal = 0.00
            Dim pivotDeductions As Decimal = 0.00
            Dim closingDeductions As Decimal = 5
            Dim frameLDeductions As Decimal = 0.00
            Dim frameRDeductions As Decimal = 0.00
            Dim hingedDeductions As Decimal = 0.00

            Dim result1 As Integer = 0
            Dim parts As String() = layoutCode.Split("/"c)
            If parts.Length > 0 Then
                result1 = CountMultiLayout(parts(0), New String() {"L", "R", "F"}) - 1
            End If

            Dim result2 As Integer = 0
            If layoutCode.Contains("/") Then
                Dim partss As String() = layoutCode.Split("/"c)
                If partss.Length > 1 Then
                    result2 = CountMultiLayout(partss(1), New String() {"L", "R", "F"}) - 1
                End If
            End If

            hingedDeductions = (result1 + result2) * 5

            If mounting = "Inside" Then insideDeductions = 2

            Dim uniqueLetters As New HashSet(Of Char)
            For Each c As Char In layoutCode.ToLower()
                If Char.IsLetter(c) Then
                    uniqueLetters.Add(c)
                End If
            Next

            pivotDeductions = 5 * uniqueLetters.Count

            If Not frameLeft = "No" Then frameLDeductions = 22
            If Not frameRight = "No" Then frameRDeductions = 22

            result = width - insideDeductions - pivotDeductions - closingDeductions - frameLDeductions - frameRDeductions - hingedDeductions
        End If

        If blindName = "Track Sliding" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim frameLDeductions As Decimal = 0
            Dim frameRDeductions As Decimal = 0
            Dim insideDeductions As Decimal = 0

            If Not frameLeft = "No" Then frameLDeductions = 22 * panelQty
            If Not frameRight = "No" Then frameRDeductions = 22 * panelQty
            If mounting = "Inside" Then insideDeductions = 2

            result = width - frameLDeductions - frameRDeductions - insideDeductions
        End If

        If blindName = "Fixed" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            If frameType = "U Channel" Then
                Dim clearanceLDeduction As Decimal = 1
                Dim clearanceRDeduction As Decimal = 1

                Dim frameLDeduction As Decimal = 0
                Dim frameRDeduction As Decimal = 0


                If frameLeft = "L Strip" Then clearanceLDeduction = 2
                If frameRight = "L Strip" Then clearanceRDeduction = 2

                If frameLeft = "L Strip" Then frameLDeduction = 7.25
                If frameRight = "L Strip" Then frameRDeduction = 7.25

                result = width - clearanceLDeduction - clearanceRDeduction - frameLDeduction - frameRDeduction
            End If

            If frameType = "19x19 Light Block" Then
                Dim clearanceDeduction As Decimal = 5

                result = width - clearanceDeduction
            End If
        End If
        Return result
    End Function

    Public Function HeightDeductEvolve(data As Object()) As Decimal
        Dim result As Decimal = 0

        Dim blindName As String = Convert.ToString(data(0))
        Dim drop As Integer = Convert.ToInt32(data(1))
        Dim mounting As String = Convert.ToString(data(2))
        Dim frameType As String = Convert.ToString(data(3))
        Dim frameTop As String = Convert.ToString(data(4))
        Dim frameBottom As String = Convert.ToString(data(5))
        Dim bottomTrackType As String = Convert.ToString(data(6))
        Dim horizontalTPost As String = Convert.ToString(data(7))

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            Dim crDeductionTop As Decimal = 0
            Dim crDeductionBottom As Decimal = 0

            Dim cfDeductionTop As Decimal = 0
            Dim cfDeductionBottom As Decimal = 0

            Dim frameDeduction As Decimal = 0
            Dim frameTDeduction As Decimal = 0
            Dim frameBDeduction As Decimal = 0

            If mounting = "Inside" Then
                If frameType = "Beaded L 49mm" Or frameType = "Insert L 49mm" Then
                    If frameTop = "L Striker Plate" Then crDeductionTop = 2
                    If frameBottom = "L Striker Plate" Then crDeductionBottom = 2
                End If

                If frameType.Contains("Z Frame") Then
                    If frameTop = "L Striker Plate" Then crDeductionTop = 3.2
                    If frameBottom = "L Striker Plate" Then crDeductionBottom = 3.2
                End If
            End If

            If Not frameTop = "No" Then cfDeductionTop = 3
            If Not frameBottom = "No" Then cfDeductionBottom = 3

            If frameType = "Beaded L 49mm" Then frameDeduction = 22.5
            If frameType = "Insert L 49mm" Then frameDeduction = 22.5
            If frameType = "No Frame" Then frameDeduction = 22.5
            If frameType = "Small Bullnose Z Frame" Then frameDeduction = 22.5
            If frameType = "Large Bullnose Z Frame" Then frameDeduction = 22.5

            If Not frameTop = "No" Then frameTDeduction = frameDeduction
            If Not frameBottom = "No" Then frameBDeduction = frameDeduction

            result = drop - crDeductionTop - crDeductionBottom - cfDeductionTop - cfDeductionBottom - frameTDeduction - frameBDeduction
        End If

        If blindName = "Track Bi-fold" Or blindName = "Track Sliding" Then
            Dim frameDeduction As Decimal = 0
            Dim frameTDeduction As Decimal = 0
            Dim frameBDeduction As Decimal = 0

            If frameTop = "Yes" Then frameTDeduction = 1

            If frameTop = "Yes" Then frameTDeduction = 73
            If frameBottom = "Yes" Then frameTDeduction = 46
            If frameBottom = "No" Then frameTDeduction = 24

            result = drop - frameTDeduction - frameBDeduction
        End If

        If blindName = "Fixed" Then
            If frameType = "U Channel" Then
                Dim topDeduction As Decimal = 0
                Dim bottomDeduction As Decimal = 0

                If frameTop = "Yes" Then topDeduction = 17.5
                If frameBottom = "Yes" Then bottomDeduction = 9.5

                result = drop - topDeduction - bottomDeduction
            End If

            If frameType = "19x19 Light Block" Then
                result = drop - 3
            End If
        End If

        Return result
    End Function
End Class
