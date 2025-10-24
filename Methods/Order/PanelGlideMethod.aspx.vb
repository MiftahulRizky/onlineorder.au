Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic

Partial Class Methods_OrderFormPage_PanelGlides_PanelGlideMethod
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
    Public Shared Function BindFabricType(ByVal designid As String, ByVal blindname As String) As Object
        Try
            Dim type2 As String = "(PG Sewless Plantation)"
            If blindname = "Plain" Then
                type2 = "(PG Plain)"
            End If
            
            Dim datas As DataSet = publicCfg.GetListData("SELECT Type FROM Fabrics WHERE DesignId='" + designid + "' AND Type LIKE '%"+type2+"%' AND Active='1' GROUP BY Type ORDER BY Type ASC")
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
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, Colour FROM Fabrics WHERE DesignId='" + designid + "' AND Active='1' AND Type='" + fabrictype + "' ORDER BY Name ASC")
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


    '#------------------------||BindItemOrder||------------------------#
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


    '#-----------------------------------------------------------------------
    '# SUBMIT FUNCTION
    '#-----------------------------------------------------------------------

    '#----------------------------|| INISIASI FIELD ||-----------------------#
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
        Public Property layoutcode As String
        Public Property nopanel As String
        Public Property tracktype As String
        Public Property trackcolour As String
        Public Property wandposition As String
        Public Property wandlength As String
        Public Property wandcolour As String
        Public Property batten As String
        Public Property battencolour As String
        Public Property fitting As String
        Public Property notes As String
        Public Property markup As String
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
    End Class

    '#----------------------------|| DEFIND CLASS RESPONSE ||-----------------------#
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
    Public Shared Function SubmitForm(ByVal data As FormData) As Object
        Try
            '#-------------------------|| SET VALIDATE RULES ||-----------------------#
            '#-------------------------|| blindtype ||-----------------------#
            If String.IsNullOrEmpty(data.blindtype) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "type is required !",
                        .field = "blindtype"
                    }
                }
            End If

            '#-----------------------|| colourtype ||-----------------------#
            If String.IsNullOrEmpty(data.colourtype) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "colour is required !",
                        .field = "colourtype"
                    }
                }
            End If

            Dim blindName As String = publicCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + data.blindtype + "'")

            '#-----------------------|| qty ||-----------------------#
            Dim qty As Integer
            If String.IsNullOrEmpty(data.qty) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "qty is required !",
                        .field = "qty"
                    }
                }
            End If
            If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "qty must be a positive integer !",
                        .field = "qty"
                    }
                }
            End If
            If qty > 5 Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "qty must be less than or equal to 5 !",
                        .field = "qty"
                    }
                }
            End If

            '#-----------------------|| room ||-----------------------#
            If String.IsNullOrEmpty(data.room) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "room to install is required !",
                        .field = "room"
                    }
                }
            End If

            '#-----------------------|| mounting ||-----------------------#
            If String.IsNullOrEmpty(data.mounting) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "mounting is required !",
                        .field = "mounting"
                    }
                }
            End If

            '#-----------------------|| fabrictype ||-----------------------#
            If String.IsNullOrEmpty(data.fabrictype) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "fabric type is required !",
                        .field = "fabrictype"
                    }
                }
            End If

            '#-----------------------|| fabriccolour ||-----------------------#
            If String.IsNullOrEmpty(data.fabriccolour) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "fabric colour is required !",
                        .field = "fabriccolour"
                    }
                }
            End If

            '#-----------------------|| width ||-----------------------#
            Dim width As Integer
            If String.IsNullOrEmpty(data.width) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "width is required !",
                        .field = "width"
                    }
                }
            End If
            If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "width must be a positive integer !",
                        .field = "width"
                    }
                }
            End If
            If width > 6000 Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "width must be less than or equal to 6000 !",
                        .field = "width"
                    }
                }
            End If
            '#-----------------------|| drop ||-----------------------#
            Dim drop As Integer
            If String.IsNullOrEmpty(data.drop) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "drop is required !",
                        .field = "drop"
                    }
                }
            End If
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "drop must be a positive integer !",
                        .field = "drop"
                    }
                }
            End If

            If drop > 3000 Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "drop must be less than or equal to 3000 !",
                        .field = "drop"
                    }
                }
            End If

            '#-----------------------|| layoutcode ||-----------------------#
            If String.IsNullOrEmpty(data.layoutcode) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "layout code is required !",
                        .field = "layoutcode"
                    }
                }
            End If

            '#-----------------------|| nopanel ||-----------------------#
            If String.IsNullOrEmpty(data.nopanel) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "no of panel is required !",
                        .field = "nopanel"
                    }
                }
            End If

            '#-----------------------|| tracktype ||-----------------------#
            If String.IsNullOrEmpty(data.tracktype) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "track type is required !",
                        .field = "tracktype"
                    }
                }
            End If

            '#-----------------------|| trackcolour ||-----------------------#
            If String.IsNullOrEmpty(data.trackcolour) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "track colour is required !",
                        .field = "trackcolour"
                    }
                }
            End If

            '#-----------------------|| wandposition ||-----------------------#
            If String.IsNullOrEmpty(data.wandposition) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "wand position is required !",
                        .field = "wandposition"
                    }
                }
            End If

            '#-----------------------|| wandlength ||-----------------------#
            Dim wandlength As Integer
            If String.IsNullOrEmpty(data.wandlength) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "wand length is required !",
                        .field = "wandlength"
                    }
                }
            End If
             If Not Integer.TryParse(data.wandlength, wandlength) OrElse wandlength <= 0 Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "wand length must be a positive integer !",
                        .field = "wandlength"
                    }
                }
            End If

            '#-----------------------|| wandcolour ||-----------------------#
            If String.IsNullOrEmpty(data.wandcolour) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "wand colour is required !",
                        .field = "wandcolour"
                    }
                }
            End If

            '#-----------------------|| batten ||-----------------------#
            If String.IsNullOrEmpty(data.batten) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "batten is required !",
                        .field = "batten"
                    }
                }
            End If

            '#-----------------------|| battencolour ||-----------------------#
            If data.batten = "Yes" Then
                If String.IsNullOrEmpty(data.battencolour) Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "batten colour is required !",
                            .field = "battencolour"
                        }
                    }
                End If
            End If

            '#-----------------------|| fitting ||-----------------------#
            If String.IsNullOrEmpty(data.fitting) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "fitting is required !",
                        .field = "fitting"
                    }
                }
            End If

            '#--------------------------|| notes ||--------------------------#
            If Not String.IsNullOrEmpty(data.notes) Then
                If data.notes.Trim().Length > 1000 Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "notes must be less than 1000 characters !",
                            .field = "notes"
                        }
                    }
                End If
            End If

             '#--------------------------|| notes ||--------------------------#
            Dim markup As Integer
            If Not String.IsNullOrEmpty(data.markup) Then
                If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "please check your markup !",
                            .field = "markup"
                        }
                    }
                End If
            End If


            '#------------------------------------------------|| Prepare Submit ||-------------------------------------------------#
            '#-----------------------------------|| Set default values before submission ||----------------------------------------#
            Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            
            Dim numOfWand As Integer = 1
            If data.layoutcode = "E" Or data.layoutcode = "F" Then
                numOfWand = 2
            End If

            Dim findBattenColour As String = data.battencolour
            If data.batten = "No" Then
                findBattenColour = ""
            End If

            Dim soeKitId As String = publicCfg.GetItemData("SELECT SoeId FROM HardwareKits WHERE Id = '" + data.colourtype + "'")
            Dim fabricData As DataSet = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + data.fabriccolour + "'")
            
            Dim fabricId As String = fabricData.Tables(0).Rows(0).Item("Id").ToString()
            Dim fabricGroupName As String = fabricData.Tables(0).Rows(0).Item("Group").ToString()

            Dim peiceGroupName As String = "Panel Glide - " & fabricGroupName
            Dim priceGroupId As String = publicCfg.GetPriceGroupId(data.designId,peiceGroupName)

          
            '#-----------------------|| SUBMIT VALIDATE ||-----------------------#
            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then

                Dim itemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, KitId, SoeKitId, FabricId, PriceGroupId, BlindNo, Qty, Location, Mounting, Width, [Drop], Layout, NumOfPanel, TrackType, TrackColour, NumOfWand, WandPosition, WandLength, WandColour, Batten, BattenColour, Fitting, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active) VALUES (@Id, @HeaderId, @KitId, @SoeKitId, @FabricId, @PriceGroupId, 'Blind 1', @Qty, @Location, @Mounting, @Width, @Drop, @Layout, @NumOfPanel, @TrackType, @TrackColour, @NumOfWand, @WandPosition, @WandLength, @WandColour, @Batten, @BattenColour, @Fitting, @Notes, 0, 0, 0, 0, 0, 0, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.colourtype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", soeKitId)
                        myCmd.Parameters.AddWithValue("@FabricId", fabricId)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(priceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@Layout", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@NumOfPanel", data.nopanel)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                        myCmd.Parameters.AddWithValue("@NumOfWand", numOfWand)
                        myCmd.Parameters.AddWithValue("@WandPosition", data.wandposition)
                        myCmd.Parameters.AddWithValue("@WandLength", data.wandlength)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                        myCmd.Parameters.AddWithValue("@Batten", data.batten)
                        myCmd.Parameters.AddWithValue("@BattenColour", findBattenColour)
                        myCmd.Parameters.AddWithValue("@Fitting", data.fitting)
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

                Return New SuccessResponse With {
                    .success = "Data has been saved successfully."
                }
            End If


            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then
                Dim itemId As String = data.itemid

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET  KitId = @KitId, SoeKitId = @SoeKitId, FabricId = @FabricId, PriceGroupId = @PriceGroupId, BlindNo = 'Blind 1', Qty = @Qty, Location = @Location, Mounting = @Mounting, Width = @Width, [Drop] = @Drop, Layout = @Layout, NumOfPanel = @NumOfPanel, TrackType = @TrackType, TrackColour = @TrackColour, NumOfWand = @NumOfWand, WandPosition = @WandPosition, WandLength = @WandLength, WandColour = @WandColour, Batten = @Batten, BattenColour = @BattenColour, Fitting = @Fitting, Notes = @Notes, MarkUp = @MarkUp WHERE Id = @Id")
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
                        myCmd.Parameters.AddWithValue("@Layout", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@NumOfPanel", data.nopanel)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                        myCmd.Parameters.AddWithValue("@NumOfWand", numOfWand)
                        myCmd.Parameters.AddWithValue("@WandPosition", data.wandposition)
                        myCmd.Parameters.AddWithValue("@WandLength", data.wandlength)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                        myCmd.Parameters.AddWithValue("@Batten", data.batten)
                        myCmd.Parameters.AddWithValue("@BattenColour", findBattenColour)
                        myCmd.Parameters.AddWithValue("@Fitting", data.fitting)
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

                Return New SuccessResponse With {
                    .success = "Data has been updated successfully."
                }
            End If
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function
End Class
