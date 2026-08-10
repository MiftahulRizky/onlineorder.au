
Partial Class Order_Detail
    Inherits System.Web.UI.Page

    Public Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim HeaderId As String = Request.QueryString("param")
        Dim OrderType As String = Request.QueryString("ordertype")
        
        Dim Url = String.Format("/order/orderdetails?param={0}&ordertype={1}", HeaderId, OrderType.ToLower())
        Response.Redirect(Url, False)
    End Sub

End Class
