<%@ Page Language="VB" AutoEventWireup="false" CodeFile="JobSheets.aspx.vb" Inherits="jobSheets" Title="Job Sheets" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Job Sheets</title>
    <link rel="icon" type="image/x-icon" href="~/Content/static/new-favicon.png" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <embed runat="server" id="embPrint" type="application/pdf" width="1500" height="900" />
        </div>
    </form>
</body>
</html>
