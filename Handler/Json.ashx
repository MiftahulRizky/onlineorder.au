<%@ WebHandler Language="VB" Class="Json" %>

Imports System.IO
Imports System.Web.Script.Serialization
Imports System.Data.SqlClient
Imports System.Threading.Tasks

Public Class Json : Implements IHttpHandler

    Dim orderCfg As New OrderConfig

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"

        Dim apiKeyHeader As String = context.Request.Headers("X-API-KEY")
        Dim validApiKey As String = System.Configuration.ConfigurationManager.AppSettings("ApiKey")

        If String.IsNullOrEmpty(apiKeyHeader) OrElse apiKeyHeader <> validApiKey Then
            context.Response.StatusCode = 401 ' Unauthorized
            context.Response.Write("{""status"": ""error"", ""message"": ""Invalid or missing API key""}")
            Return
        End If

        If context.Request.HttpMethod <> "POST" Then
            context.Response.StatusCode = 405 ' Method Not Allowed
            context.Response.Write("{""status"": ""error"", ""message"": ""Method not allowed""}")
            Return
        End If

        Dim inputStream As Stream = context.Request.InputStream
        Dim reader As New StreamReader(inputStream)
        Dim jsonString As String = reader.ReadToEnd()

        If String.IsNullOrEmpty(jsonString) Then
            context.Response.StatusCode = 400
            context.Response.Write("{""status"": ""error"", ""message"": ""Request body is empty""}")
            Return
        End If

        Dim serializer As New JavaScriptSerializer()
        Try
            Dim orderData As OrderData = serializer.Deserialize(Of OrderData)(jsonString)

            Dim connStr As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
            Using conn As New SqlConnection(connStr)
                conn.Open()
                Dim transaction As SqlTransaction = conn.BeginTransaction()

                Try
                    Dim headerId As String = orderCfg.CreateOrderHeaderId()
                    Dim cashsale As String = orderCfg.GetItemData("SELECT CashSale FROM Customers WHERE Id = 'LS-A224'")
                    Dim Depo As String = "1"
                    If cashsale = "True" Then Depo = "0"
                    Dim orderId As String = "SPP-" & headerId

                    Dim FieldName As String = "Id, OrderId, JobId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, SubmittedBy, SubmittedDate, Deposit, Approved, Active"

                    Dim FieldValue As String = "@Id, @OrderId, @JobId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, @OrderType, 'New Order', @CreatedBy, GETDATE(), @SubmittedBy, GETDATE(), @Deposit, 1, 1"

                    '#OrderHeader
                    Using cmd As New SqlCommand("INSERT INTO OrderHeaders({FieldName}) VALUES ({FieldValue}); INSERT INTO OrderQuotes (Id) VALUES (@Id);", conn, transaction)
                        cmd.Parameters.AddWithValue("@Id", headerId)
                        cmd.Parameters.AddWithValue("@OrderId", orderId)
                        cmd.Parameters.AddWithValue("@JobId", DBNull.Value)
                        cmd.Parameters.AddWithValue("@CustomerId", "LS-A224")
                        cmd.Parameters.AddWithValue("@OrderNumber", orderData.OrderNumber)
                        cmd.Parameters.AddWithValue("@OrderName", orderData.OrderName)
                        cmd.Parameters.AddWithValue("@OrderNote", DBNull.Value)
                        cmd.Parameters.AddWithValue("@OrderType", "Panorama")
                        cmd.Parameters.AddWithValue("@CreatedBy", "699A6B77-BF0C-4583-9DF2-2A16DEC6FC89")
                        cmd.Parameters.AddWithValue("@SubmittedBy", "699A6B77-BF0C-4583-9DF2-2A16DEC6FC89")
                        cmd.Parameters.AddWithValue("@Deposit", Depo)
                        cmd.ExecuteNonQuery()
                    End Using

                    '#OrderDetails
                    If orderData.Details IsNot Nothing AndAlso orderData.Details.Count > 0 Then
                        Dim fieldNameList As New List(Of String) From {
                            "Id", "Number", "HeaderId", "ProductId", "ExactId", "ProductPriceGroupId", "Qty", "Room", "Mounting", "Width", "[Drop]", "TrackLength", "TrackQty", "Layout", "LayoutSpecial", "PanelQty", "CustomHeaderLength", "SemiInsideMount", "BottomTrackType", "BottomTrackRecess", "LouvreSize", "LouvrePosition", "HingeColour", "HingeQtyPerPanel", "PanelQtyWithHinge", "MidrailHeight1", "MidrailHeight2", "MidrailCritical", "FrameType", "FrameLeft", "FrameRight", "FrameTop", "FrameBottom", "Buildout", "BuildoutPosition", "LocationTPost1", "LocationTPost2", "LocationTPost3", "LocationTPost4", "LocationTPost5", "HorizontalTPost", "HorizontalTPostHeight", "JoinedPanels", "TiltrodType", "TiltrodSplit", "SplitHeight1", "SplitHeight2", "ReverseHinged", "PelmetFlat", "ExtraFascia", "HingesLoose", "DoorCutOut", "SpecialShape", "TemplateProvided", "LinearMetre", "SquareMetre", "Notes", "Cost", "CostOverride", "Discount", "FinalCost", "MarkUp", "TotalBlinds", "Production", "Paid", "Active"
                        }

                        Dim propertyNames As New List(Of String)
                        For Each fn In fieldNameList
                            propertyNames.Add(fn.Replace("[", "").Replace("]", ""))
                        Next

                        Dim dynamicColumns As New List(Of String)
                        Dim dynamicPlaceholders As New List(Of String)

                        For Each d In fieldNameList
                            dynamicColumns.Add(d) ' Sudah termasuk bracket bila ada
                            dynamicPlaceholders.Add(String.Format("@{0}", d.Replace("[", "").Replace("]", "")))
                        Next

                        Dim columns As String = String.Join(",", dynamicColumns)
                        Dim placeholders As String = String.Join(",", dynamicPlaceholders)

                        Dim DesignId As String = "0CB7C37F-D478-49BA-94CB-DCDE83FB84C8"


                        Dim defaultValues As New Dictionary(Of String, Object) From {
                            {"Notes", DBNull.Value},
                            {"Cost", 0},
                            {"CostOverride", 0},
                            {"Discount", 0},
                            {"FinalCost", 0},
                            {"MarkUp", 0},
                            {"TotalBlinds", 1},
                            {"Production", "Orion"},
                            {"Paid", 0},
                            {"Active", 1}
                        }

                        Dim ItemId As String = orderCfg.CreateOrderItemId()
                        Dim ItemNumber As String = orderCfg.CreateOrderItemNumber(headerId)
                        Dim processedItems As New List(Of Integer)

                        For Each d In orderData.Details
                            Dim props = d.GetType().GetProperties().ToDictionary(Function(p) p.Name, Function(p) p.GetValue(d, Nothing))

                            Dim BlindName As String = If(IsDBNull(props("BlindName")), String.Empty, props("BlindName").ToString())
                            Dim Colour As String = If(IsDBNull(props("Colour")), String.Empty, props("Colour").ToString())
                            Dim BlindId As String = orderCfg.GetItemData(String.Format("SELECT Id FROM Blinds WHERE DesignId = '{0}' AND Name = '{1}'", DesignId, BlindName))
                            Dim ProductId As String = orderCfg.GetItemData(String.Format("SELECT Id FROM Products WHERE BlindId = '{0}' AND ColourType = '{1}'", BlindId, Colour))

                            Dim exactName As String = String.Format("Panorama PVC Shutters - {0}", BlindName)
                            Dim ExactId As String = orderCfg.GetItemData(String.Format("SELECT ExactId FROM Exacts WHERE Name = '{0}'", exactName))

                            Dim doorCutOut As String = If(IsDBNull(props("DoorCutOut")), String.Empty, props("DoorCutOut").ToString())
                            Dim productPriceGroupName As String = If(doorCutOut = "Yes", "Panorama Sunlight - French Door Cut-Out", "Panorama Sunlight")

                            Dim ProductPriceGroupId = orderCfg.GetProductPriceGroupId(DesignId, productPriceGroupName)

                            Using cmd As New SqlCommand(String.Format("INSERT INTO OrderDetails({0}) VALUES({1})", columns, placeholders), conn, transaction)
                                For Each propName In propertyNames
                                    Dim value As Object = Nothing
                                    If defaultValues.ContainsKey(propName) Then
                                        value = defaultValues(propName)
                                    ElseIf propName = "Id" Then
                                        value = ItemId
                                        ItemId += 1
                                    ElseIf propName = "Number" Then
                                        value = ItemNumber
                                        ItemNumber += 1
                                    ElseIf propName = "HeaderId" Then
                                        value = headerId
                                    ElseIf propName = "ProductId" Then
                                        value = ProductId
                                    ElseIf propName = "ExactId" Then
                                        value = ExactId
                                    ElseIf propName = "ProductPriceGroupId" Then
                                        value = ProductPriceGroupId
                                    ElseIf props.ContainsKey(propName) Then
                                        value = props(propName)
                                    End If
                                    cmd.Parameters.AddWithValue(String.Format("@{0}", propName), If(value IsNot Nothing, value, DBNull.Value))
                                Next
                                cmd.ExecuteNonQuery()
                            End Using

                            Dim findItemId As Integer = ItemId - 1
                            processedItems.Add(findItemId)
                        Next

                        Task.Run(Sub()
                                     Try
                                         For Each findItemId In processedItems
                                             ' --- Proses cost & reset ---
                                             orderCfg.ResetPriceDetail(headerId, findItemId)
                                             Dim cost As Decimal = orderCfg.CountCost(headerId, findItemId)
                                             orderCfg.UpdateCost(findItemId, cost)
                                             orderCfg.UpdateCostOverride(findItemId, cost)
                                             orderCfg.UpdateFinalCost(findItemId)
                                             orderCfg.ResetAuthorization(headerId, findItemId)

                                             ' --- Logging ---
                                             Dim dataLog As Object() = {headerId, findItemId, "699A6B77-BF0C-4583-9DF2-2A16DEC6FC89", "Add Item Order"}
                                             orderCfg.Log_Orders(dataLog)
                                         Next

                                         ' --- Update product type sekali saja ---
                                         orderCfg.UpdateProductType(headerId)

                                     Catch ex As Exception
                                         ' Tangani error agar tidak crash
                                         Console.WriteLine("Async error: " & ex.Message)
                                     End Try
                                 End Sub)


                    End If

                    transaction.Commit()
                    context.Response.StatusCode = 200
                    context.Response.Write("{""status"":""success"",""message"":""Data diterima dan disimpan dengan sukses""}")
                    context.ApplicationInstance.CompleteRequest()

                Catch ex As Exception
                    transaction.Rollback()
                    context.Response.StatusCode = 500
                    context.Response.Write("{""status"":""error"",""message"":""Database operation failed: " & ex.Message.Replace("""", "\""") & """}")
                    context.ApplicationInstance.CompleteRequest()
                End Try
                conn.Close()
            End Using

        Catch ex As Exception
            context.Response.StatusCode = 400
            context.Response.Write("{""status"": ""error"", ""message"": ""Invalid JSON format"", ""details"": """ & ex.Message & """}")
            context.ApplicationInstance.CompleteRequest()
        End Try
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class

<Serializable()>
Public Class OrderData
    Public Property Id As Integer
    Public Property OrderId As String
    Public Property JobId As String
    Public Property CustomerId As String
    Public Property OrderNumber As String
    Public Property OrderName As String
    Public Property OrderNote As String
    Public Property OrderType As String
    Public Property Status As String
    Public Property CreatedBy As String
    Public Property Details As List(Of OrderDetail)
End Class

<Serializable()>
Public Class OrderDetail
    Public Property Id As Integer
    Public Property Number As Integer
    Public Property HeaderId As Integer
    Public Property ProductId As String
    Public Property BlindName As String
    Public Property Colour As String
    Public Property ExactId As String
    Public Property ProductPriceGroupId As String
    Public Property Qty As Integer
    Public Property Room As String
    Public Property Mounting As String
    Public Property Width As Integer
    Public Property Drop As Integer
    Public Property TrackLength As Integer
    Public Property TrackQty As Integer
    Public Property Layout As String
    Public Property LayoutSpecial As String
    Public Property PanelQty As Integer
    Public Property CustomHeaderLength As Integer
    Public Property SemiInsideMount As String
    Public Property BottomTrackType As String
    Public Property BottomTrackRecess As String
    Public Property LouvreSize As String
    Public Property LouvrePosition As String
    Public Property HingeColour As String
    Public Property HingeQtyPerPanel As Integer
    Public Property PanelQtyWithHinge As Integer
    Public Property MidrailHeight1 As Integer
    Public Property MidrailHeight2 As Integer
    Public Property MidrailCritical As String
    Public Property FrameType As String
    Public Property FrameLeft As String
    Public Property FrameRight As String
    Public Property FrameTop As String
    Public Property FrameBottom As String
    Public Property Buildout As String
    Public Property BuildoutPosition As String
    Public Property LocationTPost1 As String
    Public Property LocationTPost2 As String
    Public Property LocationTPost3 As String
    Public Property LocationTPost4 As String
    Public Property LocationTPost5 As String
    Public Property HorizontalTPost As String
    Public Property HorizontalTPostHeight As Integer
    Public Property JoinedPanels As String
    Public Property TiltrodType As String
    Public Property TiltrodSplit As String
    Public Property SplitHeight1 As Integer
    Public Property SplitHeight2 As Integer
    Public Property ReverseHinged As String
    Public Property PelmetFlat As String
    Public Property ExtraFascia As String
    Public Property HingesLoose As String
    Public Property DoorCutOut As String
    Public Property SpecialShape As String
    Public Property TemplateProvided As String
    Public Property LinearMetre As Decimal
    Public Property SquareMetre As Decimal
    Public Property Notes As String
    Public Property Cost As Decimal
    Public Property CostOverride As Decimal
    Public Property Discount As Decimal
    Public Property FinalCost As Decimal
    Public Property MarkUp As Decimal
    Public Property TotalBlinds As Integer
    Public Property Production As String
    Public Property Paid As Integer
    'Public Property Active As Integer
End Class