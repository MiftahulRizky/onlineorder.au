Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic

Partial Class Methods_Order_RomanBlindMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()

    '#-------------------------------|| INITIALIZE CLASS ||--------------------------#
    Public Class ParamSaveData
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
    Public Shared Function GetDesignName(ByVal designId As String) As Object
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
            Dim datas As DataSet = publicCfg.GetListData("SELECT Id, ControlType FROM HardwareKits WHERE DesignId = '" + designId + "' AND BlindId='" + UCase(blindId).ToString() + "' AND Active=1 ORDER BY ControlType ASC")
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
    Public Shared Function BindFabricType(ByVal designid As String, ByVal blindname As String, ByVal controlname As String) As Object
        Try
            Dim type As String = "(Roman)"
            Dim type2 As String = ""
            Dim type3 As String = ""

            Select Case blindname
                Case "Classic"
                    type2 = "(Roman Classic)"
                    If controlname = "Chain" Then
                        type3 = "(Roman Classic Chain)"
                    Else
                        type3 = "(Roman Classic Cord)"
                    End If
                Case "Plantation"
                    type2 = "(Roman Plantation)"
                    If controlname = "Chain" Then
                        type3 = "(Roman Plantation Chain)"
                    Else
                        type3 = "(Roman Plantation Cord)"
                    End If
                Case "Sewless"
                    type2 = "(Roman Sewless)"
                    If controlname = "Chain" Then
                        type3 = "(Roman Sewless Chain)"
                    Else
                        type3 = "(Roman Sewless Cord)"
                    End If
            End Select

            

            ' Jalankan query
            Dim datas As DataSet = publicCfg.GetListData("SELECT Type FROM Fabrics WHERE DesignId='"+designId+"' AND (Type LIKE '%"+type+"%' OR Type LIKE '%"+type2+"%' OR Type LIKE '%"+type3+"%') AND Active='1' GROUP BY Type ORDER BY Type ASC")

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


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SaveData(ByVal data As ParamSaveData) As Object
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

            '#-----------------------|| controltype ||-----------------------#
            If String.IsNullOrEmpty(data.controltype) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "control type is required !",
                        .field = "controltype"
                    }
                }
            End If

            Dim blindName As String = publicCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + data.blindtype + "'")
            Dim controlname As String = publicCfg.GetItemData("SELECT ControlType FROM HardwareKits WHERE Id = '" + data.controltype + "'")

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
            If width > 3000 Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "width must be less than or equal to 3000 !",
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

            If drop > 3200 Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "drop must be less than or equal to ` !",
                        .field = "drop"
                    }
                }
            End If

            '#-----------------------|| controlposition ||-----------------------#
            If String.IsNullOrEmpty(data.controlposition) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "control position is required !",
                        .field = "controlposition"
                    }
                }
            End If

            '#-----------------------|| chain control ||-----------------------#
            If controlname = "Chain" Then
                '#-----------------------|| materialchain ||-----------------------#
                If String.IsNullOrEmpty(data.materialchain) Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "material chain is required !",
                            .field = "materialchain"
                        }
                    }
                End If

                '#-----------------------|| chaincolour ||-----------------------#
                If String.IsNullOrEmpty(data.chaincolour) Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "chain colour is required !",
                            .field = "chaincolour"
                        }
                    }
                End If

                '#-----------------------|| chainlength ||-----------------------#
                ' If String.IsNullOrEmpty(data.chainlength) Then
                '     Return New ErrorResponse With {
                '         .error = New ErrorDetail With {
                '             .message = "chain length is required !",
                '             .field = "chainlength"
                '         }
                '     }
                ' End If

                '#-----------------------|| battencolour ||-----------------------#
                If String.IsNullOrEmpty(data.battencolour) Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "batten colour is required !",
                            .field = "battencolour"
                        }
                    }
                End If
            End If'#/chain control

            '#-----------------------|| cordlock control ||-----------------------#
            If controlname = "Cord" Then
                '#-----------------------|| cordcolour ||-----------------------#
                If String.IsNullOrEmpty(data.cordcolour) Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "cord colour is required !",
                            .field = "cordcolour"
                        }
                    }
                End If

                '#-----------------------|| cordlength ||-----------------------#
                If String.IsNullOrEmpty(data.cordlength) Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "cord length is required !",
                            .field = "cordlength"
                        }
                    }
                End If

                '#-----------------------|| battencolour ||-----------------------#
                If Not blindName = "Classic" Then
                    If String.IsNullOrEmpty(data.battencolour) Then
                        Return New ErrorResponse With {
                            .error = New ErrorDetail With {
                                .message = "batten colour is required !",
                                .field = "battencolour"
                            }
                        }
                    End If
                End If

                '#-----------------------|| plasticcolour ||-----------------------#
                If String.IsNullOrEmpty(data.plasticcolour) Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "acorn plastic colour is required !",
                            .field = "plasticcolour"
                        }
                    }
                End If

                '#-----------------------|| cleat ||-----------------------#
                If String.IsNullOrEmpty(data.cleat) Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "cleat is required !",
                            .field = "cleat"
                        }
                    }
                End If
            End If '#/cordlock control

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

            Dim msg As String = "debug message"
            Dim chainId As String

            Dim chainColour As String = "(" & data.chaincolour & ")"
            Dim chainLength As String = data.chainlength
            If controlname = "Chain" Then

                '#jika chain length kosong maka diisi dengan default
                If String.IsNullOrEmpty(data.chainlength) Then
                    chainLength = "500"
                    If data.drop > 700 Then : chainLength = "600" : End If
                    If data.drop > 800 Then : chainLength = "800" : End If
                    If data.drop > 1100 Then : chainLength = "1000" : End If
                    If data.drop > 1300 Then : chainLength = "1200" : End If
                    If data.drop > 1600 Then : chainLength = "1500" : End If
                    If data.drop > 2000 Then : chainLength = "1800" : End If
                    If data.drop > 2400 Then : chainLength = "2000" : End If
                    If data.drop > 2700 Then : chainLength = "2200" : End If
                End If

                Dim chainName As String = chainLength & " " & "Chain + Joiner" & " " & chainColour
                Dim FormulaChain As String = publicCfg.GetItemData("SELECT Id FROM Chains WHERE Name = '" + chainName + "'")

                IF Not FormulaChain = "" Then
                    chainId = FormulaChain
                End If

                If FormulaChain = "" Then
                    chainName = "Custom Chain + Joiner " & chainColour
                    chainId = publicCfg.GetItemData("SELECT Id FROM Chains WHERE Name = '" + chainName + "'")
                End If

                '# kosongkan opsi cord
                data.cordcolour = "" : data.cordlength = "" : data.plasticcolour ="" : data.cleat = ""

                '#debug
                ' Return New ErrorResponse With {
                '     .error = New ErrorDetail With {
                '         .message = chainColour,
                '         .field = "chaincolour"
                '     }
                ' }
            End If

            If controlname = "Cord" Then
                '# kosongkan opsi chain
                chainId = "" : data.materialchain = "" : data.chaincolour = "" : data.chainlength = ""
                If blindname = "Classic" Then : data.battencolour = "" : End If
            End If

            Dim soeKitId As String = publicCfg.GetItemData("SELECT SoeId FROM HardwareKits WHERE Id = '" + data.controlType + "'")
            Dim fabricData As DataSet = publicCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + data.fabriccolour + "'")
            
            Dim fabricId As String = fabricData.Tables(0).Rows(0).Item("Id").ToString()
            Dim fabricGroupName As String = fabricData.Tables(0).Rows(0).Item("Group").ToString()

            Dim peiceGroupName As String = "Roman Blind - " & fabricGroupName
            Dim priceGroupId As String = publicCfg.GetPriceGroupId(data.designId,peiceGroupName)


            

            '#-----------------------|| SUBMIT VALIDATE ||-----------------------#
            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim itemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, KitId, SoeKitId, FabricId, ChainId, PriceGroupId, BlindNo, Qty, Location, Mounting, Width, [Drop], ControlPosition, MaterialChain, ChainLength, CordColour, CordLength, BattenColour,  AcornPlasticColour, Cleat, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active) VALUES (@Id, @HeaderId, @KitId, @SoeKitId, @FabricId, @ChainId, @PriceGroupId, 'Blind 1', @Qty, @Location, @Mounting, @Width, @Drop, @ControlPosition, @MaterialChain, @ChainLength, @CordColour, @CordLength, @BattenColour, @AcornPlasticColour, @Cleat, @Notes, 0, 0, 0, 0, 0, 0, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", soeKitId)
                        myCmd.Parameters.AddWithValue("@FabricId", fabricId)
                        myCmd.Parameters.AddWithValue("@ChainId", chainId)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(priceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@MaterialChain", data.materialchain)
                        myCmd.Parameters.AddWithValue("@ChainLength", chainLength)
                        myCmd.Parameters.AddWithValue("@CordColour", data.cordcolour)
                        myCmd.Parameters.AddWithValue("@CordLength", data.cordlength)
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

                msg = "Data has been saved successfully."
            End If


            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then
                Dim itemId As String = data.itemid

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET KitId=@KitId, SoeKitId=@SoeKitId, FabricId=@FabricId, ChainId=@ChainId, PriceGroupId=@PriceGroupId, BlindNo='Blind 1', Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, ControlPosition=@ControlPosition, MaterialChain=@MaterialChain, ChainLength=@ChainLength, CordColour=@CordColour, CordLength=@CordLength, BattenColour=@BattenColour, AcornPlasticColour=@AcornPlasticColour, Cleat=@Cleat, Notes=@Notes, Matrix=0, Charge=0, Discount=0, TotalMatrix=0, TotalCharge=0, TotalDiscount=0, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        ' myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", soeKitId)
                        myCmd.Parameters.AddWithValue("@FabricId", fabricId)
                        myCmd.Parameters.AddWithValue("@ChainId", chainId)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(priceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@MaterialChain", data.materialchain)
                        myCmd.Parameters.AddWithValue("@ChainLength", chainLength)
                        myCmd.Parameters.AddWithValue("@CordColour", data.cordcolour)
                        myCmd.Parameters.AddWithValue("@CordLength", data.cordlength)
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

                msg = "Data has been updated successfully."
            End If


            Return New SuccessResponse With { .success = msg }
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
