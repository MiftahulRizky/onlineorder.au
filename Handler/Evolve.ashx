<%@ WebHandler Language="VB" Class="Evolve" %>

Imports System
Imports System.Web
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web.Script.Serialization

Public Class Evolve
    Implements IHttpHandler

    Private ReadOnly myConn As String =
        ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        context.Response.Cache.SetNoStore()

        Dim headerId As String = context.Request.QueryString("id")

        If String.IsNullOrEmpty(headerId) Then
            WriteJson(context, New With {.error = "id is required"})
            Return
        End If

        Try
            Dim result As New Dictionary(Of String, Object)

            Dim header = GetHeader(headerId)
            If header Is Nothing Then
                WriteJson(context, New With {.error = "Header not found"})
                Return
            End If

            result("header") = header
            result("items") = GetItems(headerId)

            WriteJson(context, result)

        Catch ex As Exception
            WriteJson(context, New With {.error = ex.Message})
        End Try
    End Sub

    Private Function GetHeader(headerId As String) As Dictionary(Of String, Object)
        Using con As New SqlConnection(myConn)
            Using cmd As New SqlCommand("SELECT Customers.Name AS CustomerName, OrderHeaders_Shutters.OrderId AS OrderId, OrderHeaders_Shutters.OrderNumber AS OrderNumber, OrderHeaders_Shutters.OrderName AS OrderName, OrderHeaders_Shutters.OrderNote AS OrderNote, OrderHeaders_Shutters.JobDate AS JobDate FROM OrderHeaders_Shutters LEFT JOIN Customers ON OrderHeaders_Shutters.CustomerId=Customers.Id WHERE OrderHeaders_Shutters.Id=@HeaderId AND OrderHeaders_Shutters.Status='In Production' AND OrderHeaders_Shutters.OrderType='Evolve'", con)
                cmd.Parameters.AddWithValue("@HeaderId", headerId)
                con.Open()

                Using dr = cmd.ExecuteReader()
                    If dr.Read() Then
                        Return New Dictionary(Of String, Object) From {
                            {"OrderId", dr("OrderId")},
                            {"OrderNote", dr("OrderNote")},
                            {"OrderDate", Convert.ToDateTime(dr("JobDate")).ToString("yyyy-MM-dd")}
                        }
                    End If
                End Using
            End Using
        End Using

        Return Nothing
    End Function

    Private Function GetItems(headerId As String) As List(Of Dictionary(Of String, Object))
        Dim items As New List(Of Dictionary(Of String, Object))

        Using con As New SqlConnection(myConn)
            Using cmd As New SqlCommand("SELECT OrderDetails_Shutters.Qty, OrderDetails_Shutters.Room, OrderDetails_Shutters.Mounting, OrderDetails_Shutters.Width, OrderDetails_Shutters.[Drop], Blinds.Name AS BlindName, Products.ColourType AS ProductColour, OrderDetails_Shutters.SemiInsideMount, OrderDetails_Shutters.LouvreSize, OrderDetails_Shutters.LouvrePosition, OrderDetails_Shutters.HingeColour, OrderDetails_Shutters.MidrailHeight1, OrderDetails_Shutters.MidrailHeight2, OrderDetails_Shutters.MidrailCritical, OrderDetails_Shutters.Layout, OrderDetails_Shutters.LayoutSpecial, OrderDetails_Shutters.CustomHeaderLength, OrderDetails_Shutters.FrameType, OrderDetails_Shutters.FrameLeft, OrderDetails_Shutters.FrameRight, OrderDetails_Shutters.FrameTop, OrderDetails_Shutters.FrameBottom, OrderDetails_Shutters.BottomTrackType, OrderDetails_Shutters.Buildout, OrderDetails_Shutters.LocationTPost1, OrderDetails_Shutters.LocationTPost2, OrderDetails_Shutters.LocationTPost3, OrderDetails_Shutters.LocationTPost4, OrderDetails_Shutters.LocationTPost5, OrderDetails_Shutters.HorizontalTPost, OrderDetails_Shutters.HorizontalTPostHeight, OrderDetails_Shutters.JoinedPanels, OrderDetails_Shutters.ReverseHinged, OrderDetails_Shutters.PelmetFlat, OrderDetails_Shutters.ExtraFascia, OrderDetails_Shutters.HingesLoose, OrderDetails_Shutters.TiltrodType, OrderDetails_Shutters.TiltrodSplit, OrderDetails_Shutters.SplitHeight1, OrderDetails_Shutters.SplitHeight2, OrderDetails_Shutters.Notes FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails_Shutters.HeaderId=@HeaderId AND OrderDetails_Shutters.Active=1 AND Designs.Type='Evolve'", con)
                cmd.Parameters.AddWithValue("@HeaderId", headerId)
                con.Open()

                Using dr = cmd.ExecuteReader()
                    While dr.Read()
                        Dim thisLayout As String = dr("Layout").ToString()
                        If dr("Layout") = "Other" Then
                            thisLayout = dr("LayoutSpecial").ToString()
                        End If
                    
                        items.Add(New Dictionary(Of String, Object) From {
                            {"Qty", dr("Qty")},
                            {"Location", dr("Room")},
                            {"Mounting", dr("Mounting")},
                            {"SemiInsideMount", dr("SemiInsideMount")},
                            {"Width", dr("Width")},
                            {"Drop", dr("Drop")},
                            {"EvolveType", dr("BlindName")},
                            {"EvolveColour", dr("ProductColour")},
                            {"LouvreSize", dr("LouvreSize")},
                            {"LouvrePosition", dr("LouvrePosition")},
                            {"HingeColour", dr("HingeColour")},
                            {"MidrailHeight1", dr("MidrailHeight1")},
                            {"MidrailHeight2", dr("MidrailHeight2")},
                            {"MidrailCritical", dr("MidrailCritical")},
                            {"Layout", thisLayout},
                            {"CustomHeaderLength", dr("CustomHeaderLength")},
                            {"FrameType", dr("FrameType")},
                            {"FrameLeft", dr("FrameLeft")},
                            {"FrameRight", dr("FrameRight")},
                            {"FrameTop", dr("FrameTop")},
                            {"FrameBottom", dr("FrameBottom")},
                            {"BottomTrackType", dr("BottomTrackType")},
                            {"Buildout", dr("Buildout")},
                            {"GAP1", dr("LocationTPost1")},
                            {"GAP2", dr("LocationTPost2")},
                            {"GAP3", dr("LocationTPost3")},
                            {"GAP4", dr("LocationTPost4")},
                            {"GAP5", dr("LocationTPost5")},
                            {"HorizontalTPost", dr("HorizontalTPost")},
                            {"HorizontalTPostHeight", dr("HorizontalTPostHeight")},
                            {"JoinedPanels", dr("JoinedPanels")},
                            {"ReverseHinged", dr("ReverseHinged")},
                            {"PelmetFlat", dr("PelmetFlat")},
                            {"ExtraFascia", dr("ExtraFascia")},
                            {"HingesLoose", dr("HingesLoose")},
                            {"TiltrodType", dr("TiltrodType")},
                            {"TiltrodSplit", dr("TiltrodSplit")},
                            {"SplitHeight1", dr("SplitHeight1")},
                            {"SplitHeight2", dr("SplitHeight2")},
                            {"Notes", dr("Notes")}
                        })
                    End While
                End Using
            End Using
        End Using

        Return items
    End Function

    Private Sub WriteJson(context As HttpContext, obj As Object)
        Dim serializer As New JavaScriptSerializer()
        context.Response.Write(serializer.Serialize(obj))
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class
