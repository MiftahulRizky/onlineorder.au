
Partial Class Order_Create
    Inherits System.Web.UI.Page

    Public Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim HeaderId As String = Request.QueryString("param")
        Dim OrderType As String = Request.QueryString("ordertype")
        Dim Action As String = Request.QueryString("action")
        Dim RoleName As String = Session("RoleName")
        
        ' If RoleName = "Administrator" Then
            Dim Url As String = "/order/orderheaders"
            If Action.ToLower() = "add" Then
                Url = "/order/createorders?action=add"
            End If
            If Action.ToLower() = "edit" Then
                Url = String.Format("/order/createorders?action=edit&param={0}&ordertype={1}", HeaderId, OrderType.ToLower())
            End If
            Response.Redirect(Url, False)
        ' End IF
    End Sub

End Class
