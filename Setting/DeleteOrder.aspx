<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DeleteOrder.aspx.vb" Inherits="Setting_DeleteOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Status = 'Canceled', StatusDescription = 'Your order was deleted by the system because it has expired.<br /><br />Please inform support@onlineorder.au to restore this order.' WHERE NOT CreatedDate >= DATEADD(MONTH, -3, GETDATE()) AND Status = 'Draft'"></asp:SqlDataSource>
    </form>
</body>
</html>
