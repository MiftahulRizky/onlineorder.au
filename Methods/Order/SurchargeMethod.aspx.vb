Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Partial Class Methods_Order_SurchargeMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared orderCfg As New OrderConfig()
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ParamSubmit
        '#Submit OrderHeader
        Public Property blindtype As String
        Public Property tubetype As String
        Public Property controltype As String
        

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
                    query = String.Format("SELECT TubeType FROM HardwareKits WHERE DesignId = '{0}' AND BlindId = '{1}' AND Active=1 GROUP BY TubeType ORDER BY TubeType ASC", data.designid, UCase(data.blindtype).ToString())
                    Return GetFormattedData(query, "TubeType", "TubeType")

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
            If String.IsNullOrEmpty(data.blindtype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "category is required !", .field = "blindtype"}}
            End If

            If String.IsNullOrEmpty(data.tubetype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "product is required !", .field = "tubetype"}}
            End If

            If String.IsNullOrEmpty(data.controltype) Then
                Return New ErrorResponse With { .error = New ErrorDetail With { .message = "surcharge type is required !", .field = "controltype"}}
            End If

            Dim BlindName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Blinds WHERE Id = '{0}'", data.blindtype))
            Dim TubeName As String = publicCfg.GetItemData(String.Format("SELECT TubeType FROM HardwareKits WHERE Id = '{0}'", data.controltype))
            Dim ControlName As String = publicCfg.GetItemData(String.Format("SELECT ControlType FROM HardwareKits WHERE Id = '{0}'", data.controltype))
            Dim KitName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM HardwareKits WHERE Id = '{0}'", data.controltype))
            Dim SoeId As String = publicCfg.GetItemData(String.Format("SELECT SoeId FROM HardwareKits WHERE Id='{0}'", data.controltype))
            Dim Delivery As String = publicCfg.GetItemData(String.Format("SELECT Delivery FROM OrderHeaders WHERE Id='{0}'", data.headerid))


            Dim priceGroupName As String = "Surcharge"
            IF InArray(BlindName, "Interim Levy") Then
                PriceGroupName = BlindName
            End If
            
            IF InArray(BlindName, "Residential Home Address") Then
                If Delivery = "Pick Up" Then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Pick Up Residential Home Address is not allowed!", .field = "#modalAddService #id"}}
                End If
                PriceGroupName = BlindName
            End If

            Dim PriceGroupId As String = publicCfg.GetPriceGroupId(data.designid, priceGroupName)
            If String.IsNullOrEmpty(PriceGroupId) Then
               Throw New Exception("price group id not found !")
            End If

            If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
                Dim itemId As String = publicCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, KitId, SoeKitId, PriceGroupId, BlindNo, Qty, Width, [Drop], Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active) VALUES (@Id, @HeaderId, @KitId, @SoeKitId, @PriceGroupId, 'Blind 1', 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(PriceGroupId).ToString())
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                publicCfg.ResetPriceDetail(itemId)
                publicCfg.HitungHarga(data.headerid, itemId)
                publicCfg.HitungSurcharge(data.headerid, itemId)
                Dim OrderType As String = publicCfg.GetItemData(String.Format("SELECT OrderType FROM OrderHeaders WHERE Id='{0}'", data.headerid))
                Dim dataLog As Object() = {data.headerid, itemId, OrderType, data.loginid, "Add Surcharge Oder"}
                orderCfg.Log_Orders(dataLog)


                msg = "Surcharge added successfully !"
            End If

            If data.itemaction = "EditItem" OrElse data.itemaction = "ViewItem" Then
                Dim itemId As String = data.itemid
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("UPDATE OrderDetails SET KitId=@KitId, SoeKitId=@SoeKitId, PriceGroupId=@PriceGroupId WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        ' myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.controltype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", SoeId)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(PriceGroupId).ToString())
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                publicCfg.ResetPriceDetail(itemId)
                publicCfg.HitungHarga(data.headerid, itemId)
                ' publicCfg.HitungSurcharge(data.headerid, itemId)
                Dim OrderType As String = publicCfg.GetItemData(String.Format("SELECT OrderType FROM OrderHeaders WHERE Id='{0}'", data.headerid))
                Dim dataLog As Object() = {data.headerid, itemId, OrderType, data.loginid, "Update Surcharge Order"}
                orderCfg.Log_Orders(dataLog)

                msg = "Surcharge updated successfully !"
            End If

            Return New SuccessResponse With {.success = msg}
        Catch ex As Exception
            Dim msg As String = "Please contact our IT team at support@onlineorder.au"
            If data.rolename = "Administrator" Then msg = ex.Message
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = msg, .field = ""}}
        End Try
    End Function


End Class
